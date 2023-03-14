using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/JsonValidation", typeof(AreaUnitModel))]
    public interface IJsonValidationApi : ICommonResource<AreaUnitModel>
    {
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegistAsString(string value);

        [WebApiPost("RegisterList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegisterListAsString(string value);

        [WebApiPatch("UpdateById/{id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> UpdateById(string id, string value);
    }
}
