using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    public class DefaultAuthenticationService : IAuthenticationService
    {
        public IAuthenticationResult Authentication(IServerEnvironment serverEnvironment, IAuthenticationInfo authenticationInfo)
        {
            if (authenticationInfo is CombinationAuthenticationInfo cai)
            {
                var result = new CombinationAuthenticationResult();
                foreach (var authinfo in cai)
                {
                    var service = authinfo.GetType().CreateAuthenticationServiceByAuthenticationInfo();
                    var authresult = service.Authentication(serverEnvironment, authinfo);
                    result.Add(authresult);
                }
                result.Info = authenticationInfo;
                return result;
            }
            return null;
        }
    }
}
