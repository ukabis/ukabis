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
    public class SensorDeviceCreateThingsModel
    {
        public string sensorDeviceId { get; set; }

        public SensorDeviceRegisterExThingModel thing { get; set; }

        public List<SensorDeviceRegisterExDataStreamModel> dataStreams { get; set; }
    }
}
