namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class AccessVendorViewModel
    {
        /// <summary>
        /// AccessVendorId
        /// </summary>
        public string AccessVendorId { get; set; }

        /// <summary>
        /// VendorId
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// SystemId
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// アクセスキー
        /// </summary>
        public string AccessKey { get; set; }
    }
}
