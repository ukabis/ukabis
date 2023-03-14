using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class JsonSchemaEscapeKeywordsTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void JsonSchemaEscapeKeywordsTest_NormalScenario(Repository repository)
        {
            var escapeKeywordProperties = new List<string> { "Asc", "As", "And", "By", "Between", "Case", "Cast", "Convert", "Cross", "Desc", "Distinct", "Else", "End", "Exists", "For", "From", "Group", "Having", "In", "Inner", "Insert", "Into", "Is", "Join", "Left", "Like", "Not", "On", "Or", "Order", "Outer", "Right", "Select", "Set", "Then", "Top", "Update", "Value", "When" };

            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IJsonSchemaEscapeKeywordsApi>();

            // クリーンアップ
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを登録①
            client.GetWebApiResponseResult(api.RegisterAsString(CreateData(escapeKeywordProperties, "test1"))).Assert(RegisterSuccessExpectStatusCode);

            // データを登録②
            client.GetWebApiResponseResult(api.RegisterAsString(CreateData(escapeKeywordProperties, "test2"))).Assert(RegisterSuccessExpectStatusCode);

            // データ一覧を取得（GetList）
            var jsonResult = client.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            // 予約語のプロパティを持つAPIが参照できることを確認（BadRequestではないこと）
            jsonResult.Count().Is(2);

            // データを取得（Get）
            jsonResult = client.GetWebApiResponseResult(api.GetByQuery("test2")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            escapeKeywordProperties.ForEach(keyword =>
            {
                Assert.AreEqual(jsonResult[keyword].ToString(), "test2");
            });
            // 子要素に予約語ある場合も参照できること
            Assert.AreEqual(jsonResult["Where"]["Value"].ToString(), "test2");

            // ODataでデータを取得（$select句）
            jsonResult = client.GetWebApiResponseResult(api.OData("$select=TestPropertyKey,Asc")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            jsonResult.Count().Is(2);

            // ODataでデータを取得（$filter句）
            jsonResult = client.GetWebApiResponseResult(api.OData("$filter=As eq 'test2'")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            escapeKeywordProperties.ForEach(keyword =>
            {
                Assert.AreEqual(jsonResult[0][keyword].ToString(), "test2");
            });
            Assert.AreEqual(jsonResult[0]["Where"]["Value"].ToString(), "test2");

            // ODataでデータを取得（$filter句、子要素）
            // SQLServerはany未対応のためスキップ
            if (repository != Repository.SqlServer)
            {
                jsonResult = client.GetWebApiResponseResult(api.OData("$filter=With/any(o: o/Value eq 'test2')")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
                escapeKeywordProperties.ForEach(keyword =>
                {
                    Assert.AreEqual(jsonResult[0][keyword].ToString(), "test2");
                });
                Assert.AreEqual(jsonResult[0]["Where"]["Value"].ToString(), "test2");
            }

            // ODataでデータを取得（$orderby句）
            jsonResult = client.GetWebApiResponseResult(api.OData("$orderby=Desc desc")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            escapeKeywordProperties.ForEach(keyword =>
            {
                Assert.AreEqual(jsonResult[0][keyword].ToString(), "test2");
                Assert.AreEqual(jsonResult[1][keyword].ToString(), "test1");
            });
            Assert.AreEqual(jsonResult[0]["Where"]["Value"].ToString(), "test2");
            Assert.AreEqual(jsonResult[1]["Where"]["Value"].ToString(), "test1");

            client.GetWebApiResponseResult(api.Delete("test1")).Assert(DeleteSuccessStatusCode);
            client.GetWebApiResponseResult(api.ODataDelete("$filter=Value eq 'test2'")).Assert(DeleteSuccessStatusCode);

            // データ一が削除されていることを確認
            client.GetWebApiResponseResult(api.GetList()).Assert(NotFoundStatusCode);
        }


        public string CreateData(List<string> selectEscapeKeywords, string value)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            selectEscapeKeywords.ForEach(keyword =>
            {
                sb.AppendLine($"    '{keyword}': '{value}',");
            });
            sb.AppendLine("    'Where': {'Value': '" + value + "'},");
            sb.AppendLine("    'With': [{'Value': '" + value + "'}],");
            sb.AppendLine($"    'TestPropertyKey': '{value}'");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}