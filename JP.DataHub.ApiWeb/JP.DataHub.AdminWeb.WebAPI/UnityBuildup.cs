using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;
using Unity;

namespace JP.DataHub.AdminWeb.WebAPI
{
    [UnityBuildup]
    public class UnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            container.BuildupApiDifinition();
        }
    }
}