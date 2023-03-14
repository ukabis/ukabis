using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.Api.Core.Database;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class UpdateVendorViewModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        [RequiredGuid(ErrorMessage = "必須項目です。")]
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.Authority, DocumentDatabase.TABLE_VENDOR, DocumentDatabase.COLUMN_VENDOR_VENDOR_NAME)]
        public string VendorName { get; set; }

        public string RepresentativeMailAddress { get; set; }

        /// <summary>
        /// データ提供か？
        /// </summary>
        public bool IsDataOffer { get; set; }

        /// <summary>
        /// データ利用か？
        /// </summary>
        public bool IsDataUse { get; set; }

        /// <summary>
        /// 有効状態
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
