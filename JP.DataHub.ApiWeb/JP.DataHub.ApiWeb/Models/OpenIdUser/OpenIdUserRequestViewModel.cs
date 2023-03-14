using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ApiWeb.Models.OpenIdUser
{
    /// <summary>
    /// OpenIDユーザー情報
    /// </summary>
    public class OpenIdUserRequestViewModel
    {
        /// <summary>
        /// ユーザーID(電子メールアドレス)
        /// </summary>
        [DisplayName("ユーザーID(電子メールアドレス)")]
        [Required]
        [EmailAddress]
        public string UserId { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        [DisplayName("パスワード")]
        [Required]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)|(?=.*[a-z])(?=.*[A-Z])(?=.*[^A-Za-z0-9])|(?=.*[a-z])(?=.*\d)(?=.*[^A-Za-z0-9])|(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]))([A-Za-z\d@#$%^&*\-_+=[\]{}|\\:',?/`~""();!]|\.(?!@)){8,16}$",
            ErrorMessage = @"小文字、大文字、数字 (0-9)、記号という 4 つのカテゴリのうち 3 つを含む、8 文字から 16 文字です。記号は、次の 1 つ以上を含めます: @ # $ % ^ & * - _ + = [ ] {{ }} | \ : ' , ?/ ` ~ "" ( ) ; .")]
        public string Password { get; set; }

        /// <summary>
        /// 表示名
        /// </summary>
        [DisplayName("表示名")]
        [Required]
        public string UserName { get; set; }
    }
}