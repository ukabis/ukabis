using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.MVC.Misc
{
    public static class RequestHeaderUtil
    {
        private static Lazy<IHttpContextAccessor> _httpContextAccessor = new Lazy<IHttpContextAccessor>(() => UnityCore.Resolve<IHttpContextAccessor>());
        private static IHttpContextAccessor httpContextAccessor => _httpContextAccessor.Value;

        public static T GetHeaderValueAs<T>(string headerName)
        {
            StringValues values = new StringValues();
            if (httpContextAccessor.HttpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.
                if (!rawValues.IsNullOrWhiteSpace())
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }
    }
}
