using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Infrastructure.Database.Data;
using JP.DataHub.Infrastructure.Database.Data.MongoDb;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [TestClass]
    public class UnitTest_NewMongoDbDataStoreRepository : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private static readonly XRequestContinuation xRequestContinuation = new XRequestContinuation("DUMMY");
        private static readonly NativeQuery nativeQuery = new NativeQuery(@"{ ""Select"": ""{ 'id': 1, 'key': 1 }"", ""Where"": ""{ 'key': 'hoge' }"", ""OrderBy"": ""{ 'key': 1 }"", ""Top"": null, ""Aggregate"": null }", new Dictionary<string, object>());
        private static readonly NativeQuery nativeQueryWithTop = new NativeQuery(@"{ ""Select"": ""{ 'id': 1, 'key': 1 }"", ""Where"": ""{ 'key': 'hoge' }"", ""OrderBy"": ""{ 'key': 1 }"", ""Top"": 1, ""Aggregate"": null }", new Dictionary<string, object>());
        private static readonly ApiQuery apiODataQuery = new ApiQuery("$filter=id eq 'hoge'");
        private static readonly ApiQuery apiODataQueryWithTop = new ApiQuery("$filter=id eq 'hoge'&$top=1");
        private static readonly ApiQuery apiQuery = new ApiQuery("{ 'Select': { 'id': 1, 'key': 1 }, 'Where': { 'key': 'hoge' }, 'OrderBy': { 'key': 1 }, 'Top': null, 'Aggregate': null }");
        private static readonly ApiQuery apiQueryAggregate = new ApiQuery("{ Aggregate: [], Top: 1 }");

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>();

            var mockContainerDynamicSeparationRepository = new Mock<IContainerDynamicSeparationRepository>();
            UnityContainer.RegisterInstance(mockContainerDynamicSeparationRepository.Object);
        }

        #region Query

        [TestMethod]
        public void QueryOnce_OK_0件() => ExecuteQueryOnce(0);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation無() => ExecuteQueryOnce(1);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation有() => ExecuteQueryOnce(1, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation無_NativeQuery有() => ExecuteQueryOnce(1, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation有_NativeQuery有() => ExecuteQueryOnce(1, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation無_ApiQuery有() => ExecuteQueryOnce(1, apiQuery: apiQuery);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation有_ApiQuery有() => ExecuteQueryOnce(1, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation無() => ExecuteQueryOnce(10);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation有() => ExecuteQueryOnce(10, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation無_NativeQuery有() => ExecuteQueryOnce(10, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation有_NativeQuery有() => ExecuteQueryOnce(10, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation無_ApiQuery有() => ExecuteQueryOnce(10, apiQuery: apiQuery);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation有_ApiQuery有() => ExecuteQueryOnce(10, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void QueryEnumerable_OK_0件() => ExecuteQueryEnumerable(0);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation無() => ExecuteQueryEnumerable(1);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation有() => ExecuteQueryEnumerable(1, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation無_NativeQuery有() => ExecuteQueryEnumerable(1, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation有_NativeQuery有() => ExecuteQueryEnumerable(1, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation無_ApiQuery有() => ExecuteQueryEnumerable(1, apiQuery: apiQuery);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation有_ApiQuery有() => ExecuteQueryEnumerable(1, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation無() => ExecuteQueryEnumerable(10);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation有() => ExecuteQueryEnumerable(10, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation無_NativeQuery有() => ExecuteQueryEnumerable(10, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation有_NativeQuery有() => ExecuteQueryEnumerable(10, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation無_ApiQuery有() => ExecuteQueryEnumerable(10, apiQuery: apiQuery);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation有_ApiQuery有() => ExecuteQueryEnumerable(10, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void Query_OK_0件() => ExecuteQuery(0);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation無() => ExecuteQuery(1);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation有() => ExecuteQuery(1, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation無_NativeQuery有() => ExecuteQuery(1, nativeQuery: nativeQuery);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation有_NativeQuery有() => ExecuteQuery(1, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation無_ApiQuery有() => ExecuteQuery(1, apiQuery: apiQuery);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation有_ApiQuery有() => ExecuteQuery(1, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation無() => ExecuteQuery(10);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation有() => ExecuteQuery(10, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation無_NativeQuery有() => ExecuteQuery(10, nativeQuery: nativeQuery);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation有_NativeQuery有() => ExecuteQuery(10, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation無_ApiQuery有() => ExecuteQuery(10, apiQuery: apiQuery);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation有_ApiQuery有() => ExecuteQuery(10, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);


        [TestMethod]
        [TestCase(ActionType.DeleteData, null, false, true)]
        [TestCase(ActionType.DeleteData, false, false, true)]
        [TestCase(ActionType.DeleteData, true, false, false)]
        [TestCase(ActionType.ODataDelete, null, false, true)]
        [TestCase(ActionType.ODataDelete, false, false, true)]
        [TestCase(ActionType.ODataDelete, true, false, false)]
        [TestCase(ActionType.DeleteData, false, true, false)]
        [TestCase(ActionType.ODataDelete, false, true, false)]
        public void QueryEnumerable_OK_Delete()
        {
            TestContext.Run((ActionType actionType, bool? isDocumentHistory, bool isAggregation, bool getOnlyId) =>
            {
                var target = CreateDummyRepository();
                var (targetData, expectData) = CreateQueryTestData(2);

                // MockでconnectionClientを上書き
                var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
                SetUpQueryDocument(mockJPDataHubMongoDB, targetData, null);
                SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

                // ダミーのActionを作成
                var apiQuery = isAggregation ? new ApiQuery(@"
{
    ""Aggregate"": [
        { ""$match"": { ""key"": ""hoge"" } }
    ]
}") : null;
                var dummyAction = CreateDummyQueryAction(null, apiQuery, new ActionTypeVO(actionType), isDocumentHistory.HasValue ? new IsDocumentHistory(isDocumentHistory.Value) : null);

                // 対象メソッド実行
                var result = target.QueryEnumerable(ValueObjectUtil.Create<QueryParam>(dummyAction)).ToList();

                // Mockの期待値が返却されること
                result.Count.Is(2);
                result.IsStructuralEqual(expectData);

                // 呼ばれたか確認
                VerifyQueryDocument(mockJPDataHubMongoDB, null);
                //mockJPDataHubMongoDB.Verify(x => x.QueryDocument(
                //    It.IsAny<string>(), 
                //    It.Is<string>(y => y == (getOnlyId ? "{ \"id\": 1, \"_id\": 0 }" : (isAggregation ? null : ""))), 
                //    It.IsAny<string>(), 
                //    It.IsAny<int?>(), 
                //    It.IsAny<string>()
                //    ), Times.Once);
            });
        }


        [TestMethod]
        public void 既定のクエリ_QueryString無()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""Name"": /Test/
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}");
        }

        [TestMethod]
        public void 既定のクエリ_QueryString無_AND()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""$and"": [
            {
                ""Name"": /Test/
            }
        ]
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}");
        }

        [TestMethod]
        public void 既定のクエリ_QueryString無_OR()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""$or"": [
            {
                ""Name"": /Test/
            }
        ]
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}");
        }

        [TestMethod]
        public void 既定のクエリ_QueryString無_複数()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""Name"": /Test/,
        ""Name2"": /Test2/
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}");
        }

        [TestMethod]
        public void 既定のクエリ_QueryString無_複数_AND()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""$and"": [
            {
                ""Name"": /Test/
            },
            {
                ""Name2"": /Test2/
            }
        ]
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}");
        }

        [TestMethod]
        public void 既定のクエリ_QueryString無_複数_OR()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""$or"": [
            {
                ""Name"": /Test/
            },
            {
                ""Name2"": /Test2/
            }
        ]
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}");
        }

        [TestMethod]
        public void 既定のクエリ_QueryString有()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""Code"": {Code}
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}"
            , new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_QueryString有_AND()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""$and"": [
            {
                ""Code"": {Code}
            }
        ]
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}"
            , new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_QueryString有_OR()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""$or"": [
            {
                ""Code"": {Code}
            }
        ]
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}"
            , new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_QueryString有_複数()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""Code"": {Code},
        ""Code2"": {Code2}
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}"
            ,
            new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA"),
                [new QueryStringKey("Code2")] = new QueryStringValue("BB")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_QueryString有_複数_AND()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""$and"": [
            {
                ""Code"": {Code}
            },
            {
                ""Code2"": {Code2}
            }
        ]
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}"
            ,
            new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA"),
                [new QueryStringKey("Code2")] = new QueryStringValue("BB")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_QueryString有_複数_OR()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""$or"": [
            {
                ""Code"": {Code}
            },
            {
                ""Code2"": {Code2}
            }
        ]
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}"
            ,
            new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA"),
                [new QueryStringKey("Code2")] = new QueryStringValue("BB")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_階層あり()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""$and"": [
            {
                ""Code"": {Code}
            },
            {
                ""$or"": [
                    {
                        ""Code2"": {Code2}
                    },
                    {
                        ""Code3"": {Code3}
                    }
                ]
            }
        ]
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}"
            ,
            new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA"),
                [new QueryStringKey("Code2")] = new QueryStringValue("BB"),
                [new QueryStringKey("Code3")] = new QueryStringValue("CC"),
            }));
        }

        // 実際には必ず_partitionkeyなどのVersion情報が検索条件に追加されるため、/ODataと同じ結果になる
        [TestMethod]
        public void 既定のクエリ_全項目Null()
        {
            ExcuteQueryAsDefaultQuery(@"
{
    ""Select"": null,
    ""Where"": null,
    ""OrderBy"": null,
    ""Top"": null
}");
        }

        [TestMethod]
        public void 既定のクエリ_Aggregate_geoNearなし()
        {
            ExcuteQueryWithAggregate(@"
{
    ""Aggregate"": [
        { ""$match"": { ""key"": {Code} } }
    ]
}"
            ,
            new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_Aggregate_geoNearあり_queryなし()
        {
            ExcuteQueryWithAggregate(@"
{
    ""Aggregate"": [
        { 
            ""$geoNear"": 
            { 
                ""near"": { ""type"": ""Point"", ""coordinates"": [ {Lat} , {Lng} ]},
                ""distanceField"": ""dist.calculated"",
                ""maxDistance"": 56,
                ""includeLocs"": ""dist.location"",
                ""key"": ""GeoSearch.GeoSearch_1"" 
            } 
        },
        { ""$match"": { ""key"": {Code} } }
    ]
}"
            ,
            new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA"),
                [new QueryStringKey("Lat")] = new QueryStringValue("1.0015"),
                [new QueryStringKey("Lng")] = new QueryStringValue("1.000")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_Aggregate_geoNearあり_queryあり()
        {
            ExcuteQueryWithAggregate(@"
{
    ""Aggregate"": [
        { 
            ""$geoNear"": 
            { 
                ""near"": { ""type"": ""Point"", ""coordinates"": [ {Lat} , {Lng} ] },
                ""distanceField"": ""dist.calculated"",
                ""maxDistance"": 56,
                ""includeLocs"": ""dist.location"",
                ""key"": ""GeoSearch.GeoSearch_1"",
                ""query"": { ""geokey"": ""geovalue"" }
            } 
        },
        { ""$match"": { ""key"": {Code} } }
    ]
}"
            ,
            new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA"),
                [new QueryStringKey("Lat")] = new QueryStringValue("1.0015"),
                [new QueryStringKey("Lng")] = new QueryStringValue("1.000")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_Aggregate_unionWith_geoNearなし()
        {
            ExcuteQueryWithAggregate(@"
{
    ""Aggregate"": [
        { ""$match"": { ""key"": {Code} } },
        { ""$unionWith"": { ""coll"": {COLLECTION_NAME}, ""pipeline"": [{ ""$match"": { ""key2"": {Code2} } }] } }
    ]
}"
            ,
            new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA"),
                [new QueryStringKey("Code2")] = new QueryStringValue("BB")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_Aggregate_unionWith_geoNearあり_queryなし()
        {
            ExcuteQueryWithAggregate(@"
{
    ""Aggregate"": [
        { ""$match"": { ""key"": {Code} } },
        { ""$unionWith"": 
            { 
                ""coll"": {COLLECTION_NAME}, 
                ""pipeline"": [
                    {
                        ""$geoNear"": 
                        { 
                            ""near"": { ""type"": ""Point"", ""coordinates"": [ {Lat} , {Lng} ] },
                            ""distanceField"": ""dist.calculated"",
                            ""maxDistance"": 56,
                            ""includeLocs"": ""dist.location"",
                            ""key"": ""GeoSearch.GeoSearch_1""
                        }
                    }
                ] 
            } 
        }
    ]
}"
            ,
            new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA"),
                [new QueryStringKey("Lat")] = new QueryStringValue("1.0015"),
                [new QueryStringKey("Lng")] = new QueryStringValue("1.000")
            }));
        }

        [TestMethod]
        public void 既定のクエリ_Aggregate_unionWith_geoNearあり_queryあり()
        {
            ExcuteQueryWithAggregate(@"
{
    ""Aggregate"": [
        { ""$match"": { ""key"": {Code} } },
        { ""$unionWith"": 
            { 
                ""coll"": {COLLECTION_NAME}, 
                ""pipeline"": [
                    {
                        ""$geoNear"": 
                        { 
                            ""near"": { ""type"": ""Point"", ""coordinates"": [ {Lat} , {Lng} ] },
                            ""distanceField"": ""dist.calculated"",
                            ""maxDistance"": 56,
                            ""includeLocs"": ""dist.location"",
                            ""key"": ""GeoSearch.GeoSearch_1""
                            ""query"": { ""geokey"": ""geovalue"" }
                        }
                    }
                ] 
            } 
        }
    ]
}"
            ,
            new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA"),
                [new QueryStringKey("Lat")] = new QueryStringValue("1.0015"),
                [new QueryStringKey("Lng")] = new QueryStringValue("1.000")
            }));
        }

        #region XRequestContinuationNeedsTopCount

        [TestMethod]
        public void QueryOnece_NeedsTopCount有_top無() => ExecuteQueryOnceWithNeedsTopCount(apiODataQuery, nativeQuery, false);

        [TestMethod]
        public void QueryOnece_NeedsTopCount有_top有() => ExecuteQueryOnceWithNeedsTopCount(apiODataQueryWithTop, nativeQueryWithTop, true);

        [TestMethod]
        public void QueryOnece_NeedsTopCount有_aggregate() => ExecuteQueryOnceWithNeedsTopCount(apiQueryAggregate, null, true);


        [TestMethod]
        public void QueryEnumerable_NeedsTopCount有_top無() => ExecuteQueryEnumerableWithNeedsTopCount(apiODataQuery, nativeQuery, false);

        [TestMethod]
        public void QueryEnumerable_NeedsTopCount有_top有() => ExecuteQueryEnumerableWithNeedsTopCount(apiODataQueryWithTop, nativeQueryWithTop, true);

        [TestMethod]
        public void QueryEnumerable_NeedsTopCount有_aggregate() => ExecuteQueryEnumerableWithNeedsTopCount(apiQueryAggregate, null, true);


        [TestMethod]
        public void Query_NeedsTopCount有_top無() => ExecuteQueryWithNeedsTopCount(apiODataQuery, nativeQuery, false);

        [TestMethod]
        public void Query_NeedsTopCount有_top有() => ExecuteQueryWithNeedsTopCount(apiODataQueryWithTop, nativeQueryWithTop, true);

        [TestMethod]
        public void Query_NeedsTopCount有_aggregate() => ExecuteQueryWithNeedsTopCount(apiQueryAggregate, null, true);

        #endregion

        #region コンテナ分離＋領域越え

        [TestMethod]
        public void ExecuteQueryEnumerable_コンテナ分離領域越え_Query()
        {
            var defaultQuery = @"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""$and"": [
            {
                ""Code"": {Code}
            },
            {
                ""$or"": [
                    {
                        ""Code2"": {Code2}
                    },
                    {
                        ""Code3"": {Code3}
                    }
                ]
            }
        ]
    },
    ""OrderBy"": {
        ""Code"": 1
    },
    ""Top"": 2
}";
            var targetQueryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code")] = new QueryStringValue("AA"),
                [new QueryStringKey("Code2")] = new QueryStringValue("BB"),
                [new QueryStringKey("Code3")] = new QueryStringValue("CC")
            });

            var mainContainerName = Guid.NewGuid().ToString();
            var containerNames = new List<string>() { mainContainerName, Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var mockContainerDynamicSeparationRepository = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparationRepository.Setup(x => x.GetAllContainerNames(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>())).Returns(containerNames);
            UnityContainer.RegisterInstance(mockContainerDynamicSeparationRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Guid.NewGuid().ToString();
            perRequestDataContainer.SystemId = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance(perRequestDataContainer);

            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(1);

            var partitionKey = Guid.NewGuid().ToString();
            var metaConditions = new Dictionary<string, object>()
            {
                { "_Type", partitionKey },
                { "_Version", 1 }
            };

            // MockでconnectionClientを上書き
            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            mockJPDataHubMongoDB.SetupGet(x => x.CollectionName).Returns(mainContainerName);
            mockJPDataHubMongoDB.Setup(x => x.QueryDocument(It.IsAny<IEnumerable<BsonDocument>>()))
                .Callback<IEnumerable<BsonDocument>>((pipeline) =>
                {
                    // 「既定のクエリ」をBsonに変換（MongoDBの既定のクエリはBson形式も可とする）
                    var expectedQuery = defaultQuery.Replace("{COLLECTION_NAME}", $"\"{mainContainerName}\"");
                    targetQueryString?.Dic.ToList().ForEach(x => expectedQuery = expectedQuery.Replace($"{{{x.Key.Value}}}", $"\"{x.Value.Value}\""));
                    var expectedBson = expectedQuery.ToDecimalizedBsonDocument();

                    // 引数チェック
                    var i = 0;
                    var pipelineArray = pipeline.ToList();
                    VerifyQueryDocumentWhereConditions(expectedBson["Where"], pipelineArray[i++]["$match"].ToString(), metaConditions);
                    foreach (var containerName in containerNames.Where(x => x != mainContainerName))
                    {
                        var unionWith = pipelineArray[i++]["$unionWith"];
                        unionWith["coll"].ToString().Is(containerName);
                        VerifyQueryDocumentWhereConditions(expectedBson["Where"], unionWith["pipeline"].AsBsonArray.Single()["$match"].ToString(), metaConditions);
                    }
                    if (expectedBson["OrderBy"] != BsonNull.Value)
                    {
                        VerifyQueryDocumentBsonProperty(expectedBson["OrderBy"], pipelineArray[i++]["$sort"].ToString());
                    }
                    if (expectedBson["Select"] != BsonNull.Value)
                    {
                        VerifyQueryDocumentBsonProperty(expectedBson["Select"], pipelineArray[i++]["$project"].ToString());
                    }
                    if (expectedBson["Top"] != BsonNull.Value)
                    {
                        VerifyQueryDocumentBsonProperty(expectedBson["Top"], resultInt: pipelineArray[i++]["$limit"].ToInt32());
                    }
                })
                .Returns(targetData);
            SetDummyConnectionClient(mockJPDataHubMongoDB.Object, "endpoint=;database=;collection=");

            // ダミーのActionを作成
            var dummyAction = new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.ApiQuery, new ApiQuery(defaultQuery))
                .SetupProperty(x => x.QueryType, new QueryType(QueryTypes.NativeDbQuery))
                .SetupProperty(x => x.Query, targetQueryString)
                .SetupProperty(x => x.RepositoryKey, new RepositoryKey(partitionKey))
                .SetupProperty(x => x.IsContainerDynamicSeparation, new IsContainerDynamicSeparation(true))
                .SetupProperty(x => x.IsOverPartition, new IsOverPartition(true))
                .SetupProperty(x => x.IsVendor, new IsVendor(true))
                .Object;

            // 「既定のクエリ」を設定した場合のquery（Privateメソッド）の動作を確認
            var result = target.Query(ValueObjectUtil.Create<QueryParam>(dummyAction), out XResponseContinuation _).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(1);
            result.IsStructuralEqual(expectData);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubMongoDB, null);
        }

        [TestMethod]
        public void ExecuteQueryEnumerable_コンテナ分離領域越え_Aggregate()
        {
            var defaultQuery = @"
{
    ""Aggregate"": [
        { ""$match"": { ""key1"": {Code1} } },
        { ""$match"": { ""key2"": {Code2} } }
    ]
}";
            var targetQueryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                [new QueryStringKey("Code1")] = new QueryStringValue("AA"),
                [new QueryStringKey("Code2")] = new QueryStringValue("BB")
            });

            var mainContainerName = Guid.NewGuid().ToString();
            var containerNames = new List<string>() { mainContainerName, Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var mockContainerDynamicSeparationRepository = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparationRepository.Setup(x => x.GetAllContainerNames(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>())).Returns(containerNames);
            UnityContainer.RegisterInstance(mockContainerDynamicSeparationRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Guid.NewGuid().ToString();
            perRequestDataContainer.SystemId = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance(perRequestDataContainer);

            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(1);

            var partitionKey = Guid.NewGuid().ToString();
            var metaConditions = new Dictionary<string, object>()
            {
                { "_Type", partitionKey },
                { "_Version", 1 }
            };

            // MockでconnectionClientを上書き
            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            mockJPDataHubMongoDB.SetupGet(x => x.CollectionName).Returns(mainContainerName);
            mockJPDataHubMongoDB.Setup(x => x.QueryDocument(It.IsAny<IEnumerable<BsonDocument>>()))
                .Callback<IEnumerable<BsonDocument>>((pipeline) =>
                {
                    // 「既定のクエリ」をBsonに変換（MongoDBの既定のクエリはBson形式も可とする）
                    var expectedQuery = defaultQuery.Replace("{COLLECTION_NAME}", $"\"{mainContainerName}\"");
                    targetQueryString?.Dic.ToList().ForEach(x => expectedQuery = expectedQuery.Replace($"{{{x.Key.Value}}}", $"\"{x.Value.Value}\""));
                    var expectedBsonArray = expectedQuery.ToDecimalizedBsonDocument()["Aggregate"].AsBsonArray;

                    // 引数チェック
                    var i = 0;
                    var metaMatch = $"{{ '$match': {{ '$and': [ {string.Join(",", metaConditions.Select(x => $"{{ '{x.Key}': {(x.Value is int ? $"{x.Value}" : $"'{x.Value}'")} }}"))} ] }} }}".ToDecimalizedBsonDocument();
                    var pipelineArray = pipeline.ToList();
                    SameAs(pipelineArray[i++], metaMatch).IsTrue();
                    SameAs(pipelineArray[i++], expectedBsonArray[0]).IsTrue();
                    SameAs(pipelineArray[i++], expectedBsonArray[1]).IsTrue();
                    foreach (var containerName in containerNames.Where(x => x != mainContainerName))
                    {
                        var unionWith = pipelineArray[i++]["$unionWith"];
                        unionWith["coll"].ToString().Is(containerName);
                        SameAs(unionWith["pipeline"].AsBsonArray, new BsonArray(new[] { metaMatch, expectedBsonArray[0], expectedBsonArray[1] })).IsTrue();
                    }
                })
                .Returns(targetData);
            SetDummyConnectionClient(mockJPDataHubMongoDB.Object, "endpoint=;database=;collection=");

            // ダミーのActionを作成
            var dummyAction = new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.ApiQuery, new ApiQuery(defaultQuery))
                .SetupProperty(x => x.QueryType, new QueryType(QueryTypes.NativeDbQuery))
                .SetupProperty(x => x.Query, targetQueryString)
                .SetupProperty(x => x.RepositoryKey, new RepositoryKey(partitionKey))
                .SetupProperty(x => x.IsContainerDynamicSeparation, new IsContainerDynamicSeparation(true))
                .SetupProperty(x => x.IsOverPartition, new IsOverPartition(true))
                .SetupProperty(x => x.IsVendor, new IsVendor(true))
                .Object;

            // 「既定のクエリ」を設定した場合のquery（Privateメソッド）の動作を確認
            var result = target.Query(ValueObjectUtil.Create<QueryParam>(dummyAction), out XResponseContinuation _).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(1);
            result.IsStructuralEqual(expectData);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubMongoDB, null);
        }

        #endregion

        #endregion

        #region Register

        [TestMethod]
        public void RegisterOnce_OK()
        {
            var target = CreateDummyRepository();

            var expectCounts = new int[] { 0, 1 }; // データが0, 1件のケース
            foreach (var expectCount in expectCounts)
            {
                var json = expectCount == 0 ? new JObject() : new JObject { ["id"] = 1 };
                var targetId = json["id"]?.ToString();

                // MockでconnectionClientを上書き
                var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
                mockJPDataHubMongoDB.Setup(x => x.UpsertDocument(
                    It.IsAny<JToken>()))
                    .Returns(targetId);
                SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

                // 対象メソッド実行
                var result = target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(new Mock<IDynamicApiAction>().Object, json));

                // データが1件以上の場合Mockの1件目と同じ期待値が返却されること
                result.Value.Is(targetId);

                // 呼ばれたか確認
                mockJPDataHubMongoDB.Verify(x => x.UpsertDocument(
                    It.IsAny<JToken>()),
                    Times.Exactly(1));
            }
        }

        [TestMethod]
        public void RegisterOnce_OK_IsOverrideId()
        {
            var target = CreateDummyRepository();

            var expectCounts = new int[] { 0, 1 }; // データが0, 1件のケース
            foreach (var expectCount in expectCounts)
            {
                var json = expectCount == 0 ? new JObject() : new JObject { ["id"] = 1 };
                var targetId = json["id"]?.ToString();

                // MockでconnectionClientを上書き
                var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
                mockJPDataHubMongoDB.Setup(x => x.UpsertDocument(
                    It.IsAny<JToken>()))
                    .Returns(targetId);
                SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

                // ダミーのActionを作成
                var dummyAction = new Mock<IDynamicApiAction>()
                    .SetupProperty(x => x.PartitionKey, new PartitionKey(""))
                    .SetupProperty(x => x.RepositoryKey, new RepositoryKey("/API/Public/UnitTest/RegisterOnce"))
                    .SetupProperty(x => x.IsVendor, new IsVendor(false))
                    .SetupProperty(x => x.VendorId, new VendorId(Guid.NewGuid().ToString()))
                    .SetupProperty(x => x.SystemId, new SystemId(Guid.NewGuid().ToString()))
                    .SetupProperty(x => x.OpenId, new OpenId(Guid.NewGuid().ToString()))
                    .SetupProperty(x => x.IsAutomaticId, new IsAutomaticId(false))
                    .SetupProperty(x => x.IsOverrideId, new IsOverrideId(true))
                    .SetupProperty(x => x.IsPerson, new IsPerson(false))
                    .Object;

                // 対象メソッド実行
                var result = target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(dummyAction, json));

                // データが1件以上の場合Mockの1件目と同じ期待値が返却されること
                result.Value.Is(targetId);

                // 呼ばれたか確認
                mockJPDataHubMongoDB.Verify(x => x.UpsertDocument(
                    It.IsAny<JToken>()),
                    Times.Exactly(1));
            }
        }

        #endregion

        #region Delete

        [TestMethod]
        public void DeleteOnce_OK()
        {
            var target = CreateDummyRepository();

            var expectCounts = new int[] { 0, 1 }; // データが0, 1件のケース
            foreach (var expectCount in expectCounts)
            {
                JToken json = expectCount == 0 ? new JObject() : new JObject { ["id"] = 1 };

                // MockでconnectionClientを上書き
                var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
                mockJPDataHubMongoDB.Setup(x => x.DeleteDocument(
                    It.IsAny<string>()))
                    .Returns(expectCount);
                SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

                // 対象メソッド実行
                target.DeleteOnce(ValueObjectUtil.Create<DeleteParam>(new Mock<IDynamicApiAction>().Object, json));

                // 呼ばれたか確認
                mockJPDataHubMongoDB.Verify(x => x.DeleteDocument(
                    It.IsAny<string>()),
                    Times.Exactly(1));
            }
        }

        [TestMethod]
        public void Delete_OK()
        {
            // 対象メソッド実行（Deleteは未実装）
            AssertEx.Catch<NotImplementedException>(() =>
            {
                var target = CreateDummyRepository();
                target.Delete(ValueObjectUtil.Create<DeleteParam>(new Mock<IDynamicApiAction>().Object));
            });
        }

        #endregion

        #region GetInternalAddWhereString

        [TestMethod]
        public void GetInternalAddWhereString_OK()
        {
            // 対象メソッド実行（GetInternalAddWhereStringは空を返却）
            var target = CreateDummyRepository();
            var result = target.GetInternalAddWhereString(ValueObjectUtil.Create<QueryParam>(new Mock<IDynamicApiAction>().Object), out _);
            result.Is(string.Empty);
        }

        #endregion

        #region GetConnection

        [TestMethod]
        public void GetConnection_コンテナ分離なし_ベンダー依存なし_個人依存なし() => GetConnectionTest(false, false, false);

        [TestMethod]
        public void GetConnection_コンテナ分離なし_ベンダー依存あり_個人依存なし() => GetConnectionTest(false, true, false);

        [TestMethod]
        public void GetConnection_コンテナ分離なし_ベンダー依存あり_個人依存なし_データ共有() => GetConnectionTest(false, true, false, true);

        [TestMethod]
        public void GetConnection_コンテナ分離なし_ベンダー依存なし_個人依存あり() => GetConnectionTest(false, false, true);

        [TestMethod]
        public void GetConnection_コンテナ分離なし_ベンダー依存なし_個人依存あり_個人共有() => GetConnectionTest(false, false, true, false, true);

        [TestMethod]
        public void GetConnection_コンテナ分離なし_ベンダー依存あり_個人依存あり() => GetConnectionTest(false, true, true);


        [TestMethod]
        public void GetConnection_コンテナ分離あり_ベンダー依存あり_個人依存なし() => GetConnectionTest(true, true, false);

        [TestMethod]
        public void GetConnection_コンテナ分離あり_ベンダー依存あり_個人依存なし_データ共有() => GetConnectionTest(true, true, false, true);

        [TestMethod]
        public void GetConnection_コンテナ分離あり_ベンダー依存なし_個人依存あり() => GetConnectionTest(true, false, true);

        [TestMethod]
        public void GetConnection_コンテナ分離あり_ベンダー依存なし_個人依存あり_個人共有() => GetConnectionTest(true, false, true, false, true);

        [TestMethod]
        public void GetConnection_コンテナ分離あり_ベンダー依存あり_個人依存あり() => GetConnectionTest(true, true, true);

        [TestMethod]
        public void GetConnection_コンテナ分離あり_新規コンテナ() => GetConnectionTest(true, true, true, false, false, true);


        private void GetConnectionTest(bool isContainerDynamicSeparation, bool isVendor, bool isPerson, bool isSharingWith = false, bool isSharingPerson = false, bool isNewContainer = false)
        {
            var baseConnectionString = "endpoint=mongodb://servername;database=dbname;collection=";
            var defaultCollectionName = Guid.NewGuid().ToString();
            var separatedCollectionName = Guid.NewGuid().ToString();
            var sharingWithCollectionName = Guid.NewGuid().ToString();
            var sharingPersonCollectionName = Guid.NewGuid().ToString();
            var repositoryConnectionString = $"{baseConnectionString}{defaultCollectionName}";

            var expectedCollectionName = (isContainerDynamicSeparation && (isVendor || isPerson))
                ? (isSharingWith ? sharingWithCollectionName : (isSharingPerson ? sharingPersonCollectionName : separatedCollectionName))
                : defaultCollectionName;
            var expectedConnectionString = $"{baseConnectionString}{expectedCollectionName}";

            var repositoryInfo = new RepositoryInfo("mng", new Dictionary<string, bool>() { { repositoryConnectionString, false } });

            var container = new PerRequestDataContainer();
            container.VendorId = Guid.NewGuid().ToString();
            container.SystemId = Guid.NewGuid().ToString();
            container.OpenId = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(container);

            var sharingFromVendorId = Guid.NewGuid().ToString();
            var sharingFromSystemId = Guid.NewGuid().ToString();
            var sharingFromOpenId = Guid.NewGuid().ToString();

            var isRegistered = isNewContainer;
            var mock = new Mock<IContainerDynamicSeparationRepository>();
            mock.Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.Is<VendorId>(v => v.Value == container.VendorId), It.Is<SystemId>(s => s.Value == container.SystemId), It.Is<OpenId>(o => o.Value == Guid.Empty.ToString()), out isRegistered)).Returns(separatedCollectionName);
            mock.Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.Is<VendorId>(v => v.Value == Guid.Empty.ToString()), It.Is<SystemId>(s => s.Value == Guid.Empty.ToString()), It.Is<OpenId>(o => o.Value == container.OpenId), out isRegistered)).Returns(separatedCollectionName);
            mock.Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.Is<VendorId>(v => v.Value == container.VendorId), It.Is<SystemId>(s => s.Value == container.SystemId), It.Is<OpenId>(o => o.Value == container.OpenId), out isRegistered)).Returns(separatedCollectionName);
            mock.Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.Is<VendorId>(v => v.Value == sharingFromVendorId), It.Is<SystemId>(s => s.Value == sharingFromSystemId), It.Is<OpenId>(o => o.Value == Guid.Empty.ToString()), out isRegistered)).Returns(sharingWithCollectionName);
            mock.Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.Is<VendorId>(v => v.Value == Guid.Empty.ToString()), It.Is<SystemId>(s => s.Value == Guid.Empty.ToString()), It.Is<OpenId>(o => o.Value == sharingFromOpenId), out isRegistered)).Returns(sharingPersonCollectionName);
            UnityContainer.RegisterInstance<IContainerDynamicSeparationRepository>(mock.Object);

            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            mockJPDataHubMongoDB.SetupProperty(x => x.ConnectionString);
            UnityContainer.RegisterInstance<IJPDataHubMongoDb>(mockJPDataHubMongoDB.Object);

            var target = CreateDummyRepository();
            var result = target.GetType().InvokeMember("GetConnection", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, target, new object[]
            {
                repositoryInfo,
                new IsContainerDynamicSeparation(isContainerDynamicSeparation),
                new ControllerId(Guid.NewGuid().ToString()),
                new IsVendor(isVendor),
                new IsPerson(isPerson),
                isSharingWith ? new VendorId(sharingFromVendorId) : null,
                isSharingWith ? new SystemId(sharingFromSystemId) : null,
                isSharingPerson ? new OpenId(sharingFromOpenId) : null
            }) as IJPDataHubMongoDb;
            result.ConnectionString.Is(expectedConnectionString);

            if (!isContainerDynamicSeparation)
            {
                mock.Verify(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>(), out isRegistered), Times.Never);
            }
            else if (isSharingWith)
            {
                mock.Verify(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.Is<VendorId>(v => v.Value == sharingFromVendorId), It.Is<SystemId>(s => s.Value == sharingFromSystemId), It.Is<OpenId>(o => o.Value == Guid.Empty.ToString()), out isRegistered), Times.Once);
            }
            else if (isSharingPerson)
            {
                mock.Verify(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.Is<VendorId>(v => v.Value == Guid.Empty.ToString()), It.Is<SystemId>(s => s.Value == Guid.Empty.ToString()), It.Is<OpenId>(o => o.Value == sharingFromOpenId), out isRegistered), Times.Once);
            }
            else if (isVendor && !isPerson)
            {
                mock.Verify(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.Is<VendorId>(v => v.Value == container.VendorId), It.Is<SystemId>(s => s.Value == container.SystemId), It.Is<OpenId>(o => o.Value == Guid.Empty.ToString()), out isRegistered), Times.Once);
            }
            else if (!isVendor && isPerson)
            {
                mock.Verify(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.Is<VendorId>(v => v.Value == Guid.Empty.ToString()), It.Is<SystemId>(s => s.Value == Guid.Empty.ToString()), It.Is<OpenId>(o => o.Value == container.OpenId), out isRegistered), Times.Once);
            }
            else
            {
                mock.Verify(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.Is<VendorId>(v => v.Value == container.VendorId), It.Is<SystemId>(s => s.Value == container.SystemId), It.Is<OpenId>(o => o.Value == container.OpenId), out isRegistered), Times.Once);
            }

            var times = isNewContainer ? Times.Once() : Times.Never();
            mockJPDataHubMongoDB.Verify(x => x.CreateWildcardIndex(), times);
        }

        [TestMethod]
        public void GetConnection_NoCollectionConnectionString()
        {
            var noCollectionConnectionString = "endpoint=mongodb://servername;database=dbname;collection=";
            var expectedCollectionName = Guid.NewGuid().ToString();
            var expectedConnectionString = $"{noCollectionConnectionString}{expectedCollectionName}";

            var repositoryInfo = new RepositoryInfo("mng", new Dictionary<string, bool>() { { noCollectionConnectionString, false } });

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Guid.NewGuid().ToString();
            perRequestDataContainer.SystemId = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance(perRequestDataContainer);

            var isRegistered = false;
            var mock = new Mock<IContainerDynamicSeparationRepository>();
            mock.Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>(), out isRegistered)).Returns(expectedCollectionName);
            UnityContainer.RegisterInstance<IContainerDynamicSeparationRepository>(mock.Object);

            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            mockJPDataHubMongoDB.SetupProperty(x => x.ConnectionString);
            UnityContainer.RegisterInstance<IJPDataHubMongoDb>(mockJPDataHubMongoDB.Object);

            var target = CreateDummyRepository();
            var result = target.GetType().InvokeMember("GetConnection", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, target, new object[]
            {
                repositoryInfo,
                new IsContainerDynamicSeparation(true),
                new ControllerId(Guid.NewGuid().ToString()),
                new IsVendor(true),
                new IsPerson(false),
                null,
                null,
                null
            }) as IJPDataHubMongoDb;
            result.ConnectionString.Is(expectedConnectionString);

            mockJPDataHubMongoDB.Verify(x => x.CreateWildcardIndex(), Times.Never);
        }

        #endregion


        /// <summary>
        /// QueryOnceのテストを実行する
        /// </summary>
        /// <param name="expectCount"></param>
        /// <param name="xRequestContinuation"></param>
        /// <param name="nativeQuery"></param>
        /// <param name="apiQuery"></param>
        private void ExecuteQueryOnce(int expectCount, XRequestContinuation xRequestContinuation = null, NativeQuery nativeQuery = null, ApiQuery apiQuery = null)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(expectCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            SetUpQueryDocument(mockJPDataHubMongoDB, targetData, xRequestContinuation);
            SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(xRequestContinuation, apiQuery);

            // 対象メソッド実行
            var result = target.QueryOnce(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery));
            if (targetData.Count() == 0)
            {
                // データが0件の場合結果がnullで返却されること
                result.IsNull();
            }
            else
            {
                // データが1件以上の場合Mockの1件目と同じ期待値が返却されること
                result.IsStructuralEqual(expectData.FirstOrDefault());
            }

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubMongoDB, xRequestContinuation);
        }

        /// <summary>
        /// QueryEnumerableのテストを実行する
        /// </summary>
        /// <param name="expectCount"></param>
        /// <param name="xRequestContinuation"></param>
        /// <param name="nativeQuery"></param>
        /// <param name="apiQuery"></param>
        private void ExecuteQueryEnumerable(int expectCount, XRequestContinuation xRequestContinuation = null, NativeQuery nativeQuery = null, ApiQuery apiQuery = null)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(expectCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            SetUpQueryDocument(mockJPDataHubMongoDB, targetData, xRequestContinuation);
            SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(xRequestContinuation, apiQuery);

            // 対象メソッド実行
            var result = target.QueryEnumerable(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery)).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(expectCount);
            result.IsStructuralEqual(expectData);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubMongoDB, xRequestContinuation);
        }

        /// <summary>
        /// Queryのテストを実行する
        /// </summary>
        /// <param name="expectCount"></param>
        /// <param name="xRequestContinuation"></param>
        /// <param name="nativeQuery"></param>
        /// <param name="apiQuery"></param>
        private void ExecuteQuery(int expectCount, XRequestContinuation xRequestContinuation = null, NativeQuery nativeQuery = null, ApiQuery apiQuery = null)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(expectCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            SetUpQueryDocument(mockJPDataHubMongoDB, targetData, xRequestContinuation);
            SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(xRequestContinuation, apiQuery);

            // 対象メソッド実行
            var result = target.Query(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery), out XResponseContinuation xResponseContinuation).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(expectCount);
            result.IsStructuralEqual(expectData);
            xResponseContinuation?.ContinuationString.Is(xRequestContinuation?.ContinuationString);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubMongoDB, xRequestContinuation);
        }

        /// <summary>
        /// Queryのテストを実行する（既定のクエリ）
        /// </summary>
        /// <param name="defaultQuery"></param>
        /// <param name="targetQueryString"></param>
        private void ExcuteQueryAsDefaultQuery(string defaultQuery, QueryStringVO targetQueryString = null)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(1);

            var partitionKey = Guid.NewGuid().ToString();
            var metaConditions = new Dictionary<string, object>()
            {
                { "_Type", partitionKey },
                { "_Version", 1 }
            };

            // MockでconnectionClientを上書き
            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            mockJPDataHubMongoDB.Setup(x => x.QueryDocument(
                It.IsAny<IEnumerable<BsonDocument>>()))
                .Callback<IEnumerable<BsonDocument>>((pipeline) =>
                {
                    // 「既定のクエリ」をBsonに変換（MongoDBの既定のクエリはBson形式も可とする）
                    var expectedQuery = defaultQuery;
                    targetQueryString?.Dic.ToList().ForEach(x => expectedQuery = expectedQuery.Replace($"{{{x.Key.Value}}}", $"\"{x.Value.Value}\""));
                    var expectedBson = expectedQuery.ToDecimalizedBsonDocument();

                    // 引数チェック
                    var i = 0;
                    var pipelineArray = pipeline.ToList();
                    VerifyQueryDocumentWhereConditions(expectedBson["Where"], pipelineArray[i++]["$match"].ToString(), metaConditions);
                    if (expectedBson["OrderBy"] != BsonNull.Value)
                    {
                        VerifyQueryDocumentBsonProperty(expectedBson["OrderBy"], pipelineArray[i++]["$sort"].ToString());
                    }
                    if (expectedBson["Select"] != BsonNull.Value)
                    {
                        VerifyQueryDocumentBsonProperty(expectedBson["Select"], pipelineArray[i++]["$project"].ToString());
                    }
                    if (expectedBson["Top"] != BsonNull.Value)
                    {
                        VerifyQueryDocumentBsonProperty(expectedBson["Top"], resultInt: pipelineArray[i++]["$limit"].ToInt32());
                    }
                })
                .Returns(targetData);
            SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

            // ダミーのActionを作成
            var dummyAction = new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.ApiQuery, new ApiQuery(defaultQuery))
                .SetupProperty(x => x.QueryType, new QueryType(QueryTypes.NativeDbQuery))
                .SetupProperty(x => x.Query, targetQueryString)
                .SetupProperty(x => x.RepositoryKey, new RepositoryKey(partitionKey))
                .Object;

            // 「既定のクエリ」を設定した場合のquery（Privateメソッド）の動作を確認
            var result = target.Query(ValueObjectUtil.Create<QueryParam>(dummyAction), out XResponseContinuation _).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(1);
            result.IsStructuralEqual(expectData);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubMongoDB, null);
        }

        /// <summary>
        /// Queryのテストを実行する（既定のクエリAggregate）
        /// </summary>
        private void ExcuteQueryWithAggregate(string defaultQuery, QueryStringVO targetQueryString = null)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(1);

            var partitionKey = Guid.NewGuid().ToString();
            var metaConditions = new Dictionary<string, object>()
            {
                { "_Type", partitionKey },
                { "_Version", 1 }
            };

            // MockでconnectionClientを上書き
            var containerName = Guid.NewGuid().ToString();
            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            mockJPDataHubMongoDB.SetupGet(x => x.CollectionName).Returns(containerName);
            mockJPDataHubMongoDB.Setup(x => x.QueryDocument(
                It.IsAny<IEnumerable<BsonDocument>>()))
                .Callback<IEnumerable<BsonDocument>>((pipeline) =>
                {
                    // 「既定のクエリ」をBsonに変換（MongoDBの既定のクエリはBson形式も可とする）
                    var expectedQuery = defaultQuery.Replace("{COLLECTION_NAME}", $"\"{containerName}\"");
                    targetQueryString?.Dic.ToList().ForEach(x => expectedQuery = expectedQuery.Replace($"{{{x.Key.Value}}}", $"\"{x.Value.Value}\""));
                    var expectedBson = expectedQuery.ToDecimalizedBsonDocument();

                    // 引数チェック
                    var pipelineStr = PipelineDefinition<BsonDocument, BsonDocument>.Create(pipeline).ToString();
                    VerifyQueryDocumentAggregationPipeline(expectedBson["Aggregate"], pipelineStr, metaConditions);
                })
                .Returns(targetData);
            SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

            // ダミーのActionを作成
            var dummyAction = new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.ApiQuery, new ApiQuery(defaultQuery))
                .SetupProperty(x => x.QueryType, new QueryType(QueryTypes.NativeDbQuery))
                .SetupProperty(x => x.Query, targetQueryString)
                .SetupProperty(x => x.RepositoryKey, new RepositoryKey(partitionKey))
                .Object;

            // 「既定のクエリ」を設定した場合のquery（Privateメソッド）の動作を確認
            var result = target.Query(ValueObjectUtil.Create<QueryParam>(dummyAction), out XResponseContinuation _).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(1);
            result.IsStructuralEqual(expectData);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubMongoDB, null);
        }


        /// <summary>
        /// QueryOnceのテストを実行する(XRequestContinuationNeedsTopCount=true)
        /// </summary>
        private void ExecuteQueryOnceWithNeedsTopCount(ApiQuery apiQuery, NativeQuery nativeQuery, bool isContinuationExpected)
        {
            UnityContainer.RegisterInstance<bool>("XRequestContinuationNeedsTopCount", true);

            var target = CreateDummyRepository();
            var expectedCount = 1;
            var (targetData, expectData) = CreateQueryTestData(expectedCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            SetUpQueryDocument(mockJPDataHubMongoDB, targetData, isContinuationExpected ? xRequestContinuation : null);
            SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(xRequestContinuation, apiQuery);

            // 対象メソッド実行
            var result = target.QueryOnce(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery));
            if (targetData.Count() == 0)
            {
                // データが0件の場合結果がnullで返却されること
                result.IsNull();
            }
            else
            {
                // データが1件以上の場合Mockの1件目と同じ期待値が返却されること
                result.IsStructuralEqual(expectData.FirstOrDefault());
            }

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubMongoDB, isContinuationExpected);
        }

        /// <summary>
        /// QueryEnumerableのテストを実行する(XRequestContinuationNeedsTopCount=true)
        /// </summary>
        private void ExecuteQueryEnumerableWithNeedsTopCount(ApiQuery apiQuery, NativeQuery nativeQuery, bool isContinuationExpected)
        {
            UnityContainer.RegisterInstance<bool>("XRequestContinuationNeedsTopCount", true);

            var target = CreateDummyRepository();
            var expectedCount = 1;
            var (targetData, expectData) = CreateQueryTestData(expectedCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            SetUpQueryDocument(mockJPDataHubMongoDB, targetData, isContinuationExpected ? xRequestContinuation : null);
            SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(xRequestContinuation, apiQuery);

            // 対象メソッド実行
            var result = target.QueryEnumerable(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery)).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(expectedCount);
            result.IsStructuralEqual(expectData);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubMongoDB, isContinuationExpected);
        }

        /// <summary>
        /// Queryのテストを実行する(XRequestContinuationNeedsTopCount=true)
        /// </summary>
        private void ExecuteQueryWithNeedsTopCount(ApiQuery apiQuery, NativeQuery nativeQuery, bool isContinuationExpected)
        {
            UnityContainer.RegisterInstance<bool>("XRequestContinuationNeedsTopCount", true);

            var target = CreateDummyRepository();
            var expectedCount = 1;
            var (targetData, expectData) = CreateQueryTestData(expectedCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubMongoDB = new Mock<IJPDataHubMongoDb>();
            SetUpQueryDocument(mockJPDataHubMongoDB, targetData, isContinuationExpected ? xRequestContinuation : null);
            SetDummyConnectionClient(mockJPDataHubMongoDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(xRequestContinuation, apiQuery);

            // 対象メソッド実行
            var result = target.Query(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery), out var xResponseContinuation).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(expectedCount);
            result.IsStructuralEqual(expectData);
            xResponseContinuation?.ContinuationString.Is(isContinuationExpected ? xRequestContinuation.ContinuationString : null);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubMongoDB, isContinuationExpected);
        }


        /// <summary>
        /// ダミーのRepositoryを作成する
        /// </summary>
        /// <returns></returns>
        private NewMongoDbDataStoreRepository CreateDummyRepository()
        {
            var target = UnityContainer.Resolve<NewMongoDbDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo("mng", new Dictionary<string, bool>() { { "connectionstring", false } });

            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(
                It.IsAny<RepositoryKey>(),
                It.IsAny<XVersion>()))
                .Returns(new ResourceVersion(1));
            mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            string message;
            var mockValidator = new Mock<MongoDbQuerySyntaxValidatior>();
            mockValidator.Setup(x => x.ValidatePipelineSyntax(It.IsAny<BsonArray>(), It.IsAny<string>(), out message)).Returns(true);
            UnityContainer.RegisterInstance<IQuerySyntaxValidator>("mng", mockValidator.Object);

            return target;
        }

        /// <summary>
        /// テスト用データを作成する（Query系用）
        /// </summary>
        /// <returns></returns>
        private (IEnumerable<JToken>, IEnumerable<JsonDocument>) CreateQueryTestData(int expectCount)
        {
            var targetData = Enumerable.Range(1, expectCount)
                .Select(x => (JToken)new JObject { ["id"] = x })
                .ToList();

            var expectData = Enumerable.Range(1, expectCount)
                .Select(x => new JsonDocument(new JObject { ["id"] = x }))
                .ToList();
            return (targetData, expectData);
        }

        /// <summary>
        /// ダミーのActionを作成する（Query系用）
        /// </summary>
        /// <returns></returns>
        private IDynamicApiAction CreateDummyQueryAction(XRequestContinuation xRequestContinuation = null, ApiQuery apiQuery = null, ActionTypeVO actionType = null, IsDocumentHistory isDocumentHistory = null)
        {
            return new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.ActionType, actionType ?? new ActionTypeVO(ActionType.Query))
                .SetupProperty(x => x.ApiQuery, apiQuery ?? new ApiQuery(""))
                .SetupProperty(x => x.QueryType, new QueryType(apiQuery == null ? QueryTypes.NativeDbQuery : QueryTypes.ODataQuery))
                .SetupProperty(x => x.XRequestContinuation, xRequestContinuation)
                .SetupProperty(x => x.IsDocumentHistory, isDocumentHistory)
                .Object;
        }

        /// <summary>
        /// IJPDataHubMongoDbのMockをセットアップする
        /// </summary>
        /// <param name="mockJPDataHubMongoDB"></param>
        /// <param name="targetData"></param>
        /// <param name="xRequestContinuation"></param>
        private void SetUpQueryDocument(Mock<IJPDataHubMongoDb> mockJPDataHubMongoDB, IEnumerable<JToken> targetData, XRequestContinuation xRequestContinuation)
        {
            if (xRequestContinuation == null)
            {
                mockJPDataHubMongoDB.Setup(x => x.QueryDocument(
                    It.IsAny<IEnumerable<BsonDocument>>()))
                    .Returns(targetData);
            }
            else
            {
                mockJPDataHubMongoDB.Setup(x => x.QueryDocumentContinuation(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<BsonDocument>>()))
                    .Returns(targetData.Select(x => new Tuple<JToken, string>(x, xRequestContinuation.ContinuationString))); // xRequestContinuationをそのまま返却
            }
        }

        /// <summary>
        /// IJPDataHubMongoDbのMockのメソッドが呼ばれたか確認する
        /// </summary>
        /// <param name="mockJPDataHubMongoDB"></param>
        /// <param name="xRequestContinuation"></param>
        private void VerifyQueryDocument(Mock<IJPDataHubMongoDb> mockJPDataHubMongoDB, XRequestContinuation xRequestContinuation)
        {
            VerifyQueryDocument(mockJPDataHubMongoDB, (xRequestContinuation != null));
        }
        private void VerifyQueryDocument(Mock<IJPDataHubMongoDb> mockJPDataHubMongoDB, bool isContinuationExpected)
        {
            if (isContinuationExpected)
            {
                mockJPDataHubMongoDB.Verify(x => x.QueryDocumentContinuation(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<IEnumerable<BsonDocument>>()),
                    Times.Exactly(1));
            }
            else
            {
                mockJPDataHubMongoDB.Verify(x => x.QueryDocument(
                    It.IsAny<IEnumerable<BsonDocument>>()),
                    Times.Exactly(1));
            }
        }

        /// <summary>
        /// connectionClient（静的フィールド）を上書きする
        /// </summary>
        /// <param name="jPDataHubMongoDB"></param>
        private void SetDummyConnectionClient(IJPDataHubMongoDb jPDataHubMongoDB, string connectionString = "connectionstring")
        {
            var field = typeof(NewMongoDbDataStoreRepository).GetField("s_connectionClient", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, new ConcurrentDictionary<string, IJPDataHubMongoDb>(new Dictionary<string, IJPDataHubMongoDb>() { { connectionString, jPDataHubMongoDB } }));
        }

        /// <summary>
        /// QueryDocumentの引数が想定通りか確認する
        /// </summary>
        private void VerifyQueryDocumentBsonProperty(BsonValue expectedBsonValue, string resultString = "DUMMY", int? resultInt = 9999)
        {
            if (resultString == null || resultInt == null)
            {
                // nullが指定された場合Bsonの型がBsonNullであることを確認
                expectedBsonValue.IsBsonNull.IsTrue();
            }
            else if (resultString != "DUMMY")
            {
                // stringが指定された場合Bsonに変換して比較
                resultString.ToDecimalizedBsonDocument().Is(expectedBsonValue);
            }
            else if (resultInt != 9999)
            {
                // intが指定された場合値を直接比較
                resultInt.Is(Convert.ToInt32(expectedBsonValue.AsDecimal128));
            }
            else
            {
                // 上記以外は想定外
                Assert.Fail();
            }
        }

        /// <summary>
        /// QueryDocumentの引数が想定通りか確認する
        /// </summary>
        private void VerifyQueryDocumentWhereConditions(BsonValue expectedBsonValue, string resultString, Dictionary<string, object> mergedConditions)
        {
            var resultBson = resultString.ToDecimalizedBsonDocument();
            var resultConditions = resultBson.First();
            resultConditions.Name.Is("$and");

            // マージされた条件を全て含むこと
            mergedConditions.All(x => resultConditions.Value.AsBsonArray.Any(y =>
            {
                if (y.AsBsonDocument.Contains(x.Key))
                {
                    var resultElement = y.AsBsonDocument.GetElement(x.Key);
                    if (resultElement.Value is BsonString)
                    {
                        return resultElement.Value.AsString == (string)x.Value;
                    }
                    else if (resultElement.Value is BsonDecimal128)
                    {
                        return Convert.ToInt32(resultElement.Value.AsDecimal128) == (int)x.Value;
                    }
                }
                return false;
            })).IsTrue();

            // 想定条件を含むこと
            if (!(expectedBsonValue is BsonNull))
            {
                resultConditions.Value.AsBsonArray.Any(x => x == expectedBsonValue).IsTrue();
            }

            resultConditions.Value.AsBsonArray.Count().Is(mergedConditions.Count() + (expectedBsonValue is BsonNull ? 0 : 1));
        }

        /// <summary>
        /// QueryDocumentの引数が想定通りか確認する
        /// </summary>
        private void VerifyQueryDocumentAggregationPipeline(BsonValue expectedBsonValue, string resultString, Dictionary<string, object> mergedConditions)
        {
            var resultPipeline = resultString.ToBsonArray();
            var expectedPipeline = expectedBsonValue.AsBsonArray;

            ComparePipeline(expectedPipeline, resultPipeline, mergedConditions);
        }

        private void ComparePipeline(BsonArray expectedPipeline, BsonArray resultPipeline, Dictionary<string, object> mergedConditions)
        {
            var resultBaseIndex = 0;

            // 全体件数及びステージ挿入
            var resultFirstStage = resultPipeline.FirstOrDefault().AsBsonDocument;
            if (resultFirstStage.Contains("$geoNear"))
            {
                resultPipeline.Count.Is(expectedPipeline.Count);
            }
            else
            {
                resultPipeline.Count.Is(expectedPipeline.Count + 1);
                resultFirstStage.Contains("$match").IsTrue();
                resultFirstStage["$match"].AsBsonDocument.FirstOrDefault().Name.Is("$and");
                var resultQuery = resultFirstStage["$match"].AsBsonDocument["$and"].AsBsonArray;

                resultQuery.Count.Is(mergedConditions.Count);

                // マージされた条件を全て含むこと
                mergedConditions.All(x => resultQuery.Any(y =>
                {
                    if (y.AsBsonDocument.Contains(x.Key))
                    {
                        var resultElement = y.AsBsonDocument.GetElement(x.Key);
                        if (resultElement.Value is BsonString)
                        {
                            return resultElement.Value.AsString == (string)x.Value;
                        }
                        else if (resultElement.Value is BsonDecimal128)
                        {
                            return Convert.ToInt32(resultElement.Value.AsDecimal) == (int)x.Value;
                        }
                    }
                    return false;
                })).IsTrue();

                resultBaseIndex++;
            }

            // 後続ステージ
            for (var i = 0; i < expectedPipeline.Count; i++)
            {
                var resultStage = resultPipeline[i + resultBaseIndex].AsBsonDocument;
                var expectedStage = expectedPipeline[i].AsBsonDocument;

                if (resultStage.Contains("$geoNear"))
                {
                    var resultQuery = resultStage["$geoNear"]["query"].AsBsonDocument["$and"].AsBsonArray;
                    var expectedQueryExists = expectedStage["$geoNear"].AsBsonDocument.Contains("query");

                    // マージされた条件を全て含むこと
                    mergedConditions.All(x => resultQuery.Any(y =>
                    {
                        if (y.AsBsonDocument.Contains(x.Key))
                        {
                            var resultElement = y.AsBsonDocument.GetElement(x.Key);
                            if (resultElement.Value is BsonString)
                            {
                                return resultElement.Value.AsString == (string)x.Value;
                            }
                            else if (resultElement.Value is BsonDecimal128)
                            {
                                return Convert.ToInt32(resultElement.Value.AsDecimal) == (int)x.Value;
                            }
                        }
                        return false;
                    })).IsTrue();

                    if (!expectedQueryExists)
                    {
                        resultQuery.Count.Is(mergedConditions.Count);
                    }
                    else
                    {
                        var expectedQuery = expectedStage["$geoNear"]["query"].AsBsonDocument;

                        resultQuery.Count.Is(mergedConditions.Count + expectedQuery.Elements.Count());
                        expectedQuery.Elements.All(x => resultQuery.Any(y =>
                        {
                            if (y.AsBsonDocument.Contains(x.Name))
                            {
                                var resultElement = y.AsBsonDocument.GetElement(x.Name);
                                if (resultElement.Value is BsonString)
                                {
                                    return resultElement.Value.AsString == (string)x.Value;
                                }
                                else if (resultElement.Value is BsonInt32)
                                {
                                    return resultElement.Value.AsInt32 == (int)x.Value;
                                }
                            }
                            return false;
                        })).IsTrue();
                    }
                }
                else if (resultStage.Contains("$unionWith"))
                {
                    resultStage["$unionWith"]["coll"].Is(expectedStage["$unionWith"]["coll"]);

                    var resultChildPipeline = resultStage["$unionWith"]["pipeline"].AsBsonArray;
                    var expectedChildPipeline = expectedStage["$unionWith"]["pipeline"].AsBsonArray;

                    ComparePipeline(expectedChildPipeline, resultChildPipeline, mergedConditions);
                }
                else
                {
                    resultStage.Is(expectedStage);
                }
            }
        }


        public static bool SameAs(BsonDocument source, BsonDocument other, params string[] ignoreFields)
        {
            var elements = source.Elements.Where(x => !ignoreFields.Contains(x.Name)).ToArray();
            if (elements.Length == 0 && other.Elements.Where(x => !ignoreFields.Contains(x.Name)).Count() > 0) return false;
            foreach (BsonElement element in source.Elements)
            {
                if (ignoreFields.Contains(element.Name)) continue;
                if (!other.Names.Contains(element.Name)) return false;
                BsonValue value = element.Value;
                BsonValue otherValue = other[element.Name];
                if (!SameAs(value, otherValue)) return false;
            }
            return true;
        }

        public static bool SameAs(BsonValue value, BsonValue otherValue)
        {
            if (value.IsBsonDocument)
            {
                if (!otherValue.IsBsonDocument) return false;
                if (!SameAs(value.AsBsonDocument, otherValue.AsBsonDocument)) return false;
            }
            else if (value.IsBsonArray)
            {
                if (!otherValue.IsBsonArray) return false;
                if (value.AsBsonArray.Count != otherValue.AsBsonArray.Count) return false;
                var array = value.AsBsonArray.OrderBy(x => x).ToArray();
                var otherArray = otherValue.AsBsonArray.OrderBy(x => x).ToArray();
                return !array.Where((t, i) => !SameAs(t, otherArray[i])).Any();
            }
            else return value.Equals(otherValue);
            return true;
        }
    }
}