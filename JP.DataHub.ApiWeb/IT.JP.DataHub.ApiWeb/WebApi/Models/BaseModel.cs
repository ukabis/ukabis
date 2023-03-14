using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public abstract class BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _Owner_Id { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _Reguser_Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _Regdate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _Upduser_Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _Upddate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? _Version { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _partitionkey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _Type { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _etag { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _Vendor_Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string _System_Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ODataPatchWhere _Where { get; set; }
    }


    public class ODataPatchWhere
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ColumnName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Operator { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Object { get; set; }
    }
}
