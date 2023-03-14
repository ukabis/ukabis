using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/AdaptResourceSchemaTest", typeof(AreaUnitModel))]
    public interface IAdaptResourceSchemaApi : ICommonResource<AreaUnitModel>
    {
        [WebApiPost("RegisterList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegisterListAsString(string value);
    }
}
