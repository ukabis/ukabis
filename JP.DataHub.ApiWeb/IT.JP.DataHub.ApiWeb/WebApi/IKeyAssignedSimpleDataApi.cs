using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/KeyAssignedSimpleData", typeof(AreaUnitModel))]
    public interface IKeyAssignedSimpleDataApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll();

        [WebApi("Get?AreaUnitCode={areaUnitCode}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByQuery(string areaUnitCode);

        [WebApi("Get?id={id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetById(string id);

        [WebApi("Get/{key}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByHex(string key);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModel model);

        [WebApiPost("AutoKeyRegist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> AutoKeyRegist(AreaUnitModel model);

        [WebApiPost("RegistNoValidation")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistNoValidation(AreaUnitModel model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<AreaUnitModel> model);

        [WebApiPatch("UpdateNoValidation/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel UpdateNoValidation(string key, AreaUnitModel model);

        [WebApiPatch("Update/{key}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel UpdateByHex(string key, AreaUnitModel model);

        [WebApiDelete("Delete/{key}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteByHex(string key);
    }
}
