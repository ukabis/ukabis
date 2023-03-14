using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    /// <summary>
    /// ベンダー情報
    /// </summary>
    public class VendorSimpleViewModel
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
        /// システム一覧
        /// </summary>
        public List<VendorSystemViewModel> SystemList { get; set; }
    }
}
