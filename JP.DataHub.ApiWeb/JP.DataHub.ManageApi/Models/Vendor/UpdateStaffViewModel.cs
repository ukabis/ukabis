using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.Api.Core.Database;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class UpdateStaffViewModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        [RequiredGuid(ErrorMessage = "必須項目です。")]
        public string VendorId { get; set; }

        /// <summary>
        /// スタッフID
        /// </summary>
        [RequiredGuid(ErrorMessage = "必須項目です。")]
        public string StaffId { get; set; }

        /// <summary>
        /// OpenIDアカウント
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.Authority, AuthorityDatabase.TABLE_STAFF, AuthorityDatabase.COLUMN_STAFF_ACCOUNT)]
        public string Account { get; set; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        [EmailAddress(ErrorMessage = "メールアドレスの形式で入力してください")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 削除の有無
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
