using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Api.Core.Authentication;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Serializable]
    public class VendorAccessTokenResult
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public long ExpiresIn { get; set; }

        public VendorAccessTokenResult()
        {
        }

        public VendorAccessTokenResult(AccessToken accessToken)
        {
            AccessToken = accessToken.AccessTokenId;
        }
    }
}
