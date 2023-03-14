using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_UpdateAction : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>("multiThread", perRequestDataContainer);

            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", true);
            UnityContainer.RegisterInstance<bool>("Return.JsonValidator.ErrorDetail", true);
            UnityContainer.RegisterInstance(new Mock<ICache>().Object);

            UnityContainer.RegisterType<IHttpContextAccessor, HttpContextAccessor>();
        }

        [TestMethod]
        public void UpdateTest()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true
            };

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            getResult.RemoveTokenToJson(false);

            var mockGetReturn = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn.BeginData();
            foreach (var str in getResult.RemoveTokenToJson(false))
            {
                mockGetReturn.AddString(str);
            }
            mockGetReturn.EndData();

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateUpdateAction();
            mockDynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockDynamicApiDataStoreRepository.Object });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NoContent);

            mockDynamicApiDataStoreRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void UpdateTest_正常系_スキーマあり()
        {
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "123",
                Number = 1,
                Is = true,
            };

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));

            var action = CreateUpdateAction();

            action.RequestSchema = new DataSchema(testSchema);
            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(
                new List<INewDynamicApiDataStoreRepository>
                {
                    mockDynamicApiDataStoreRepository.Object
                });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NoContent);

        }

        [TestMethod]
        public void UpdateTest_異常系_スキーマあり_型不一致()
        {
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "123",
                Number = 1,
                Is = true,
            };

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);

            var action = CreateUpdateAction();

            action.RequestSchema = new DataSchema(testSchema2);
            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(
                new List<INewDynamicApiDataStoreRepository>
                {
                            mockDynamicApiDataStoreRepository.Object
                });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void UpdateTest_異常系_スキーマあり_桁数オーバー()
        {
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "123123123123123123123",
                Number = 1,
                Is = true,
            };

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);

            var action = CreateUpdateAction();

            action.RequestSchema = new DataSchema(testSchema);
            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(
                new List<INewDynamicApiDataStoreRepository>
                {
                                        mockDynamicApiDataStoreRepository.Object
                });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);

        }
        [TestMethod]
        public void UpdateTest_異常系_スキーマあり_必須エラー()
        {
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = null,
                Number = 1,
                Is = true,
            };

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);

            var action = CreateUpdateAction();

            action.RequestSchema = new DataSchema(testSchema);
            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(
    new List<INewDynamicApiDataStoreRepository>
    {
                            mockDynamicApiDataStoreRepository.Object
    });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void UpdateTest_異常系_配列エラー()
        {
            var baseData = new List<BaseData>
                    {
                        new BaseData()
                        {
                            Id = "AAAAA",
                            Name = null,
                            Number = 1,
                            Is = true,
                        }
                    };
            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);

            var action = CreateUpdateAction();

            action.RequestSchema = new DataSchema(testSchema);
            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(
                new List<INewDynamicApiDataStoreRepository>
                {
                            mockDynamicApiDataStoreRepository.Object
                });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10409.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void UpdateTest_異常系_URLスキーマあり_必須エラー()
        {
            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()));

            var action = CreateUpdateAction();

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
        public void UpdateTest_NotFound()
        {
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true
            };

            List<JsonDocument> getResult = new List<JsonDocument>() { };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);

            var action = CreateUpdateAction();

            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockDynamicApiDataStoreRepository.Object });
            var result = action.ExecuteAction();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10407.ToString(), actualMessage["error_code"]);
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NotFound);

        }

        string AABBRepositoryConfirm = @"{
          'Id' : 'AAAAA',
          'Name' : 'BBBB',
          'Number' : 1,
          'Is' : true,
          '_Regdate' : '{{*}}',
          '_Reguser_Id' : '{{*}}',
          '_Upddate' : '{{*}}',
          '_Upduser_Id' : '{{*}}'
        }";

        [TestMethod]
        public void UpdateTest_リポジトリ_NotImplementException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true
            };

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateUpdateAction();
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                }).Throws(new NotImplementedException());


            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockDynamicApiDataStoreRepository.Object });

            AssertEx.Catch<NotImplementedException>(() => action.ExecuteAction());
            mockDynamicApiDataStoreRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void UpdateTest_リポジトリ2個()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true
            };
            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockDynamicApiDataStoreRepository2 = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository1.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockDynamicApiDataStoreRepository2.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            var action = CreateUpdateAction();

            mockDynamicApiDataStoreRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository1.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            mockDynamicApiDataStoreRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository2.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockDynamicApiDataStoreRepository1.Object,
                        mockDynamicApiDataStoreRepository2.Object
                    });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NoContent);

            mockDynamicApiDataStoreRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository1.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(0));
            mockDynamicApiDataStoreRepository2.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));

        }

        [TestMethod]
        public void UpdateTest_キャッシュ有()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true
            };
            var confirm = @"{
          'Id' : 'AAAAA',
          'Name' : 'BBBB',
          'Number' : 1,
          'Is' : true,
          '_Regdate' : '{{*}}',
          '_Reguser_Id' : '{{*}}',
          '_Upddate' : '{{*}}',
          '_Upduser_Id' : '{{*}}'
        }".ToJson();

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            getResult.RemoveTokenToJson(false);

            var keyRepository = "keyRepository";
            var cacheMinute = 30;
            var expectCacheSecond = cacheMinute * 60;
            var expectKeyCache = CacheManager.CreateKey(
                "DynamicApiAction",
                ProviderVendorId,
                ProviderSystemId,
                ControllerId);

            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.RemoveFirstMatch(It.IsAny<string>()))
                .Callback<string>((keyCache) =>
                {
                    keyCache.Is(expectKeyCache);
                });
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var action = CreateUpdateAction();
            action.CacheInfo = new CacheInfo(true, cacheMinute, "KeyCache");

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository);
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });
            mockDynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockDynamicApiDataStoreRepository.Object });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NoContent);

            mockDynamicApiDataStoreRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
            mockCache.Verify(x => x.RemoveFirstMatch(It.IsAny<string>()), Times.Between(0, 1, Moq.Range.Inclusive));
        }


        [TestMethod]
        public void UpdateTest_JSonSchemaエラー()
        {
            var contents = $@"{{'a1':10000, 'a2':'hoge', 'a3':'fuga'}}";
            var model = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a1': {
              'title': 'a1',
              'type': ['string','null']
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 1,
              'enum': [ 'hoghogehogheo', null],
              'type': 'string'
            }
          },
          'required':['a0','a4']
        }";

            var action = CreateUpdateAction(definitionContents: contents, requestModel: model);


            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(contents))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockDynamicApiDataStoreRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository1.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            mockDynamicApiDataStoreRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository2.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockDynamicApiDataStoreRepository1.Object,
                        mockDynamicApiDataStoreRepository2.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            //メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(4);
            var chk = msg["errors"]["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = msg["errors"]["a2"].ToList();
            chk.Count.Is(2);
            chk[0].ToString().Is("String 'hoge' exceeds maximum length of 1.(code:4)");
            chk[1].ToString().Is("Value \"hoge\" is not defined in enum.(code:17)");
            chk = msg["errors"]["a3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            chk = msg["errors"]["a0,a4"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void UpdateTest_Bodyが空()
        {
            var contents = $@"";
            var model = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a1': {
              'title': 'a1',
              'type': ['string','null']
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 1,
              'enum': [ 'hoghogehogheo', null],
              'type': 'string'
            }
          },
          'required':['a0','a4']
        }";

            var action = CreateUpdateAction(definitionContents: contents, requestModel: model);


            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(contents))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockDynamicApiDataStoreRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository1.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            mockDynamicApiDataStoreRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository2.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockDynamicApiDataStoreRepository1.Object,
                        mockDynamicApiDataStoreRepository2.Object
                    });

            var result = action.ExecuteAction();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10406.ToString(), actualMessage["error_code"]);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
        }

        [TestMethod]
        public void UpdateTest_BodyがArray()
        {
            var contents = $@"[]";
            var model = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a1': {
              'title': 'a1',
              'type': ['string','null']
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 1,
              'enum': [ 'hoghogehogheo', null],
              'type': 'string'
            }
          },
          'required':['a0','a4']
        }";

            var action = CreateUpdateAction(definitionContents: contents, requestModel: model);


            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(contents))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockDynamicApiDataStoreRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository1.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            mockDynamicApiDataStoreRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository2.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockDynamicApiDataStoreRepository1.Object,
                        mockDynamicApiDataStoreRepository2.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10409.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void UpdateTest_DataNotFound()
        {
            var contents = $@"[]";
            var model = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a1': {
              'title': 'a1',
              'type': ['string','null']
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 1,
              'enum': [ 'hoghogehogheo', null],
              'type': 'string'
            }
          },
          'required':['a0','a4']
        }";

            var action = CreateUpdateAction(definitionContents: contents, requestModel: model);


            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            List<JsonDocument> getResult = new List<JsonDocument>();

            var mockDynamicApiDataStoreRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockDynamicApiDataStoreRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository1.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            mockDynamicApiDataStoreRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository2.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockDynamicApiDataStoreRepository1.Object,
                        mockDynamicApiDataStoreRepository2.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);
            //レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10407.ToString());
        }

        public void UpdateTest_JSonSchemaエラー_XML()
        {
            var contents = $@"{{'a1':10000, 'a2':'hoge', 'a3':'fuga'}}";
            var model = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a1': {
              'title': 'a1',
              'type': ['string','null']
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 1,
              'enum': [ 'hoghogehogheo', null],
              'type': 'string'
            }
          },
          'required':['a0','a4']
        }";

            var action = CreateUpdateAction(definitionContents: contents, requestModel: model, "application/xml");


            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(contents))) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockDynamicApiDataStoreRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            mockDynamicApiDataStoreRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository1.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            mockDynamicApiDataStoreRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository2.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockDynamicApiDataStoreRepository1.Object,
                        mockDynamicApiDataStoreRepository2.Object
                    });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //レスポンスのコンテントタイプ は、application/problem+xml
            result.Content.Headers.ContentType.MediaType.Is("application/problem+xml");
            var msg = result.Content.ReadAsStringAsync().Result.StringToXml();
            msg.Element("title").Value.Is("one or more Json Validation errors occurred.");
            var chk = msg.Element("errors").Elements("a1").ToList();
            chk.Count.Is(1);
            chk[0].Value.Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = msg.Element("errors").Elements("a2").ToList();
            chk.Count.Is(2);
            chk[0].Value.Is("String 'hoge' exceeds maximum length of 1.(code:4)");
            chk[1].Value.Is(@"Value ""hoge"" is not defined in enum.(code:17)");

            chk = msg.Element("errors").Elements("a3").ToList();
            chk.Count.Is(1);
            chk[0].Value.Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");

            chk = msg.Element("errors").Elements("a0_x002C_a4").ToList();
            chk.Count.Is(1);
            chk[0].Value.Is("Required properties are missing from object: a0, a4.(14)");
        }

        [TestMethod]
        public void UpdateTest_リポジトリキー無し()
        {
            var action = CreateUpdateAction();
            action.RepositoryKey = new RepositoryKey(null);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Is("");
        }

        [TestMethod]
        public void UpdateTest_Get()
        {
            var action = CreateUpdateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Is("");
        }

        [TestMethod]
        public void UpdateTest_Post()
        {
            var action = CreateUpdateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Is("");
        }

        [TestMethod]
        public void UpdateTest_Put()
        {
            var action = CreateUpdateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PUT);

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Is("");
        }

        [TestMethod]
        public void UpdateTest_Delete()
        {
            var action = CreateUpdateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE);

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.Is("");
        }

        [TestMethod]
        public void GetRepositoryData_リポジトリ1つ()
        {
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true
            };

            List<JsonDocument> getResult = new List<JsonDocument>() {
                        new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            var mockGetReturn = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn.BeginData();
            foreach (var str in getResult.RemoveTokenToJson(false))
            {
                mockGetReturn.AddString(str);
            }
            mockGetReturn.EndData();

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);

            var action = CreateUpdateAction();
            var result = (JsonSearchResult)action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockDynamicApiDataStoreRepository.Object
                }) });

            //result.Value.Is(mockGetReturn.Value);
            result.Count.Is(mockGetReturn.Count);
            result.JToken.IsStructuralEqual(mockGetReturn.JToken);

        }

        [TestMethod]
        public void GetRepositoryData_リポジトリ2つ_1つ目でヒット()
        {
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true
            };

            List<JsonDocument> getResult1 = new List<JsonDocument>() {
                        new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            var mockGetReturn1 = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn1.BeginData();
            foreach (var str in getResult1.RemoveTokenToJson(false))
            {
                mockGetReturn1.AddString(str);
            }
            mockGetReturn1.EndData();

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult1);

            List<JsonDocument> getResult2 = new List<JsonDocument>() {
                        new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            var mockGetReturn2 = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn2.BeginData();
            foreach (var str in getResult2.RemoveTokenToJson(false))
            {
                mockGetReturn2.AddString(str);
            }
            mockGetReturn2.EndData();

            var mockDynamicApiDataStoreRepository2 = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult2);

            var action = CreateUpdateAction();
            var result = (JsonSearchResult)action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockDynamicApiDataStoreRepository.Object,
                    mockDynamicApiDataStoreRepository2.Object
                }) });

            result.Count.Is(mockGetReturn1.Count);
            result.JToken.IsStructuralEqual(mockGetReturn1.JToken);

        }

        [TestMethod]
        public void GetRepositoryData_リポジトリ2つ_2つ目でヒット()
        {
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true
            };

            List<JsonDocument> getResult1 = new List<JsonDocument>() { };
            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockGetReturn1 = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn1.BeginData();
            foreach (var str in getResult1.RemoveTokenToJson(false))
            {
                mockGetReturn1.AddString(str);
            }
            mockGetReturn1.EndData();

            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult1);

            List<JsonDocument> getResult2 = new List<JsonDocument>() {
                        new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            var mockGetReturn2 = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn2.BeginData();
            foreach (var str in getResult2.RemoveTokenToJson(false))
            {
                mockGetReturn2.AddString(str);
            }
            mockGetReturn2.EndData();

            var mockDynamicApiDataStoreRepository2 = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult2);

            var action = CreateUpdateAction();
            var result = (JsonSearchResult)action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockDynamicApiDataStoreRepository.Object,
                    mockDynamicApiDataStoreRepository2.Object
                }) });

            result.Count.Is(mockGetReturn2.Count);
            result.JToken.IsStructuralEqual(mockGetReturn2.JToken);
        }

        [TestMethod]
        public void GetRepositoryData_NotImplementException()
        {
            var getResult1 = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            getResult1.BeginData();
            getResult1.EndData();
            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Throws(new NotImplementedException());

            var action = CreateUpdateAction();
            var result = (JsonSearchResult)action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                            mockDynamicApiDataStoreRepository.Object
                }) });

            result.Count.Is(getResult1.Count);
            result.JToken.IsNull();
        }

        [TestMethod]
        public void JToken_Merge()
        {
            var targetData = new
            {
                id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true,
                _Reguser_Id = "Reguser_Id",
                _Regdate = "Regdate",
                _Version = "Version",
                _partitionkey = "partitionkey",
                _Type = "Type",
                _rid = "rid",
                _self = "self",
                _etag = "etag",
                _attachments = "attachments",
                _ts = "ts"
            };
            var sourceData = new
            {
                Name = "GGGGG",
            };

            var resultData = new
            {
                id = "AAAAA",
                Name = "GGGGG",
                Number = 1,
                Is = true,
                _Reguser_Id = "Reguser_Id",
                _Regdate = "Regdate",
                _Version = "Version",
                _partitionkey = "partitionkey",
                _Type = "Type",
            };

            var action = CreateUpdateAction();
            JToken target = JToken.Parse(JsonConvert.SerializeObject(targetData));
            JToken source = JToken.Parse(JsonConvert.SerializeObject(sourceData));
            var result = (Tuple<JToken, JToken>)action.GetType().InvokeMember("MergeJson", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { target, source });

            result.Item1.Is(JToken.Parse(JsonConvert.SerializeObject(resultData)));
            result.Item2.IsNull();
        }

        [TestMethod]
        public void JToken_MergeInternal()
        {
            var targetData = new
            {
                id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true,
                _Reguser_Id = "Reguser_Id",
                _Regdate = "Regdate",
                _Version = "Version",
                _partitionkey = "partitionkey",
                _Type = "Type",
                _rid = "rid",
                _self = "self",
                _etag = "etag",
                _attachments = "attachments",
                _ts = "ts"
            };
            var sourceData = $"{{ '{AbstractDynamicApiAction.INTERNAL_UPDATE}' : '{AbstractDynamicApiAction.INTERNAL_UPDATE}', 'Name' : 'GGGGG' }}";

            var resultData = new
            {
                id = "AAAAA",
                Name = "GGGGG",
                Number = 1,
                Is = true,
                _Reguser_Id = "Reguser_Id",
                _Regdate = "Regdate",
                _Version = "Version",
                _partitionkey = "partitionkey",
                _Type = "Type",
            };
            var expectDiff = "{ 'Name' : 'BBBB' }";

            var action = CreateUpdateAction();
            JToken target = JToken.Parse(JsonConvert.SerializeObject(targetData));
            JToken source = sourceData.ToJson();
            var result = (Tuple<JToken, JToken>)action.GetType().InvokeMember("MergeJson", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { target, source });

            result.Item1.Is(JToken.Parse(JsonConvert.SerializeObject(resultData)));
            result.Item2.Is(expectDiff.ToJson());
        }

        [TestMethod]
        //システム項目は更新されないことの確認
        public void JToken_Merge_SystemKeyUpdate()
        {
            var targetData = new
            {
                id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true,
                _Reguser_Id = "Reguser_Id",
                _Regdate = "Regdate",
                _Version = "Version",
                _partitionkey = "partitionkey",
                _Type = "Type",
                _rid = "rid",
                _self = "self",
                _etag = "etag",
                _attachments = "attachments",
                _ts = "ts"
            };
            var sourceData = new
            {
                Name = "GGGGG",
                _Reguser_Id = "AA",
                _Regdate = "BB",
                _Version = "CC",
                _partitionkey = "DD",
                _Type = "FF",
                _rid = "GG",
                _self = "HHH",
                _etag = "FFFF",
                _attachments = "JJJ",
                _ts = "KKK"

            };

            var resultData = new
            {
                id = "AAAAA",
                Name = "GGGGG",
                Number = 1,
                Is = true,
                _Reguser_Id = "AA",
                _Regdate = "BB",
                _Version = "CC",
                _partitionkey = "DD",
                _Type = "FF",
            };

            var action = CreateUpdateAction();
            JToken target = JToken.Parse(JsonConvert.SerializeObject(targetData));
            JToken source = JToken.Parse(JsonConvert.SerializeObject(sourceData));
            var result = (Tuple<JToken, JToken>)action.GetType().InvokeMember("MergeJson", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { target, source });

            result.Item1.IsStructuralEqual(JToken.Parse(JsonConvert.SerializeObject(resultData)));
            result.Item2.IsNull();
        }

        [TestMethod]
        public void JToken_Merge_ArrayAppend()
        {
            var targetData = new
            {
                id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true,
            };
            var sourceData = new
            {
                Name = "GGGGG",
                ArrayObject = new[] { new { aa = "aa", bb = "bb" }, new { aa = "aa", bb = "bb" } },
            };

            var resultData = new
            {
                id = "AAAAA",
                Name = "GGGGG",
                Number = 1,
                Is = true,
                ArrayObject = new[] { new { aa = "aa", bb = "bb" }, new { aa = "cc", bb = "dd" } },

            };

            var action = CreateUpdateAction();
            JToken target = JToken.Parse(JsonConvert.SerializeObject(targetData));
            JToken source = JToken.Parse(JsonConvert.SerializeObject(sourceData));
            var result = (Tuple<JToken, JToken>)action.GetType().InvokeMember("MergeJson", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { target, source });

            result.Item1.IsStructuralEqual(JToken.Parse(JsonConvert.SerializeObject(resultData)));
            result.Item2.IsNull();
        }

        [TestMethod]
        public void JToken_Merge_ArrayUpdate()
        {
            var targetData = new
            {
                id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true,
                ArrayObject = new[] { new { aa = "1", bb = "2" } },

            };
            var sourceData = new
            {
                Name = "GGGGG",
                ArrayObject = new[] { new { aa = "aa", bb = "bb" }, new { aa = "aa", bb = "bb" } },
            };

            var resultData = new
            {
                id = "AAAAA",
                Name = "GGGGG",
                Number = 1,
                Is = true,
                ArrayObject = new[] { new { aa = "aa", bb = "bb" }, new { aa = "cc", bb = "dd" } },
            };

            var action = CreateUpdateAction();
            JToken target = JToken.Parse(JsonConvert.SerializeObject(targetData));
            JToken source = JToken.Parse(JsonConvert.SerializeObject(sourceData));
            var result = (Tuple<JToken, JToken>)action.GetType().InvokeMember("MergeJson", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { target, source });

            result.Item1.IsStructuralEqual(JToken.Parse(JsonConvert.SerializeObject(resultData)));
        }

        [TestMethod]
        public void JToken_Merge_ObjectUpdate()
        {
            var targetData = new
            {
                id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true,
                KObject = new
                {
                    ID = "AA",
                    NAME = "BB",
                    MObject = new
                    {
                        MID = "MM",
                        MNAME = "MNAME"
                    }
                }

            };
            var sourceData = new
            {
                Name = "GGGGG",
                ArrayObject = new[] { new { aa = "aa", bb = "bb" }, new { aa = "aa", bb = "bb" } },
                KObject = new
                {
                    ID = "aa",
                    MObject = new
                    {
                        MID = "MM",
                        MNAME = "MNAME",
                        HOGE = "POGE"
                    }
                }
            };

            var resultData = new
            {
                id = "AAAAA",
                Name = "GGGGG",
                Number = 1,
                Is = true,
                ArrayObject = new[] { new { aa = "aa", bb = "bb" }, new { aa = "cc", bb = "dd" } },
                KObject = new
                {
                    ID = "aa",
                    NAME = "BB",
                    MObject = new
                    {
                        MID = "MM",
                        MNAME = "MNAME",
                        HOGE = "POGE"
                    }
                }
            };

            var action = CreateUpdateAction();
            JToken target = JToken.Parse(JsonConvert.SerializeObject(targetData));
            JToken source = JToken.Parse(JsonConvert.SerializeObject(sourceData));
            var result = (Tuple<JToken, JToken>)action.GetType().InvokeMember("MergeJson", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { target, source });

            SortJson(result.Item1);
            var ans = JObject.Parse(JsonConvert.SerializeObject(resultData));
            SortJson(ans);

            result.Item1.IsStructuralEqual(ans);
            result.Item2.IsNull();
        }

        [TestMethod]
        public void JToken_Merge_NullUpdate()
        {
            var targetData = new BaseData()
            {
                Id = "AA",
                Name = "",
                Number = 0
            };
            var sourceData = new BaseData()
            {
                Id = "AA",
                Name = null,
                Number = 0
            };


            var resultData = new BaseData()
            {
                Id = "AA",
                Name = null,
                Number = 0
            };

            var action = CreateUpdateAction();
            JToken target = JToken.Parse(JsonConvert.SerializeObject(targetData));
            JToken source = JToken.Parse(JsonConvert.SerializeObject(sourceData));
            var result = (Tuple<JToken, JToken>)action.GetType().InvokeMember("MergeJson", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { target, source });

            SortJson(result.Item1);
            var ans = JObject.Parse(JsonConvert.SerializeObject(resultData));
            SortJson(ans);

            result.Item1.IsStructuralEqual(ans);
            result.Item2.IsNull();
        }

        [TestMethod]
        public void UpdateTest_additionalProperties_false_Badrequest()
        {
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "123",
                Number = 1,
                Is = true,
            };

            List<JsonDocument> getResult = new List<JsonDocument>() {
                        new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            var mockGetReturn = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn.BeginData();
            foreach (var str in getResult.RemoveTokenToJson(false))
            {
                mockGetReturn.AddString(str);
            }
            mockGetReturn.EndData();

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            // mockDynamicApiDataStoreRepository.Setup(x => x.GetData(It.IsAny<UpdateAction>(), It.Is<HasSingleData>(param => param.Value == false))).Returns(mockGetReturn);
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);

            var action = CreateUpdateAction();

            action.RequestSchema = new DataSchema(testSchema4);
            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(
                new List<INewDynamicApiDataStoreRepository>
                {
                            mockDynamicApiDataStoreRepository.Object
                });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);

        }
        [TestMethod]
        public void UpdateTest_additionalProperties_false_Success()
        {
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "123",
                Number = 1,
                Is = true,
            };

            List<JsonDocument> getResult = new List<JsonDocument>() {
                        new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            var mockGetReturn = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn.BeginData();
            foreach (var str in getResult.RemoveTokenToJson(false))
            {
                mockGetReturn.AddString(str);
            }
            mockGetReturn.EndData();

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            //            mockDynamicApiDataStoreRepository.Setup(x => x.GetData(It.IsAny<UpdateAction>(), It.Is<HasSingleData>(param => param.Value == false))).Returns(mockGetReturn);
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            var action = CreateUpdateAction();

            action.RequestSchema = new DataSchema(testSchema3);
            action.Contents = new Contents(JsonConvert.SerializeObject(baseData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(
                new List<INewDynamicApiDataStoreRepository>
                {
                            mockDynamicApiDataStoreRepository.Object
                });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NoContent);

        }

        [TestMethod]
        public void ExecuteAction_MailTemplateあり()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            //            var mockGetReturn = new JsonSearchResult(JsonConvert.SerializeObject(new { key = "value", item1 = "data1" }), 1);
            List<JsonDocument> mockGetReturn = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(new { id = "value", item1 = "data1" }))) };

            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);
            var mockPrimaryRepository = new Mock<INewDynamicApiDataStoreRepository>();
            //            mockPrimaryRepository.Setup(x => x.GetData(It.IsAny<UpdateAction>(), It.Is<HasSingleData>(param => param.Value == false))).Returns(mockGetReturn);
            mockPrimaryRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(mockGetReturn);
            mockPrimaryRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            // テスト対象のインスタンスを設定
            var action = CreateUpdateAction();
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            var inputData = new { key = "value", item1 = "data2" };
            action.Contents = new Contents(JsonConvert.SerializeObject(inputData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockPrimaryRepository.Object
                    });
            action.HasMailTemplate = new HasMailTemplate(true);

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Update(It.Is<IDynamicApiAction>(a => a == action),
                It.Is<JToken>(j => j.Value<string>("key") == inputData.key && j.Value<string>("item1") == inputData.item1),
                It.Is<JToken>(j => new JTokenEqualityComparer().Equals(j, JObject.FromObject(inputData)))), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Webhookあり()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            //            var mockGetReturn = new JsonSearchResult(JsonConvert.SerializeObject(new { key = "value", item1 = "data1" }), 1);
            List<JsonDocument> mockGetReturn = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(new { id = "value", item1 = "data1" }))) };

            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);
            var mockPrimaryRepository = new Mock<INewDynamicApiDataStoreRepository>();
            //            mockPrimaryRepository.Setup(x => x.GetData(It.IsAny<UpdateAction>(), It.Is<HasSingleData>(param => param.Value == false))).Returns(mockGetReturn);
            mockPrimaryRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(mockGetReturn);
            mockPrimaryRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            // テスト対象のインスタンスを設定
            var action = CreateUpdateAction();
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            var inputData = new { key = "value", item1 = "data2" };
            action.Contents = new Contents(JsonConvert.SerializeObject(inputData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockPrimaryRepository.Object
                    });
            action.HasWebhook = new HasWebhook(true);

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Update(It.Is<IDynamicApiAction>(a => a == action),
                It.Is<JToken>(j => j.Value<string>("key") == inputData.key && j.Value<string>("item1") == inputData.item1),
                It.Is<JToken>(j => new JTokenEqualityComparer().Equals(j, JObject.FromObject(inputData)))), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_Webhookあり_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableWebHookAndMailTemplate", false);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            //            var mockGetReturn = new JsonSearchResult(JsonConvert.SerializeObject(new { key = "value", item1 = "data1" }), 1);
            List<JsonDocument> mockGetReturn = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(new { id = "value", item1 = "data1" }))) };

            // モックを作成
            var mockRepository = new Mock<IResourceChangeEventHubStoreRepository>();
            UnityContainer.RegisterInstance(mockRepository.Object);
            var mockPrimaryRepository = new Mock<INewDynamicApiDataStoreRepository>();
            //            mockPrimaryRepository.Setup(x => x.GetData(It.IsAny<UpdateAction>(), It.Is<HasSingleData>(param => param.Value == false))).Returns(mockGetReturn);
            mockPrimaryRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(mockGetReturn);
            mockPrimaryRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            // テスト対象のインスタンスを設定
            var action = CreateUpdateAction();
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            var inputData = new { key = "value", item1 = "data2" };
            action.Contents = new Contents(JsonConvert.SerializeObject(inputData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockPrimaryRepository.Object
                    });
            action.HasWebhook = new HasWebhook(true);

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockRepository.Verify(x => x.Register(It.IsAny<IDynamicApiAction>(), It.IsAny<JToken>()), Times.Never);
        }

        [TestMethod]
        public void ExecuteAction_Blockchainあり_履歴なし()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            List<JsonDocument> mockGetReturn = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(new { id = "value", item1 = "data1" }))) };

            // モックを作成
            var updateData = new List<JToken>();
            var mockPrimaryRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockPrimaryRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockPrimaryRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(mockGetReturn);
            mockPrimaryRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((registerParam) =>
                {
                    updateData.Add(registerParam.Json);
                });
            var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
            mockBlockchainEventhub.Setup(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<string, JToken, RepositoryType, string>((id, token, type, version) =>
                {
                    blockchainEventList.Add(new KeyValuePair<JToken, string>(token, version));
                });
            blockchainEventList = new List<KeyValuePair<JToken, string>>();

            UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);

            // テスト対象のインスタンスを設定
            var action = CreateUpdateAction();
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            var inputData = new { key = "value", item1 = "data2" };
            action.Contents = new Contents(JsonConvert.SerializeObject(inputData));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                    {
                        mockPrimaryRepository.Object
                    });
            action.IsEnableBlockchain = new IsEnableBlockchain(true);

            // テスト対象のメソッドを実行
            action.ExecuteAction();
            // モックを検証
            mockBlockchainEventhub.Verify(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()), Times.Once);
            blockchainEventList.Count().Is(1);
            blockchainEventList.Single().Key.Is(updateData.Single());
            blockchainEventList.Single().Value.IsNull();
        }

        [TestMethod]
        public void ExecuteAction_Blockchainあり_履歴あり()
        {
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());
            List<JsonDocument> mockGetReturn = new List<JsonDocument>() { new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(new { id = "value", item1 = "data1" }))) };

            // モックを作成
            var updateData = new List<JToken>();
            var mockPrimaryRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockPrimaryRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockPrimaryRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(mockGetReturn);
            mockPrimaryRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((registerParam) =>
                {
                    updateData.Add(registerParam.Json);
                });
            var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
            mockBlockchainEventhub.Setup(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<string, JToken, RepositoryType, string>((id, token, type, version) =>
                {
                    blockchainEventList.Add(new KeyValuePair<JToken, string>(token, version));
                });
            blockchainEventList = new List<KeyValuePair<JToken, string>>();

            UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);

            //履歴のモック
            var mockDocVerion = new Mock<IDocumentVersionRepository>();
            mockDocVerion.Setup(x => x.SaveDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<RepositoryKeyInfo>(), It.IsAny<bool>())).Returns(() =>
            {
                var ret = new DocumentDbDocumentVersions();
                ret.Id = "value";
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
            mockPrimaryRepository.SetupProperty(x => x.DocumentVersionRepository, UnityContainer.Resolve<IDocumentVersionRepository>());

            // テスト対象のインスタンスを設定
            var action = CreateUpdateAction();
            action.RepositoryKey = new RepositoryKey("/API/Test/{key}");
            var inputData = new { key = "value", item1 = "data2" };
            action.Contents = new Contents(JsonConvert.SerializeObject(inputData));
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
            mockBlockchainEventhub.Verify(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()), Times.Once);
            blockchainEventList.Count().Is(1);
            var bcevent = blockchainEventList.Single();
            bcevent.Key.Is(updateData.Single());
            bcevent.Value.Is(VersionKey1);

            var historyHeader = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(result.Headers.GetValues("X-DocumentHistory").Single()).Single();
            historyHeader.documents.Single(x => x.documentKey == "value" && x.versionKey == VersionKey1);


        }

        [TestMethod]
        public void UpdateTest_Base64()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var baseData = new BaseDataAttachFile()
            {
                id = "hoge",
                Name = "BBBB",
                File1 = CreateBase64Registed("AAAA"),
                File2 = CreateBase64Registed("BBBB"),
                Is = true
            };
            var confirm = @"{
          'id' : 'hoge',
          'Name' : null,
          'Number' : 0,
          'Is' : false,
          'File1' : '{{*}}',
          'File2' : '{{*}}',
          '_Regdate' : '{{*}}',
          '_Reguser_Id' : '{{*}}',
          '_Upddate' : '{{*}}',
          '_Upduser_Id' : '{{*}}'
        }".ToJson();

            var baseDataupdate = new BaseDataAttachFile()
            {
                File1 = CreateBase64Query(base64StringNormal),
                File2 = CreateBase64Query(base64StringNormal2),
            };

            List<JsonDocument> getResult = new List<JsonDocument>() {
                        new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            var mockGetReturn = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn.BeginData();
            foreach (var str in getResult.RemoveTokenToJson(true))
            {
                mockGetReturn.AddString(str);
            }
            mockGetReturn.EndData();

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateUpdateAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;
            mockDynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()));

            action.Contents = new Contents(JsonConvert.SerializeObject(baseDataupdate));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockDynamicApiDataStoreRepository.Object });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NoContent);

            mockDynamicApiDataStoreRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));

            mockBlobRepository.Verify(x => x.DeleteFilestoBase64(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals($"attachfilebase64/hoge"))), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal)), It.Is<string>(c => c.Equals("attachfilebase64/hoge/File1"))), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal2)), It.Is<string>(c => c.Equals("attachfilebase64/hoge/File2"))), Times.Exactly(1));
        }

        [TestMethod]
        public void UpdateTest_Base64_Add()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var baseData = new BaseDataAttachFile()
            {
                id = "hoge",
                Name = "BBBB",
                File1 = CreateBase64Registed("AAAA"),
                Is = true
            };
            var confirm = @"{
          'id' : 'hoge',
          'Name' : null,
          'Number' : 0,
          'Is' : false,
          'File1' : '{{*}}',
          'File2' : '{{*}}',
          '_Regdate' : '{{*}}',
          '_Reguser_Id' : '{{*}}',
          '_Upddate' : '{{*}}',
          '_Upduser_Id' : '{{*}}'
        }".ToJson();

            var baseDataupdate = new BaseDataAttachFile()
            {
                File1 = CreateBase64Query(base64StringNormal),
                File2 = CreateBase64Query(base64StringNormal2),
            };

            List<JsonDocument> getResult = new List<JsonDocument>() {
                        new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            var mockGetReturn = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn.BeginData();
            foreach (var str in getResult.RemoveTokenToJson(true))
            {
                mockGetReturn.AddString(str);
            }
            mockGetReturn.EndData();

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateUpdateAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;

            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()));
            mockDynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            action.Contents = new Contents(JsonConvert.SerializeObject(baseDataupdate));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockDynamicApiDataStoreRepository.Object });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NoContent);

            mockDynamicApiDataStoreRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.DeleteFilestoBase64(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals($"attachfilebase64/hoge"))), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal)), It.Is<string>(c => c.Equals("attachfilebase64/hoge/File1"))), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal2)), It.Is<string>(c => c.Equals("attachfilebase64/hoge/File2"))), Times.Exactly(1));
        }

        [TestMethod]
        public void UpdateTest_Base64_ファイルなし()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var baseData = new BaseDataAttachFile()
            {
                id = "hoge",
                Name = "BBBB",
                Is = true
            };
            var confirm = @"{
          'id' : 'hoge',
          'Name' : null,
          'Number' : 0,
          'Is' : false,
          'File1' : '{{*}}',
          'File2' : '{{*}}',
          '_Regdate' : '{{*}}',
          '_Reguser_Id' : '{{*}}',
          '_Upddate' : '{{*}}',
          '_Upduser_Id' : '{{*}}'
        }".ToJson();

            var baseDataupdate = new BaseDataAttachFile()
            {
                File1 = CreateBase64Query(base64StringNormal),
                File2 = CreateBase64Query(base64StringNormal2),
            };

            List<JsonDocument> getResult = new List<JsonDocument>() {
                        new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            var mockGetReturn = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn.BeginData();
            foreach (var str in getResult.RemoveTokenToJson(true))
            {
                mockGetReturn.AddString(str);
            }
            mockGetReturn.EndData();

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));

            var action = CreateUpdateAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            var mockBlobRepository = SetUpBase64AttachFileRepositoryMock();
            action.AttachFileDynamicApiDataStoreRepository = mockBlobRepository.Object;

            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()));

            action.Contents = new Contents(JsonConvert.SerializeObject(baseDataupdate));
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository> { mockDynamicApiDataStoreRepository.Object });
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NoContent);

            mockDynamicApiDataStoreRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.DeleteFilestoBase64(It.IsAny<VendorId>(), It.IsAny<string>()), Times.Once);
            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal)), It.Is<string>(c => c.Equals("attachfilebase64/hoge/File1"))), Times.Exactly(1));
            mockBlobRepository.Verify(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.Is<string>(a => a.Equals(base64StringNormal2)), It.Is<string>(c => c.Equals("attachfilebase64/hoge/File2"))), Times.Exactly(1));
        }

        [TestMethod]
        public void DocumentHistory_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            var baseData = new BaseData()
            {
                Id = "AAAAA",
                Name = "BBBB",
                Number = 1,
                Is = true
            };

            List<JsonDocument> getResult = new List<JsonDocument>() {
                        new JsonDocument(JToken.Parse(JsonConvert.SerializeObject(baseData))) };
            var mockGetReturn = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            mockGetReturn.BeginData();
            foreach (var str in getResult.RemoveTokenToJson(false))
            {
                mockGetReturn.AddString(str);
            }
            mockGetReturn.EndData();

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);

            var action = CreateUpdateAction();
            var result = (JsonSearchResult)action.GetType().InvokeMember("GetRepositoryData", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockDynamicApiDataStoreRepository.Object
                }) });

            //result.Value.Is(mockGetReturn.Value);
            result.Count.Is(mockGetReturn.Count);
            result.JToken.IsStructuralEqual(mockGetReturn.JToken);
            mockDynamicApiDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Never);
        }

        [TestMethod]
        public void UpdateTest_StrictValidation_OK()
        {
            UnityContainer.RegisterInstance<bool>("UseStrictValidationOnUpdate", true);

            var input = $@"{{'id':'hoge', 'value1':'foo'}}";
            var baseData = $@"{{'id':'hoge', 'key':'fuga', 'value1':'piyo', 'value2':'piyo'}}";
            var requestModel = @"
        {
          'type': 'object',
          'additionalProperties': false,
          'properties': {
            'value1':  {
              'type': ['string','null']
            },
            'value2':  {
              'type': ['string','null']
            }
          }
        }";
            var controllerModel = @"
        {
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'key': {
              'type': 'string'
            },
            'value1':  {
              'type': 'string'
            },
            'value2':  {
              'type': 'string'
            }
          }
        }";

            var action = CreateUpdateAction(definitionContents: input, requestModel: requestModel, controllerModel: controllerModel);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(baseData)) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockDynamicApiDataStoreRepository.Object
                });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);

            mockDynamicApiDataStoreRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void UpdateTest_StrictValidation_バリデーションエラー_リクエストモデル()
        {
            UnityContainer.RegisterInstance<bool>("UseStrictValidationOnUpdate", true);

            var input = $@"{{'id':'hoge', 'key':'foo', 'value1':'bar'}}";
            var baseData = $@"{{'id':'hoge', 'key':'fuga', 'value1':'piyo', 'value2':'piyo'}}";
            var requestModel = @"
        {
          'type': 'object',
          'additionalProperties': false,
          'properties': {
            'value1':  {
              'type': ['string','null']
            },
            'value2':  {
              'type': ['string','null']
            }
          }
        }";
            var controllerModel = @"
        {
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'key': {
              'type': 'string'
            },
            'value1':  {
              'type': 'string'
            },
            'value2':  {
              'type': 'string'
            }
          }
        }";

            var action = CreateUpdateAction(definitionContents: input, requestModel: requestModel, controllerModel: controllerModel);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(baseData)) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockDynamicApiDataStoreRepository.Object
                });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            // レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            // メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["key"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'key' has not been defined and the schema does not allow additional properties.(code:15)");

            mockDynamicApiDataStoreRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void UpdateTest_StrictValidation_バリデーションエラー_リソースモデル()
        {
            UnityContainer.RegisterInstance<bool>("UseStrictValidationOnUpdate", true);

            var input = $@"{{'id':'hoge', 'value1':null}}";
            var baseData = $@"{{'id':'hoge', 'key':'fuga', 'value1':'piyo', 'value2':'piyo'}}";
            var requestModel = @"
        {
          'type': 'object',
          'additionalProperties': false,
          'properties': {
            'value1':  {
              'type': ['string','null']
            },
            'value2':  {
              'type': ['string','null']
            }
          }
        }";
            var controllerModel = @"
        {
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'key': {
              'type': 'string'
            },
            'value1':  {
              'type': 'string'
            },
            'value2':  {
              'type': 'string'
            }
          }
        }";

            var action = CreateUpdateAction(definitionContents: input, requestModel: requestModel, controllerModel: controllerModel);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            List<JsonDocument> getResult = new List<JsonDocument>() { new JsonDocument(JToken.Parse(baseData)) };
            getResult.RemoveTokenToJson(false);

            var mockDynamicApiDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockDynamicApiDataStoreRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));
            mockDynamicApiDataStoreRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(getResult);
            mockDynamicApiDataStoreRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((document) =>
                {
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                });

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
                {
                    mockDynamicApiDataStoreRepository.Object
                });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            // レスポンスのコンテントタイプ は、application/problem+json
            result.Content.Headers.ContentType.MediaType.Is("application/problem+json");
            var msg = result.Content.ReadAsStringAsync().Result.ToJson();
            msg["error_code"].ToString().Is(ErrorCodeMessage.Code.E10402.ToString());
            // メッセージチェック
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["value1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String but got Null.(code:18)");

            mockDynamicApiDataStoreRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockDynamicApiDataStoreRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(0));
        }


        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();
        private Guid ProviderVendorId = Guid.NewGuid();
        private Guid ProviderSystemId = Guid.NewGuid();

        private UpdateAction CreateUpdateAction(string definitionContents = null, string requestModel = null, string contentType = null, string controllerModel = null)
        {
            UpdateAction action = UnityCore.Resolve<UpdateAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.ProviderSystemId = new SystemId(ProviderSystemId.ToString());
            action.ProviderVendorId = new VendorId(ProviderVendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PATCH);
            action.RepositoryKey = new RepositoryKey("/API/Private/UpdateTest/{Id}");
            action.ActionType = new ActionTypeVO(ActionType.Update);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);

            action.ControllerSchema = controllerModel == null ? new DataSchema(null) : new DataSchema(controllerModel);
            action.RequestSchema = requestModel == null ? new DataSchema(null) : new DataSchema(requestModel);
            action.Contents = definitionContents == null ? new Contents("") : new Contents(definitionContents);
            action.Accept = string.IsNullOrEmpty(contentType) ? new Accept("*/*") : new Accept(contentType);
            action.AttachFileBlobRepositoryInfo = new RepositoryInfo("afb", new Dictionary<string, bool>() { { "DefaultEndpointsProtocol=https;AccountName=fuga;AccountKey=hoge;EndpointSuffix=core.windows.net", false } });
            action.IsEnableBlockchain = new IsEnableBlockchain(false);
            action.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo>() { new RepositoryInfo("ddb", new Dictionary<string, bool>() { { "connectionstring", false } }) });
            return action;
        }
        private Mock<IDynamicApiAttachFileRepository> SetUpBase64AttachFileRepositoryMock()
        {
            var mockBlobRepository = new Mock<IDynamicApiAttachFileRepository>();
            mockBlobRepository.Setup(x => x.UploadBase64ToFile(It.IsAny<VendorId>(), It.IsAny<string>(), It.IsAny<string>()));
            mockBlobRepository.Setup(x => x.DeleteFilestoBase64(It.IsAny<VendorId>(), It.IsAny<string>()));
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

        static void SortJson(JToken token)
        {
            if (token is JObject)
            {
                SortJson((JObject)token);
            }
        }

        static void SortJson(JObject jObj)
        {
            var props = jObj.Properties().ToList();
            foreach (var prop in props)
            {
                prop.Remove();
            }

            foreach (var prop in props.OrderBy(p => p.Name))
            {
                jObj.Add(prop);
                if (prop.Value is JObject)
                    SortJson((JObject)prop.Value);
                if (prop.Value is JArray)
                {
                    Int32 iCount = prop.Value.Count();
                    for (Int32 iIterator = 0; iIterator < iCount; iIterator++)
                        if (prop.Value[iIterator] is JObject)
                            SortJson((JObject)prop.Value[iIterator]);
                }
            }
        }


        public class BaseData
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
            public bool Is { get; set; }

        }

        public class BaseDataAttachFile
        {
            public string id { get; set; }
            public string Name { get; set; }
            public int Number { get; set; }
            public bool Is { get; set; }
            public string File1 { get; set; }
            public string File2 { get; set; }

        }

        private string testSchema = @"
{
  'properties': {
    'Name': {
      'type': 'string',
      'maxLength': 12,
      'required': true
    },
    'Number': {
      'type': 'integer'
    }
  },
  'type': 'object'
}
";

        private string testSchema2 = @"
{
  'properties': {
    'Name': {
      'type': 'integer',
      'maxLength': 12,
      'required': true
    },
    'Number': {
      'type': 'integer'
    }
  },
  'type': 'object'
}
";

        private string testSchema3 = @"
{
  'properties': {
'Id': {
      'type': 'string',
      'maxLength': 12,
      'required': true
    },    
'Name': {
      'type': 'string',
      'maxLength': 12,
      'required': true
    },
    'Number': {
      'type': 'integer'
    },
    'Is': {
      'type': 'boolean'
    }
  },
  'additionalProperties': false,
  'type': 'object'
}
";
        private string testSchema4 = @"
{
  'properties': {
'Id': {
      'type': 'string',
      'maxLength': 12,
      'required': true
    },    
'Name': {
      'type': 'string',
      'maxLength': 12,
      'required': true
    },
    'Number': {
      'type': 'integer'
    }
  },
  'additionalProperties': false,
  'type': 'object'
}
";
        private List<KeyValuePair<JToken, string>> blockchainEventList;
        private string VersionKey1 = Guid.NewGuid().ToString();
    }
}
