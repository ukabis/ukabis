using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_RegisterRawDataAction : UnitTestBase
    {
        private string _openId = Guid.NewGuid().ToString();

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>("multiThread", perRequestDataContainer);

            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
        }

        private HttpResponseMessage ExecuteAction_Common(
            string requestContents,
            string expectContent,
            HttpMethodType designatedHttpMethodType = null)
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Configuration.GetSection("AppConfig:OperatingVendorVendorId").Get<string[]>().First();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var contentsObject = JToken.Parse(requestContents);
            var expectContentsObject = JToken.Parse(expectContent);
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockNewRepository = new Mock<INewDynamicApiDataStoreRepository>();
            int i = 0;
            var documentDataIds = new List<DocumentDataId>();
            mockRepository.Setup(x => x.KeyManagement.GetId(It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), It.IsAny<IPerRequestDataContainer>())).Returns(() =>
            {
                i++;
                DocumentDataId docId = null;
                if (contentsObject[i - 1].IsExistProperty("id"))
                {
                    docId = new DocumentDataId(contentsObject[i - 1]["id"].ToString(), null, null);
                }
                else
                {
                    docId = new DocumentDataId("hogeAutoId" + i, null, null);
                }
                documentDataIds.Add(docId);
                return docId;
            });
            DocumentDataId documentDataId;
            mockRepository.Setup(x => x.KeyManagement.IsIdValid(It.IsAny<JToken>(), It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), out documentDataId)).Returns(() =>
            {
                return false;
            });
            mockRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));

            int j = 0;
            mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((para) =>
                {
                    if (expectContentsObject is JArray)
                    {
                        var array = expectContentsObject as JArray;
                        var json = array[j];
                        var actualJson = para.Json;
                        actualJson.ToString().ToJson().Is(json);
                    }
                    UnityContainer.Resolve<IPerRequestDataContainer>("multiThread").UserId.Is(perRequestDataContainer.UserId);
                })
                .Returns(() => {
                    var registerId = documentDataIds.Count == 0 ? contentsObject[j]["id"].ToString() : documentDataIds[j].Id.ToString();
                    j++;
                    return new RegisterOnceResult(registerId);
                });

            var action = CreateRegisterRawDataAction(designatedHttpMethodType: designatedHttpMethodType);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.Contents = new Contents(requestContents);

            var result = action.ExecuteAction();

            return result;
        }

        [TestMethod]
        public void ExecuteAction_Post_正常系テスト_管理項目全部入り()
        {
            var content = @"[{'id':'hogeId1','prop1':'hogeProp1','_Upduser_Id':'hogeUser','_Upddate':'2020-09-11T01:34:44.2986311Z','_Reguser_Id':'hogeUser2','_Regdate':'2020-09-11T01:34:44.2986311Z', '_Owner_Id':'hogeOwner'},
                             {'id':'hogeId2','prop1':'hogeProp2','_Upduser_Id':'hogeUser3','_Upddate':'2020-09-11T02:34:44.2986311Z','_Reguser_Id':'hogeUser4','_Regdate':'2020-09-12T01:34:44.2986311Z', '_Owner_Id':'hogeOwner2'}]";
            //管理項目等来たものをそのまま登録するので、expectはcontentと同一のはず
            var expectContent = content;

            var result = ExecuteAction_Common(
               content,
               expectContent);
            result.StatusCode.Is(HttpStatusCode.Created);
            var actualReturn = result.Content.ReadAsStringAsync().Result;

            var expectRetrun = "[{'id':'hogeId1'},{'id':'hogeId2'}]";
            actualReturn.ToJson().Is(expectRetrun.ToJson());
        }

        [TestMethod]
        public void ExecuteAction_Post_正常系テスト_管理項目無し()
        {
            var content = @"[{'id':'hogeId1','prop1':'hogeProp1'},
                             {'id':'hogeId2','prop1':'hogeProp2'}]";

            var expectContent = $@"[{{'id':'hogeId1','prop1':'hogeProp1','_Upduser_Id':'{_openId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{_openId}','_Regdate':'{{{{*}}}}', '_Owner_Id':'{_openId}'}},
                                    {{'id':'hogeId2','prop1':'hogeProp2','_Upduser_Id':'{_openId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{_openId}','_Regdate':'{{{{*}}}}', '_Owner_Id':'{_openId}'}}]";

            var result = ExecuteAction_Common(
               content,
               expectContent);
            result.StatusCode.Is(HttpStatusCode.Created);
            var actualReturn = result.Content.ReadAsStringAsync().Result;

            var expectRetrun = "[{'id':'hogeId1'},{'id':'hogeId2'}]";
            actualReturn.ToJson().Is(expectRetrun.ToJson());
        }

        [TestMethod]
        public void ExecuteAction_Post_正常系テスト_管理項目無し_id無し()
        {
            var content = @"[{'prop1':'hogeProp1'},
                             {'prop1':'hogeProp2'}]";

            var expectContent = $@"[{{'id':'hogeAutoId1','prop1':'hogeProp1','_Upduser_Id':'{_openId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{_openId}','_Regdate':'{{{{*}}}}', '_Owner_Id':'{_openId}'}},
                                    {{'id':'hogeAutoId2','prop1':'hogeProp2','_Upduser_Id':'{_openId}','_Upddate':'{{{{*}}}}','_Reguser_Id':'{_openId}','_Regdate':'{{{{*}}}}', '_Owner_Id':'{_openId}'}}]";

            var result = ExecuteAction_Common(
               content,
               expectContent);
            result.StatusCode.Is(HttpStatusCode.Created);
            var actualReturn = result.Content.ReadAsStringAsync().Result;

            var expectRetrun = "[{'id':'hogeAutoId1'},{'id':'hogeAutoId2'}]";
            actualReturn.ToJson().Is(expectRetrun.ToJson());
        }

        [TestMethod]
        public void ExecuteAction_Post_正常系テスト_コンテナ分離_ベンダー依存()
        {
            var vendorId = Configuration.GetSection("AppConfig:OperatingVendorVendorId").Get<string[]>().First();
            var systemId = Guid.NewGuid().ToString();
            var dataVendorId = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var dataSystemId = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var requestContents = $@"
[
    {{ 'id':'hogeId1','prop1':'hogeProp1','_Upduser_Id':'hogeUser1','_Upddate':'2020-09-11T01:34:44.2986311Z','_Reguser_Id':'hogeUser2','_Regdate':'2020-09-11T01:34:44.2986311Z','_Owner_Id':'hogeOwner1','_Vendor_Id':'{dataVendorId[0]}','_System_Id':'{dataSystemId[0]}' }},
    {{ 'id':'hogeId2','prop1':'hogeProp2','_Upduser_Id':'hogeUser3','_Upddate':'2020-09-11T02:34:44.2986311Z','_Reguser_Id':'hogeUser4','_Regdate':'2020-09-12T01:34:44.2986311Z','_Owner_Id':'hogeOwner2','_Vendor_Id':'{dataVendorId[1]}','_System_Id':'{dataSystemId[1]}' }}
]";

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = vendorId;
            perRequestDataContainer.SystemId = systemId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var contentsObject = JToken.Parse(requestContents);
            var expectContentsObject = JToken.Parse(requestContents);

            int j = 0;
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((para) =>
                {
                    var array = expectContentsObject as JArray;
                    para.Json.ToString().ToJson().Is(array[j]);
                    perRequestDataContainer.VendorId.Is(dataVendorId[j]);
                    perRequestDataContainer.SystemId.Is(dataSystemId[j]);
                })
                .Returns(() => {
                    j++;
                    return new RegisterOnceResult(Guid.NewGuid().ToString());
                });

            var action = CreateRegisterRawDataAction();
            action.IsVendor = new IsVendor(true);
            action.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(true);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.Contents = new Contents(requestContents);

            var result = action.ExecuteAction();
            perRequestDataContainer.VendorId.Is(vendorId);
            perRequestDataContainer.SystemId.Is(systemId);
            mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(2));
        }

        [TestMethod]
        public void ExecuteAction_Post_異常系テスト_コンテナ分離_ベンダー依存()
        {
            var vendorId = Configuration.GetSection("AppConfig:OperatingVendorVendorId").Get<string[]>().First();
            var systemId = Guid.NewGuid().ToString();
            var dataVendorId = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var dataSystemId = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var requestContents = $@"
[
    {{ 'id':'hogeId1','prop1':'hogeProp1','_Upduser_Id':'hogeUser1','_Upddate':'2020-09-11T01:34:44.2986311Z','_Reguser_Id':'hogeUser2','_Regdate':'2020-09-11T01:34:44.2986311Z','_Owner_Id':'hogeOwner1','_Vendor_Id':'{dataVendorId[0]}','_System_Id':'{dataSystemId[0]}' }},
    {{ 'id':'hogeId2','prop1':'hogeProp2','_Upduser_Id':'hogeUser3','_Upddate':'2020-09-11T02:34:44.2986311Z','_Reguser_Id':'hogeUser4','_Regdate':'2020-09-12T01:34:44.2986311Z','_Owner_Id':'hogeOwner2','_Vendor_Id':'{dataVendorId[1]}','_System_Id':'{dataSystemId[1]}' }}
]";

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = vendorId;
            perRequestDataContainer.SystemId = systemId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var contentsObject = JToken.Parse(requestContents);

            var message = Guid.NewGuid().ToString();
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>())).Throws(new Exception(message));

            var action = CreateRegisterRawDataAction();
            action.IsVendor = new IsVendor(true);
            action.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(true);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.Contents = new Contents(requestContents);

            AssertEx.Throws<Exception>(() => action.ExecuteAction()).Message.Is(message);
            perRequestDataContainer.VendorId.Is(vendorId);
            perRequestDataContainer.SystemId.Is(systemId);
        }

        [TestMethod]
        public void ExecuteAction_Post_正常系テスト_コンテナ分離_個人依存()
        {
            var openId = Guid.NewGuid().ToString();
            var dataOpenId = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var requestContents = $@"
[
    {{ 'id':'hogeId1','prop1':'hogeProp1','_Upduser_Id':'hogeUser1','_Upddate':'2020-09-11T01:34:44.2986311Z','_Reguser_Id':'hogeUser2','_Regdate':'2020-09-11T01:34:44.2986311Z','_Owner_Id':'{dataOpenId[0]}' }},
    {{ 'id':'hogeId2','prop1':'hogeProp2','_Upduser_Id':'hogeUser3','_Upddate':'2020-09-11T02:34:44.2986311Z','_Reguser_Id':'hogeUser4','_Regdate':'2020-09-12T01:34:44.2986311Z','_Owner_Id':'{dataOpenId[1]}' }}
]";

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Configuration.GetSection("AppConfig:OperatingVendorVendorId").Get<string[]>().First();
            perRequestDataContainer.OpenId = openId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var contentsObject = JToken.Parse(requestContents);
            var expectContentsObject = JToken.Parse(requestContents);

            int j = 0;
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()))
                .Callback<RegisterParam>((para) =>
                {
                    var array = expectContentsObject as JArray;
                    para.Json.ToString().ToJson().Is(array[j]);
                    perRequestDataContainer.OpenId.Is(dataOpenId[j]);
                })
                .Returns(() => {
                    j++;
                    return new RegisterOnceResult(Guid.NewGuid().ToString());
                });

            var action = CreateRegisterRawDataAction();
            action.IsPerson = new IsPerson(true);
            action.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(true);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.Contents = new Contents(requestContents);

            var result = action.ExecuteAction();
            perRequestDataContainer.OpenId.Is(openId);
            mockRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Exactly(2));
        }

        [TestMethod]
        public void ExecuteAction_Post_異常系テスト_コンテナ分離_個人依存()
        {
            var openId = Guid.NewGuid().ToString();
            var dataOpenId = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var requestContents = $@"
[
    {{ 'id':'hogeId1','prop1':'hogeProp1','_Upduser_Id':'hogeUser1','_Upddate':'2020-09-11T01:34:44.2986311Z','_Reguser_Id':'hogeUser2','_Regdate':'2020-09-11T01:34:44.2986311Z','_Owner_Id':'{dataOpenId[0]}' }},
    {{ 'id':'hogeId2','prop1':'hogeProp2','_Upduser_Id':'hogeUser3','_Upddate':'2020-09-11T02:34:44.2986311Z','_Reguser_Id':'hogeUser4','_Regdate':'2020-09-12T01:34:44.2986311Z','_Owner_Id':'{dataOpenId[1]}' }}
]";

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Configuration.GetSection("AppConfig:OperatingVendorVendorId").Get<string[]>().First();
            perRequestDataContainer.OpenId = openId;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var contentsObject = JToken.Parse(requestContents);

            var message = Guid.NewGuid().ToString();
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>())).Throws(new Exception(message));

            var action = CreateRegisterRawDataAction();
            action.IsPerson = new IsPerson(true);
            action.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(true);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            action.Contents = new Contents(requestContents);

            AssertEx.Throws<Exception>(() => action.ExecuteAction());
            perRequestDataContainer.OpenId.Is(openId);
        }

        [TestMethod]
        public void ExecuteAction_Post_正常系テスト_空配列()
        {
            //空配列
            var content = @"[]";

            var expectContent = @"[]";

            var result = ExecuteAction_Common(
               content,
               expectContent);
            result.StatusCode.Is(HttpStatusCode.Created);
            var actualReturn = result.Content.ReadAsStringAsync().Result;

            var expectRetrun = "[]";
            actualReturn.ToJson().Is(expectRetrun.ToJson());
        }

        [TestMethod]
        public void ExecuteAction_Post_異常系_テスト_Requestが非配列()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Configuration.GetSection("AppConfig:OperatingVendorVendorId").Get<string[]>().First();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            var content = @"{'id':'hogeId1','prop1':'hogeProp1'}";

            var action = CreateRegisterRawDataAction();
            action.Contents = new Contents(content);

            var result = action.ExecuteAction();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10405.ToString(), actualMessage["error_code"]);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_Post_異常系_テスト_RequestBody_空_空Json_Json異常_JValue()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Configuration.GetSection("AppConfig:OperatingVendorVendorId").Get<string[]>().First();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            //空
            string content = string.Empty;

            var action = CreateRegisterRawDataAction();
            action.Contents = new Contents(content);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);

            //空Json
            content = "{}";

            action = CreateRegisterRawDataAction();
            action.Contents = new Contents(content);

            result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);

            //Json異常-1
            content = "{'id':'hoge}";

            action = CreateRegisterRawDataAction();
            action.Contents = new Contents(content);

            result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E10403.ToString(), actualMessage["error_code"]);

            //Json異常-2
            content = "hoge";

            action = CreateRegisterRawDataAction();
            action.Contents = new Contents(content);

            result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            //JValue
            content = "{123}";

            action = CreateRegisterRawDataAction();
            action.Contents = new Contents(content);

            result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_正常系_Put()
        {
            var content = @"[{'id':'hogeId1','prop1':'hogeProp1','_Upduser_Id':'hogeUser','_Upddate':'2020-09-11T01:34:44.2986311Z','_Reguser_Id':'hogeUser2','_Regdate':'2020-09-11T01:34:44.2986311Z', '_Owner_Id':'hogeOwner'},
                             {'id':'hogeId2','prop1':'hogeProp2','_Upduser_Id':'hogeUser3','_Upddate':'2020-09-11T02:34:44.2986311Z','_Reguser_Id':'hogeUser4','_Regdate':'2020-09-12T01:34:44.2986311Z', '_Owner_Id':'hogeOwner2'}]";
            //管理項目等来たものをそのまま登録するので、expectはcontentと同一のはず
            var expectContent = content;

            var result = ExecuteAction_Common(
               content,
               expectContent,
               designatedHttpMethodType: new HttpMethodType(HttpMethodType.MethodTypeEnum.PUT));

            result.StatusCode.Is(HttpStatusCode.OK);
        }

        [TestMethod]
        public void ExecuteAction_異常系_リポジトリキー無し_Post()
        {
            var action = CreateRegisterRawDataAction();
            action.RepositoryKey = new RepositoryKey(null);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_異常系_リポジトリキー無し_Put()
        {
            var action = CreateRegisterRawDataAction();
            action.RepositoryKey = new RepositoryKey(null);
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PUT);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_異常系_Get()
        {
            var action = CreateRegisterRawDataAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_異常系_Delete()
        {
            var action = CreateRegisterRawDataAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_異常系_Patch()
        {
            var action = CreateRegisterRawDataAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PATCH);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_異常系_非運用ベンダー()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegisterRawDataAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.Forbidden);
        }

        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();

        private RegisterRawDataAction CreateRegisterRawDataAction(HttpMethodType designatedHttpMethodType = null)
        {
            RegisterRawDataAction action = UnityCore.Resolve<RegisterRawDataAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.MethodType = designatedHttpMethodType == null ? new HttpMethodType(HttpMethodType.MethodTypeEnum.POST) : designatedHttpMethodType;
            action.ControllerRelativeUrl = new ControllerUrl("/API/Private/QueryTest");
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{hoge}");
            action.ActionType = new ActionTypeVO(ActionType.RegisterRawData);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.IsVendor = new IsVendor(false);
            action.IsPerson = new IsPerson(false);
            action.OpenId = new OpenId(_openId);
            action.IsOpenIdAuthentication = new IsOpenIdAuthentication(false);
            action.IsAutomaticId = new IsAutomaticId(false);
            action.ProviderSystemId = new SystemId(SystemId.ToString());
            action.ProviderVendorId = new VendorId(VendorId.ToString());

            action.RequestSchema = new DataSchema(null);
            action.Accept = new Accept("*/*");
            action.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo>() { new RepositoryInfo("ddb", new Dictionary<string, bool>() { { "connectionstring", false } }) });
            action.PostDataType = new PostDataType("array");

            return action;
        }

    }
}
