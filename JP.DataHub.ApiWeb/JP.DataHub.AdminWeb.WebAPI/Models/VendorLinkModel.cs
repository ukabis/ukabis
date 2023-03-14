using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    public class VendorLinkModel
    {
        /// <summary>
        /// ベンダーリンクID
        /// </summary>
        public string VendorLinkId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// リンク表示名
        /// </summary>
        [Required(ErrorMessage = "タイトルは必須項目です")]
        [MaxLengthEx(MaxLength = 100, ErrorMessageFormat = "タイトルは{0}文字以内で入力して下さい")]
        public string LinkTitle { get; set; }

        /// <summary>
        /// リンク詳細
        /// </summary>
        [Required(ErrorMessage = "説明は必須項目です")]
        [MaxLengthEx(MaxLength = 1000, ErrorMessageFormat = "説明は{0}文字以内で入力して下さい")]
        public string LinkDetail { get; set; }

        /// <summary>
        /// リンクURL
        /// </summary>
        [Required(ErrorMessage = "URLは必須項目です")]
        [Url(ErrorMessage = "正しいURLではありません")]
        [MaxLengthEx(MaxLength = 512, ErrorMessageFormat = "URLは{0}文字以内で入力して下さい")]
        public string LinkUrl { get; set; }

        /// <summary>
        /// リンク表示フラグ
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// デフォルトフラグ
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// このインスタンスの簡易コピーを取得する。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public VendorLinkModel Clone()
        {
            return (VendorLinkModel)MemberwiseClone();
        }
    }
}
