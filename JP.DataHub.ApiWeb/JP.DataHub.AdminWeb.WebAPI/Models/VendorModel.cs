
using System.ComponentModel.DataAnnotations;
using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    /// <summary>
    /// ベンダー情報
    /// </summary>
    public class VendorModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        [Required(ErrorMessage = "ベンダー名は必須項目です。")]
        [MaxLengthEx(MaxLength = 100, ErrorMessageFormat = "ベンダー名は{0}文字以内で入力して下さい。")]
        public string VendorName { get; set; }

        /// <summary>
        /// 代表者メールアドレス
        /// </summary>
        [EmailAddressEx]
        [MaxLengthEx(MaxLength = 260, ErrorMessageFormat = "代表メールアドレスは{0}文字以内で入力して下さい。")]
        public string RepresentativeMailAddress { get; set; }

        /// <summary>
        /// データ提供者
        /// </summary>
        public bool IsDataOffer { get; set; }

        /// <summary>
        /// データ利用者
        /// </summary>
        public bool IsDataUse { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// VendorLink一覧
        /// </summary>
        [LinkDefaultCount]
        [ValidateComplexType]
        public List<VendorLinkModel> VendorLinkList { get; set; }
        /// <summary>
        /// 添付ファイルストレージID
        /// </summary>
        public string AttachFileStorageId { get; set; }
        /// <summary>
        /// 添付ファイルストレージリスト
        /// </summary>
        public List<AttachFileStorageModel> AttachFileStorage { get; set; }

        /// <summary>
        /// OpenID認証局リスト
        /// </summary>
        public List<VendorOpenIdCaModel> OpenIdCaList { get; set; }

        /// <summary>
        /// システム一覧
        /// </summary>
        public List<SystemModel> SystemList { get; set; }

        /// <summary>
        /// スタッフ一覧
        /// </summary>
        [ValidateComplexType]
        public List<StaffModel> StaffList { get; set; }

        public class SystemModel
        {
            /// <summary>
            /// システムID
            /// </summary>
            public string SystemId { get; set; }
            /// <summary>
            /// システム名
            /// </summary>
            public string SystemName { get; set; }
        }
    }
}
