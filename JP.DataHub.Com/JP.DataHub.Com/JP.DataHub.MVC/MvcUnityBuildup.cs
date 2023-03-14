using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.MVC.Http;
using Microsoft.Extensions.Configuration;
using Unity;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.PolicyInjection;

namespace JP.DataHub.MVC
{
    [UnityBuildup]
    public class MvcUnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            container.RegisterType<IAuthenticationInfoSerializer, SessionAuthenticationInfoSerializer> (new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
        }
    }
}
