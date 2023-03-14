using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [Ignore("廃止：ブロックチェーン")] // 「AGRI_ICT-5946 BaaS短期：ITでブロックチェーンをスキップするようにする」による対応
    [TestClass]
    public class BlockchainApiTest : ApiWebItTestCase
    {
#if false
        private WebApiContext context = null;

        //2000msおきに9回までリトライ
        private RetryPolicy<HttpResponseMessage> retryPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    return false;
                }
                var c = r.Content.ReadAsStringAsync().Result;
                return JToken.Parse(c)["Result"].ToString() == "NG";
            }
        )
            .WaitAndRetry(9, i => TimeSpan.FromMilliseconds(2000));


        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Initialize();
            context = new WebApiContext(TestContext);

        }

        [TestMethod]
        public void BlockchainApiTest_NormalScenario()
        {
            const int delayms = 200;
            var resource = new BlockchainApi(context);

            //念のためデータを全消去
            var api = resource.ODataDelete("$top=100");
            context.ActionAndAssert(api.Request, resource.DeleteExpectStatusCodes);

            //データを複数件で登録
            api = resource.RegisterList(resource.data1);
            var reg1Ids = context.ActionAndAssert<IEnumerable<RegistResultJson>>(api.Request, resource.RegistSuccessExpectStatusCode);
            Task.Delay(TimeSpan.FromMilliseconds(delayms));

            //登録した全データについて検証OKになるまでValidate実行
            foreach (var regId in reg1Ids)
            {
                api = resource.DoValidateWithBlockchain(regId.id);
                context.ActionAndAssert(api.Request, resource.GetSuccessExpectStatusCode, JToken.Parse(resource.ResponseValidateOK), retryPolicy: retryPolicy);
            }

            //登録したデータを一部更新
            api = resource.Update("AAA", resource.Data1Patch);
            context.ActionAndAssert(api.Request, resource.UpdateSuccessExpectStatusCode);

            //変更したデータについてValidate実行
            string reg1id = reg1Ids.Single(x => x.id.EndsWith("AAA")).id;
            api = resource.DoValidateWithBlockchain(reg1id);
            context.ActionAndAssert(api.Request, resource.GetSuccessExpectStatusCode, JToken.Parse(resource.ResponseValidateOK), retryPolicy: retryPolicy);


            //データを1件削除
            api = resource.Delete("AAA");
            context.ActionAndAssert(api.Request, resource.DeleteSuccessStatusCode);

            //削除したデータについて検証
            api = resource.DoValidateWithBlockchain(reg1id);
            context.ActionAndAssert(api.Request, resource.GetSuccessExpectStatusCode, JToken.Parse(resource.ResponseValidateOK), retryPolicy: retryPolicy);

            //データをodatadeleteで全件削除
            api = resource.ODataDelete("$top=100");
            context.ActionAndAssert(api.Request, resource.DeleteSuccessStatusCode);

            //最初に登録した全データについて検証OKになるまでValidate実行
            foreach (var regId in reg1Ids)
            {
                api = resource.DoValidateWithBlockchain(regId.id);
                context.ActionAndAssert(api.Request, resource.GetSuccessExpectStatusCode, JToken.Parse(resource.ResponseValidateOK), retryPolicy: retryPolicy);
            }
        }

        [TestMethod]
        public void BlockchainApiTest_AttachFileScenario()
        {
            const int delayms = 200;
            var resource = new BlockchainApi(context);

            //メタ作成
            var api = resource.CreateAttachFile(resource.DataAttachFile);
            var fileId = context.ActionAndAssert<CreateAttachFileResult>(api.Request, resource.RegistSuccessExpectStatusCode);

            //ファイルのアップロード
            resource.ChunkUploadAttachFile(fileId.FileId, resource.SmallContentsPath);
            Task.Delay(TimeSpan.FromMilliseconds(delayms));

            //添付ファイルの情報を検証する
            api = resource.DoValidateAttachFileWithBlockchain(fileId.FileId);
            context.ActionAndAssert(api.Request, resource.GetSuccessExpectStatusCode, JToken.Parse(resource.ResponseValidateOK), retryPolicy: retryPolicy);

            //ファイルの削除
            api = resource.DeleteAttachFile(fileId.FileId);
            context.ActionAndAssert(api.Request, resource.DeleteSuccessStatusCode);

            //削除した情報が記録されているかを検証する
            api = resource.DoValidateAttachFileWithBlockchain(fileId.FileId);
            context.ActionAndAssert(api.Request, resource.GetSuccessExpectStatusCode, JToken.Parse(resource.ResponseValidateOK), retryPolicy: retryPolicy);

        }
#endif
    }
}

