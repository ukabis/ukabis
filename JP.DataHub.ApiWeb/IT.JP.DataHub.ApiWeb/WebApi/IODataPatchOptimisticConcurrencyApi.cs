using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ODataPatchOptimisticConcurrencyTest", typeof(AreaUnitModel))]
    public interface IODataPatchOptimisticConcurrencyApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll();

        [WebApiPatch("ODataPatch", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataPatchAll(string dummy, AreaUnitModel model);
    }
}
