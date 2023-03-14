using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    internal class DriveOutDocumentAction : QueryAction, IEntity
    {
        // .NET6
        public override HttpResponseMessage ExecuteAction()
        {
            if (this.MethodType.IsGet != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30410, this.RelativeUri?.Value);
            }
            if (EnableJsonDocumentHistory == false || IsDocumentHistory?.Value != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30501, this.RelativeUri?.Value);

            }
            if (Query == null || Query.ContainKey("id") == false)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30402, this.RelativeUri?.Value);
            }
            var id = new Identification(Query.GetValue("id"));

            // 管理項目も含めて取得
            PerRequestDataContainer.XgetInternalAllField = true;
            var result = DynamicApiDataStoreRepository[0].QueryOnce(ValueObjectUtil.Create<QueryParam>(this));
            if (result == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30412, this.RelativeUri?.Value);
            }

            // キャッシュ削除
            var cacheDeleteTasks = RefreshApiResourceCache(CreateResourceCacheKey());

            // データ退避
            MakeHistory(new string[] { id.Value }, new JToken[] { result.Value }, false, true);
            // データ削除
            DynamicApiDataStoreRepository[0].DeleteOnce(ValueObjectUtil.Create<DeleteParam>(result.Value, this));
            try
            {
                Task.WaitAll(cacheDeleteTasks);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    if (e is NotImplementedException)
                    {
                        // cache none....
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.NoContent, ""));
        }
    }
}
