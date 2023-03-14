namespace IT.JP.DataHub.ManageApi.WebApi.Models.System
{
    public class UpdateSystemModel
    {
        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// OpenId認証のアプリケーションID
        /// </summary>
        public string OpenIdApplicationId { get; set; }

        /// <summary>
        /// OpenId認証のクライアントシークレット
        /// </summary>
        public string OpenIdClientSecret { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; } = true;
    }
}
