using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.ManageApi.Core.DataContainer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class AbstractRepository
    {
        private static Lazy<TimeSpan> _cacheExpirationTimeSpan = new Lazy<TimeSpan>(() => UnityCore.Resolve<TimeSpan>("ManageApiCacheExpirationTimeSpan"));
        private Lazy<ICache> _cache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>("ManageAPI"));

        protected ICache Cache { get => _cache.Value; }

        /// <summary>
        /// CacheTimeの有効期限
        /// </summary>
        protected static TimeSpan CacheExpirationTimeSpan { get => _cacheExpirationTimeSpan.Value; }

        private readonly Lazy<IPerRequestDataContainer> _requestDataContainer = new Lazy<IPerRequestDataContainer>(
            () =>
            {
                try
                {
                    return UnityCore.Resolve<IPerRequestDataContainer>();
                }
                catch
                {
                    // ignored
                    return UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                }
            });

        /// <summary>
        /// Constructor.
        /// </summary>
        protected AbstractRepository() { }

        /// <summary>
        /// PerRequestDataContainer
        /// </summary>
        protected IPerRequestDataContainer PerRequestDataContainer => _requestDataContainer.Value;

        protected DateTime UtcNow { get => PerRequestDataContainer.GetDateTimeUtil().GetUtc(this.PerRequestDataContainer.GetDateTimeUtil().LocalNow); }

        protected static Lazy<IConfiguration> s_lazyConfiguration = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        protected static IConfiguration s_configuration { get => s_lazyConfiguration.Value; }

        protected static Lazy<IConfigurationSection> s_lazyAppConfig = new Lazy<IConfigurationSection>(() => UnityCore.Resolve<IConfiguration>().GetSection("AppConfig"));
        protected static IConfigurationSection s_appConfig { get => s_lazyAppConfig.Value; }
    }
}
