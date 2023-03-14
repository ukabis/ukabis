using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;
using JP.DataHub.UnitTest.Com.Extensions;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class AutoKey3DataTest : ApiWebItTestCase
    {
        #region TestData

        private class AutoKey3DataTestData : TestDataBase
        {
            public AutoKey3DataModel Data1 = new AutoKey3DataModel()
            {
                key1 = "key-1",
                key2 = "key-2",
                key3 = "key-3"
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~AutoKey3Data~1~{WILDCARD}"
            };
            public AutoKey3DataModel Data1Get = new AutoKey3DataModel()
            {
                key1 = "key-1",
                key2 = "key-2",
                key3 = "key-3",
                id = $"API~IntegratedTest~AutoKey3Data~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };

            public AutoKey3DataTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
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
        public void AutoKey3DataTest_NormalSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAutoKey3DataApi>();
            var testData = new AutoKey3DataTestData(repository, api.ResourceUrl);

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            var reg = client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected).Result;

            // 登録した１件を取得
            client.GetWebApiResponseResult(api.Get(reg.id)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
        }
    }
}