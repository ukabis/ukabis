namespace IT.JP.DataHub.ManageApi.WebApi.Models.System
{
    public class UpdateClientModel
    {
        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// クライアントID
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>クライアントシークレット</summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// 有効期限
        /// </summary>
        public string AccessTokenExpirationTimeSpan { get; set; }
    }
}
