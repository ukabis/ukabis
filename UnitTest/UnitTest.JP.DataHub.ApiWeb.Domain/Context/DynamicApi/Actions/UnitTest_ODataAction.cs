using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language;
using Moq.Language.Flow;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    public static class MoqExtensions
    {
        public delegate void OutAction<TOut>(out TOut outVal);
        public delegate void OutAction<in T1, TOut>(T1 arg1, out TOut outVal);

        public static IReturnsThrows<TMock, TReturn> OutCallback<TMock, TReturn, TOut>(this ICallback<TMock, TReturn> mock, OutAction<TOut> action)
            where TMock : class
        {
            return OutCallbackInternal(mock, action);
        }

        public static IReturnsThrows<TMock, TReturn> OutCallback<TMock, TReturn, T1, TOut>(this ICallback<TMock, TReturn> mock, OutAction<T1, TOut> action)
            where TMock : class
        {
            return OutCallbackInternal(mock, action);
        }

        private static IReturnsThrows<TMock, TReturn> OutCallbackInternal<TMock, TReturn>(ICallback<TMock, TReturn> mock, object action)
            where TMock : class
        {
            mock.GetType()
                .Assembly.GetType("Moq.MethodCall")
                .InvokeMember("SetCallbackWithArguments", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, mock,
                    new[] { action });
            return mock as IReturnsThrows<TMock, TReturn>;
        }
    }

    [TestClass]
    public class UnitTest_ODataAction : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", true);

            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
            UnityContainer.RegisterType<IHttpContextAccessor, HttpContextAccessor>();
        }

        private Mock<INewDynamicApiDataStoreRepository> CreateINewDynamicApiDataStoreRepositoryMock(
            List<JsonDocument> jsonDocuments, QueryParam queryParam,
            XResponseContinuation responseContinuation, string returnresponseContinuation = null,
            Exception exception = null)
        {
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            if (exception == null)
            {
                mockRepository.Setup(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation))
                    .Callback((QueryParam para, out XResponseContinuation continuation) =>
                    {
                        continuation = null;
                        if (returnresponseContinuation != null)
                        {
                            continuation = new XResponseContinuation(returnresponseContinuation);
                        }
                        if (queryParam != null)
                        {
                            para.IsNotStructuralEqual(queryParam);
                        }
                    })
                    .Returns(jsonDocuments);
            }
            else
            {
                mockRepository.Setup(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation))
                    .Callback((QueryParam para, out XResponseContinuation continuation) =>
                    {
                        continuation = null;
                        if (responseContinuation != null)
                        {
                            continuation = new XResponseContinuation(returnresponseContinuation);
                        }

                        if (queryParam != null)
                        {
                            para.IsNotStructuralEqual(queryParam);
                        }
                    })
                    .Throws(exception);
            }
            return mockRepository;
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_データ有()
        {
            var action = CreateODataAction();
            var exceptionResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };
            XResponseContinuation responseContinuation = null;
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(exceptionResult[0].Value.ToString());
            result.Headers.Contains("X-ResponseContinuation").Is(false);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_データ無()
        {
            var action = CreateODataAction();

            XResponseContinuation responseContinuation = null;
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), responseContinuation);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);
            result.Headers.Contains("X-ResponseContinuation").Is(false);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.I10401.ToString(), actualMessage["error_code"]);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_NotImplementedException()
        {

            var action = CreateODataAction();

            XResponseContinuation responseContinuation = null;
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), responseContinuation, null, new NotImplementedException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            AssertEx.Catch<NotImplementedException>(() => action.ExecuteAction());

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));

        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目データ有()
        {
            var action = CreateODataAction();
            var exceptionResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };

            XResponseContinuation responseContinuation1 = null;
            XResponseContinuation responseContinuation2 = null;

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation1);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), responseContinuation2);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(exceptionResult[0].Value.ToString());
            result.Headers.Contains("X-ResponseContinuation").Is(false);

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation1), Times.Exactly(1));

            // 2つ目は無視
            mockRepository2.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation2), Times.Exactly(0));

        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目データ無_2つ目データ有()
        {

            var action = CreateODataAction();
            var exceptionResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };

            XResponseContinuation responseContinuation1 = null;
            XResponseContinuation responseContinuation2 = null;

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), responseContinuation1);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation2);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(exceptionResult[0].Value.ToString());
            result.Headers.Contains("X-ResponseContinuation").Is(false);

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation1), Times.Exactly(1));
            mockRepository2.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation2), Times.Exactly(1));

        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目データ無_2つ目データ無()
        {

            var action = CreateODataAction();

            XResponseContinuation responseContinuation1 = null;
            XResponseContinuation responseContinuation2 = null;
            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), responseContinuation1);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), responseContinuation2);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);
            result.Headers.Contains("X-ResponseContinuation").Is(false);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.I10401.ToString(), actualMessage["error_code"]);

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation1), Times.Exactly(1));
            mockRepository2.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation2), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目NotImplementedException_2つ目データ有()
        {

            var action = CreateODataAction();

            var exceptionResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };

            XResponseContinuation responseContinuation1 = null;
            XResponseContinuation responseContinuation2 = null;

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), responseContinuation1, null, new NotImplementedException());
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation2);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(exceptionResult[0].Value.ToString());
            result.Headers.Contains("X-ResponseContinuation").Is(false);

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation1), Times.Exactly(1));
            mockRepository2.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation2), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目NotImplementedException_2つ目NotImplementedException()
        {

            var action = CreateODataAction();

            var exceptionResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };

            XResponseContinuation responseContinuation1 = null;
            XResponseContinuation responseContinuation2 = null;

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), responseContinuation1, null, new NotImplementedException());
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), responseContinuation2, null, new NotImplementedException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });


            AssertEx.Catch<AggregateException>(() => action.ExecuteAction());
            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation1), Times.Exactly(1));
            mockRepository2.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation2), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_ResponseContinuation有()
        {
            var action = CreateODataAction();

            var expectResponseContinuation = Guid.NewGuid().ToString();
            var exceptionResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };
            XResponseContinuation responseContinuation = null;
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation, expectResponseContinuation);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(exceptionResult[0].Value.ToString());
            result.Headers.GetValues("X-ResponseContinuation").Count().Is(1);
            result.Headers.GetValues("X-ResponseContinuation").FirstOrDefault().Is(expectResponseContinuation);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));

        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_2つ目のデータ取得_ResponseContinuation有()
        {
            var action = CreateODataAction();

            var expectResponseContinuation = Guid.NewGuid().ToString();
            var notExpectResponseContinuation = Guid.NewGuid().ToString();

            var exceptionResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };

            XResponseContinuation responseContinuation1 = null;
            XResponseContinuation responseContinuation2 = null;
            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), responseContinuation1, notExpectResponseContinuation);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation2, expectResponseContinuation);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });


            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(exceptionResult[0].Value.ToString());
            result.Headers.GetValues("X-ResponseContinuation").Count().Is(1);
            result.Headers.GetValues("X-ResponseContinuation").FirstOrDefault().Is(expectResponseContinuation);

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation1), Times.Exactly(1));
            mockRepository2.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation2), Times.Exactly(1));

        }

        private HttpResponseMessage ExecuteAction_ForHistoryTest_Common(
            List<JsonDocument> getDataByODataResult,
            string exceptResult,
            Func<ODataAction, ODataAction> setODataActionFunc = null,
            XResponseContinuation parmaResponseContinuation = null,
            XResponseContinuation expectResponseContinuation = null,
            int expectFunctionCallCount = 0,
            bool isDocumentHistoryNull = false
        )
        {
            var id = getDataByODataResult.Count == 0 ? null : getDataByODataResult[0].Value.GetPropertyValue("id")?.ToString();
            var action = CreateODataAction(isHistoryTest: true);
            if (setODataActionFunc != null)
            {
                action = setODataActionFunc(action);
            }

            XResponseContinuation responseContinuation = null;
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(getDataByODataResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation, parmaResponseContinuation?.ContinuationString);

            var mockDocVerion = new Mock<IDocumentVersionRepository>();
            if (isDocumentHistoryNull)
            {
                mockDocVerion.Setup(x => x.GetDocumentVersion(It.IsAny<DocumentKey>()));
            }
            else
            {
                mockDocVerion.Setup(x => x.GetDocumentVersion(It.IsAny<DocumentKey>())).Returns(() =>
                {
                    var ret = new DocumentDbDocumentVersions();
                    ret.Id = id;
                    ret.Etag = "hogeTag";
                    ret.Partitionkey = "hogePart";
                    ret.RegDate = DateTime.Now;
                    ret.RegUserId = "hogeUser";
                    ret.Type = "hogeType";
                    ret.UpdDate = DateTime.Now;
                    ret.UpdUserId = "hogeUser";
                    var mappingConfig = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<DocumentHistory, DocumentDbDocumentVersion>().ReverseMap();
                        cfg.CreateMap<DocumentHistories, DocumentDbDocumentVersions>().ReverseMap();
                    });
                    IMapper m = new Mapper(mappingConfig);
                    return m.Map<DocumentHistories>(ret);
                });
            }

            UnityContainer.RegisterInstance<IDocumentVersionRepository>(mockDocVerion.Object);
            mockRepository.SetupProperty(x => x.DocumentVersionRepository, UnityContainer.Resolve<IDocumentVersionRepository>());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(expectFunctionCallCount));

            return result;
        }

        [TestMethod]
        public void ExecuteAction_HistoryHeaderTest()
        {
            var result = ExecuteAction_ForHistoryTest_Common(
                new List<JsonDocument>
                {
                    new JsonDocument("{'id':'hogeId1'}".ToJson())
                }, "{'id':'hogeId1'}", expectFunctionCallCount: 1);

            var header = result.Headers.Count(x => x.Key == "X-DocumentHistory");
            header.Is(1);

            //履歴ヘッダの中身チェック
            var headers = result.Headers.GetValues("X-DocumentHistory").First().ToJson();
            headers.Is($"[{{'isSelfHistory':true,'resourcePath':'/API/Private/QueryTest',documents:[{{'documentKey':'hogeId1','versionKey':null}}]}}]".ToJson());
        }

        [TestMethod]
        public void ExecuteAction_HistoryHeaderTest_JValue()
        {
            var result = ExecuteAction_ForHistoryTest_Common(
                new List<JsonDocument>
                {
                            new JsonDocument(new JValue("value1"))
                }, "\"value1\"", expectFunctionCallCount: 2);

            //ヘッダ無し
            var header = result.Content.Headers.Count(x => x.Key == "X-DocumentHistory");
            header.Is(0);
        }

        [TestMethod]
        public void ExecuteAction_HistoryHeaderTest_履歴設定はあるが履歴が無い()
        {
            var result = ExecuteAction_ForHistoryTest_Common(
                new List<JsonDocument>
                {
                    new JsonDocument("{'id':'hogeId1'}".ToJson())
                }, "{'id':'hogeId1'}", expectFunctionCallCount: 1, isDocumentHistoryNull: true);

            //ヘッダ有り
            var header = result.Headers.Count(x => x.Key == "X-DocumentHistory");
            header.Is(1);

            //履歴ヘッダの中身チェック
            var headers = result.Headers.GetValues("X-DocumentHistory").First().ToJson();
            //履歴設定があるが、履歴が取得できなかった（途中から履歴をONにして該当ドキュメントの履歴がまだない等）は、documentKeyがあり、versionkeyはnullであること
            headers.Is($"[{{'isSelfHistory':true,'resourcePath':'/API/Private/QueryTest',documents:[{{'documentKey':'hogeId1','versionKey':null}}]}}]".ToJson());
        }

        [TestMethod]
        public void ExecuteAction_HistoryHeaderTest_Count1()
        {
            var result = ExecuteAction_ForHistoryTest_Common(
                new List<JsonDocument>
                {
                            new JsonDocument(new JValue("1"))
                }, "\"value1\"", expectFunctionCallCount: 2);

            //ヘッダ無し
            var header = result.Content.Headers.Count(x => x.Key == "X-DocumentHistory");
            header.Is(0);
        }

        private void GetRepositoryData_Common(
            List<JsonDocument> getDataByODataResult,
            string exceptResult,
            Func<ODataAction, ODataAction> setODataActionFunc = null,
            XResponseContinuation parmaResponseContinuation = null,
            XResponseContinuation expectResponseContinuation = null
        )
        {
            var action = CreateODataAction();
            if (setODataActionFunc != null)
            {
                action = setODataActionFunc(action);
            }

            XResponseContinuation responseContinuation = null;
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(getDataByODataResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation, parmaResponseContinuation?.ContinuationString);

            var header = new Dictionary<string, string>();
            var param = new object[] { mockRepository.Object, null, header };
            var result = (string)action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, param);
            result.ToJValue().Is(exceptResult.ToJValue());
            param[1].IsStructuralEqual(expectResponseContinuation);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void GetRepositoryData_データ有_JValue()
        {
            GetRepositoryData_Common(
                new List<JsonDocument>
                {
                            new JsonDocument(new JValue("value1"))
                }, "\"value1\"");
        }

        [TestMethod]
        public void GetRepositoryData_データ有_JValue以外()
        {
            var exceptResult = new JObject
            {
                ["field1"] = "value1"
            };
            GetRepositoryData_Common(
                new List<JsonDocument>
                {
                            new JsonDocument(exceptResult)
                }, exceptResult.ToString());
        }

        [TestMethod]
        public void GetRepositoryData_データ有_array()
        {
            var exceptResult = new JObject
            {
                ["field1"] = "value1"
            };
            GetRepositoryData_Common(
                new List<JsonDocument>
                {
                            new JsonDocument(exceptResult)
                },
                $"[\r\n{exceptResult.ToString()}\r\n]",
                action =>
                {
                    action.PostDataType = new PostDataType("array");
                    return action;
                });
        }

        [TestMethod]
        public void GetRepositoryData_データ有_apiQuery有_結果1つ()
        {
            var exceptResult = new JObject
            {
                ["field1"] = "value1"
            };
            GetRepositoryData_Common(
                new List<JsonDocument>
                {
                            new JsonDocument(exceptResult)
                },
                exceptResult.ToString(),
                action =>
                {
                    action.ApiQuery = new ApiQuery("SELECT *");
                    return action;
                });
        }

        [TestMethod]
        public void GetRepositoryData_データ有_apiQuery有_結果2つ()
        {
            var exceptResult1 = new JObject
            {
                ["field1"] = "value1"
            };
            var exceptResult2 = new JObject
            {
                ["field2"] = "value2"
            };
            GetRepositoryData_Common(
                new List<JsonDocument>
                {
                            new JsonDocument(exceptResult1),
                            new JsonDocument(exceptResult2)
                },
                $"[\r\n{exceptResult1.ToString()},\r\n{exceptResult2.ToString()}\r\n]",
                action =>
                {
                    action.ApiQuery = new ApiQuery("SELECT *");
                    return action;
                });

        }

        [TestMethod]
        public void GetRepositoryData_データ無_null()
        {
            var action = CreateODataAction();
            var keyword = (string)action.GetType().GetField("NULL_KEYWORD", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(action);

            GetRepositoryData_Common(
                (List<JsonDocument>)null,
                keyword);
        }

        [TestMethod]
        public void GetRepositoryData_データ無_count0()
        {
            var action = CreateODataAction();
            var keyword = (string)action.GetType().GetField("NULL_KEYWORD", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(action);

            GetRepositoryData_Common(
                new List<JsonDocument> { },
                keyword);
        }

        [TestMethod]
        public void GetRepositoryData_データ有_ResponseContinuation設定()
        {
            var continuation = new XResponseContinuation("test");
            GetRepositoryData_Common(
                new List<JsonDocument>
                {
                            new JsonDocument(new JValue("value1"))
                }, "\"value1\"",
                null,
                continuation,
                continuation);
        }

        [TestMethod]
        public void GetRepositoryData_データ無_null_ResponseContinuation設定しない()
        {
            var action = CreateODataAction();
            var keyword = (string)action.GetType().GetField("NULL_KEYWORD", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(action);

            GetRepositoryData_Common(
                (List<JsonDocument>)null,
                keyword,
                null,
                null,
                null);
        }

        [TestMethod]
        public void GetRepositoryData_データ無_count0_ResponseContinuation設定しない()
        {
            var action = CreateODataAction();
            var keyword = (string)action.GetType().GetField("NULL_KEYWORD", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(action);

            GetRepositoryData_Common(
                new List<JsonDocument> { },
                keyword,
                null,
                null,
                null);
        }

        [TestMethod]
        public void ExecuteAction_Base64()
        {

            var action = CreateODataAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            var json = CreateBase64AttachFileJson(CreateBase64Registed("file"));
            var exceptionResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(json)) };

            XResponseContinuation responseContinuation = null;

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var resultcomp = JToken.Parse(CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal)));
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(resultcomp.ToString());

        }

        [TestMethod]
        public void ExecuteAction_Base64_添付ファイル設定なし()
        {
            var action = CreateODataAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(false);
            var json = CreateBase64AttachFileJson(CreateBase64Registed("file"));
            var exceptionResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(json)) };

            XResponseContinuation responseContinuation = null;

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(JToken.Parse(json).ToString());

        }

        [TestMethod]
        public void ExecuteAction_Base64_添付ファイル設定Null()
        {
            var action = CreateODataAction();
            action.IsEnableAttachFile = null;
            var json = CreateBase64AttachFileJson(CreateBase64Registed("file"));
            var exceptionResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(json)) };

            XResponseContinuation responseContinuation = null;
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(JToken.Parse(json).ToString());

        }

        [TestMethod]
        public void DocumentHistory_Disable()
        {
            var action = CreateODataAction();
            var exceptionResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };
            XResponseContinuation responseContinuation = null;
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(exceptionResult[0].Value.ToString());
            result.Headers.Contains("X-DocumentHistory").Is(false);
        }

        [TestMethod]
        [TestCase("application/geo+json")]
        [TestCase("application/vnd.geo+json")]
        public void ExecuteAction_getGeoJson()
        {
            TestContext.Run((string mediaType) =>
            {
                var action = CreateODataAction();
                action.PostDataType = new PostDataType("array");
                action.MediaType = new MediaType(mediaType);
                var queryResult = new List<JsonDocument>
                {
                    new JsonDocument(new JObject
                    {
                        ["CityCode"] = "472085",
                        ["Longitude"] = 127.735523872,
                        ["Latitude"] = 26.2496550830001
                    }),
                    new JsonDocument(new JObject
                    {
                        ["CityCode"] = "472086",
                        ["Longitude"] = 127.735825747,
                        ["Latitude"] = 26.2498129090001
                    })
                };
                var expectedResult = $@"
{{
    'type': 'FeatureCollection',
    'features': 
    [
        {{
            'type': 'Feature',
            'geometry': 
            {{ 
                'type': 'Point',
                'coordinates': [ 127.735523872, 26.2496550830001 ] 
            }},
            'properties': 
            {{
                'CityCode': '472085'
            }}
        }},
        {{
            'type': 'Feature',
            'geometry': 
            {{ 
                'type': 'Point',
                'coordinates': [ 127.735825747, 26.2498129090001 ] 
            }},
            'properties': 
            {{
                'CityCode': '472086'
            }}
        }}
    ]
}}";

                XResponseContinuation responseContinuation = null;
                var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(queryResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockRepository.Object
                });

                var result = action.ExecuteAction();
                result.StatusCode.Is(HttpStatusCode.OK);
                result.Content.Headers.ContentType.MediaType.Is(action.MediaType.Value);
                result.Headers.Contains("X-ResponseContinuation").Is(false);
                var geoJson = result.Content.ReadAsStringAsync().Result;
                JToken.Parse(geoJson).ToString().Is(JToken.Parse(expectedResult).ToString());
            });
        }

        [TestMethod]
        [TestCase("application/geo+json")]
        [TestCase("application/vnd.geo+json")]
        public void ExecuteAction_getGeoJson_Unconvertible()
        {
            TestContext.Run((string mediaType) =>
            {
                var action = CreateODataAction();
                action.PostDataType = new PostDataType("array");
                action.MediaType = new MediaType(mediaType);
                var queryResult = new List<JsonDocument>
                {
                    new JsonDocument(new JObject
                    {
                        ["code"] = "foo1",
                        ["name"] = "bar1"
                    }),
                    new JsonDocument(new JObject
                    {
                        ["code"] = "foo2",
                        ["name"] = "bar2"
                    })
                };
                var expectedResult = $@"
[
    {{
        'code': 'foo1',
        'name': 'bar1',
    }},
    {{
        'code': 'foo2',
        'name': 'bar2',
    }}
]";

                XResponseContinuation responseContinuation = null;
                var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(queryResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockRepository.Object
                });

                var result = action.ExecuteAction();
                result.StatusCode.Is(HttpStatusCode.OK);
                result.Content.Headers.ContentType.MediaType.Is("application/json");
                result.Headers.Contains("X-ResponseContinuation").Is(false);
                var geoJson = result.Content.ReadAsStringAsync().Result;
                JToken.Parse(geoJson).ToString().Is(JToken.Parse(expectedResult).ToString());
            });
        }


        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();

        private ODataAction CreateODataAction(bool isHistoryTest = false, string designationRepokey = null)
        {
            ODataAction action = UnityCore.Resolve<ODataAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{Id}");
            action.ControllerRelativeUrl = new ControllerUrl("/API/Private/QueryTest");
            action.RepositoryKey = new RepositoryKey(string.IsNullOrEmpty(designationRepokey) ? "/API/Private/QueryTest/{Id}" : designationRepokey);
            action.ActionType = new ActionTypeVO(ActionType.OData);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            if (isHistoryTest)
            {
                action.IsDocumentHistory = new IsDocumentHistory(true);
                action.HistoryEvacuationDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>().Object;
            }
            action.PostDataType = new PostDataType("");
            action.ApiQuery = new ApiQuery("");
            action.Accept = new Accept("*/*");
            return action;
        }


        private Mock<IDynamicApiAttachFileRepository> SetUpBase64AttachFileRepositoryMock(string json)
        {
            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.IsAny<string>())).Returns(json);
            return mockBlobRepository;
        }

        private static string base64StringNormal = "";

        private static string CreateBase64Query(string value)
        {
            return $"$Base64({value})";
        }
        private static string CreateBase64Registed(string value)
        {
            return $"$Base64Reference({value})";
        }
        private static string CreateBase64AttachFileJson(string base64attachfile)
        {
            return $@"{{
'hoge':'hoge',
'AreaUnitCode': 'あああああああああああああああああああああああああああ',
'file':'{base64attachfile}'
}}";
        }
        private static string CreateBase64AttachFileJson2(string base64attachfile, string base64attachfile2)
        {
            return $@"{{
'hoge':'hoge',
'AreaUnitCode': 'あああああああああああああああああああああああああああ',
'file':'{base64attachfile}',
'file2':'{base64attachfile2}'
}}";
        }


    }
}
