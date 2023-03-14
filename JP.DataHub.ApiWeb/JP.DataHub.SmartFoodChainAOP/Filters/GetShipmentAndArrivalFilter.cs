using JP.DataHub.Aop;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.SmartFoodChainAOP.ErrorCode;
using JP.DataHub.SmartFoodChainAOP.Models;
using JP.DataHub.SmartFoodChainAOP.Extensions;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace JP.DataHub.SmartFoodChainAOP.Filters
{
    public class GetShipmentAndArrivalFilter : AbstractApiFilter
    {
        // キャッシュキーの接頭辞
        private static readonly string CompanyCacheKeyPrefix = "CompanyCache"; 
        private static readonly string OfficeCacheKeyPrefix = "OfficeCache";

        // API
        private static readonly string ShipmentApi = "/API/Traceability/V3/Private/Shipment/GetByProductCode?ProductCode={0}";
        private static readonly string ArrivalApi = "/API/Traceability/V3/Private/Arrival/GetByProductCode?ProductCode={0}";
        private static readonly string CompanyApi = "/API/CompanyMaster/V3/Private/Company/ODataOtherAccessible?$filter=CompanyId in ({0})";
        private static readonly string OfficeApi = "/API/CompanyMaster/V3/Private/Office/ODataOtherAccessible?$filter=OfficeId in ({0})";

        private const string ShipmentType = "shp";
        private const string ArrivalType = "arv";

        private Dictionary<string, List<ShipmentModel>> ShipmentDic = new Dictionary<string, List<ShipmentModel>>();
        private Dictionary<string, List<ArrivalModel>> ArrivalDic = new Dictionary<string, List<ArrivalModel>>();
        private List<GetShipmentAndArrivalResultModel> TraceResult = new List<GetShipmentAndArrivalResultModel>();


        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            // URLクエリ取出し
            if (string.IsNullOrEmpty(param.QueryString))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104400, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            param.QueryStringDic
                .ThrowMessage(x => x.ContainsKey("ProductCode") == false, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E104400));
            var lastProductCode = param.QueryStringDic.GetOrDefault("ProductCode");
            var lastArrivalId = param.QueryStringDic.GetOrDefault("ArrivalId");

            // 指定の入荷から指定の商品コードでトレースバック
            TraceBack(param, lastArrivalId, lastProductCode);

            // 最初の出荷まで辿れていなければ最初の出荷からトレースフォワード
            var firstProductCode = TraceResult.LastOrDefault()?.ProductCode ?? lastProductCode;
            var firstShipment = ShipmentDic[firstProductCode].OrderBy(x => x.ShipmentDate).FirstOrDefault();

            if (firstShipment != null &&
                !TraceResult.Any(x => x.ShipmentId == firstShipment.ShipmentId))
            {
                TraceForward(param, firstShipment.ShipmentId, firstProductCode);
            }

            // トレースできていなければNotFound
            if (!TraceResult.Any())
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E104403, param.LanguageInfo, $"{param.ResourceUrl}/{param.ApiUrl}");
            }

            // 事業者の情報を取得する
            FillCompanyAndOfficeName(param);

            TraceResult = TraceResult.OrderBy(x => x.DateTime).ToList();

            return new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(JsonConvert.SerializeObject(TraceResult), Encoding.UTF8, "application/json") };
        }


        /// <summary>
        /// 出荷を取得する
        /// </summary>
        private async Task<List<ShipmentModel>> GetShipmentsByProductCodeAsync(IApiFilterActionParam param, string productCode)
        {
            return (await param.ApiHelper.ExecuteGetApiAsync(string.Format(ShipmentApi, productCode)))
                    .ToWebApiResponseResult<List<ShipmentModel>>()
                    .ThrowRfc7807(HttpStatusCode.BadRequest)
                    .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E104402))
                    .Result;
        }

        /// <summary>
        /// 入荷を取得する
        /// </summary>
        private async Task<List<ArrivalModel>> GetArrivalsByProductCodeAsync(IApiFilterActionParam param, string productCode)
        {
            return (await param.ApiHelper.ExecuteGetApiAsync(string.Format(ArrivalApi, productCode)))
                    .ToWebApiResponseResult<List<ArrivalModel>>()
                    .ThrowRfc7807(HttpStatusCode.BadRequest)
                    .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E104402))
                    .Result;
        }

        /// <summary>
        /// 事業者を取得する
        /// </summary>
        private async Task<List<CompanyModel>> GetCompaniesAsync(IApiFilterActionParam param, IEnumerable<string> companyIds)
        {
            var cacheKey = string.Join("_", companyIds);
            var inPhrase = string.Join(",", companyIds.Select(x => $"'{x}'"));

            return await Task.Run(() => 
                CacheHelper.GetOrAdd($"{CompanyCacheKeyPrefix}.{cacheKey}", () =>
                    param.ApiHelper.ExecuteGetApi(string.Format(CompanyApi, inPhrase))
                        .ToWebApiResponseResult<List<CompanyModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E104402))
                        .Result));
        }

        /// <summary>
        /// 事業所を取得する
        /// </summary>
        private async Task<List<OfficeModel>> GetOfficesAsync(IApiFilterActionParam param, IEnumerable<string> officeIds)
        {
            var cacheKey = string.Join("_", officeIds);
            var inPhrase = string.Join(",", officeIds.Select(x => $"'{x}'"));

            return await Task.Run(() =>
                CacheHelper.GetOrAdd($"{OfficeCacheKeyPrefix}.{cacheKey}", () =>
                    param.ApiHelper.ExecuteGetApi(string.Format(OfficeApi, inPhrase))
                        .ToWebApiResponseResult<List<OfficeModel>>()
                        .ThrowRfc7807(HttpStatusCode.BadRequest)
                        .ThrowMessage(x => !x.IsSuccessStatusCode && x.StatusCode != HttpStatusCode.NotFound, m => param.MakeRfc7807Response(ErrorCodeMessage.Code.E104402))
                        .Result));
        }

        /// <summary>
        /// ProductCodeで入出荷を読込
        /// </summary>
        private void LoadShipmentsAndArrivals(IApiFilterActionParam param, string productCode)
        {
            if (!ShipmentDic.ContainsKey(productCode))
            {
                try
                {
                    var shipmentTask = GetShipmentsByProductCodeAsync(param, productCode);
                    var arrivalTask = GetArrivalsByProductCodeAsync(param, productCode);
                    Task.WaitAll(shipmentTask, arrivalTask);

                    ShipmentDic.Add(productCode, shipmentTask.Result);
                    ArrivalDic.Add(productCode, arrivalTask.Result);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to get shipments and arrivals.");
                    throw;
                }
            }
        }

        /// <summary>
        /// 入荷から出荷元をトレースする
        /// </summary>
        private void TraceBack(IApiFilterActionParam param, string arrivalId, string productCode)
        {
            // ProductCodeで入出荷を読込
            LoadShipmentsAndArrivals(param, productCode);
            var shipmentList = ShipmentDic[productCode];
            var arrivalList = ArrivalDic[productCode];

            // 入荷
            var arrival = arrivalList.FirstOrDefault(x => x.ArrivalId == arrivalId);
            if (arrival == null)
            {
                return;
            }
            if (!TraceResult.Any(x => x.ArrivalId == arrival.ArrivalId))
            {
                TraceResult.Add(ToResultModel(arrival, productCode));
            }
            else
            {
                // 循環参照のため中断
                return;
            }

            // 出荷
            ShipmentModel? shipment;
            if (arrival.ShipmentId == null)
            {
                // 直近の出荷と紐付け（紐付け漏れの補完）
                shipment = shipmentList
                    .Where(x =>
                        x.Shipping.ShippingGln == arrival.ArrivalGln &&
                        x.ShipmentProducts?.Any(y => y.ProductCode == productCode) == true &&
                        x.ShipmentDate < arrival.ArrivalDate)
                    .OrderByDescending(x => x.ShipmentDate)
                    .FirstOrDefault();
            }
            else
            {
                shipment = shipmentList.FirstOrDefault(x => x.ShipmentId == arrival.ShipmentId);
            }
            if (shipment == null)
            {
                return;
            }
            if (!TraceResult.Any(x => x.ShipmentId == shipment.ShipmentId))
            {
                TraceResult.Add(ToResultModel(shipment, productCode));
            }
            else
            {
                // 循環参照のため中断
                return;
            }

            // 入荷元トレース
            foreach (var shipmentProduct in shipment.ShipmentProducts.Where(x => x.ProductCode == productCode))
            {
                if (shipmentProduct.ArrivalProductMap?.Count > 0)
                {
                    // 入荷ごとに再帰的にトレースバック
                    foreach (var map in shipmentProduct.ArrivalProductMap)
                    {
                        TraceBack(param, map.ArrivalId, map.ArrivalProductCode);
                    }
                }
                else
                {
                    // 直近の入荷と紐付け（紐付け漏れの補完）
                    var latestArrival = arrivalList
                        .Where(x => 
                            x.ArrivalGln == shipment.Shipment.ShipmentGln && 
                            x.ArrivalProduct?.Any(y => y.ProductCode == productCode) == true &&
                            x.ArrivalDate < shipment.ShipmentDate)
                        .OrderByDescending(x => x.ArrivalDate)
                        .FirstOrDefault();
                    if (latestArrival != null)
                    {
                        TraceBack(param, latestArrival.ArrivalId, productCode);
                    }
                }
            }
        }

        /// <summary>
        /// 出荷から出荷先をトレースフォワードする
        /// </summary>
        private void TraceForward(IApiFilterActionParam param, string shipmentId, string productCode)
        {
            // ProductCodeで入出荷を読込
            LoadShipmentsAndArrivals(param, productCode);
            var shipmentList = ShipmentDic[productCode];
            var arrivalList = ArrivalDic[productCode];

            // 出荷
            var shipment = shipmentList.FirstOrDefault(x => x.ShipmentId == shipmentId);
            if (shipment == null || TraceResult.Any(x => x.ShipmentId == shipment.ShipmentId))
            {
                return;
            }
            else
            {
                TraceResult.Add(ToResultModel(shipment, productCode));
            }

            // 入荷
            var arrival = arrivalList.FirstOrDefault(x => x.ShipmentId == shipment.ShipmentId);
            if (arrival == null || TraceResult.Any(x => x.ArrivalId == arrival.ArrivalId))
            {
                return;
            }
            else
            {
                TraceResult.Add(ToResultModel(arrival, productCode));
            }

            // 出荷先トレース
            var shipments = shipmentList.Where(x => x.ShipmentProducts?.Any(y => y.ArrivalProductMap?.Any(z => z.ArrivalId == arrival.ArrivalId) == true) == true).ToList();
            if (shipments.Count != 1)
            {
                // 出荷が複数の場合、正しい経路の判別はできないため中断
                return;
            }
            else
            {
                TraceForward(param, shipments.First().ShipmentId, productCode);
            }
        }

        /// <summary>
        /// トレース結果に事業者/事業所名を設定
        /// </summary>
        /// <param name="param"></param>
        private void FillCompanyAndOfficeName(IApiFilterActionParam param)
        {
            var companyIdList = new List<string>();
            var officeIdList = new List<string>();
            companyIdList.AddRange(TraceResult.Select(x => x.Company.CompanyId).Distinct());
            officeIdList.AddRange(TraceResult.Select(x => x.Company.OfficeId).Distinct());

            // マスタ取得
            var companyTask = GetCompaniesAsync(param, companyIdList);
            var officeTask = GetOfficesAsync(param, officeIdList);
            Task.WaitAll(companyTask, officeTask);

            var companyList = companyTask.Result;
            var officeList = officeTask.Result;

            // 返却モデルを作成する
            foreach (var result in TraceResult)
            {
                var company = companyList.FirstOrDefault(x => x.CompanyId == result.Company.CompanyId);
                result.Company.CompanyName = company?.CompanyName;
                result.Company.CompanyNameLang = company?.CompanyNameLang;

                var office = officeList.FirstOrDefault(x => x.OfficeId == result.Company.OfficeId);
                result.Company.OfficeName = office?.OfficeName;
                result.Company.OfficeNameLang = office?.OfficeNameLang;
            }
        }

        /// <summary>
        /// 入荷をレスポンスの型に変換
        /// </summary>
        private GetShipmentAndArrivalResultModel ToResultModel(ArrivalModel arrival, string productCode)
        {
            var packageQuantity = arrival.ArrivalProduct?.Where(x => x.ProductCode == productCode).Sum(x => x.PackageQuantity) ?? 0;

            return new GetShipmentAndArrivalResultModel()
            {
                ProductCode = productCode,
                DateTime = arrival.ArrivalDate,
                Type = ArrivalType,
                Company = new GetShipmentAndArrivalResultCompanyModel()
                {
                    CompanyId = arrival.ArrivalCompanyId,
                    OfficeId = arrival.ArrivalOfficeId,
                    GlnCode = arrival.ArrivalGln
                },
                PackageQuantity = packageQuantity,
                ArrivalId = arrival.ArrivalId,
                PreviousShipmentId = arrival.ShipmentId
            };
        }

        /// <summary>
        /// 出荷をレスポンスの型に変換
        /// </summary>
        private GetShipmentAndArrivalResultModel ToResultModel(ShipmentModel shipment, string productCode)
        {
            var packageQuantity = shipment.ShipmentProducts?.Where(x => x.ProductCode == productCode).Sum(x => x.PackageQuantity) ?? 0;

            return new GetShipmentAndArrivalResultModel()
            {
                ProductCode = productCode,
                DateTime = shipment.ShipmentDate,
                Type = ShipmentType,
                Company = new GetShipmentAndArrivalResultCompanyModel()
                {
                    CompanyId = shipment.Shipment.ShipmentCompanyId,
                    OfficeId = shipment.Shipment.ShipmentOfficeId,
                    GlnCode = shipment.Shipment.ShipmentGln,
                },
                PackageQuantity = packageQuantity,
                ShipmentId = shipment.ShipmentId,
                PreviousShipmentId = shipment.PreviousShipmentId?.FirstOrDefault()
            };
        }
    }
}