using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Aop;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;

namespace JP.DataHub.SmartFoodChainAOP.Extensions
{
    public static class IApiFilterActionParamExExtensions
    {
        public static AopResponseException MakeRfc7807Response(this IApiFilterActionParam param, ErrorCodeMessage.Code code)
            => new AopResponseException(ErrorCodeMessage.GetRFC7807HttpResponseMessage(code, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}"));

        public static HttpResponseMessage GetRFC7807HttpResponseMessage(this IApiFilterActionParam param, ErrorCodeMessage.Code code)
            => ErrorCodeMessage.GetRFC7807HttpResponseMessage(code, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
    }
}
