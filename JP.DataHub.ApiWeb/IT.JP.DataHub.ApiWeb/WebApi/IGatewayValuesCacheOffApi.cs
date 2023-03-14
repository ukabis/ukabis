using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/Gateway/Values/ChacheOff", typeof(string))]
    public interface IGatewayValuesCacheOffApi : ICommonResource<string>
    {
        [WebApi("Get?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> GetByQueryString(string querystring);

        [WebApi("Get?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AsyncRequestResponseModel> GetByQueryStringAsync(string querystring);
    }
}
