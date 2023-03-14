using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using JP.DataHub.Com.Log;
using JP.DataHub.ApiWeb.Domain.Scripting;

namespace JP.DataHub.ApiWeb.Infrastructure.Scripting.Roslyn
{
    internal class CSharpScriptingExecuter : IScriptingExecuter
    {
        private static readonly JPDataHubLogger s_logger = new JPDataHubLogger(typeof(CSharpScriptingExecuter));
        private static string[] s_disableNameSpace = { "System.IO", "System.Reflection", "System.Diagnostics" };

        private static List<string> DisableclassNames => s_disableclassNames.Value;
        private static Lazy<List<string>> s_disableclassNames = new Lazy<List<string>>(() =>
        {
            var ret = new List<string>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var classnames = assembly.GetTypes().Where(x => s_disableNameSpace.Where(dn => dn == x.Namespace).Any() && x.IsClass && x.IsPublic).Select(x => x.Name);
                    if (classnames.Any())
                    {
                        ret.AddRange(classnames);
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // Cshap.CodeAnalyTics 関連のアセンブリを読み込もうとするとExcptionが発生するので握りつぶす
                }
            }
            return ret;
        });

        private MemoryCache _scriptCache = MemoryCache.Default;
        private ScriptOptions _option = ScriptOptions.Default.WithImports("System.Net", "Newtonsoft.Json", "Newtonsoft.Json.Linq", "JP.DataHub.ApiWeb.Domain.Scripting.Roslyn", "Unity", "JP.DataHub.ApiWeb.Infrastructure").WithReferences(Assembly.Load("Newtonsoft.Json"), Assembly.Load("JP.DataHub.ApiWeb.Domain"), Assembly.Load("Unity.Interception"), Assembly.Load("JP.DataHub.ApiWeb.Infrastructure"), Assembly.Load("System.Net.Http"), Assembly.Load("AutoMapper")).WithEmitDebugInformation(true);
        private string[] _prohibitedClass = { };
        private string _scriptCode = null;


        public string Compile<T>(string scriptCode, object parameters, bool isEnableScriptRuntimeLogException = false)
        {
            ScriptCodeCheck(scriptCode);
            if (isEnableScriptRuntimeLogException)
            {
                scriptCode = InjectTryCatchClause(scriptCode);
            }
            if (_scriptCache.Get(scriptCode) != null)
            {
                _scriptCode = scriptCode;
                return "";
            }
            var script = Create<T>(scriptCode, parameters);
            return Compile<T>(script);
        }

        /// <summary>
        /// スクリプトにtry-catch節を注入する
        /// このtry-catchは実行時例外をcatchし、カスタム例外にラップしてthrowする。
        /// </summary>
        /// <param name="scriptCode">RoslynScriptコード</param>
        /// <returns>try-catch注入後のコード</returns>
        private static string InjectTryCatchClause(string scriptCode)
        {
            //tryの付加
            var syntaxtrees = CSharpSyntaxTree.ParseText(scriptCode);
            var usings = syntaxtrees.GetRoot().ChildNodes().Where(n => n.Kind() == SyntaxKind.UsingDirective);
            scriptCode = scriptCode.Insert(usings.Any() ? usings.Last().Span.End : 0, Environment.NewLine + "try{ " + Environment.NewLine);

            //catchの付加
            scriptCode += Environment.NewLine + @"}catch(System.Exception e){ throw new JP.DataHub.Api.Core.Exceptions.RoslynScriptRuntimeException(""RuntimeException Occured."",e) ; }";
            return scriptCode;
        }

        public T ExecuteScript<T>(object parameters, string scriptCode = null, bool isEnableCatchException = false)
        {
            // scriptCodeが未指定の場合はCompile実行時のscriptCodeを使用
            if (string.IsNullOrEmpty(scriptCode)) scriptCode = _scriptCode;

            ScriptCodeCheck(scriptCode);

            if (isEnableCatchException)
            {
                scriptCode = InjectTryCatchClause(scriptCode);
            }
            var compiledScript = (ScriptRunner<T>)_scriptCache.Get(scriptCode);
            if (compiledScript != null)
            {
                try
                {
                    return compiledScript.Invoke(parameters).Result;
                }
                catch (AggregateException ae)
                {
                    StringBuilder sb = new StringBuilder();
                    ae.InnerExceptions?.ToList()?.ForEach(x => sb.Append(x));
                    s_logger.Error($"RoslynScriptError AggregateException={sb}");
                    throw;
                }
                catch (Exception e)
                {
                    s_logger.Error($"RoslynScriptError Exception={e}");
                    throw;
                }
            }
            var script = Create<T>(scriptCode, parameters);

            return script.RunAsync(parameters).Result.ReturnValue;
        }

        private void ChildNode(IEnumerable<Microsoft.CodeAnalysis.SyntaxNode> nodes, List<string> errors)
        {
            foreach (CSharpSyntaxNode child in nodes)
            {
                if (child.Kind() == SyntaxKind.UsingDirective)
                {
                    continue;
                }
                if (child.Kind() == SyntaxKind.ObjectCreationExpression || child.Kind() == SyntaxKind.SimpleMemberAccessExpression || child.Kind() == SyntaxKind.QualifiedName)
                {
                    var msg = CheckDisableClass(child);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        errors.Add(msg);
                    }
                    continue;
                }
                if (child.ChildNodes().Any())
                {
                    ChildNode(child.ChildNodes(), errors);
                }
            }
        }

        private string CheckDisableClass(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode node)
        {
            var text = node.GetText();
            var location = node.GetLocation().GetMappedLineSpan();
            foreach (var classname in DisableclassNames)
            {
                if (text.ToString().Contains(classname))
                {
                    // 前後どちらかにアルファベットか数字がある場合は、禁止クラスではない扱いにする
                    if (!System.Text.RegularExpressions.Regex.IsMatch(text.ToString(), "[0-9a-zA-Z]+" + classname) &&
                        !System.Text.RegularExpressions.Regex.IsMatch(text.ToString(), classname + "[0-9a-zA-Z]+"))
                    {
                        // Roslyn 上　-1の行、桁が返って来てしまうので暫定で+1
                        return $"({location.StartLinePosition.Line + 1},{location.StartLinePosition.Character})  {classname} not Use ";
                    }
                }
            }
            return "";
        }

        private void ScriptCodeCheck(string scriptCode)
        {
            if (string.IsNullOrEmpty(scriptCode)) throw new Exception("ScriptCode is null or Empty");
        }

        private Script<T> Create<T>(string scriptCode, object parameters)
            => CSharpScript.Create<T>(scriptCode, _option, parameters.GetType());

        private string Compile<T>(Script<T> script)
        {
            var ret = string.Join("\r\n", script.Compile().Select(x => $"{ x.ToString()}"));
            if (string.IsNullOrEmpty(ret))
            {
                var syntaxTrees = script.GetCompilation().SyntaxTrees;
                foreach (var syntaxTree in syntaxTrees)
                {
                    var classErrors = new List<string>();
                    ChildNode(syntaxTree.GetRoot().ChildNodes(), classErrors);
                    ret = string.Join("\r\n", classErrors.Select(x => $"{x}"));
                }
            }

            if (string.IsNullOrEmpty(ret))
            {
                var cacheItemPolicy = new CacheItemPolicy();
                _scriptCache.AddOrGetExisting(script.Code, script.CreateDelegate(), DateTime.Now.Add(TimeSpan.FromMinutes(10)));
                _scriptCode = script.Code;
            }

            return ret;
        }
    }
}
