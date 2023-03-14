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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JP.DataHub.SmartFoodChainAOP.Helper;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class JasPrintGetPrintableCountFilter : AbstractApiFilter
    {
        // APIのURL
        private static readonly string JasPrintLogApi = "/API/Traceability/JasFreshnessManagement/V3/Private/JasPrintLog/?$filter=ProductCode eq '{0}'";
        private static readonly string ArrivalApi = "/API/Traceability/V3/Private/Arrival/ODataCertifiedApplication?$filter=contains(ArrivalProduct, '{0}') and ArrivalGln eq '{1}'";

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // URLクエリ取出し
            if (string.IsNullOrEmpty(param.QueryString))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102410, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            var queryStrings = param.QueryStringDic
                .ThrowMessage(x => x.ContainsKey("product") == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102403))
                .ThrowMessage(x => x.ContainsKey("gln") == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102407))
                .ThrowMessage(x => x.ContainsKey("arrivalId") == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102417));
            var product = param.QueryStringDic.GetOrDefault("product");
            var gln = param.QueryStringDic.GetOrDefault("gln");
            var arrivalId = param.QueryStringDic.GetOrDefault("arrivalId");

            // 非同期で実行したいAPI
            var judgmentFunc = this.GetJudgmentHistoryFunc(param, product, gln, arrivalId);
            var productFunc = this.GetProductDetailFunc(param, product);
            Func<List<JasPrintLogModel>> jasPrintLogFunc = () =>
                param.ApiHelper.ExecuteGetApi(string.Format(JasPrintLogApi, Regex.Escape(product)))
                    .ToWebApiResponseResult<List<JasPrintLogModel>>()
                    .ThrowRfc7807(HttpStatusCode.BadRequest)
                    .ThrowMessage(x=>!x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound,m=> param.MakeRfc7807Response(ErrorCodeMessage.Code.E102408))
                    .Result;
            Func<List<ArrivalModel>> arrivalFunc = () =>
                param.ApiHelper.ExecuteGetApi(string.Format(ArrivalApi, Regex.Escape(product), Regex.Escape(gln)))
                    .ToWebApiResponseResult<List<ArrivalModel>>()
                    .ThrowRfc7807(HttpStatusCode.BadRequest)
                    .ThrowMessage(x=>!x.IsSuccessStatusCode,m=> param.MakeRfc7807Response(ErrorCodeMessage.Code.E102415))
                    .Result;
#if (false) // SYNC
            // JAS判定履歴が存在するか？
            var task1 = Task.Run<List<JudgmentModel>>(() => judgmentFunc());
            var tmp1 = task1.Result;
            // 商品コード詳細を取得
            var task2 = Task.Run<List<ProductCodeDetailModel>>(() => productFunc());
            var tmp2 = task2.Result;
            // JAS印刷ログを取得
            var task3 =Task.Run<List<JasPrintLogModel>>(() => jasPrintLogFunc());
            var tmp3 = task3.Result;
            // 入荷データを取得
            var task4 =Task.Run<List<ArrivalModel>>(() => arrivalFunc());
            var tmp4 = task4.Result;
#else // ASYNC
            // JAS判定履歴が存在するか？
            var task1 = Task.Run<List<JudgmentModel>>(() => judgmentFunc());
            // 商品コード詳細を取得
            var task2 = Task.Run<List<ProductCodeDetailModel>>(() => productFunc());
            // JAS印刷ログを取得
            var task3 =Task.Run<List<JasPrintLogModel>>(() => jasPrintLogFunc());
            // 入荷データを取得
            var task4 =Task.Run<List<ArrivalModel>>(() => arrivalFunc());
#endif

            //JAS判定を確認する
            var judgmentList = task1.Result;
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
            var productMaster = task2.Result;
            if (productMaster?.Any() != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102408, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }
            if (string.IsNullOrEmpty(productMaster[0].GtinCode))
            {
                //項目が不正
                Logger.Error($"Required items are empty: GtinCode");
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102408, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            var paramArray = Param.Split(',');
            var numManagement = ProductCodeHelper.GetNumberManagementMethod(product, productMaster[0].GtinCode);
            if (paramArray.All(x=>x!= ProductCodeHelper.GetCode(numManagement)))
            {
                //項目が不正
                Logger.Error($"Incorrect format: ProductCode");
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102406, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }
            var  quantity = productMaster.First().Quantity;

            // 入荷数を取得
            var arrivalList = task4.Result;
            var receivePackageQuantity = 1;
            foreach (var arrivalProduct in arrivalList.First().ArrivalProduct)
            {
                if (arrivalProduct.ProductCode== product)
                {
                    receivePackageQuantity = arrivalProduct.ReceivePackageQuantity;
                    break;
                }
            }

            // 個数を計算
            var count = quantity * receivePackageQuantity;

            //これまでの印刷結果を取得
            var JasPrintLogList = task3.Result;
            var printed = JasPrintLogList?.Select(x => x.PrintCount).Sum() ?? 0;
            var   reprinted = JasPrintLogList?.Select(x => x.ReprintCount).Sum() ?? 0;

            var result = new GetPrintableCountResultModel()
            {
                ProductCode = product,
                Count = count,
                PrintedCount = printed + reprinted,
                PrintableCount = count - printed,
            };

            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json") };
        }
    }
}