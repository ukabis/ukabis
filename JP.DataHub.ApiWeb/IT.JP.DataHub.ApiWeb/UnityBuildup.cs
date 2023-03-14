using Microsoft.Extensions.Configuration;
using Unity;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;

namespace IT.JP.DataHub.ApiWeb
{
    [UnityBuildup]
    internal class UnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            container.BuildupApiDifinition();

            var authenticationService = AuthenticationServiceFactory.Create(AuthenticationType.combination);
            container.RegisterInstance<IAuthenticationService>(authenticationService);
        }
    }
}
