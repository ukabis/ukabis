using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    class NothingAuthenticationService : IAuthenticationService
    {
        public NothingAuthenticationService()
        {
        }

        public IAuthenticationResult Authentication(IServerEnvironment serverEnvironment, IAuthenticationInfo authenticationInfo)
        {
            return null;
        }

        public IAuthenticationResult Authentication(List<IAuthenticationServerInfo> serverAuthentications, IAuthenticationInfo authenticationInfo)
        {
            return null;
        }
    }
}
