using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest.Attributes;

namespace JP.DataHub.Com.Web.Authentication
{
    public enum AuthenticationServerType
    {
        [Concrete(typeof(OpenIdAuthenticationServerInfo))]
        OpenId,
        [Concrete(typeof(OpenIdAuthenticationServerInfo))]
        Vendor
    }
}
