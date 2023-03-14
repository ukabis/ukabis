using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class ShipmentSensorRawDataResultModel
    {
        public string sensorDeviceId { get; set; }
        public string thingsId { get; set; }
        public string datestreamId { get; set; }
        public string observedPropertyId { get; set; }
        public string observedPropertyName { get; set; }
        public string measurementId { get; set; }
        public string measurementName { get; set; }
        public string observationId { get; set; }
        public DateTime phenomenonTime { get; set; }
        public string observationResult { get; set; }
    }
}
