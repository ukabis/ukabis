using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/V3/Public/SensorTraceability", typeof(ShipmentAggregateSensorResultModel))]
    public interface ISensorTraceabilityApi 
    {
        [WebApiGet("GetAggregateSensor?ProductCode={ProductCode}&ArrivalId={ArrivalId}&Time={Time}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<List<ShipmentAggregateSensorResultModel>> GetAggregateSensor(string ProductCode, string ArrivalId, int? Time);
    }
}
