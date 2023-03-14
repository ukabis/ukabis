using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class HistoryThrowAwayAction : QueryAction
    {
        public override HttpResponseMessage ExecuteAction()
        {
            if (this.MethodType.IsDelete != true)
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }
            if (EnableJsonDocumentHistory == false || IsDocumentHistory?.Value != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30501, this.RelativeUri?.Value);
            }
            if (Query == null || Query.ContainKey("id") == false)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30402, this.RelativeUri?.Value);
            }

            this.ShallowMapProperty(DynamicApiDataStoreRepository[0].DocumentVersionRepository);
            var documentKey = new DocumentKey(this.RepositoryKey, Query.GetValue("id"));
            var result = DynamicApiDataStoreRepository[0].DocumentVersionRepository.HistoryThrowAway(documentKey);
            return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(result == true ? HttpStatusCode.OK : HttpStatusCode.NotFound, null));
        }
    }
}