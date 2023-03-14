using System;
using System.Collections.Generic;
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
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class CacheEntityFireAttribute : HandlerAttribute
    {
        public List<string> Entities { get; private set; }

        public CacheEntityFireAttribute()
        {
        }

        public CacheEntityFireAttribute(params object[] enums)
        {
            Entities = new List<string>();
            Entities.AddRange(enums.Select(x => x.ToString()).ToArray());
        }

        public CacheEntityFireAttribute(params string[] entities)
        {
            Entities = new List<string>();
            Entities.AddRange(entities.Select(x => x.ToString()).ToArray());
        }

        public override ICallHandler CreateHandler(IUnityContainer container) => new Handler(Entities);

        private class Handler : ICallHandler
        {
            private Lazy<ICacheManager> _lazyCacheManager = new Lazy<ICacheManager>(() => UnityCore.Resolve<ICacheManager>());
            private ICacheManager _cacheManager { get => _lazyCacheManager.Value; }
            private List<string> Entities;

            public Handler(List<string> entities)
            {
                Entities = entities;
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
                if (Entities != null)
                {
                    candidate.AddRange(Entities);
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
                    //task = Task.Run(() => cacheManager.FireEntityAsync(candidate));
                    cacheManager.FireEntityAsync(candidate);
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