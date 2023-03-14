using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.Api.Core.Database;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class AddInvitedUserViewModel
    {
        /// <summary>
        /// 招待ID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string InvitationId { get; set; }

        /// <summary>
        /// OpenID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.Authority, AuthorityDatabase.TABLE_STAFF, AuthorityDatabase.COLUMN_STAFF_ACCOUNT)]
        public string OpenId { get; set; }

        /// <summary>
        /// 招待メールアドレス
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [EmailAddress]
        public string MailAddress { get; set; }
    }
}
