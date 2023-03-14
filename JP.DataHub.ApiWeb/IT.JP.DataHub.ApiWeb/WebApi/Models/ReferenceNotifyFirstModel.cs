using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class ReferenceNotifyFirstModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? temp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref4 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref4_type_stringnull { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref5 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref5_type_objectnull { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref6 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref6_numbercheck { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref7 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref7_numbercheck { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref8 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref9 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref10 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref11 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref12 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Ref13 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<PropArrayItem> PropArray { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PropObject PropObject { get; set; }
    }
}
