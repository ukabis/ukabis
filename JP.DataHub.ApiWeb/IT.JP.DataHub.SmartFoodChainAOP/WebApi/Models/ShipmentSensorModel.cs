using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class ShipmentSensorModel
    {
        public string ShipmentSensorId { get; set; }
        public string ShipmentId { get; set; }
        public string SensorDeviceId { get; set; }
        public List<string> ProductCode { get; set; }
        public DateTime? MeasurementStartDateTime { get; set; }
        public DateTime? MeasurementEndDateTime { get; set; }
        public string LastArrivalId { get; set; }
        public bool? IsLostSensor { get; set; }
    }
}
