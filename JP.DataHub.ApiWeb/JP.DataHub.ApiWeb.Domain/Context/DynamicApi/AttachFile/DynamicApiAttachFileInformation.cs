using System.Collections.ObjectModel;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.HashAlgorithm;
using JP.DataHub.ApiWeb.Core.DataContainer;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile
{
    // .NET6
    internal record DynamicApiAttachFileInformation : IValueObject
    {
        /// <summary>
        /// ファイルID
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FileId { get; }

        /// <summary>
        /// ファイル名
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FileName { get; }

        /// <summary>
        /// Key
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; }

        /// <summary>
        /// ContentType
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContentType { get; }

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        public long FileLength { get; }

        /// <summary>
        /// DRMありか
        /// </summary>
        public bool IsDrm { get; }

        /// <summary>
        /// Drmのタイプ
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DrmType { get; }

        /// <summary>
        /// Drmのキー
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DrmKey { get; }

        /// <summary>
        /// ファイルをアップロード済みか
        /// </summary>
        public bool IsUploaded { get; }

        /// <summary>
        /// Fileのパス
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// メタ
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ReadOnlyCollection<Meta> MetaList { get; }

        /// <summary>
        /// 外部添付ファイルかどうか
        /// </summary>
        public bool IsExternalAttachFile { get; }

        /// <summary>
        /// 外部添付ファイル情報
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExternalAttachFileInfomation ExternalAttachFile { get; }

        [JsonIgnore]
        public string BlobContainerName
        {
            get
            {
                var ps = FilePath.Split('/');
                if (ps.Length > 1)
                {
                    return ps[0];
                }
                return "";
            }
        }

        [JsonIgnore]
        public string BlobName
        {
            get
            {
                var index = FilePath.IndexOf("/");
                if (index > 1)
                {
                    return FilePath.Substring(index + 1, FilePath.Length - (index + 1));
                }
                return "";
            }
        }

        [JsonIgnore]
        public string BlobFileName
        {
            get
            {
                var index = FilePath.LastIndexOf("/");
                if (index > 1)
                {
                    return FilePath.Substring(index + 1, FilePath.Length - (index + 1));
                }
                return "";
            }
        }

        public static DynamicApiAttachFileInformation PerseFromJToken(JToken json)
        {
            var metalist = new List<Meta>();
            if (json.Select(p => p.Path).Contains(nameof(MetaList)))
            {
                foreach (var meta in json[nameof(MetaList)].Children())
                {
                    metalist.Add(new Meta()
                    {
                        MetaKey = meta.Value<string>("MetaKey"),
                        MetaValue = meta.Value<string>("MetaValue")
                    });
                }
            }

            var externalAttachFile = json.IsExistProperty("ExternalAttachFile")
                ? ExternalAttachFileInfomation.PerseFromJToken(json["ExternalAttachFile"])
                : null;

            return new DynamicApiAttachFileInformation(
                fileId: json.Value<string>(nameof(FileId)),
                fileName: json.Value<string>(nameof(FileName)),
                key: json.Value<string>(nameof(Key)),
                contentType: json.Value<string>(nameof(ContentType)),
                fileLength: json.Value<long>(nameof(FileLength)),
                isDrm: json.Value<bool>(nameof(IsDrm)),
                drmType: json.Value<string>(nameof(DrmType)),
                drmKey: json.Value<string>(nameof(DrmKey)),
                isUploaded: json.Value<bool>(nameof(IsUploaded)),
                filePath: json.Value<string>(nameof(FilePath)),
                metalist: metalist,
                isExternalAttachFile: json.Value<bool>(nameof(IsExternalAttachFile)),
                externalAttachFile: externalAttachFile
             );
        }

        public DynamicApiAttachFileInformation(
            string fileId,
            string fileName,
            string contentType,
            long fileLength,
            string filePath,
            string key = null,
            bool isDrm = false,
            string drmType = null,
            string drmKey = null,
            bool isUploaded = false,
            List<Meta> metalist = null,
            bool isExternalAttachFile = false,
            ExternalAttachFileInfomation externalAttachFile = null)
        {
            if (fileLength <= 0)
            {
                throw new ArgumentNullException("fileLength");
            }

            if (fileId == null)
            {
                FileId = Guid.NewGuid().ToString();
                if (!isExternalAttachFile)
                {
                    FilePath = GetPath(FileId, fileName);
                }
            }
            else
            {
                FileId = fileId;
                FilePath = filePath;
            }

            FileName = fileName;
            ContentType = contentType;
            FileLength = fileLength;
            Key = key;
            IsDrm = isDrm;
            DrmType = drmType;
            DrmKey = drmKey;
            IsUploaded = isUploaded;
            MetaList = metalist?.AsReadOnly();
            IsExternalAttachFile = isExternalAttachFile;
            ExternalAttachFile = externalAttachFile;
        }

        public bool IsKeyMatch(string key)
        {
            if (!string.IsNullOrEmpty(this.Key) && (this.Key != key))
            {
                return false;
            }
            return true;
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        private string GetPath(string fileId, string fileName)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            var vendor = (perRequestDataContainer.VendorId != null) ? perRequestDataContainer.VendorId : UnityCore.Resolve<string>("VendorSystemAuthenticationDefaultVendorId");
            var fileNameHash = string.IsNullOrWhiteSpace(fileName) ? null : HashCalculation.ComputeHashString(Encoding.Unicode.GetBytes(fileName), HashAlgorithmType.Sha256);
            var path = $"{vendor}/{fileId}/{fileNameHash}";
            return path;
        }


        public class Meta
        {
            public string MetaKey { get; set; }
            public string MetaValue { get; set; }
        }
    }
}
