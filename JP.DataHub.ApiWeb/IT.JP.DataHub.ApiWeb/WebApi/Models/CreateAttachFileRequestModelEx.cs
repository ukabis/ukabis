using System.Collections.Generic;
using Newtonsoft.Json;
using JP.DataHub.Com.Net.Http.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class CreateAttachFileRequestModelEx : CreateAttachFileRequestModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FileId { get; set; }
    }

    public class CreateAttachFileRequestModelEx2 : CreateAttachFileRequestModel
    {
        [JsonProperty("FileLength")]
        public new string fileLength { get; set; }
    }
}
