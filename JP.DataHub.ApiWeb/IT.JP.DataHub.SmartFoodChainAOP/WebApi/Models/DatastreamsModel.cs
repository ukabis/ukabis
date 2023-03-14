using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class DatastreamsModel
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

        public class PolygonModel
        {
            public enum Type
            {
                Polygon
            }

            public string type { get; set; }

            public List<List<List<double>>> coordinates { get; set; }
        }
    }
}
