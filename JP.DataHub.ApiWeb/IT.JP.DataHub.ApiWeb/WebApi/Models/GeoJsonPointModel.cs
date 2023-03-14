using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class GeoJsonPointDataModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CityCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Longitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Latitude { get; set; }
    }

    public class GeoJsonPointModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<GeoJsonPointFeature> features { get; set; }
    }

    public class GeoJsonPointFeature
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public GeoJsonPointGeometry geometry { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public GeoJsonPointProperty properties { get; set; }
    }

    public class GeoJsonPointGeometry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<decimal> coordinates { get; set; }
    }

    public class GeoJsonPointProperty
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CityCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _Owner_Id { get; set; }
    }
}
