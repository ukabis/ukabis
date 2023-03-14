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
    public class DatastreamModel
    {
        public string key { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public PolygonModel observedArea { get; set; }

        public string observedPropertyId { get; set; }

        public string sensorId { get; set; }

        public string observationConditionId { get; set; }

        public string unitOfMeasurementId { get; set; }

        public string thingId { get; set; }
    }
}
