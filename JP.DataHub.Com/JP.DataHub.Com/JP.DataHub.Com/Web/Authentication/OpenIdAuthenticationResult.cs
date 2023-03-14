using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Consts;

namespace JP.DataHub.Com.Web.Authentication
{
    public class OpenIdAuthenticationResult : AbstractAuthenticationResult
    {
        public HttpResponseResult<TokenInfo> OpenId { get; set; }

        public override string ToString()
        {
            return $@"
{{
  'OpenIdToken' : {{
    'access_token' : '{OpenId.Result?.access_token}',
    'token_type' : '{OpenId.Result?.token_type}',
    'expires_in' : '{OpenId.Result?.expires_in}',
    'refresh_token' : '{OpenId.Result?.refresh_token}'
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
        }
    }
}
