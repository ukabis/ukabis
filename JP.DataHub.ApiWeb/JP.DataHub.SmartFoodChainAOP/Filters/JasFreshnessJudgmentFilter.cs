#define ASYNC
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

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class JasFreshnessJudgmentFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // 入荷ID

            param.QueryStringDic
                .ThrowMessage(x => x.ContainsKey("product") == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102403))
                .ThrowMessage(x => x.ContainsKey("gln") == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102407))
                .ThrowMessage(x => x.ContainsKey("arrivalId") == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E102417));
            var productcode = param.QueryStringDic.GetOrDefault("product");
            var glncode = param.QueryStringDic.GetOrDefault("gln");
            var arrivalId = param.QueryStringDic.GetOrDefault("arrivalId");

#if (SYNC)
            /* SYNC */
            // JAS判定履歴が存在するか？
            var judgmentHistory = this.GetJudgmentHistoryFunc(param, productcode, glncode)();
            // 過去に判定していればそれを返す
            if (judgmentHistory?.Any() == true)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(judgmentHistory[0].Result), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            }
            // 商品コード詳細を取得
            var productCodeDetailList = this.GetProductCodeDetailFunc(param, productcode)();
            if (productCodeDetailList?.Any() != true)
            {
               return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102411);
            }
            var productCodeDetail = productCodeDetailList[0];
#else
            /* ASYNC */
            // JAS判定履歴が存在するか？
            var judgmentHistoryTask = this.GetJudgmentHistoryAsync(param, productcode, glncode, arrivalId);
            // 商品コード詳細を取得
            var productCodeDetailListTask = this.GetProductCodeDetailAsync(param, productcode);
            // トレース情報を取得
            var traceabilityTask = this.GetShipmentAndArrivalAsync(param, productcode, arrivalId);

            // 過去に判定していればそれを返す
            var judgmentHistory = judgmentHistoryTask.Result;
            if (judgmentHistory?.Any() == true)
            {
                return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(judgmentHistory[0].Result), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            }

            // 商品のシリアル番号に対する情報を取得
            var productCodeDetailList = productCodeDetailListTask.Result;
            if (productCodeDetailListTask.Result?.Any() != true)
            {
               return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102411);
            }
            var productCodeDetail = productCodeDetailList[0];

            // トレーサビリティ情報がなければエラー
            var traceability = traceabilityTask.Result;
            if (traceability == null)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102418);
            }
#endif

            // GTINコードから作物や品種などを取得
            var productList = this.GetProductFunc(param, productCodeDetail.GtinCode)();
            if (productList?.Any() != true)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102411);
            }
            var product = productList[0];

            // JAS判定閾値データの取得
            var jasFreshnessJudgment = this.GetFreshnessManagementFunc(param, product.Profile.CropCode)();
            if (jasFreshnessJudgment?.Any() != true)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102412);
            }
            var freshnessJudgment = FindJudgment(jasFreshnessJudgment, product.Profile);
            if (freshnessJudgment == null)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102412);
            }

            // センサーデータの取得
            var sensorRawData = this.GetShipmentSensorRowDataFunnc(param, productcode, glncode)();
            if (sensorRawData?.Any() != true)
            {
                return param.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E102413);
            }
            sensorRawData = sensorRawData ?? new List<ShipmentSensorRawDataModel>();

#if (SYNC)
            /* SYNC */
            // 測定項目の取得
            var observationProperties = this.GetObservationPropertiesFunc(param)();
            // 測定単位の取得（これらは温度の変換のために必要）
            var measurementUnits = this.GetMeasurementUnitsFunc(param)();
#else
            /* ASYNC */
            // 測定項目の取得
            var observationPropertiesTask = this.GetObservationPropertiesAsync(param);
            // 測定単位の取得（これらは温度の変換のために必要）
            var measurementUnitsTask = this.GetMeasurementUnitsAsync(param);

            Task.WaitAll(observationPropertiesTask, measurementUnitsTask);
            var observationProperties = observationPropertiesTask.Result;
            var measurementUnits = measurementUnitsTask.Result;
#endif

            var resultJudgment = freshnessJudgment.Judgment(observationProperties, measurementUnits, sensorRawData);

            // 履歴に登録
            var result = new JudgmentResultModel() { result = resultJudgment.Any() == false, fails = resultJudgment };
            var history = new JudgmentModel() { ProductCode = productcode, GlnCode = glncode, Result = result };
            var tmp = JsonConvert.SerializeObject(result);
            this.RegisterJudgmentHistoryFunc(param, history)();

            // JAS判定がOKなら「フードチェーン情報公表JAS」(fci)を付与する
            if (result.result == true)
            {
                this.RegisterJasCertificationHistoryFunc(param, productcode, glncode, "fci")();
            }

            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, MediaTypeConst.ApplicationJson) };
        }

        /// <summary>
        /// 作物に関するJAS判定基準から、今回の商品（属性）にあった判定を１つ選択する（商品属性によって優先度が違う）
        /// 優先度は次の順番（前提条件としてCropCodeが合っていること）
        /// 1. 判定基準の利用期限が合致している？（利用期間範囲外なら使えない）
        /// 2. 品種とブランドが合っているもの
        /// 3. 品種が合っているもの
        /// 4. ブランドが合っているもの
        /// 5. 品種もブランドも指定が無いもの
        /// 6. 何でもよい１つ
        /// </summary>
        /// <param name="freshnessJudgmentList"></param>
        /// <param name="profile"></param>
        /// <returns></returns>
        private CropFreshnessJudgmentModel FindJudgment(List<CropFreshnessJudgmentModel> freshnessJudgmentList, ProductProfileModel profile)
        {
            // 利用期間内か？
            var now = DateTime.Now;
            freshnessJudgmentList = freshnessJudgmentList.Where(x => x.StartDate <= now && now <= x.EndDate).ToList();
            if (freshnessJudgmentList?.Any() != true)
            {
                return null;
            }
            // 品種とブランド合っているもの（複数あれば何れか１つ）
            List<CropFreshnessJudgmentModel> tmp;
            if (profile.BrandCode != null && profile.BreedCode != null)
            {
                tmp = freshnessJudgmentList.Where(x => x.BreedCode == profile.BreedCode && x.BrandCode == profile.BrandCode).ToList();
                if (tmp.Any())
                {
                    return tmp.First();
                }
            }
            // 品種が合っているもの（複数あれば何れか１つ）
            if (profile.BreedCode != null)
            {
                tmp = freshnessJudgmentList.Where(x => x.BreedCode == profile.BreedCode).ToList();
                if (tmp.Any())
                {
                    return tmp.First();
                }
            }
            // ブランドが合っているもの（複数あれば何れか１つ）
            if (profile.BrandCode != null)
            {
                tmp = freshnessJudgmentList.Where(x => x.BrandCode == profile.BrandCode).ToList();
                if (tmp.Any())
                {
                    return tmp.First();
                }
            }
            // 品種もブランドも指定が無いもの（複数あれば何れか１つ）
            tmp = freshnessJudgmentList.Where(x => x.BrandCode == null && x.BreedCode == null).ToList();
            if (tmp.Any())
            {
                return tmp.First();
            }
            // リストの何れか
            return freshnessJudgmentList.FirstOrDefault();
        }
    }
}
