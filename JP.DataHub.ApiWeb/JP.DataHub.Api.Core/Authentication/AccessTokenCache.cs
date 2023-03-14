using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Api.Core.Authentication
{
    public class AccessTokenCache : UnityAutoInjection, IAccessTokenCache
    {
        [Dependency("AccessToken")]
        public ICache Cache { get; set; }

        private const string KEY = "AccessToken";

        public void Store(AccessToken accessToken)
        {
            Cache.Add($"{KEY}.{accessToken.AccessTokenId}", accessToken, accessToken.ExpirationTimeSpan);
        }

        public AccessToken Get(string accessTokenId)
        {
            return Cache.Get<AccessToken>($"{KEY}.{accessTokenId}", out bool isNullValue);
        }
    }
}
