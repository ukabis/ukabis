using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Consts;

namespace JP.DataHub.Com.Web.Authentication
{
    public class VendorAuthenticationResult : AbstractAuthenticationResult
    {
        public HttpResponseResult<TokenInfo> Vendor { get; set; }

        public override string ToString()
        {
            return $@"
{{
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
            if (Vendor?.Result?.access_token != null)
            {
                headers.Add(HeaderConst.XAuthorization, Vendor?.Result?.access_token);
            }
        }
    }
}
