using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/Gateway/Images/CacheOff", typeof(string))]
    public interface IGatewayImagesCacheOffApi : ICommonResource<string>
    {
        [WebApi("Get/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AsyncRequestResponseModel> GetAsync(string key);
    }
}
