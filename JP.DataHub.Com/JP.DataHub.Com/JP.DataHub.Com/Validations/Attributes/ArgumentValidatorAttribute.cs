using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.PolicyInjection.Pipeline;

namespace JP.DataHub.Com.Validations.Attributes
{
    [DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class ArgumentValidatorAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container) => new ArgumentValidatorHandler();

        public class ArgumentValidatorHandler : ICallHandler
        {
            [DebuggerStepThrough]
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                input.ExceptionValidateValue();
                return getNext()(input, getNext);
            }

            public int Order { get => 1; set { } }
        }
    }
}