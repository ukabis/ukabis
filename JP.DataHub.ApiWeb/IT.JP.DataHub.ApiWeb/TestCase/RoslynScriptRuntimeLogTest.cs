using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
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
    [TestCategory("Roslyn")]
    public class RoslynScriptRuntimeLogTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void RoslynScriptRuntimeLogTest_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRoslynScriptRuntimeLogApi>();

            // 自ベンダーでログAPI実行
            var response = client.GetWebApiResponseResult(api.WriteLog()).Assert(GetSuccessExpectStatusCode);
            response.ContentString.Is("OK");
            Assert.IsTrue(response.Headers.Contains(HeaderConst.X_ScriptRuntimeLogId));
            var logId = response.Headers.GetValues(HeaderConst.X_ScriptRuntimeLogId).Single();

            // 自ベンダーでメタデータの取得
            var staticManageApi = UnityCore.Resolve<IScriptRuntimeLogManagementApi>();
            var metaDataJson = client.GetWebApiResponseResult(staticManageApi.GetLogMetaData(logId)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            metaDataJson.Value<bool>("IsError").Is(false);

            // 自ベンダーでファイルの取得
            var staticApi = UnityCore.Resolve<IScriptRuntimeLogApi>();
            response = client.GetWebApiResponseResult(staticApi.GetLogFile(logId)).Assert(GetSuccessExpectStatusCode);
            Assert.IsTrue(response.ContentString.Contains("/API/IntegratedTest/RoslynScriptRuntimeLogApi"));

            // 他ベンダーでメタデータの取得 取得できないことを確認
            var clientOther = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystemB");
            clientOther.GetWebApiResponseResult(staticManageApi.GetLogMetaData(logId)).Assert(NotFoundStatusCode);

            // 他ベンダーでファイルの取得 取得できないことを確認
            clientOther.GetWebApiResponseResult(staticApi.GetLogFile(logId)).Assert(NotFoundStatusCode);


            // クリーンアップ
            client.GetWebApiResponseResult(staticManageApi.DeleteLogMetaData(logId));
            client.GetWebApiResponseResult(staticApi.DeleteLogFile(logId));
        }

        [TestMethod]
        [Ignore]//このテストを実行するとDatahubで例外が発生しアラートが飛んでしまうので
        public void RoslynScriptRuntimeLogTest_ErrorScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRoslynScriptRuntimeLogApi>();

            // 自ベンダーで例外ログAPI実行
            api.AddHeaders.Add(HeaderConst.X_ScriptRuntimeLogException, "true");
            var response = client.GetWebApiResponseResult(api.ExceptionLog()).AssertErrorCode(InternalServerErrorStatusCode, "E50402");
            Assert.IsTrue(response.Headers.Contains(HeaderConst.X_ScriptRuntimeLogId));
            var logId = response.Headers.GetValues(HeaderConst.X_ScriptRuntimeLogId).Single();

            // 自ベンダーでメタデータの取得
            var staticManageApi = UnityCore.Resolve<IScriptRuntimeLogManagementApi>();
            var metaDataJson = client.GetWebApiResponseResult(staticManageApi.GetLogMetaData(logId)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            metaDataJson.Value<bool>("IsError").Is(true);

            // 自ベンダーでファイルの取得
            var staticApi = UnityCore.Resolve<IScriptRuntimeLogApi>();
            response = client.GetWebApiResponseResult(staticApi.GetLogFile(logId)).Assert(GetSuccessExpectStatusCode);

            // 他ベンダーでメタデータの取得 取得できないことを確認
            var clientOther = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystemB");
            clientOther.GetWebApiResponseResult(staticManageApi.GetLogMetaData(logId)).Assert(NotFoundStatusCode);

            // 他ベンダーでファイルの取得 取得できないことを確認
            clientOther.GetWebApiResponseResult(staticApi.GetLogFile(logId)).Assert(NotFoundStatusCode);


            // クリーンアップ
            client.GetWebApiResponseResult(staticManageApi.DeleteLogMetaData(logId));
            client.GetWebApiResponseResult(staticApi.DeleteLogFile(logId));
        }
    }
}

