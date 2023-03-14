using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/PersonPrivate/KeyAssignedSimpleData", typeof(AreaUnitModel))]
    public interface IPersonPrivateKeyAssignedSimpleDataApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetWithAdditionalPropertiesFalse/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetWithAdditionalPropertiesFalse(string key);
        
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModel model);
    }
}
