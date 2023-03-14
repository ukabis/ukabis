using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Policies;
using Unity.Interception.PolicyInjection.Pipeline;

namespace JP.DataHub.MVC.Unity.Attributes
{
    [DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    public class ExceptionHandlerAttribute : HandlerAttribute
    {
        public ExceptionHandlerAttribute()
        {
        }

        public override ICallHandler CreateHandler(IUnityContainer container) => new ExceptionHandler();

        public class ExceptionHandler : ICallHandler
        {
            public ExceptionHandler()
            {
            }

            [DebuggerStepThrough]
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var result = getNext()(input, getNext);
                var task = result.ReturnValue as Task;
                Exception catchexception = task != null ? task.Exception : result.Exception;
                return result;
            }

            public int Order { get => 1; set { } }
        }
    }
}
