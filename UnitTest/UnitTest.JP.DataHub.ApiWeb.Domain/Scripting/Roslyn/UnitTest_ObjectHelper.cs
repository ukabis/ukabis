using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [TestClass]
    public class UnitTest_ObjectHelper : UnitTestBase
    {
        [TestMethod]
        public void To()
        {
            "123".To<int>().Is(123);
            "123".To<string>().Is("123");
            "121a3".To<int>().Is(0);
            var now = DateTime.Now;
            now.To<string>().Is(now.ToString());
            "2001/01/01".To<DateTime>().Is(DateTime.Parse("2001/01/01"));
            "abc".To<DateTime>().Is(DateTime.Parse("0001/01/01"));
            var guid = Guid.NewGuid();
            guid.To<string>().Is(guid.ToString());
            "a".To<Guid>().Is(Guid.Parse("00000000-0000-0000-0000-000000000000"));

            "123".To<int?>().Is(123);
            "a".To<Guid?>().IsNull();
        }

        [TestMethod]
        public void IsValid()
        {
            "123".IsValid<int>().IsTrue();
            "123".IsValid<string>().IsTrue();
            "12a3".IsValid<int>().IsFalse();
            var now = DateTime.Now;
            now.IsValid<string>().IsTrue();
            "2001/01/01".IsValid<DateTime>().IsTrue();
            "abc".IsValid<DateTime>().IsFalse();
            "00000000-0000-0000-0000-000000000000".ToString().IsValid<Guid>().IsTrue();
            "a".IsValid<Guid>().IsFalse();
            "123".IsValid<int?>().IsTrue();
            "a".IsValid<Guid?>().IsTrue();
        }
    }
}
