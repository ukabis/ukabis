using System;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("Logging")]
    public class LoggingTest : ApiWebItTestCase
    {
        protected RetryPolicy<HttpResponseMessage> retryLoggingPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    return true;
                }

                var result = r.Content.ReadAsStringAsync().Result;
                var logging = JsonConvert.DeserializeObject<LoggingModel>(result);
                return logging.HttpStatusCode == "0";
            })
            .WaitAndRetry(9, i => TimeSpan.FromSeconds(40));

        #region TestData

        private class LoggingTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "AAName",
                ConversionSquareMeters = 10
            };
            public AreaUnitModel Data1Update = new AreaUnitModel()
            {
                AreaUnitName = "UpName",
                ConversionSquareMeters = 10
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
        public void LoggingTestNormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ILoggingApi>();
            var testData = new LoggingTestData();

            // 最初に全削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // GetInternalAllFieldを指定しない
            var testResult = client.GetWebApiResponseResult(api.Register(testData.Data1));
            Assert.IsFalse(testResult.Headers.Contains("LoggingLogId"));

            // 以降のテストはGetInternalAllFieldを指定する
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");

            // Loggingに時間がかかるためにテストを行うAPIを先にすべて実行する
            // Register
            var response_registre = client.GetWebApiResponseResult(api.Register(testData.Data1));
            var id_registre = GetLoggingIdDictionary(response_registre.Headers.GetValues("LoggingLogId")).First().Key;

            // Update
            var response_update = client.GetWebApiResponseResult(api.Update(testData.Data1.AreaUnitCode, testData.Data1Update));
            var id_update = GetLoggingIdDictionary(response_update.Headers.GetValues("LoggingLogId")).First().Key;

            // Get
            var response_get = client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode));
            var id_get = GetLoggingIdDictionary(response_get.Headers.GetValues("LoggingLogId")).First().Key;

            // Delete
            var response_delete = client.GetWebApiResponseResult(api.Delete(testData.Data1.AreaUnitCode));
            var id_delete = GetLoggingIdDictionary(response_delete.Headers.GetValues("LoggingLogId")).First().Key;

            // Gateway
            var response_gateway = client.GetWebApiResponseResult(api.Gateway());
            var id_gateway = GetLoggingIdDictionary(response_gateway.Headers.GetValues("LoggingLogId")).First().Key;

            // Roslynで内部からAPIを呼んでいるためそれもロギングされていることを確認する
            // RegisterRoslyn
            var response_regist_roslyn = client.GetWebApiResponseResult(api.RegisterRoslyn(testData.Data1));
            var id_registr_roslyn = GetLoggingIdDictionary(response_regist_roslyn.Headers.GetValues("LoggingLogId"));

            // GetRoslyn
            var response_get_roslyn = client.GetWebApiResponseResult(api.GetRoslyn());
            var id_get_roslyn = GetLoggingIdDictionary(response_get_roslyn.Headers.GetValues("LoggingLogId"));

            // DeleteRoslyn
            var response_delete_roslyn = client.GetWebApiResponseResult(api.DeleteRoslyn());
            var id_delete_roslyn = GetLoggingIdDictionary(response_delete_roslyn.Headers.GetValues("LoggingLogId"));


            // Loggingチェック
            // Loggingテーブルにデータが入るまでリトライをかける。
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageLoggingApi>();
            // Register
            var loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_registre), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "201", "post", testData.Data1.ToJsonString(), response_registre, loginfo);
            // Update
            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_update), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "204", "patch", testData.Data1Update.ToJsonString(), response_update, loginfo);
            // Get
            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_get), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "200", "get", null, response_get, loginfo);
            // Delete
            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_delete), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "204", "delete", null, response_delete, loginfo);
            // Gateway
            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_gateway), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "200", "get", null, response_gateway, loginfo, true);


            // RegisterRoslyn
            // Roslynは内部で呼ばれている分もチェックする
            var id_registr_roslyn_outer = id_registr_roslyn.Where(x => x.Value.Contains("Roslyn")).First().Key;
            var id_registr_roslyn_inner = id_registr_roslyn.Where(x => !x.Value.Contains("Roslyn")).First().Key;

            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_registr_roslyn_outer), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "201", "post", testData.Data1.ToJsonString(), response_regist_roslyn, loginfo);
            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_registr_roslyn_inner), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "201", "post", testData.Data1.ToJsonString(), response_regist_roslyn, loginfo);

            // GetRoslyn
            var id_get_roslyn_outer = id_get_roslyn.Where(x => x.Value.Contains("Roslyn")).First().Key;
            var id_get_roslyn_inner = id_get_roslyn.Where(x => !x.Value.Contains("Roslyn")).First().Key;

            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_get_roslyn_outer), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "200", "get", null, response_get_roslyn, loginfo);
            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_get_roslyn_inner), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "200", "get", null, response_get_roslyn, loginfo);

            // DeleteRoslyn
            var id_delete_roslyn_outer = id_delete_roslyn.Where(x => x.Value.Contains("Roslyn")).First().Key;
            var id_delete_roslyn_inner = id_delete_roslyn.Where(x => !x.Value.Contains("Roslyn")).First().Key;

            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_delete_roslyn_outer), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "204", "delete", null, response_delete_roslyn, loginfo);
            loginfo = clientM.GetWebApiResponseResult(manageApi.GetLog(id_delete_roslyn_inner), retryLoggingPolicy).Result;
            clientM.VerificationLogging(manageApi, "204", "delete", null, response_delete_roslyn, loginfo);
        }
    }
}
