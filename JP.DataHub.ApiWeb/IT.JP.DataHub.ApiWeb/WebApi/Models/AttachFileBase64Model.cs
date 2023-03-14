using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class AttachFileBase64Model : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string file { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string file2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string file3 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string file_01 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string file_02 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> files { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AttachFileBase64Object fileObject { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AttachFileBase64Object> fileObjects { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name { get; set; }
    }

    /// <summary>
    /// file3更新用
    /// </summary>
    public class AttachFileBase64ModelForUpdate : AttachFileBase64Model
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public new string file3 { get; set; }
    }

    public class AttachFileBase64Object
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string file_01 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string file_02 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AttachFileBase64Object file2_1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AttachFileBase64Object file2_2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string file3 { get; set; }
    }
}
