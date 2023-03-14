using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public static class Converter
    {
        public static List<(AuthenticationType AuthenticationType, Type Info, Type Service, Type Result)> list = new List<(AuthenticationType, Type, Type, Type)>()
        {
            new( AuthenticationType.openidandvendor, typeof(AuthenticationInfo), typeof(AuthenticationService), typeof(AuthenticationResult) ),
            new( AuthenticationType.openid, typeof(OpenIdAuthenticationInfo), typeof(OpenIdAuthenticationService), typeof(OpenIdAuthenticationResult) ),
            new( AuthenticationType.vendor, typeof(VendorAuthenticationInfo), typeof(VendorAuthenticationService), typeof(VendorAuthenticationResult) ),
            new( AuthenticationType.nothing, typeof(NothingAuthenticationInfo), typeof(NothingAuthenticationService), typeof(NothingAuthenticationResult) ),
            new( AuthenticationType.combination, typeof(CombinationAuthenticationInfo), typeof(DefaultAuthenticationService), typeof(CombinationAuthenticationResult) ),
        };

        public static Type ToAuthenticationInfo(this AuthenticationType type)
            => list.First(x => x.AuthenticationType == type).Info;

        public static IAuthenticationService CreateAuthenticationService(this AuthenticationType type)
            => Activator.CreateInstance(list.First(x => x.AuthenticationType == type).Service) as IAuthenticationService;

        public static IAuthenticationService CreateAuthenticationServiceByAuthenticationInfo(this Type type)
            => Activator.CreateInstance(list.First(x => x.Info == type).Service) as IAuthenticationService;

        public static AuthenticationType AuthenticationInfoTypeToAuthenticationType(this Type type)
            => list.First(x => x.Info == type).AuthenticationType;

        public static Type ToAuthenticationService(this AuthenticationType type)
            => list.First(x => x.AuthenticationType == type).Service;
    }
}
