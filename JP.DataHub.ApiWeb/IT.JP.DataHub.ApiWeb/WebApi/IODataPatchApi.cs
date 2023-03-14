using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ODataPatchTest", typeof(ODataPatchModel))]
    public interface IODataPatchApi : ICommonResource<ODataPatchModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ODataPatchModel>> GetAll();

        [WebApiPatch("ODataPatch?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataPatch(string querystring, ODataPatchModelForUpd model);

        [WebApiPatch("ODataPatch?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataPatchEx(string querystring, ODataPatchModelForUpdEx model);

        [WebApiPatch("ODataPatch?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataPatchEx2(string querystring, ODataPatchModelForUpdEx2 model);

        [WebApiPatch("ODataPatch?{querystring}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataPatchAsString(string querystring, string value);

        [WebApiPatch("ODataPatch", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel ODataPatchAll(string dummy, ODataPatchModelForUpd model);
    }
}
