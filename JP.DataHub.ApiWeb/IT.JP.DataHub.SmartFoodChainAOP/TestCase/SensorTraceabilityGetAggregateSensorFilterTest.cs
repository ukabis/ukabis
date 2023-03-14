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
    public class SensorTraceabilityGetAggregateSensorFilterTest : ItTestCaseBase
    {
        private const string TestDataArrivalId = "IntegratedTest";
        private const int TestDataTime = 9999;


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
        /// テストファイル：TestFile/SensorTraceabilityGetAggregateSensorFilterTest_Normal.json
        /// 格納先コンテナ：integratedtest
        /// </remarks>
        [TestMethod]
        public void SensorTraceabilityGetAggregateSensorFilterTest_NormalScenario()
        {
            var client = new IntegratedTestClient("test1");
            var traceabilityApi = UnityCore.Resolve<ISensorTraceabilityApi>();
            var summaryApi = UnityCore.Resolve<ISensorTraceabilitySummaryApi>();

            // データ登録(削除は内部呼び出し限定のためID固定データ)
            var productCode = Guid.NewGuid().ToString();
            var registerResult = client.GetWebApiResponseResult(summaryApi.Register(new SensorTraceabilitySummaryModel
            {
                ManagementId = "ac827d1c-d819-40b2-abe8-8372f3bfebfa",
                aggregationTime = DateTime.Parse("2020-01-02T03:04:06"),
                blobUrl = Guid.NewGuid().ToString(),
                blobContainer = "integratedtest",
                blobFileName = "SensorTraceabilityGetAggregateSensorFilterTest_Normal.json",
                ProductCodes = new[] { Guid.NewGuid().ToString(), productCode, Guid.NewGuid().ToString() },
                arrivalId = TestDataArrivalId,
                time = TestDataTime
            })).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(summaryApi.Register(new SensorTraceabilitySummaryModel
            {
                ManagementId = "0eee47a1-3475-4cec-a579-16d212492662",
                aggregationTime = DateTime.Parse("2020-01-02T03:04:05"),
                blobUrl = Guid.NewGuid().ToString(),
                blobContainer = Guid.NewGuid().ToString(),
                blobFileName = Guid.NewGuid().ToString(),
                ProductCodes = new [] { Guid.NewGuid().ToString(), productCode, Guid.NewGuid().ToString() },
                arrivalId = TestDataArrivalId,
                time = TestDataTime
            })).Assert(RegisterSuccessExpectStatusCode);

            // 取得
            var getResult = client.GetWebApiResponseResult(traceabilityApi.GetAggregateSensor(productCode, TestDataArrivalId, TestDataTime)).Assert(GetSuccessExpectStatusCode);
            var expected = Encoding.UTF8.GetString(Properties.Resources.SensorTraceabilityGetAggregateSensorFilterTest_Normal);
            getResult.RawContentString.Is(expected);
        }

        /// <remarks>
        /// テスト用ファイルを事前に手動でストレージに配置しておくこと
        /// テストファイル：TestFile/SensorTraceabilityGetAggregateSensorFilterTest_Empty.json
        /// 格納先コンテナ：integratedtest
        /// </remarks>
        [TestMethod]
        public void SensorTraceabilityGetAggregateSensorFilterTest_EmptyScenario()
        {
            var client = new IntegratedTestClient("test1");
            var traceabilityApi = UnityCore.Resolve<ISensorTraceabilityApi>();
            var summaryApi = UnityCore.Resolve<ISensorTraceabilitySummaryApi>();

            // データ登録(削除は内部呼び出し限定のためID固定データ)
            var productCode = Guid.NewGuid().ToString();
            client.GetWebApiResponseResult(summaryApi.Register(new SensorTraceabilitySummaryModel
            {
                ManagementId = "ac827d1c-d819-40b2-abe8-8372f3bfebfa",
                aggregationTime = DateTime.Parse("2020-01-02T03:04:06"),
                blobUrl = Guid.NewGuid().ToString(),
                blobContainer = "integratedtest",
                blobFileName = "SensorTraceabilityGetAggregateSensorFilterTest_Empty.json",
                ProductCodes = new[] { Guid.NewGuid().ToString(), productCode, Guid.NewGuid().ToString() },
                arrivalId = TestDataArrivalId,
                time = TestDataTime
            })).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(summaryApi.Register(new SensorTraceabilitySummaryModel
            {
                ManagementId = "0eee47a1-3475-4cec-a579-16d212492662",
                aggregationTime = DateTime.Parse("2020-01-02T03:04:05"),
                blobUrl = Guid.NewGuid().ToString(),
                blobContainer = Guid.NewGuid().ToString(),
                blobFileName = Guid.NewGuid().ToString(),
                ProductCodes = new[] { Guid.NewGuid().ToString(), productCode, Guid.NewGuid().ToString() },
                arrivalId = TestDataArrivalId,
                time = TestDataTime
            })).Assert(RegisterSuccessExpectStatusCode);

            // 取得
            client.GetWebApiResponseResult(traceabilityApi.GetAggregateSensor(productCode, TestDataArrivalId, TestDataTime)).AssertErrorCode(NotFoundStatusCode, "E103407");
        }

        [TestMethod]
        public void SensorTraceabilityGetAggregateSensorFilterTest_NotFoundScenario()
        {
            var client = new IntegratedTestClient("test1");
            var traceabilityApi = UnityCore.Resolve<ISensorTraceabilityApi>();

            // 取得
            client.GetWebApiResponseResult(traceabilityApi.GetAggregateSensor(Guid.NewGuid().ToString(), TestDataArrivalId, TestDataTime)).AssertErrorCode(NotFoundStatusCode, "E103406");
        }

        [TestMethod]
        public void SensorTraceabilityGetAggregateSensorFilterTest_BadRequestScenario()
        {
            var client = new IntegratedTestClient("test1");
            var traceabilityApi = UnityCore.Resolve<ISensorTraceabilityApi>();

            // 取得(ProductCodeなし)
            client.GetWebApiResponseResult(traceabilityApi.GetAggregateSensor(null, TestDataArrivalId, TestDataTime)).AssertErrorCode(BadRequestStatusCode, "E103404");

            // 取得(ProductCodeなし)
            client.GetWebApiResponseResult(traceabilityApi.GetAggregateSensor(Guid.NewGuid().ToString(), null, TestDataTime)).AssertErrorCode(BadRequestStatusCode, "E103405");

            // 取得(AggregationTimeなし)
            client.GetWebApiResponseResult(traceabilityApi.GetAggregateSensor(Guid.NewGuid().ToString(), TestDataArrivalId, null)).AssertErrorCode(BadRequestStatusCode, "E103408");

            // 取得(AggregationTime不正)
            client.GetWebApiResponseResult(traceabilityApi.GetAggregateSensor(Guid.NewGuid().ToString(), TestDataArrivalId, 0)).AssertErrorCode(BadRequestStatusCode, "E103408");
        }
    }
}
