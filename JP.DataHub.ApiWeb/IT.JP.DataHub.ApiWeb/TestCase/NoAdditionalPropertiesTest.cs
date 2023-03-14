using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class NoAdditionalPropertiesTest : ApiWebItTestCase
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
        public void NoAdditionalPropertiesTest_AddtionalProperty無し(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<INoAdditionalPropertiesApi>();

            // 既定のプロパティのみ（エラーなし）
            var data = new NoAdditionalPropertiesModel()
            {
                AreaUnitCode = "aa",
                AreaUnitName = "aa-name",
                ConversionSquareMeters = "1"
            };
            client.GetWebApiResponseResult(api.Register(data)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void NoAdditionalPropertiesTest_AddtionalProperty_idあり(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<INoAdditionalPropertiesApi>();

            // 既定以外（id）のプロパティあり（以前ならエラーになったが、今回からはエラーにならない）
            var data = new NoAdditionalPropertiesModel()
            {
                AreaUnitCode = "aa",
                AreaUnitName = "aa-name",
                ConversionSquareMeters = "1",
                id = "hoge"
            };
            client.GetWebApiResponseResult(api.Register(data)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void NoAdditionalPropertiesTest_AddtionalProperty_id_testあり(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<INoAdditionalPropertiesApi>();

            // 既定以外（id）のプロパティあり（以前ならエラー、今回もエラー=Keyプロパティがあるため）
            var data = new NoAdditionalPropertiesModel()
            {
                AreaUnitCode = "aa",
                AreaUnitName = "aa-name",
                ConversionSquareMeters = "1",
                id = "hoge",
                test = 1
            };
            client.GetWebApiResponseResult(api.Register(data)).AssertErrorCode(RegisterErrorExpectStatusCode, "E10402");
        }
    }
}