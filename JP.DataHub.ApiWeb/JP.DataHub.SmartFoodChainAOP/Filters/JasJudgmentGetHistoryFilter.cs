using System.Net;
using System.Text;
using Newtonsoft.Json;
using JP.DataHub.Aop;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Helper;
using JP.DataHub.SmartFoodChainAOP.Logic;
using JP.DataHub.SmartFoodChainAOP.Models;
using System.Text.RegularExpressions;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    internal class JasJudgmentGetHistoryFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            param.QueryStringDic
                .ThrowMessage(x => x.ContainsKey("product") == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102403))
                .ThrowMessage(x => x.ContainsKey("gln") == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102407))
                .ThrowMessage(x => x.ContainsKey("arrivalId") == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102417));
            var productcode = param.QueryStringDic.GetOrDefault("product");
            var glncode = param.QueryStringDic.GetOrDefault("gln");
            var arrivalId = param.QueryStringDic.GetOrDefault("arrivalId");
            return param.ApiHelper.ExecuteGetApi(string.Format("/API/Traceability/JasFreshnessManagement/V3/Public/JudgmentHistory/ODataCertifiedApplication?$filter=ProductCode eq '{0}' and GlnCode eq '{1}' and ArrivalId eq '{2}'", Regex.Escape(productcode), Regex.Escape(glncode), Regex.Escape(arrivalId)));
        }
    }
}
