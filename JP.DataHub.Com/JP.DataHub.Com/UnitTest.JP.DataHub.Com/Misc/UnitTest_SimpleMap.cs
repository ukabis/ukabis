using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Misc;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.Com.Misc
{
    internal class SrcChild
    {
        public string PropStrA { get; set; }
        public string PropStrB { get; set; }
    }

    internal class SrcDeep
    {
        public string PropStr1 { get; set; }
        public string PropStr2 { get; set; }
        public int PropInt1 { get; set; }
        public int PropInt2 { get; set; }
        public List<SrcChild> List { get; set; } = new List<SrcChild>();
        public SrcChild Object { get; set; }
    }

    internal class DstChild
    {
        public string PropStrA { get; set; }
        public string PropStrB { get; set; }
    }

    internal class DstDeep
    {
        public string PropStr1 { get; set; }
        public int PropStr2 { get; set; }
        public int PropInt1 { get; set; }
        public string PropInt2 { get; set; }
        public List<DstChild> List { get; set; } = new List<DstChild>();
        public DstChild Object { get; set; }
    }

    [TestClass]
    public class UnitTest_SimpleMap : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod]
        public void GenericVersion_DeepMap()
        {
            var list = new List<SrcChild>() { new SrcChild() { PropStrA = "1", PropStrB = "2" }, new SrcChild() { PropStrA = "A", PropStrB = "B" }  };
            var obj = new SrcChild { PropStrA = "AA", PropStrB = "BB" };
            var src = new SrcDeep() { PropStr1 = "hoge", PropStr2 = "123", PropInt1 = 100, PropInt2 = 200, List = list, Object = obj };
            var dst = src.Map<DstDeep>();
            dst.PropStr1.Is(src.PropStr1);
            dst.PropStr2.Is(123);
            dst.PropInt1.Is(src.PropInt1);
            dst.PropInt2.Is("200");
            dst.List.Count.Is(2);
            dst.List[0].PropStrA.Is(src.List[0].PropStrA);
            dst.List[0].PropStrB.Is(src.List[0].PropStrB);
            dst.List[1].PropStrA.Is(src.List[1].PropStrA);
            dst.List[1].PropStrB.Is(src.List[1].PropStrB);
            dst.Object.PropStrA.Is(src.Object.PropStrA);
            dst.Object.PropStrB.Is(src.Object.PropStrB);
        }

        [TestMethod]
        public void NonGenericVersion_DeepMap()
        {
            var list = new List<SrcChild>() { new SrcChild() { PropStrA = "1", PropStrB = "2" }, new SrcChild() { PropStrA = "A", PropStrB = "B" } };
            var obj = new SrcChild { PropStrA = "AA", PropStrB = "BB" };
            var src = new SrcDeep() { PropStr1 = "hoge", PropStr2 = "123", PropInt1 = 100, PropInt2 = 200, List = list, Object = obj };
            var dst = src.Map(typeof(DstDeep)) as DstDeep;
            dst.PropStr1.Is(src.PropStr1);
            dst.PropStr2.Is(123);
            dst.PropInt1.Is(src.PropInt1);
            dst.PropInt2.Is("200");
            dst.List.Count.Is(2);
            dst.List[0].PropStrA.Is(src.List[0].PropStrA);
            dst.List[0].PropStrB.Is(src.List[0].PropStrB);
            dst.List[1].PropStrA.Is(src.List[1].PropStrA);
            dst.List[1].PropStrB.Is(src.List[1].PropStrB);
            dst.Object.PropStrA.Is(src.Object.PropStrA);
            dst.Object.PropStrB.Is(src.Object.PropStrB);
        }

        public class GradeViewModel
        {
            public string ProductGradeCode { get; set; }
            public string ProductGradeName { get; set; }
            public List<GradeViewLang> GradeLang { get; set; }
            public List<string> GtinCode { get; set; }
        }

        public class GradeViewLang
        {
            public string GradeName { get; set; }
            public string LocaleCode { get; set; }
        }

        public class GradeNameModel
        {
            public string ProductGradeCode { get; set; }
            public string ProductGradeName { get; set; }
            public List<GradeNameLang> GradeLang { get; set; }
            public List<string> GtinCode { get; set; }
        }

        public class GradeNameLang
        {
            public string GradeName { get; set; }
            public string LocaleCode { get; set; }
        }

        [TestMethod]
        public void NonGenericVersion_Grade()
        {
            var lang = new List<GradeNameLang>() { new GradeNameLang() { LocaleCode = "hoge", GradeName = "hogehoge" } };
            var code = new List<string>() { "123", "456" };
            var src = new GradeNameModel() { ProductGradeCode = "CODE", ProductGradeName = "NAME", GradeLang = lang, GtinCode = code  };
            var dst = src.Map(typeof(GradeViewModel)) as GradeViewModel;
            dst.ProductGradeCode.Is(src.ProductGradeCode);
            dst.ProductGradeName.Is(src.ProductGradeName);
            dst.GradeLang.Count.Is(src.GradeLang.Count);
            dst.GradeLang[0].LocaleCode.Is(src.GradeLang[0].LocaleCode);
            dst.GradeLang[0].GradeName.Is(src.GradeLang[0].GradeName);
            dst.GtinCode.Count.Is(src.GtinCode.Count);
            dst.GtinCode[0].Is(src.GtinCode[0]);
            dst.GtinCode[1].Is(src.GtinCode[1]);
        }
    }
}