using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
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
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_ODataDeleteDataAction : UnitTestBase
    {
        private const int WaitTimes = 3;
        private const int WaitMs = 1000;

        private static Accept _defaultAccept = new Accept("*/*");

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread");
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", true);

            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<IDataContainer>(new PerRequestDataContainer());
        }


        private Mock<INewDynamicApiDataStoreRepository> CreateINewDynamicApiDataStoreRepositoryMock(List<JsonDocument> jsonDocuments,
            DeleteParam deleteParam, QueryParam queryParam, IPerRequestDataContainer perRequestDataContainer, Exception exception = null)
        {

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
                mockRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>())).Callback<DeleteParam>((para) =>
                {
                    if (deleteParam != null)
                    {
                        para.IsNotStructuralEqual(deleteParam);
                    }
                    if (perRequestDataContainer != null)
                    {
                        UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                    }
                    para.CallbackDelete?.Invoke(para.Json.RemoveTokenToJson(XGetInnerAllField: true).ToString().ToJson(), RepositoryType.CosmosDB);
                });

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
                mockRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>())).Callback<DeleteParam>((para) =>
                {
                    if (deleteParam != null)
                    {
                        para.IsNotStructuralEqual(deleteParam);
                    }
                    if (perRequestDataContainer != null)
                        UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                }).Throws(exception);
            }
            mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));

            return mockRepository;
        }

        List<JsonDocument> defResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["id"] = "value"
                })
            };


        [TestMethod]
        public void ExecuteAction_正常_QueryString無し()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            var action = CreateODataDeleteDataAction(data);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            perRequestDataContainer.XgetInternalAllField.IsFalse();
        }

        [TestMethod]
        public void ExecuteAction_正常_HistoryHeaderCheck()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            var action = CreateODataDeleteDataAction(data, isHistoryTest: true);
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            DeleteParam deleteParam = ValueObjectUtil.Create<DeleteParam>(action);
            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            mockRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>())).Callback<DeleteParam>((param) =>
            {
                param.CallbackDelete?.Invoke(param.Json, RepositoryType.CosmosDB);
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

            var registerId = "hogeId1";
            mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Returns(() => {
                    var dic = new Dictionary<string, object>();
                    dic.Add("Container", registerId);
                    return new RegisterOnceResult(registerId, dic);
                });

            mockDocVerion.Setup(x => x.SaveDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<RepositoryKeyInfo>(), It.IsAny<bool>())).Returns(() =>
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
            mockDocVerion.Setup(x => x.GetDocumentVersion(It.IsAny<DocumentKey>())).Returns(() =>
            {
                var dochist = new List<DocumentHistory>();
                dochist.Add(new DocumentHistory("hogeVer", 1, DateTime.Now, "hogeId", DocumentHistory.StorageLocationType.LowPerformance, new RepositoryKeyInfo(Guid.NewGuid(), Guid.NewGuid()), "hogePath"));
                var dochists = new DocumentHistories(dochist);

                return dochists;
            });
            mockDocVerion.Setup(x => x.SaveDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<RepositoryKeyInfo>(), It.IsAny<DocumentHistory>(), It.IsAny<bool>())).Returns(() =>
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
            mockDocVerion.Setup(x => x.SaveDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<RepositoryKeyInfo>(), It.IsAny<bool>())).Returns(() =>
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
            var doc = new Mock<IJPDataHubCosmosDb>();
            UnityContainer.RegisterInstance<IJPDataHubCosmosDb>(doc.Object);

            mockRepository.SetupProperty(x => x.DocumentVersionRepository, UnityContainer.Resolve<IDocumentVersionRepository>());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });
            UnityContainer.RegisterInstance<INewDynamicApiDataStoreRepository>(mockRepository.Object);
            action.HistoryEvacuationDataStoreRepository = mockRepository.Object;

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            perRequestDataContainer.XgetInternalAllField.IsTrue();

            var header = result.Headers.Count(x => x.Key == "X-DocumentHistory");
            header.Is(1);

            //履歴ヘッダの中身チェック
            var headers = result.Headers.GetValues("X-DocumentHistory").First().ToJson();
            headers.Is($"[{{'isSelfHistory':true,'resourcePath':'/API/Private/QueryTest',documents:[{{'documentKey':'aaa','versionKey':null}}]}}]".ToJson());
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_base64無し_削除有_0件削除()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_base64無し_削除有_1件削除()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });


            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_base64無し_削除有_複数件削除()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });


            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_base64無し_削除失敗_削除データnull()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });


            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_NotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);


            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new NotImplementedException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });


            AssertEx.Catch<NotImplementedException>(() => action.ExecuteAction());

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        private void ExecuteAction_リポジトリ2つ_Common(
            List<List<JsonDocument>> deleteDataResults,
            HttpStatusCode expectStatusCode,
            string expectContent,
            ErrorCodeMessage.Code errorCode = default
        )
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(deleteDataResults[0], ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(deleteDataResults[1], ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });


            var result = action.ExecuteAction();
            result.StatusCode.Is(expectStatusCode);
            if (errorCode == default)
            {
                result.Content.ReadAsStringAsync().Result.Is(expectContent);
            }
            else
            {
                result.Content.ReadAsStringAsync().Result.ToJson()["error_code"] = errorCode.ToString();
            }

            int count1 = deleteDataResults[0].Any() ? 1 : 0;
            int count2 = deleteDataResults[1].Any() ? 1 : 0;

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(count1));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(count2));
        }
        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_両方削除有()
        {
            var exceptionResult1 = defResult;
            var exceptionResult2 = defResult;

            ExecuteAction_リポジトリ2つ_Common(
                new List<List<JsonDocument>> { exceptionResult1, exceptionResult2 },
                HttpStatusCode.NoContent,
                "");
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_片方データ無()
        {
            var exceptionResult1 = defResult;

            var exceptionResult2 = new List<JsonDocument>();

            ExecuteAction_リポジトリ2つ_Common(
                new List<List<JsonDocument>> { exceptionResult1, exceptionResult2 },
                HttpStatusCode.NoContent,
                "");
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_両方データ無()
        {
            var exceptionResult1 = new List<JsonDocument>();

            var exceptionResult2 = new List<JsonDocument>();

            ExecuteAction_リポジトリ2つ_Common(
                new List<List<JsonDocument>> { exceptionResult1, exceptionResult2 },
                HttpStatusCode.NotFound,
                string.Empty,
                ErrorCodeMessage.Code.E10428);
        }

        [TestMethod]
        public async Task ExecuteAction_リポジトリ2つ_キャッシュ削除エラー_NotImplementedException()
        {
            var exceptionResult1 = defResult;
            var exceptionResult2 = defResult;

            var callCount = 0;
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.RemoveFirstMatch(It.IsAny<string>()))
                .Callback<string>(x => callCount++)
                .Throws(new NotImplementedException());
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            ExecuteAction_リポジトリ2つ_Common(
                new List<List<JsonDocument>> { exceptionResult1, exceptionResult2 },
                HttpStatusCode.NoContent,
                "");

            // キャッシュ削除は非同期のため実行されるまで待機
            for (var i = 0; i < WaitTimes; i++)
            {
                if (callCount > 0) break;
                await Task.Delay(WaitMs);
            }

            mockCache.Verify(x => x.RemoveFirstMatch(It.Is<string>(it =>
                it == CacheManager.CreateKey(
                    "DynamicApiAction",
                    VendorId,
                    SystemId,
                    ControllerId)
            )), Times.Exactly(1));
        }

        [TestMethod]
        public async Task ExecuteAction_リポジトリ2つ_キャッシュ削除エラー_Exception()
        {
            var exceptionResult1 = defResult;
            var exceptionResult2 = defResult;

            var callCount = 0;
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.RemoveFirstMatch(It.IsAny<string>()))
                .Callback<string>(x => callCount++)
                .Throws(new Exception());
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            #region ExecuteAction_リポジトリ2つ_Common を流用
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult1, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult2, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,mockRepository2.Object
            });

            action.ExecuteAction(); // Exception投げられない

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            #endregion

            // キャッシュ削除は非同期のため実行されるまで待機
            for (var i = 0; i < WaitTimes; i++)
            {
                if (callCount > 0) break;
                await Task.Delay(WaitMs);
            }

            mockCache.Verify(x => x.RemoveFirstMatch(It.Is<string>(it =>
                it == CacheManager.CreateKey(
                    "DynamicApiAction",
                    VendorId,
                    SystemId,
                    ControllerId)
            )), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_PrimaryがNotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new NotImplementedException());
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            AssertEx.Catch<NotImplementedException>(() => action.ExecuteAction());

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_Primaryがその他エラー()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new Exception("想定外のエラー"));
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            AssertEx.Catch<Exception>(() => action.ExecuteAction());

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_両方NotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new NotImplementedException());
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new NotImplementedException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });


            AssertEx.Catch<NotImplementedException>(() => action.ExecuteAction());

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_ODataException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);
            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object
                    });


            AssertEx.Catch<ODataException>(() => action.ExecuteAction());

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_ODataInvalidFilterColumnException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataInvalidFilterColumnException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object
                    });

            AssertEx.Catch<ODataException>(() => action.ExecuteAction());

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_PrimaryがODataException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);


            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataInvalidFilterColumnException());
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            AssertEx.Catch<ODataException>(() => action.ExecuteAction());

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_PrimaryがODataInvalidColumnException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataInvalidFilterColumnException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }


        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_PrimaryがODataException_SecondaryがODataInvalidColumnException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataException("test"));
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataInvalidFilterColumnException("test"));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            AssertEx.Catch<ODataException>(() => action.ExecuteAction());

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_両方ODataException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataException("test1"));
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataException("hoge"));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            AssertEx.Catch<ODataException>(() => action.ExecuteAction());

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }


        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_両方ODataInvalidColumnException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataException("test"));
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataInvalidFilterColumnException("hoge"));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });
            AssertEx.Catch<ODataException>(() => action.ExecuteAction());

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_プライマリ正常_セカンダリがその他エラー()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new Exception("全く想定外エラー"));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            AssertEx.Catch<Exception>(() => action.ExecuteAction());

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_プライマリ正常_セカンダリNotImple()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new NotImplementedException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_プライマリ正常_セカンダリODataException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var exceptionResult = new List<JsonDocument>
                    {
                        new JsonDocument(new JObject
                        {
                            ["field"] = "value"
                        })
                    };

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_プライマリ正常_セカンダリODataInvalidFilterColumnException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            var mockRepository2 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer, new ODataInvalidFilterColumnException());
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object,mockRepository2.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);

            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_異常系_URLスキーマあり_必須エラー()
        {
            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            action.UriSchema = new DataSchema(@"
        {
          type: ""object"",
          properties: {
            id: { type: ""string"" }
          },
          required: [ ""id"" ]
        }
        ");

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            var mockRepository1 = CreateINewDynamicApiDataStoreRepositoryMock(null, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository1.Object
                    });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void ExecuteAction_MailTemplateあり()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());

            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            // テスト対象のインスタンスを設定
            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);
            action.HasMailTemplate = new HasMailTemplate(true);

            var exceptionResult = new List<JsonDocument>();
            var mockPrimaryRepository = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockPrimaryRepository.Object
                    });

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            // モックを検証
            mockRepository.Verify(x => x.Delete(It.Is<IDynamicApiAction>(a => a == action)), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Webhookあり()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());

            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            // テスト対象のインスタンスを設定
            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);
            action.HasWebhook = new HasWebhook(true);

            var exceptionResult = new List<JsonDocument>();

            var mockPrimaryRepository = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockPrimaryRepository.Object
                    });

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Delete(It.Is<IDynamicApiAction>(a => a == action)), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Webhookあり_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableWebHookAndMailTemplate", false);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());

            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            // テスト対象のインスタンスを設定
            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);
            action.HasWebhook = new HasWebhook(true);

            var exceptionResult = new List<JsonDocument>();

            var mockPrimaryRepository = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockPrimaryRepository.Object
                    });

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Register(It.IsAny<IDynamicApiAction>(), It.IsAny<JToken>()), Times.Never);
        }

        [TestMethod]
        public void ExecuteAction_Blockchainあり_履歴なし()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());

            // モックを作成
            var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
            mockBlockchainEventhub.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<string, RepositoryType, string>((id, type, version) =>
                {
                    blockchainEventList.Add(new KeyValuePair<string, string>(id, version));
                });
            blockchainEventList = new List<KeyValuePair<string, string>>();

            UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);


            string dataId = Guid.NewGuid().ToString();

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);


            // テスト対象のインスタンスを設定
            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);
            action.IsEnableBlockchain = new IsEnableBlockchain(true);
            var expectResult = new List<JsonDocument>
                    {
                        new JsonDocument(new JObject
                        {
                            ["id"] = dataId,
                            ["field"] = "value"
                        })
                    };

            var mockPrimaryRepository = CreateINewDynamicApiDataStoreRepositoryMock(expectResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockPrimaryRepository.Object
                    });

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockBlockchainEventhub.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<RepositoryType>(), It.IsAny<string>()), Times.Once);
            blockchainEventList.Count().Is(1);
            blockchainEventList.Single().Key.Is(dataId);
            blockchainEventList.Single().Value.IsNull();
        }

        [TestMethod]
        public void ExecuteAction_Blockchainあり_履歴あり()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());

            // モックを作成
            var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
            mockBlockchainEventhub.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<string, RepositoryType, string>((id, type, version) =>
                {
                    blockchainEventList.Add(new KeyValuePair<string, string>(id, version));
                });
            blockchainEventList = new List<KeyValuePair<string, string>>();

            UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);


            string dataId = Guid.NewGuid().ToString();

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            //履歴のモック
            var mockDocVerion = new Mock<IDocumentVersionRepository>();
            mockDocVerion.Setup(x => x.SaveDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<RepositoryKeyInfo>(), It.IsAny<bool>())).Returns(() =>
            {
                var ret = new DocumentDbDocumentVersions();
                ret.Id = dataId;
                ret.Etag = "hogeTag";
                ret.Partitionkey = "hogePart";
                ret.RegDate = DateTime.Now;
                ret.RegUserId = "hogeUser";
                ret.Type = "hogeType";
                ret.UpdDate = DateTime.Now;
                ret.UpdUserId = "hogeUser";
                ret.DocumentVersions = new List<DocumentDbDocumentVersion>()
                {
                    new DocumentDbDocumentVersion(){ VersionKey = VersionKey1, CreateDate = DateTime.Now.ToString()  }
                };
                var mappingConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DocumentHistory, DocumentDbDocumentVersion>().ReverseMap();
                    cfg.CreateMap<DocumentHistories, DocumentDbDocumentVersions>().ReverseMap();
                });
                IMapper m = new Mapper(mappingConfig);
                return m.Map<DocumentHistories>(ret);
            });
            UnityContainer.RegisterInstance<IDocumentVersionRepository>(mockDocVerion.Object);



            // テスト対象のインスタンスを設定
            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);
            action.IsEnableBlockchain = new IsEnableBlockchain(true);
            action.IsDocumentHistory = new IsDocumentHistory(true);
            action.HistoryEvacuationDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>().Object;
            var expectResult = new List<JsonDocument>
                    {
                        new JsonDocument(new JObject
                        {
                            ["id"] = dataId,
                            ["field"] = "value"
                        })
                    };

            var mockPrimaryRepository = CreateINewDynamicApiDataStoreRepositoryMock(expectResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockPrimaryRepository.Object
                    });
            mockPrimaryRepository.SetupProperty(x => x.DocumentVersionRepository, UnityContainer.Resolve<IDocumentVersionRepository>());

            // テスト対象のメソッドを実行
            var result = action.ExecuteAction();
            // モックを検証
            mockBlockchainEventhub.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<RepositoryType>(), It.IsAny<string>()), Times.Once);
            blockchainEventList.Count().Is(1);
            blockchainEventList.Single().Key.Is(dataId);
            blockchainEventList.Single().Value.Is(VersionKey1);

            //履歴ヘッダーを検証
            var historyHeader = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(result.Headers.GetValues("X-DocumentHistory").Single()).Single();
            historyHeader.documents.Single(x => x.documentKey == dataId && x.versionKey == VersionKey1);

        }


        [TestMethod]
        public void ExecuteAction_Base64()
        {

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            var json = CreateBase64AttachFileJson(CreateBase64Registed("file"));
            var exceptionResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(json)) };
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });


            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var resultcomp = JToken.Parse(CreateBase64AttachFileJson(CreateBase64Query(base64StringNormal)));
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void ExecuteAction_Base64_添付ファイル設定なし()
        {

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            action.IsEnableAttachFile = new IsEnableAttachFile(false);
            var json = CreateBase64AttachFileJson(CreateBase64Registed("file"));
            var exceptionResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(json)) };

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });

            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void ExecuteAction_Base64_添付ファイル設定Null()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data);

            action.IsEnableAttachFile = null;
            var json = CreateBase64AttachFileJson(CreateBase64Registed("file"));
            var exceptionResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(json)) };

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockRepository.Object });
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock(base64StringNormal);
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void DocumentHistory_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("$filter"), new QueryStringValue("hoge=hogehoge"));
            var action = CreateODataDeleteDataAction(data, true);

            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(defResult, ValueObjectUtil.Create<DeleteParam>(action), ValueObjectUtil.Create<QueryParam>(action), perRequestDataContainer);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockRepository.Object
                    });


            var mockHistoryDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            action.HistoryEvacuationDataStoreRepository = mockHistoryDataRepository.Object;

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Headers.Contains("X-DocumentHistory").IsFalse();
            mockHistoryDataRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Never);
        }

        private Mock<IDynamicApiAttachFileRepository> SetUpBase64AttachFileRepositoryMock(string json)
        {
            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.GetFiletoBase64String(It.IsAny<string>())).Returns(json);
            return mockBlobRepository;
        }

        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();

        private ODataDeleteAction CreateODataDeleteDataAction(Dictionary<QueryStringKey, QueryStringValue> odataQueryString, bool isHistoryTest = false)
        {
            ODataDeleteAction action = UnityCore.Resolve<ODataDeleteAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.ProviderSystemId = new SystemId(SystemId.ToString());
            action.ProviderVendorId = new VendorId(VendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{Id}");
            action.ControllerRelativeUrl = new ControllerUrl("/API/Private/QueryTest");
            action.ActionType = new ActionTypeVO(ActionType.ODataDelete);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.Accept = _defaultAccept;
            action.Query = new QueryStringVO(odataQueryString);
            action.IsEnableBlockchain = new IsEnableBlockchain(false);
            if (isHistoryTest)
            {
                action.IsDocumentHistory = new IsDocumentHistory(true);
                action.HistoryEvacuationDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>().Object;
            }
            return action;
        }

        private static string base64StringNormal = "";
        private List<KeyValuePair<string, string>> blockchainEventList;
        private string VersionKey1 = Guid.NewGuid().ToString();

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
'id':'fuga',
'hoge':'hoge',
'AreaUnitCode': 'あああああああああああああああああああああああああああ',
'file':'{base64attachfile}'
}}";
        }
        private static string CreateBase64AttachFileJson2(string base64attachfile, string base64attachfile2)
        {
            return $@"{{
'id':'fuga',
'hoge':'hoge',
'AreaUnitCode': 'あああああああああああああああああああああああああああ',
'file':'{base64attachfile}',
'file2':'{base64attachfile2}'
}}";
        }

    }
}
