using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using MessagePack;

namespace JP.DataHub.Api.Core.Authentication
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class AccessToken
    {
        public string AccessTokenId { get; set; }

        /// <summary>クライアントID</summary>
        public string ClientId { get; set; }

        /// <summary>システムID</summary>
        public string SystemId { get; set; }

        /// <summary>アクセストークンの有効期限</summary>
        public TimeSpan ExpirationTimeSpan { get; set; }
    }
}
