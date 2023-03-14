using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Core.DataContainer;

namespace JP.DataHub.ManageApi.Service
{
    internal class AbstractService
    {
        private readonly Lazy<IPerRequestDataContainer> _requestDataContainer = new Lazy<IPerRequestDataContainer>(() =>
            {
                try
                {
                    return UnityCore.Resolve<IPerRequestDataContainer>();
                }
                catch
                {
                    return UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                }
            });

        /// <summary>
        /// PerRequestDataContainer
        /// </summary>
        protected IPerRequestDataContainer PerRequestDataContainer => _requestDataContainer.Value;

        protected static Lazy<IConfiguration> s_lazyConfiguration = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        protected static IConfiguration Configuration { get => s_lazyConfiguration.Value; }

        protected static Lazy<IConfigurationSection> s_lazyAppConfig = new Lazy<IConfigurationSection>(() => UnityCore.Resolve<IConfiguration>().GetSection("AppConfig"));
        protected static IConfigurationSection AppConfig { get => s_lazyAppConfig.Value; }
    }
}
