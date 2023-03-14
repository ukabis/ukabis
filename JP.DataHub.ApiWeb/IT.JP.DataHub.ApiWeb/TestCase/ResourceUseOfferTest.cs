using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ResourceUseOfferTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void Offer()
        {
            var client = new IntegratedTestClient("dataofferonly", "SmartFoodChainDataOfferOnly");

            // 自分で定義したものを呼び出せる
            var api = UnityCore.Resolve<IDataOfferApi>();
            client.GetWebApiResponseResult(api.OData()).Assert(GetExpectStatusCodes);

            // 他社が定義したAPIは呼び出せない
            var apiOthers = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();
            client.GetWebApiResponseResult(apiOthers.OData()).Assert(ForbiddenExpectStatusCode);
        }

        [TestMethod]
        public void Use()
        {
            var client = new IntegratedTestClient("datauseonly", "SmartFoodChainDataUseOnly");

            // 他社で定義したものは呼び出せる
            var apiOthers = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();
            client.GetWebApiResponseResult(apiOthers.OData()).Assert(GetExpectStatusCodes);
        }

        [TestMethod]
        public void NotAccess()
        {
            var client = new IntegratedTestClient("datanotaccess", "SmartFoodChainDataNotAccess");

            // 自分で定義したものは呼び出せない
            var api = UnityCore.Resolve<IDataOfferApi>();
            client.GetWebApiResponseResult(api.OData()).Assert(ForbiddenExpectStatusCode);

            // 他社で定義したものも呼び出せない
            var apiOthers = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();
            client.GetWebApiResponseResult(apiOthers.OData()).Assert(ForbiddenExpectStatusCode);
        }
    }
}
