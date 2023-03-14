namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class RegisterVendorResultViewModel
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
        /// データ提供か？
        /// </summary>
        public bool IsDataOffer { get; set; }

        /// <summary>
        /// データ利用か？
        /// </summary>
        public bool IsDataUse { get; set; }

        /// <summary>
        /// 有効状態
        /// </summary>
        public bool IsEnable { get; set; }
    }
}