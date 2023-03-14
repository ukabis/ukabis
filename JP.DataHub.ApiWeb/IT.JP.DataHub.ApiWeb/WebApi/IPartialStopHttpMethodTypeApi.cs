using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/PartialStopTest/HttpMethodTypeTest", typeof(AreaUnitModel))]
    public interface IPartialStopHttpMethodTypeApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetSuccess")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetSuccess();

        [WebApi("GetDisabled")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetDisabled();

        [WebApi("GetError")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetError();


        [WebApiPost("PostSuccess")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> PostSuccess();

        [WebApiPost("PostDisabled")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> PostDisabled();

        [WebApiPost("PostError")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> PostError();


        [WebApiPut("PutSuccess")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> PutSuccess();

        [WebApiPut("PutDisabled")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> PutDisabled();

        [WebApiPut("PutError")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> PutError();


        [WebApiDelete("DeleteSuccess")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> DeleteSuccess();

        [WebApiDelete("DeleteDisabled")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> DeleteDisabled();

        [WebApiDelete("DeleteError")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> DeleteError();


        [WebApiPatch("PatchSuccess")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> PatchSuccess();

        [WebApiPatch("PatchDisabled")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> PatchDisabled();

        [WebApiPatch("PatchError")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> PatchError();
    }
}
