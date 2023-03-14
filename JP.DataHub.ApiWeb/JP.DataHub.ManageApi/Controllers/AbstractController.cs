using System;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using JP.DataHub.ManageApi.Core.DataContainer;
using JP.DataHub.ManageApi.Filters;

namespace JP.DataHub.ManageApi.Controllers
{
    [RFC7807ExceptionFilter]
    public class AbstractController : ControllerBase
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
        protected static IConfiguration s_configuration { get => s_lazyConfiguration.Value; }

        protected static Lazy<IConfigurationSection> s_lazyAppConfig = new Lazy<IConfigurationSection>(() => UnityCore.Resolve<IConfiguration>().GetSection("AppConfig"));
        protected static IConfigurationSection s_appConfig { get => s_lazyAppConfig.Value; }

        public AbstractController()
        {
            this.AutoInjection();
        }
    }
}
