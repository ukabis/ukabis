using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ApiWeb.Models.AttachFile
{
    /// <summary>
    /// メタ
    /// </summary>
    public class MetaViewModel
    {
        /// <summary>
        /// メタのキー
        /// </summary>
        [DisplayName("メタのキー")]
        [Required(ErrorMessage = "必須項目です。")]
        public string MetaKey { get; set; }

        /// <summary>
        /// メタの値
        /// </summary>
        [DisplayName("メタの値")]
        [Required(ErrorMessage = "必須項目です。")]
        public string MetaValue { get; set; }
    }
}