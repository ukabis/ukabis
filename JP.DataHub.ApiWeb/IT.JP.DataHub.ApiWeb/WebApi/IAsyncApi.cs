using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/AsyncAPI", typeof(GetStatusResponseModel))]
    public interface IAsyncApi : ICommonResource<GetStatusResponseModel>
    {
        [WebApi("GetStatus?RequestId={requestId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<GetStatusResponseModel> GetStatus(string requestId);

        [WebApi("GetResult?RequestId={requestId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetResult(string requestId);
    }
}
