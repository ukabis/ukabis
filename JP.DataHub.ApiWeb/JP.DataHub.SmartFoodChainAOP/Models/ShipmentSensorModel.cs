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
    public class ShipmentSensorModel
    {
        public string ShipmentSensorId { get; set; }
        public string ShipmentId { get; set; }
        public DateTime? MeasurementStartDateTime { get; set; }
        public DateTime? MeasurementEndDateTime { get; set; }
        public List<string> ProductCode { get; set; }
        public string SensorDeviceId { get; set; }
        public string LastArrivalId { get; set; }
        public bool? IsLostSensor { get; set; }
    }
}
