using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [TestClass]
    class UnitTest_QueryStringHelper : UnitTestBase
    {
        [TestMethod]
        public void Find()
        {
            var list = new Dictionary<string, string>();
            list.Add("123", "456");
            list.Add("456", "abc");
            list.Add("abc", "def");
            list.Add("null", null);

            var hit1 = list.Find<int>("123");
            hit1.HasKey.IsTrue();
            hit1.IsValid.IsTrue();
            hit1.Value.Is(456);
            hit1.Type.Is(typeof(int));
            hit1.Source.Is("456");

            var hit2 = list.Find<string>("456");
            hit2.HasKey.IsTrue();
            hit2.IsValid.IsTrue();
            hit2.Value.Is("abc");
            hit2.Type.Is(typeof(string));
            hit2.Source.Is("abc");

            var hit3 = list.Find<int>("456");
            hit3.HasKey.IsTrue();
            hit3.IsValid.IsFalse();
            hit3.Object.IsNull();
            hit3.Type.Is(typeof(int));
            hit3.Source.Is("abc");

            var hit4 = list.Find<string>("xyz");
            hit4.HasKey.IsFalse();
            hit4.IsValid.IsFalse();
            hit4.Object.IsNull();
            hit4.Value.IsNull();
            hit4.Type.Is(typeof(string));
            hit4.Source.IsNull();
        }
    }
}
