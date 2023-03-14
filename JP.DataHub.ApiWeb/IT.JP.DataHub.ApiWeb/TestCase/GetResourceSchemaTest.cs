using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class GetResourceSchemaTest : ApiWebItTestCase
    {
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
            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();

            api.AddHeaders.Add(HeaderConst.X_Cache, "on");

            var body = client.GetWebApiResponseResult(api.GetResourceSchema()).Assert(GetSuccessExpectStatusCode).ContentString;
            body.Is(@"{
  'description':'面積単位マスター',
  'properties': {
    'AreaUnitCode': {
      'title': '面積単位コード',
      'type': 'string',
      'required':true,
    },
    'AreaUnitName': {
      'maxLength': 20,
      'title': '面積単位名',
      'type': 'string',
      'required':true,
    },
    'ConversionSquareMeters': {
      'title': '平方メートル換算',
      'type': 'number'
    },
    'additionalProperties' : false
  },
  'type': 'object'
}");
        }
    }
}