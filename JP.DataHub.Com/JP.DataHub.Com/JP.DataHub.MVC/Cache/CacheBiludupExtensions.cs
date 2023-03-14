using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unity;
using Unity.Injection;
using Unity.Interception.Utilities;
using Unity.Lifetime;
using JP.DataHub.MVC.Unity;

namespace JP.DataHub.Com.Cache
{
    public static class CacheBiludupExtensions
    {
        public static void CacheBiludup(this IUnityContainer container, IConfiguration configuration)
        {
            var cacheManager = new CacheManager();
            container.RegisterInstance<ICacheManager>(cacheManager);

            var section = configuration.GetSection(CacheConstValue.ConfigSection);
            var defaultName = section.GetValue<string>(CacheConstValue.ConfigDefaultSection);
            container.RegisterInstance<string>("DefaultCacheName", defaultName);

            cacheManager.IsEnable = section.GetValue<bool>("Enable", true);
            cacheManager.IsEnableFire = section.GetValue<bool>("Fire", true);

            var useProfiler = configuration.GetSection("Profiler").GetValue<bool>("UseProfiler");
            foreach (var conf in configuration.GetSection(CacheConstValue.ConfigSection).GetChildren())
            {
                var name = conf.Key;
                var mode = conf.GetValue<string>(CacheConstValue.ConfigModeSection);
                if (mode == null)
                {
                    continue;
                }

                var lifetimeName = conf.GetValue<string>(CacheConstValue.ConfigLifetimeManagerSection);
                if (useProfiler)
                {
                    var childName = name + CacheConstValue.ProfiledCacheChildPostfix;
                    container.RegisterType<ICache, ProfiledCache>(name, lifetimeName.CreateLifeTimeManager(), new InjectionConstructor(container, childName));
                    if (name == defaultName)
                    {
                        container.RegisterType<ICache, ProfiledCache>(lifetimeName.CreateLifeTimeManager(), new InjectionConstructor(container, childName));
                    }
                    name = childName;
                }
                var maxSizeLimit = conf.GetValue<int>(CacheConstValue.ConfigMaxSizeSection, default);
                CacheProviderBiludup(container, section, mode, name, lifetimeName.CreateLifeTimeManager(), maxSizeLimit);
                if (name == defaultName)
                {
                    CacheProviderDefaultBiludup(container, section, mode, lifetimeName.CreateLifeTimeManager(), maxSizeLimit);
                }
            }
        }

        public static void CacheProviderBiludup(IUnityContainer container, IConfigurationSection section, string mode, string name, ITypeLifetimeManager typeLifetimeManager, int maxSizeLimit = default)
        {
            switch (mode)
            {
                case CacheConstValue.ConfigModeRedis:
                    container.RegisterType<ICache, RedisCache>(name, typeLifetimeManager, new InjectionConstructor(name, maxSizeLimit));
                    break;
                case CacheConstValue.ConfigModeInMomery:
                    var inmemorymode = section.GetValue<string>($"{CacheConstValue.InMemoryMode}");
                    if (inmemorymode?.ToLower() == "json")
                    {
                        container.RegisterType<ICache, InMemoryCacheJsonVersion>(name, typeLifetimeManager, new InjectionConstructor(name, maxSizeLimit));
                    }
                    else
                    {
                        container.RegisterType<ICache, InMemoryCache>(name, typeLifetimeManager, new InjectionConstructor(name, maxSizeLimit));
                    }
                    break;
                default:
                    container.RegisterType<ICache, NoCache>(name, typeLifetimeManager, new InjectionConstructor(name));
                    break;
            }
        }
        public static void CacheProviderDefaultBiludup(IUnityContainer container, IConfigurationSection section, string mode, ITypeLifetimeManager typeLifetimeManager, int maxSizeLimit = default)
        {
            switch (mode)
            {
                case CacheConstValue.ConfigModeRedis:
                    container.RegisterType<ICache, RedisCache>(typeLifetimeManager, new InjectionConstructor(maxSizeLimit));
                    break;
                case CacheConstValue.ConfigModeInMomery:
                    var inmemorymode = section.GetValue<string>($"{CacheConstValue.InMemoryMode}");
                    if (inmemorymode?.ToLower() == "json")
                    {
                        container.RegisterType<ICache, InMemoryCacheJsonVersion>(typeLifetimeManager, new InjectionConstructor(maxSizeLimit));
                    }
                    else
                    {
                        container.RegisterType<ICache, InMemoryCache>(typeLifetimeManager, new InjectionConstructor(maxSizeLimit));
                    }
                    break;
                default:
                    container.RegisterType<ICache, NoCache>(typeLifetimeManager);
                    break;
            }
        }
    }
}
