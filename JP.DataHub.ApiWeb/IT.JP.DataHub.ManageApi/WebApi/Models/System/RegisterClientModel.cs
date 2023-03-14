using System;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.System
{
    public class RegisterClientModel
    {
        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>クライアントシークレット</summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// 有効期限
        /// </summary>
        public string AccessTokenExpirationTimeSpan { get; set; }
    }
}
