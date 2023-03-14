using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Serializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Unity;

namespace JP.DataHub.Com.Cache
{
    public class InMemoryCacheJsonVersion : AbstractCache
    {
        public InMemoryCacheJsonVersion(string name, int maxSizeLimit)
            : base(name, maxSizeLimit)
        {
        }

        public InMemoryCacheJsonVersion(string name)
            : base(name)
        {
        }

        public InMemoryCacheJsonVersion(int maxSizeLimit)
            : base(maxSizeLimit)
        {
        }

        public InMemoryCacheJsonVersion()
            : base()
        {
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
            var serialized = Encoding.UTF8.GetBytes(obj.ToJsonString());
            if (maxSaveSize < serialized.Length)
            {
                return;
            }
            Provider.Set(key, serialized, absoluteExpiration);
        }


        public override object Get(Type type, string key)
        {
            if (IsFlash)
            {
                return null;
            }
            var cacheResult = Provider.Get<byte[]>(key);
            if (cacheResult.IsNull || cacheResult.Value.Equals(NullValue))
            {
                return null;
            }
            return Encoding.UTF8.GetString(cacheResult.Value).ToObject(type);
        }

        public override T Get<T>(string key, out bool isNullValue, bool isUseMessagePack = false)
        {
            var cacheResult = Provider.Get<byte[]>(key);
            isNullValue = !cacheResult.IsNull && cacheResult.Value.Equals(NullValue);
            if (cacheResult.IsNull || cacheResult.Value.Equals(NullValue))
            {
                return default(T);
            }
            return Encoding.UTF8.GetString(cacheResult.Value).ToObject<T>();
        }
    }
}
