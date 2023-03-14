using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/VendorPrivate/SimpleData", typeof(AreaUnitModel))]
    public interface IVendorPrivateSimpleDataApi : ICommonResource<AreaUnitModel>
    {
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModel model);

        [WebApi("GetWithAdditionalPropertiesFalse/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetWithAdditionalPropertiesFalse(string key);
    }
}
