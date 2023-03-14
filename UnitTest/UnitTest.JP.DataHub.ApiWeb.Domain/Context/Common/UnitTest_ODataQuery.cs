using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_ODataQuery : UnitTestBase
    {
        [TestMethod]
        public void ODataQuery_none()
        {
            var testClass = new ODataQuery("");
            testClass.HasValue.Is(false);
            testClass.HasAnyQuery.Is(false);
        }

        [TestMethod]
        public void ODataQuery_normal()
        {
            var testClass = new ODataQuery("$count=true");
            testClass.HasValue.Is(true);
            testClass.HasAnyQuery.Is(false);
        }

        [TestMethod]
        public void ODataQuery_normal_Upper()
        {
            var testClass = new ODataQuery("$Count=True");
            testClass.HasValue.Is(true);
            testClass.HasAnyQuery.Is(false);
        }

        [TestMethod]
        public void ODataQuery_AnyQuery()
        {
            var testClass = new ODataQuery("$filter=(AreaUnitCode eq 'AD' ) and (hoge/any(o: o/fuga eq 'hogehoge2' or o/fuga eq 'hogehoge1'))&$count=true");
            testClass.HasValue.Is(true);
            testClass.HasAnyQuery.Is(true);
        }

        [TestMethod]
        public void ODataQuery_AnyQuery_Upper()
        {
            var testClass = new ODataQuery("$FILTER=(AreaUnitCode eq 'AD' ) and (hoge/Any(o: o/fuga eq 'hogehoge2' or o/fuga eq 'hogehoge1'))&$count=true");
            testClass.HasValue.Is(true);
            testClass.HasAnyQuery.Is(true);
        }

        [TestMethod]
        public void ODataQuery_AnyQuery2()
        {
            var testClass = new ODataQuery("$filter=hoge/any(o: o/fuga eq 'hogehoge2' or o/fuga eq 'hogehoge1')");
            testClass.HasValue.Is(true);
            testClass.HasAnyQuery.Is(true);
        }
    }
}
