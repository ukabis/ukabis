using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Cache;
using MessagePack;
using Microsoft.AspNetCore.Http;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Utilities;

namespace JP.DataHub.Com.Cache.Attributes
{
    //[DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class CacheAttribute : HandlerAttribute
    {
        public string KeyPrefix { get; set; }
        private string Name;
        private TimeSpan? Expiration;
        private int _maxSaveSize;
        private Type Type;
        
        public CacheAttribute() => SetParam(null, null);
        public CacheAttribute(string keyPrefix) => SetParam(null, keyPrefix);
        public CacheAttribute(string keyPrefix, int secExpiration) => SetParam(null, keyPrefix, new TimeSpan(0, 0, secExpiration <= 0 ? AbstractCache.CACHE_EXPIRATION_SECOND : secExpiration));
        public CacheAttribute(string keyPrefix, TimeSpan timeSpan) => SetParam(null, keyPrefix, timeSpan);
        public CacheAttribute(string name, string keyPrefix) => SetParam(name, keyPrefix);
        public CacheAttribute(string name, string keyPrefix, int secExpiration) => SetParam(name, keyPrefix, new TimeSpan(0, 0, secExpiration <= 0 ? AbstractCache.CACHE_EXPIRATION_SECOND : secExpiration));
        public CacheAttribute(string name, string keyPrefix, TimeSpan timeSpan) => SetParam(name, keyPrefix, timeSpan);

        public CacheAttribute(string name, string keyPrefix, TimeSpan timeSpan, int maxSaveSize) => SetParam(name, keyPrefix, timeSpan, maxSaveSize);
        public CacheAttribute(string name, string keyPrefix, int secExpiration, int maxSaveSize) => SetParam(name, keyPrefix, new TimeSpan(0, 0, secExpiration <= 0 ? AbstractCache.CACHE_EXPIRATION_SECOND : secExpiration), maxSaveSize);

        public CacheAttribute(Type type) => SetParamByType(type);
        public CacheAttribute(Type type, int secExpiration) => SetParamByType(type, new TimeSpan(0, 0, secExpiration <= 0 ? AbstractCache.CACHE_EXPIRATION_SECOND : secExpiration));
        public CacheAttribute(Type type, TimeSpan timeSpan) => SetParamByType(type, timeSpan);
        public CacheAttribute(Type type, int secExpiration, int maxSaveSize) => SetParamByType(type, new TimeSpan(0, 0, secExpiration <= 0 ? AbstractCache.CACHE_EXPIRATION_SECOND : secExpiration), maxSaveSize);
        public CacheAttribute(Type type, TimeSpan timeSpan, int maxSaveSize) => SetParamByType(type, timeSpan, maxSaveSize);

        private void SetParamByType(Type type, TimeSpan? timeSpan = null, int maxSaveSize = default)
        {
            Type = type;
            Expiration = timeSpan;
            _maxSaveSize = maxSaveSize;
        }

        private void SetParam(string name, string keyPrefix, TimeSpan? timeSpan = null, int maxSaveSize = default)
        {
            Name = name;
            KeyPrefix = keyPrefix;
            Expiration = timeSpan;
            _maxSaveSize = maxSaveSize;
        }

        public override ICallHandler CreateHandler(IUnityContainer container) => new CacheHandler(container, Name, Expiration, KeyPrefix, type : Type);

        public class CacheHandler : ICallHandler
        {
            private Lazy<ICacheManager> _lazyCacheManager = new Lazy<ICacheManager>(() => UnityCore.Resolve<ICacheManager>());
            private ICacheManager _cacheManager { get => _lazyCacheManager.Value; }

            protected ICache cache;
            private TimeSpan? Expiration;
            private string KeyPrefix;
            private string Name;
            private int MaxSaveSize;
            private Type Type;

            public CacheHandler(IUnityContainer unityContainer, string name, TimeSpan? absoluteExpiration, string keyPrefix, int maxSaveSize = default, Type type = null)
            {
                cache = string.IsNullOrEmpty(name) ? unityContainer.ResolveOrDefault<ICache>() : unityContainer.ResolveOrDefault<ICache>(name);
                Expiration = absoluteExpiration;
                KeyPrefix = keyPrefix;
                Name = name;
                MaxSaveSize = maxSaveSize;
                Type = type;
            }

            //[DebuggerStepThrough]
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                if (_cacheManager.IsEnable == false)
                {
                    return getNext()(input, getNext);
                }

                // キャッシュキー Prefixの指定が存在しない場合は、「クラス名-メソッド名」とする※ユニークになる確率が高いため
                if (string.IsNullOrEmpty(KeyPrefix))
                {
                    var implType = input.Target?.GetType();
                    var className = implType.Name;
                    KeyPrefix = $"{className}-{input.MethodBase.Name}";
                }

                string keyPrefix = CalcKeyPrefix(KeyPrefix, input.Target);
                Type = Type ?? typeof(DefaultCacheKey);

                // キャッシュキーの作成
                // IModifyCacheがある場合、それを呼び出してキャッシュキーの変更をする必要があるか確認する
                var paramCacheKey = CacheHelper.CreateCacheKeyParam(keyPrefix, input.MethodBase as MethodInfo, Type, input.Target, input.Arguments, input.Inputs.Cast<object>().ToList());
                bool result = true;
                // ターゲット（属性をしているクラス）でICacheKeyを実装していれば、ターゲットにキャッシュするか？しないかを確認する
                if (input.Target is ICacheKey modify)
                {
                    result = modify.ModifyCacheKey(paramCacheKey);
                }
                // ICacheKeyのターゲットがキャッシュしない、または、Typeの指定でキャッシュしない場合は、デフォルトのメソッドを呼び出し、それを返す
                if (result == false || paramCacheKey.IsNoCache == true)
                {
                    // キャッシュキーの文字列が無い場合は、キャッシュしないという意味なので、何もせず次をコールする
                    return getNext()(input, getNext);
                }

                var key = paramCacheKey.Rebuild();      // キャッシュキーの取得
                var returnType = input.MethodBase.GetPropertyValue<Type>("ReturnType");
                // voidの場合はキャッシュする必要がない
                // キーがnull or string.empty or [null]
                if (returnType == typeof(void) || string.IsNullOrEmpty(key) == true || key == "[null]")
                {
                    return getNext()(input, getNext);
                }
                var obj = GetCache(returnType, key);
                if (obj != null)
                {
                    var isNull = obj switch
                    {
                        string cacheStr => cacheStr == AbstractCache.NullValue,
                        _ => false,
                    };
                    return input.CreateMethodReturn(isNull ? null : obj);
                }
                var action = getNext()(input, getNext);
                if (action.Exception != null)
                {
                    return action;
                }
                
                //キャッシュ対象のActionがIWantCacheを実装するならキャッシュ可否を確認する
                if (input.Target is IWantCache condition && !condition.IsCache())
                {
                    return action;
                }

                if (key != null)
                {
                    if (Expiration == null)
                    {
                        Expiration = UnityCore.ResolveOrDefault<TimeSpan>(Name == null ? "Cache.Expiration.Default" : $"Cache.Expiration.{Name}", new TimeSpan(0, 0, AbstractCache.CACHE_EXPIRATION_SECOND));
                    }
                    AddCache(key, action.ReturnValue, Expiration.Value, MaxSaveSize);
                }

                return action;
            }

            public int Order { get => 1; set { } }

            /// <summary>
            /// KeyPrefixが動的な場合の処理
            /// [Cache("abcdefg")]　は通常の固定文字
            /// [Cache("{abc.def}")] は動的。Taretオブジェクトからabcプロパティのdefプロパティの値をキー名とする  
            /// </summary>
            /// <param name="prefix"></param>
            /// <param name="target"></param>
            /// <returns></returns>
            private string CalcKeyPrefix(string prefix, object target)
            {
                if (prefix?.StartsWith('{') == true && prefix?.EndsWith('}') == true)
                {
                    prefix = target.FindObjectPath(prefix.Substring(1, prefix.Length - 2))?.ToString();
                }
                return prefix;
            }

            private void AddCache(string key, object obj, TimeSpan absoluteExpiration, int maxSaveSize)
            {
                if (cache != null)
                {
                    cache.Add(key, obj, absoluteExpiration, maxSaveSize);
                }
            }

            private object GetCache(Type type, string key)
            {
                if (cache == null)
                {
                    return null;
                }
                try
                {
                    return key == null ? null : cache.Get(type, key);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}