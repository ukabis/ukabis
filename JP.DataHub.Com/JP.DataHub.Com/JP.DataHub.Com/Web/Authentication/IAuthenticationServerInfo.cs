using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    [JsonConverter(typeof(IAuthenticationServerInfoConverter))]
    public interface IAuthenticationServerInfo
    {
        AuthenticationServerType Type { get; set; }
        void AddHeader(HttpRequestHeaders headers);
    }
}
