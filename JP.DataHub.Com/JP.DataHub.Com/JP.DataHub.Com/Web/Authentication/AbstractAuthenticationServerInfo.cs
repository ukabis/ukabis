using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public abstract class AbstractAuthenticationServerInfo : IAuthenticationServerInfo
    {
        public AuthenticationServerType Type { get; set; }
        public abstract void AddHeader(HttpRequestHeaders headers);
    }
}
