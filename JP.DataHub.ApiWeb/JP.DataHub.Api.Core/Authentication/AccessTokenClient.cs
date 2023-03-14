using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Authentication
{
    public class AccessTokenClient
    {
        /// <summary>クライアントID</summary>
        public string ClientId { get; set; }

        /// <summary>クライアントシークレット</summary>
        public string ClientSecret { get; set; }

        /// <summary>ベンダーID</summary>
        public string VendorId { get; set; }

        /// <summary>システムID</summary>
        public string SystemId { get; set; }

        /// <summary>アクセストークンの有効期限</summary>
        public TimeSpan ExpirationTimeSpan { get; set; }

        private string accessTokenId = null;

        public AccessTokenClient(string clientId, string clientSecret, string vendorId, string systemId, TimeSpan expirationTimeSpan)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            VendorId = vendorId;
            SystemId = systemId;
            ExpirationTimeSpan = expirationTimeSpan;
        }

        public bool VerificationClientSecret(ClientSecret clientSecret)
        {
            var result = ClientSecret.Equals(clientSecret.Value);
            if (result)
            {
                accessTokenId = Guid.NewGuid().ToString();
            }
            return result;
        }

        public AccessToken CreateAccessToken()
        {
            return new AccessToken { ClientId = this.ClientId, SystemId = this.SystemId, AccessTokenId = accessTokenId, ExpirationTimeSpan = this.ExpirationTimeSpan.Add(TimeSpan.FromMinutes(5)) };
        }
    }
}
