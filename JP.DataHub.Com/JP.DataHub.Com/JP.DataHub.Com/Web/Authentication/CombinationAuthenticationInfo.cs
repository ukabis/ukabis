using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    public class CombinationAuthenticationInfo : List<IAuthenticationInfo>, IAuthenticationInfo
    {
        public string Type { get; set; }

        public string AuthenticationInfoId { get; set; }

        public bool HasAuthenticationResult { get => AuthenticationResult != null; }
        public ServerEnvironment Environment { get; set; }
        public IAuthenticationResult AuthenticationResult { get; set; }

        public CombinationAuthenticationInfo()
        {
        }

        public CombinationAuthenticationInfo(IEnumerable<IAuthenticationInfo> authenticationInfos)
        {
            foreach (var a in authenticationInfos)
            {
                this.Add(a);
            }
        }

        public IAuthenticationInfo Merge(params IAuthenticationInfo[] authenticationInfo)
        {
            foreach (var a in authenticationInfo)
            {
                this.Add(a);
            }
            return this;
        }

        public void ClearAuthenticationResult()
        {
            AuthenticationResult = null;
        }

        public new string ToString()
        {
            return null;
        }

        public void Setup(IServerEnvironment serverEnvironment)
        {
        }

        public AuthenticationType ToAuthenticationType()
             => GetType().AuthenticationInfoTypeToAuthenticationType();

        public IAuthenticationService GetAuthenticationService()
            => GetType().CreateAuthenticationServiceByAuthenticationInfo();
    }
}
