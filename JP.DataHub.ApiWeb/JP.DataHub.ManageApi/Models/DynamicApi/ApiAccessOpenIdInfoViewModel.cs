namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class ApiAccessOpenIdInfoViewModel
    {
        /// <summary>
        /// ApiAccessOpenId
        /// </summary>
        public string ApiAccessOpenId { get; set; }

        /// <summary>
        /// ApiId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// アクセスキー
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
