using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    public class SendInvitationModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public Guid VendorId { get; set; }

        /// <summary>
        /// ロールID
        /// </summary>
        [Required(ErrorMessage = "権限は必須項目です。")]
        public Guid? RoleId { get; set; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        [Required(ErrorMessage = "メールアドレスは必須項目です。")]
        [EmailAddressAttribute(ErrorMessage = "メールアドレスの形式で入力してください。")]
        public string MailAddress { get; set; }
    }
}
