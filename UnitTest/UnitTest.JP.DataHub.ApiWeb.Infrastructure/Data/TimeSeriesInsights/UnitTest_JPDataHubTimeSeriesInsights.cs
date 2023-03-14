using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Infrastructure.Data.TimeSeriesInsights;
using JP.DataHub.Infrastructure.Database.Data;
using JP.DataHub.UnitTest.Com;
using UnitTest.JP.DataHub.ApiWeb.Infrastructure.MockClass;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Data.TimeSeriesInsights
{
    [TestClass]
    public class UnitTest_JPDataHubTimeSeriesInsights : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private UnityContainer _unityContainer;
        private Mock<IJPDataHubHttpClientFactory> _mockHttpClientFactory;


        [TestInitialize]
        public void TestInitialize()
        {
            _unityContainer = new UnityContainer();
            UnityCore.UnityContainer = _unityContainer;

            _mockHttpClientFactory = new Mock<IJPDataHubHttpClientFactory>();
            _unityContainer.RegisterInstance(_mockHttpClientFactory.Object);

            _unityContainer.RegisterInstance(Configuration);
            _unityContainer.RegisterType<JPDataHubTimeSeriesInsights>();

            var perRequestDataContainer = new PerRequestDataContainer();
            _unityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
        }


        [TestMethod]
        public void QueryDocument_OK()
        {
            var expectedContent = "{ 'key': 'valye' }";
            var moqResponses = new MoqResponse[]
            {
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = "{ 'access_token': 'hoge' }" },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = expectedContent }
            };

            var moqHttpClient = SetupHttpClient(moqResponses);

            var tsi = _unityContainer.Resolve<JPDataHubTimeSeriesInsights>();
            tsi.Setting = new TimeSeriesInsightsSetting(Guid.NewGuid().ToString());

            var result = tsi.QueryDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            result.ToString().Is(JObject.Parse(expectedContent).ToString());
            moqHttpClient.Sequence.Is(2);
        }

        [TestMethod]
        public void QueryDocument_BadRequest()
        {
            var expectedContent = "{ 'key': 'valye' }";
            var moqResponses = new MoqResponse[]
            {
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = "{ 'access_token': 'hoge' }" },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.BadRequest, Content = expectedContent }
            };

            var moqHttpClient = SetupHttpClient(moqResponses);

            var tsi = _unityContainer.Resolve<JPDataHubTimeSeriesInsights>();
            tsi.Setting = new TimeSeriesInsightsSetting(Guid.NewGuid().ToString());

            AssertEx.Throws<QuerySyntaxErrorException>(() => tsi.QueryDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            moqHttpClient.Sequence.Is(2);
        }

        [TestMethod]
        public void QueryDocument_Retry()
        {
            var expectedContent = "{ 'key': 'valye' }";
            var moqResponses = new MoqResponse[]
            {
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = "{ 'access_token': 'hoge' }" },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.InternalServerError, Content = Guid.NewGuid().ToString() },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.InternalServerError, Content = Guid.NewGuid().ToString() },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = expectedContent }
            };

            var moqHttpClient = SetupHttpClient(moqResponses);

            var tsi = _unityContainer.Resolve<JPDataHubTimeSeriesInsights>();
            tsi.Setting = new TimeSeriesInsightsSetting(Guid.NewGuid().ToString());
            tsi.Setting.MaxAttempts = 2;

            var result = tsi.QueryDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            result.ToString().Is(JObject.Parse(expectedContent).ToString());
            moqHttpClient.Sequence.Is(4);
        }

        [TestMethod]
        public void QueryDocument_RetryOver()
        {
            var expectedContent = "{ 'key': 'valye' }";
            var moqResponses = new MoqResponse[]
            {
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = "{ 'access_token': 'hoge' }" },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.InternalServerError, Content = Guid.NewGuid().ToString() },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.InternalServerError, Content = Guid.NewGuid().ToString() },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.InternalServerError, Content = Guid.NewGuid().ToString() },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = expectedContent }
            };

            var moqHttpClient = SetupHttpClient(moqResponses);

            var tsi = _unityContainer.Resolve<JPDataHubTimeSeriesInsights>();
            tsi.Setting = new TimeSeriesInsightsSetting(Guid.NewGuid().ToString());
            tsi.Setting.MaxAttempts = 2;

            AssertEx.Throws<Exception>(() => tsi.QueryDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            moqHttpClient.Sequence.Is(4);
        }

        [TestMethod]
        public void QueryDocument_TokenCache()
        {
            var firstToken = Guid.NewGuid().ToString();
            var secondToken = Guid.NewGuid().ToString();
            var expectedContent1 = "{ 'key1': 'valye1' }";
            var expectedContent2 = "{ 'key2': 'valye2' }";
            var expectedContent3 = "{ 'key3': 'valye3' }";
            var moqResponses = new MoqResponse[]
            {
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = $"{{ 'access_token': '{firstToken}' }}" },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = expectedContent1 },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = expectedContent2 },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.Unauthorized, Content = Guid.NewGuid().ToString() },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = $"{{ 'access_token': '{secondToken}' }}" },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = expectedContent3 }
            };

            var moqHttpClient = SetupHttpClient(moqResponses);

            var tokenCacheKey = Guid.NewGuid().ToString();
            var tsi = _unityContainer.Resolve<JPDataHubTimeSeriesInsights>();
            tsi.Setting = new TimeSeriesInsightsSetting(tokenCacheKey);

            // 1回目(OK)
            var result = tsi.QueryDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            result.ToString().Is(JObject.Parse(expectedContent1).ToString());
            moqHttpClient.Sequence.Is(2);

            var tokenCache = (ConcurrentDictionary<string, string>)typeof(JPDataHubTimeSeriesInsights).GetFields(BindingFlags.NonPublic | BindingFlags.Static).First().GetValue(null);
            tokenCache.TryGetValue(tokenCacheKey, out var token).IsTrue();
            token.Is(firstToken);

            // 2回目(OK)
            result = tsi.QueryDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            result.ToString().Is(JObject.Parse(expectedContent2).ToString());
            moqHttpClient.Sequence.Is(3);

            // 3回目(期限切れ⇒リトライOK)
            result = tsi.QueryDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            result.ToString().Is(JObject.Parse(expectedContent3).ToString());
            moqHttpClient.Sequence.Is(6);

            tokenCache.TryGetValue(tokenCacheKey, out token).IsTrue();
            token.Is(secondToken);
        }

        [TestMethod]
        public void QueryDocument_MultiConnection()
        {
            var client1Token = Guid.NewGuid().ToString();
            var client2Token = Guid.NewGuid().ToString();
            var client1ExpectedContent = "{ 'key1': 'valye1' }";
            var client2ExpectedContent = "{ 'key2': 'valye2' }";
            var moqResponses = new MoqResponse[]
            {
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = $"{{ 'access_token': '{client1Token}' }}" },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = client1ExpectedContent },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = $"{{ 'access_token': '{client2Token}' }}" },
                new MoqResponse() { HttpStatusCode = HttpStatusCode.OK, Content = client2ExpectedContent }
            };

            var moqHttpClient = SetupHttpClient(moqResponses);

            var client1TokenCacheKey = Guid.NewGuid().ToString();
            var tsi1 = _unityContainer.Resolve<JPDataHubTimeSeriesInsights>();
            tsi1.Setting = new TimeSeriesInsightsSetting(client1TokenCacheKey);

            var client2TokenCacheKey = Guid.NewGuid().ToString();
            var tsi2 = _unityContainer.Resolve<JPDataHubTimeSeriesInsights>();
            tsi2.Setting = new TimeSeriesInsightsSetting(client2TokenCacheKey);


            // クライアント1(OK)
            var result = tsi1.QueryDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            result.ToString().Is(JObject.Parse(client1ExpectedContent).ToString());
            moqHttpClient.Sequence.Is(2);

            // クライアント2(OK)
            result = tsi2.QueryDocument(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            result.ToString().Is(JObject.Parse(client2ExpectedContent).ToString());
            moqHttpClient.Sequence.Is(4);

            var tokenCache = (ConcurrentDictionary<string, string>)typeof(JPDataHubTimeSeriesInsights).GetFields(BindingFlags.NonPublic | BindingFlags.Static).First().GetValue(null);
            tokenCache.TryGetValue(client1TokenCacheKey, out var token).IsTrue();
            token.Is(client1Token);
            tokenCache.TryGetValue(client2TokenCacheKey, out token).IsTrue();
            token.Is(client2Token);
        }


        private MoqHttpClient SetupHttpClient(MoqResponse[] moqResponses)
        {
            var moqClient = new MoqHttpClient();
            moqClient.MoqResponses = moqResponses;
            var client = new HttpClient(moqClient);
            client.BaseAddress = new Uri("http://localhost/");
            _mockHttpClientFactory.Setup(x => x.CreateClient()).Returns(client);

            return moqClient;
        }
    }
}