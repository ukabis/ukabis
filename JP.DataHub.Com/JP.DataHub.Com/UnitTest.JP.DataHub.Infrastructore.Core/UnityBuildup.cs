using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unity;
using JP.DataHub.Com.Unity;
using Unity.Interception;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.PolicyInjection;

namespace UnitTest.JP.DataHub.Infrastructore.Core
{
    [UnityBuildup]
    class UnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
        }
    }
}
