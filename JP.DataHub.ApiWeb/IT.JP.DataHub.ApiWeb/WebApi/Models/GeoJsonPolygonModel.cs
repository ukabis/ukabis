using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class GeoJsonPolygonDataModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CityCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<GeoJsonPolygonDataPolygon> Polygons { get; set; }
    }

    public class GeoJsonPolygonDataPolygon
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<GeoJsonPolygonDataPoint> Coordinates { get; set; }
    }

    public class GeoJsonPolygonDataPoint
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Latitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Longitude { get; set; }
    }


    public class GeoJsonPolygonModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<GeoJsonPolygonFeature> features { get; set; }
    }

    public class GeoJsonPolygonFeature
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public GeoJsonPolygonGeometry geometry { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public GeoJsonPolygonProperty properties { get; set; }
    }

    public class GeoJsonPolygonGeometry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<List<List<decimal>>> coordinates { get; set; }
    }

    public class GeoJsonPolygonProperty
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CityCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _Owner_Id { get; set; }
    }
}
