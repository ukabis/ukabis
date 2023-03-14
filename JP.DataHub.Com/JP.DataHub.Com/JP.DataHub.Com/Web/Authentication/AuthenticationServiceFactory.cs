using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public static class AuthenticationServiceFactory
    {
        public static IAuthenticationService Create(AuthenticationType name)
            => Activator.CreateInstance(name.ToAuthenticationService()) as IAuthenticationService;
    }
}
