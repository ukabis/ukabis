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
    internal class ExceptionHandlingAttribute : HandlerAttribute
    {
        private Type _exception = null;
        private ErrorCodeMessage.Code _code = default;
        private ConvertType _convertType;

        public ExceptionHandlingAttribute(Type exception, ErrorCodeMessage.Code code, ConvertType convertType = default)
        {
            this._exception = exception;
            this._code = code;
            this._convertType = convertType;
        }

        public override ICallHandler CreateHandler(IUnityContainer container) => new ExceptionHandler(_exception, _code, _convertType);

        public class ExceptionHandler : ICallHandler
        {
            private Lazy<IHttpContextAccessor> _lazyHttpContextAccessor = new Lazy<IHttpContextAccessor>(() => UnityCore.Resolve<IHttpContextAccessor>());
            private IHttpContextAccessor _httpContextAccessor => _lazyHttpContextAccessor.Value;

            private Type _exception;
            private ErrorCodeMessage.Code _code;
            private ConvertType _convertType;
            private ExceptionConverter _excConverter = new ExceptionConverter();
            private string _needToModifyErrorApiListStr => UnityCore.Resolve<string>("NeedToModifyErrorApiList");

            public ExceptionHandler(Type exception, ErrorCodeMessage.Code code, ConvertType convertType)
            {
                this._exception = exception;
                this._code = code;
                this._convertType = convertType;
            }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                var result = getNext()(input, getNext);
                if (result.Exception == null || _exception != result.Exception.GetType())
                {
                    return result;
                }

                var retVal = _excConverter.Convert(ErrorCodeMessage.GetRFC7807(_code, null), result.Exception, _convertType);

                if ((result.Exception is QuerySyntaxErrorException || result.Exception is ODataException) && IsNeedToModifyErrorApi())
                {
                    var json = JObject.Parse(retVal.Content.ReadAsStringAsync().Result);
                    json.Remove("detail");
                    json.Remove("instance");
                    json["title"] = "クエリが失敗しました";
                    retVal = new HttpResponseMessage(retVal.StatusCode)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, retVal.Content.Headers.ContentType.MediaType)
                    };
                }
                result.ReturnValue = retVal;
                result.Exception = null;
                return result;
            }

            public int Order { get => 1; set { } }

            private bool IsNeedToModifyErrorApi()
            {
                var path = _httpContextAccessor.HttpContext.Request.Path;
                if (string.IsNullOrEmpty(path))
                {
                    return false;
                }
                return !string.IsNullOrEmpty(_needToModifyErrorApiListStr) && _needToModifyErrorApiListStr.Split(';').Any(x => Regex.IsMatch(path, $"^{x}$"));
            }
        }
    }
}
