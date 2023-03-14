using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ODataPatchPersonPrivateTest", typeof(ODataPatchModel))]
    public interface IODataPatchPersonPrivateApi : ICommonResource<ODataPatchModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ODataPatchModel>> GetAll();

        [WebApiPatch("ODataPatch", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataPatchAll(string dummy, ODataPatchModelForUpd model);
    }
}
