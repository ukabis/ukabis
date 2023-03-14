using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/RegisterRawDataTest", typeof(AreaUnitModel))]
    public interface IRegisterRawDataApi : ICommonResource<AreaUnitModel>
    {
        [WebApiPost("RegisterRawData")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegisterRawDataAsString(string value);
    }
}
