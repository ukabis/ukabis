using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.Api.Core.Database;

namespace JP.DataHub.ManageApi.Models.System
{
    public class SystemAdminViewModel
    {
        /// <summary>
        /// システムID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string SystemId { get; set; }

        /// <summary>
        /// 管理者認証キー
        /// </summary>
        [JpDataHubMaxLength(Domains.Authority, AuthorityDatabase.TABLE_SYSTEMADMIN, AuthorityDatabase.COLUMN_SYSTEMADMIN_ADMIN_SECRET)]
        public string AdminSecret { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool IsActive { get; set; }
    }
}
