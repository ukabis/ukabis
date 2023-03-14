using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unity;
using Unity.Interception;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.PolicyInjection;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.IT.Api.Com.WebApi;

namespace JP.DataHub.IT.Api.Com
{
    [UnityBuildup]
    public class ItApiComUnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
        }
    }
}
