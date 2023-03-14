
namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    /// <summary>
    /// 招待ユーザ情報
    /// </summary>
    public class AddInvitedUserModel
    {
        /// <summary>
        /// 招待ID
        /// </summary>
        public string InvitationId { get; set; }

        /// <summary>
        /// OpenID
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 招待メールアドレス
        /// </summary>
        public string MailAddress { get; set; }
    }
}
