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
    public class ResourceSharingViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid ResourceSharingRuleId { get; set; }

        /// <summary>
        /// データ所有ベンダーID
        /// </summary>
        public Guid SharingFromVendorId { get; set; }

        /// <summary>
        /// データ所有システムID
        /// </summary>
        public Guid SharingFromSystemId { get; set; }

        /// <summary>
        /// ApiID
        /// </summary>
        public Guid ApiId { get; set; }

        /// <summary>
        /// データ公開対象ベンダーID
        /// </summary>
        public Guid SharingToVendorId { get; set; }

        /// <summary>
        /// データ公開対象システムID
        /// </summary>
        public Guid SharingToSystemId { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string ResourceSharingRuleName { get; set; }

        /// <summary>
        /// 条件クエリ
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// 条件スクリプト
        /// </summary>
        public string RoslynScript { get; set; }

        /// <summary>
        /// 有効
        /// </summary>
        public bool IsEnable { get; set; } = true;
    }
}
