using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public interface IOpenIdAuthentication
    {
        HttpResponseResult<TokenInfo> Authentication(OpenIdAuthenticationServerInfo openIdServerAuthenticationInfo, string account, string password);
    }
}
