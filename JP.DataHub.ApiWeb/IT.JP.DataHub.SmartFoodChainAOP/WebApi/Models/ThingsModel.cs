using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class ThingsModel
    {
        public string key { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string thingType { get; set; }

        public string vendorPlaceId { get; set; }

        public PropertiesModel properties { get; set; }

        public class PropertiesModel
        {
            public string sensorId { get; set; }

            public List<SensorModel> sensors { get; set; }

            public string ownerName { get; set; }

            public string tel { get; set; }

            public string address { get; set; }

            public string mailaddress { get; set; }
        }

        public class SensorModel
        {
            public string sensorId { get; set; }

            public string sensorDeviceId { get; set; }
        }
    }
}
