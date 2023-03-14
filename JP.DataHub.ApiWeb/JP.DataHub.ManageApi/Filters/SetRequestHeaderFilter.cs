using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using JP.DataHub.Com.Authentication;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Misc;
using JP.DataHub.Api.Core.HttpHeaderConfig;
using JP.DataHub.ManageApi.Core.DataContainer;

namespace JP.DataHub.ManageApi.Filters
{
    public class SetRequestHeaderFilter : ActionFilterAttribute
    {
        private static Lazy<string[]> s_lazySupportedCultures = new(() => UnityCore.Resolve<string[]>("SupportedCultures"));
        private static string[] s_supportedCultures = s_lazySupportedCultures.Value;

        private Lazy<IHttpContextAccessor> _lazyHttpContextAccessor = new(() => UnityCore.Resolve<IHttpContextAccessor>());
        private IHttpContextAccessor _httpContextAccessor => _lazyHttpContextAccessor.Value;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var dataContainer = UnityCore.Resolve<IPerRequestDataContainer>();

            HttpHeaderSplit HttpHeaderSplit = new HttpHeaderSplit(UnityCore.Resolve<string>("LoggingHttpHeaders"));
            dataContainer.RequestHeaders = HttpHeaderSplit.FilterHeader(_httpContextAccessor.HttpContext.Request.Headers);

            string xAcceptLanguage = null;
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Accept-Language", out var langs))
            {
                xAcceptLanguage = langs.FirstOrDefault();
            }

            //ヘッダーに指定があるならそれに従う 無いならConfig
            dataContainer.CultureInfo = !string.IsNullOrEmpty(xAcceptLanguage) && s_supportedCultures.Contains(xAcceptLanguage)
                    ? new CultureInfo(xAcceptLanguage)
                    : new CultureInfo(s_supportedCultures[0]);
        }
    }
}
