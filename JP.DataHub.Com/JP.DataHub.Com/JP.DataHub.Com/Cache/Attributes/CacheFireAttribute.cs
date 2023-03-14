using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Utilities;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Cache.Attributes
{
    [DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class CacheFireAttribute : HandlerAttribute
    {
        public List<string> Keys { get; private set; }

        public CacheFireAttribute()
        {
        }

        public CacheFireAttribute(params object[] enums)
        {
            Keys = new List<string>();
            Keys.AddRange(enums.Select(x => x.ToString()).ToArray());
        }

        public CacheFireAttribute(params string[] entities)
        {
            Keys = new List<string>();
            Keys.AddRange(entities.Select(x => x.ToString()).ToArray());
        }

        public override ICallHandler CreateHandler(IUnityContainer container) => new Handler(Keys);

        private class Handler : ICallHandler
        {
            private Lazy<ICacheManager> _lazyCacheManager = new Lazy<ICacheManager>(() => UnityCore.Resolve<ICacheManager>());
            private ICacheManager _cacheManager { get => _lazyCacheManager.Value; }
            private List<string> Keys;

            public Handler(List<string> keys)
            {
                Keys = keys;
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                if (_cacheManager.IsEnableFire == false)
                {
                    return getNext()(input, getNext);
                }

                // キャッシュ削除用
                Task task = null;

                // キャッシュを消す
                var target = input.Target;
                List<string> candidate = new List<string>();
                if (Keys != null)
                {
                    candidate.AddRange(Keys);
                }
                if (target is ICacheFire fire)
                {
                    candidate = fire.CacheKeys(candidate).ToList();
                }
                for (int i = 0; i < candidate.Count; i++)
                {
                    string key = candidate[i];
                    if (key.StartsWith('{') && key.EndsWith('}'))
                    {
                        key = input.Target.FindObjectPath(key.Substring(1, key.Length - 2))?.ToString();
                    }
                    candidate[i] = key;

                }
                if (candidate?.Count > 0)
                {
                    var cacheManager = UnityCore.Resolve<ICacheManager>();
                    task = Task.Run(() => cacheManager.FireKeyAsync(candidate));
                }

                var result = getNext()(input, getNext);
                if (task != null)
                {
                    task.Wait();
                }
                return result;
            }

            public int Order { get => 1; set { } }
        }
    }
}