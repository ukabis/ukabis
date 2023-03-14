using System;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class AddStafforResultViewModel
    {
        /// <summary>
        /// スタッフID
        /// </summary>
        public string StaffId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public Guid VendorId { get; set; }

        /// <summary>
        /// OpenIDアカウント
        /// </summary>
        public string Account { get; set; }
    }
}
