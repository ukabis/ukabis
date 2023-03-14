using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Models
{
    [MessagePackObject(true)]
    [Serializable]
    public class SensorDeviceModel
    {
        public string sensorDeviceId { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string ownerOpenId { get; set; }

        public string thingsId { get; set; }

        public string sensorId { get; set; }

        public string uuid { get; set; }

        public List<string> datastreamIds { get; set; }
    }
}
