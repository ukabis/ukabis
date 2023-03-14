using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Net.Http.Models
{
    public class CreateAttachFileRequestModel
    {
        /// <summary>
        /// ファイル名
        /// </summary>
        [JsonProperty("FileName")]
        public string fileName { get; set; }

        /// <summary>
        /// キー
        /// </summary>
        [JsonProperty("Key")]
        public string? key { get; set; }

        /// <summary>
        /// Content-Type
        /// </summary>
        [JsonProperty("ContentType")]
        public string contentType { get; set; }

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        [JsonProperty("FileLength")]
        public int fileLength { get; set; }

        /// <summary>
        /// Drmの有無
        /// </summary>
        [JsonProperty("IsDrm")]
        public bool isDrm { get; set; }

        /// <summary>
        /// Drmのタイプ
        /// </summary>
        [JsonProperty("DrmType")]
        public string? drmType { get; set; }

        /// <summary>
        /// Drmのキー
        /// </summary>
        [JsonProperty("DrmKey")]
        public string? drmKey { get; set; }

        /// <summary>
        /// メタのリスト
        /// </summary>
        [JsonProperty("MetaList")]
        public List<MetaInfo> metaList { get; set; }

        public class MetaInfo
        {
            [JsonProperty("MetaKey")]
            public string metaKey { get; set; }
            [JsonProperty("MetaValue")]
            public string metaValue { get; set; }
        }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsExternalAttachFile { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExternalAttachFileModel ExternalAttachFile { get; set; }
    }
}
