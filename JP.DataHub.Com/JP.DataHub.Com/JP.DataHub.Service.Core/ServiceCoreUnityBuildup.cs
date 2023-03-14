using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Service.Core;
using JP.DataHub.Service.Core.Impl;

namespace JP.DataHub.Service
{
    [UnityBuildup]
    public class ServiceCoreUnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            container.RegisterType<ICommonCrudService, CommonCrudService>();
            container.RegisterType<ICommon, CommonDynamicApiClientSelector>();
            container.RegisterType<ILoginUser, LoginUserDynamicApiClientSelector>();
        }
    }
}
