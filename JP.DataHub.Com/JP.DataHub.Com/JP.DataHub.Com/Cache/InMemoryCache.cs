using JP.DataHub.Com.Serializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Unity;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.Cache
{
    public class InMemoryCache : AbstractCache 
    {
        private ISerializer _serializer { get; set; }

        public InMemoryCache(string name, int maxSizeLimit)
            : base(name, maxSizeLimit)
        {
        }

        public InMemoryCache(string name)
            : base(name)
        {
        }

        public InMemoryCache(int maxSizeLimit)
            : base(maxSizeLimit)
        {
        }

        public InMemoryCache()
            : base()
        {
        }

        protected override void Init()
        {
            base.Init();
            _serializer = UnityCore.ResolveOrDefault<ISerializer>("InMemoryCache");
            if (_serializer == null)
            {
                _serializer = UnityCore.ResolveOrDefault<ISerializer>();
            }
        }

        public override IEnumerable<string> Keys()
        {
            var innerCacheId = typeof(EasyCaching.InMemory.DefaultInMemoryCachingProvider).GetField("_cache", BindingFlags.NonPublic | BindingFlags.Instance);
            var innerCache = innerCacheId.GetValue(this.Provider);
            var innerMemoryId = typeof(EasyCaching.InMemory.InMemoryCaching).GetField("_memory", BindingFlags.NonPublic | BindingFlags.Instance);

            var collection = innerMemoryId.GetValue(innerCache) as ICollection;
            var items = new List<string>();
            if (collection != null)
            {
                foreach (var item in collection)
                {
                    var methodInfo = item.GetType().GetProperty("Key");
                    var val = methodInfo.GetValue(item);
                    items.Add(val.ToString());
                }
            }
            return items;
        }

        public override void Add(string key, object obj, TimeSpan absoluteExpiration, int maxSaveSize)
        {
            if (obj is null)
            {
                Provider.Set(key, NullValue, absoluteExpiration);
                return;
            }
            maxSaveSize = default(int) == maxSaveSize || ProviderSizeLimit < maxSaveSize ? ProviderSizeLimit : maxSaveSize;
            var serialized = _serializer.SerializeByte(obj);

            if (maxSaveSize < serialized.Length)
            {
                return;
            }
            Provider.Set(key, serialized, absoluteExpiration);
        }

        public override object Get(Type type, string key)
        {
            if (IsFlash)
                return null;
            var cacheResult = Provider.Get<object>(key);
            if (cacheResult.IsNull)
                return null;
            if(cacheResult.Value.Equals(NullValue))
                return null;
            //MemorycacheはStreamを復元した際にStreamが閉じられているのでデシリアライズしてあげる必要がある
            return _serializer.Deserialize(type, (byte[])cacheResult.Value);
        }

        public override T Get<T>(string key, out bool isNullValue, bool isUseMessagePack = false)
        {
            var cacheResult = Provider.Get<object>(key);
            isNullValue = !cacheResult.IsNull && cacheResult.Value.Equals(NullValue);
            if (cacheResult.IsNull)
                return default(T);
            if (cacheResult.Value.Equals(NullValue))
                return default(T);
            return _serializer.Deserialize<T>((byte[])cacheResult.Value);
        }
    }
}
