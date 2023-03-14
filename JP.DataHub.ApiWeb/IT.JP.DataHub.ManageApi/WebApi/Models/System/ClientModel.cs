using System;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.System
{
    public class ClientModel
    {
        /// <summary>
        /// クライアントID
        /// </summary>
        public string ClientId { get; set; }

        /* クライアントシークレットは暗号化されており、そのまま出力しても長い文字列・セキュリティ的に宜しくないので出さない */

        /// <summary>
        /// 有効期限
        /// </summary>
        public string AccessTokenExpirationTimeSpan { get; set; }
    }
}
