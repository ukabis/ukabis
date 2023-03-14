using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [TestClass]
    public class UnitTest_ODataFilterHelper : UnitTestBase
    {
        [TestMethod]
        public void ToFilterGeneric()
        {
            "123".ToFilter<string>().Is("'123'");
            "".ToFilter<string>().Is("''");
            string str = null;
            str.ToFilter<string>().Is("null");
            "123".ToFilter<int>().Is("123");
            "a".ToFilter<int>().Is("''");
            "abc'def".ToFilter<string>().Is("'abc''def'");
            "2B02378F-1044-4F25-BC4A-2BC6CB712ABC".ToFilter<Guid>().Is("'2b02378f-1044-4f25-bc4a-2bc6cb712abc'");
            "{2B02378F-1044-4F25-BC4A-2BC6CB712ABC}".ToFilter<Guid>().Is("'2b02378f-1044-4f25-bc4a-2bc6cb712abc'");
            "abcdef".ToFilter<Guid>().Is("''");
        }

        [TestMethod]
        public void ToFilter()
        {
            "123".ToFilter().Is("'123'");
            "".ToFilter().Is("''");
            string str = null;
            str.ToFilter().Is("null");
            "abc'def".ToFilter().Is("'abc''def'");
        }

        [TestMethod]
        public void ToFilterKeyValueResult()
        {
            var list = new Dictionary<string, string>();
            list.Add("123", "456");
            list.Add("456", "abc");
            list.Add("abc", "def");
            list.Add("null", null);
            list.Find<int>("123").ToFilter().Is("456");
            list.Find<string>("null").ToFilter().Is("''");
            list.Find<string>("notfound").ToFilter().Is("null");
            list.Find<int>("456").ToFilter().Is("''");
        }
    }
}
