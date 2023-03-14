using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Com.Web.Authentication;

namespace JP.DataHub.Com.Web.Authentication
{
    internal class IAuthenticationServerInfoConverter : InterfaceJsonConverter<IAuthenticationServerInfo>
    {
        public IAuthenticationServerInfoConverter()
        {
            TypePropertyName = "Type";
            Mapping.Add(nameof(AuthenticationServerType.OpenId), typeof(OpenIdAuthenticationServerInfo));
            Mapping.Add(nameof(AuthenticationServerType.Vendor), typeof(VendorAuthenticationServerInfo));
        }
    }
}
