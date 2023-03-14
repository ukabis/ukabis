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
    public class UnitTest_TypeExtensions : UnitTestBase
    {
        interface xxx { }
        class yyy : xxx { public object Value { get; set; } public yyy() { } public yyy(object val) { Value = val; } }
        class zzz { public object Value { get; set; } public zzz() { } public zzz(object val) { Value = val; } }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod]
        public void HasInheritance_Success()
        {
            typeof(yyy).HasInheritance<xxx>().Is(true);
        }

        [TestMethod]
        public void HasInheritance_Fail()
        {
            typeof(zzz).HasInheritance<xxx>().Is(false);
            typeof(yyy).HasInheritance<zzz>().Is(false);
        }

        [TestMethod]
        public void Create_Success()
        {
            typeof(yyy).Create<yyy>().GetType().Is(typeof(yyy));
            typeof(zzz).Create<zzz>().GetType().Is(typeof(zzz));
        }

        [TestMethod]
        public void Create_Fail()
        {
            AssertEx.Catch<MissingMethodException>(() => typeof(xxx).Create<xxx>());
        }

        [TestMethod]
        public void CreateArg_Success()
        {
            var y = typeof(yyy).Create<yyy>("abc");
            y.GetType().Is(typeof(yyy));
            y.Value.Is("abc");
            var z = typeof(zzz).Create<zzz>("xyz");
            z.GetType().Is(typeof(zzz));
            z.Value.Is("xyz");
        }

        [TestMethod]
        public void CreateArg_Fail()
        {
            AssertEx.Catch<MissingMethodException>(() => typeof(xxx).Create<xxx>("abc","xyz"));
        }
    }
}