using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Extensions;
using JP.DataHub.UnitTest.Com;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class ShipmentSensorRawDataGetRawDataSensorFilterTest : ItTestCaseBase
    {
        private const string TestDataArrivalId = "IntegratedTest";


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
        /// テストファイル：TestFile/ShipmentSensorRawDataGetRawDataSensorFilterTest_Normal.json
        /// 格納先コンテナ：integratedtest
        /// </remarks>
        [TestMethod]
        public void ShipmentSensorRawDataGetRawDataSensorFilterTest_NormalScenario()
        {
            var client = new IntegratedTestClient("test1");
            var rawDataApi = UnityCore.Resolve<IShipmentSensorRawDataApi>();
            var summaryApi = UnityCore.Resolve<ISensorTraceabilitySummaryApi>();

            // データ登録(削除は内部呼び出し限定のためID固定データ)
            var productCode = Guid.NewGuid().ToString();
            client.GetWebApiResponseResult(summaryApi.Register(new SensorTraceabilitySummaryModel
            {
                ManagementId = "7a5e3f42-8edf-43ac-9dd3-ad0d3f58a8ce",
                aggregationTime = DateTime.Parse("2020-01-02T03:04:06"),
                blobUrl = Guid.NewGuid().ToString(),
                blobContainer = "integratedtest",
                blobFileName = "ShipmentSensorRawDataGetRawDataSensorFilterTest_Normal.json",
                ProductCodes = new[] { Guid.NewGuid().ToString(), productCode, Guid.NewGuid().ToString() },
                arrivalId = TestDataArrivalId,
                time = 0
            })).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(summaryApi.Register(new SensorTraceabilitySummaryModel
            {
                ManagementId = "1b955c0a-1a0f-43f0-bd5a-e4240833ee1c",
                aggregationTime = DateTime.Parse("2020-01-02T03:04:05"),
                blobUrl = Guid.NewGuid().ToString(),
                blobContainer = Guid.NewGuid().ToString(),
                blobFileName = Guid.NewGuid().ToString(),
                ProductCodes = new[] { Guid.NewGuid().ToString(), productCode, Guid.NewGuid().ToString() },
                arrivalId = TestDataArrivalId,
                time = 0
            })).Assert(RegisterSuccessExpectStatusCode);

            // 取得
            var getResult = client.GetWebApiResponseResult(rawDataApi.GetRawDataSensor(productCode, TestDataArrivalId)).Assert(GetSuccessExpectStatusCode);
            var expected = Encoding.UTF8.GetString(Properties.Resources.ShipmentSensorRawDataGetRawDataSensorFilterTest_Normal);
            getResult.RawContentString.Is(expected);
        }

        /// <remarks>
        /// テスト用ファイルを事前に手動でストレージに配置しておくこと
        /// テストファイル：TestFile/ShipmentSensorRawDataGetRawDataSensorFilterTest_Empty.json
        /// 格納先コンテナ：integratedtest
        /// </remarks>
        [TestMethod]
        public void ShipmentSensorRawDataGetRawDataSensorFilterTest_EmptyScenario()
        {
            var client = new IntegratedTestClient("test1");
            var rawDataApi = UnityCore.Resolve<IShipmentSensorRawDataApi>();
            var summaryApi = UnityCore.Resolve<ISensorTraceabilitySummaryApi>();

            // データ登録(削除は内部呼び出し限定のためID固定データ)
            var productCode = Guid.NewGuid().ToString();
            client.GetWebApiResponseResult(summaryApi.Register(new SensorTraceabilitySummaryModel
            {
                ManagementId = "7a5e3f42-8edf-43ac-9dd3-ad0d3f58a8ce",
                aggregationTime = DateTime.Parse("2020-01-02T03:04:06"),
                blobUrl = Guid.NewGuid().ToString(),
                blobContainer = "integratedtest",
                blobFileName = "ShipmentSensorRawDataGetRawDataSensorFilterTest_Empty.json",
                ProductCodes = new[] { Guid.NewGuid().ToString(), productCode, Guid.NewGuid().ToString() },
                arrivalId = TestDataArrivalId,
                time = 0
            })).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(summaryApi.Register(new SensorTraceabilitySummaryModel
            {
                ManagementId = "1b955c0a-1a0f-43f0-bd5a-e4240833ee1c",
                aggregationTime = DateTime.Parse("2020-01-02T03:04:05"),
                blobUrl = Guid.NewGuid().ToString(),
                blobContainer = Guid.NewGuid().ToString(),
                blobFileName = Guid.NewGuid().ToString(),
                ProductCodes = new[] { Guid.NewGuid().ToString(), productCode, Guid.NewGuid().ToString() },
                arrivalId = TestDataArrivalId,
                time = 0
            })).Assert(RegisterSuccessExpectStatusCode);

            // 取得
            client.GetWebApiResponseResult(rawDataApi.GetRawDataSensor(productCode, TestDataArrivalId)).AssertErrorCode(NotFoundStatusCode, "E103407");
        }

        [TestMethod]
        public void ShipmentSensorRawDataGetRawDataSensorFilterTest_NotFoundScenario()
        {
            var client = new IntegratedTestClient("test1");
            var rawDataApi = UnityCore.Resolve<IShipmentSensorRawDataApi>();

            // 取得
            client.GetWebApiResponseResult(rawDataApi.GetRawDataSensor(Guid.NewGuid().ToString(), TestDataArrivalId)).AssertErrorCode(NotFoundStatusCode, "E103406");
        }

        [TestMethod]
        public void ShipmentSensorRawDataGetRawDataSensorFilterTest_BadRequestScenario()
        {
            var client = new IntegratedTestClient("test1");
            var rawDataApi = UnityCore.Resolve<IShipmentSensorRawDataApi>();

            // 取得(ProductCodeなし)
            client.GetWebApiResponseResult(rawDataApi.GetRawDataSensor(null, TestDataArrivalId)).AssertErrorCode(BadRequestStatusCode, "E103404");

            // 取得(ProductCodeなし)
            client.GetWebApiResponseResult(rawDataApi.GetRawDataSensor(Guid.NewGuid().ToString(), null)).AssertErrorCode(BadRequestStatusCode, "E103405");
        }
    }
}
