using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class SelectBuildTest : ApiWebItTestCase
    {
        #region TestData

        private class SelectBuildTestData : TestDataBase
        {
            protected override string BaseRepositoryKeyPrefix { get; set; } = "API~IntegratedTest~SelectBuildNone";

            public AreaUnitModelEx Data1 = new AreaUnitModelEx()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                DummyItem = "dummy"
            };
            public AreaUnitModelEx Data1Get = new AreaUnitModelEx()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                DummyItem = "dummy"
            };
            public AreaUnitModelEx Data1GetForUseSchema = new AreaUnitModelEx()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public AreaUnitModelEx Data1GetSelected = new AreaUnitModelEx()
            {
                AreaUnitName = "aaa"
            };

            public SelectBuildTestData(string resourceUrl) : base(Repository.Default, resourceUrl) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void SelectBuildTest_NormalNoneSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ISelectBuildNoneApi>();
            var testData = new SelectBuildTestData(api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 全取得（1レコードのはず）
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get });
        }

        [TestMethod]
        public void SelectBuildTest_NormalUseSchemaSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ISelectBuildUseSchemaApi>();
            var testData = new SelectBuildTestData(api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 全取得（1レコードのはず）
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1GetForUseSchema });
        }

        [TestMethod]
        public void SelectBuildTest_NormalUseQuerySenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ISelectBuildUseQueryApi>();
            var testData = new SelectBuildTestData(api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 全取得（1レコードのはず）
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1GetSelected });
        }

        [TestMethod]
        public void SelectBuildTest_NormalAdditionalPropertiesSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ISelectBuildAdditionalPropertiesApi>();
            var testData = new SelectBuildTestData(api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 全取得（1レコードのはず）
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get });
        }
    }
}
