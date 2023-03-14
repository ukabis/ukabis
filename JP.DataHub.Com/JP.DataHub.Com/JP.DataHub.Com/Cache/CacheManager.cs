using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Newtonsoft.Json;
using Unity;
using Unity.Interception.Utilities;
using Unity.Interception.PolicyInjection.Pipeline;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.Cache
{
    public class CacheManager : ICacheManager
    {
        public bool IsEnable { get; set; }
        public bool IsEnableFire { get; set; }

        private List<ClassMethodAttributeDefine> methodMapping = new List<ClassMethodAttributeDefine>();

        public static CacheManager Startup(Type type)
        {
            return null;
        }

        public CacheManager(string targetAssembliePrefix = null)
        {
            ParseAllAssemblies(targetAssembliePrefix ?? "JP.DataHub");
        }

        public void FireKey(string keyName) => FireKey(null, new string[] { keyName });
        public void FireKey(params string[] keyName) => FireKey(null, keyName);
        public void FireKey(ICache cache, string keyName) => FireKey(cache, new string[] { keyName });
        public void FireKey(ICache cache, params string[] keyName)
        {
            cache = cache == null ? UnityCore.Resolve<ICache>() : cache;
            foreach (var key in keyName)
            {
                cache.RemoveFirstMatch(key);
            }
        }

        public Task FireKeyAsync(string keyName) => FireKeyAsync(null, new string[] { keyName });
        public Task FireKeyAsync(IEnumerable<string> keyName) => FireKeyAsync(null, keyName);
        public Task FireKeyAsync(ICache cache, string keyName) => FireKeyAsync(cache, new string[] { keyName });
        public Task FireKeyAsync(ICache cache, IEnumerable<string> keyName)
        {
            var tasks = new List<Task>();
            cache = cache == null ? UnityCore.Resolve<ICache>() : cache;
            foreach (var key in keyName)
            {
                tasks.Add(cache.RemoveFirstMatchAsync(key));
            }
            return Task.WhenAll(tasks);
        }

        public void FireEntity(string entity) => FireEntity(null, new string[] { entity });
        public void FireEntity(params string[] entities) => FireEntity(null, entities);
        public void FireEntity(ICache cache, string entity) => FireEntity(cache, new string[] { entity });
        public void FireEntity(ICache cache, params string[] entities)
        {
            cache = cache == null ? UnityCore.Resolve<ICache>() : cache;
            foreach (var entitity in entities)
            {
                methodMapping.Where(x => x.CacheEntity?.Entities?.Contains(entitity) == true).Where(x => x.KeyPrefix != null).ForEach(x => cache.RemoveFirstMatch(x.KeyPrefix));
            }
        }

        public Task FireEntityAsync(string entity) => FireEntityAsync(null, new string[] { entity });
        public Task FireEntityAsync(IEnumerable<string> entities) => FireEntityAsync(null, entities);
        public Task FireEntityAsync(ICache cache, string entity) => FireEntityAsync(cache, new string[] { entity });

        public Task FireEntityAsync(ICache cache, IEnumerable<string> entities)
        {
            var tasks = new List<Task>();
            cache = cache == null ? UnityCore.Resolve<ICache>() : cache;
            foreach (var entitity in entities)
            {
                methodMapping.Where(x => x.CacheEntity?.Entities?.Contains(entitity) == true).Where(x => x.KeyPrefix != null).ForEach(x => tasks.Add(cache.RemoveFirstMatchAsync(x.KeyPrefix)));
            }
            return Task.WhenAll(tasks);
        }
        public void FireId(string keyName, string val) => FireId(null, keyName, val);

        public void FireId(ICache cache, string keyName, string val)
        {
            val = val?.Replace(".", "[dot]");
            cache = cache == null ? UnityCore.Resolve<ICache>() : cache;
            foreach (var cacheKey in cache.Keys())
            {
                var cacheKeyParam = CacheHelper.ParseCacheKeyParam(cacheKey);
                if (cacheKeyParam?.Param.Where(x => x.Key == keyName && x.Value == val).Count() > 0)
                {
                    cache.Remove(cacheKey);
                }
            }
        }
        public Task FireIdAsync(string keyName, string val) => FireIdAsync(null, keyName, val);

        public Task FireIdAsync(ICache cache, string keyName, string val)
        {
            var tasks = new List<Task>();
            cache = cache == null ? UnityCore.Resolve<ICache>() : cache;
            foreach (var cacheKey in cache.Keys())
            {
                var cacheKeyParam = CacheHelper.ParseCacheKeyParam(cacheKey);
                if (cacheKeyParam?.Param.Where(x => x.Key == keyName).Count() > 0)
                {
                    tasks.Add(cache.RemoveAsync(cacheKey));
                }
            }
            return Task.WhenAll(tasks);
        }

        #region 下位互換性

        public static string CreateKey(params object[] args)
        {
            bool first = true;
            StringBuilder result = new StringBuilder();
            foreach (var arg in args)
            {
                if (first == false)
                {
                    result.Append(".");
                }
                if (arg == null)
                {
                    result.Append("[null]");
                }
                else
                {
                    var type = arg.GetType();
                    var val = arg.ToString();
                    if (type.IsPrimitive == true || type == typeof(string) || type == typeof(Guid) || val != type.FullName)
                    {
                        result.Append(val);
                    }
                    else
                    {
                        val = Convert.ToBase64String(MessagePackSerializer.Serialize(JsonConvert.SerializeObject(arg)));
                        result.Append(val);
                    }
                }
                first = false;
            }
            return result.ToString();
        }

        public static string CreateBlobKey(params object[] args)
        {
            bool first = true;
            StringBuilder result = new StringBuilder();
            foreach (var arg in args)
            {
                if (first == false)
                {
                    result.Append("-");
                }
                if (arg == null)
                {
                    result.Append("[null]");
                }
                else
                {
                    result.Append(arg.ToString());
                }
                first = false;
            }
            return result.ToString();
        }

        #endregion

        private void ParseAllAssemblies(string targetAssembliePrefix)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()/*.Where(x => x.FullName.StartsWith(targetAssembliePrefix))*/)
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        foreach (var method in type.GetMethods())
                        {
                            var c = method.GetCustomAttribute<CacheAttribute>();
                            if (c != null)
                            {
                                // キャッシュキー Prefixの指定が存在しない場合は、「クラス名-メソッド名」とする※ユニークになる確率が高いため
                                if (c.KeyPrefix == null)
                                    c.KeyPrefix = $"{method.ReflectedType.Name}-{method.Name}";
                                var ca = method.GetCustomAttribute<CacheArgAttribute>();
                                var ce = method.GetCustomAttribute<Attributes.CacheEntityAttribute>();
                                methodMapping.Add(new ClassMethodAttributeDefine() { Method = method, KeyPrefix = c.KeyPrefix, CacheArgument = ca, CacheEntity = ce, CacheAttr = c, MethodName = method.ToString() });
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
