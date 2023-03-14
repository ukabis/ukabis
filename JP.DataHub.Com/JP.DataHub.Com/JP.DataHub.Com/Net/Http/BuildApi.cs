using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Interception.PolicyInjection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using JP.DataHub.Com.Net.Http.Attributes;

namespace JP.DataHub.Com.Net.Http
{
    public static class BuildApi
    {
        private class BuildupApiResult
        {
            public Type From { get; set; }
            public Type To { get; set; }
        }

        /// <summary>
        /// API定義のinterfaceからAPI定義のコードを自動生成し、Unityに登録する
        /// 対象は呼び出し元のアセンブリ
        /// </summary>
        /// <param name="container"></param>
        public static void BuildupApiDifinition(this IUnityContainer container)
        {
            var caller = new StackFrame(1);
            BuildupApiDifinition(container, caller.GetMethod().ReflectedType.Assembly);
        }

        /// <summary>
        /// API定義のinterfaceからAPI定義のコードを自動生成し、Unityに登録する
        /// 対象はtargetを定義しているアセンブリ
        /// </summary>
        /// <param name="container"></param>
        public static void BuildupApiDifinition(this IUnityContainer container, object target)
        {
            BuildupApiDifinition(container, target.GetType().Assembly);
        }

        /// <summary>
        /// API定義のinterfaceからAPI定義のコードを自動生成し、Unityに登録する
        /// 対象はassemblyで宣言しているinterface
        /// </summary>
        /// <param name="container"></param>
        public static void BuildupApiDifinition(this IUnityContainer container, Assembly assembly)
        {
            var alltypes = assembly.GetTypes().ToList();
            var mynamespace = typeof(ICommonResource<object>).Namespace;
            var result = new List<BuildupApiResult>();
            var types = alltypes.Where(x => x.IsClass == false && x.GetCustomAttribute<WebApiResourceAttribute>() != null).ToList();
            foreach (var type in types)
            {
                var attrresource = type.GetCustomAttribute<WebApiResourceAttribute>();
                // 実装しているクラスがある場合は自動生成しない
                var impl = FindImplClass(alltypes, type);
                if (impl != null)
                {
                    if (container.IsRegistered(type) == false)
                    {
                        container.RegisterType(type, impl, new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
                    }
                    continue;
                }
                if (attrresource.Model == null)
                {
                    throw new Exception($"{type.Name}のWebApiResource属性にモデルが定義されていません");
                }

                StringBuilder sb = new StringBuilder();
                var namespaces = new List<string>() { type.Namespace, mynamespace, "JP.DataHub.Com.Web.WebRequest", "JP.DataHub.Com.Net.Http.Models", "System.Collections.Generic", "System", "System.IO" };
                if (namespaces.Contains(attrresource.Model.Namespace) == false)
                {
                    namespaces.Add(attrresource.Model.Namespace);
                }
                namespaces.ForEach(x => sb.AppendLine($"using {x};"));
                sb.AppendLine($"public class {type.Name.Substring(1)} : CommonResource<{attrresource.Model.Name}>, {type.Name}");
                sb.AppendLine($"{{");
                foreach (var prop in type.GetProperties())
                {
                    sb.AppendLine($"public string {prop.Name} {{ get; }}");
                }
                sb.AppendLine($"public {type.Name.Substring(1)}() : base() {{ }}");
                sb.AppendLine($"public {type.Name.Substring(1)}(string serverUrl) : base(serverUrl) {{ }}");
                sb.AppendLine($"public {type.Name.Substring(1)}(IServerEnvironment serverEnvironment) : base(serverEnvironment) {{ }}");
                foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
                {
                    sb.AppendLine($"public {GetMethodName(method)}");
                }
                sb.AppendLine($"}}");
                sb.AppendLine($"return typeof({type.Name.Substring(1)});");

                var sourceCode = sb.ToString();
                var tmp = "";
                var script = Create<Type>(sourceCode, tmp, assembly.ManifestModule.Name.Replace(".dll", ""));
                var x = script.RunAsync(tmp, (exception) => false).Result;
                result.Add(new BuildupApiResult() { From = type, To = x.ReturnValue });
                GC.Collect();
            }

            foreach (var type in result)
            {
                container.RegisterType(type.From, type.To, new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
            }
            GC.Collect();
        }

        private static string GetMethodName(MethodInfo method)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{method.ReturnParameter.ParameterType.GetCSharpCode()} {method.Name}({method.GetParameters().GetCSharpCode()}) => null;");
            return sb.ToString();
        }

        private static Regex _nullableMatch = new Regex("^System.Nullable`1\\[(?<type>.*?)\\]");

        private static string GetCSharpCode(this ParameterInfo[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in parameters)
            {
                if (sb.ToString().Length != 0 && sb.ToString().EndsWith(",") == false)
                {
                    sb.Append(",");
                }
                var m = _nullableMatch.Match(p.ToString());
                if (m.Success == true)
                {
                    sb.Append($"{m.Groups[1]}? {p.Name}");
                }
                else
                {
                    sb.Append($"{p.ParameterType.GetCSharpCode()} {p.Name}");
                }
            }
            return sb.ToString();
        }

        private static string GetCSharpCode(this Type type)
        {
            if (type.IsGenericType == false)
            {
                if (type == typeof(string))
                {
                    return "string";
                }
                return type.Name;
            }

            StringBuilder sb = new StringBuilder();
            if (type.Name == "List`1")
            {
                sb.Append($"List");
            }
            else
            {
                sb.Append($"{type.BaseType.FullName}");
            }
            if (type.IsGenericType == true)
            {
                sb.Append($"<");
                foreach (var t in type.GenericTypeArguments)
                {
                    if (sb.ToString().EndsWith("<") == false) sb.Append($",");
                    string name = t.GetCSharpCode();
                    sb.Append($"{name}");
                }
                sb.Append($">");
            }
            return sb.ToString();
        }

        private static Type FindImplClass(List<Type> all, Type Interface)
        {
            var implName = Interface.Name.Substring(1);
            return all.FirstOrDefault(x => x.Name == implName);
        }

        private static Script<T> Create<T>(string scriptCode, object parameters, string module)
        {
            var mymodule = typeof(ICommonResource<object>).Assembly.ManifestModule.Name.Replace(".dll", "");
            var imports = new List<string>() { "System.Net", "Newtonsoft.Json", "Newtonsoft.Json.Linq", mymodule, module };
            var loads = new List<string>() { "Newtonsoft.Json", mymodule, module };
            var asms = loads.Select(x => Assembly.Load(x)).ToList();
            var option = ScriptOptions.Default.WithImports(imports).WithReferences(asms).WithEmitDebugInformation(true);
            return CSharpScript.Create<T>(scriptCode, option, parameters.GetType());
        }
    }
}
