using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;

namespace JP.DataHub.ApiWeb.Domain.Context.Attributes
{
    internal class ExceptionConverter
    {

        bool IsInternalServerErrorDetailResponse => UnityCore.Resolve<bool>("IsInternalServerErrorDetailResponse");

        internal HttpResponseMessage Convert(RFC7807ProblemDetailExtendErrors error, Exception exception, ConvertType convertType)
        {
            var methodList = typeof(ExceptionConverter).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
            var targetMethod = methodList.Where(p => p.Name == convertType.ToString()).FirstOrDefault();
            return (HttpResponseMessage)targetMethod.Invoke(this, new object[] { error, exception });
        }

        private HttpResponseMessage HttpResponseDirect(RFC7807ProblemDetailExtendErrors error, Exception exception)
        {
            var apiex = exception as ApiException;
            if (apiex != null)
            {
                error.Detail = apiex.HttpResponseMessage?.Content?.ReadAsStringAsync()?.Result?.ToString();
                return new HttpResponseMessage((HttpStatusCode)error.Status) { Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            }

            var httpex = exception as HttpResponseException;
            if (httpex != null)
            {
                error.Detail = httpex.Response?.Content?.ReadAsStringAsync()?.Result?.ToString();
                return new HttpResponseMessage((HttpStatusCode)error.Status) { Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            }

            return new ExceptionProcessor().ExceptionToMessage(exception, (ErrorCodeMessage.Code)Enum.Parse(typeof(ErrorCodeMessage.Code), error.ErrorCode));
        }

        private HttpResponseMessage ExeptionMsgToDetail(RFC7807ProblemDetailExtendErrors error, Exception exception)
        {
            error.Detail = exception.Message;
            return new HttpResponseMessage((HttpStatusCode)error.Status) { Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
        }

        private HttpResponseMessage ExeptionMsgToDetailWithLog(RFC7807ProblemDetailExtendErrors error, Exception exception)
        {
            var log = new JPDataHubLogger(typeof(Method));
            log.Error(exception.Message);
            log.Error(exception.StackTrace);
            return ExeptionMsgToDetail(error, exception);
        }

        private HttpResponseMessage AddInnerExeptionMessage(RFC7807ProblemDetailExtendErrors error, Exception exception)
        {
            var aggreex = exception as AggregateException;
            return new ExceptionProcessor().ExceptionToMessage(aggreex, (ErrorCodeMessage.Code)Enum.Parse(typeof(ErrorCodeMessage.Code), error.ErrorCode));
        }

        private HttpResponseMessage RoslynScriptRuntimeError(RFC7807ProblemDetailExtendErrors error, Exception exception)
        {
            var re = exception as RoslynScriptRuntimeException;
            HttpResponseMessage res;

            if (IsInternalServerErrorDetailResponse)
            {
                res = new ExceptionProcessor().ExceptionToMessage(re, (ErrorCodeMessage.Code)Enum.Parse(typeof(ErrorCodeMessage.Code), error.ErrorCode));
            }
            else
            {
                //既存処理ではログを保存しないためExceptionToMessageは使えない
                var errorTemp = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E99999);
                error.Title = errorTemp.Title;
                res = new HttpResponseMessage((HttpStatusCode)error.Status) { Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            }
            if (!string.IsNullOrEmpty(re.ScriptRuntimeLogId)) { res.Headers.Add("X-ScriptRuntimeLog-Id", re.ScriptRuntimeLogId); }
            return res;
        }

        internal HttpResponseMessage Rfc7807ToDetail(RFC7807ProblemDetail error)
        {
            return new HttpResponseMessage((HttpStatusCode)error.Status) { Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
        }

        internal HttpResponseMessage Rfc7807ToDetail(RFC7807ProblemDetailExtendErrors error)
        {
            return new HttpResponseMessage((HttpStatusCode)error.Status) { Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
        }
    }
}
