using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.SmartFoodChainAOP.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JP.DataHub.SmartFoodChainAOP.Helper;
using System.Linq;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class JasPrintRegisterPrintLogFilter : AbstractApiFilter
    {
        // キャッシュキーの接頭辞
        private static readonly string JudgmentHistoryPrefix = "JasPrintRegisterPrintLogFilter.JudgmentHistoryCache";
        private static readonly string ProductCodeDetailCacheKeyPrefix = "JasPrintRegisterPrintLogFilter.ProductCodeDetailCache";

        // APIのURL
        private static readonly string JasPrintLogApi = "/API/Traceability/JasFreshnessManagement/V3/Private/JasPrintLog/Register";
        private static readonly string GetPrintableCount = "/API/Traceability/JasFreshnessManagement/V3/Public/JasPrint/GetPrintableCount?product={0}&gln={1}&arrivalId={2}";

        // メソッド名
        private static readonly string printApi = "RegisterPrintLog";
        private static readonly string reprintApi = "RegisterRePrintLog";

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // RequestBody取出し
            string contents = "";
            JasPrintLogModel contentsJson;
            if (param.ContentsStream == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102402, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            using (var sr = new StreamReader(param.ContentsStream))
            {
                contents = sr.ReadToEnd();
                if (string.IsNullOrEmpty(contents))
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102402, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
                }

                try
                {
                    contentsJson = JsonConvert.DeserializeObject<JasPrintLogModel>(contents);
                }
                catch (JsonReaderException)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102402, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
                }
            }

            //項目の確認
            if (string.IsNullOrEmpty(contentsJson.ProductCode))
            {
                Logger.Error($"Required items are empty: ProductCode");
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102403, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            if (string.IsNullOrEmpty(contentsJson.LastGln))
            {
                Logger.Error($"Required items are empty: LastGln");
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102407, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            if (string.IsNullOrEmpty(contentsJson.ArrivalId))
            {
                Logger.Error($"Required items are empty: LastGln");
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102417, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            // 非同期で実行したいAPI
            var judgmentFunc = this.GetJudgmentHistoryFunc(param, contentsJson.ProductCode, contentsJson.LastGln, contentsJson.ArrivalId);
            var productFunc = this.GetProductDetailFunc(param, contentsJson.ProductCode);

#if (true) // SYNC
            // JAS判定履歴が存在するか？
            var judgmentList = judgmentFunc();
            // 商品コード詳細を取得
            var product = productFunc();
#else // ASYNC
            // JAS判定履歴が存在するか？
            var task1 = Task.Run<List<JudgmentModel>>(() => judgmentFunc());
            // 商品コード詳細を取得
            var task2 = Task.Run<ProductCodeDetailModel>(() => productFunc());
            Task.WaitAll(task1, task2);
            var judgmentList = task1.Result;
            var product = task2.Result;
#endif

            //JAS判定を確認する
            if (judgmentList?.Any() != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102404, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }
            if (judgmentList.FirstOrDefault()?.Result?.result != true)
            {
                //判定NG
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102405, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            //バーコードの形式が許可されたものか確認
            if (product?.Any() != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102408, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }
            if (string.IsNullOrEmpty(product[0].GtinCode))
            {
                //項目が不正
                Logger.Error($"Required items are empty: GtinCode");
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102408, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            var paramArray = Param.Split(',');
            var numManagement = ProductCodeHelper.GetNumberManagementMethod(contentsJson.ProductCode, product[0].GtinCode);
            if (paramArray.All(x=>x!= ProductCodeHelper.GetCode(numManagement )))
            {
                //項目が不正
                Logger.Error($"Incorrect format: ProductCode");
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102406, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            //枚数の確認
            var jasPrintableCount = param.ApiHelper.ExecuteGetApi(string.Format(GetPrintableCount, Regex.Escape(contentsJson.ProductCode), Regex.Escape(contentsJson.LastGln), Regex.Escape(contentsJson.ArrivalId)))
                    .ToWebApiResponseResult<GetPrintableCountResultModel>()
                    .ThrowRfc7807(HttpStatusCode.BadRequest)
                    .ThrowMessage(x=>!x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound,m=> param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                    .Result;

            if (param.ApiUrl.StartsWith(printApi))
            {
                if(contentsJson.PrintCount>jasPrintableCount.PrintableCount)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102416, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
                }
            }
            else if (param.ApiUrl.StartsWith(reprintApi))
            {
                if(contentsJson.ReprintCount>jasPrintableCount.Count)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102416, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
                }
            }

            //JasPrintLog（JASシール印刷記録）に登録する
            contentsJson.PrintDate = DateTime.UtcNow;
            if (param.ApiUrl.StartsWith(printApi))
            {
                contentsJson.ReprintCount = 0;
                contentsJson.ReprintReason = null;
            }
            else if (param.ApiUrl.StartsWith(reprintApi))
            {
                contentsJson.PrintCount = 0;
            }
            var tmp = contentsJson.ToJsonString();

            var result = param.ApiHelper.ExecutePostApi(JasPrintLogApi, JsonConvert.SerializeObject(contentsJson))
                .ToWebApiResponseResult<RegisterResultModel>()
                .ThrowRfc7807(HttpStatusCode.BadRequest)
                .ThrowMessage(x => !x.IsSuccessStatusCode, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102401));

            return new HttpResponseMessage(result.StatusCode)
            {
                Content = new StringContent(JsonConvert.SerializeObject(result.Result), Encoding.UTF8, "application/json")
            };
        }
    }
}