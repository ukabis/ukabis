using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ApiHelperCorrectQueryStringTest : ApiWebItTestCase
    {
        #region TestData

        private class ApiHelperCorrectQueryStringTestData : TestDataBase
        {
            public AcceptDataModel Data1 = new AcceptDataModel()
            {
                Code = "AA",
                Name = "aaa"
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
        public void ApiHelperCorrectQueryStringTest_NormalNoneSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IApiHelperCorrectQueryStringApi>();
            var testData = new ApiHelperCorrectQueryStringTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 取得(クエリストリングが分かれている方）
            client.GetWebApiResponseResult(api.GetQuery("aaa")).Assert(GetSuccessExpectStatusCode, testData.Data1);

            // 取得(クエリストリングが分かれてない方）
            client.GetWebApiResponseResult(api.GetUrl("aaa")).Assert(GetSuccessExpectStatusCode, testData.Data1);
        }
    }
}
