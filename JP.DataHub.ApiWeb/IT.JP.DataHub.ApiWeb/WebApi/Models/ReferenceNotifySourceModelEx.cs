using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    /// <summary>
    /// ArrayDataがnullプロパティあり
    /// </summary>
    public class ReferenceNotifySourceModelEx : BaseModel
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

        // NULLでもプロパティあり
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<string> ArrayData { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<decimal> NumberArray { get; set; }

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

    /// <summary>
    /// 全てnullプロパティあり
    /// </summary>
    public class ReferenceNotifySourceModelEx2 : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string AreaUnitCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string AreaUnitName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public decimal? ConversionSquareMeters { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public decimal? ConversionSquareMeters2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public ReferenceNotifyObjectEx Obj { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public PropObjectEx Object { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<string> Array { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<string> ArrayData { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<decimal?> NumberArray { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<ReferenceNotifySourceArrayObjectItemEx> ArrayObject { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public bool? BooleanNullProp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public decimal? NumberNullProp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string StringNullProp { get; set; }
    }
}
