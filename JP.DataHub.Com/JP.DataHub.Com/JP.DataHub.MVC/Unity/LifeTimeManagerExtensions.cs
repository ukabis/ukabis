using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Interception.PolicyInjection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;

namespace JP.DataHub.MVC.Unity
{
    public static class LifeTimeManagerExtensions
    {
        public static ITypeLifetimeManager CreateLifeTimeManager(this string lifetime)
             => CreateInstanseLifeTimeManager(lifetime);

        public static ITypeLifetimeManager CreateInstanseLifeTimeManager(this string lifetime)
        {
            if (lifetime == "ContainerControlledLifetimeManager")
            {
                return new ContainerControlledLifetimeManager();
            }
            else if (lifetime == "PerResolveLifetimeManager")
            {
                return new PerResolveLifetimeManager();
            }
            else if (lifetime == "PerThreadLifetimeManager")
            {
                return new PerThreadLifetimeManager();
            }
            else if (lifetime == "PerRequestLifetimeManager")
            {
                return new PerRequestLifetimeManager();
            }

            //デフォルトはPerRequest
            return new PerRequestLifetimeManager();
        }
    }
}
