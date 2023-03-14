using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.Utilities;
using JP.DataHub.Com.Net.Http;

namespace JP.DataHub.Com.Net.Http.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AutoGenerateReturnModelAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container) => new AutoGenerateReturnModelHandler();

        public AutoGenerateReturnModelAttribute()
        {
        }

        internal class AutoGenerateReturnModelHandler : ICallHandler
        {
            public int Order { get => 1; set { } }

            public AutoGenerateReturnModelHandler()
            {
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var result = getNext()(input, getNext);
                return input.CreateMethodReturn(Resource.MakeApiResult(input, (input.Target as Resource)?.ServerUrl));
            }
        }
    }
}
