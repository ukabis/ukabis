using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Consts;

namespace JP.DataHub.Com.Web.Authentication
{
    public class VendorAuthentication : IVendorAuthentication
    {
        public HttpResponseResult<TokenInfo> Authentication(VendorAuthenticationServerInfo vendorServerAuthenticationInfo, string client_id, string client_secret, string url = null)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            var id = client_id ?? vendorServerAuthenticationInfo.ClientId;
            var secret = client_secret ?? vendorServerAuthenticationInfo.ClientSecret;
            var tokenUrl = url ?? vendorServerAuthenticationInfo.Url;
            var contentStr = $"grant_type=client_credentials&client_id={id}&client_secret={secret}";
            return AuthenticationMisc.Post<TokenInfo>(tokenUrl, new MemoryStream(Encoding.UTF8.GetBytes(contentStr)), MediaTypeConst.MimeApplicationXWwwFormUrlencoded);
        }
    }
}
