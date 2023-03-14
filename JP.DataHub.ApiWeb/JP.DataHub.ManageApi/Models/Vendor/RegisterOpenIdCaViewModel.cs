using JP.DataHub.ManageApi.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class RegisterOpenIdCaViewModel
    {
        [RequiredGuid(ErrorMessage = "必須項目です。")]
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダーOpenId認証局ID
        /// </summary>
        [ValidateGuid]
        public string VendorOpenidCaId { get; set; }

        /// <summary>
        /// アプリケーションID
        /// </summary>
        [RequiredGuid(ErrorMessage = "必須項目です。")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// アクセス制御（alw:許可 / dny:拒否 / inh:継承）
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [RegularExpression("(^alw$|^dny$|^inh$)")]
        public string AccessControl { get; set; }
    }
}
