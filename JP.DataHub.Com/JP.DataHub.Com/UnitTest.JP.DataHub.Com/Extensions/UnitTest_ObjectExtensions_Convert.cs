using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.Com.Extensions
{
    [TestClass]
    public class UnitTest_ObjectExtensions_Convert : UnitTestBase
    {
        string string_number = "123";
        string string_float = "123.4";
        string string_string = "xyz";
        string string_guid1 = "{550B5978-6DA4-4A2F-ABFA-F29CA929DB63}";
        string string_guid2 = "550B5978-6DA4-4A2F-ABFA-F29CA929DB63";
        string string_bool_true1 = "true";
        string string_bool_true2 = "TRUE";
        string string_bool_false1 = "false";
        string string_bool_false2 = "FALSE";
        Guid guid = new Guid("550B5978-6DA4-4A2F-ABFA-F29CA929DB63");

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        [TestMethod]
        public void Convert_StringTo()
        {
            string_number.Convert<decimal>().Is((decimal)123);
            string_number.Convert<double>().Is((double)123);
            string_number.Convert<float>().Is((float)123);
            string_number.Convert<int>().Is(123);
            string_number.Convert<uint>().Is((uint)123);
            string_number.Convert<long>().Is((long)123);
            string_number.Convert<ulong>().Is((ulong)123);
            string_number.Convert<short>().Is((short)123);
            string_number.Convert<ushort>().Is((ushort)123);

            string_float.Convert<decimal>().Is((decimal)123.4);
            string_float.Convert<double>().Is((double)123.4);
            string_float.Convert<float>().Is((float)123.4);
            string_float.Convert<int>().Is(0);
            string_float.Convert<uint>().Is((uint)0);
            string_float.Convert<long>().Is((long)0);
            string_float.Convert<ulong>().Is((ulong)0);
            string_float.Convert<short>().Is((short)0);
            string_float.Convert<ushort>().Is((ushort)0);

            string_string.Convert<int>().Is(0);

            string_guid1.Convert<Guid>().Is(guid);
            string_guid2.Convert<Guid>().Is(guid);

            string_bool_true1.Convert<bool>().Is(true);
            string_bool_true2.Convert<bool>().Is(true);
            string_bool_false1.Convert<bool>().Is(false);
            string_bool_false2.Convert<bool>().Is(false);
        }

        [TestMethod]
        public void IsConvert_StringTo()
        {
            string_number.IsConvert<decimal>().Is(true);
            string_number.IsConvert<double>().Is(true);
            string_number.IsConvert<float>().Is(true);
            string_number.IsConvert<int>().Is(true);
            string_number.IsConvert<uint>().Is(true);
            string_number.IsConvert<long>().Is(true);
            string_number.IsConvert<ulong>().Is(true);
            string_number.IsConvert<short>().Is(true);
            string_number.IsConvert<ushort>().Is(true);

            string_float.IsConvert<decimal>().Is(true);
            string_float.IsConvert<double>().Is(true);
            string_float.IsConvert<float>().Is(true);
            string_float.IsConvert<int>().Is(false);
            string_float.IsConvert<uint>().Is(false);
            string_float.IsConvert<long>().Is(false);
            string_float.IsConvert<ulong>().Is(false);
            string_float.IsConvert<short>().Is(false);
            string_float.IsConvert<ushort>().Is(false);

            string_string.IsConvert<int>().Is(false);

            string_guid1.IsConvert<Guid>().Is(true);
            string_guid2.IsConvert<Guid>().Is(true);
            string_string.IsConvert<Guid>().Is(false);

            string_bool_true1.Convert<bool>().Is(true);
            string_bool_true2.Convert<bool>().Is(true);
            string_bool_false1.IsConvert<bool>().Is(true);
            string_bool_false2.IsConvert<bool>().Is(true);
            string_string.IsConvert<bool>().Is(false);
        }

        public class ClassA
        {
            public string A { get; set; } = "ABC";
            public int B { get; set; } = 123;
            public float C = 456.7f;
        }

        [TestMethod]
        public void ShallowJoinProperties()
        {
            object x = null;
            x.JoinedValueShallowJProperties().IsNull();

            new ClassA().JoinedValueShallowJProperties().Is("ABC,123");
        }

        [TestMethod]
        public void PropertiesGetHashCode()
        {
            new ClassA().PropertiesGetHashCode().GetType().Is(typeof(int));
        }

        [TestMethod]
        public void IsSameValue_primitive()
        {
            int a = 100;
            a.IsSameValue(100).Is(true);
            a.IsSameValue(200).Is(false);

            float b = 123.4f;
            b.IsSameValue(123.4f).Is(true);
            b.IsSameValue(123.4).Is(false);

            string c = "123";
            c.IsSameValue("123").Is(true);
            c.IsSameValue("456").Is(false);

            guid.IsSameValue(new Guid("550B5978-6DA4-4A2F-ABFA-F29CA929DB63")).IsTrue();
            guid.IsSameValue(new Guid("{550B5978-6DA4-4A2F-ABFA-F29CA929DB63}")).IsTrue();
            guid.IsSameValue(new Guid("550B5978-6DA4-4A2F-ABFA-F29CA929DB60")).IsFalse();
        }

        [TestMethod]
        public void IsSameValue_class()
        {
            var a = new ClassA();
            var b = new ClassA() { B = 456 };
            a.IsSameValue(new ClassA()).IsTrue();
            a.IsSameValue(b).IsFalse();
        }
    }
}