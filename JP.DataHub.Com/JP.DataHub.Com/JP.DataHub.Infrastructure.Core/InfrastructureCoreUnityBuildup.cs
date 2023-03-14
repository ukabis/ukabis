using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Logging;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Core.Database;
using JP.DataHub.Infrastructure.Core.EventNotify;
using JP.DataHub.Infrastructure.Core.Logging;
using JP.DataHub.Infrastructure.Core.Storage;
using JP.DataHub.MVC.Unity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Interception.Utilities;
using Unity.Lifetime;
using JP.DataHub.Infrastructure.Core.Repository;
using JP.DataHub.Service.Core.Repository;

namespace JP.DataHub.Infrastructure.Core
{
    [UnityBuildup]
    public class InfrastructureCoreUnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            if (UnityCore.IsRegistered<IConnectionStrings>() == true)
            {
                var list = UnityCore.Resolve<IConnectionStrings>();
                list?.ForEach(x =>
                {
                    container.RegisterType<IJPDataHubDbConnection, JPDataHubDbConnection>(x.Name,
                            new PerResolveLifetimeManager(), new InjectionConstructor(x.ConnectionString, x.Provider, false));
                    container.RegisterType<IJPDataHubDbConnection, JPDataHubDbConnection>(x.Name + "-Multithread",
                        new PerResolveLifetimeManager(), new InjectionConstructor(x.ConnectionString, x.Provider, true));
                });
            }
            container.RegisterType<ICharacterLimit, CharacterLimit>(new SingletonLifetimeManager());
            container.RegisterType<ILoggingInterceptor, LoggingInterceptor>();
            container.RegisterType<ICommonCrudRepository, CommonCrudRepository>();

            var charLimit = container.Resolve<ICharacterLimit>();
            charLimit.TablePrefix = "DB_";
            charLimit.Namespace = "JP.DataHub.Infrastructure.Models.Database";

            //キャッシュビルドアップ
            CacheBiludup(container, configuration);
            //イベント通知
            EventNotifyBiludup(container, configuration);
            //Storage
            StorageBiludup(container, configuration);
        }

        private void StorageBiludup(IUnityContainer container, IConfiguration configuration)
        {
            if (!configuration.GetSection(StorageConfig.SectionName).Exists())
            {
                return;
            }
            var configs = new List<StorageConfig>();
            ConfigurationBinder.Bind(configuration.GetSection(StorageConfig.SectionName), configs);
            foreach (var config in configs)
            {
                if (config.Type == StorageConfig.TypeEnum.OciObjectStorage.ToString())
                {
                    container.RegisterType<IStorageClient, OciObjectStorageClient>(config.Name, new PerResolveLifetimeManager()
                        , new InjectionConstructor(config.OciObjectStorage.ConfigFilePath, config.OciObjectStorage.NamespaceName, config.OciObjectStorage.BucketName, config.OciObjectStorage.RootPath));
                }
                else if (config.Type == StorageConfig.TypeEnum.Smb.ToString())
                {
                    container.RegisterType<IStorageClient, SmbStorageClient>(config.Name, new PerResolveLifetimeManager(), new InjectionConstructor(config.Smb.RootPath));
                }
            }
        }

        private void EventNotifyBiludup(IUnityContainer container, IConfiguration configuration)
        {
            if (!configuration.GetSection(EventNotiryConfig.SectionName).Exists())
            {
                return;
            }
            var eventNotiryConfigs = new List<EventNotiryConfig>();
            ConfigurationBinder.Bind(configuration.GetSection(EventNotiryConfig.SectionName), eventNotiryConfigs);

            foreach (var eventNotiryConfig in eventNotiryConfigs)
            {
                eventNotiryConfig.Verify();
                if (eventNotiryConfig.Type == EventNotiryConfig.Types.OracleDb.ToString())
                {
                    container.RegisterType<IEventNotifyProvider, OracleDbEventNotifyProvider>(eventNotiryConfig.Name, new PerResolveLifetimeManager()
                        , new InjectionConstructor(eventNotiryConfig.OracleDbSettings, false));
                    container.RegisterType<IEventNotifyProvider, OracleDbEventNotifyProvider>(eventNotiryConfig.Name + "-Multithread", new PerResolveLifetimeManager()
                        , new InjectionConstructor(eventNotiryConfig.OracleDbSettings, true));
                }
                else if (eventNotiryConfig.Type == EventNotiryConfig.Types.OracleAq.ToString())
                {
                    container.RegisterType<IEventNotifyProvider, OracleAqEventNotifyProvider>(eventNotiryConfig.Name, new PerResolveLifetimeManager()
                        , new InjectionConstructor(eventNotiryConfig.OracleAqSettings, false));
                    container.RegisterType<IEventNotifyProvider, OracleAqEventNotifyProvider>(eventNotiryConfig.Name + "-Multithread", new PerResolveLifetimeManager()
                        , new InjectionConstructor(eventNotiryConfig.OracleAqSettings, true));
                }
            }
        }

        private void CacheBiludup(IUnityContainer container, IConfiguration configuration)
        {
            if (!configuration.GetSection(CacheConstValue.ConfigSection).Exists()) return;

            container.RegisterInstance<ICacheManager>(new CacheManager());

            var def = configuration.GetSection(CacheConstValue.ConfigSection).GetChildren()
                .FirstOrDefault(x => x.Key == CacheConstValue.ConfigDefaultSection);
            if (def != null)
            {
                container.RegisterInstance<string>("DefaultCacheName", def.Value);
            }

            var useProfiler = configuration.GetSection("Profiler").GetValue<bool>("UseProfiler");
            foreach (var conf in configuration.GetSection(CacheConstValue.ConfigSection).GetChildren())
            {
                var name = conf.Key;
                if (useProfiler)
                {
                    var childName = name + CacheConstValue.ProfiledCacheChildPostfix;
                    container.RegisterType<ICache, ProfiledCache>(name, new PerRequestLifetimeManager(),
                        new InjectionConstructor(container, childName));
                    if (name == def?.Value)
                        container.RegisterType<ICache, ProfiledCache>(new PerRequestLifetimeManager(),
                            new InjectionConstructor(container, childName));
                    name = childName;
                }

                var mode = conf.GetValue<string>(CacheConstValue.ConfigModeSection);
                if (mode == null) continue;

                var maxSizeLimit = conf.GetValue<int>(CacheConstValue.ConfigMaxSizeSection, default);

                CacheProviderBiludup(container, configuration, mode, name,
                    CreateInstanseLifeTimeManager(
                        conf.GetValue<string>(CacheConstValue.ConfigLifetimeManagerSection)), maxSizeLimit);
                if (name == def?.Value)
                    CacheProviderDefaultBiludup(container, configuration, mode,
                        CreateInstanseLifeTimeManager(
                            conf.GetValue<string>(CacheConstValue.ConfigLifetimeManagerSection)), maxSizeLimit);
            }
        }

        private void CacheProviderBiludup(IUnityContainer container, IConfiguration configuration, string mode, string name, ITypeLifetimeManager typeLifetimeManager, int maxSizeLimit = default)
        {
            switch (mode)
            {
                case CacheConstValue.ConfigModeRedis:
                    container.RegisterType<ICache, RedisCache>(name, typeLifetimeManager, new InjectionConstructor(name, maxSizeLimit));
                    break;
                case CacheConstValue.ConfigModeInMomery:
                    var inmemorymode = configuration.GetSection(CacheConstValue.ConfigSection).GetValue<string>(CacheConstValue.InMemoryMode);
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
        private void CacheProviderDefaultBiludup(IUnityContainer container, IConfiguration configuration, string mode, ITypeLifetimeManager typeLifetimeManager, int maxSizeLimit = default)
        {
            switch (mode)
            {
                case CacheConstValue.ConfigModeRedis:
                    container.RegisterType<ICache, RedisCache>(typeLifetimeManager, new InjectionConstructor(maxSizeLimit));
                    break;
                case CacheConstValue.ConfigModeInMomery:
                    var inmemorymode = configuration.GetSection(CacheConstValue.ConfigSection).GetValue<string>(CacheConstValue.InMemoryMode);
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
        private ITypeLifetimeManager CreateInstanseLifeTimeManager(string lifetime = null)
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
