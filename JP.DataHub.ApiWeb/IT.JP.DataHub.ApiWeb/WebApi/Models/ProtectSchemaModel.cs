using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class ProtectSchemaModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? temp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ap { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ap2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<PropArrayItem> PropArray { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PropObject PropObject { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ArrayObjectProtectItem> ArrayObjectProtect { get; set; }
    }

    public class PropArrayItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Date { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ObservationItemCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ObservationValue { get; set; }
    }

    public class PropObject
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string prop1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string prop2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PropObjectChild prop3 { get; set; }
    }
    public class PropObjectChild
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string prop31 { get; set; }
    }

    /// <summary>
    /// 全てnullプロパティあり
    /// </summary>
    public class PropObjectEx
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string prop1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string prop2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public PropObjectChildEx prop3 { get; set; }
    }

    /// <summary>
    /// 全てnullプロパティあり
    /// </summary>
    public class PropObjectChildEx
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string prop31 { get; set; }
    }

    public class ArrayObjectProtectItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Date { get; set; }
    }
}
