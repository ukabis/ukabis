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
    public class SensorModel
    {
        public string key { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string manufacturer { get; set; }

        public string productName { get; set; }

        public string modelNumber { get; set; }

        public string manufacturerMailaddress { get; set; }

        public List<MetadataModel> metadata { get; set; }

        public List<ObservationItemModel> observations { get; set; }
        
        [MessagePackObject(true)]
        [Serializable]
        public class MetadataModel
        {
            public string encodingType { get; set; }

            public string uri { get; set; }
        }

        [MessagePackObject(true)]
        [Serializable]
        public class ObservationItemModel
        {
            public string observedPropertyId { get; set; }

            public List<string> measurementUnitId { get; set; }
        }
    }
}
