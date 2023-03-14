using JP.DataHub.Api.Core.Database;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace JP.DataHub.ManageApi.Models.ResourceSharing
{
    /// <summary>
    /// データ利用許可ビューモデル
    /// </summary>
    public class RegisterResourceSharingViewModel
    {
        /// <summary>
        /// データ所有ベンダーID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid SharingFromVendorId { get; set; }

        /// <summary>
        /// データ所有システムID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid SharingFromSystemId { get; set; }

        /// <summary>
        /// ApiID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid ApiId { get; set; }

        /// <summary>
        /// データ公開対象ベンダーID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid SharingToVendorId { get; set; }

        /// <summary>
        /// データ公開対象システムID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid SharingToSystemId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_RESOURCESHARINGRULE, DynamicApiDatabase.COLUMN_RESOURCESHARINGRULE_RESOURCE_SHARING_RULE_NAME)]

        public string ResourceSharingRuleName { get; set; }

        /// <summary>
        /// 条件クエリ
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [AllowHtml]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_RESOURCESHARINGRULE, DynamicApiDatabase.COLUMN_RESOURCESHARINGRULE_QUERY)]
        public string Query { get; set; }

        /// <summary>
        /// 条件スクリプト
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_RESOURCESHARINGRULE, DynamicApiDatabase.COLUMN_RESOURCESHARINGRULE_ROSLYN_SCRIPT)]
        public string RoslynScript { get; set; }
        /// <summary>
        /// 有効
        /// </summary>
        public bool IsEnable { get; set; } = true;
    }
}
