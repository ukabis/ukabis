using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http.Models
{
    public class CreateAttachFileResponseModel
    {
        [JsonProperty("FileId")]
        public string fileId { get; set; }
    }
}
