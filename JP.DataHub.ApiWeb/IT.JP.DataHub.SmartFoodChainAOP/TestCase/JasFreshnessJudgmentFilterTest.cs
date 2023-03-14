using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using JP.DataHub.UnitTest.Com.Extensions;
using System.Net;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class JasFreshnessJudgmentFilterTest : ItTestCaseBase
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
        
        /// <remarks>
        /// テスト用ファイルを事前に手動でストレージに配置しておくこと
        /// テストファイル：
        ///   TestFile/JasFreshnessJudgmentFilterTest_Norma_NG.json
        ///   TestFile/JasFreshnessJudgmentFilterTest_Norma_OK.json
        /// 格納先コンテナ：integratedtest
        /// </remarks>
        [TestMethod]
        public void JasFreshnessJudgmentFilter_NormalScenario()
        {
            var client = UnityCore.Resolve<IDynamicApiClient>();
            var partyProfileApi = UnityCore.Resolve<ICompanyApi>();
            var partyProductApi = UnityCore.Resolve<ICompanyProductApi>();
            var productCodeDetailApi = UnityCore.Resolve<IProductDetailApi>();
            var cropFreshnessManagementApi = UnityCore.Resolve<ICropFreshnessManagementApi>();
            var sensorTraceabilitySummaryApi = UnityCore.Resolve<ISensorTraceabilitySummaryApi>();
            var judgmentApi = UnityCore.Resolve<IJudgmentApi>();
            var judgmentHistoryApi = UnityCore.Resolve<IJudgmentHistoryApi>();
            
            var testGtinCode = "01XXXXXXXXXXX";
            var testProductCode1 = testGtinCode + "210001";
            var testProductCode2 = testGtinCode + "210002";
            var testGlnCode = "JasJudgeTest";
            var testPartyId = "00A6181C-C079-4294-B36F-16987B123175";
            var testCropCode = "TEST0010";
            var testCropFreshnessManagementId = "04A46A5C-395E-4C0F-8E4A-B2EE63B960CA";
            var testObservedPropertiesCodeTemperature = "BC6A4FE1-4211-478A-A85E-EF1F3CE34B54";
            var testObservedPropertiesCodeHumidity = "36C983A9-349A-45F5-AD3B-6C5FACCD1A7F";
            var testObservedPropertiesCodeShock = "13943EF9-80E4-4AEC-A18B-56F3CAF0AA80";
            var testMeasurementUnitsTemperature = "18E5AA34-FDE6-411F-A455-EEA588CAB34B";
            var testMeasurementUnitsHumidity = "444C0F86-FA92-462C-AB6B-B4C3FB6B5E51";
            var testMeasurementUnitsShock = "92567BDB-2194-4526-9BE0-2FC2BA9080A4";
            var testManagementId1 = "5548613A-35C0-47EA-9ADE-D9534E08783B";
            var testManagementId2 = "5121485C-6F5A-4FEC-8CBF-3D1D4337E5C8";
            
            // データ削除
            client.GetWebApiResponseResult(partyProfileApi.Delete(testPartyId));
            client.GetWebApiResponseResult(partyProductApi.Delete(testGtinCode));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode1));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode2));
            client.GetWebApiResponseResult(cropFreshnessManagementApi.Delete(testCropFreshnessManagementId));
            client.GetWebApiResponseResult(judgmentHistoryApi.ODataDelete(testGlnCode));

            // データ登録
            client.GetWebApiResponseResult(partyProfileApi.Register(new CompanyModel
                {
                    CompanyId = testPartyId,
                    CompanyName = "テスト用事業者1",
                    CountryCode = "JP",
                    GlnCode = testGlnCode,
                    GS1CompanyCode = "hoge",
                })).Assert(RegisterSuccessExpectStatusCode);
            
            client.GetWebApiResponseResult(partyProductApi.Register(new CompanyProductModel
            {
                GtinCode = testGtinCode,
                CodeType = "GS1-128",
                CompanyId = testPartyId,
                IsOrganic = false,
                ProductName = "テスト用",
                RegistrationDate = "2022-09-01",
                Profile = new ProductProfileModel() { CropCode = testCropCode}
            })).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(productCodeDetailApi.RegisterList(new List<ProductDetailModel>
            {
                new()
                {
                    ProductCode = testProductCode1,
                    GtinCode = testGtinCode,
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel { FoodRegistrationNo = "1" }
                },
                new()
                {
                    ProductCode = testProductCode2,
                    GtinCode = testGtinCode,
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel { FoodRegistrationNo = "1" }
                }
            })).Assert(RegisterSuccessExpectStatusCode);
            
            // JAS判定
            var measurementDetailTemperature = new MeasurementDetailModel
            {
                // 温度20度以上が61分以上
                // 温度10度以下が61分以下
                ObservedPropertiesCode =  new[] { testObservedPropertiesCodeTemperature },
                FailThreshold = new List<FailThresholdModel>()
                {
                    new()
                    {
                        ThresholdValue = 20,
                        Operator = "GreaterEqual",
                        ThresholdGreatTotalMinute = 61,
                        ThresholdLessTotalMinute = null,
                        ThresholdGreatTimes= null,
                        ThresholdLessTimes = null,
                    },
                    new()
                    {
                        ThresholdValue = 10,
                        Operator = "LessEqual",
                        ThresholdGreatTotalMinute = null,
                        ThresholdLessTotalMinute = 61,
                        ThresholdGreatTimes= null,
                        ThresholdLessTimes = null,
                    }
                }
            };
            var measurementDetailHumidity = new MeasurementDetailModel
            {
                // 湿度70%未満が1回以下
                ObservedPropertiesCode =  new[] { testObservedPropertiesCodeHumidity },
                FailThreshold = new List<FailThresholdModel>()
                {
                    new()
                    {
                        ThresholdValue = 70,
                        Operator = "Less",
                        ThresholdGreatTotalMinute = null,
                        ThresholdLessTotalMinute = null,
                        ThresholdGreatTimes= null,
                        ThresholdLessTimes = 1,
                    }
                }
            };
            var measurementDetailShock = new MeasurementDetailModel
            {
                // 衝撃10Gより大きいのが2回以上
                ObservedPropertiesCode = new[] { testObservedPropertiesCodeShock },
                FailThreshold = new List<FailThresholdModel>()
                {
                    new()
                    {
                        ThresholdValue = 10000,
                        Operator = "Greater",
                        ThresholdGreatTotalMinute = null,
                        ThresholdLessTotalMinute = null,
                        ThresholdGreatTimes = 2,
                        ThresholdLessTimes = null,
                    }
                }
            };
            
            client.GetWebApiResponseResult(cropFreshnessManagementApi.Register(new CropFreshnessManagementModel()
            {
                CropFreshnessManagementId = testCropFreshnessManagementId,
                CropCode = testCropCode,
                MeasurementDetail = new List<MeasurementDetailModel>
                {
                    measurementDetailTemperature,
                    measurementDetailHumidity,
                    measurementDetailShock
                },
                JudgmentSyntax = "",
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2050, 1, 1),
                Explanation = "テスト用",
            })).Assert(RegisterSuccessExpectStatusCode);
            
            // 本当の流れはセンサーデータと入出荷を登録して自動でセンサー集計するけど、
            // テストなので直接集計データを登録する
            // 判定OK
            client.GetWebApiResponseResult(sensorTraceabilitySummaryApi.Register(new SensorTraceabilitySummaryModel
            {
                ManagementId = testManagementId1,
                aggregationTime = DateTime.Parse("2022-10-01T03:00:00"),
                blobUrl = "A78C17F0-5E02-46EC-874C-752C3C04EB44",
                blobContainer = "integratedtest",
                blobFileName = "JasFreshnessJudgmentFilterTest_Normal_OK.json",
                ProductCodes = new[]{testProductCode1},
                arrivalId = testGlnCode,
                time = 0,
            })).Assert(RegisterSuccessExpectStatusCode);

            //判定NG
            client.GetWebApiResponseResult(sensorTraceabilitySummaryApi.Register(new SensorTraceabilitySummaryModel
            {
                ManagementId = testManagementId2,
                aggregationTime = DateTime.Parse("2022-10-01T03:00:00"),
                blobUrl = "A78C17F0-5E02-46EC-874C-752C3C04EB44",
                blobContainer = "integratedtest",
                blobFileName = "JasFreshnessJudgmentFilterTest_Normal_NG.json",
                ProductCodes = new[]{testProductCode2},
                arrivalId = testGlnCode,
                time = 0,
            })).Assert(RegisterSuccessExpectStatusCode);
            
            // JAS判定OKデータを取得する
            var result = client.GetWebApiResponseResult(judgmentApi.JasFreshnessJudgment(testProductCode1, testGlnCode)).Assert(HttpStatusCode.OK);
            result.Result.result.IsTrue();
            
            // JAS判定OKデータを取得する（2回目は履歴から取るので同じAPIを2回実行する）
            result = client.GetWebApiResponseResult(judgmentApi.JasFreshnessJudgment(testProductCode1, testGlnCode)).Assert(HttpStatusCode.OK);
            result.Result.result.IsTrue();

            // JAS判定NGデータを取得する
            var resultExpect = new JasFreshnessJudgmentResultModel
            {
                result = false,
                fails = new List<JasFreshnessJudgmentFailModel>
                {
                    new()
                    {
                        ObservedPropertiesCode = measurementDetailTemperature.ObservedPropertiesCode,
                        MeasurementUnitId = testMeasurementUnitsTemperature,
                        DatastreamId = "hoge1",
                        Count = 120,
                        Threshold = new List<FailThresholdModel> { measurementDetailTemperature.FailThreshold[0]}
                    },
                    new()
                    {
                        ObservedPropertiesCode = measurementDetailTemperature.ObservedPropertiesCode,
                        MeasurementUnitId = testMeasurementUnitsTemperature,
                        DatastreamId = "hoge1",
                        Count = 60,
                        Threshold = new List<FailThresholdModel> { measurementDetailTemperature.FailThreshold[1]}
                    },
                    new()
                    {
                        ObservedPropertiesCode = measurementDetailHumidity.ObservedPropertiesCode,
                        MeasurementUnitId = testMeasurementUnitsHumidity,
                        DatastreamId = "hoge2",
                        Count = 1,
                        Threshold = measurementDetailHumidity.FailThreshold
                    },
                    new()
                    {
                        ObservedPropertiesCode = measurementDetailShock.ObservedPropertiesCode,
                        MeasurementUnitId = "{{*}}",
                        DatastreamId = "hoge3",
                        Count = 2,
                        Threshold = measurementDetailShock.FailThreshold
                    }
                } 
            };
            client.GetWebApiResponseResult(judgmentApi.JasFreshnessJudgment(testProductCode2, testGlnCode)).Assert(HttpStatusCode.OK, resultExpect);
         
            // JAS判定NGデータを取得する（2回目は履歴から取るので同じAPIを2回実行する）
            client.GetWebApiResponseResult(judgmentApi.JasFreshnessJudgment(testProductCode2, testGlnCode)).Assert(HttpStatusCode.OK, resultExpect);

            // データ削除
            client.GetWebApiResponseResult(partyProfileApi.Delete(testPartyId));
            client.GetWebApiResponseResult(partyProductApi.Delete(testGtinCode));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode1));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode2));
            client.GetWebApiResponseResult(cropFreshnessManagementApi.Delete(testCropFreshnessManagementId));
            client.GetWebApiResponseResult(judgmentHistoryApi.ODataDelete(testGlnCode));
        }

        
        [TestMethod]
        public void JasFreshnessJudgmentFilter_ErrorScenario()
        {
            var client = UnityCore.Resolve<IDynamicApiClient>();
            var partyProductApi = UnityCore.Resolve<ICompanyProductApi>();
            var productCodeDetailApi = UnityCore.Resolve<IProductDetailApi>();
            var cropFreshnessManagementApi = UnityCore.Resolve<ICropFreshnessManagementApi>();
            var judgmentApi = UnityCore.Resolve<IJudgmentApi>();
            
            var testGtinCode1 = "02XXXXXXXXXXX";
            var testGtinCode2 = "03XXXXXXXXXXX";
            var testGtinCode3 = "04XXXXXXXXXXX";
            var testProductCode1 = testGtinCode1 + "210003";
            var testProductCode2 = testGtinCode1 + "210004";
            var testProductCode3 = testGtinCode2 + "210005";
            var testProductCode4 = testGtinCode3 + "210006";
            var testGlnCode = "JasJudgeTest";
            var testPartyId = "00A6181C-C079-4294-B36F-16987B123175";
            var testCropCode1 = "TEST0020";
            var testCropCode2 = "TEST0021";
            var testCropFreshnessManagementId = "8CBCD1BC-EFBD-41CF-91A2-94EE1221BCAE";
            var testObservedPropertiesCodeTemperature = "BC6A4FE1-4211-478A-A85E-EF1F3CE34B54";
            
            // データ削除
            client.GetWebApiResponseResult(partyProductApi.Delete(testGtinCode2));
            client.GetWebApiResponseResult(partyProductApi.Delete(testGtinCode3));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode1));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode2));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode3));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode4));
            client.GetWebApiResponseResult(cropFreshnessManagementApi.Delete(testCropFreshnessManagementId));
            
            // 引数エラー
            client.GetWebApiResponseResult(judgmentApi.JasFreshnessJudgment(null, testGlnCode)).AssertErrorCode(HttpStatusCode.BadRequest, "E102403");
            client.GetWebApiResponseResult(judgmentApi.JasFreshnessJudgment(testProductCode1, null)).AssertErrorCode(HttpStatusCode.BadRequest, "E102407");

            // 商品コード未登録エラー
            client.GetWebApiResponseResult(judgmentApi.JasFreshnessJudgment(testProductCode1, testGlnCode)).AssertErrorCode(HttpStatusCode.NotFound, "E102411");

            // GTIN未登録エラー
            client.GetWebApiResponseResult(partyProductApi.RegisterList(new List<CompanyProductModel>()
            {
                new()
                {
                    GtinCode = testGtinCode1,
                    CodeType = "GS1-128",
                    CompanyId = testPartyId,
                    IsOrganic = false,
                    ProductName = "テスト用",
                    RegistrationDate = "2022-09-01",
                    Profile = new ProductProfileModel() { CropCode = testCropCode1}
                },
                new()
                {
                    GtinCode = testGtinCode2,
                    CodeType = "GS1-128",
                    CompanyId = testPartyId,
                    IsOrganic = false,
                    ProductName = "テスト用",
                    RegistrationDate = "2022-09-01",
                    Profile = new ProductProfileModel() { CropCode = testCropCode1}
                },
                new()
                {
                    GtinCode = testGtinCode3,
                    CodeType = "GS1-128",
                    CompanyId = testPartyId,
                    IsOrganic = false,
                    ProductName = "テスト用",
                    RegistrationDate = "2022-09-01",
                    Profile = new ProductProfileModel() { CropCode = testCropCode2}
                }
            })).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(productCodeDetailApi.RegisterList(new List<ProductDetailModel>
            {
                new()
                {
                    ProductCode = testProductCode2,
                    GtinCode = testGtinCode1,
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel { FoodRegistrationNo = "1" }
                },
                new()
                {
                    ProductCode = testProductCode3,
                    GtinCode = testGtinCode2,
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel { FoodRegistrationNo = "1" }
                },
                new()
                {
                    ProductCode = testProductCode4,
                    GtinCode = testGtinCode3,
                    Quantity = 100,
                    FDA = new ProductDetailFDAModel { FoodRegistrationNo = "1" }
                }
            })).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(partyProductApi.Delete(testGtinCode1));
            client.GetWebApiResponseResult(judgmentApi.JasFreshnessJudgment(testProductCode2, testGlnCode)).AssertErrorCode(HttpStatusCode.NotFound, "E102411");

            // JAS判定未登録エラー
            client.GetWebApiResponseResult(judgmentApi.JasFreshnessJudgment(testProductCode3, testGlnCode)).AssertErrorCode(HttpStatusCode.NotFound, "E102412");

            // センサーデータ未登録エラー
            client.GetWebApiResponseResult(cropFreshnessManagementApi.Register(new CropFreshnessManagementModel()
            {
                CropFreshnessManagementId = testCropFreshnessManagementId,
                CropCode = testCropCode2,
                MeasurementDetail = new List<MeasurementDetailModel>
                {
                    new()
                    {
                        // 温度20度以上が61分以上
                        // 温度10度以下が61分以下
                        ObservedPropertiesCode = new[] { testObservedPropertiesCodeTemperature },
                        FailThreshold = new List<FailThresholdModel>()
                        {
                            new()
                            {
                                ThresholdValue = 20,
                                Operator = "GreaterEqual",
                                ThresholdGreatTotalMinute = 61,
                                ThresholdLessTotalMinute = null,
                                ThresholdGreatTimes = null,
                                ThresholdLessTimes = null,
                            }
                        }
                    }
                },
                JudgmentSyntax = "",
                StartDate = new DateTime(2022, 1, 1),
                EndDate = new DateTime(2050, 1, 1),
                Explanation = "テスト用",
            })).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(judgmentApi.JasFreshnessJudgment(testProductCode4, testGlnCode)).AssertErrorCode(HttpStatusCode.NotFound, "E102413");

            // データ削除
            client.GetWebApiResponseResult(partyProductApi.Delete(testGtinCode2));
            client.GetWebApiResponseResult(partyProductApi.Delete(testGtinCode3));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode1));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode2));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode3));
            client.GetWebApiResponseResult(productCodeDetailApi.Delete(testProductCode4));
            client.GetWebApiResponseResult(cropFreshnessManagementApi.Delete(testCropFreshnessManagementId));
        }
    }
}
