using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("Logging")]
    public class LoggingAttachfileTest : ApiWebItTestCase
    {
        protected RetryPolicy<HttpResponseMessage> retryLoggingPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    return true;
                }

                var logging = JsonConvert.DeserializeObject<LoggingModel>(r.Content.ReadAsStringAsync().Result);
                return logging.HttpStatusCode == "0";
            })
            .WaitAndRetry(9, i => TimeSpan.FromSeconds(40));

        #region TestData

        private class LoggingAttachfileTestData : TestDataBase
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
                        metaKey = "TestKey"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
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
        public void LoggingAttachFileNormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ILoggingAttachfileApi>();
            var testData = new LoggingAttachfileTestData();

            var testKey = Guid.NewGuid().ToString();
            testData.Data1.metaList.First().metaValue = testKey;
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            var response_create = client.GetWebApiResponseResult(api.CreateAttachFile(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            var id_create = GetLoggingIdDictionary(response_create.Headers.GetValues("LoggingLogId")).First().Key;
            var fileid = response_create.Result.fileId;

            // FileUpload
            client.ChunkUploadAttachFile(fileid, client.SmallContentsPath, api);

            // FileDownload
            var response_download = client.GetWebApiResponseResult(api.GetAttachFile(fileid));
            var id_download = GetLoggingIdDictionary(response_download.Headers.GetValues("LoggingLogId")).First().Key;

            // FileDelete
            var response_delete = client.GetWebApiResponseResult(api.DeleteAttachFile(fileid));
            var id_delete = GetLoggingIdDictionary(response_delete.Headers.GetValues("LoggingLogId")).First().Key;


            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageLoggingApi>();

            var loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_create), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "201", "post", testData.Data1.ToJsonString(), response_create, loginfo);

            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_download), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "200", "get", null, response_download, loginfo, false, true);

            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_delete), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "204", "delete", null, response_delete, loginfo);
        }

        [TestMethod]
        public void LoggingAttachFileBase64_NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ILoggingAttachfileApi>();
            var testData = new LoggingAttachfileTestData();

            var testKey = Guid.NewGuid().ToString();
            testData.Data1.metaList.First().metaValue = testKey;
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            var data = new AttachFileBase64Model()
            {
                file = "$Base64(44G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044KI)",
                name = "hogehoge"
            };
            var registResponse = client.GetWebApiResponseResult(api.RegistBase64(data));
            var id_regist = GetLoggingIdDictionary(registResponse.Headers.GetValues("LoggingLogId")).First().Key;

            var getResponse = client.GetWebApiResponseResult(api.GetAllBase64());
            var id_get = GetLoggingIdDictionary(getResponse.Headers.GetValues("LoggingLogId")).First().Key;


            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageLoggingApi>();

            var loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_regist), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "201", "post", testData.Data1.ToJsonString(), registResponse, loginfo);

            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_get), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "200", "get", null, getResponse, loginfo);
        }
    }
}
