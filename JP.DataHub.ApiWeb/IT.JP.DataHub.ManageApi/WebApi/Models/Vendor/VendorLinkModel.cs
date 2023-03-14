using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Vendor
{
    public class RegisterVendorLinkModel
    {
        public string VendorLinkId { get; set; }
        public string VendorId { get; set; }
        public string LinkTitle { get; set; }
        public string LinkDetail { get; set; }
        public string LinkUrl { get; set; }
        public bool IsVisible { get; set; }
        public bool IsDefault { get; set; }
    }
    public class VendorLinkModel
    {
        /// <summary>
        /// VendorリンクID
        /// </summary>
        public string VendorLinkId { get; set; }

        /// <summary>
        /// VendorId
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// リンク表示名
        /// </summary>
        public string LinkTitle { get; set; }

        /// <summary>
        /// リンク詳細
        /// </summary>
        public string LinkDetail { get; set; }

        /// <summary>
        /// リンクURL
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// リンク表示フラグ
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// デフォルトフラグ
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
