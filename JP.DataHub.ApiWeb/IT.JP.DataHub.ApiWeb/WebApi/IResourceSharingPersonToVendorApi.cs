using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ResourceSharingPersonToVendor", typeof(AreaUnitModel))]
    public interface IResourceSharingPersonToVendorApi : ICommonResource<AreaUnitModel>
    {
        [WebApiPost("AgentRegister")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> AgentRegister(AreaUnitModel model);

        [WebApiPatch("AgentUpdate/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> AgentUpdate(string key, AreaUnitModel model);
    }
}
