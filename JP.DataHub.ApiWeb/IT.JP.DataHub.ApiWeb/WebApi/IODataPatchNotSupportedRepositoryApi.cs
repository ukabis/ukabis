using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ODataPatchNotSupportedRepositoryTest", typeof(AreaUnitModel))]
    public interface IODataPatchNotSupportedRepositoryApi : ICommonResource<AreaUnitModel>
    {
        [WebApiPatch("ODataPatch", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataPatchAll(string dummy, AreaUnitModel model);
    }
}
