using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.Api.Core.Database;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class RegisterVendorLinkViewModel
    {
        /// <summary>
        /// ベンダーリンクID
        /// </summary>
        [ValidateGuid]
        public string VendorLinkId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        [RequiredGuid(ErrorMessage = "必須項目です。")]
        public string VendorId { get; set; }

        /// <summary>
        /// リンク表示名
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_VENDORLINK, DynamicApiDatabase.COLUMN_VENDORLINK_TITLE)]
        public string LinkTitle { get; set; }

        /// <summary>
        /// リンク詳細
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_VENDORLINK, DynamicApiDatabase.COLUMN_VENDORLINK_DETAIL)]
        public string LinkDetail { get; set; }

        /// <summary>
        /// リンクURL
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [Url(ErrorMessage = "正しいURLではありません。")]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_VENDORLINK, DynamicApiDatabase.COLUMN_VENDORLINK_URL)]
        public string LinkUrl { get; set; }

        /// <summary>
        /// リンク表示フラグ
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// デフォルトフラグ
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
