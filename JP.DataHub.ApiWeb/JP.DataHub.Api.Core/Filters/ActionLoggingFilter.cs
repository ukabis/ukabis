using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.DataContainer;
using JP.DataHub.Api.Core.HttpHeaderConfig;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Service;

namespace JP.DataHub.Api.Core.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ActionLoggingFilter : ActionFilterAttribute, IExceptionFilter
    {
        private static JPDataHubLogger s_logger = new JPDataHubLogger(typeof(ActionLoggingFilter));
        
        private static Lazy<IConfigurationSection> s_lazyAppConfig = new Lazy<IConfigurationSection>(() => UnityCore.Resolve<IConfiguration>().GetSection("AppConfig"));
        private static IConfigurationSection AppConfig => s_lazyAppConfig.Value;
        
        private static Lazy<bool> s_lazyUseLogging = new Lazy<bool>(() => AppConfig.GetValue<bool>("UseLogging", true));
        private static bool UseLogging => s_lazyUseLogging.Value;

        private Lazy<ILoggingFilterService> _lazyLoggingService => new Lazy<ILoggingFilterService>(() => UnityCore.Resolve<ILoggingFilterService>());
        private ILoggingFilterService LoggingService => _lazyLoggingService.Value;

        private Lazy<string> _lazyVendorSystemAuthenticationDefaultVendorId = new Lazy<string>(() => UnityCore.Resolve<string>("VendorSystemAuthenticationDefaultVendorId"));
        private string VendorSystemAuthenticationDefaultVendorId => _lazyVendorSystemAuthenticationDefaultVendorId.Value;
        
        private Lazy<string> _lazyVendorSystemAuthenticationDefaultSystemId = new Lazy<string>(() => UnityCore.Resolve<string>("VendorSystemAuthenticationDefaultSystemId"));
        private string VendorSystemAuthenticationDefaultSystemId => _lazyVendorSystemAuthenticationDefaultSystemId.Value;


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var dataContainer = DataContainerUtil.ResolveDataContainer() as IApiDataContainer;
            dataContainer.Time = DateTime.UtcNow;
            if (context.ActionDescriptor is ControllerActionDescriptor cad)
            {
                dataContainer.ActionName = cad.ActionName;
                dataContainer.ControllerName = cad.ControllerName;
                dataContainer.Argument = cad.Parameters;
                dataContainer.ControllerId = context.Controller.GetType().GetCustomAttributes<ManageApiAttribute>().FirstOrDefault()?.Id;
                dataContainer.ActionId = cad?.MethodInfo?.GetCustomAttributes<ManageActionAttribute>()?.FirstOrDefault()?.Id;
                dataContainer.IsStaticApi = (dataContainer.ControllerId != null);

                var url = new Uri(UriHelper.GetDisplayUrl(context.HttpContext.Request));
                dataContainer.RequestUriScheme = url.Scheme;
                dataContainer.RequestUriAuthority = url.Authority;
            }
        }

        public void OnException(ExceptionContext context)
        {
            if (!UseLogging)
            {
                return;
            }

            RecordLogging(context.HttpContext.Request, context.HttpContext.Response);
        }

        public override void OnResultExecuted(ResultExecutedContext context)
        {
            if (!UseLogging)
            {
                return;
            }

            RecordLogging(context.HttpContext.Request, context.HttpContext.Response);
        }

        private void RecordLogging(HttpRequest request, HttpResponse response)
        {
            var dataContainer = DataContainerUtil.ResolveDataContainer() as IApiDataContainer;
            var endDate = DateTime.UtcNow;
            var executeTime = endDate.Subtract(dataContainer.Time);

            // StaticAPIの場合はLogging情報を送信
            if (dataContainer.IsStaticApi)
            {
                var model = CreateLoggingModel(request, response, executeTime, dataContainer);
                if (model == null)
                {
                    return;
                }

                // StaticAPIでApiiIdとControllerId が紐づけれない場合はメータリング対象外
                if (string.IsNullOrEmpty(model.ApiId) || string.IsNullOrEmpty(model.ControllerId))
                {
                    return;
                }

                // StaticAPIを認証なしで実行された場合、集計できないので共通のベンダーを設定する
                if (string.IsNullOrEmpty(model?.VendorId) && string.IsNullOrEmpty(model?.SystemId))
                {
                    model.VendorId = VendorSystemAuthenticationDefaultVendorId;
                    model.SystemId = VendorSystemAuthenticationDefaultSystemId;
                }

                // レスポンス情報をセット
                dataContainer.LoggingIdUrlList.TryAdd(model.LogId, model.Url);

                // 情報登録
                LoggingService.Write(model);
                s_logger.Info($"logging sent logid={model.LogId}");
            }

            // ヘッダーにX-GetInternalAllFieldが設定されている場合、LogIdを返す
            if (request.Headers.ContainsKey("X-GetInternalAllField"))
            {
                response.Headers.Add("LoggingLogId", JsonConvert.SerializeObject(dataContainer.LoggingIdUrlList));
            }

            s_logger.Info($"Action End{executeTime}");
        }

        private ApiRequestResponseLogModel CreateLoggingModel(HttpRequest request, HttpResponse response, TimeSpan executeTime, IApiDataContainer dataContainer)
        {
            var httpHeaderSiplit = new HttpHeaderSplit(UnityCore.Resolve<string>("LoggingHttpHeaders"));
            var result = new ApiRequestResponseLogModel()
            {
                LogId = Guid.NewGuid().ToString(),
                HttpStatusCode = (HttpStatusCode)response.StatusCode,
                RequestDate = dataContainer.Time,
                ExecuteTime = executeTime,
                ActionName = dataContainer.ActionName,
                ControllerName = dataContainer.ControllerName,
                ControllerId = dataContainer.ControllerId,
                ApiId = dataContainer.ActionId,
                ClientIpAddress = dataContainer.ClientIpAddress,
                HttpMethodType = request.Method,
                OpenId = dataContainer.OpenId,
                QueryString = request.QueryString.Value,
                RequestContentLength = request.Headers.ContentLength ?? 0L,
                RequestContentType = request.ContentType,
                RequestHeaders = httpHeaderSiplit.FilterHeader(request.Headers),
                Url = UriHelper.GetDisplayUrl(request),
                SystemId = dataContainer.SystemId,
                VendorId = dataContainer.VendorId,
                IsInternalCall = false
            };

            // Exception発生時に以下のコードを成功しないため
            try
            {
                result.ResponseContentLength = response.Headers.ContentLength ?? response.Body.Length;
            }
            catch
            {
            }
            result.ResponseContentType = response.ContentType;
            result.ResponseHeaders = httpHeaderSiplit.FilterHeader(response.Headers);

            return result;
        }
    }
}
