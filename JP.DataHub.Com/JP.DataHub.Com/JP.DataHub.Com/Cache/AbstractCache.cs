using EasyCaching.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Serializer;
using JP.DataHub.Com.Transaction;
using Unity;
using JP.DataHub.Com.Unity;
using MessagePack;

namespace JP.DataHub.Com.Cache
{
    public abstract class AbstractCache : ICache
    {
        public static readonly int CACHE_EXPIRATION_SECOND = 3600;

        public const string NullValue = "[null]";

        protected int ProviderSizeLimit = int.MaxValue; 
        protected string Name { get; set; }
        protected IEasyCachingProvider Provider { get; set; }

        private static Lazy<IList<IEasyCachingProvider>> s_lazyCachingProviderList;
        private static IList<IEasyCachingProvider> s_cachingProviderList { get => s_lazyCachingProviderList.Value; }

        private static Lazy<IConfiguration> s_lazyConfig = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        private static Lazy<bool> s_lazyUseProfiler = new Lazy<bool>(() => s_lazyConfig.Value.GetSection("Profiler").GetValue<bool>("UseProfiler"));
        private static bool s_useProfiler { get => s_lazyUseProfiler.Value; }

        private JPDataHubLogger _logger = new JPDataHubLogger(typeof(AbstractJPDataHubDbConnection));

        static AbstractCache()
        {
            s_lazyCachingProviderList = new Lazy<IList<IEasyCachingProvider>>(() => UnityCore.Resolve<IEnumerable<IEasyCachingProvider>>()?.ToList());
        }

        public AbstractCache()
            : this(null, default)
        {
            Init();
        }

        public AbstractCache(string name = null)
            : this(name, default)
        {
            Init();
        }

        public AbstractCache(int providerSizeLimit)
            : this(null, default)
        {
            if (providerSizeLimit != default)
            {
                ProviderSizeLimit = providerSizeLimit;
            }
            Init();
        }

        public AbstractCache(string name = null, int providerSizeLimit = default)
        {
            s_lazyCachingProviderList = new Lazy<IList<IEasyCachingProvider>>(() => UnityCore.Resolve<IEnumerable<IEasyCachingProvider>>()?.ToList());
            if (name == null)
            {
                name = UnityCore.Resolve<string>("DefaultCacheName");
            }
            Name = name;
            if (providerSizeLimit != default)
            {
                ProviderSizeLimit = providerSizeLimit;
            }
            Init();
        }

        public static void DeInit()
        {
            s_lazyCachingProviderList = null;
        }

        protected virtual void Init()
        {
            if (s_useProfiler == true)
            {
                Name = Name.Replace(CacheConstValue.ProfiledCacheChildPostfix, string.Empty);
            }
            Provider = s_cachingProviderList.Where(x => x.Name == Name).FirstOrDefault();
        }

        public virtual bool IsFlash { get; set; } = false;
        public Action<bool> CacheStatusNotify { get; set; }
        public virtual void Close()
        {
        }

        public virtual void Clear()
        {
            Provider.Flush();
        }
        public virtual bool Contains(string key)
        {
            if (IsFlash)
            {
                return false;
            }
            return Provider.Exists(key);
        }

        public virtual void Add(string key, object obj, TimeSpan absoluteExpiration, int maxSaveSize)
        {
            if (obj is null)
            {
                Provider.Set(key, NullValue, absoluteExpiration);
                return;
            }
            maxSaveSize = default(int) == maxSaveSize || ProviderSizeLimit < maxSaveSize ? ProviderSizeLimit : maxSaveSize;
            var bytes = MessagePackSerializer.Serialize(obj);
            if (maxSaveSize < bytes.Length)
            {
                return;
            }
            Provider.Set(key, obj, absoluteExpiration);
        }

        public virtual void Add(string key, object obj, TimeSpan absoluteExpiration) => Add(key, obj, absoluteExpiration, 0);
        public virtual void Add(string key, object obj, int hourExpiration, int minuteExpiration, int secExpiration) => Add(key, obj, new TimeSpan(hourExpiration, minuteExpiration, secExpiration));
        public virtual void Add(string key, object obj, int secExpiration) => Add(key, obj, new TimeSpan(0, 0, secExpiration));
        public virtual void Add(string key, object obj, int secExpiration, int maxSaveSize) => Add(key, obj, new TimeSpan(0, 0, secExpiration), maxSaveSize);
        public virtual void Add(string key, object obj) => Add(key, obj, CACHE_EXPIRATION_SECOND);

        private readonly ISerializer _serializer = UnityCore.ResolveOrDefault<ComMessagePackSerializer>();
        public virtual T Get<T>(string key, out bool isNullValue, bool isUseMessagePack = false)
        {
            try
            {
                var tempObject= Provider.Get<object>(key).Value;
                isNullValue = tempObject!=null && tempObject.Equals(NullValue);

                if (tempObject == null)
                {
                    isNullValue = false;
                    return default(T);
                }
                if (tempObject.Equals(NullValue))
                {
                    return default(T);
                }

                if (isUseMessagePack)
                {
                    return  Provider.Get<T>(key).Value;
                }

                var tempByte = _serializer.SerializeByte(tempObject);
                return _serializer.Deserialize<T>(tempByte);

            }
            catch (Exception e)
            {
                _logger.Fatal($"キャッシュから取得できないためdefault(T)を返却。エラー詳細:{ e.Message }");
                isNullValue = false;
                return default(T);
            }
        }

        public virtual object Get(Type type, string key, TimeSpan absoluteExpiration, ActionObject misshit_action)
        {
            if (IsFlash == false)
            {
                var cacheGetResult = Get(type, key);
                if (CacheStatusNotify != null)
                {
                    CacheStatusNotify(cacheGetResult != null);
                }
                if (cacheGetResult != null)
                {
                    return cacheGetResult.Equals(NullValue) ? null : cacheGetResult;
                }
            }

            object result = null;
            object data = misshit_action();
            if (data != null)
            {
                result = data;
                Add(key, data, absoluteExpiration);
            }
            else
            {
                Add(key, result, absoluteExpiration);
            }
            return result;
        }

        public virtual T Get<T>(string key, TimeSpan absoluteExpiration, ActionObject misshit_action)
        {
            if (IsFlash == false)
            {
                var cacheGetResult = Get<T>(key, out var isNullValue, false);
                if (CacheStatusNotify != null)
                {
                    CacheStatusNotify(cacheGetResult != null);
                }

                if (isNullValue)
                {
                    return default(T);
                }
                if (cacheGetResult != null && !cacheGetResult.Equals(default(T)))
                {
                    return cacheGetResult;
                }
            }

            T result = default(T);
            object data = misshit_action();
            if (data != null)
            {
                result = (T)data;
                Add(key, data, absoluteExpiration);
            }
            else
            {
                Add(key, result, absoluteExpiration);
            }
            return result;
        }

        public virtual T Get<T>(string key, int hourExpiration, int minuteExpiration, int secExpiration, ActionObject misshit_action)
        {
            return Get<T>(key, new TimeSpan(hourExpiration, minuteExpiration, secExpiration), misshit_action);
        }

        public virtual T Get<T>(string key, int secExpiration, ActionObject misshit_action)
        {
            return Get<T>(key, new TimeSpan(0, 0, secExpiration), misshit_action);
        }

        public virtual T Get<T>(string key, ActionObject misshit_action)
        {
            return Get<T>(key, CACHE_EXPIRATION_SECOND, misshit_action);
        }

        public virtual object Get(Type type, string key, int hourExpiration, int minuteExpiration, int secExpiration, ActionObject misshit_action)
        {
            return Get(type, key, new TimeSpan(hourExpiration, minuteExpiration, secExpiration), misshit_action);
        }

        public virtual object Get(Type type, string key, int secExpiration, ActionObject misshit_action)
        {
            return Get(type, key, new TimeSpan(0, 0, secExpiration), misshit_action);
        }

        public virtual object Get(Type type, string key, ActionObject misshit_action)
        {
            return Get(type, key, CACHE_EXPIRATION_SECOND, misshit_action);
        }

        public virtual object Get(Type type, string key)
        {
            return IsFlash == true ? null : Provider.GetAsync(key, type).Result;
        }

        public virtual void Remove(string key)
        {
            Provider.Remove(key);
        }

        public virtual Task RemoveAsync(string key)
        {
            return Provider.RemoveAsync(key);
        }

        public virtual void RemovePattern(List<object> param)
        {
            List<string> p = param.Select(x => x == null ? null : x.ToString()).ToList();

            List<string> remove = new List<string>();
            foreach (string cacheKey in Keys())
            {
                if (cacheKey.StartsWith(p[0]) == false)
                {
                    continue;
                }

                string[] sp = cacheKey.Split(".".ToCharArray());
                if (sp.Length == p.Count())
                {
                    //Index0にはキャッシュのキーが入っているためIndex1から判定する
                    for (int i = 1; i < sp.Length; i++)
                    {
                        if (p[i] != null && p[i] != sp[i])
                        {
                            continue;
                        }
                        else if (p[i] == null && sp[i] != NullValue)
                        {
                            continue;
                        }
                        if (!remove.Any(x => x == cacheKey))
                        {
                            remove.Add(cacheKey);
                        }
                    }
                }
            }
            remove.ForEach(x => Provider.Remove(x));
        }

        public virtual void RemovePatternByKeyOnly(string key)
        {
            // Idが付いているはずなので、「.」まで判定する
            var targetKey = $"{key}.";
            Provider.RemoveByPrefix(targetKey);
        }

        public virtual void RemoveFirstMatch(string key)
        {
            Provider.RemoveByPrefix(key);
        }

        public virtual Task RemoveFirstMatchAsync(string key)
        {
            return Provider.RemoveByPrefixAsync(key);
        }

        public virtual IEnumerable<string> Keys()
        {
            return null;
        }
    }
}
