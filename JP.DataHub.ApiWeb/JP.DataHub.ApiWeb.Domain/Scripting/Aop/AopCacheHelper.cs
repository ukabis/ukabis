using Microsoft.Extensions.Configuration;
using JP.DataHub.Aop;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Aop
{
    class AopCacheHelper : IAopCacheHelper
    {
        protected static readonly int DEFAULT_CACHE_EXPIRATION_MAX_SECOND = 86400;
        protected static readonly int DEFAULT_CACHE_EXPIRATION_SECOND = 1800;
        protected static readonly int DEFAULT_CACHE_KEY_MAX_LENGTH = 1000;
        protected static readonly int DEFAULT_CACHE_VALUE_MAX_SIZE = 1048576;

        private static IConfigurationSection appconfig = UnityCore.Resolve<IConfiguration>().GetSection("AppConfig");
        protected static TimeSpan MaxCacheExpiration { get; } = new TimeSpan(0, 0, appconfig.GetValue<int>("AopCacheExpirationMaxSecond", DEFAULT_CACHE_EXPIRATION_MAX_SECOND));
        protected static TimeSpan DefaultCacheExpiration { get; } = new TimeSpan(0, 0, appconfig.GetValue<int>("AopCacheExpirationDefaultSecond", DEFAULT_CACHE_EXPIRATION_SECOND));
        protected static int CacheKeyMaxLength { get; } = appconfig.GetValue<int>("AopCacheKeyMaxLength", DEFAULT_CACHE_KEY_MAX_LENGTH);
        protected static int CacheValueMaxSize { get; } = appconfig.GetValue<int>("AopCacheValueMaxSize", DEFAULT_CACHE_VALUE_MAX_SIZE);

        protected readonly string KeyPrefix;

        protected ICache Cache { get; } = UnityCore.Resolve<ICache>("AopCache");
        protected bool IsAopCacheEnable = UnityCore.Resolve<bool>("IsAopCacheEnable");


        public AopCacheHelper(string keyPrefix)
        {
            KeyPrefix = keyPrefix;
        }


        /// <summary>
        /// キャッシュを追加する。
        /// </summary>
        public void Add(string key, object value)
        {
            Add(key, value, DefaultCacheExpiration);
        }

        /// <summary>
        /// キャッシュを追加する。
        /// </summary>
        public void Add(string key, object value, TimeSpan expiration)
        {
            if (!IsAopCacheEnable)
            {
                return;
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var cacheKey = CreateKey(key);
            if (Cache.Contains(cacheKey))
            {
                Cache.Remove(cacheKey);
            }
            Cache.Add(cacheKey, value, GetCappedExpiration(expiration), CacheValueMaxSize);
        }


        /// <summary>
        /// キャッシュを取得する。
        /// </summary>
        public T Get<T>(string key)
        {
            if (!IsAopCacheEnable)
            {
                return default(T);
            }

            return Cache.Get<T>(CreateKey(key), out var isNullValue);
        }


        /// <summary>
        /// キャッシュを取得する。ミスヒットの場合は指定された関数の結果をキャッシュに追加する。
        /// </summary>

        public T GetOrAdd<T>(string key, Func<T> misshitAction)
        {
            return GetOrAdd<T>(key, DefaultCacheExpiration, misshitAction);
        }

        /// <summary>
        /// キャッシュを取得する。ミスヒットの場合は指定された関数の結果をキャッシュに追加する。
        /// </summary>
        public T GetOrAdd<T>(string key, TimeSpan expiration, Func<T> misshitAction)
        {
            if (!IsAopCacheEnable)
            {
                return misshitAction.Invoke();
            }

            var cacheValue = Cache.Get<T>(CreateKey(key), out var isNullValue);
            if (!EqualityComparer<T>.Default.Equals(cacheValue, default(T)) && !isNullValue)
            {
                return cacheValue;
            }

            // ミスヒット
            var returnValue = misshitAction.Invoke();
            if (!EqualityComparer<T>.Default.Equals(returnValue, default(T)))
            {
                Cache.Add(CreateKey(key), returnValue, GetCappedExpiration(expiration), CacheValueMaxSize);
            }
            return returnValue;
        }


        /// <summary>
        /// キャッシュを削除する。
        /// </summary>
        public void Remove(string key)
        {
            if (!IsAopCacheEnable)
            {
                return;
            }

            Cache.Remove(CreateKey(key));
        }


        private TimeSpan GetCappedExpiration(TimeSpan expiration)
        {
            if (TimeSpan.Compare(expiration, MaxCacheExpiration) == 1)
            {
                return MaxCacheExpiration;
            }

            return expiration;
        }

        private string CreateKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (key.Length > CacheKeyMaxLength)
            {
                throw new ArgumentException($"{nameof(key)} length should not be more than {CacheKeyMaxLength}");
            }

            return $"{KeyPrefix}.{key}";
        }
    }
}
