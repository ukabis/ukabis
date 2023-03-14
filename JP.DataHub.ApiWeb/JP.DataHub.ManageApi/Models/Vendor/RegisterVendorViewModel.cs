using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.Com.Validations.Annotations;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    /// <summary>
    /// ベンダー情報
    /// </summary>
    public class RegisterVendorViewModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        [ValidateGuid]
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.Authority, "Vendor", "vendor_name")]
        public string VendorName { get; set; }

        /// <summary>
        /// 代表者メールアドレス
        /// </summary>
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
