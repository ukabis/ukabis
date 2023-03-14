using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.Context.Aop
{
    [DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    internal class DynamicApiActionFilterAttribute : HandlerAttribute
    {
        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new DynamicApiActionHandler();
        }


        public class DynamicApiActionHandler : ICallHandler
        {
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                IMethodReturn result = null;
                try
                {
                    // 非同期リクエスト時にはActionInjectiorは実行しない。
                    var action = input.Target as IDynamicApiAction;
                    if (action != null && action.ActionInjectorHandler != null && action.ActionType.Value != DynamicApi.ActionType.Async)
                    {
                        action.ActionInjectorHandler.Target = action;
                        action.ActionInjectorHandler.Execute(() =>
                        {
                            result = getNext()(input, getNext);
                            action.ActionInjectorHandler.ReturnValue = result.ReturnValue;
                        });
                        if (result == null)
                        {
                            result = new VirtualMethodReturn(input, "OK", new object[1]);
                        }
                        result.ReturnValue = action.ActionInjectorHandler.ReturnValue;
                    }
                    else
                    {
                        result = getNext()(input, getNext);
                    }
                }
                catch (Exception)
                {
                    //new JPDataHubLogger(input.MethodBase.DeclaringType.FullName).Error(e);
                    throw;
                }
                return result;
            }

            public int Order
            {
                get { return 1; }
                set { }
            }
        }
    }
}
