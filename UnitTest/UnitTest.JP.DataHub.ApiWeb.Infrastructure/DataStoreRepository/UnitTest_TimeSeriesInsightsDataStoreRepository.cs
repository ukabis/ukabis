using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.Data.TimeSeriesInsights;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [TestClass]
    public class UnitTest_TimeSeriesInsightsDataStoreRepository : UnitTestBase
    {
        public TestContext TestContext { get; set; }

#if Oracle
        private Mock<IEventHubStreamingService> _mockEventHub;
#else
        private Mock<IJPDataHubEventHub> _mockEventHub;
#endif
        private Mock<IJPDataHubTimeSeriesInsights> _mockTsi;

        private QueryStringVO _queryString;
        private string _qsValue1;
        private string _qsValue2;

        private string _timestampName;

        [TestInitialize]
        public override void TestInitialize()
        {
            _timestampName = "ts";


#if Oracle
            _mockEventHub = new Mock<IEventHubStreamingService>();
#else
            _mockEventHub = new Mock<IJPDataHubEventHub>();
#endif
            _mockTsi = new Mock<IJPDataHubTimeSeriesInsights>();
            _mockTsi.SetupGet(x => x.Setting).Returns(new TimeSeriesInsightsSetting("") { TimestampName = _timestampName });

            UnityCore.UnityContainer = new UnityContainer();
            UnityContainer = UnityCore.UnityContainer;

            UnityContainer.RegisterInstance(Configuration);
            UnityContainer.RegisterType<INewDynamicApiDataStoreRepository, TimeSeriesInsightsDataStoreRepository>();
#if Oracle
            UnityContainer.RegisterInstance<IEventHubStreamingService>(_mockEventHub.Object);
#else
            UnityContainer.RegisterInstance<IJPDataHubEventHub>(_mockEventHub.Object);
#endif
            UnityContainer.RegisterInstance<IJPDataHubTimeSeriesInsights>(_mockTsi.Object);

            var dataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(dataContainer);

            _qsValue1 = Guid.NewGuid().ToString();
            _qsValue2 = Guid.NewGuid().ToString();
            _queryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                { new QueryStringKey("key1"), new QueryStringValue(_qsValue1) },
                { new QueryStringKey("key2"), new QueryStringValue(_qsValue2) }
            });
        }


        #region Query

        [TestMethod]
        public void Query_OK_依存なし()
        {
            var inputQuery = @"{ 'getEvents': { 'timeSeriesId': [ '{key1}', '{key2}' ] } }";
            var expectedQuery = $@"{{ ""getEvents"": {{ ""timeSeriesId"": [ '{_qsValue1}', '{_qsValue2}'] }} }}";
            var response = @"
{
    'timestamps': [ '2021-11-09T18:12:00Z', '2021-11-09T18:13:00Z' ],
    'properties': 
    [
        { 'name': 'hoge', 'Type': 'String', 'values': [ 'A', 'B' ] },
        { 'name': 'fuga', 'Type': 'Long', 'values': [ 1, 2 ] }
    ]
}";
            var expectedResult = new List<JsonDocument>()
            {
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:12:00Z', 'hoge': 'A', 'fuga': 1 }}")),
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:13:00Z', 'hoge': 'B', 'fuga': 2 }}"))
            };

            _mockTsi.Setup(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((query, token) =>
                {
                    query.Is(JToken.Parse(expectedQuery).ToString());
                })
                .Returns(JObject.Parse(response));

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var param = ValueObjectUtil.Create<QueryParam>(new ApiQuery(inputQuery), _queryString);
            var result = target.Query(param, out var xResponseContinuation);
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Value.ToString().Is(expectedResult[i].Value.ToString());
            }
            xResponseContinuation.IsNull();

            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Query_NotFound()
        {
            var inputQuery = @"{ 'getEvents': { 'timeSeriesId': [ '{key1}', '{key2}' ] } }";
            var expectedQuery = $@"{{ ""getEvents"": {{ ""timeSeriesId"": [ '{_qsValue1}', '{_qsValue2}'] }} }}";
            var response = @"{ 'timestamps': [], 'properties': [] }";

            _mockTsi.Setup(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((query, token) =>
                {
                    query.Is(JToken.Parse(expectedQuery).ToString());
                })
                .Returns(JObject.Parse(response));

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var param = ValueObjectUtil.Create<QueryParam>(new ApiQuery(inputQuery), _queryString);
            var result = target.Query(param, out var xResponseContinuation);
            result.Count.Is(0);
            xResponseContinuation.IsNull();

            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Query_OK_ページング()
        {
            var inputQuery = @"{ 'getEvents': { 'timeSeriesId': [ '{key1}', '{key2}' ] } }";
            var continuationToken = Guid.NewGuid().ToString();
            var response1 = $@"
{{
    'timestamps': [ '2021-11-09T18:12:00Z', '2021-11-09T18:13:00Z' ],
    'properties': 
    [
        {{ 'name': 'hoge', 'Type': 'String', 'values': [ 'A', 'B' ] }},
        {{ 'name': 'fuga', 'Type': 'Long', 'values': [ 1, 2 ] }}
    ],
    'continuationToken': '{continuationToken}'
}}";
            var response2 = $@"
{{
    'timestamps': [ '2021-11-09T18:14:00Z', '2021-11-09T18:15:00Z' ],
    'properties': 
    [
        {{ 'name': 'hoge', 'Type': 'String', 'values': [ 'C', 'D' ] }},
        {{ 'name': 'fuga', 'Type': 'Long', 'values': [ 3, 4 ] }}
    ]
}}";
            var expectedResult = new List<JsonDocument>()
            {
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:12:00Z', 'hoge': 'A', 'fuga': 1 }}")),
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:13:00Z', 'hoge': 'B', 'fuga': 2 }}")),
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:14:00Z', 'hoge': 'C', 'fuga': 3 }}")),
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:15:00Z', 'hoge': 'D', 'fuga': 4 }}"))
            };

            _mockTsi.SetupSequence(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(JObject.Parse(response1))
                .Returns(JObject.Parse(response2));

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var param = ValueObjectUtil.Create<QueryParam>(new ApiQuery(inputQuery), _queryString);
            var result = target.Query(param, out var xResponseContinuation);
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Value.ToString().Is(expectedResult[i].Value.ToString());
            }
            xResponseContinuation.IsNull();

            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.Is<string>(y => y == null)), Times.Exactly(1));
            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.Is<string>(z => z == continuationToken)), Times.Exactly(1));
        }

        [TestMethod]
        public void Query_OK_ベンダー依存()
        {
            var vendorId = Guid.NewGuid().ToString();
            var systemId = Guid.NewGuid().ToString();

            var inputQuery = @"{ 'getEvents': { 'timeSeriesId': [ '{key1}', '{key2}' ] } }";
            var expectedQuery = $@"{{ ""getEvents"": {{ ""timeSeriesId"": [ '{_qsValue1}', '{_qsValue2}'], ""filter"": {{ ""tsx"": ""$event._Vendor_Id.String='{vendorId}' AND $event._System_Id.String='{systemId}'"" }} }} }}";
            var response = @"
{
    'timestamps': [ '2021-11-09T18:12:00Z', '2021-11-09T18:13:00Z' ],
    'properties': 
    [
        { 'name': 'hoge', 'Type': 'String', 'values': [ 'A', 'B' ] },
        { 'name': 'fuga', 'Type': 'Long', 'values': [ 1, 2 ] }
    ]
}";
            var expectedResult = new List<JsonDocument>()
            {
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:12:00Z', 'hoge': 'A', 'fuga': 1 }}")),
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:13:00Z', 'hoge': 'B', 'fuga': 2 }}"))
            };

            _mockTsi.Setup(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((query, token) =>
                {
                    query.Is(JToken.Parse(expectedQuery).ToString());
                })
                .Returns(JObject.Parse(response));

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var param = ValueObjectUtil.Create<QueryParam>(new ApiQuery(inputQuery), _queryString, new IsVendor(true), new VendorId(vendorId), new SystemId(systemId));
            var result = target.Query(param, out var xResponseContinuation);
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Value.ToString().Is(expectedResult[i].Value.ToString());
            }
            xResponseContinuation.IsNull();

            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Query_OK_ベンダー依存_領域越え()
        {
            var vendorId = Guid.NewGuid().ToString();
            var systemId = Guid.NewGuid().ToString();

            var inputQuery = @"{ 'aggregateSeries': { 'timeSeriesId': [ '{key1}', '{key2}' ], 'filter': { 'tsx': 'hoge' } } }";
            var expectedQuery = $@"{{ ""aggregateSeries"": {{ ""timeSeriesId"": [ '{_qsValue1}', '{_qsValue2}'], ""filter"": {{ ""tsx"": ""hoge"" }} }} }}";
            var response = @"
{
    'timestamps': [ '2021-11-09T18:12:00Z', '2021-11-09T18:13:00Z' ],
    'properties': 
    [
        { 'name': 'hoge', 'Type': 'String', 'values': [ 'A', 'B' ] },
        { 'name': 'fuga', 'Type': 'Long', 'values': [ 1, 2 ] }
    ]
}";
            var expectedResult = new List<JsonDocument>()
            {
                new JsonDocument(JObject.Parse($"{{ 'timestamp': '2021-11-09T18:12:00Z', 'hoge': 'A', 'fuga': 1 }}")),
                new JsonDocument(JObject.Parse($"{{ 'timestamp': '2021-11-09T18:13:00Z', 'hoge': 'B', 'fuga': 2 }}"))
            };

            _mockTsi.Setup(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((query, token) =>
                {
                    query.Is(JToken.Parse(expectedQuery).ToString());
                })
                .Returns(JObject.Parse(response));

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var param = ValueObjectUtil.Create<QueryParam>(new ApiQuery(inputQuery), _queryString, new IsVendor(true), new VendorId(vendorId), new SystemId(systemId), new IsOverPartition(true));
            var result = target.Query(param, out var xResponseContinuation);
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Value.ToString().Is(expectedResult[i].Value.ToString());
            }
            xResponseContinuation.IsNull();

            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Query_OK_ベンダー依存_データ共有()
        {
            var vendorId = Guid.NewGuid().ToString();
            var systemId = Guid.NewGuid().ToString();
            var sharingToVendorId = Guid.NewGuid().ToString();
            var sharingToSystemId = Guid.NewGuid().ToString();

            var inputQuery = @"{ 'getEvents': { 'timeSeriesId': [ '{key1}', '{key2}' ], 'filter': { 'tsx': 'hoge' } } }";
            var expectedQuery = $@"{{ ""getEvents"": {{ ""timeSeriesId"": [ '{_qsValue1}', '{_qsValue2}'], ""filter"": {{ ""tsx"": ""(hoge) AND $event._Vendor_Id.String='{sharingToVendorId}' AND $event._System_Id.String='{sharingToSystemId}'"" }} }} }}";
            var response = @"
{
    'timestamps': [ '2021-11-09T18:12:00Z', '2021-11-09T18:13:00Z' ],
    'properties': 
    [
        { 'name': 'hoge', 'Type': 'String', 'values': [ 'A', 'B' ] },
        { 'name': 'fuga', 'Type': 'Long', 'values': [ 1, 2 ] }
    ]
}";
            var expectedResult = new List<JsonDocument>()
            {
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:12:00Z', 'hoge': 'A', 'fuga': 1 }}")),
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:13:00Z', 'hoge': 'B', 'fuga': 2 }}"))
            };

            _mockTsi.Setup(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((query, token) =>
                {
                    query.Is(JToken.Parse(expectedQuery).ToString());
                })
                .Returns(JObject.Parse(response));

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var rule = new ApiResourceSharingRule(Guid.NewGuid(), Guid.NewGuid(), null, null, null, true);
            var sharing = new ApiResourceSharing() { ResourceSharingRuleList = new List<ApiResourceSharingRule>() { rule } };
            var sharingWith = new XResourceSharingWith(new Dictionary<string, string>() { { "VendorId", sharingToVendorId }, { "SystemId", sharingToSystemId } });
            var param = ValueObjectUtil.Create<QueryParam>(new ApiQuery(inputQuery), _queryString, new IsVendor(true), new VendorId(vendorId), new SystemId(systemId), sharing, sharingWith);
            var result = target.Query(param, out var xResponseContinuation);
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Value.ToString().Is(expectedResult[i].Value.ToString());
            }
            xResponseContinuation.IsNull();

            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Query_OK_個人依存()
        {
            var ownerId = Guid.NewGuid().ToString();

            var inputQuery = @"{ 'getEvents': { 'timeSeriesId': [ '{key1}', '{key2}' ] } }";
            var expectedQuery = $@"{{ ""getEvents"": {{ ""timeSeriesId"": [ '{_qsValue1}', '{_qsValue2}'], ""filter"": {{ ""tsx"": ""$event._Owner_Id.String='{ownerId}'"" }} }} }}";
            var response = @"
{
    'timestamps': [ '2021-11-09T18:12:00Z', '2021-11-09T18:13:00Z' ],
    'properties': 
    [
        { 'name': 'hoge', 'Type': 'String', 'values': [ 'A', 'B' ] },
        { 'name': 'fuga', 'Type': 'Long', 'values': [ 1, 2 ] }
    ]
}";
            var expectedResult = new List<JsonDocument>()
            {
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:12:00Z', 'hoge': 'A', 'fuga': 1 }}")),
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:13:00Z', 'hoge': 'B', 'fuga': 2 }}"))
            };

            _mockTsi.Setup(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((query, token) =>
                {
                    query.Is(JToken.Parse(expectedQuery).ToString());
                })
                .Returns(JObject.Parse(response));

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var param = ValueObjectUtil.Create<QueryParam>(new ApiQuery(inputQuery), _queryString, new IsPerson(true), new OpenId(ownerId));
            var result = target.Query(param, out var xResponseContinuation);
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Value.ToString().Is(expectedResult[i].Value.ToString());
            }
            xResponseContinuation.IsNull();

            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Query_OK_個人依存_領域越え()
        {
            var ownerId = Guid.NewGuid().ToString();

            var inputQuery = @"{ 'aggregateSeries': { 'timeSeriesId': [ '{key1}', '{key2}' ], 'filter': { 'tsx': 'hoge' } } }";
            var expectedQuery = $@"{{ ""aggregateSeries"": {{ ""timeSeriesId"": [ '{_qsValue1}', '{_qsValue2}'], ""filter"": {{ ""tsx"": ""hoge"" }} }} }}";
            var response = @"
{
    'timestamps': [ '2021-11-09T18:12:00Z', '2021-11-09T18:13:00Z' ],
    'properties': 
    [
        { 'name': 'hoge', 'Type': 'String', 'values': [ 'A', 'B' ] },
        { 'name': 'fuga', 'Type': 'Long', 'values': [ 1, 2 ] }
    ]
}";
            var expectedResult = new List<JsonDocument>()
            {
                new JsonDocument(JObject.Parse($"{{ 'timestamp': '2021-11-09T18:12:00Z', 'hoge': 'A', 'fuga': 1 }}")),
                new JsonDocument(JObject.Parse($"{{ 'timestamp': '2021-11-09T18:13:00Z', 'hoge': 'B', 'fuga': 2 }}"))
            };

            _mockTsi.Setup(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((query, token) =>
                {
                    query.Is(JToken.Parse(expectedQuery).ToString());
                })
                .Returns(JObject.Parse(response));

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var param = ValueObjectUtil.Create<QueryParam>(new ApiQuery(inputQuery), _queryString, new IsPerson(true), new OpenId(ownerId), new IsOverPartition(true));
            var result = target.Query(param, out var xResponseContinuation);
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Value.ToString().Is(expectedResult[i].Value.ToString());
            }
            xResponseContinuation.IsNull();

            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Query_OK_個人依存_データ共有()
        {
            var ownerId = Guid.NewGuid().ToString();
            var sharingToOpenId = Guid.NewGuid().ToString();

            var inputQuery = @"{ 'getEvents': { 'timeSeriesId': [ '{key1}', '{key2}' ], 'filter': { 'tsx': 'hoge' } } }";
            var expectedQuery = $@"{{ ""getEvents"": {{ ""timeSeriesId"": [ '{_qsValue1}', '{_qsValue2}'], ""filter"": {{ ""tsx"": ""(hoge) AND $event._Owner_Id.String='{sharingToOpenId}'"" }} }} }}";
            var response = @"
{
    'timestamps': [ '2021-11-09T18:12:00Z', '2021-11-09T18:13:00Z' ],
    'properties': 
    [
        { 'name': 'hoge', 'Type': 'String', 'values': [ 'A', 'B' ] },
        { 'name': 'fuga', 'Type': 'Long', 'values': [ 1, 2 ] }
    ]
}";
            var expectedResult = new List<JsonDocument>()
            {
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:12:00Z', 'hoge': 'A', 'fuga': 1 }}")),
                new JsonDocument(JObject.Parse($"{{ '{_timestampName}': '2021-11-09T18:13:00Z', 'hoge': 'B', 'fuga': 2 }}"))
            };

            _mockTsi.Setup(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string>((query, token) =>
                {
                    query.Is(JToken.Parse(expectedQuery).ToString());
                })
                .Returns(JObject.Parse(response));

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var rule = new ResourceSharingPersonRule(new ResourceSharingPersonRuleId(""), null, new RelativeUri(""), null, null, null, null, null, null, new IsActive(true), null, null);
            var sharing = new List<ResourceSharingPersonRule>() { rule };
            var sharingPerson = new XResourceSharingPerson(sharingToOpenId);
            var param = ValueObjectUtil.Create<QueryParam>(new ApiQuery(inputQuery), _queryString, new IsPerson(true), new OpenId(ownerId), sharing, sharingPerson);
            var result = target.Query(param, out var xResponseContinuation);
            for (var i = 0; i < result.Count; i++)
            {
                result[i].Value.ToString().Is(expectedResult[i].Value.ToString());
            }
            xResponseContinuation.IsNull();

            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Query_APIクエリなし()
        {
            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var param = ValueObjectUtil.Create<QueryParam>();
            AssertEx.Throws<QuerySyntaxErrorException>(() => target.Query(param, out var xResponseContinuation));

            _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        [TestCase("hoge")]
        [TestCase("{ 'hoge': 'fuga' }")]
        public void Query_APIクエリが不正()
        {
            TestContext.Run((string apiQuery) =>
            {
                var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
                target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

                var param = ValueObjectUtil.Create<QueryParam>(new ApiQuery(apiQuery));
                AssertEx.Throws<QuerySyntaxErrorException>(() => target.Query(param, out var xResponseContinuation));

                _mockTsi.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(0));
            });
        }

        #endregion

        #region Register

        [TestMethod]
        public void RegisterOnce_OK()
        {
            var json = JToken.Parse("{ 'key': 'value' }");

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.TimeSeriesInsights.ToCode(), new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });

            var result = target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(json));
            result.IsNull();

            _mockEventHub.Verify(x => x.SendMessageAsync(It.IsAny<JToken>(), It.IsAny<string>()), Times.Exactly(1));
        }

        #endregion
    }
}
