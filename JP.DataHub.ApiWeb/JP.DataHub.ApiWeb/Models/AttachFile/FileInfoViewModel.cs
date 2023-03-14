using System.Collections.Generic;
using System.ComponentModel;

namespace JP.DataHub.ApiWeb.Models.AttachFile
{
    /// <summary>
    /// ファイル情報
    /// </summary>
    public class FileInfoViewModel
    {
        /// <summary>
        /// ファイルID
        /// </summary>
        [DisplayName("ファイルID")]
        public string FileId { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        [DisplayName("ファイル名")]
        public string FileName { get; set; }

        /// <summary>
        /// Content-Type
        /// </summary>
        [DisplayName("Content-Type")]
        public string ContentType { get; set; }

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        [DisplayName("ファイルサイズ")]
        public long FileLength { get; set; }

        /// <summary>
        /// メタ
        /// </summary>
        [DisplayName("メタ")]
        public List<MetaViewModel> MetaList { get; set; }
    }
}