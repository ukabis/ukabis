using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.HttpHeaderConfig;
using JP.DataHub.ApiWeb.Core.DataContainer;
using System.Collections.Generic;

namespace JP.DataHub.ApiWeb.Filters
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

            // 言語
            string xAcceptLanguage = null;
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Accept-Language", out var langs))
            {
                xAcceptLanguage = langs.FirstOrDefault();
            }
            dataContainer.CultureInfo = !string.IsNullOrEmpty(xAcceptLanguage) && s_supportedCultures.Contains(xAcceptLanguage)
                    ? new CultureInfo(xAcceptLanguage)
                    : new CultureInfo(s_supportedCultures[0]);

            // データ共有
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-ResourceSharingWith", out var sharingInfo) && !string.IsNullOrEmpty(sharingInfo))
            {
                try
                {
                    var sharingInfoSets = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(sharingInfo);

                    // もしヘッダーに任意の項目がなければエラーにする
                    if (!sharingInfoSets.ContainsKey("VendorId") || !sharingInfoSets.ContainsKey("SystemId") ||
                        !Guid.TryParse(sharingInfoSets["VendorId"], out Guid tempSharingVendorId) ||
                        !Guid.TryParse(sharingInfoSets["SystemId"], out Guid tempSharingSystemId))
                    {
                        context.Result = new BadRequestObjectResult("X-ResourceSharingWith invalid format.");
                        return;
                    }
                    dataContainer.XResourceSharingWith = sharingInfoSets;
                }
                catch
                {
                    context.Result = new BadRequestObjectResult("X-ResourceSharingWith invalid format.");
                    return;
                }
            }

            // 個人データ共有(バリデーションのみ)
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-ResourceSharingPerson", out var sharingUserId))
            {
                if (string.IsNullOrEmpty(sharingUserId))
                {
                    context.Result = new BadRequestObjectResult("X-ResourceSharingPerson is empty.");
                    return;
                }
                else if (!Guid.TryParse(sharingUserId, out _))
                {
                    context.Result = new BadRequestObjectResult("X-ResourceSharingPerson invalid format.");
                    return;
                }
            }
            // 認可
            if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-UserResourceSharing", out var userResourceSharing) && !string.IsNullOrEmpty(userResourceSharing))
            {
                try
                {
                    if (userResourceSharing.ToString().ToUpper() == "ALL")
                    {
                        dataContainer.XUserResourceSharing = new List<string>() { userResourceSharing.ToString().ToUpper() };
                    }
                    else
                    {
                        var userResourceSharingSets = JsonConvert.DeserializeObject<List<string>>(userResourceSharing);
                        userResourceSharingSets.ForEach(x =>
                        {
                            if (!Guid.TryParse(x, out Guid tempOpenId))
                            {
                                context.Result = new BadRequestObjectResult("X-UserResourceSharing invalid format.");
                                return;
                            }
                        });
                        dataContainer.XUserResourceSharing = userResourceSharingSets;
                    }
                }
                catch
                {
                    context.Result = new BadRequestObjectResult("X-UserResourceSharing invalid format.");
                    return;
                }
            }
        }
    }
}
