using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
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
