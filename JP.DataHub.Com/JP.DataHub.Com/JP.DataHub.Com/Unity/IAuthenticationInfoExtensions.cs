using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Resolution;
using JP.DataHub.Com.Web.Authentication;

namespace JP.DataHub.Com.Unity
{
    public static class IAuthenticationInfoExtensions
    {
        public static ParameterOverride ToCI(this IAuthenticationInfo auth)
            => auth.ToCI<IAuthenticationInfo>();
    }
}
