using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.Api.Core.Database;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class AddStafforViewModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        [RequiredGuid(ErrorMessage = "必須項目です。")]
        public string VendorId { get; set; }

        /// <summary>
        /// OpenIDアカウント
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.Authority, AuthorityDatabase.TABLE_STAFF, AuthorityDatabase.COLUMN_STAFF_ACCOUNT)]
        public string Account { get; set; }

        public string StaffId { get; set; }

        /// <summary>
        /// EmailAddress
        /// </summary>
        public string EmailAddress { get; set; }


        public string StaffRoleId { get; set; }

        /// <summary>
        /// RoleId
        /// </summary>
        [RequiredGuid(ErrorMessage = "必須項目です。")]
        public string RoleId { get; set; }
    }
}
