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
    public class AsyncDynamicApiAttachFileTest : ApiWebItTestCase
    {
        #region TestData

        private class AsyncDynamicApiAttachFileTestData : TestDataBase
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
                }
            };
            public GetAttachFileResponseModel Data1MetaExpected = new GetAttachFileResponseModel()
            {
                fileId = WILDCARD,
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                isDrm = false,
                isUploaded = true,
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
        public void AsyncAttachFileScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncAttachfileApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiAttachFileTestData();

            var testKey = Guid.NewGuid().ToString();
            testData.Data1.metaList.First().metaValue = testKey;

            // 【非同期】メタ作成
            var creatRequest = api.CreateAttachFile(testData.Data1);
            var requestId = client.ExecAsyncApiJson(creatRequest);
            var fileId = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<CreateAttachFileResponseModel>(RegisterSuccessExpectStatusCode).fileId;

            // FileUpload（非同期実行するには非同期バッチの修正が必要）
            client.ChunkUploadAttachFile(fileId, client.SmallContentsPath, api);

            // 【非同期】FileDownload
            var downloadRequest = api.GetAttachFile(fileId);
            requestId = client.ExecAsyncApiJson(downloadRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode, GetContentsByte(client.SmallContentsPath));

            // 【非同期】MetaGet
            var metaRequest = api.GetAttachFileMeta(fileId);
            requestId = client.ExecAsyncApiJson(metaRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult(GetSuccessExpectStatusCode, testData.Data1MetaExpected);

            // 【非同期】MetaGetList
            var metaListRequest = api.GetAttachFileMetaList($"TestKey={testKey}");
            requestId = client.ExecAsyncApiJson(metaListRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult(GetSuccessExpectStatusCode, new List<GetAttachFileResponseModel>() { testData.Data1MetaExpected });

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

