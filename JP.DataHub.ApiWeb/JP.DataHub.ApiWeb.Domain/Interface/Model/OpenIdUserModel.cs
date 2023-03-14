
namespace JP.DataHub.ApiWeb.Domain.Interface.Model
{
    /// <summary>
    /// OpenIdユーザーのModel
    /// </summary>
    public class OpenIdUserModel
    {
        /// <summary>ユーザーID(電子メールアドレス)</summary>
        public string UserId { get; set; }

        /// <summary>パスワード</summary>
        public string Password { get; set; }

        /// <summary>表示名</summary>
        public string UserName { get; set; }

        /// <summary>作成日時</summary>
        public DateTime? CreatedDateTime { get; set; }

        /// <summary>OpenID</summary>
        public string OpenId { get; set; }
    }
}
