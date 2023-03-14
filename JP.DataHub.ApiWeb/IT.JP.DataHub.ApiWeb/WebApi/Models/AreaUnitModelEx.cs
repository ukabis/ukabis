using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class AreaUnitModelEx : AreaUnitModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AreaUnitMeta> MetaList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? TestNum { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PK { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Timestamp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AreaUnitCodeLower { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AreaUnitCodeUpper { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? IntValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? DoubleValue1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? DoubleValue2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public GeoJsonPointGeometry GeoSearch { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DummyItem { get; set; }
    }

    public class AreaUnitMeta
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MetaKey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MetaValue { get; set; }
    }
}
