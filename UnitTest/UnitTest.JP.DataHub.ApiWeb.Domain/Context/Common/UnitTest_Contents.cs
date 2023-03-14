using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;
using Microsoft.Extensions.Configuration;
using Unity;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_Contents : UnitTestBase
    {
        internal const string MEDIATYPE_JSON = "application/json";
        internal const string MEDIATYPE_XML = "application/xml";
        internal const string MEDIATYPE_TEXTXML = "text/xml";
        internal const string MEDIATYPE_CSV = "text/csv";

        public class TestClass
        {
            public string Hoge { get; set; }

            public int Foo { get; set; }
        }

        public class TestClassCsv
        {
            public string stringItem { get; set; }

            public decimal number { get; set; }

            public int integer { get; set; }

            public bool boolean { get; set; }

            public string objectItem { get; set; }

            public string array { get; set; }

            public string nullItem { get; set; }
        }

        public class TestClassString
        {
            public string stringItem { get; set; }

            public string number { get; set; }

            public string integer { get; set; }

            public string boolean { get; set; }

            public string objectItem { get; set; }

            public string array { get; set; }

            public string nullItem { get; set; }
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
            UnityContainer.RegisterInstance<IConfiguration>(Configuration);
        }
        [TestMethod]
        public void Contents_ConvertJson()
        {
            var target = new TestClass { Hoge = "aaaa", Foo = 1 };
            var test = new Contents(JsonConvert.SerializeObject(target));
            var ret = test.ConvertContents(new MediaType(MEDIATYPE_JSON), new DataSchema(""), new IsArray(false));
            ret.Item1.IsFalse();
            ret.Item2.IsStructuralEqual(test);
        }

        [TestMethod]
        public void Contents_ConvertXml()
        {
            var target = new TestClass { Hoge = "aaaa", Foo = 1 };
            var xmlSeri = new XmlSerializer(typeof(TestClass));
            string xmlData = "";

            using (var writer = new StringWriter())
            {
                var nameSpace = new XmlSerializerNamespaces();
                nameSpace.Add(string.Empty, string.Empty);
                xmlSeri.Serialize(writer, target, nameSpace);
                writer.Flush();
                xmlData = writer.GetStringBuilder().ToString();
            }
            var test = new Contents(xmlData);
            var ret = test.ConvertContents(new MediaType(MEDIATYPE_XML), new DataSchema(""), new IsArray(false));
            ret.Item1.IsTrue();
            target.IsStructuralEqual(JsonConvert.DeserializeObject<TestClass>(ret.Item2.ReadToString()));

        }

        [TestMethod]
        public void Contents_ConvertXmlTextXml()
        {
            var target = new TestClass { Hoge = "aaaa", Foo = 1 };
            var xmlSeri = new XmlSerializer(typeof(TestClass));
            string xmlData = "";

            using (var writer = new StringWriter())
            {
                var nameSpace = new XmlSerializerNamespaces();
                nameSpace.Add(string.Empty, string.Empty);
                xmlSeri.Serialize(writer, target, nameSpace);
                writer.Flush();
                xmlData = writer.GetStringBuilder().ToString();
            }
            var test = new Contents(xmlData);
            var ret = test.ConvertContents(new MediaType(MEDIATYPE_TEXTXML), new DataSchema(""), new IsArray(false));
            ret.Item1.IsTrue();
            target.IsStructuralEqual(JsonConvert.DeserializeObject<TestClass>(ret.Item2.ReadToString()));

        }

        [TestMethod]
        public void Contents_ConvertXmlNonVersion()
        {
            var target = "<root><Hoge>aaaa</Hoge><Foo>10</Foo></root>";
            var compClass = new TestClass { Hoge = "aaaa", Foo = 10 };
            var test = new Contents(target);
            var ret = test.ConvertContents(new MediaType(MEDIATYPE_TEXTXML), new DataSchema(""), new IsArray(false));
            ret.Item1.IsTrue();
            compClass.IsStructuralEqual(JsonConvert.DeserializeObject<TestClass>(ret.Item2.ReadToString()));

        }

        [TestMethod]
        public void Contents_ConvertCsv()
        {
            var test = new Contents(csvData);

            var expectedString = new List<TestClassString>
            {
                new TestClassString() {stringItem = "STRING_ITEM", number = "1.1", integer = "1", boolean = "true", objectItem = "OBJECT_ITEM", array = "[1,2,3]", nullItem = "null"},
                new TestClassString() {stringItem = "string_item", number = "2.2", integer = "2", boolean = "false", objectItem = "object_item", array = @"[a,b]", nullItem = "null"}
            };

            var ret = test.ConvertContents(new MediaType(MEDIATYPE_CSV), new DataSchema(null), new IsArray(true));
            ret.Item1.IsFalse();
            expectedString.IsStructuralEqual(JsonConvert.DeserializeObject<List<TestClassString>>(ret.Item2.ReadToString()));
        }

        [TestMethod]
        public void Contents_ConvertCsvSchema()
        {
            var test = new Contents(csvData);

            var expectedSchema = new List<TestClassCsv>
            {
                new TestClassCsv() {stringItem = "STRING_ITEM", number = 1.1M, integer = 1, boolean = true, objectItem = "OBJECT_ITEM", array = "[1,2,3]", nullItem = "null"},
                new TestClassCsv() {stringItem = "string_item", number = 2.2M, integer = 2, boolean = false, objectItem = "object_item", array = @"[a,b]", nullItem = "null"}
            };

            var ret = test.ConvertContents(new MediaType(MEDIATYPE_CSV), new DataSchema(dataSchema), new IsArray(true));
            ret.Item1.IsFalse();
            expectedSchema.IsStructuralEqual(JsonConvert.DeserializeObject<List<TestClassCsv>>(ret.Item2.ReadToString()));
        }

        [TestMethod]
        public void Contents_ReadToString()
        {
            var test = new Contents(csvData);

            var expectedString = new List<TestClassString>
            {
                new TestClassString() {stringItem = "STRING_ITEM", number = "1.1", integer = "1", boolean = "true", objectItem = "OBJECT_ITEM", array = "[1,2,3]", nullItem = "null"},
                new TestClassString() {stringItem = "string_item", number = "2.2", integer = "2", boolean = "false", objectItem = "object_item", array = @"[a,b]", nullItem = "null"}
            };

            var result = test.ReadToString();
            result.Is(csvData);
        }



        private const string csvData = @"stringItem,number,integer,boolean,objectItem,array,nullItem
""STRING_ITEM"",""1.1"",""1"",""true"",""OBJECT_ITEM"",""[1,2,3]"",""null""
""string_item"",2.2,2,false,object_item,""[a,b]"",null
";

        private const string dataSchema = @"{
  'description':'スキーマ',
  'type': 'object',
  'properties': {
    'stringItem': {
      'title': 'stringItem',
      'type': 'string'
    },
    'number': {
      'title': 'number',
      'type': 'number'
    },
    'integer': {
      'title': 'integer',
      'type': 'integer'
    },
    'boolean': {
      'title': 'boolean',
      'type': 'boolean'
    }
  }
}
";
    }
}
