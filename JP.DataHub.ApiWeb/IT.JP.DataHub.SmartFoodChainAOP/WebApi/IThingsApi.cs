using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;
using JP.DataHub.Com.Net.Http.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{

    [WebApiResource("/API/Sensing/V3/Private/Things", typeof(ThingsModel))]
    public interface IThingsApi
    {
        [WebApiDelete("ODataDelete?$filter={filter}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<string> ODataDelete(string filter);

        [WebApi("Get/{id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ThingsModel> Get(string id);
    }
}
