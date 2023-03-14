using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    /// <summary>
    /// ベンダー情報
    /// </summary>
    public class VendorViewModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 代表者メールアドレス
        /// </summary>
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
        public List<VendorLinkViewModel> VendorLinkList { get; set; }

        /// <summary>
        /// 添付ファイルストレージID
        /// </summary>
        public string AttachFileStorageId { get; set; }

        /// <summary>
        /// 添付ファイルストレージリスト
        /// </summary>
        public List<AttachFileStorageViewModel> AttachFileStorage { get; set; }

        /// <summary>
        /// OpenID認証局リスト
        /// </summary>
        public List<OpenIdCaViewModel> OpenIdCaList { get; set; }

        /// <summary>
        /// システム一覧
        /// </summary>
        public List<VendorSystemViewModel> SystemList { get; set; }

        /// <summary>
        /// スタッフ一覧
        /// </summary>
        public List<StaffViewModel> StaffList { get; set; }
    }
}
