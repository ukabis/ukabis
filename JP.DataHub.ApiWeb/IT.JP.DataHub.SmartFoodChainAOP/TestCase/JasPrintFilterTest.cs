using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Authentication;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using JP.DataHub.Com.Extensions;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class JasPrintFilterTest : ItTestCaseBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestCleanup]
        public new void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        public void JasPrintFilterTest_NormalScenario()
        {
            var client = UnityCore.Resolve<IDynamicApiClient>();
            var jasPrint = UnityCore.Resolve<IJasPrintApi>();
            var jasPrintLog = UnityCore.Resolve<IJasPrintLogApi>();
            var judgmentHistory = UnityCore.Resolve<IJudgmentHistoryApi>();
            var partyProduct = UnityCore.Resolve<ICompanyProductApi>();
            var productCodeDetail = UnityCore.Resolve<IProductDetailApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();

            // データ削除
            var deleteResult = client.Request(arrival.ODataDelete("FilterIT"))
                .ToWebApiResponseResult<List<string>>();
            Assert.IsTrue(deleteResult.StatusCode == HttpStatusCode.NotFound || deleteResult.StatusCode == HttpStatusCode.NoContent);
            //var deleteResult = client.Request(judgmentHistory.ODataDelete("jasPrintGln"))
            //    .ToWebApiResponseResult<List<string>>();
            //Assert.IsTrue(deleteResult.StatusCode == HttpStatusCode.NotFound || deleteResult.StatusCode == HttpStatusCode.NoContent);

            // OpenIdトークンの取得
            string openid = null;
            if (client.AuthenticationResult is CombinationAuthenticationResult comb)
            {
                foreach (var x in comb)
                {
                    var jwt = x.GetPropertyValue<Jwt>("OpenIdJwt");
                    if (jwt != null)
                    {
                        openid = jwt.oid;
                    }
                }
            }

            // データ登録
            var registerResult = client.Request(partyProduct.Register(new CompanyProductModel
            {
                GtinCode = "01jasPrint",
                CodeType = "GS1-128",
                CompanyId = "JasPrintFilterTest",
                IsOrganic = false,
                ProductName = "JasPrintFilterTest",
                RegistrationDate = "2021-11-12",
                Profile = new ProductProfileModel() { CropCode = "NSM0010000100000" }
            }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.Created);
            registerResult = client.Request(productCodeDetail.RegisterList(new List<ProductDetailModel>
                {
                    new ProductDetailModel
                    {
                        GtinCode = "01jasPrint",
                        ProductCode = "01jasPrint21ok",
                        Quantity = 100,
                        FDA = new ProductDetailFDAModel{FoodRegistrationNo = "1"}
                    },
                    new ProductDetailModel
                    {
                        GtinCode = "01jasPrint",
                        ProductCode = "01jasPrint21ng",
                        Quantity = 100,
                        FDA = new ProductDetailFDAModel{FoodRegistrationNo = "1"}
                    },
                    new ProductDetailModel
                    {
                        GtinCode = "01jasPrint",
                        ProductCode = "01jasPrint10ng",
                        Quantity = 100,
                        FDA = new ProductDetailFDAModel{FoodRegistrationNo = "1"}
                    }
                }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.Created);

            registerResult = client.Request(arrival.Register(new ArrivalModel()
            {
                ArrivalId = null,
                ShipmentTypeCode = "PUP",
                ArrivalDate = new DateTime(2021, 12, 2),
                InvoiceCode = "FilterIT",
                ShipmentId = null,
                ShipmentCompanyId = "FilterITPartyProfile1",
                ShipmentOfficeId = "FilterITPartyOffice1",
                ShipmentGln = "FilterITPartyOfficeGln1",
                ArrivalCompanyId = "FilterITPartyProfile2",
                ArrivalOfficeId = "FilterITPartyOffice2",
                ArrivalGln = "jasPrintGln",
                ArrivalProduct = new List<ArrivalProductModel>()
                    {
                        new ArrivalProductModel()
                        {
                            ProductCode = "01jasPrint21ok",
                            Quantity = 1,
                            DamageQuantity = 0,
                            InvoiceCode = "hoge",
                            CropCode = "01010010",
                            PackageQuantity = 11,
                            SinglePackageWeight = 1,
                            CapacityUnitCode = "KG",
                            ReceivePackageQuantity = 1000,
                            DamagePackageQuantity = 1,
                        }
                    },
            }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.Created);

            // このデータを作成するには、以下の手順が必要です。
            // 1. JASモデルの「/API/Traceability/JasFreshnessManagement/V3/Public/JudgmentHistory」リソースのis_internal_call_only=trueをfalseに変更してDEPLOY
            // 2. Postmanなどで以下のjsonを登録する(配列になっているが3件)。リソースは/API/Traceability/JasFreshnessManagement/V3/Public/JudgmentHistory
            //[ 
            //  { "ProductCode": "01jasPrint21ok", "GlnCode": "jasPrintGln", "ArrivalId": "arrivalId1", "Result": { "result": true } },
            //  { "ProductCode": "01jasPrint21ng", "GlnCode": "jasPrintGln", "ArrivalId": "arrivalId2", "Result": { "result": false } },
            //  { "ProductCode": "01jasPrint10ng", "GlnCode": "jasPrintGln", "ArrivalId": "arrivalId3", "Result": { "result": true } }
            //]
            // 3. Postmanなどで以下のjsonを登録する(1件)。リソースは/API/Traceability/V3/Private/Arrival
            //{
            //  "ShipmentTypeCode": "PUP",
            //  "ArrivalDate": "2021-12-02T00:00:00",
            //  "InvoiceCode": "FilterIT",
            //  "ShipmentId": null,
            //  "ShipmentCompanyId": "FilterITPartyProfile1",
            //  "ShipmentOfficeId": "FilterITPartyOffice1",
            //  "ShipmentGln": "FilterITPartyOfficeGln1",
            //  "ArrivalCompanyId": "FilterITPartyProfile2",
            //  "ArrivalOfficeId": "FilterITPartyOffice2",
            //  "ArrivalGln": "jasPrintGln",
            //  "ArrivalProduct": [
            //    {
            //      "ProductCode": "01jasPrint21ok",
            //      "InvoiceCode": "hoge",
            //      "CropCode": "01010010",
            //      "BreedCode": null,
            //      "BrandCode": null,
            //      "GradeCode": null,
            //      "SizeCode": null,
            //      "Quantity": 0,
            //      "DamageQuantity": 0,
            //      "PackageQuantity": 11,
            //      "SinglePackageWeight": 1,
            //      "CapacityUnitCode": "KG",
            //      "ReceivePackageQuantity": 1000,
            //      "DamagePackageQuantity": 1,
            //      "AttachFileId": null
            //    }
            //  ]
            //}
            // 4. 1でis_internal_call_only=falseをtrueに戻して再度DEPLOY
            //registerResult = client.Request(judgmentHistory.RegisterList(new List<JudgmentHistoryModel>
            //    {
            //        new JudgmentHistoryModel
            //        {
            //            ProductCode = "01jasPrint21ok",
            //            GlnCode="jasPrintGln",
            //            ArrivalId = "arrivalId1",
            //            Result = new JudgmentHistoryResultModel{result = true}
            //        },
            //        new JudgmentHistoryModel
            //        {
            //            ProductCode = "01jasPrint21ng",
            //            GlnCode="jasPrintGln",
            //            ArrivalId = "arrivalId2",
            //            Result = new JudgmentHistoryResultModel{result = false}
            //        },
            //        new JudgmentHistoryModel
            //        {
            //            ProductCode = "01jasPrint10ng",
            //            GlnCode="jasPrintGln",
            //            ArrivalId = "arrivalId3",
            //            Result = new JudgmentHistoryResultModel{result = true}
            //        }
            //    }))
            //    .ToWebApiResponseResult<List<string>>();
            //registerResult.StatusCode.Is(HttpStatusCode.Created);

            // 印刷可能枚数取得
            var printableResult = client.Request(jasPrint.GetPrintableCount("01jasPrint21ok", "jasPrintGln", "arrivalId1"))
                .ToWebApiResponseResult<GetPrintableCountResultModel>();
            printableResult.IsSuccessStatusCode.IsTrue();
            printableResult.Result.Count.Is(100000);
            var printableCount = printableResult.Result.PrintableCount;
            var printedCount = printableResult.Result.PrintedCount;
            // JAS印刷データ成功パターン
            registerResult = client.Request(jasPrint.RegisterPrintLog(new JasPrintRequestModel()
            {
                ProductCode = "01jasPrint21ok",
                PrintCount = 1,
                CompanyId = "jasPartyId",
                PrinterId = "PrinterId",
                LastGln = "jasPrintGln",
                PrintUser = "PrintUser",
                ArrivalId = "arrivalId1",
                OpenId = openid,
            }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.Created);
            // 印刷可能枚数取得
            printableResult = client.Request(jasPrint.GetPrintableCount("01jasPrint21ok", "jasPrintGln", "arrivalId1"))
                .ToWebApiResponseResult<GetPrintableCountResultModel>();
            printableResult.IsSuccessStatusCode.IsTrue();
            printableResult.Result.Count.Is(100000);
            printableResult.Result.PrintableCount.Is(printableCount - 1);
            printableResult.Result.PrintedCount.Is(printedCount + 1);
            // JAS再印刷データ成功パターン
            registerResult = client.Request(jasPrint.RegisterRePrintLog(new JasRePrintRequestModel()
            {
                ProductCode = "01jasPrint21ok",
                ReprintCount = 1,
                CompanyId = "jasPartyId",
                PrinterId = "PrinterId",
                LastGln = "jasPrintGln",
                PrintUser = "PrintUser",
                ArrivalId = "arrivalId1",
                OpenId = openid,
            }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.Created);
            // 印刷可能枚数取得
            printableResult = client.Request(jasPrint.GetPrintableCount("01jasPrint21ok", "jasPrintGln", "arrivalId1"))
                .ToWebApiResponseResult<GetPrintableCountResultModel>();
            printableResult.IsSuccessStatusCode.IsTrue();
            printableResult.Result.Count.Is(100000);
            printableResult.Result.PrintableCount.Is(printableCount - 1);
            printableResult.Result.PrintedCount.Is(printedCount + 2);
            // 印刷、再印刷ログが複数件ある場合に、正しく印刷可能枚数が取得できる
            registerResult = client.Request(jasPrint.RegisterPrintLog(new JasPrintRequestModel()
            {
                ProductCode = "01jasPrint21ok",
                PrintCount = 1,
                CompanyId = "jasPartyId",
                PrinterId = "PrinterId",
                LastGln = "jasPrintGln",
                PrintUser = "PrintUser",
                ArrivalId = "arrivalId1",
                OpenId = openid,
            }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.Created);
            registerResult = client.Request(jasPrint.RegisterRePrintLog(new JasRePrintRequestModel()
            {
                ProductCode = "01jasPrint21ok",
                ReprintCount = 1,
                CompanyId = "jasPartyId",
                PrinterId = "PrinterId",
                LastGln = "jasPrintGln",
                PrintUser = "PrintUser",
                ArrivalId = "arrivalId1",
                OpenId = openid,
            }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.Created);
            printableResult = client.Request(jasPrint.GetPrintableCount("01jasPrint21ok", "jasPrintGln", "arrivalId1"))
                .ToWebApiResponseResult<GetPrintableCountResultModel>();
            printableResult.IsSuccessStatusCode.IsTrue();
            printableResult.Result.Count.Is(100000);
            printableResult.Result.PrintableCount.Is(printableCount - 2);
            printableResult.Result.PrintedCount.Is(printedCount + 4);
            //ログ確認       
            var printLogResult = client.Request(jasPrintLog.OData("01jasPrint21ok", "jasPrintGln"))
                .ToWebApiResponseResult<List<JasPrintModel>>();
            printLogResult.IsSuccessStatusCode.IsTrue();
            printLogResult.Result.All(x => x.ProductCode == "01jasPrint21ok").IsTrue();
            printLogResult.Result.All(x => x.LastGln == "jasPrintGln").IsTrue();
        }

        [TestMethod]
        public void JasPrintFilterTest_BadScenario()
        {
            var client = UnityCore.Resolve<IDynamicApiClient>();
            var jasPrint = UnityCore.Resolve<IJasPrintApi>();
            var jasPrintLog = UnityCore.Resolve<IJasPrintLogApi>();
            var judgmentHistory = UnityCore.Resolve<IJudgmentHistoryApi>();
            var partyProduct = UnityCore.Resolve<ICompanyProductApi>();
            var productCodeDetail = UnityCore.Resolve<IProductDetailApi>();
            var arrival = UnityCore.Resolve<IArrivalApi>();

            // OpenIdトークンの取得
            string openid = null;
            if (client.AuthenticationResult is CombinationAuthenticationResult comb)
            {
                foreach (var x in comb)
                {
                    var jwt = x.GetPropertyValue<Jwt>("OpenIdJwt");
                    if (jwt != null)
                    {
                        openid = jwt.oid;
                    }
                }
            }

            // 失敗パターン
            // JAS判定がNG
            var registerResult = client.Request(jasPrint.RegisterPrintLog(new JasPrintRequestModel()
                {
                    ProductCode = "01jasPrint21ng",
                    PrintCount = 1,
                    CompanyId = "jasPartyId",
                    PrinterId = "PrinterId",
                    LastGln = "jasPrintGln",
                    PrintUser = "PrintUser",
                    ArrivalId = "arrivalId2",
                    OpenId = openid,
                }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.BadRequest);
            // JAS判定なし
            registerResult   = client.Request(jasPrint.RegisterPrintLog(new JasPrintRequestModel()
                {
                    ProductCode = "01jasPrint21None",
                    PrintCount = 1,
                    CompanyId = "jasPartyId",
                    PrinterId = "PrinterId",
                    LastGln = "jasPrintGln",
                    PrintUser = "PrintUser",
                    ArrivalId = "arrivalId99",
                    OpenId = openid,
                }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.BadRequest);
            // 商品がロット
            registerResult   = client.Request(jasPrint.RegisterPrintLog(new JasPrintRequestModel()
                {
                    ProductCode = "01jasPrint10ng",
                    PrintCount = 1,
                    CompanyId = "jasPartyId",
                    PrinterId = "PrinterId",
                    LastGln = "jasPrintGln",
                    PrintUser = "PrintUser",
                    ArrivalId = "arrivalId3",
                    OpenId = openid,
                }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.BadRequest);
            // RequestBodyがよくない
            registerResult   = client.Request(jasPrint.RegisterPrintLog(new JasPrintRequestModel()
                {
                    ProductCode = "01jasPrint21ok",
                    LastGln = "jasPrintGln",
                }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.BadRequest);
            // SQLインジェクション
            registerResult   = client.Request(jasPrint.RegisterPrintLog(new JasPrintRequestModel()
                {
                    ProductCode = "' or 1=1'",
                    PrintCount = 1,
                    CompanyId = "jasPartyId",
                    PrinterId = "PrinterId",
                    LastGln = "' or 1=1'",
                    PrintUser = "PrintUser",
                    ArrivalId = "arrivalId99",
                    OpenId = openid,
                }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.BadRequest);
            //印刷する枚数が印刷可能枚数を超える
            registerResult   = client.Request(jasPrint.RegisterPrintLog(new JasPrintRequestModel()
                {
                    ProductCode = "01jasPrint21ok",
                    PrintCount = 100001,
                    CompanyId = "jasPartyId",
                    PrinterId = "PrinterId",
                    LastGln = "jasPrintGln",
                    PrintUser = "PrintUser",
                    ArrivalId = "arrivalId1",
                    OpenId = openid,
            }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.BadRequest);
            //再印刷する枚数が印刷可能枚数を超える
            registerResult   = client.Request(jasPrint.RegisterRePrintLog(new JasRePrintRequestModel()
                {
                    ProductCode = "01jasPrint21ok",
                    ReprintCount = 100001,
                    CompanyId = "jasPartyId",
                    PrinterId = "PrinterId",
                    LastGln = "jasPrintGln",
                    PrintUser = "PrintUser",
                    ArrivalId = "arrivalId1",
                    OpenId = openid,
                }))
                .ToWebApiResponseResult<List<string>>();
            registerResult.StatusCode.Is(HttpStatusCode.BadRequest);

            //// データ削除
            //var deleteResult = client.Request(arrival.ODataDelete("FilterIT"))
            //    .ToWebApiResponseResult<List<string>>();
            //Assert.IsTrue(deleteResult.StatusCode == HttpStatusCode.NotFound || deleteResult.StatusCode == HttpStatusCode.NoContent);
        }
    }
}
