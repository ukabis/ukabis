using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_ScriptType : UnitTestBase
    {
        [TestMethod]
        public void ScriptType_Parse()
        {
            var ret = "rss".ToScriptType();
            ret.Is(ScriptType.RoslynScript);
        }
    }
}
