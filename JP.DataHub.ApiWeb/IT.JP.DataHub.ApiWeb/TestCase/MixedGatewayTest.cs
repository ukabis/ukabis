using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class MixedGatewayTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// 通常のCosmosDBがあるRepositoryとGatewayが混在した場合
        /// その両方がアクセスできるか？
        /// AOPでgatewayのみのResourceで透過APIを作るようにしたときの影響で、両方混在しているのがダメになってしまうケースの確認
        /// </summary>
        [TestMethod]
        public void MixedRepositoryAndGateway()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IMixedGatewayApi>();

            client.GetWebApiResponseResult(api.OData()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.gwimage()).Assert(GetExpectStatusCodes);
        }
    }
}
