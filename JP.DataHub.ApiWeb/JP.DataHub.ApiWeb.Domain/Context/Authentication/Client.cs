using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Authentication;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Authentication
{
    internal class Client : IEntity
    {
        /// <summary>クライアントID</summary>
        public ClientId ClientId { get; set; }

        /// <summary>クライアントシークレット</summary>
        public ClientSecret ClientSecret { get; set; }

        /// <summary>システムID</summary>
        public SystemId SystemId { get; set; }

        /// <summary>アクセストークンの有効期限</summary>
        public AccessTokenExpirationTimeSpan AccessTokenExpirationTimeSpan { get; set; }

        public VendorId VendorId { get; set; }


        private AccessTokenId accessTokenId;


        public bool VerificationClientSecret(ClientSecretVO clientSecret)
        {
            var result = ClientSecret.Value.Equals(clientSecret.Value);
            if (!result)
            {
            //    var domainEvent = new VendorSystemAuthenticationEventData(DateTime.UtcNow, this.ClientId, new AccessTokenId(Guid.Empty), clientIpaddress, VendorId, SystemId, new IsSuccess(false));
            //    //publisher.Publish<VendorSystemAuthenticationEventData>(domainEvent);
            }
            else
            {
                accessTokenId = new AccessTokenId(Guid.NewGuid());
            //    var domainEvent = new VendorSystemAuthenticationEventData(DateTime.UtcNow, this.ClientId, accessTokenId, clientIpaddress, VendorId, SystemId, new IsSuccess(true));
            //    //publisher.Publish<VendorSystemAuthenticationEventData>(domainEvent);
            }
            return result;
        }

        public AccessToken CreateAccessToken()
        {
            return new AccessToken { ClientId = this.ClientId.Value, SystemId = this.SystemId.Value, AccessTokenId = accessTokenId.Value, ExpirationTimeSpan = this.AccessTokenExpirationTimeSpan.Value.Add(TimeSpan.FromMinutes(5)) };
        }

        public static Client Create(ClientId clientId, ClientSecret clientSecret = null, SystemId systemId = null, AccessTokenExpirationTimeSpan accessTokenExpirationTimeSpan = null, VendorId vendorId = null)
        {
            if (clientId?.Value == null)
            {
                clientId = new ClientId(Guid.NewGuid());
            }
            return new Client() { ClientId = clientId, ClientSecret = clientSecret, SystemId = systemId, AccessTokenExpirationTimeSpan = accessTokenExpirationTimeSpan, VendorId = vendorId };
        }
    }
}
