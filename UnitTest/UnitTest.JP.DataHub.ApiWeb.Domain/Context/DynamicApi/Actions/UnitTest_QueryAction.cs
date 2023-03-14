using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.QueryCompiler;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_QueryAction : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterType<IApiQueryCompiler, DefaultApiQueryCompiler>(RepositoryType.CosmosDB.ToCode());

            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<IHttpContextAccessor>(new Mock<IHttpContextAccessor>().Object);
        }

        private Mock<INewDynamicApiDataStoreRepository> CreateINewDynamicApiDataStoreRepositoryMock(JsonSearchResult queryResult, QueryParam queryParam, Exception exception = null, XResponseContinuation responseContinuation = null, string returnresponseContinuation = null)
        {
            List<JsonDocument> jsonDocuments = new List<JsonDocument>();

            if (queryResult != null)
            {
                var array = queryResult.JToken as JArray;
                if (array != null)
                {
                    foreach (var j in array)
                    {
                        jsonDocuments.Add(new JsonDocument(j));
                    }
                }
                else
                {
                    jsonDocuments.Add(new JsonDocument(queryResult.JToken));
                }
            }
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            if (exception == null)
            {
                mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                    .Callback<QueryParam>((para) => {
                        if (queryParam != null)
                        {
                            para.IsNotStructuralEqual(queryParam);
                        }
                    })
                    .Returns(jsonDocuments);
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
                mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                    .Callback<QueryParam>((para) => {
                        if (queryParam != null)
                        {
                            para.IsNotStructuralEqual(queryParam);
                        }
                    })
                    .Throws(exception);
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
                    .Throws(exception);
            }

            mockRepository.SetupGet(x => x.RepositoryInfo).Returns(
                new RepositoryInfo(Guid.NewGuid(), RepositoryType.CosmosDB.ToCode(), new Tuple<string, bool, Guid?>(Guid.NewGuid().ToString(), false, Guid.NewGuid()))
            );

            return mockRepository;
        }


        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_データ有()
        {
            var action = CreateQueryAction();

            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult.BeginData();
            foreach (var str in new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            exceptionResult.EndData();

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            var resultcomp = exceptionResult.Value;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.ToJson().Is(resultcomp.ToJson());

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_データ有_JValue()
        {
            var action = CreateQueryAction();
            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult.BeginData();
            foreach (var str in new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            exceptionResult.EndData();
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            var jsonDocuments = new List<JsonDocument>();
            //JValueセット
            jsonDocuments.Add(new JsonDocument(new JValue(1)));
            XResponseContinuation responseContinuation = null;
            mockRepository.Setup(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation))
                .Returns(jsonDocuments);
            var resultcomp = exceptionResult.Value;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.ToJson().Is(1.ToJson());
            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
            //履歴ヘッダは無いこと
            var header = result.Content.Headers.Count(x => x.Key == "X-DocumentHistory");
            header.Is(0);
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_データ無()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateQueryAction();
            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);

            var msg = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.I10403.ToString(), actualMessage["error_code"]);

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_NotImplementedException()
        {

            var action = CreateQueryAction();

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), new NotImplementedException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            AssertEx.Catch<NotImplementedException>(() => action.ExecuteAction());

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目データ有()
        {

            var action = CreateQueryAction();
            var exceptionResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };
            var compareResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            compareResult.BeginData();
            foreach (var str in exceptionResult.RemoveTokenToJson(false))
            {
                compareResult.AddString(str);
            }
            compareResult.EndData();
            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(compareResult, ValueObjectUtil.Create<QueryParam>(action));
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });
            var testResult = compareResult.Value;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.ToJson().Is(testResult.ToJson());

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository1.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
            // 2つ目は無視
            mockRepository2.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目データ無_2つ目データ有()
        {
            var action = CreateQueryAction();

            var exceptionResult1 = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            var exceptionResult2 = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult2.BeginData();
            var data = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };
            foreach (var str in data.RemoveTokenToJson(false))
            {
                exceptionResult2.AddString(str);
            }

            exceptionResult2.EndData();

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action));
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult2, ValueObjectUtil.Create<QueryParam>(action));

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            var result2 = exceptionResult2.Value;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.ToJson().Is(result2.ToJson());

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository1.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
            mockRepository2.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目データ無_2つ目データ無()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateQueryAction();

            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action));
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action));

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.I10403.ToString(), actualMessage["error_code"]);

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository1.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
            mockRepository2.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }


        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目NotImplementedException_2つ目NotImplementedException()
        {
            var action = CreateQueryAction();
            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), new NotImplementedException());
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<QueryParam>(action), new NotImplementedException());

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            AssertEx.Catch<NotImplementedException>(() => action.ExecuteAction());

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository1.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Post()
        {
            var action = CreateQueryAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);

            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult.BeginData();
            foreach (var str in new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            exceptionResult.EndData();

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var resultcomp = exceptionResult.Value;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.ToJson().Is(resultcomp.ToJson());

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Put()
        {

            var action = CreateQueryAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PUT);

            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult.BeginData();
            foreach (var str in new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            exceptionResult.EndData();

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var resultcomp = exceptionResult.Value;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.ToJson().Is(resultcomp.ToJson());

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Delete()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateQueryAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);

            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10419.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_Patch()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateQueryAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PATCH);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10419.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_hasSingleDataがfalse_Array()
        {

            var action = CreateQueryAction();
            action.PostDataType = new PostDataType("array");

            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult.BeginData();
            foreach (var str in new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            exceptionResult.EndData();

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            var resultcomp = exceptionResult.Value;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            //result.Content.ReadAsStringAsync().Result.ToJson().Is(resultcomp.ToJson());

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_hasSingleDataがfalse_query有()
        {
            var action = CreateQueryAction();
            action.ApiQuery = new ApiQuery(Guid.NewGuid().ToString());

            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult.BeginData();
            foreach (var str in new List<JsonDocument>
                    {
                        new JsonDocument(new JObject
                        {
                            ["field"] = "value"
                        })
                    }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            exceptionResult.EndData();

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            var resultcomp = exceptionResult.Value;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.ToJson().Is(resultcomp.ToJson());

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_異常系_URLスキーマあり_必須エラー()
        {
            var action = CreateQueryAction();

            action.UriSchema = new DataSchema(@"
        {
          type: ""object"",
          properties: {
            id: { type: ""string"" }
          },
          required: [ ""id"" ]
        }
        ");

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void GetRepositoryData_キャッシュ有_キャッシュヒット()
        {
            var keyRepository = "keyRepository";

            var expectResult = "\"value1\"";
            var expectKeyCache = CacheManager.CreateKey(
                "DynamicApiAction",
                ProviderVendorId,
                ProviderSystemId,
                ControllerId,
                ApiId,
                keyRepository);

            var returnCache = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            returnCache.BeginData();
            returnCache.AddString(expectResult);
            returnCache.EndData();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Contains(It.IsAny<string>()))
                .Callback<string>(keyCache =>
                {
                    keyCache.Is(expectKeyCache);
                })
                .Returns(true);
            var outValue = false;
            mockCache.Setup(x => x.Get<JsonSearchResult>(It.IsAny<string>(), out outValue, false))
                .Callback((string keyCache, out bool outValue2, bool boolValue) =>
                {
                    keyCache.Is(expectKeyCache);
                    outValue2 = false;
                    boolValue = false;
                })
                .Returns(returnCache);
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var action = CreateQueryAction();
            action.CacheInfo = new CacheInfo(true, 0, "KeyCache");

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository);

            var header = new Dictionary<string, string>();
            var result = ((JsonSearchResult, string))action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { mockRepository.Object, ValueObjectUtil.Create<QueryParam>(action), header });
            result.Item1.Count.Is((returnCache.Count));
            result.Item1.Value.Is((returnCache.Value));
            result.Item1.JToken.IsStructuralEqual(returnCache.JToken);
            result.Item2.Is($"HIT key:{expectKeyCache}");

            mockRepository.Verify(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>()), Times.Exactly(1));
            mockCache.Verify(x => x.Contains(It.IsAny<string>()), Times.Exactly(1));
            mockCache.Verify(x => x.Get<JsonSearchResult>(It.IsAny<string>(), out outValue, false), Times.Exactly(1));
        }

        [TestMethod]
        public void GetRepositoryData_キャッシュ有_キャッシュヒットなし()
        {
            var keyRepository = "keyRepository";
            var cacheMinute = 30;

            var expectCacheSecond = cacheMinute * 60;
            var expectKeyCache = CacheManager.CreateKey(
                "DynamicApiAction",
                VendorId,
                SystemId,
                ControllerId,
                ApiId,
                keyRepository);


            var action = CreateQueryAction();
            action.CacheInfo = new CacheInfo(true, cacheMinute, "KeyCache");
            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult.BeginData();
            foreach (var str in new List<JsonDocument>
                    {
                        new JsonDocument(new JObject
                        {
                            ["field"] = "value"
                        })
                    }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            exceptionResult.EndData();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()))
                .Callback<string, object, int>((keyCache, text, second) =>
                {
                    keyCache.Is(expectKeyCache);
                    //text.Is(compareResult);
                    second.Is(expectCacheSecond);
                });
            mockCache.Setup(x => x.Contains(It.IsAny<string>()))
                .Callback<string>(keyCache =>
                {
                    keyCache.Is(expectKeyCache);
                })
                .Returns(false);
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);


            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            mockRepository.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository);
            var header = new Dictionary<string, string>();
            var result = ((JsonSearchResult, string))action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { mockRepository.Object, ValueObjectUtil.Create<QueryParam>(action), header });
            result.Item1.Value.ToJValue().Is(exceptionResult.Value.ToJValue());
            result.Item1.Count.Is(exceptionResult.Count);
            result.Item2.IsNull();

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
            mockRepository.Verify(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_HistoryHeaderTest()
        {
            var action = CreateQueryAction(isHistoryTest: true);
            XResponseContinuation responseContinuation = null;

            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult.BeginData();
            foreach (var str in new List<JsonDocument>
            {
                new JsonDocument("{'id':'hogeId1'}".ToJson())
            }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            exceptionResult.EndData();
            var jsonDocuments = new List<JsonDocument>();
            jsonDocuments.Add(new JsonDocument("{'id':'hogeId1'}"));
            var queryPrm = ValueObjectUtil.Create<QueryParam>(action);
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation))
                .Returns(jsonDocuments);
            mockRepository.SetupGet(x => x.RepositoryInfo).Returns(
                new RepositoryInfo(Guid.NewGuid(), RepositoryType.CosmosDB.ToCode(), new Tuple<string, bool, Guid?>(Guid.NewGuid().ToString(), false, Guid.NewGuid()))
            );

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var mockDocVerion = new Mock<IDocumentVersionRepository>();
            mockDocVerion.Setup(x => x.GetDocumentVersion(It.IsAny<DocumentKey>())).Returns(() =>
            {
                var ret = new DocumentDbDocumentVersions();
                ret.Id = "hogeId1";
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

            UnityContainer.RegisterInstance<IDocumentVersionRepository>(mockDocVerion.Object);
            mockRepository.SetupProperty(x => x.DocumentVersionRepository, UnityContainer.Resolve<IDocumentVersionRepository>());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));

            var header = result.Headers.Count(x => x.Key == "X-DocumentHistory");
            header.Is(1);

            //履歴ヘッダの中身チェック
            var headers = result.Headers.GetValues("X-DocumentHistory").First().ToJson();
            headers.Is($"[{{'isSelfHistory':true,'resourcePath':'/API/Private/QueryTest',documents:[{{'documentKey':'hogeId1','versionKey':null}}]}}]".ToJson());
        }

        [TestMethod]
        public void ExecuteAction_HistoryHeaderTest_履歴設定はあるが履歴が無い()
        {
            var action = CreateQueryAction(isHistoryTest: true);
            XResponseContinuation responseContinuation = null;

            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult.BeginData();
            foreach (var str in new List<JsonDocument>
            {
                new JsonDocument("{'id':'hogeId1'}".ToJson())
            }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            exceptionResult.EndData();
            var jsonDocuments = new List<JsonDocument>();
            jsonDocuments.Add(new JsonDocument("{'id':'hogeId1'}"));
            var queryPrm = ValueObjectUtil.Create<QueryParam>(action);
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation))
                .Returns(jsonDocuments);
            mockRepository.SetupGet(x => x.RepositoryInfo).Returns(
                new RepositoryInfo(Guid.NewGuid(), RepositoryType.CosmosDB.ToCode(), new Tuple<string, bool, Guid?>(Guid.NewGuid().ToString(), false, Guid.NewGuid()))
            );

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var mockDocVerion = new Mock<IDocumentVersionRepository>();
            //履歴は、nullを返す
            mockDocVerion.Setup(x => x.GetDocumentVersion(It.IsAny<DocumentKey>()));

            UnityContainer.RegisterInstance<IDocumentVersionRepository>(mockDocVerion.Object);
            mockRepository.SetupProperty(x => x.DocumentVersionRepository, UnityContainer.Resolve<IDocumentVersionRepository>());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));

            var header = result.Headers.Count(x => x.Key == "X-DocumentHistory");
            header.Is(1);

            //履歴ヘッダの中身チェック
            var headers = result.Headers.GetValues("X-DocumentHistory").First().ToJson();
            //履歴設定があるが、履歴が取得できなかった（途中から履歴をONにして該当ドキュメントの履歴がまだない等）は、documentKeyがあり、versionkeyはnullであること
            headers.Is($"[{{'isSelfHistory':true,'resourcePath':'/API/Private/QueryTest',documents:[{{'documentKey':'hogeId1','versionKey':null}}]}}]".ToJson());
        }

        [TestMethod]
        public void ExecuteAction_HistoryHeaderTest_履歴設定はあるが履歴が無い_Array()
        {
            var action = CreateQueryAction(isHistoryTest: true, isArrayTest: true);
            XResponseContinuation responseContinuation = null;

            var exceptionResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            exceptionResult.BeginData();
            foreach (var str in new List<JsonDocument>
            {
                new JsonDocument("{'id':'hogeId1'}".ToJson())
            }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            foreach (var str in new List<JsonDocument>
            {
                new JsonDocument("{'id':'hogeId2'}".ToJson())
            }.RemoveTokenToJson(false))
            {
                exceptionResult.AddString(str);
            }
            exceptionResult.EndData();
            var jsonDocuments = new List<JsonDocument>();
            jsonDocuments.Add(new JsonDocument("{'id':'hogeId1'}"));
            jsonDocuments.Add(new JsonDocument("{'id':'hogeId2'}"));
            var queryPrm = ValueObjectUtil.Create<QueryParam>(action);
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation))
                .Returns(jsonDocuments);
            mockRepository.SetupGet(x => x.RepositoryInfo).Returns(
                new RepositoryInfo(Guid.NewGuid(), RepositoryType.CosmosDB.ToCode(), new Tuple<string, bool, Guid?>(Guid.NewGuid().ToString(), false, Guid.NewGuid()))
            );

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var mockDocVerion = new Mock<IDocumentVersionRepository>();
            //履歴は、nullを返す
            mockDocVerion.Setup(x => x.GetDocumentVersion(It.IsAny<DocumentKey>()));

            UnityContainer.RegisterInstance<IDocumentVersionRepository>(mockDocVerion.Object);
            mockRepository.SetupProperty(x => x.DocumentVersionRepository, UnityContainer.Resolve<IDocumentVersionRepository>());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));

            var header = result.Headers.Count(x => x.Key == "X-DocumentHistory");
            header.Is(1);

            //履歴ヘッダの中身チェック
            var headers = result.Headers.GetValues("X-DocumentHistory").First().ToJson();
            //履歴設定があるが、履歴が取得できなかった（途中から履歴をONにして該当ドキュメントの履歴がまだない等）は、documentKeyがあり、versionkeyはnullであること
            headers.Is($"[{{'isSelfHistory':true,'resourcePath':'/API/Private/QueryTest',documents:[{{'documentKey':'hogeId1','versionKey':null}},{{'documentKey':'hogeId2','versionKey':null}}]}}]".ToJson());
        }

        [TestMethod]
        public void ExecuteAction_Base64()
        {
            var action = CreateQueryAction();
            var json = CreateBase64AttachFileJson(CreateBase64Registed("file"));

            var exceptionResult = new JsonSearchResult(json, 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            action.AttachFileDynamicApiDataStoreRepository = SetUpBase64AttachFileRepositoryMock(base64StringNormal).Object;
            var resultcomp = JToken.Parse(CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal)));
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(resultcomp.ToString());
        }

        [TestMethod]
        public void ExecuteAction_Base64_添付ファイル設定なし()
        {
            var action = CreateQueryAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(false);
            var json = CreateBase64AttachFileJson(CreateBase64Registed("file"));

            var exceptionResult = new JsonSearchResult(json, 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            action.AttachFileDynamicApiDataStoreRepository = SetUpBase64AttachFileRepositoryMock(base64StringNormal).Object;
            var resultcomp = json;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.ToJson().Is(resultcomp.ToJson());
        }
        [TestMethod]
        public void ExecuteAction_Base64_添付ファイル設定Null()
        {
            var action = CreateQueryAction();
            action.IsEnableAttachFile = null;
            var json = CreateBase64AttachFileJson(CreateBase64Registed("file"));

            var exceptionResult = new JsonSearchResult(json, 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            action.AttachFileDynamicApiDataStoreRepository = SetUpBase64AttachFileRepositoryMock(base64StringNormal).Object;
            var resultcomp = json;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.ToJson().Is(resultcomp.ToJson());
        }

        [TestMethod]
        public void ExecuteAction_Base64_Array()
        {
            var action = CreateQueryAction();
            var json = CreateBase64AttachFileJson(CreateBase64Registed("file"));
            var json2 = CreateBase64AttachFileJson(CreateBase64Registed("file2"));

            var exceptionResult = new JsonSearchResult($"[{json},{json2}]", 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.Is<string>(a => a == "file"))).Returns(base64StringNormal);
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.Is<string>(a => a == "file2"))).Returns(base64StringNormal2);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            action.PostDataType = new PostDataType("array");
            var resultcomp = JToken.Parse($"[{CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal))},{CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal2))}]");
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(resultcomp.ToString());
        }

        [TestMethod]
        public void ExecuteAction_Base64_複数ファイル()
        {
            var action = CreateQueryAction();
            var json = CreateBase64AttachFileJson2(CreateBase64Registed("file"), CreateBase64Registed("file2"));

            var exceptionResult = new JsonSearchResult(json, 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.Is<string>(a => a == "file"))).Returns(base64StringNormal);
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.Is<string>(a => a == "file2"))).Returns(base64StringNormal2);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;

            var resultcomp = JToken.Parse(CreateBase64AttachFileJson2(CreateBase64Query(base64StringNormal), CreateBase64Query(base64StringNormal2)));
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(resultcomp.ToString());
        }

        [TestMethod]
        public void ExecuteAction_Base64_内部Array()
        {
            var action = CreateQueryAction();
            var json = $@"{{
        'id':'hogeId1',
        'hoge':'hoge',
        'AreaUnitCode': 'あああああああああああああああああああああああああああ',
        'file': [
        		{{
                    'fuga': '{CreateBase64Registed("file")}'
                }},
        		{{
                    'hoge': 'piyo'
                }}
        	]
        }}";

            var jsonResult = $@"{{
        'id':'hogeId1',
        'hoge':'hoge',
        'AreaUnitCode': 'あああああああああああああああああああああああああああ',
        'file': [
        		{{
                    'fuga': '{CreateBase64Query(base64StringNormal)}'
                }},
        		{{
                    'hoge': 'piyo'
                }}
        	]
        }}";


            var exceptionResult = new JsonSearchResult(json, 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.IsAny<string>())).Returns(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;

            var resultcomp = JToken.Parse(jsonResult);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(resultcomp.ToString());
        }

        [TestMethod]
        public void ExecuteAction_Base64_内部Array2()
        {
            var action = CreateQueryAction();
            var json = $@"{{
            'id':'hogeId1',
            'hoge':'hoge',
            'AreaUnitCode': 'あああああああああああああああああああああああああああ',
            'file': ['{CreateBase64Registed("file")}','piyo']
            }}";

            var jsonResult = $@"{{
            'id':'hogeId1',
            'hoge':'hoge',
            'AreaUnitCode': 'あああああああああああああああああああああああああああ',
            'file': ['{CreateBase64Query(base64StringNormal)}', 'piyo']
            }}";
            var exceptionResult = new JsonSearchResult(json, 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);

            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.IsAny<string>())).Returns(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var resultcomp = JToken.Parse(jsonResult);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(resultcomp.ToString());
        }

        [TestMethod]
        public void ExecuteAction_getCsv()
        {
            var action = CreateQueryAction();
            action.MediaType = new MediaType("text/csv");
            var json = $@"{{
            'code':'foo',
            'name': 'bar',
            }}";

            var csvResult = $@"code,name
foo,bar
";
            var exceptionResult = new JsonSearchResult(json, 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.IsAny<string>())).Returns(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(csvResult);
        }

        [TestMethod]
        public void ExecuteAction_getCsv_array()
        {
            var action = CreateQueryAction();
            action.MediaType = new MediaType("text/csv");
            var json = $@"{{
        'array': [1,2,3]
        }}";
            var exceptionResult = new JsonSearchResult(json, 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.IsAny<string>())).Returns(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;

            AssertEx.Catch<NotParseCsvException>(() => action.ExecuteAction());
        }

        [TestMethod]
        public void ExecuteAction_getCsv_object()
        {
            var action = CreateQueryAction();
            action.MediaType = new MediaType("text/csv");
            var json = $@"{{
        'obj':
            {{'item1':'hoge','item2':'2'}}
        }}";

            var exceptionResult = new JsonSearchResult(json, 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.IsAny<string>())).Returns(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;

            AssertEx.Catch<NotParseCsvException>(() => action.ExecuteAction());
        }

        [TestMethod]
        public void ExecuteAction_getCsv_arrayObject1()
        {
            var action = CreateQueryAction();
            action.MediaType = new MediaType("text/csv");
            var json = $@"{{
            'obj':[
                {{
                    'item1':'hoge','item2':1
                }},
                {{
                    'item1':'fuga','item2':2
                }}
            ]
        }}";

            var exceptionResult = new JsonSearchResult(json, 1);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, null);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.IsAny<string>())).Returns(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;

            AssertEx.Catch<NotParseCsvException>(() => action.ExecuteAction());
        }

        [TestMethod]
        [TestCase("application/geo+json")]
        [TestCase("application/vnd.geo+json")]
        public void ExecuteAction_getGeoJson_single()
        {
            TestContext.Run((string mediaType) =>
            {
                var action = CreateQueryAction();
                action.MediaType = new MediaType(mediaType);
                var json = $@"
{{
    'CityCode': '472085',
    'Longitude': 127.735523872,
    'Latitude': 26.2496550830001
}}";
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
        }}
    ]
}}";

                var queryResult = new JsonSearchResult(json, 1);
                var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(queryResult, null);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

                var result = action.ExecuteAction();
                result.StatusCode.Is(HttpStatusCode.OK);
                result.Content.Headers.ContentType.MediaType.Is(action.MediaType.Value);
                var geoJson = result.Content.ReadAsStringAsync().Result;
                JToken.Parse(geoJson).ToString().Is(JToken.Parse(expectedResult).ToString());
            });
        }

        [TestMethod]
        [TestCase("application/geo+json")]
        [TestCase("application/vnd.geo+json")]
        public void ExecuteAction_getGeoJson_array()
        {
            TestContext.Run((string mediaType) =>
            {
                var action = CreateQueryAction(false, true);
                action.MediaType = new MediaType(mediaType);
                var json = $@"
[
    {{
        'CityCode': '472085',
        'Longitude': 127.735523872,
        'Latitude': 26.2496550830001
    }},
    {{
        'CityCode': '472086',
        'Longitude': 127.735825747,
        'Latitude': 26.2498129090001
    }}
]";
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

                var queryResult = new JsonSearchResult(json, 1);
                var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(queryResult, null);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

                var result = action.ExecuteAction();
                result.StatusCode.Is(HttpStatusCode.OK);
                result.Content.Headers.ContentType.MediaType.Is(action.MediaType.Value);
                var geoJson = result.Content.ReadAsStringAsync().Result;
                JToken.Parse(geoJson).ToString().Is(JToken.Parse(expectedResult).ToString());
            });
        }

        [TestMethod]
        [TestCase("application/geo+json")]
        [TestCase("application/vnd.geo+json")]
        public void ExecuteAction_getGeoJson_Unconvertible_single()
        {
            TestContext.Run((string mediaType) =>
            {
                var action = CreateQueryAction();
                action.MediaType = new MediaType(mediaType);
                var json = $@"
{{
    'code':'foo',
    'name': 'bar',
}}";
                var expectedResult = json;

                var queryResult = new JsonSearchResult(json, 1);
                var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(queryResult, null);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

                var result = action.ExecuteAction();
                result.StatusCode.Is(HttpStatusCode.OK);
                result.Content.Headers.ContentType.MediaType.Is("application/json");
                var geoJson = result.Content.ReadAsStringAsync().Result;
                JToken.Parse(geoJson).ToString().Is(JToken.Parse(expectedResult).ToString());
            });
        }

        [TestMethod]
        [TestCase("application/geo+json")]
        [TestCase("application/vnd.geo+json")]
        public void ExecuteAction_getGeoJson_Unconvertible_array()
        {
            TestContext.Run((string mediaType) =>
            {
                var action = CreateQueryAction(false, true);
                action.MediaType = new MediaType(mediaType);
                var json = $@"
[
    {{
        'code':'foo1',
        'name': 'bar1',
    }},
    {{
        'code':'foo2',
        'name': 'bar2',
    }}
]";
                var expectedResult = json;

                var queryResult = new JsonSearchResult(json, 1);
                var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(queryResult, null);
                action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

                var result = action.ExecuteAction();
                result.StatusCode.Is(HttpStatusCode.OK);
                result.Content.Headers.ContentType.MediaType.Is("application/json");
                var geoJson = result.Content.ReadAsStringAsync().Result;
                JToken.Parse(geoJson).ToString().Is(JToken.Parse(expectedResult).ToString());
            });
        }

        [TestMethod]
        public void ExecuteAction_BlobCache_キャッシュヒット()
        {
            var keyRepository = "keyRepository";

            var expectResult = $@"{{
          ""code"": ""foo"",
          ""name"": ""bar""
        }}";
            var expectKeyCache = CacheManager.CreateBlobKey(
                                     "DynamicApiAction",
                                     ProviderVendorId,
                                     ProviderSystemId,
                                     ControllerId)
                                 + "/" +
                                 CacheManager.CreateBlobKey(
                                     ApiId,
                                     keyRepository);

            var returnCache = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            returnCache.BeginData();
            returnCache.AddString(expectResult);
            returnCache.EndData();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Contains(It.IsAny<string>()))
                .Callback<string>(keyCache =>
                {
                    keyCache.Is(expectKeyCache);
                })
                .Returns(true);
            var outValue = false;
            mockCache.Setup(x => x.Get<JsonSearchResult>(It.IsAny<string>(), out outValue, false))
                .Callback((string keyCache, out bool outValue2, bool boolValue) =>
                {
                    keyCache.Is(expectKeyCache);
                    outValue2 = false;
                    boolValue = false;
                })
                .Returns(returnCache);
            UnityContainer.RegisterInstance("DynamicApiBlobCache", mockCache.Object);

            var action = CreateQueryAction();
            action.CacheInfo = new CacheInfo(true, 0, "KeyCache");
            action.IsUseBlobCache = new IsUseBlobCache(true);

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository);

            var header = new Dictionary<string, string>();
            var result = (ValueTuple<JsonSearchResult, string>)action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { mockRepository.Object, ValueObjectUtil.Create<QueryParam>(action), header });
            result.Item1.Count.Is((returnCache.Count));
            result.Item1.Value.Is((returnCache.Value));
            result.Item1.JToken.IsStructuralEqual(returnCache.JToken);
            result.Item2.Is($"HIT key:{expectKeyCache}");

            mockRepository.Verify(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>()), Times.Exactly(2));
            mockCache.Verify(x => x.Contains(It.IsAny<string>()), Times.Exactly(1));
            mockCache.Verify(x => x.Get<JsonSearchResult>(It.IsAny<string>(), out outValue, false), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_BlobCache_キャッシュヒットなし()
        {
            var keyRepository = "keyRepository";
            var cacheMinute = 30;

            var expectCacheSecond = cacheMinute * 60;
            var expectKeyCache = CacheManager.CreateBlobKey(
                                     "DynamicApiAction",
                                     ProviderVendorId,
                                     ProviderSystemId,
                                     ControllerId)
                                 + "/" +
                                 CacheManager.CreateBlobKey(
                                     ApiId,
                                     keyRepository);


            var action = CreateQueryAction();
            action.CacheInfo = new CacheInfo(true, cacheMinute, "KeyCache");
            action.IsUseBlobCache = new IsUseBlobCache(true);
            var compareResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            compareResult.BeginData();
            foreach (var str in new List<JsonDocument>
                    {
                        new JsonDocument(new JObject
                        {
                            ["field"] = "value"
                        })
                    }.RemoveTokenToJson(false))
            {
                compareResult.AddString(str);
            }
            compareResult.EndData();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()))
                .Callback<string, object, int>((keyCache, text, second) =>
                {
                    keyCache.Is(expectKeyCache);
                    //text.Is(compareResult);
                    second.Is(expectCacheSecond);
                });
            mockCache.Setup(x => x.Contains(It.IsAny<string>()))
                .Callback<string>(keyCache =>
                {
                    keyCache.Is(expectKeyCache);
                })
                .Returns(false);
            UnityContainer.RegisterInstance<ICache>("DynamicApiBlobCache", mockCache.Object);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(compareResult, null);
            mockRepository.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository);

            var header = new Dictionary<string, string>();
            var result = (ValueTuple<JsonSearchResult, string>)action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { mockRepository.Object, ValueObjectUtil.Create<QueryParam>(action), header });
            result.Item1.Value.ToJValue().Is(compareResult.Value.ToJValue());
            result.Item1.Count.Is(compareResult.Count);
            result.Item2.IsNull();

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
            mockRepository.Verify(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task ExecuteAction_BlobCache_保存に失敗()
        {
            var keyRepository = "keyRepository";
            var cacheMinute = 30;

            var expectCacheSecond = cacheMinute * 60;
            var expectKeyCache = CacheManager.CreateBlobKey(
                                     "DynamicApiAction",
                                     ProviderVendorId,
                                     ProviderSystemId,
                                     ControllerId)
                                 + "/" +
                                 CacheManager.CreateBlobKey(
                                     ApiId,
                                     keyRepository);


            var action = CreateQueryAction();
            action.CacheInfo = new CacheInfo(true, cacheMinute, "KeyCache");
            action.IsUseBlobCache = new IsUseBlobCache(true);
            var compareResult = new JsonSearchResult(new ApiQuery(""), new PostDataType(""),
                new ActionTypeVO(ActionType.Query));
            compareResult.BeginData();
            foreach (var str in new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            }.RemoveTokenToJson(false))
            {
                compareResult.AddString(str);
            }

            compareResult.EndData();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<int>()))
                .Callback<string, object, int>((keyCache, text, second) => throw new Exception());
            mockCache.Setup(x => x.Contains(It.IsAny<string>()))
                .Callback<string>(keyCache => { keyCache.Is(expectKeyCache); })
                .Returns(false);
            UnityContainer.RegisterInstance<ICache>("DynamicApiBlobCache", mockCache.Object);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(compareResult, null);
            mockRepository.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(),
                    It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>()))
                .Returns(keyRepository);

            var header = new Dictionary<string, string>();
            var result = (ValueTuple<JsonSearchResult, string>)action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { mockRepository.Object, ValueObjectUtil.Create<QueryParam>(action), header });
            result.Item1.Value.ToJValue().Is(compareResult.Value.ToJValue());
            result.Item1.Count.Is(compareResult.Count);
            result.Item2.IsNull();

            // 引数はCallbackで判定
            XResponseContinuation responseContinuation = null;
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
            mockRepository.Verify(
                x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(),
                    It.IsAny<IResourceVersionRepository>()), Times.Exactly(2));

            //Disposeで消えないか確認
            result.Item1.Dispose();
            result.Item1.Value.ToJValue().Is(compareResult.Value.ToJValue());
        }


        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();
        private Guid ProviderVendorId = Guid.NewGuid();
        private Guid ProviderSystemId = Guid.NewGuid();

        private QueryAction CreateQueryAction(bool isHistoryTest = false, bool isArrayTest = false)
        {
            QueryAction action = UnityCore.Resolve<QueryAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.ProviderSystemId = new SystemId(ProviderSystemId.ToString());
            action.ProviderVendorId = new VendorId(ProviderVendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.ControllerRelativeUrl = new ControllerUrl("/API/Private/QueryTest");
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{Id}");
            action.ActionType = new ActionTypeVO(ActionType.Query);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.IsVendor = new IsVendor(false);
            action.IsPerson = new IsPerson(false);
            action.OpenId = null;
            action.ResourceSharingPersonRules = new List<ResourceSharingPersonRule> { };
            {
                action.IsDocumentHistory = new IsDocumentHistory(true);
                action.HistoryEvacuationDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>().Object;
            }

            action.PostDataType = isArrayTest ? new PostDataType("array") : new PostDataType("");
            action.ApiQuery = new ApiQuery("");
            action.Accept = new Accept("*/*");
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.IsUseBlobCache = new IsUseBlobCache(false);

            return action;
        }

        private Mock<IDynamicApiAttachFileRepository> SetUpBase64AttachFileRepositoryMock(string json)
        {
            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.IsAny<string>())).Returns(json);
            return mockBlobRepository;
        }

        private static string base64StringNormal = "";
        private static string base64StringNormal2 = "";

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
'id':'hogeId1',
'hoge':'hoge',
'AreaUnitCode': 'あああああああああああああああああああああああああああ',
'file':'{base64attachfile}'
}}";
        }
        private static string CreateBase64AttachFileJson2(string base64attachfile, string base64attachfile2)
        {
            return $@"{{
'id':'hogeId1',
'hoge':'hoge',
'AreaUnitCode': 'あああああああああああああああああああああああああああ',
'file':'{base64attachfile}',
'file2':'{base64attachfile2}'
}}";
        }
    }
}
