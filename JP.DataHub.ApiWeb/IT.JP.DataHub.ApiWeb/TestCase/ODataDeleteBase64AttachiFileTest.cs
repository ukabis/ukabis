using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ODataDeleteBase64AttachiFileTest : ApiWebItTestCase
    {
        #region TestData

        private class ODataDeleteBase64AttachiFileTestData : TestDataBase
        {
            public ODataDeleteModel Data1 = new ODataDeleteModel()
            {
                id = "API~IntegratedTest~ODataDeleteBase64Test~1~AA",
                name = "aaa",
                data = "data1",
                Image = "$Base64(dGVzdA==)"
            };
            public ODataDeleteModel Data1Rdbms = new ODataDeleteModel()
            {
                key = "AA",
                name = "aaa",
                data = "data1",
                Image = "$Base64(dGVzdA==)"
            };
            public ODataDeleteModel Data1Expected = new ODataDeleteModel()
            {
                id = "API~IntegratedTest~ODataDeleteBase64Test~1~AA",
                name = "aaa",
                data = "data1",
                Image = "$Base64(dGVzdA==)",
            };

            public ODataDeleteBase64AttachiFileTestData(Repository repository, string resourceUrl, IntegratedTestClient client) : base(repository, resourceUrl, client: client) { } 
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
        public void ODataDeleteBase64AttachiFileTest_NormalScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IODataDeleteBase64Api>();
            var testData = new ODataDeleteBase64AttachiFileTestData(repository, api.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データを1件登録
            var data = (repository == Repository.SqlServer ? testData.Data1Rdbms : testData.Data1);
            client.GetWebApiResponseResult(api.Regist(data)).Assert(RegisterSuccessExpectStatusCode);

            // データ存在確認
            client.GetWebApiResponseResult(api.OData("$select=id,name,data,Image")).Assert(GetSuccessExpectStatusCode, new List<ODataDeleteModel>() { testData.Data1Expected });

            // ODataDeleteでデータ削除
            client.GetWebApiResponseResult(api.ODataDelete($"$filter=id eq '{testData.Data1.id}'")).Assert(DeleteSuccessStatusCode);

            // データ非存在確認
            client.GetWebApiResponseResult(api.OData($"$filter=id eq '{testData.Data1.id}'")).Assert(NotFoundStatusCode);

            // 掃除
            client.GetWebApiResponseResult(api.ODataDelete($"$top=100")).Assert(DeleteExpectStatusCodes);

            // 全件削除確認
            client.GetWebApiResponseResult(api.OData("$select=id,name,data")).Assert(NotFoundStatusCode);
        }
    }
}