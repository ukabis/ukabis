using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ActionInjector;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ActionInjector
{
    [TestClass]
    public class UnitTest_GetAttachFileMetaActionInjector : UnitTestBase
    {
        public TestContext TestContext { get; set; }


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread");
            UnityContainer.RegisterInstance(new Mock<ICache>().Object);

            var mockRepository = new Mock<IDynamicApiRepository>();
            mockRepository.Setup(x => x.GetSchemaModelById(It.IsAny<DataSchemaId>())).Returns(new DataSchema(""));
            UnityContainer.RegisterInstance(mockRepository.Object);
        }

        [TestMethod]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void Execute_正常()
        {
            var content = $@"
{{
    ""FileId"": ""30a15221-e1fd-4b6e-b556-b00cc3618d4a"",
    ""FileName"": ""hoge.jpg"",
    ""ContentType"": ""image/jpeg"",
    ""FileLength"": 4076717,
    ""IsDrm"": false,
    ""IsUploaded"": false,
    ""MetaList"": [
        {{
            ""MetaKey"": ""TestKey"",
            ""MetaValue"": ""testKey""
        }},
        {{
            ""MetaKey"": ""Key2"",
            ""MetaValue"": ""KeyValue2""
        }}
    ],
    ""IsExternalAttachFile"": false
}}";
            var contentAllField = $@"
{{
    ""FileId"": ""30a15221-e1fd-4b6e-b556-b00cc3618d4a"",
    ""FileName"": ""hoge.jpg"",
    ""ContentType"": ""image/jpeg"",
    ""FileLength"": 4076717,
    ""IsDrm"": false,
    ""IsUploaded"": false,
    ""MetaList"": [
        {{
            ""MetaKey"": ""TestKey"",
            ""MetaValue"": ""testKey""
        }},
        {{
            ""MetaKey"": ""Key2"",
            ""MetaValue"": ""KeyValue2""
        }}
    ],
    ""IsExternalAttachFile"": false,
    ""_Type"": ""API~hoge~Attachfile"",
    ""_Reguser_Id"": null,
    ""_Regdate"": ""2022-04-12T03:41:12.9360933Z"",
    ""_Upduser_Id"": null,
    ""_Upddate"": ""2022-04-12T03:41:12.9360933Z"",
    ""_Version"": 1,
    ""_partitionkey"": ""API~hoge~Attachfile~1""
}}";

            TestContext.Run((bool allfield, bool nullValue) =>
            {
                var perRequestDataContainer = new PerRequestDataContainer();
                perRequestDataContainer.UserId = new Guid();
                UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

                var action = CreateRegistDataAction();
                if (allfield || !nullValue)
                {
                    action.XGetInnerAllField = new XGetInnerField(allfield);
                }

                var target = UnityCore.Resolve<GetAttachFileMetaActionInjector>();
                target.Target = action;
                target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(allfield ? contentAllField : content) };
                target.Execute(() => { });

                var query = action.Query;
                query.Dic[new QueryStringKey("$filter")].Value.Is($"FileId eq {FileId}");
                if (allfield)
                {
                    query.Dic[new QueryStringKey("$select")].Value.Is($"FileId,FileName,ContentType,FileLength,IsDrm,DrmType,DrmKey,IsUploaded,MetaList,IsExternalAttachFile,ExternalAttachFile,_Type,_Vendor_Id,_System_Id,_Reguser_Id,_Regdate,_Upduser_Id,_Upddate,_Version,_partitionkey,_Owner_Id");
                }
                else
                {
                    query.Dic[new QueryStringKey("$select")].Value.Is($"FileId,FileName,ContentType,FileLength,IsDrm,DrmType,DrmKey,IsUploaded,MetaList");
                }
                action.ControllerSchema.IsNotNull();
                action.OperationInfo.IsAttachFileOperation.IsTrue();
                action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

                var response = (HttpResponseMessage)target.ReturnValue;
                response.StatusCode.Is(HttpStatusCode.Created);
                var meta = response.Content.ReadAsStringAsync().Result.ToJson();
                meta.ToString().Is((allfield ? contentAllField : content).ToJson().ToString());
            });
        }

        [TestMethod]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void Execute_正常_外部添付ファイル()
        {
            var content = $@"
{{
    ""FileId"": ""30a15221-e1fd-4b6e-b556-b00cc3618d4a"",
    ""FileName"": ""hoge.jpg"",
    ""ContentType"": ""image/jpeg"",
    ""FileLength"": 4076717,
    ""IsDrm"": false,
    ""IsUploaded"": false,
    ""MetaList"": [
        {{
            ""MetaKey"": ""TestKey"",
            ""MetaValue"": ""testKey""
        }},
        {{
            ""MetaKey"": ""Key2"",
            ""MetaValue"": ""KeyValue2""
        }}
    ]
}}";
            var expected = content;

            var contentAllField = $@"
{{
    ""FileId"": ""30a15221-e1fd-4b6e-b556-b00cc3618d4a"",
    ""FileName"": ""hoge.jpg"",
    ""ContentType"": ""image/jpeg"",
    ""FileLength"": 4076717,
    ""IsDrm"": false,
    ""IsUploaded"": false,
    ""MetaList"": [
        {{
            ""MetaKey"": ""TestKey"",
            ""MetaValue"": ""testKey""
        }},
        {{
            ""MetaKey"": ""Key2"",
            ""MetaValue"": ""KeyValue2""
        }}
    ],
    ""IsExternalAttachFile"": true,
    ""ExternalAttachFile"": {{
        ""DataSourceType"": ""az-blob-accesskey"",
        ""Credentials"": [
            ""hoge"",
            ""fuga""
        ],
        ""FilePath"": ""hoge/response.jpg""
    }},
    ""_Type"": ""API~hoge~Attachfile"",
    ""_Reguser_Id"": null,
    ""_Regdate"": ""2022-04-12T03:41:12.9360933Z"",
    ""_Upduser_Id"": null,
    ""_Upddate"": ""2022-04-12T03:41:12.9360933Z"",
    ""_Version"": 1,
    ""_partitionkey"": ""API~hoge~Attachfile~1""
}}";
            var expectedAllField = $@"
{{
    ""FileId"": ""30a15221-e1fd-4b6e-b556-b00cc3618d4a"",
    ""FileName"": ""hoge.jpg"",
    ""ContentType"": ""image/jpeg"",
    ""FileLength"": 4076717,
    ""IsDrm"": false,
    ""IsUploaded"": false,
    ""MetaList"": [
        {{
            ""MetaKey"": ""TestKey"",
            ""MetaValue"": ""testKey""
        }},
        {{
            ""MetaKey"": ""Key2"",
            ""MetaValue"": ""KeyValue2""
        }}
    ],
    ""IsExternalAttachFile"": true,
    ""ExternalAttachFile"": {{
        ""DataSourceType"": ""az-blob-accesskey"",
        ""Credentials"": [
            ""***"",
            ""***""
        ],
        ""FilePath"": ""hoge/response.jpg""
    }},
    ""_Type"": ""API~hoge~Attachfile"",
    ""_Reguser_Id"": null,
    ""_Regdate"": ""2022-04-12T03:41:12.9360933Z"",
    ""_Upduser_Id"": null,
    ""_Upddate"": ""2022-04-12T03:41:12.9360933Z"",
    ""_Version"": 1,
    ""_partitionkey"": ""API~hoge~Attachfile~1""
}}";

            TestContext.Run((bool allfield, bool nullValue) =>
            {
                var perRequestDataContainer = new PerRequestDataContainer();
                perRequestDataContainer.UserId = new Guid();
                UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

                var action = CreateRegistDataAction();
                if (allfield || !nullValue)
                {
                    action.XGetInnerAllField = new XGetInnerField(allfield);
                }

                var target = UnityCore.Resolve<GetAttachFileMetaActionInjector>();
                target.Target = action;
                target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(allfield ? contentAllField : content) };
                target.Execute(() => { });

                var query = action.Query;
                query.Dic[new QueryStringKey("$filter")].Value.Is($"FileId eq {FileId}");
                if (allfield)
                {
                    query.Dic[new QueryStringKey("$select")].Value.Is($"FileId,FileName,ContentType,FileLength,IsDrm,DrmType,DrmKey,IsUploaded,MetaList,IsExternalAttachFile,ExternalAttachFile,_Type,_Vendor_Id,_System_Id,_Reguser_Id,_Regdate,_Upduser_Id,_Upddate,_Version,_partitionkey,_Owner_Id");
                }
                else
                {
                    query.Dic[new QueryStringKey("$select")].Value.Is($"FileId,FileName,ContentType,FileLength,IsDrm,DrmType,DrmKey,IsUploaded,MetaList");
                }
                action.ControllerSchema.IsNotNull();
                action.OperationInfo.IsAttachFileOperation.IsTrue();
                action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

                var response = (HttpResponseMessage)target.ReturnValue;
                response.StatusCode.Is(HttpStatusCode.Created);
                var meta = response.Content.ReadAsStringAsync().Result.ToJson();
                meta.ToString().Is((allfield ? expectedAllField : expected).ToJson().ToString());
            });
        }

        [TestMethod]
        public void Execute_正常_NotFound()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction();
            var target = UnityCore.Resolve<GetAttachFileMetaActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.NotFound);
            target.Execute(() => { });

            var query = action.Query;
            query.Dic[new QueryStringKey("$filter")].Value.Is($"FileId eq {FileId}");
            query.Dic[new QueryStringKey("$select")].Value.Is($"FileId,FileName,ContentType,FileLength,IsDrm,DrmType,DrmKey,IsUploaded,MetaList");
            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            var response = (HttpResponseMessage)target.ReturnValue;
            response.StatusCode.Is(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void Execute_異常_パラメータなし()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction();
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { });
            var target = UnityCore.Resolve<GetAttachFileMetaActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });
            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void ExecuteAction_異常_設定無効()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(false);
            var target = UnityCore.Resolve<GetAttachFileMetaActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            target.Execute(() => { });
            var result = target.ReturnValue as HttpResponseMessage;
            result.StatusCode.Is(HttpStatusCode.NotImplemented);
        }


        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();
        private static string FileId = "D261DF4B-0305-46AD-9FF5-12026BEE3F89";

        private QueryAction CreateRegistDataAction()
        {
            QueryAction action = UnityCore.Resolve<QueryAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{FileId}");
            action.ActionType = new ActionTypeVO(ActionType.OData);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.RequestSchema = new DataSchema(null);
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue(FileId) } });
            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.SetupGet(x => x.AttachFileMetaManagementFields).Returns(new string[]
            {
                "_Type",
                "_Vendor_Id",
                "_System_Id",
                "_Reguser_Id",
                "_Regdate",
                "_Upduser_Id",
                "_Upddate",
                "_Version",
                "_partitionkey",
                "_Owner_Id"
            });
            mockDataRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>())).Returns(new RegisterOnceResult(""));
            return action;
        }

    }
}
