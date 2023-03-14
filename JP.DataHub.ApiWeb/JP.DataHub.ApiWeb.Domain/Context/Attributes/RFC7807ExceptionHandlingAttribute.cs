using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Interception.PolicyInjection.Policies;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;

namespace JP.DataHub.ApiWeb.Domain.Context.Attributes
{
    // .NET6
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
    internal class RFC7807ExceptionHandlingAttribute : HandlerAttribute
    {
        private ConvertType _convertType;

        public RFC7807ExceptionHandlingAttribute(ConvertType convertType = default)
        {
            this._convertType = convertType;
        }

        public override ICallHandler CreateHandler(IUnityContainer container) => new RFC7807ExceptionHandler(_convertType);

        public class RFC7807ExceptionHandler : ICallHandler
        {
            private ConvertType _convertType;
            private ExceptionConverter _exceptionConverter = new ExceptionConverter();

            public RFC7807ExceptionHandler(ConvertType convertType)
            {
                this._convertType = convertType;
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var result = getNext()(input, getNext);

                var exception = result.Exception as Rfc7807Exception ?? result.Exception?.InnerException as Rfc7807Exception;
                if (exception == null)
                {
                    return result;
                }
                result.ReturnValue = _exceptionConverter.Rfc7807ToDetail(exception.Rfc7807);
                result.Exception = null;
                return result;
            }

            public int Order { get => 1; set { } }
        }
    }
}
