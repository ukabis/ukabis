using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using JP.DataHub.Com.Consts;

namespace JP.DataHub.Com.Web.Authentication
{
    public class OpenIdAuthentication : IOpenIdAuthentication
    {
        public HttpResponseResult<TokenInfo> Authentication(OpenIdAuthenticationServerInfo openIdServerAuthenticationInfo, string account, string password)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            string content = $"grant_type=password&username={HttpUtility.UrlEncode(account)}&password={password}&scope={openIdServerAuthenticationInfo.Scope}&client_id={openIdServerAuthenticationInfo.ClientId}&client_secret={openIdServerAuthenticationInfo.ClientSecret}&response_type=token+id_token";
            return AuthenticationMisc.Post<TokenInfo>(openIdServerAuthenticationInfo.Url, new MemoryStream(Encoding.UTF8.GetBytes(content)), MediaTypeConst.MimeApplicationXWwwFormUrlencoded);
        }
    }
}
