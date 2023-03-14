using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.Cache.Attributes
{
    //[DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class CacheIdFireAttribute : HandlerAttribute
    {
        public string KeyName { get; private set; }
        public string ObjectPath { get; private set; }

        public CacheIdFireAttribute(object keyName, string objectPath = null)
        {
            KeyName = keyName?.ToString();
            ObjectPath = objectPath ?? KeyName;
        }

        public CacheIdFireAttribute(string keyName, string objectPath = null)
        {
            KeyName = keyName;
            ObjectPath = objectPath ?? KeyName;
        }

        public override ICallHandler CreateHandler(IUnityContainer container) => new Handler(KeyName, ObjectPath);

        private class Handler : ICallHandler
        {
            private Lazy<ICacheManager> _lazyCacheManager = new Lazy<ICacheManager>(() => UnityCore.Resolve<ICacheManager>());
            private ICacheManager _cacheManager { get => _lazyCacheManager.Value; }

            public string KeyName { get; private set; }
            public string ObjectPath { get; private set; }

            public Handler(string keyName, string objectPath)
            {
                KeyName = keyName;
                ObjectPath = objectPath;
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                if (_cacheManager.IsEnableFire == false)
                {
                    return getNext()(input, getNext);
                }

                Task task = null;
                if (string.IsNullOrEmpty(KeyName) == false && string.IsNullOrEmpty(ObjectPath) == false)
                {
                    var cacheManager = UnityCore.Resolve<ICacheManager>();
                    var val = input.Arguments.MethodArgumentToValue(ObjectPath);
                    cacheManager.FireId(KeyName, val);
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