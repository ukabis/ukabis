using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("Roslyn")]
    public class RoslynScriptTest : ApiWebItTestCase
    {
        #region TestData

        private class RoslynScriptTestData : TestDataBase
        {
            public List<AreaUnitModel> Data1 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 10
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 1000
                }
            };
            public List<RegisterResponseModel> Data1RegistExpected = new List<RegisterResponseModel>()
            {
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~Roslyn~1~AA"
                },
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~Roslyn~1~BB"
                },
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~Roslyn~1~CC"
                }
            };
            public List<AreaUnitModel> DataScriptTestNameOnry = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitName = "aaa"
                },
                new AreaUnitModel()
                {
                    AreaUnitName = "bbb"
                },
                new AreaUnitModel()
                {
                    AreaUnitName = "ccc"
                }
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
        public void RoslynScriptTest_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRoslynScriptApi>();
            var testData = new RoslynScriptTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 全データを登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // ScriptTest AreaUnitNameのみ取得
            client.GetWebApiResponseResult(api.ScriptTestGetNameOnly()).Assert(GetSuccessExpectStatusCode, testData.DataScriptTestNameOnry);
        }

        [TestMethod]
        public void RoslynScriptTest_NormalScenario_Roslyn_認証引継ぎ()
        {
            var api = UnityCore.Resolve<IRoslynScriptApi>();

            // 認証ありAPI(Roslyn)から認証ありAPIの呼び出し
            var client = new IntegratedTestClient(AppConfig.Account);
            client.GetWebApiResponseResult(api.ScriptEntryPointWithAuth()).Assert(GetSuccessExpectStatusCode);

            // ベンダーシステム認証なしAPI(Roslyn)から認証ありAPIの呼び出し
            client = new IntegratedTestClient(AppConfig.Account, null);
            var result = client.GetWebApiResponseResult(api.ScriptEntryPointWithoutAuth()).AssertErrorCode(ForbiddenExpectStatusCode, "E02402").RawContentString.ToJson();
            result["instance"].Is("/API/IntegratedTest/Roslyn/ScripCallAuth");
        }
    }
}

