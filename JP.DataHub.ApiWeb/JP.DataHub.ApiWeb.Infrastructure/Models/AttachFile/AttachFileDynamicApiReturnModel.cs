
namespace JP.DataHub.ApiWeb.Infrastructure.Models.AttachFile
{
    public class AttachFileDynamicApiReturnModel
    {
        /// <summary>
        /// ファイルID
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// AttachFileStorageId
        /// </summary>
        public string AttachFileStorageId { get; set; }

        /// <summary>
        /// BlobUrl
        /// </summary>
        public string BlobUrl { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        public long FileLength { get; set; }

        /// <summary>
        /// DRMありか
        /// </summary>
        public bool IsDrm { get; set; }

        /// <summary>
        /// Drmのタイプ
        /// </summary>
        public string DrmType { get; set; }

        /// <summary>
        /// Drmのキー
        /// </summary>
        public string DrmKey { get; set; }

        /// <summary>
        /// メタ
        /// </summary>
        public List<AttachFileMeta> MetaList { get; set; }

        /// <summary>
        /// VendorId
        /// </summary>
        public Guid VendorId { get; set; }

        /// <summary>
        /// ファイルアップロード済みか
        /// </summary>
        public bool? IsUploaded { get; set; }
    }


    /// <summary>
    /// 添付ファイルのメタ
    /// </summary>
    public class AttachFileMeta
    {
        /// <summary>
        /// MetaKey
        /// </summary>
        public string MetaKey { get; set; }
        /// <summary>
        /// MetaValue
        /// </summary>
        public string MetaValue { get; set; }
    }
}