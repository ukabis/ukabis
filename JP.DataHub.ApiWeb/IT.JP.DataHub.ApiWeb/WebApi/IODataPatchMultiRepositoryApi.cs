using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ODataPatchMultiRepositoryTest", typeof(AreaUnitModel))]
    public interface IODataPatchMultiRepositoryApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAllFromCosmosDB")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAllFromCosmosDB();

        [WebApi("GetAllFromSqlServer")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAllFromSqlServer();

        [WebApiPatch("ODataPatch", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataPatchAll(string dummy, AreaUnitModel model);
    }
}
