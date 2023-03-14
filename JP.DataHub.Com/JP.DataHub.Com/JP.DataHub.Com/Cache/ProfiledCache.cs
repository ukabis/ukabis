using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.Com.Cache
{
    public class ProfiledCache : ICache
    {
        protected ICache _cache;
        private bool Hit;

        public ProfiledCache(IUnityContainer container, string childName)
        {
            _cache = container.Resolve<ICache>(childName);
            _cache.CacheStatusNotify = CacheResult;
        }

        public bool IsFlash { get => _cache.IsFlash; set => _cache.IsFlash = value; }
        public Action<bool> CacheStatusNotify { get; set; }
        public void Close() => _cache.Close();
        public void Clear() => _cache.Clear();
        public bool Contains(string key) => _cache.Contains(key);
        public virtual void Add(string key, object obj) => _cache.Add(key, obj);
        public virtual void Add(string key, object obj, int secExpiration) => _cache.Add(key, obj, secExpiration);
        public virtual void Add(string key, object obj, int secExpiration, int maxSaveSize) => _cache.Add(key, obj, secExpiration, maxSaveSize);
        public virtual void Add(string key, object obj, int hourExpiration, int minuteExpiration, int secExpiration) 
            => _cache.Add(key, obj, hourExpiration, minuteExpiration, secExpiration);
        public void Add(string key, object obj, TimeSpan absoluteExpiration) => _cache.Add(key, obj, absoluteExpiration);
        public void Add(string key, object obj, TimeSpan absoluteExpiration, int maxSaveSize) => _cache.Add(key, obj, absoluteExpiration, maxSaveSize);
        public void Remove(string key) => _cache.Remove(key);
        public Task RemoveAsync(string key) => _cache.RemoveAsync(key);
        public void RemovePattern(List<object> param) => _cache.RemovePattern(param);
        public void RemovePatternByKeyOnly(string key) => _cache.RemovePatternByKeyOnly(key);
        public void RemoveFirstMatch(string key) => _cache.RemoveFirstMatch(key);
        public Task RemoveFirstMatchAsync(string key) => _cache.RemoveFirstMatchAsync(key);

        public T Get<T>(string key, out bool isNullValue, bool isUseMessagePack = false)
        {
            if (MiniProfiler.Current != null)
            {
                using (var ct = MiniProfiler.Current.CustomTiming("cache", $"Get {key}"))
                {
                    var result = _cache.Get<T>(key, out isNullValue, isUseMessagePack);
                    Hit = result != null;
                    UpdateSnippetResult(ct, Hit);
                    return result;
                }
            }
            return _cache.Get<T>(key, out isNullValue, isUseMessagePack);
        }
        public T Get<T>(string key, ActionObject misshit_action) => Get<T>(key, AbstractCache.CACHE_EXPIRATION_SECOND, misshit_action);
        public T Get<T>(string key, int secExpiration, ActionObject misshit_action) => Get<T>(key, new TimeSpan(0, 0, secExpiration), misshit_action);
        public T Get<T>(string key, int hourExpiration, int minuteExpiration, int secExpiration, ActionObject misshit_action) 
            => Get<T>(key, new TimeSpan(hourExpiration, minuteExpiration, secExpiration), misshit_action);
        public T Get<T>(string key, TimeSpan absoluteExpiration, ActionObject misshit_action)
        {
            if (MiniProfiler.Current != null)
            {
                using (var ct = MiniProfiler.Current.CustomTiming("cache", $"Get {key}"))
                {
                    Hit = false;
                    var result = _cache.Get<T>(key, absoluteExpiration, misshit_action);
                    UpdateSnippetResult(ct, Hit);
                    return result;
                }
            }
            return _cache.Get<T>(key, absoluteExpiration, misshit_action);
            
        }

        public object Get(Type type, string key)
        {
            if (MiniProfiler.Current != null)
            {
                using (var ct = MiniProfiler.Current.CustomTiming("cache", $"Get {key}"))
                {
                    var result = _cache.Get(type, key);
                    Hit = result != null;
                    UpdateSnippetResult(ct, Hit);
                    return result;
                }
            }
            return _cache.Get(type, key);
        }
        public object Get(Type type, string key, ActionObject misshit_action) => Get(type, key, AbstractCache.CACHE_EXPIRATION_SECOND, misshit_action);
        public object Get(Type type, string key, int secExpiration, ActionObject misshit_action) => Get(type, key, new TimeSpan(0, 0, secExpiration), misshit_action);
        public object Get(Type type, string key, int hourExpiration, int minuteExpiration, int secExpiration, ActionObject misshit_action)
            => Get(type, key, new TimeSpan(hourExpiration, minuteExpiration, secExpiration), misshit_action);
        public object Get(Type type, string key, TimeSpan absoluteExpiration, ActionObject misshit_action)
        {
            if (MiniProfiler.Current != null)
            {
                using (var ct = MiniProfiler.Current.CustomTiming("cache", $"Get {key}"))
                {
                    Hit = false;
                    var result = _cache.Get(type, key, absoluteExpiration, misshit_action);
                    UpdateSnippetResult(ct, Hit);
                    return result;
                }
            }
            return _cache.Get(type, key, absoluteExpiration, misshit_action);
        }
        public IEnumerable<string> Keys() => _cache.Keys();

        private void UpdateSnippetResult(CustomTiming ct, bool result) => ct.CommandString = $"{result} {ct.CommandString}";
        private void CacheResult(bool hit) => Hit = hit;
    }
}
