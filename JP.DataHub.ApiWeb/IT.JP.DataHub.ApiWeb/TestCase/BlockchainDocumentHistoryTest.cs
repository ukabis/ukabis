using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    // 「AGRI_ICT-5946 BaaS短期：ITでブロックチェーンをスキップするようにする」による対応
    [Ignore("廃止：ブロックチェーン")] 
    [TestClass]
    public class BlockchainDocumentHistoryTest : ApiWebItTestCase
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
        public void BlockchainApiTest_DocumentHistoryScenario()
        {
            var resource = new BlockchainWithDocumentHistoryApi(context);

            var versions = new List<KeyValuePair<string, string>>();
            var headerExpect = new Dictionary<string, string[]>() { { "X-DocumentHistory", new string[] { "*" } } };
            void versionSaveAction(string key, IEnumerable<string> vals)
            {
                if (key == "X-DocumentHistory")
                {
                    var historyHeaders = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(vals.Single());
                    historyHeaders.Single().documents.ToList().ForEach(x => versions.Add(new KeyValuePair<string, string>(x.documentKey, x.versionKey)));
                }
            }

            //念のためデータを全消去
            var api = resource.ODataDelete("$top=100");
            context.ActionAndAssert(api.Request, resource.DeleteExpectStatusCodes);

            //データを複数件で登録
            api = resource.RegisterList(resource.data1);
            var reg1Ids = JsonConvert.DeserializeObject<IEnumerable<RegistResultJson>>(context.ActionAndAssert(api.Request, new HttpStatusCode[] { resource.RegistSuccessExpectStatusCode }, headerExpect, null, versionSaveAction));

            //登録したデータを一部更新
            api = resource.Update("AAA", resource.Data1Patch);
            context.ActionAndAssert(api.Request, new HttpStatusCode[] { resource.UpdateSuccessExpectStatusCode }, headerExpect, null, versionSaveAction);

            //データを1件削除
            api = resource.Delete("AAA");
            context.ActionAndAssert(api.Request, resource.DeleteExpectStatusCodes);
            var deletedVersion = context.ActionAndAssert<List<Version>>(resource.GetDocumentVersion(reg1Ids.Single(x => x.id.EndsWith("AAA")).id).Request, resource.GetSuccessExpectStatusCode).OrderByDescending(x => DateTime.Parse(x.CreateDate)).First();
            versions.Add(new KeyValuePair<string, string>(reg1Ids.Single(x => x.id.EndsWith("AAA")).id, deletedVersion.VersionKey));

            //データをodatadeleteで全件削除
            api = resource.ODataDelete("$top=100");
            context.ActionAndAssert(api.Request, resource.DeleteExpectStatusCodes);
            deletedVersion = context.ActionAndAssert<List<Version>>(resource.GetDocumentVersion(reg1Ids.Single(x => x.id.EndsWith("BBB")).id).Request, resource.GetSuccessExpectStatusCode).OrderByDescending(x => DateTime.Parse(x.CreateDate)).First();
            versions.Add(new KeyValuePair<string, string>(reg1Ids.Single(x => x.id.EndsWith("BBB")).id, deletedVersion.VersionKey));
            deletedVersion = context.ActionAndAssert<List<Version>>(resource.GetDocumentVersion(reg1Ids.Single(x => x.id.EndsWith("CCC")).id).Request, resource.GetSuccessExpectStatusCode).OrderByDescending(x => DateTime.Parse(x.CreateDate)).First();
            versions.Add(new KeyValuePair<string, string>(reg1Ids.Single(x => x.id.EndsWith("CCC")).id, deletedVersion.VersionKey));


            //これまで貯めてきたバージョン情報に順々に検証API実行
            foreach (var version in versions)
            {
                context.SetXValidateWithBlockchainVersion(version.Value);
                api = resource.DoValidateWithBlockchain(version.Key);
                context.ActionAndAssert(api.Request, resource.GetSuccessExpectStatusCode, JToken.Parse(resource.ResponseValidateOK), retryPolicy: retryPolicy);
            }
            //履歴データを削除しておく
            resource.DeleteHistories(reg1Ids);
        }
#endif
    }
}
