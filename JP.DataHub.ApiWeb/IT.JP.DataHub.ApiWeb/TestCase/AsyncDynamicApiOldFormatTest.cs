using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [Ignore("廃止：旧型式の非同期ファイルの互換性を切ったため")]
    [TestClass]
    [TestCategory("Async")]
    public class AsyncDynamicApiOldFormatTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

#if false
        [TestMethod]//Stagingでしか動かない想定
        public void OldDataScenario()
        {
            var resource = new AsyncDynamicApi(context);
            //stgならこれ "07c17168-5ba8-4764-bd1c-25b2154eb926"
            //localならこれ 
            var api = resource.GetResult("07c17168-5ba8-4764-bd1c-25b2154eb926");
            context.ActionAndAssert(api.Request, HttpStatusCode.OK, JToken.Parse(resource.DataOldFormat));
        }
#endif
    }
}

