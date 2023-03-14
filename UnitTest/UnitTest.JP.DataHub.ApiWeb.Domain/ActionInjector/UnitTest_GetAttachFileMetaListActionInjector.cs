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
    public class UnitTest_GetAttachFileMetaListActionInjector : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private static string s_content = $@"
[
    {{
        ""FileId"": ""30a15221-e1fd-4b6e-b556-b00cc3618d4a"",
        ""FileName"": ""hoge.jpg"",
        ""ContentType"": ""image/jpeg"",
        ""FileLength"": 4076717,
        ""IsDrm"": false,
        ""IsUploaded"": false,
        ""MetaList"": [
            {{
                ""MetaKey"": ""Key1"",
                ""MetaValue"": ""hoge""
            }},
            {{
                ""MetaKey"": ""Key2"",
                ""MetaValue"": ""fuga""
            }}
        ]
    }},
    {{
        ""FileId"": ""9d82fa83-9b2c-4b27-8618-b77580874d2f"",
        ""FileName"": ""fuga.png"",
        ""ContentType"": ""image/png"",
        ""FileLength"": 4076718,
        ""IsDrm"": false,
        ""IsUploaded"": true,
        ""MetaList"": [
            {{
                ""MetaKey"": ""Key1"",
                ""MetaValue"": ""fuga""
            }},
            {{
                ""MetaKey"": ""Key2"",
                ""MetaValue"": ""hoge""
            }}
        ]
    }}
]";
        private static string s_expected = s_content;

        private static string s_contentAllField = $@"
[
    {{
        ""FileId"": ""30a15221-e1fd-4b6e-b556-b00cc3618d4a"",
        ""FileName"": ""hoge.jpg"",
        ""ContentType"": ""image/jpeg"",
        ""FileLength"": 4076717,
        ""IsDrm"": false,
        ""IsUploaded"": false,
        ""MetaList"": [
            {{
                ""MetaKey"": ""Key1"",
                ""MetaValue"": ""hoge""
            }},
            {{
                ""MetaKey"": ""Key2"",
                ""MetaValue"": ""fuga""
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
    }},
    {{
        ""FileId"": ""9d82fa83-9b2c-4b27-8618-b77580874d2f"",
        ""FileName"": ""fuga.png"",
        ""ContentType"": ""image/png"",
        ""FileLength"": 4076718,
        ""IsDrm"": false,
        ""IsUploaded"": true,
        ""MetaList"": [
            {{
                ""MetaKey"": ""Key1"",
                ""MetaValue"": ""fuga""
            }},
            {{
                ""MetaKey"": ""Key2"",
                ""MetaValue"": ""hoge""
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
    }}
]";
        private static string s_expectedAllField = $@"
[
    {{
        ""FileId"": ""30a15221-e1fd-4b6e-b556-b00cc3618d4a"",
        ""FileName"": ""hoge.jpg"",
        ""ContentType"": ""image/jpeg"",
        ""FileLength"": 4076717,
        ""IsDrm"": false,
        ""IsUploaded"": false,
        ""MetaList"": [
            {{
                ""MetaKey"": ""Key1"",
                ""MetaValue"": ""hoge""
            }},
            {{
                ""MetaKey"": ""Key2"",
                ""MetaValue"": ""fuga""
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
    }},
    {{
        ""FileId"": ""9d82fa83-9b2c-4b27-8618-b77580874d2f"",
        ""FileName"": ""fuga.png"",
        ""ContentType"": ""image/png"",
        ""FileLength"": 4076718,
        ""IsDrm"": false,
        ""IsUploaded"": true,
        ""MetaList"": [
            {{
                ""MetaKey"": ""Key1"",
                ""MetaValue"": ""fuga""
            }},
            {{
                ""MetaKey"": ""Key2"",
                ""MetaValue"": ""hoge""
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
    }}
]";

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
                action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
                {
                    { new QueryStringKey("Key1"), new QueryStringValue("hoge") },
                    { new QueryStringKey("Key2"), new QueryStringValue("fuga") }
                });

                var target = UnityCore.Resolve<GetAttachFileMetaListActionInjector>();
                target.Target = action;
                target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(allfield ? s_contentAllField : s_content) };
                target.Execute(() => { });

                var query = action.Query;
                query.Dic[new QueryStringKey("$filter")].Value.Is($"MetaList/any(o: o/MetaKey eq 'Key1' and o/MetaValue eq 'hoge') and MetaList/any(o: o/MetaKey eq 'Key2' and o/MetaValue eq 'fuga')");
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
                action.OperationInfo.AttachFileOperation.IsMetaQuery.IsTrue();

                var response = (HttpResponseMessage)target.ReturnValue;
                response.StatusCode.Is(HttpStatusCode.Created);
                var meta = response.Content.ReadAsStringAsync().Result.ToJson();
                meta.ToString().Is((allfield ? s_expectedAllField : s_expected).ToJson().ToString());
            });
        }

        [TestMethod]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void Execute_正常_パラメータなし()
        {
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
                action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { });

                var target = UnityCore.Resolve<GetAttachFileMetaListActionInjector>();
                target.Target = action;
                target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(allfield ? s_contentAllField : s_content) };
                target.Execute(() => { });

                var query = action.Query;
                query.ContainKey("$filter").IsFalse();
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
                action.OperationInfo.AttachFileOperation.IsMetaQuery.IsTrue();

                var response = (HttpResponseMessage)target.ReturnValue;
                response.StatusCode.Is(HttpStatusCode.Created);
                var meta = response.Content.ReadAsStringAsync().Result.ToJson();
                meta.ToString().Is((allfield ? s_expectedAllField : s_expected).ToJson().ToString());
            });
        }

        [TestMethod]
        public void Execute_正常_Keyのみ()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction();
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("Key1"), null } });
            var target = UnityCore.Resolve<GetAttachFileMetaListActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(s_content) };
            target.Execute(() => { });

            var query = action.Query;
            query.ContainKey("$filter").IsFalse();
            query.Dic[new QueryStringKey("$select")].Value.Is($"FileId,FileName,ContentType,FileLength,IsDrm,DrmType,DrmKey,IsUploaded,MetaList");
            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsTrue();

            var response = (HttpResponseMessage)target.ReturnValue;
            response.StatusCode.Is(HttpStatusCode.Created);
            var meta = response.Content.ReadAsStringAsync().Result.ToJson();
            meta.ToString().Is(s_expected.ToJson().ToString());
        }

        [TestMethod]
        public void Execute_正常_NotFound()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateRegistDataAction();
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
                {
                    { new QueryStringKey("Key1"), new QueryStringValue("hoge") },
                    { new QueryStringKey("Key2"), new QueryStringValue("fuga") }
                });

            var target = UnityCore.Resolve<GetAttachFileMetaListActionInjector>();
            target.Target = action;
            target.ReturnValue = new HttpResponseMessage(HttpStatusCode.NotFound);
            target.Execute(() => { });

            var query = action.Query;
            query.Dic[new QueryStringKey("$filter")].Value.Is($"MetaList/any(o: o/MetaKey eq 'Key1' and o/MetaValue eq 'hoge') and MetaList/any(o: o/MetaKey eq 'Key2' and o/MetaValue eq 'fuga')");
            query.Dic[new QueryStringKey("$select")].Value.Is($"FileId,FileName,ContentType,FileLength,IsDrm,DrmType,DrmKey,IsUploaded,MetaList");
            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsTrue();

            var response = (HttpResponseMessage)target.ReturnValue;
            response.StatusCode.Is(HttpStatusCode.NotFound);
        }

        [TestMethod]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void Execute_正常_ODATA_ANY非対応()
        {
            TestContext.Run((bool allfield, bool nullValue) =>
            {
                var perRequestDataContainer = new PerRequestDataContainer();
                perRequestDataContainer.UserId = new Guid();
                UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

                var action = CreateRegistDataAction(false);
                if (allfield || !nullValue)
                {
                    action.XGetInnerAllField = new XGetInnerField(allfield);
                }
                var query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
                {
                    { new QueryStringKey("Key1"), new QueryStringValue("hoge") },
                    { new QueryStringKey("Key2"), new QueryStringValue("fuga") }
                });
                action.Query = query;

                var target = UnityCore.Resolve<GetAttachFileMetaListActionInjector>();
                target.Target = action;
                target.ReturnValue = new HttpResponseMessage(HttpStatusCode.Created) { Content = new StringContent(allfield ? s_contentAllField : s_content) };
                target.Execute(() => { });

                action.Query.IsSameValue(query);
                action.ControllerSchema.IsNotNull();
                action.OperationInfo.IsAttachFileOperation.IsTrue();
                action.OperationInfo.AttachFileOperation.IsMetaQuery.IsTrue();
                if (allfield)
                {
                    action.OperationInfo.AttachFileOperation.QuerySelectFields.Is($"FileId,FileName,ContentType,FileLength,IsDrm,DrmType,DrmKey,IsUploaded,MetaList,IsExternalAttachFile,ExternalAttachFile,_Type,_Vendor_Id,_System_Id,_Reguser_Id,_Regdate,_Upduser_Id,_Upddate,_Version,_partitionkey,_Owner_Id");
                }
                else
                {
                    action.OperationInfo.AttachFileOperation.QuerySelectFields.Is($"FileId,FileName,ContentType,FileLength,IsDrm,DrmType,DrmKey,IsUploaded,MetaList");
                }

                var response = (HttpResponseMessage)target.ReturnValue;
                response.StatusCode.Is(HttpStatusCode.Created);
                var meta = response.Content.ReadAsStringAsync().Result.ToJson();
                meta.ToString().Is((allfield ? s_expectedAllField : s_expected).ToJson().ToString());
            });
        }


        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();


        private QueryAction CreateRegistDataAction(bool isMetaQueriableRepository = true)
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
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("Key1"), new QueryStringValue("hoge") }, { new QueryStringKey("Key2"), new QueryStringValue("fuga") } });
            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.SetupGet(x => x.CanQueryAttachFileMetaByOData).Returns(isMetaQueriableRepository);
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
