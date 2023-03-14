using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Unity;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.PolicyInjection.Pipeline;
using JP.DataHub.Com.Unity;
using Unity.Interception.Utilities;
using Microsoft.Extensions.Options;

namespace JP.DataHub.MVC.Http
{
    [DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class HttpResponseHeaderAttribute : HandlerAttribute
    {
        private static Lazy<IOptions<List<AdditionalHttpResponseHeader>>> s_lazyHeader = new Lazy<IOptions<List<AdditionalHttpResponseHeader>>>(() => UnityCore.Resolve<IOptions<List<AdditionalHttpResponseHeader>>>());
        private static List<AdditionalHttpResponseHeader> s_header => s_lazyHeader.Value.Value;

        public override ICallHandler CreateHandler(IUnityContainer container) => new HttpResponseHeaderHandler();

        public class HttpResponseHeaderHandler : ICallHandler
        {
            [DebuggerStepThrough]
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var result = getNext()(input, getNext);
                if (result.ReturnValue is HttpResponseMessage msg)
                {
                    s_header?.ForEach(x => msg.Headers.Add(x.Name, x.Value));
                }
                return result;
            }

            public int Order { get => 1; set { } }
        }
    }
}