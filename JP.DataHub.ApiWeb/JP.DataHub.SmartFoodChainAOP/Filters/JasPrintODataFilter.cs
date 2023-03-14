#if (false) // このAOPは不要になったため
using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using JP.DataHub.SmartFoodChainAOP.Shared;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class JasPrintODataFilter : AbstractApiFilter
    {
        private static readonly string JasPrintLogApi = "/API/Traceability/JasFreshnessManagement/V3/Private/JasPrintLog/OData";

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // URLクエリ取出し
            if (string.IsNullOrEmpty(param.QueryString))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102410, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            if (param.QueryStringDic.IsNullOrEmpty("$filter"))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102403, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }
            var filter = param.QueryStringDic.GetOrDefault("$filter");
            if (!filter.Contains("ProductCode"))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102403, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            // 印刷結果を取得
            try
            {                                        
                var result = param.ApiHelper.ExecuteGetApi(JasPrintLogApi, null, param.QueryString)
                    .ToWebApiResponseResult<List<GetJasPrintLogModel>>()
                    .ThrowRfc7807(HttpStatusCode.BadRequest)
                    .ThrowMessage(x => x.StatusCode == HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102409))
                    .ThrowMessage(x=>!x.IsSuccessStatusCode,m=> param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408));

                foreach (var row in result.Result.Where(row => row._Owner_Id != param.OpenId))
                {
                    row.PrintUser = null;
                    row._Owner_Id = null;
                }

                return new HttpResponseMessage(result.StatusCode){Content = new StringContent(JsonConvert.SerializeObject(result.Result), Encoding.UTF8, "application/json")};
            }
            catch
            {
                Logger.Error($"Failed at {JasPrintLogApi}. Requested by OpenID: {param.OpenId}");
                throw;
            }
        }
    }
}
#endif