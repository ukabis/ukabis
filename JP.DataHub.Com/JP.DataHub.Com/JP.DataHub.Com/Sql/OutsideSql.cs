using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Interception.PolicyInjection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.SQL.Attributes;

namespace JP.DataHub.Com.SQL
{
    public static class OutsideSql
    {
        private class BuildupOutsideSqlResult
        {
            public Type From { get; set; }
            public Type To { get; set; }
        }

        /// <summary>
        /// 外だしSQLのinterface定義から実装クラスを自動生成しUnityに登録する
        /// 対象は呼び出し元のアセンブリ
        /// </summary>
        /// <param name="container"></param>
        public static void BuildupOutsideSql(this IUnityContainer container)
        {
            var caller = new StackFrame(1);
            BuildupOutsideSql(container, caller.GetMethod().ReflectedType.Assembly);
        }

        /// <summary>
        /// 外だしSQLのinterface定義から実装クラスを自動生成しUnityに登録する
        /// 対象はtargetを定義しているアセンブリ
        /// </summary>
        /// <param name="container"></param>
        /// <param name="target"></param>
        public static void BuildupOutsideSql(this IUnityContainer container, object target)
        {
            BuildupOutsideSql(container, target.GetType().Assembly);
        }

        /// <summary>
        /// 外だしSQLのinterface定義から実装クラスを自動生成しUnityに登録する
        /// 対象は引数で指定したアセンブリ
        /// </summary>
        /// <param name="container"></param>
        /// <param name="assembly"></param>
        public static void BuildupOutsideSql(this IUnityContainer container, Assembly assembly)
        {
            var alltypes = assembly.GetTypes().ToList();
            var mynamespace = typeof(TwowaySqlParser).Namespace;
            var result = new List<BuildupOutsideSqlResult>();
            var types = alltypes.Where(x => x.IsClass == false && x.GetCustomAttribute<OutsideSqlAttribute>() != null).ToList();
            foreach (var type in types)
            {
                // 実装しているクラスがある場合は自動生成しない
                if (FindImplClass(alltypes, type) != null)
                {
                    continue;
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"using {type.Namespace};");
                sb.AppendLine($"using {mynamespace};");
                sb.AppendLine($"public class {type.Name.Substring(1)} : AbstractOutsideSQL, {type.Name}");
                sb.AppendLine($"{{");
                foreach (var prop in type.GetProperties())
                {
                    sb.AppendLine($"public string {prop.Name} {{ get; }}");
                }
                sb.AppendLine($"}}");
                sb.AppendLine($"return typeof({type.Name.Substring(1)});");

                var sourceCode = sb.ToString();
                var tmp = "";
                var script = Create<Type>(sourceCode, tmp, assembly.ManifestModule.Name.Replace(".dll", ""));
                var x = script.RunAsync(tmp, (exception) => false).Result;
                result.Add(new BuildupOutsideSqlResult() { From = type, To = x.ReturnValue });
                GC.Collect();
            }

            foreach (var type in result)
            {
                container.RegisterType(type.From, type.To, new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
                object obj = container.Resolve(type.From);
                container.RegisterInstance(type.From, obj);
            }
            GC.Collect();
        }

        private static Type FindImplClass(List<Type> all, Type Interface)
        {
            var implName = Interface.Name.Substring(1);
            return all.FirstOrDefault(x => x.Name == implName);
        }

        private static Script<T> Create<T>(string scriptCode, object parameters, string module)
        {
            var mymodule = typeof(TwowaySqlParser).Assembly.ManifestModule.Name.Replace(".dll", "");
            var imports = new List<string>() { "System.Net", "Newtonsoft.Json", "Newtonsoft.Json.Linq", mymodule, module };
            var loads = new List<string>() { "Newtonsoft.Json", mymodule, module };
            var asms = loads.Select(x => Assembly.Load(x)).ToList();
            var option = ScriptOptions.Default.WithImports(imports).WithReferences(asms).WithEmitDebugInformation(true);
            return CSharpScript.Create<T>(scriptCode, option, parameters.GetType());
        }
    }
}
