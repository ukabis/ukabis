using System;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class VendorOnlyViewModel
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
