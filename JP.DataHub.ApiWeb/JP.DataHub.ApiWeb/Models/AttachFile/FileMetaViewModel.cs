using System.Collections.Generic;
using System.ComponentModel;

namespace JP.DataHub.ApiWeb.Models.AttachFile
{
    /// <summary>
    /// ファイル情報
    /// </summary>
    public class FileMetaViewModel
    {
        /// <summary>
        /// ファイルId
        /// </summary>
        [DisplayName("ファイルId")]
        public string FileId { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        [DisplayName("ファイル名")]
        public string FileName { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        [DisplayName("Key")]
        public string Key { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        [DisplayName("ContentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        [DisplayName("ファイルサイズ")]
        public long FileLength { get; set; }

        /// <summary>
        /// DRMありか
        /// </summary>
        [DisplayName("DRMありか")]
        public bool IsDrm { get; set; }

        /// <summary>
        /// Drmのタイプ
        /// </summary>
        [DisplayName("Drmのタイプ")]
        public string DrmType { get; set; }

        /// <summary>
        /// Drmのキー
        /// </summary>
        [DisplayName("Drmのキー")]
        public string DrmKey { get; set; }

        /// <summary>
        /// メタ
        /// </summary>
        [DisplayName("メタ")]
        public List<MetaViewModel> MetaList { get; set; }
    }
}