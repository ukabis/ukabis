using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.Api.Core.Exceptions;

namespace JP.DataHub.ApiWeb.Domain.Context.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    internal class AllExceptionHandlingAttribute : HandlerAttribute
    {
        private static ExceptionProcessor s_exceptionProcessor = new ExceptionProcessor();
        private ErrorCodeMessage.Code _code = default;
        private ConvertType _exceptionMessageType;

        public AllExceptionHandlingAttribute(ErrorCodeMessage.Code code, ConvertType exceptionMessageType = default)
        {
            this._code = code;
            this._exceptionMessageType = exceptionMessageType;
        }

        public override ICallHandler CreateHandler(IUnityContainer container) => new ExceptionHandler(_code, _exceptionMessageType);

        public class ExceptionHandler : ICallHandler
        {
            private ErrorCodeMessage.Code code;
            private ConvertType exceptionMessageType;

            public ExceptionHandler(ErrorCodeMessage.Code code, ConvertType exceptionMessageType = default)
            {
                this.code = code;
                this.exceptionMessageType = exceptionMessageType;
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var result = getNext()(input, getNext);
                if (result.Exception == null)
                {
                    return result;
                }

                result.ReturnValue = new ExceptionProcessor().ExceptionToMessage(result.Exception, ErrorCodeMessage.Code.E99999);
                result.Exception = null;
                return result;
            }

            public int Order { get => 1; set { } }
        }
    }
}