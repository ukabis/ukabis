using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class AgriculturalLandModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CityCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Latitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Longitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AgriculturalLandGeoSearch GeoSearch { get; set; }
    }

    public class AgriculturalLandGeoSearch
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<decimal> coordinates { get; set; }
    }
}
