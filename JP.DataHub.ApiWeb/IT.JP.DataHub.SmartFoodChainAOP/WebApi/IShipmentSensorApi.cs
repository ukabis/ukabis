using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.IT.Api.Com.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using Newtonsoft.Json;
using JP.DataHub.Com.Net.Http.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi
{
    [WebApiResource("/API/Traceability/V3/Private/ShipmentSensor", typeof(ShipmentSensorModel))]
    public interface IShipmentSensorApi : ICommonResource<ShipmentSensorModel>
    {
        [WebApiDelete("RemoveSensor?ShipmentSensorId={ShipmentSensorId}&LastArrivalId={LastArrivalId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> RemoveSensor(string ShipmentSensorId, string LastArrivalId);

        [WebApiDelete("Lost?ShipmentSensorId={ShipmentSensorId}")]
        [AutoGenerateReturnModel]
        public WebApiRequestModel<string> Lost(string ShipmentSensorId);
    }
}
