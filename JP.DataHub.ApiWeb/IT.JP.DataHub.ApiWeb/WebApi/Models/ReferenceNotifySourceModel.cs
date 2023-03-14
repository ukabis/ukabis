using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class ReferenceNotifySourceModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AreaUnitCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AreaUnitName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? ConversionSquareMeters { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? ConversionSquareMeters2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ReferenceNotifyObject Obj { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PropObject Object { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Array { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ArrayData { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<decimal?> NumberArray { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ReferenceNotifySourceArrayObjectItem> ArrayObject { get; set; }

        // NULLでもプロパティあり
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public bool? BooleanNullProp { get; set; }

        // NULLでもプロパティあり
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public decimal? NumberNullProp { get; set; }

        // NULLでもプロパティあり
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string StringNullProp { get; set; }
    }

    public class ReferenceNotifySourceArrayObjectItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string p1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string p2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Date { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ObservationItemCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ObservationValue { get; set; }
    }

    /// <summary>
    /// 全てnullプロパティあり
    /// </summary>
    public class ReferenceNotifySourceArrayObjectItemEx
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string Date { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string ObservationItemCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string ObservationValue { get; set; }
    }
}
