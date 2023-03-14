using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unity;
using EasyCaching.Core;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.Cache
{
    public static class CacheServiceCollectionExtensions
    {
        public static readonly int CACHE_EXPIRATION_SECOND = 3600;
        public static TimeSpan DefaultExpiration = new TimeSpan(0, 0, CACHE_EXPIRATION_SECOND);

        public static Dictionary<string, TimeSpan> TimeSpanDictionary = new Dictionary<string, TimeSpan>();

        public static void RegisterCache(this IUnityContainer container)
        {
            TimeSpanDictionary.ToList().ForEach(x => container.RegisterInstance<TimeSpan>(x.Key, x.Value));
        }

        public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(CacheConstValue.ConfigSection);
            if (section?.Exists() == true)
            {
                foreach (var conf in section.GetChildren())
                {
                    var mode = conf.GetValue<string>(CacheConstValue.ConfigModeSection);
                    switch (mode)
                    {
                        case CacheConstValue.ConfigModeRedis:
                            AddRedis(services, conf);
                            break;
                        case CacheConstValue.ConfigModeInMomery:
                            AddImMemory(services, conf);
                            break;
                    }
                }
                TimeSpanDictionary.Add("Cache.Expiration.Default", TimeSpanDictionary[$"Cache.Expiration.{section.GetDefault()}"]);
            }
            return services;
        }

        private static void AddRedis(IServiceCollection services, IConfigurationSection conf)
        {
            services.AddEasyCaching(option =>
            {
                option.UseRedis(config =>
                {
                    config.DBConfig.Configuration = conf.GetConnectionStrings();
                    config.SerializerName = "msgpack";
                }, conf.Key.ToString()).WithMessagePack();
                TimeSpanDictionary.Add($"Cache.Expiration.{conf.Key}", conf.GetExpiration());
            });
        }

        private static void AddImMemory(IServiceCollection services, IConfigurationSection conf)
        {
            services.AddEasyCaching(option =>
            {
                option.UseInMemory(config =>
                {
                    config.SerializerName = "msgpack";
                }, conf.Key.ToString()).WithMessagePack();
            });
            TimeSpanDictionary.Add($"Cache.Expiration.{conf.Key}", conf.GetExpiration());
        }

        private static string GetDefault(this IConfigurationSection configurationSection)
            => configurationSection.GetValue<string>(CacheConstValue.ConfigDefaultSection);

        private static string GetConnectionStrings(this IConfigurationSection configurationSection)
            => configurationSection.GetValue<string>($"{CacheConstValue.ConfigOptionSection}:{CacheConstValue.ConfigConnectionStrings}");

        private static TimeSpan GetExpiration(this IConfigurationSection configurationSection)
            => configurationSection.GetValue<TimeSpan?>(CacheConstValue.ConfigExpiration) ?? DefaultExpiration;
    }
}
