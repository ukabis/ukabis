using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public class OpenIdAuthenticationServerInfo : AbstractAuthenticationServerInfo
    {
        public string Url { get; set; }

        public string Scope { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public OpenIdAuthenticationServerInfo()
        {
            Type = AuthenticationServerType.OpenId;
        }

        public override void AddHeader(HttpRequestHeaders headers)
        {
            // nothing...
        }
    }
}
