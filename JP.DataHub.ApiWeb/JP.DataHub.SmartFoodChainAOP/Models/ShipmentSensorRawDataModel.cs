using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Models
{
    [MessagePackObject(true)]
    [Serializable]
    public class ShipmentSensorRawDataModel
    {
        public string sensorDeviceId { get; set; }
        public string thingsId { get; set; }
        public string datastreamId { get; set; }
        public string observedPropertyId { get; set; }
        public string observedPropertyName { get; set; }
        public string measurementId { get; set; }
        public string measurementName { get; set; }
        public string observationId { get; set; }
        public DateTime phenomenonTime { get; set; }
        public float observationResult { get; set; }
    }
}
