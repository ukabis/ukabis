using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;

namespace JP.DataHub.ApiWeb.Domain.Context.Attributes
{
    internal class ExceptionProcessor
    {
        private static Lazy<bool> s_lasyIsInternalServerErrorDetailResponse = new Lazy<bool>(() => UnityCore.Resolve<bool>("IsInternalServerErrorDetailResponse"));
        private static bool IsInternalServerErrorDetailResponse { get => s_lasyIsInternalServerErrorDetailResponse.Value; }

        internal HttpResponseMessage ExceptionToMessage(Exception ex, ErrorCodeMessage.Code code)
        {
            var log = new JPDataHubLogger(typeof(Method));

            var errorMessage = CreateCommonErrorMassage(log, ex);
            var error = ErrorCodeMessage.GetRFC7807(code);
            if (IsInternalServerErrorDetailResponse)
            {
                var errorsMsg = new List<string>();
                foreach (var message in errorMessage.Select((v, i) => (new { Value = v, Index = i })))
                {
                    if (message.Index == 0)
                    {
                        error.Detail = message.Value;
                    }
                    else
                    {
                        errorsMsg.Add(message.Value);
                    }
                }

                if (errorsMsg.Any())
                {
                    error.Errors = new Dictionary<string, dynamic> { { "InnerException", errorsMsg } };
                }

                return new HttpResponseMessage((HttpStatusCode)error.Status) { Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            }
            else
            {
                return new HttpResponseMessage((HttpStatusCode)error.Status) { Content = new StringContent(JsonConvert.SerializeObject(error), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            }
        }

        private static IEnumerable<string> CreateCommonErrorMassage(JPDataHubLogger log, Exception ex)
        {
            if (ex is AggregateException aex)
            {
                return CreateErrorResponseFromAggregateException(log, aex);
            }
            else
            {
                return CreateErrorResponseFromException(log, ex);
            }
        }

        private static IEnumerable<string> CreateErrorResponseFromAggregateException(JPDataHubLogger log, AggregateException ex)
        {
            var exceptionMessage = new List<string>();
            exceptionMessage.Add($"{ ex.Message}");
            foreach (var inner in ex.InnerExceptions)
            {
                exceptionMessage.Add($"{ inner.Message}");
                log.Error(inner.Message);
                log.Error(inner.StackTrace);
            }
            return exceptionMessage;
        }

        internal static IEnumerable<string> CreateErrorResponseFromException(JPDataHubLogger log, Exception ex)
        {
            log.Error(ex.Message);
            log.Error(ex.StackTrace);
            var exceptionMessage = new List<string>();
            exceptionMessage.Add($"{ ex.Message}");
            if (ex.InnerException != null)
            {
                exceptionMessage.Add($"{ ex.InnerException.Message}");
            }
            return exceptionMessage;
        }

    }
}
