using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace JP.DataHub.ApiWeb.Infrastructure.Configuration
{
    /// <summary>
    /// Oracle Object Storage Setting.
    /// </summary>
    public class OciStorageSetting
    {
        /// <summary> ネームスペース </summary>
        [DisplayName("ネームスペース")]
        [Required]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string NameSpaceName { get; set; }

        /// <summary> バケット名 </summary>
        [DisplayName("バケット名")]
        [Required]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string BucketName { get; set; }

        /// <summary> rootディレクトリパス </summary>
        [DisplayName("rootディレクトリパス")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string RootPath { get; set; }

        /// <summary> リトライ間隔(ミリ秒) </summary>
        [DisplayName("リトライ間隔(ミリ秒)")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int RetryInterval { get; set; }

        /// <summary> リトライ回数 </summary>
        [DisplayName("リトライ回数")]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public int RetryCount { get; set; }

        /// <summary> 構成ファイルパス </summary>
        [DisplayName("構成ファイルパス")]
        [Required]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string ConfigPath { get; set; }

        /// <summary> プロファイル名 </summary>
        [DisplayName("プロファイル名")]
        [Required]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string ProfileName { get; set; }

        /// <summary> キーサプライヤパス </summary>
        [DisplayName("キーサプライヤパス")]
        [Required]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string PemFilePath { get; set; }

        /// <summary> ホスト名 </summary>
        [DisplayName("ホスト名")]
        [Required]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string HostName { get; set; }
    }
}
