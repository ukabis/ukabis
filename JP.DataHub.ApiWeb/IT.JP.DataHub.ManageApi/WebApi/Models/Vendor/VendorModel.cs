using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Vendor
{
    public class UpdateVendorModel
    {
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public bool IsDataOffer { get; set; }
        public bool IsDataUse { get; set; }
        public bool IsEnable { get; set; }
    }
    public class RegisterVendorModel
    {
        public string VendorName { get; set; }
        public bool IsDataOffer { get; set; }
        public bool IsDataUse { get; set; }
        public bool IsEnable { get; set; }
    }
    public class RegisterVendorResultModel
    {
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public bool IsDataOffer { get; set; }
        public bool IsDataUse { get; set; }
        public bool IsEnable { get; set; }
    }
    public class VendorModel
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
        public List<VendorLinkModel> VendorLinkList { get; set; }

        /// <summary>
        /// 添付ファイルストレージID
        /// </summary>
        public string AttachFileStorageId { get; set; }

        /// <summary>
        /// OpenID認証局リスト
        /// </summary>
        public List<OpenIdCaModel> OpenIdCaList { get; set; }

        /// <summary>
        /// システム一覧
        /// </summary>
        public List<VendorSystemModel> SystemList { get; set; }

        /// <summary>
        /// スタッフ一覧
        /// </summary>
        public List<StaffModel> StaffList { get; set; }
    }
}
