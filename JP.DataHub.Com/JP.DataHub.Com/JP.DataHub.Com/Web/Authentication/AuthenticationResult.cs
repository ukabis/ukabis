using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Authentication;

namespace JP.DataHub.Com.Web.Authentication
{
    public class AuthenticationResult : AbstractAuthenticationResult
    {
        public HttpResponseResult<TokenInfo> OpenId { get; set; }
        public Jwt OpenIdJwt { get; set; }
        public HttpResponseResult<TokenInfo> Vendor { get; set; }

        public override string ToString()
        {
            return $@"
{{
  'OpenIdToken' : {{
    'access_token' : '{OpenId.Result?.access_token}',
    'token_type' : '{OpenId.Result?.token_type}',
    'expires_in' : '{OpenId.Result?.expires_in}',
    'refresh_token' : '{OpenId.Result?.refresh_token}'
  }},
  'VendorToken' : {{
    'access_token' : '{Vendor.Result?.access_token}',
    'token_type' : '{Vendor.Result?.token_type}',
    'expires_in' : '{Vendor.Result?.expires_in}',
    'refresh_token' : '{Vendor.Result?.refresh_token}'
  }}
}}
";
        }

        public override void AddHeader(HttpRequestHeaders headers)
        {
            if (OpenId?.Result?.access_token != null)
            {
                headers.Authorization = new AuthenticationHeaderValue(HeaderConst.Bearer, OpenId?.Result?.access_token);
            }
            if (Vendor?.Result?.access_token != null)
            {
                headers.Add(HeaderConst.XAuthorization, Vendor?.Result?.access_token);
            }
        }
    }
}
