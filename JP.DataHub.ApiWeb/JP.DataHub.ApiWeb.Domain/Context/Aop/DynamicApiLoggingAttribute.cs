using System;
using Microsoft.Extensions.Configuration;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.HttpHeaderConfig;
using JP.DataHub.Api.Core.Service;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Aop
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DynamicApiLoggingAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new DynamicApiLoggingHandler();
        }

        //[DebuggerStepThrough]
        public class DynamicApiLoggingHandler : ICallHandler
        {
            private static Lazy<IConfigurationSection> s_lazyAppConfig = new Lazy<IConfigurationSection>(() => UnityCore.Resolve<IConfiguration>().GetSection("AppConfig"));
            private static IConfigurationSection AppConfig => s_lazyAppConfig.Value;

            private static Lazy<HttpHeaderSplit> s_httpHeaderSplit = new Lazy<HttpHeaderSplit>(() => new HttpHeaderSplit(UnityCore.Resolve<string>("LoggingHttpHeaders")));
            private static HttpHeaderSplit HttpHeaderSplit => s_httpHeaderSplit.Value;

            private static Lazy<bool> s_lazyUseLogging = new Lazy<bool>(() => AppConfig.GetValue<bool>("UseLogging", true));
            private static bool UseLogging => s_lazyUseLogging.Value;

            public int Order
            {
                get { return 1; }
                set { }
            }

            private JPDataHubLogger _logger = new JPDataHubLogger(typeof(DynamicApiLoggingHandler));


            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                IMethodReturn result = null;
                try
                {
                    var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();

                    // Logging無効の場合は何もしない
                    if (!UseLogging)
                    {
                        return getNext()(input, getNext);
                    }

                    // Request実行
                    var startTimestamp = DateTime.UtcNow;
                    result = getNext()(input, getNext);

                    // Logging情報作成
                    var logId = (input.Arguments["actionId"] as ActionId).Value;
                    var endTimestamp = DateTime.UtcNow;
                    var executeTime = endTimestamp.Subtract(startTimestamp);
                    var request = input.Target as IMethod;
                    var requestContentsType = input.Arguments["mediaType"] as MediaType;
                    var response = (HttpResponseMessage)result.ReturnValue;

                    var model = new ApiRequestResponseLogModel();
                    model.LogId = logId;
                    model.RequestDate = startTimestamp;
                    model.ExecuteTime = executeTime;
                    model.ClientIpAddress = perRequestDataContainer.ClientIpAddress;
                    model.VendorId = perRequestDataContainer.VendorId;
                    model.SystemId = perRequestDataContainer.SystemId;
                    model.OpenId = perRequestDataContainer.OpenId;
                    model.IsInternalCall = perRequestDataContainer.IsInternalCall;

                    model.ControllerId = request?.ControllerId?.Value;
                    model.ControllerName = perRequestDataContainer.ControllerName;
                    model.ApiId = request?.ApiId?.Value;
                    model.ActionName = perRequestDataContainer.ActionName;
                    model.HttpMethodType = request.MethodType.Value.ToString().ToUpper();
                    model.QueryString = request?.Query?.OriginalQueryString ?? "";
                    model.Url = $"{perRequestDataContainer.RequestUriScheme}://{perRequestDataContainer.RequestUriAuthority}{request.RelativeUri.Value}{model.QueryString}";

                    if (perRequestDataContainer.RequestHeaders.TryGetValue("Content-Length", out var tmp) &&
                        long.TryParse(tmp?.FirstOrDefault(), out var requestContentLength))
                    {
                        model.RequestContentLength = requestContentLength;
                    }
                    model.RequestContentType = requestContentsType?.Value;
                    model.RequestHeaders = HttpHeaderSplit.FilterHeader(perRequestDataContainer.RequestHeaders);

                    model.HttpStatusCode = response.StatusCode;
                    model.ResponseContentLength = response.Content.Headers.ContentLength ?? 0;
                    model.ResponseContentType = response.Content.Headers.ContentType?.MediaType;
                    model.ResponseHeaders = GetHeaders(response);

                    // レスポンスで返却されるLogIdにリストに追加
                    perRequestDataContainer.LoggingIdUrlList.TryAdd(logId, model.Url);

                    // Logging送信
                    UnityCore.Resolve<ILoggingFilterService>().Write(model);
                    _logger.Info($"logging sent logid={logId}");
                }
                catch (Exception e)
                {
                    new JPDataHubLogger(input.MethodBase.DeclaringType.FullName).Error(e);
                    throw;
                }
                return result;
            }

            private Dictionary<string, List<string>> GetHeaders(HttpResponseMessage response)
            {
                var responseHeaders = HttpHeaderSplit.FilterHeader(response.Headers);
                foreach (var header in HttpHeaderSplit.FilterHeader(response.Content.Headers))
                {
                    responseHeaders.TryAdd(header.Key, header.Value);
                }

                return responseHeaders;
            }

        }
    }
}
