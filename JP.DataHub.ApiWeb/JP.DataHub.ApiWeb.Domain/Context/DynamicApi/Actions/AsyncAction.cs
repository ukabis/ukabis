using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class AsyncAction : AbstractDynamicApiAction, IEntity
    {
        private Lazy<IAsyncDyanamicApiRepository> _lazyAsyncDyanamicApiRepository = new Lazy<IAsyncDyanamicApiRepository>(() => UnityCore.Resolve<IAsyncDyanamicApiRepository>());
        private IAsyncDyanamicApiRepository AsyncDyanamicApiRepository => _lazyAsyncDyanamicApiRepository.Value;

        public override HttpResponseMessage ExecuteAction()
        {
            //EntityのValueオブジェクトから非同期APIのモデルを作成するが業務ロジック
            var registData = new JObject();
            var requestId = new AsyncRequestId(Guid.NewGuid().ToString());

            var container = UnityCore.Resolve<IPerRequestDataContainer>();
            var perRequestContainner = new PerRequestDataContainer();
            perRequestContainner.AccessBeyondVendorKey = container.AccessBeyondVendorKey;
            perRequestContainner.ClientIpAddress = container.ClientIpAddress;
            perRequestContainner.OpenId = this.OpenId?.Value;
            perRequestContainner.Claims = container.Claims;
            perRequestContainner.IsDeveloper = container.IsDeveloper;
            perRequestContainner.VendorSystemAuthenticated = container.VendorSystemAuthenticated;
            perRequestContainner.SystemId = this.SystemId?.Value;
            perRequestContainner.VendorId = this.VendorId?.Value;
            perRequestContainner.Xadmin = this.Xadmin?.Value;
            perRequestContainner.XgetInternalAllField = this.XGetInnerAllField?.Value ?? false;
            perRequestContainner.XRequestContinuation = this.XRequestContinuation?.ContinuationString;
            perRequestContainner.Xversion = this.Xversion?.Value ?? null;
            perRequestContainner.XResourceSharingWith = container.XResourceSharingWith;
            perRequestContainner.XResourceSharingPerson = container.XResourceSharingPerson;
            perRequestContainner.RequestHeaders = container.RequestHeaders;
            //フラグは折る
            perRequestContainner.XAsync = false;

            registData.AddFirst(new JProperty("PerRequestDataContainer", JObject.FromObject(perRequestContainner)));
            if (!string.IsNullOrEmpty(Contents.ReadToString()))
            {
                registData.Add(new JProperty("RequestBody", Contents.ReadToString()));

            }

            registData.Add(new JProperty("MethodType", this.MethodType.Value.ToString()));
            string tempQuery = "";
            if (!string.IsNullOrEmpty(ConvertQueryString().GetQueryString()))
            {
                tempQuery = $"?{ConvertQueryString().GetQueryString()}";

            }
            registData.Add(new JProperty("ActionType", this.AsyncOriginalActionType.Code));

            registData.Add(new JProperty("Accept", this.Accept?.GetResponseMediaType(this.MediaType).First().Value ?? "application/json"));
            registData.Add(new JProperty("QueryString", tempQuery));
            registData.Add(new JProperty("Url", RelativeUri.Value));

            registData.Add(new JProperty("RequestId", requestId.Value));
            var asyncResultLog = new JObject();
            asyncResultLog.Add(new JProperty("RequestId", requestId.Value));
            asyncResultLog.Add(new JProperty("Status", AsyncStatus.Request.ToString()));
            asyncResultLog.Add(new JProperty("RequestDate", DateTime.UtcNow));
#if Oracle
            asyncResultLog.Add(new JProperty("StartDate", null));
            asyncResultLog.Add(new JProperty("EndDate", null));
#else
            asyncResultLog.Add(new JProperty("StartDate", ""));
            asyncResultLog.Add(new JProperty("EndDate", ""));
#endif
            asyncResultLog.Add(new JProperty("ExecutionTime", ""));
            asyncResultLog.Add(new JProperty("ResultPath", ""));
            asyncResultLog.Add(new JProperty("OpenId", this.OpenId?.Value ?? ""));
            asyncResultLog.Add(new JProperty("VendorId", this.VendorId?.Value ?? ""));
            asyncResultLog.Add(new JProperty("SystemId", this.SystemId?.Value ?? ""));
            asyncResultLog.Add(new JProperty("Url", RelativeUri.Value));
            asyncResultLog.Add(new JProperty("QueryString", tempQuery));
            asyncResultLog.Add(new JProperty("RequestBody", this.Contents?.ReadToString() ?? ""));
            asyncResultLog.Add(new JProperty("MethodType", this.MethodType.Value.ToString()));
            asyncResultLog.Add(new JProperty("Accept", this.Accept?.GetResponseMediaType(this.MediaType).First().Value ?? "application/json"));

            if (container.ClientName == "JP.DataHub.AdminWeb")
            {
                AsyncDyanamicApiRepository.RequestOnCache(requestId, new JsonDocument(registData), new JsonDocument(asyncResultLog), this.ControllerId);
            }
            else
            {
                AsyncDyanamicApiRepository.Request(requestId, new JsonDocument(registData), new JsonDocument(asyncResultLog));
            }
            var returnData = new JObject();
            returnData.Add(new JProperty("RequestId", requestId.Value));
            return TupleToHttpResponseMessage(new Tuple<System.Net.HttpStatusCode, string>(System.Net.HttpStatusCode.Accepted, returnData.ToString()));
        }

        private QueryStringVO ConvertQueryString()
        {
            var tempRet = new Dictionary<QueryStringKey, QueryStringValue>();
            if (this.Query != null)
            {
                if (KeyValue != null && KeyValue.Dic.Count > 0)
                {
                    foreach (var query in this.Query.Dic)
                    {
                        if (KeyValue.Dic.Any(x => x.Key.Value != query.Key.Value))
                        {
                            tempRet.Add(new QueryStringKey(query.Key.Value), new QueryStringValue(query.Value.Value));
                        }
                    }
                }
                else
                {
                    return Query;
                }
            }
            return new QueryStringVO(tempRet);
        }
    }
}
