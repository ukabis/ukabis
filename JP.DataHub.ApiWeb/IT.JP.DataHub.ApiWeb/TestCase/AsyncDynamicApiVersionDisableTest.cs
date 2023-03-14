using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("Async")]
    public class AsyncDynamicApiVersionDisableTest : ApiWebItTestCase
    {
        #region TestData

        private class AsyncDynamicApiVersionDisableTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~VersionDisable~Async~1~{WILDCARD}"
            };
            public AreaUnitModel Data1Get = new AreaUnitModel()
            {
                id = $"API~IntegratedTest~VersionDisable~Async~1~{WILDCARD}",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public AreaUnitModel Data1Patch = new AreaUnitModel()
            {
                ConversionSquareMeters = 2
            };
            public AreaUnitModel Data1Patched = new AreaUnitModel()
            {
                id = $"API~IntegratedTest~VersionDisable~Async~1~{WILDCARD}",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 2
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
        public void AsyncDynamicApiVersionDisableTest_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDisableVersionAsyncApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiVersionDisableTestData();

            // クリーンアップ
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データをAsyncで登録
            var regRequest = api.Regist(testData.Data1);
            var requestId = client.ExecAsyncApiJson(regRequest);
            var reg1 = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 登録したデータ1件をAsyncでページングなしでGet
            var getRequest = api.Get("AA");
            var requestId2 = client.ExecAsyncApiJson(getRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId2)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // 結果をAsyncで変更
            var updRequest = api.Update("AA", testData.Data1Patch);
            var requestId3 = client.ExecAsyncApiJson(updRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId3)).Assert(UpdateSuccessExpectStatusCode);

            // 正しく変更されたか確認
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Patched);

            // データをAsyncで削除
            var delRequest = api.Delete("AA");
            var requestId4 = client.ExecAsyncApiJson(delRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId4)).Assert(DeleteSuccessStatusCode);

            // 削除されたか確認
            client.GetWebApiResponseResult(api.Get("AA")).Assert(NotFoundStatusCode);
        }
    }
}
