using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [Ignore("廃止：ETL")]
    [TestClass]
    public class EtlScriptTest : ApiWebItTestCase
    {
#if false
        private WebApiContext context = null;
        private EtlScriptApi api;
        private EtlScriptApiOther otherApi;

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Initialize();
            context = new WebApiContext(TestContext);
            otherApi = new EtlScriptApiOther(context);
            context.ActionAndAssert(otherApi.DeleteAll().Request, otherApi.DeleteExpectStatusCodes);
            context.ActionAndAssert(otherApi.RegistList(otherApi.Data1).Request, otherApi.RegistSuccessExpectStatusCode);
            api = new EtlScriptApi(context);
            context.ActionAndAssert(api.DeleteAll().Request, api.DeleteExpectStatusCodes);
        }

        [TestMethod]
        public void EtlScriptNormalSenario()
        {
            //データ登録　Object編集
            context.ActionAndAssert(api.RegistObjectEdit(api.data1).Request, api.RegistSuccessExpectStatusCode);
            context.ActionAndAssert(api.Get(api.data1Key).Request, api.GetSuccessExpectStatusCode, api.Data1Get.ToJson());

            //データ登録　List編集
            context.ActionAndAssert(api.RegistArrayEdit(api.data2).Request, api.RegistSuccessExpectStatusCode);
            context.ActionAndAssert(api.Get(api.data2Key).Request, api.GetSuccessExpectStatusCode, api.Data2Get.ToJson());

            //データ取得　Object編集
            context.ActionAndAssert(api.GetObjectEdit(api.data1Key).Request, api.GetSuccessExpectStatusCode, api.Data1_1Get.ToJson());

            //データ取得　List編集
            context.ActionAndAssert(api.GetArrayEdit().Request, api.GetSuccessExpectStatusCode, api.Data1_2Get.ToJson());

            //データ取得　他APIのデータを結合
            context.ActionAndAssert(api.GetOtherApi(api.data1Key).Request, api.GetSuccessExpectStatusCode, api.dataOther1Get.ToJson());

            //データ登録　他APIのデータを結合
            context.ActionAndAssert(api.RegistGetOtherApi(api.data3).Request, api.RegistSuccessExpectStatusCode);
            context.ActionAndAssert(api.Get(api.data3Key).Request, api.GetSuccessExpectStatusCode, api.Data3Get.ToJson());
        }
#endif
    }
}
