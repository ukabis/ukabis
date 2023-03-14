using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JP.DataHub.Com.Validations.Attributes;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.Api.Core.Database;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class SendInvitationViewModel
    {
        /// <summary>
        /// 招待するベンダーのID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string VendorId { get; set; }

        /// <summary>
        /// ロールID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string RoleId { get; set; }

        /// <summary>
        /// 招待メールアドレス
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [EmailAddress]
        public string MailAddress { get; set; }
    }
}
