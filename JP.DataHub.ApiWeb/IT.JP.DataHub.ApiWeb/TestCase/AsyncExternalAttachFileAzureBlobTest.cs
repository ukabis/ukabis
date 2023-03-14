using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("Async")]
    public class AsyncExternalAttachFileAzureBlobTest : ApiWebItTestCase
    {
        #region TestData

        private class AsyncExternalAttachFileAzureBlobTestData : TestDataBase
        {
            public CreateAttachFileRequestModel Data1 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "az-blob",
                    FilePath = "integratedtest-externalattachfile/TestFiles/tractor_man.png"
                }
            };
            public GetAttachFileResponseModel Data1MetaExpected = new GetAttachFileResponseModel()
            {
                fileId = WILDCARD,
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                isDrm = false,
                isUploaded = false,
                metaList = new List<GetAttachFileResponseModel.MetaInfo>()
                {
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                        metaValue = WILDCARD,
                    },
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                }
            };
            public GetAttachFileResponseModel Data1MetaExExpected = new GetAttachFileResponseModel()
            {
                fileId = WILDCARD,
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                isDrm = false,
                isUploaded = false,
                metaList = new List<GetAttachFileResponseModel.MetaInfo>()
                {
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                        metaValue = WILDCARD,
                    },
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "az-blob",
                    Credentials = new List<string>() { "***" },
                    FilePath = "integratedtest-externalattachfile/TestFiles/tractor_man.png"
                },
                _Version = 1,
                _partitionkey = $"API~IntegratedTest~AsyncAttachfile~{WILDCARD}",
                _Type = "API~IntegratedTest~AsyncAttachfile",
                _Owner_Id = WILDCARD,
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD
            };

            public AsyncExternalAttachFileAzureBlobTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl)
            {
            }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AsyncExternalAttachFileAzureBlobScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncExternalAttachfileAzureBlobApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncExternalAttachFileAzureBlobTestData(repository, api.ResourceUrl);

            var testKey = Guid.NewGuid().ToString();
            var credential = AppConfig.ExternalStorageAzureBlob;
            testData.Data1.metaList.First().metaValue = testKey;
            testData.Data1.ExternalAttachFile.Credentials = new List<string>() { credential };

            // 【非同期】メタ作成
            var creatRequest = api.CreateAttachFile(testData.Data1);
            var requestId = client.ExecAsyncApiJson(creatRequest);
            var fileId = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<CreateAttachFileResponseModel>(RegisterSuccessExpectStatusCode).fileId;

            // 【非同期】FileDownload
            var downloadRequest = api.GetAttachFile(fileId);
            requestId = client.ExecAsyncApiJson(downloadRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode, GetContentsByte(client.SmallContentsPath));

            // 【非同期】MetaGet
            var metaRequest = api.GetAttachFileMeta(fileId);
            requestId = client.ExecAsyncApiJson(metaRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult(GetSuccessExpectStatusCode, testData.Data1MetaExpected);

            // 【非同期】MetaGet(X-GetInternalAllField)
            metaRequest = api.GetAttachFileMeta(fileId);
            requestId = client.ExecAsyncApiJson(metaRequest, null, new Dictionary<string, string[]>() { { HeaderConst.X_GetInternalAllField, "true" } });
            client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult(GetSuccessExpectStatusCode, testData.Data1MetaExExpected);

            // 【非同期】MetaGetList
            var metaListRequest = api.GetAttachFileMetaList($"TestKey={testKey}");
            requestId = client.ExecAsyncApiJson(metaListRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult(GetSuccessExpectStatusCode, new List<GetAttachFileResponseModel>() { testData.Data1MetaExpected });

            // 【非同期】MetaGetList(X-GetInternalAllField)
            metaListRequest = api.GetAttachFileMetaList($"TestKey={testKey}");
            requestId = client.ExecAsyncApiJson(metaListRequest, null, new Dictionary<string, string[]>() { { HeaderConst.X_GetInternalAllField, "true" } });
            client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult(GetSuccessExpectStatusCode, new List<GetAttachFileResponseModel>() { testData.Data1MetaExExpected });

            // 【非同期】FileDelete
            var delRequest = api.DeleteAttachFile(fileId);
            requestId = client.ExecAsyncApiJson(delRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(DeleteSuccessStatusCode);

            // MetaGet (削除確認)
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileId)).Assert(NotFoundStatusCode);

            // FileDownload(削除確認)
            client.GetWebApiResponseResult(api.GetAttachFile(fileId)).Assert(NotFoundStatusCode);
        }
    }
}

