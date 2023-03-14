using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    [JsonConverter(typeof(IAuthenticationInfoConverter))]
    public interface IAuthenticationInfo
    {
        string AuthenticationInfoId { get; set; }
        bool HasAuthenticationResult { get; }
        IAuthenticationResult AuthenticationResult { get; set; }

        ServerEnvironment Environment { get; set; }

        void ClearAuthenticationResult();

        string ToString();

        void Setup(IServerEnvironment serverEnvironment);

        IAuthenticationInfo Merge(params IAuthenticationInfo[] authenticationInfo);

        AuthenticationType ToAuthenticationType();
        IAuthenticationService GetAuthenticationService();
    }
}
