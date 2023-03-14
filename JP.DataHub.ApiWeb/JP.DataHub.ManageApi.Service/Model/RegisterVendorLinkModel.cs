using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class RegisterVendorLinkModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set;
        }
        /// <summary>
        /// VendorリンクID(指定した場合は対象のベンダーリンクを更新し、未指定の場合は新規登録)
        /// </summary>
        public string VendorLinkId { get; set; }

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
    }
}
