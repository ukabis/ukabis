using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Extensions;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.Com.Extensions
{
    [TestClass]
    public class UnitTest_EnumExtensions : UnitTestBase
    {
        private class TestAttribute : Attribute { public string Str { get; set; } public TestAttribute(string str) { Str = str; } };

        private enum hoge
        {
            [TestAttribute("ABC")]
            ABC,
            DEF,
            XYZ,
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod]
        public void Enum_GetAttribute_Success()
        {
            var attr = hoge.ABC.GetAttribute<TestAttribute>();
            attr.IsNotNull();
            attr.GetType().Is(typeof(TestAttribute));
            attr.Str.Is("ABC");
        }

        [TestMethod]
        public void Enum_GetAttribute_Fail()
        {
            hoge.DEF.GetAttribute<TestAttribute>().IsNull();
        }
    }
}