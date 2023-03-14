using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Interception.Utilities;

namespace JP.DataHub.MVC.Filters
{
    public class ProfileToHeaderFilter : IResultFilter
    {
        private static Lazy<IConfiguration> _lazyConfiguration = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        private static IConfiguration _configuration { get => _lazyConfiguration.Value; }

        private bool _xProfiler { get; set; }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            bool.TryParse(context.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "X-Profiler").Value, out bool xProfiler);
            _xProfiler = xProfiler;
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // OutputToHeaderの値によって出力するかどうかを決定する
            // always : 無条件に出力する
            // request : RequestヘッダーにX-Profilerがある場合には出力する
            // それ以外 : 出力しない
            var mode = _configuration.GetValue<string>("Profiler:OutputToHeader");
            if (mode == "always" || (mode == "request" && _xProfiler == true))
            {
                var cts = GetCustomTimings();
                foreach (var item in cts)
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        var ct = item.Value[i];
                        context.HttpContext.Response.Headers.Add($"X-Profiler{item.Key.ToPascal()}{i + 1}", $"{ct.DurationMilliseconds}ms {OutputCommand(ct)}");
                    }
                }
            }
        }

        private string OutputCommand(CustomTiming ct)
        {
            var str = ct.CommandString.NoCRLF();
            return str.Length > 200 ? str.Substring(0, 197) + "..." : str;
        }

        private Dictionary<string, List<CustomTiming>> GetCustomTimings()
        {
            var cts = new Dictionary<string, List<CustomTiming>>();
            Action<Timing> createDict = null;
            createDict = t =>
            {
                if (t.HasChildren)
                {
                    t.Children.ForEach(x => createDict(x));
                }
                if (t.HasCustomTimings)
                {
                    t.CustomTimings.ForEach(x =>
                    {
                        if (cts.ContainsKey(x.Key))
                        {
                            cts[x.Key].AddRange(x.Value);
                        }
                        else
                        {
                            cts.Add(x.Key, x.Value);
                        }
                    });
                }
            };
            createDict(MiniProfiler.Current.Root);
            return cts;
        }
    }
}
