using JP.DataHub.Com.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Microsoft.Extensions.Configuration;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.PolicyInjection;
using JP.DataHub.MVC.Storage;
using JP.DataHub.Blazor.Core.Storage;

namespace JP.DataHub.Blazor.Core
{
    [UnityBuildup]
    public class BlazorUnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            container.RegisterType<IStorage, DataHubBlazorCoreSessionStorage>(new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>());
        }
    }
}
