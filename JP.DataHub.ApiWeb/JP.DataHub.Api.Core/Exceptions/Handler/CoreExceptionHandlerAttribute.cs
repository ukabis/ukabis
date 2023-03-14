using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Microsoft.AspNetCore.Http;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Resources;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;

namespace JP.DataHub.Api.Core.Exceptions.Handler
{
    [DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]

    internal class CoreExceptionHandlerAttribute : HandlerAttribute
    {
        private List<Type> exception = new List<Type>();
        private List<ErrorCodeMessage.Code> code = new List<ErrorCodeMessage.Code>();

        public CoreExceptionHandlerAttribute(ErrorCodeMessage.Code code)
        {
            this.exception.Add(null);
            this.code.Add(code);
        }

        public CoreExceptionHandlerAttribute(Type exception, ErrorCodeMessage.Code code)
        {
            this.exception.Add(exception);
            this.code.Add(code);
        }

        public override ICallHandler CreateHandler(IUnityContainer container) => new ExceptionHandler(exception, code);

        public class ExceptionHandler : ICallHandler
        {
            private List<Type> exception;
            private List<ErrorCodeMessage.Code> code;

            public ExceptionHandler(List<Type> exception, List<ErrorCodeMessage.Code> code)
            {
                this.exception = exception;
                this.code = code;
            }

            [DebuggerStepThrough]
            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var result = getNext()(input, getNext);
                var task = result.ReturnValue as Task;
                Exception catchexception = task != null ? task.Exception : result.Exception;
                if (catchexception != null && catchexception is not Rfc7807Exception && Top(catchexception) is not Rfc7807Exception)
                {
                    int pos = FindException(catchexception);
                    if (pos == -1) pos = exception.IndexOf(null);
                    if (pos != -1)
                    {
                        var exception = new Rfc7807Exception(code[pos].GetRFC7807(UnityCore.Resolve<IHttpContextAccessor>().HttpContext.Request.Path.Value));
                        if (task != null)
                        {
                            var typeGeneric = input.FindObjectPath("MethodBase.ReturnType.GenericTypeArguments[0]") as Type;
                            // 以下を呼び出したいので、Refelectionで実施
                            //result.ReturnValue = Task.FromException<DynamicApiResponse>(CoreErrorCodeMessage.CreateRfc7807Exception(code[pos], UnityCore.Resolve<IHttpContextAccessor>().HttpContext.Request.Path.Value));
                            var genericMethod = typeof(Task).GetGenericMethod("FromException", typeGeneric);
                            result.ReturnValue = genericMethod == null ? result.ReturnValue : genericMethod.Invoke(task, new object[] { exception });
                        }
                        else
                        {
                            result.Exception = exception;
                        }
                    }
                }
                return result;
            }

            private Exception Top(Exception ex)
            {
                var aex = ex as AggregateException;
                if (aex == null)
                {
                    return null;
                }

                return aex.Flatten().InnerExceptions.FirstOrDefault();

            }

            private int FindException(Exception catchexception)
            {
                var agg = catchexception as AggregateException;
                if (agg != null)
                {
                    foreach (var ie in agg.InnerExceptions)
                    {
                        int pos = FindException(ie);
                        if (pos != -1)
                        {
                            return pos;
                        }
                    }
                }
                else
                {
                    return exception.IndexOf(catchexception.GetType());
                }
                return -1;
            }

            public int Order { get => 1; set { } }
        }
    }
}
