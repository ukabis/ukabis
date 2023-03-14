using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/RdbmsTableJoinTestBase", typeof(ODataPatchModel))]
    public interface IRdbmsTableJoinTestBaseApi : ICommonResource<ODataPatchModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ODataPatchModel>> GetAll();

        [WebApi("GetJoinedData?Key={key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RdbmsTableJoinModel>> GetJoinedData(string key);

        [WebApi("GetFailedByJoinNotAllowed")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RdbmsTableJoinModel>> GetFailedByJoinNotAllowed();

        [WebApi("GetJoinedDataWithAccessControll?Key={key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RdbmsTableJoinModel>> GetJoinedDataWithAccessControll(string key);

        [WebApi("GetFailedByUnexistingResource")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RdbmsTableJoinModel>> GetFailedByUnexistingResource();

        [WebApi("GetResourceSpecifiedJoin?Key={key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RdbmsTableJoinModel>> GetResourceSpecifiedJoin(string key);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<ODataPatchModel> model);

        [WebApiPatch("Update/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel UpdateEx(string key, ODataPatchModelForUpd model);
    }
}
