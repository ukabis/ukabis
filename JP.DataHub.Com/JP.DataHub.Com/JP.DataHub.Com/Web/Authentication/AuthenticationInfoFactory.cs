using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Web.Authentication
{
    public static class AuthenticationInfoFactory
    {
        public static IAuthenticationInfo Create(IServerEnvironment serverEnvironment)
        {
            var result = Activator.CreateInstance(serverEnvironment.Parent.AuthenticationType.ToAuthenticationInfo()) as IAuthenticationInfo;
            result.Setup(serverEnvironment);
            return result as IAuthenticationInfo;
        }

        public static IAuthenticationInfo Create(ServerEnvironment serverEnvironment, string fileName)
        {
            var result = fileName.FileToJson(serverEnvironment.Parent.AuthenticationType.ToAuthenticationInfo()) as IAuthenticationInfo;
            result.AuthenticationInfoId = serverEnvironment.EnvironmentId;
            result.Environment = serverEnvironment;
            return result;
        }
    }
}
