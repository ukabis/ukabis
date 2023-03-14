using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Attributes
{
    internal enum AuthenticationType
    {
        Nothing = 0,
        OpenId = 1,
        Vendor = 2,
        All = 255,
    }

    internal static class AuthenticationTypeExtensions
    {
        public static bool IsCheck(this AuthenticationType authentication, AuthenticationType check)
        {
            return ((int)authentication & (int)check) == (int)check;
        }
    }
}
