using Unity;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Interface.Impl;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Service;
using JP.DataHub.ApiWeb.Domain.Service.Impl;

namespace JP.DataHub.ApiWeb.Interface
{
    [UnityBuildup]
    public class UnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            container.RegisterType<IDynamicApiInterface, DynamicApiInterface>();
            container.RegisterType<IVendorSystemAuthenticationInterface, VendorSystemAuthenticationInterface>();
            container.RegisterType<IScriptRuntimeLogFileInterface, ScriptRuntimeLogFileInterface>();
            container.RegisterType<IAttachFileInterface, AttachFileInterface>();
            container.RegisterType<IOpenIdUserInterface, OpenIdUserInterface>();
            container.RegisterType<ICryptographyInterface, CryptographyInterface>();
            container.RegisterType<ICryptographyManagementInterface, CryptographyManagementInterface>();
            container.RegisterType<IMetadataInfoInterface, MetadataInfoInterface>();
            container.RegisterType<ICacheInterface, CacheInterface>();
        }
    }
}