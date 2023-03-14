using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Aop;
using JP.DataHub.Aop;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ApiWeb.Domain.ApiFilter
{
    // .NET6
    internal static class ApiFilterConfigExtensions
    {
        public static IApiFilter Load(this ApiFilterConfig config, IApiHelper apiHelper, IAopCacheHelper cacheHelper, ITermsHelper termsHelper, string param, ISimpleLogWriter logger)
        {
            if (string.IsNullOrEmpty(config.Assembly) || string.IsNullOrEmpty(config.Type))
            {
                return null;
            }
            Assembly asm = LoadAssembly(config.Assembly);
            if (asm == null)
            {
                return null;
            }
            Type type = asm?.GetType(config.Type);
            if (type != null)
            {
                object obj = Activator.CreateInstance(type);
                if (obj != null && obj is IApiFilter filter)
                {
                    filter.Apihelper = apiHelper;
                    filter.CacheHelper = cacheHelper;
                    filter.TermsHelper = termsHelper;
                    filter.Logger = logger;
                    filter.Param = param;
                    return filter;
                }
            }
            return null;
        }

        private static Assembly LoadAssembly(string path)
        {
            try
            {
                return Assembly.LoadFrom(path);
            }
            catch
            {
            }
            try
            {
                return Assembly.LoadFrom(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path));
            }
            catch
            {
            }
            return null;
        }
    }
}
