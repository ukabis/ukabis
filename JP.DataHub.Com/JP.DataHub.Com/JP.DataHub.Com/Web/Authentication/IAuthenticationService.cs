using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.Com.Web.Authentication
{
    public interface IAuthenticationService
    {
        IAuthenticationResult Authentication(IServerEnvironment serverEnvironment, IAuthenticationInfo authenticationInfo);
    }
}
