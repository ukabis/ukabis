using System.ComponentModel;

namespace JP.DataHub.ApiWeb.Models.AttachFile
{
    /// <summary>
    /// ファイル作成結果
    /// </summary>
    public class CreateFileResultViewModel
    {
        /// <summary>
        /// ファイルID
        /// </summary>
        [DisplayName("ファイルID")]
        public string FileId { get; set; }
    }
}