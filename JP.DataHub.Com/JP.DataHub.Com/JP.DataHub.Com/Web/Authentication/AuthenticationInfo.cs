using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    public class AuthenticationInfo : AbstractAuthenticationInfo
    {
        public AuthenticationInfo()
        {
            Type = typeof(AuthenticationInfo).Name;
        }

        public OpenIdInfo OpenId { get; set; }
        public VendorInfo Vendor { get; set; }

        public override string ToString()
        {
            return $@"
{{
  'OpenId' : {{
    'Account' : '{OpenId.Account}',
    'Password' : '{OpenId.Password}'
  }},
  'Vendor' : {{
    'ClientId' : '{Vendor.ClientId}',
    'ClientSecret' : '{Vendor.ClientSecret}'
  }}
}}
";
        }

        public override void Setup(IServerEnvironment serverEnvironment)
        {
            throw new NotImplementedException();
        }
    }
}
