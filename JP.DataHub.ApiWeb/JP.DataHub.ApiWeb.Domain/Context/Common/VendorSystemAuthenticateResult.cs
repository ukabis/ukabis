using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record VendorSystemAuthenticateResult : IValueObject
    {
        public bool AuthenticationSuccess { get; }

        public string AccessTokenId { get; }

        public TimeSpan ExpirationDate { get; }

        public VendorSystemAuthenticateResult(bool authenticationSuccess, string accessTokenId, TimeSpan expirationDate)
        {
            AuthenticationSuccess = authenticationSuccess;
            AccessTokenId = accessTokenId;
            ExpirationDate = expirationDate;
        }

        public static bool operator ==(VendorSystemAuthenticateResult me, object other) => me?.Equals(other) == true;

        public static bool operator !=(VendorSystemAuthenticateResult me, object other) => !me?.Equals(other) == true;
    }
}
