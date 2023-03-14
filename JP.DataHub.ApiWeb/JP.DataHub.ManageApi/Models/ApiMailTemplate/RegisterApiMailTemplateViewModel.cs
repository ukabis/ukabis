using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ApiWeb.Models.ApiMailTemplate
{
    public class RegisterApiMailTemplateViewModel
    {
        /// <summary>API ID</summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid ApiId { get; set; }

        /// <summary>ベンダーID</summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid VendorId { get; set; }

        /// <summary>メールテンプレートID</summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid MailTemplateId { get; set; }

        /// <summary>登録時通知フラグ</summary>
        [Required(ErrorMessage = "必須項目です。")]
        public bool NotifyRegister { get; set; }

        /// <summary>更新時通知フラグ</summary>
        [Required(ErrorMessage = "必須項目です。")]
        public bool NotifyUpdate { get; set; }

        /// <summary>削除時通知フラグ</summary>
        [Required(ErrorMessage = "必須項目です。")]
        public bool NotifyDelete { get; set; }
    }
}