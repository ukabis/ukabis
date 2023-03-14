using System;

namespace JP.DataHub.ManageApi.Models.System
{
    public class ClientViewModel
    {
        /// <summary>
        /// クライアントID
        /// </summary>
        public Guid? ClientId { get; set; }

        /* クライアントシークレットは暗号化されており、そのまま出力しても長い文字列・セキュリティ的に宜しくないので出さない */

        /// <summary>
        /// 有効期限
        /// </summary>
        public string AccessTokenExpirationTimeSpan { get; set; }

        public string SystemId { get; set; }

        public bool IsActive { get; set; }
    }
}
