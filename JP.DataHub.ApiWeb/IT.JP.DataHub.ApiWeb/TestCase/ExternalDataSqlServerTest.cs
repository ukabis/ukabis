using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    /// <summary>
    /// AI環境の旧SqlServerDataStoreRepository向けテーブルの互換性確認(Queryのみ対応)
    /// </summary>
    [Ignore("廃止：旧SQLServerリポジトリ")]
    [TestClass]
    public class ExternalDataSqlServerTest : ApiWebItTestCase
    {
        #region TestData

        private class ExternalDataSqlServerTestData : TestDataBase
        {
            public AcceptDataModel Data1 = new AcceptDataModel()
            {
                Code = "123",
                id = "hogehoge",
                Name = "abc"
            };

            public AcceptDataModel Data2 = new AcceptDataModel()
            {
                Code = "456",
                id = "hugahuga",
                Name = "def"
            };

            public AcceptDataModel Data1Selected = new AcceptDataModel()
            {
                id = "hogehoge",
                Name = "abc"
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
        public void ExternalDataSqlServerTest_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IExternalDataSqlServerApi>();
            var testData = new ExternalDataSqlServerTestData();

            client.GetWebApiResponseResult(api.Get("123")).Assert(GetSuccessExpectStatusCode, testData.Data1);
            client.GetWebApiResponseResult(api.Get("456")).Assert(GetSuccessExpectStatusCode, testData.Data2);

            client.GetWebApiResponseResult(api.OData("$filter=Code eq '123'")).Assert(GetSuccessExpectStatusCode, new List<AcceptDataModel>() { testData.Data1 });
            client.GetWebApiResponseResult(api.OData("$filter=Code eq '456'")).Assert(GetSuccessExpectStatusCode, new List<AcceptDataModel>() { testData.Data2 });

            // 現状、Countの結果もjson配列形式で返ってくる
            var result = client.GetWebApiResponseResult(api.OData("$count=true")).Assert(GetSuccessExpectStatusCode).RawContentString;
            var expect3Msg = "[{\"\": 2}]";
            var expect3 = JToken.Parse(expect3Msg);
            JToken.Parse(expect3Msg).Is(JToken.Parse(result));

            client.GetWebApiResponseResult(api.OData("$orderby=Code desc&$top=1")).Assert(GetSuccessExpectStatusCode, new List<AcceptDataModel>() { testData.Data2 });
            client.GetWebApiResponseResult(api.OData("$filter=Code eq '123'&$select=id,Name")).Assert(GetSuccessExpectStatusCode, new List<AcceptDataModel>() { testData.Data1Selected });
        }
    }
}
