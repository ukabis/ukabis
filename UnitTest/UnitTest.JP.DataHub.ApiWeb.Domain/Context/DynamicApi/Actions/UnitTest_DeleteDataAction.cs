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
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.DDD;
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
    [TestClass]
    public class UnitTest_DeleteDataAction : UnitTestBase
    {
        private static Accept defaultAccept = new Accept("*/*");
        private static string dataId = Guid.NewGuid().ToString();

        private const int WaitTimes = 3;
        private const int WaitMs = 1000;

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance(Configuration);
            UnityContainer.RegisterInstance(new Mock<ICache>().Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>("multiThread", perRequestDataContainer);
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_削除有()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            DeleteParam deleteParam = ValueObjectUtil.Create<DeleteParam>(action);
            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            mockRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");
            perRequestDataContainer.XgetInternalAllField.IsFalse();

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_HistoryHeaderCheck()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateDeleteDataAction(isHistoryTest: true);
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
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");
            perRequestDataContainer.XgetInternalAllField.IsTrue();

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));

            var header = result.Headers.Count(x => x.Key == "X-DocumentHistory");
            header.Is(1);

            //履歴ヘッダの中身チェック
            var headers = result.Headers.GetValues("X-DocumentHistory").First().ToJson();
            headers.Is($"[{{'isSelfHistory':true,'resourcePath':'/API/Private/QueryTest',documents:[{{'documentKey':'aaa','versionKey':null}}]}}]".ToJson());
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_削除無()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            DeleteParam deleteParam = ValueObjectUtil.Create<DeleteParam>(action);
            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { });
            mockRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10421.ToString(), actualMessage["error_code"]);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_NotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            DeleteParam deleteParam = ValueObjectUtil.Create<DeleteParam>(action);
            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            mockRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>())).Throws(new NotImplementedException()); ;

            AssertEx.Catch<NotImplementedException>(() => action.ExecuteAction());

            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリキー無し()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateDeleteDataAction();
            action.RepositoryKey = new RepositoryKey(null);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10420.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_キャッシュ有()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.RemoveFirstMatch(It.IsAny<string>()));
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.CacheInfo = new CacheInfo(true, 0, "cacheKey");

            var keyRepository = "keyRepository";
            mockRepository.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>()))
                .Returns(keyRepository);

            DeleteParam deleteParam = ValueObjectUtil.Create<DeleteParam>(action);
            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            mockRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");

            mockRepository.Verify(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>()), Times.Exactly(0));
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockCache.Verify(x => x.RemoveFirstMatch(It.Is<string>(it =>
                it == CacheManager.CreateKey(
                    "DynamicApiAction",
                    VendorId,
                    SystemId,
                    ControllerId)
            )));
        }

        private void ExecuteAction_リポジトリ2つ_Common(
            List<string> deleteDataResults,
            HttpStatusCode expectStatusCode,
            string expectContent,
            string expectIncludeContent = ""
        )
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object,
            });
            DeleteParam deleteParam = ValueObjectUtil.Create<DeleteParam>(action, JToken.Parse("{\"id\":\"aaa\"}"));
            if (deleteDataResults[0] == null)
            {
                mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { });
            }
            else
            {
                mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse(deleteDataResults[0])) });
            }
            if (deleteDataResults[1] == null)
            {
                mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { });
            }
            else
            {
                mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse(deleteDataResults[1])) });
            }

            mockRepository1.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });
            mockRepository2.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            var result = action.ExecuteAction();
            result.StatusCode.Is(expectStatusCode);

            if (string.IsNullOrEmpty(expectIncludeContent))
            {
                result.Content.ReadAsStringAsync().Result.Is(expectContent);
            }
            else
            {
                result.Content.ReadAsStringAsync().Result.Contains(expectIncludeContent).Is(true);
            }


            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(deleteDataResults[0] == null ? 0 : 1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(deleteDataResults[1] == null ? 0 : 1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_両方削除有()
        {
            ExecuteAction_リポジトリ2つ_Common(
                new List<string> { "{\"id\":\"aaa\"}", "{\"id\":\"bbb\"}" },
                HttpStatusCode.NoContent,
                "");
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_片方データ無()
        {
            ExecuteAction_リポジトリ2つ_Common(
                new List<string> { "{\"id\":\"aaa\"}", null },
                HttpStatusCode.NoContent,
                "");
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_両方データ無()
        {
            ExecuteAction_リポジトリ2つ_Common(
                new List<string> { null, null },
                HttpStatusCode.NotFound,
                string.Empty, "E10421");
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_片方NotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object,
            });

            mockRepository1.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Throws(new NotImplementedException());
            mockRepository2.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()));

            mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");

            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_両方NotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object,
            });
            mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });

            mockRepository1.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Throws(new NotImplementedException());
            mockRepository2.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Throws(new NotImplementedException());

            AssertEx.Catch<AggregateException>(() => action.ExecuteAction());

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public async Task ExecuteAction_リポジトリ2つ_キャッシュ有()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            var callCount = 0;
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.RemoveFirstMatch(It.IsAny<string>())).Callback<string>(x => callCount++);
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object,
            });
            action.CacheInfo = new CacheInfo(true, 0, "cacheKey");

            var keyRepository1 = "keyRepository1";
            var keyRepository2 = "keyRepository2";
            mockRepository1.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });
            mockRepository2.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });
            mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });

            mockRepository1.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository1);
            mockRepository2.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository2);


            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));

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
        public async Task ExecuteAction_リポジトリ2つ_キャッシュ有_キャッシュ削除エラー_NotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            var callCount = 0;
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.RemoveFirstMatch(It.IsAny<string>()))
                .Callback<string>(x => callCount++)
                .Throws(new NotImplementedException());
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object,
            });
            action.CacheInfo = new CacheInfo(true, 0, "cacheKey");

            var keyRepository1 = "keyRepository1";
            var keyRepository2 = "keyRepository2";
            mockRepository1.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });
            mockRepository2.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });
            mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });

            mockRepository1.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository1);
            mockRepository2.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository2);


            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));

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
        public async Task ExecuteAction_リポジトリ2つ_キャッシュ有_キャッシュ削除エラー_Exception()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            var callCount = 0;
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.RemoveFirstMatch(It.IsAny<string>()))
                .Callback<string>(x => callCount++)
                .Throws(new Exception());
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object,
            });
            action.CacheInfo = new CacheInfo(true, 0, "cacheKey");

            var keyRepository1 = "keyRepository1";
            var keyRepository2 = "keyRepository2";
            mockRepository1.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });
            mockRepository2.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });
            mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });

            mockRepository1.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository1);
            mockRepository2.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository2);


            action.ExecuteAction(); // Exception投げられない

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));

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
        public void ExecuteAction_Get()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateDeleteDataAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            DeleteParam deleteParam = ValueObjectUtil.Create<DeleteParam>(action, JToken.Parse("{\"id\":\"aaa\"}"));
            mockRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(deleteParam);
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Post()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateDeleteDataAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            DeleteParam deleteParam = ValueObjectUtil.Create<DeleteParam>(action, JToken.Parse("{\"id\":\"aaa\"}"));
            mockRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(deleteParam);
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Put()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateDeleteDataAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PUT);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });
            DeleteParam deleteParam = ValueObjectUtil.Create<DeleteParam>(action, JToken.Parse("{\"id\":\"aaa\"}"));
            mockRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()))
                .Callback<DeleteParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(deleteParam);
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Patch()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateDeleteDataAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PATCH);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10419.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_異常系_URLスキーマあり_必須エラー()
        {
            var action = CreateDeleteDataAction();

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
        public void ExecuteAction_MailTemplateあり()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());

            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);
            var mockPrimaryRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockPrimaryRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()));
            mockPrimaryRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });

            // テスト対象のインスタンスを設定
            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockPrimaryRepository.Object
            });
            action.HasMailTemplate = new HasMailTemplate(true);

            // テスト対象のメソッドを実行
            action.ExecuteAction();
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
            var mockPrimaryRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockPrimaryRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()));
            mockPrimaryRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });

            // テスト対象のインスタンスを設定
            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockPrimaryRepository.Object
            });
            action.HasWebhook = new HasWebhook(true);

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
            var mockPrimaryRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockPrimaryRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>()));
            mockPrimaryRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"aaa\"}")) });

            // テスト対象のインスタンスを設定
            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockPrimaryRepository.Object
            });
            action.HasWebhook = new HasWebhook(true);

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Delete(It.IsAny<IDynamicApiAction>()), Times.Never);
        }

        [TestMethod]
        public void ExecuteAction_Blockchainあり_履歴なし()
        {
            var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
            mockBlockchainEventhub.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<string, RepositoryType, string>((id, type, version) =>
                {
                    blockchainEventList.Add(new KeyValuePair<string, string>(id, version));
                });
            blockchainEventList = new List<KeyValuePair<string, string>>();

            UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);


            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());

            // モックを作成
            var mockPrimaryRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockPrimaryRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>())).Callback<DeleteParam>(p => p.CallbackDelete?.Invoke(p.Json.RemoveTokenToJson(XGetInnerAllField: true).ToString().ToJson(), RepositoryType.CosmosDB));
            mockPrimaryRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse("{\"id\":\"" + dataId + "\"}")) });
            mockPrimaryRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));

            // テスト対象のインスタンスを設定
            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockPrimaryRepository.Object
            });
            action.IsEnableBlockchain = new IsEnableBlockchain(true);

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
            var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
            mockBlockchainEventhub.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<string, RepositoryType, string>((id, type, version) =>
                {
                    blockchainEventList.Add(new KeyValuePair<string, string>(id, version));
                });
            blockchainEventList = new List<KeyValuePair<string, string>>();

            UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);

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


            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());

            // モックを作成
            var mockPrimaryRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockPrimaryRepository.Setup(x => x.DeleteOnce(It.IsAny<DeleteParam>())).Callback<DeleteParam>(p => p.CallbackDelete?.Invoke(p.Json.RemoveTokenToJson(XGetInnerAllField: true).ToString().ToJson(), RepositoryType.CosmosDB));
            mockPrimaryRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse($@"{{'id':'{dataId}', 'Item1' : 'hoge'}}")) });
            mockPrimaryRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockPrimaryRepository.SetupProperty(x => x.DocumentVersionRepository, UnityContainer.Resolve<IDocumentVersionRepository>());

            // テスト対象のインスタンスを設定
            var action = CreateDeleteDataAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockPrimaryRepository.Object
            });
            action.IsEnableBlockchain = new IsEnableBlockchain(true);
            action.IsDocumentHistory = new IsDocumentHistory(true);
            action.HistoryEvacuationDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>().Object;

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


        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();
        private string VersionKey1 = Guid.NewGuid().ToString();
        private List<KeyValuePair<string, string>> blockchainEventList;

        private DeleteDataAction CreateDeleteDataAction(bool isHistoryTest = false)
        {
            DeleteDataAction action = UnityCore.Resolve<DeleteDataAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.ProviderSystemId = new SystemId(SystemId.ToString());
            action.ProviderVendorId = new VendorId(VendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{Id}");
            action.ControllerRelativeUrl = new ControllerUrl("/API/Private/QueryTest");
            action.ActionType = new ActionTypeVO(ActionType.DeleteData);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.Accept = defaultAccept;
            action.IsEnableBlockchain = new IsEnableBlockchain(false);
            if (isHistoryTest)
            {
                action.IsDocumentHistory = new IsDocumentHistory(true);
                action.HistoryEvacuationDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>().Object;
            }
            return action;
        }
    }
}
