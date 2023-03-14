using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    /// <summary>
    /// ベンダー情報のみのViewModel
    /// </summary>
    public class VendorOnlyModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public Guid VendorId { get; set; }

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
    }
}
