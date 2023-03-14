using JP.DataHub.Api.Core.Database;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ApiWeb.Models.MailTemplate
{
    public class RegisterMailTemplateViewModel
    {
        /// <summary>ベンダーID</summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid VendorId { get; set; }

        /// <summary>メールテンプレート名</summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_VENDORMAILTEMPLATE, DynamicApiDatabase.COLUMN_VENDORMAILTEMPLATE_MAIL_TEMPLATE_NAME)]
        public string MailTemplateName { get; set; }

        /// <summary>Fromメールアドレス</summary>
        [EmailAddress]
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_VENDORMAILTEMPLATE, DynamicApiDatabase.COLUMN_VENDORMAILTEMPLATE_FROM_MAILADDRESS)]
        public string From { get; set; }

        /// <summary>Toメールアドレス</summary>
        [JpDataHubMailAddresses]
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_VENDORMAILTEMPLATE, DynamicApiDatabase.COLUMN_VENDORMAILTEMPLATE_TO_MAILADDRESS)]
        public string[] To { get; set; }

        /// <summary>CCメールアドレス</summary>
        [JpDataHubMailAddresses]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_VENDORMAILTEMPLATE, DynamicApiDatabase.COLUMN_VENDORMAILTEMPLATE_CC_MAILADDRESS)]
        public string[] Cc { get; set; }

        /// <summary>BCCメールアドレス</summary>
        [JpDataHubMailAddresses]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_VENDORMAILTEMPLATE, DynamicApiDatabase.COLUMN_VENDORMAILTEMPLATE_BCC_MAILADDRESS)]
        public string[] Bcc { get; set; }

        /// <summary>メールのタイトル</summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_VENDORMAILTEMPLATE, DynamicApiDatabase.COLUMN_VENDORMAILTEMPLATE_TITLE)]
        public string Subject { get; set; }

        /// <summary>メールの内容</summary>
        [Required(ErrorMessage = "必須項目です。")]
        public string Body { get; set; }
    }
}