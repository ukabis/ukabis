using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ApiWeb.Models.AttachFile
{
    /// <summary>
    /// ファイル情報
    /// </summary>
    public class CreateFileViewModel
    {
        /// <summary>
        /// ファイル名
        /// </summary>
        [DisplayName("ファイル名")]
        [Required(ErrorMessage = "必須項目です。")]
        public string FileName { get; set; }

        /// <summary>
        /// 設定を行えばファイルを取得する際にこのキーが必要になる
        /// </summary>
        [DisplayName("設定を行えばファイルを取得する際にこのキーが必要になる")]
        public string Key { get; set; }

        /// <summary>
        /// Content-Type
        /// </summary>
        [DisplayName("Content-Type")]
        [Required(ErrorMessage = "必須項目です。")]
        public string ContentType { get; set; }

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        [DisplayName("ファイルサイズ")]
        [Required(ErrorMessage = "必須項目です。")]
        public long FileLength { get; set; }

        /// <summary>
        /// DRMの有無
        /// </summary>
        [DisplayName("DRMの有無")]
        [Required(ErrorMessage = "必須項目です。")]
        public bool IsDrm { get; set; }

        /// <summary>
        /// DRMのタイプ
        /// </summary>
        [DisplayName("Drmのタイプ")]
        public string DrmType { get; set; }

        /// <summary>
        /// DRMのキー
        /// </summary>
        [DisplayName("Drmのキー")]
        public string DrmKey { get; set; }

        /// <summary>
        /// メタのリスト
        /// </summary>
        [DisplayName("メタのリスト")]
        public List<MetaViewModel> MetaList { get; set; }
    }
}