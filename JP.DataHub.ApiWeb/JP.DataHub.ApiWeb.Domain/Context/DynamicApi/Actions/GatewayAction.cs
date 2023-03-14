using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class GatewayAction : AbstractDynamicApiAction, IGatewayAction
    {
        private static readonly JPDataHubLogger s_logger = new JPDataHubLogger(typeof(GatewayAction));
        private IDynamicGatewayRepository _gatewayRepository => UnityCore.Resolve<IDynamicGatewayRepository>();

        public GatewayInfo GatewayInfo { get; set; }

        protected override string GetCacheKey(INewDynamicApiDataStoreRepository repository, QueryParam queryParam) => _gatewayRepository.CreateCacheKey(this);


        public override HttpResponseMessage ExecuteAction()
        {
            GatewayResponse result;
            string keyCache = CreateCacheKey(null, null);
            bool isCache = (keyCache != null);
            if (isCache && Cache.Contains(keyCache) == true)
            {
                var cache = Cache.Get<GatewayResponse>(keyCache, out bool isNullValue, true);
                if (cache == null)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10423, this.RelativeUri?.Value);
                }
                else
                {
                    var cachedMessage = cache.Message;
                    cachedMessage.Headers.Add("X-Cache", $"HIT key:{keyCache}");
                    return cachedMessage;
                }
            }
            else
            {
                RequestGatewayUrl url;
                try
                {
                    url = new RequestGatewayUrl(new GatewayUri(GatewayInfo.Url), this.Query, this.KeyValue, this.ApiUri);
                }
                catch (Exception ex)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10424, this.RelativeUri?.Value, title: null, detail: ex.Message);
                }
                try
                {
                    switch (MethodType.Value)
                    {
                        case HttpMethodType.MethodTypeEnum.PUT:
                            result = _gatewayRepository.Put(url, GatewayInfo, Contents, isCache);
                            break;
                        case HttpMethodType.MethodTypeEnum.DELETE:
                            result = _gatewayRepository.Delete(url, GatewayInfo, Contents, isCache);
                            break;
                        case HttpMethodType.MethodTypeEnum.GET:
                            result = _gatewayRepository.Get(url, GatewayInfo, Contents, isCache);
                            break;
                        case HttpMethodType.MethodTypeEnum.POST:
                            result = _gatewayRepository.Post(url, GatewayInfo, Contents, isCache);
                            break;
                        case HttpMethodType.MethodTypeEnum.PATCH:
                            result = _gatewayRepository.Patch(url, GatewayInfo, Contents, isCache);
                            break;
                        default:
                            return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10425, this.RelativeUri?.Value);
                    }
                }
                //中継先でエラーになった場合はログを出力する
                catch (HttpRequestException hre)
                {
                    s_logger.Warn($"GatewayActionResponseException HttpRequestException={hre.Message}");
                    throw;
                }
                catch (TaskCanceledException tce)
                {
                    s_logger.Warn($"GatewayActionResponseException TaskCanceledException={tce.Message}");
                    throw;
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.InnerExceptions)
                    {
                        if (e.GetType() == typeof(HttpRequestException))
                        {
                            s_logger.Warn($"GatewayActionResponseException HttpRequestException={e.Message}");
                        }
                        if (e.GetType() == typeof(TaskCanceledException))
                        {
                            s_logger.Warn($"GatewayActionResponseException TaskCanceledException={e.Message}");
                        }
                    }
                    throw;
                }
            }

            if (isCache && result.IsSaveCache())
            {
                Cache.Add(keyCache, result, CacheInfo.CacheSecond);
            }
            s_logger.Info($"GatewayActionResponse StatusCode={(int)result.Message.StatusCode}");
            return result.Message;
        }
    }
}
