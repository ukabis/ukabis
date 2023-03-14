using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class InternalApiTest : ApiWebItTestCase
    {
        #region TestData

        private class InternalApiTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~InternalApi~1~{WILDCARD}"
            };
            public AreaUnitModel Data1Get = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~InternalApi~1~{WILDCARD}",
                _Owner_Id = WILDCARD
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
        public void NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IInternalApi>();
            var testData = new InternalApiTestData();

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(api.RegistInternal(testData.Data1)).AssertErrorCode(BadRequestStatusCode, "E10415");
            client.GetWebApiResponseResult(api.RegistInternalKey(testData.Data1)).AssertErrorCode(BadRequestStatusCode, "E10415");

            // RegistInternalPassedRoslyn APIはRoslynScriptからRegister APIを呼び出しているので正常に登録できる
            // その結果レコード数が1
            client.GetWebApiResponseResult(api.RegistInternalPassedRoslyn(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            client.GetWebApiResponseResult(api.GetCount()).Assert(CountSuccessExpectStatusCode).Result.Count.Is(1);

            // 一旦消す
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // RegistInternalKeyPassedRoslyn APIはRoslynScriptからRegister APIを呼び出しているので正常に登録できる
            // その結果レコード数が1
            client.GetWebApiResponseResult(api.RegistInternalKeyPassedRoslyn(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            client.GetWebApiResponseResult(api.GetCount()).Assert(CountSuccessExpectStatusCode).Result.Count.Is(1);

            // RegistInternalKeyFailRoslyn APIはRoslynScriptからRegister APIを呼び出すがKEYWORDが違っているのでAPIの呼び出しに失敗する
            client.GetWebApiResponseResult(api.RegistInternalKeyFailRoslyn(testData.Data1)).AssertErrorCode(BadRequestStatusCode, "E10416");
        }
    }
}
