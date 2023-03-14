using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/JsonSchemaEscapeKeywordsApi", typeof(AreaUnitModel))]
    public interface IJsonSchemaEscapeKeywordsApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("Get?Value={value}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByQuery(string value);

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegisterAsString(string value);
    }
}
