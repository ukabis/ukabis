using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ODataPatchEscapeTest", typeof(AreaUnitModel))]
    public interface IODataPatchEscapeApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll();

        [WebApiPatch("ODataPatch?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataPatch(string querystring, AreaUnitModel model);
    }
}
