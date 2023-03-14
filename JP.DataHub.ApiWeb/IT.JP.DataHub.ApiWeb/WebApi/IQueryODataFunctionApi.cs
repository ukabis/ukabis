using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/QueryODataFunctionTest", typeof(AreaUnitModelEx))]
    public interface IQueryODataFunctionApi : IQueryODataApi
    {
        [WebApi("QueryODataGetNormal")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModelEx>> QueryODataGetNormal();

        [WebApi("QueryODataGetWithRepositoryKey/{areaUnitCode}/{areaUnitName}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModelEx> QueryODataGetWithRepositoryKey(string areaUnitCode, string areaUnitName);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModelEx model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<AreaUnitModelEx> model);
    }
}
