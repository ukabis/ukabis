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
    public class ThingModel
    {
        public enum ThingType
        {
            physical,
            logical
        }

        public string key { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string thingType { get; set; }

        public PropertiesModel properties { get; set; }

        [MessagePackObject(true)]
        [Serializable]
        public class PropertiesModel
        {
            public string sensorId { get; set; }

            public List<SensorModel> sensors { get; set; }

            public string ownerName { get; set; }

            public string tel { get; set; }

            public string address { get; set; }

            public string mailaddress { get; set; }
        }

        [MessagePackObject(true)]
        [Serializable]
        public class SensorModel
        {
            public string sensorId { get; set; }

            public string sensorDeviceId { get; set; }
        }
    }
}
