using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public interface IVendorAuthentication
    {
        HttpResponseResult<TokenInfo> Authentication(VendorAuthenticationServerInfo vendorServerAuthenticationInfo, string client_id, string client_secret, string url = null);
    }
}
