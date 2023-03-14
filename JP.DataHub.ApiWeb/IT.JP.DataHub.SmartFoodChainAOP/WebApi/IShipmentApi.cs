using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;
using JP.DataHub.Com.Net.Http.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/V3/Private/Shipment", typeof(ShipmentModel))]
    public interface IShipmentApi : ICommonResource<ShipmentModel>
    {
        [WebApiDelete("ODataDelete?$filter=Message eq '{Message}'")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> ODataDelete(string? Message);

        [WebApi("OData?$filter=Message eq '{Message}'")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<ShipmentModel>> OData(string? Message);
    }
}
