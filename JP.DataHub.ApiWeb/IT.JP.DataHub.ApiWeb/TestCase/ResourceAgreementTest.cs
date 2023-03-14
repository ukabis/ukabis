using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ResourceAgreementTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void ResourceAgreement_CallingFromUserSenario()
        {
            // 所有ベンダー以外から呼ばれた場合は同意と承認が必要
            var client = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem");

            // 同意も承認もしていないケース
            var resource1 = UnityCore.Resolve<IResourceAgreement1Api>();
            client.GetWebApiResponseResult(resource1.OData()).Assert(ForbiddenExpectStatusCode);

            // 同意のみしているケース
            var resource2 = UnityCore.Resolve<IResourceAgreement2Api>();
            client.GetWebApiResponseResult(resource2.OData()).Assert(ForbiddenExpectStatusCode);

            // 同意も承認をもしているケース
            var resource3 = UnityCore.Resolve<IResourceAgreement3Api>();
            client.GetWebApiResponseResult(resource3.OData()).Assert(GetExpectStatusCodes);
        }

        [TestMethod]
        public void ResourceAgreement_CallingFromProviderSenario()
        {
            // 所有ベンダーから呼ばれた場合は同意と承認が無くてもAPIを呼び出せる
            var client = new IntegratedTestClient(AppConfig.Account);

            // 同意も承認もしていないケース
            var resource1 = UnityCore.Resolve<IResourceAgreement1Api>();
            client.GetWebApiResponseResult(resource1.OData()).Assert(GetExpectStatusCodes);

            // 同意のみしているケース
            var resource2 = UnityCore.Resolve<IResourceAgreement2Api>();
            client.GetWebApiResponseResult(resource2.OData()).Assert(GetExpectStatusCodes);

            // 同意も承認をもしているケース
            var resource3 = UnityCore.Resolve<IResourceAgreement3Api>();
            client.GetWebApiResponseResult(resource3.OData()).Assert(GetExpectStatusCodes);
        }

    }
}
