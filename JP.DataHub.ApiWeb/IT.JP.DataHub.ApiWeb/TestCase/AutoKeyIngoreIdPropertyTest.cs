using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class AutoKeyIngoreIdPropertyTest : ApiWebItTestCase
    {
        #region TestData

        private class AutoKeyIngoreIdPropertyTestData : TestDataBase
        {
            public AreaUnitModelEx Data1 = new AreaUnitModelEx()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~AutoKeyOtherColumnSimpleData~1~{WILDCARD}"
            };
            public AreaUnitModelEx Data1Get = new AreaUnitModelEx()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~AutoKeyOtherColumnSimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModelEx DataAutoIdOverrideByGuid = new AreaUnitModelEx()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel DataAutoIdOverrideByGuidRegistExpect = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~AutoKeyOtherColumnSimpleData~1~{WILDCARD}"
            };


            public AutoKeyIngoreIdPropertyTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
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
        public void AutoKeyIngoreIdPropertyTest_NormalScenario(Repository repository)
        {
            // 通常の自動採番はidプロパティに行われる。
            // 下記で定義したAPIはKeyプロパティが自動採番に設定されている。
            // Keyプロパティが正しく値がセットされていること
            // idプロパティの最後のセンテンスがKeyプロパティと同じ値なことを確認する

            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAutoKeyIngoreIdPropertyApi>();
            var testData = new AutoKeyIngoreIdPropertyTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // データ1を１件登録
            var reg = client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected).Result;

            // 登録した１件を取得（Keyプロパティに値が入っていること、それが{id}の最後のセンテンスと同じであることを確認する）
            var keyval = reg.id.Split("~".ToCharArray()).LastOrDefault();
            testData.Data1Get.Key = keyval;
            var id = client.GetWebApiResponseResult(api.Get(keyval)).Assert(GetSuccessExpectStatusCode, testData.Data1Get).Result.id;
            id.Is(reg.id);

            // TODO: SQLSERVERだと現状AdditionalProperties:falseでid指定での登録が無効なのでスキップ
            if (repository != Repository.SqlServer)
            {
                // idを指定して登録したデータ更新できるか確認
                testData.Data1.id = reg.id;
                testData.Data1.AreaUnitName = "newAreaUnitName";
                var newReg = client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected).Result;
                newReg.id.Is(reg.id);

                // 同じデータを再取得
                testData.Data1Get.Key = keyval;
                testData.Data1Get.id = reg.id;
                testData.Data1Get.AreaUnitName = "newAreaUnitName";

                client.GetWebApiResponseResult(api.Get(keyval)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            }


            // リポジトリキーの値としてGuidを指定したデータを登録
            var key = Guid.NewGuid().ToString();
            testData.DataAutoIdOverrideByGuid.Key = key;
            reg = client.GetWebApiResponseResult(api.Regist(testData.DataAutoIdOverrideByGuid)).Assert(RegisterSuccessExpectStatusCode, testData.DataAutoIdOverrideByGuidRegistExpect).Result;

            // 指定したGuidがキーとして採用されていることを確認
            var resultKey = client.GetWebApiResponseResult(api.Get(key)).Assert(GetSuccessExpectStatusCode).Result.Key;
            resultKey.Is(reg.RepositoryKey);
        }
    }
}