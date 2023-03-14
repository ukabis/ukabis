using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [TestClass()]
    public class UnitTest_JsonHelper : UnitTestBase
    {
        #region setup

        public JToken stdToken;
        public JToken stdTokens;
        public const string stdJsonStr = @"
    {
        ""Id"" : 1,
        ""Name"" : ""Test1"",
        ""isDeleted"" : true
    }
";
        public const string stdJsonStrArray = @"
[
    {
        ""Id"" : 1,
        ""Name"" : ""Test1"",
        ""isDeleted"" : true
    },
    {
        ""Id"" : 2,
        ""Name"" : ""Test2"",
        ""isDeleted"" : true
    },
    {
        ""Id"" : 3,
        ""Name"" : ""Test3"",
        ""isDeleted"" : false
    }
]";

        class TestClass_JToken
        {
            public int Id;
            public string Name;
            public bool IsDelete;
        }

        #endregion

        [TestInitialize]
        public void Init()
        {
            stdToken = JToken.FromObject(new TestClass_JToken() { Id = 1, Name = "Test1", IsDelete = true });
            stdTokens = JToken.Parse(stdJsonStrArray);
        }

        [TestMethod()]
        public void ToJsonTest()
        {
            var actual = JsonHelper.ToJson(stdJsonStrArray);
            actual.Is(stdTokens);
        }

        [TestMethod()]
        public void ToJsonTest_Generics()
        {
            var actual = JsonHelper.ToJson<TestClass_JToken>(stdJsonStr);
            Assert.IsInstanceOfType(actual, typeof(TestClass_JToken));

        }

        [TestMethod()]
        public void ToArrayTest()
        {
            var actual = JsonHelper.ToArray(stdToken);
            Assert.IsTrue(actual.Type.ToString() == "Array");
        }

        [TestMethod()]
        public void ToArrayTest_withTokens()
        {
            var actual = JsonHelper.ToArray(stdToken, stdTokens);
            Assert.IsTrue(actual.Type.ToString() == "Array");
        }

        [TestMethod()]
        public void AddFieldTest()
        {
            var actual = JsonHelper.AddField(stdToken, "Height", 175.5);
            Assert.AreEqual(stdToken, actual);
            Assert.IsTrue(Convert.ToDouble(actual["Height"]) == 175.5);
        }

        [TestMethod()]
        public void RemoveFieldTest()
        {
            var actual = JsonHelper.RemoveField(stdToken, "Name");
            Assert.AreEqual(stdToken, actual);
            Assert.IsNull(actual["Name"]);
        }

        [TestMethod()]
        public void RemoveFieldsTest()
        {
            string[] fields = { "Name", "isDeleted" };
            var actual = JsonHelper.RemoveFields(stdToken, fields);
            Assert.AreEqual(stdToken, actual);
            Assert.IsTrue(Convert.ToInt32(actual["Id"]) == 1);
            Assert.IsNull(actual["Name"]);
            Assert.IsNull(actual["isDeleted"]);
        }

        [TestMethod()]
        public void ToJsonStringTest()
        {
            var list = new List<TestClass_JToken>();
            list.Add(new TestClass_JToken() { Id = 1, Name = "test1", IsDelete = false });
            list.Add(new TestClass_JToken() { Id = 2, Name = "test2", IsDelete = true });
            list.Add(new TestClass_JToken() { Id = 3, Name = "test3", IsDelete = false });

            var actual = JsonHelper.ToJsonString(list);
            var expect = @"[{""Id"":1,""Name"":""test1"",""IsDelete"":false},{""Id"":2,""Name"":""test2"",""IsDelete"":true},{""Id"":3,""Name"":""test3"",""IsDelete"":false}]";
            Assert.AreEqual(actual, expect);

        }

        static string src = @"
{
  'age' : 26,
  'string' : 'abcdefg',
  'null' : null,
  'flag1' : true,
  'flag2' : false,
  'obj' : {
    'user_id' : 'ABCDEFG',
    'user_name' : 'test taro',
  },
}
";
        static string expect = @"
{
  'age' : 26,
  'null' : null,
  'obj' : {
    'user_id' : 'ABCDEFG',
    'user_name' : 'test taro',
  },
}
";

        [TestMethod]
        public void SelectFields()
        {
            var json = JsonHelper.ToJson(src);
            var result = JsonHelper.SelectFields(json, new string[] { "age", "null", "obj", "tmp" });
            result.Is(JsonHelper.ToJson(expect));
        }

        [TestMethod]
        public void SelectFields2()
        {
            var json = JsonHelper.ToJson(src);
            var result = JsonHelper.SelectFields(json, "age", "null", "obj", "tmp");
            result.Is(JsonHelper.ToJson(expect));
        }

        [TestMethod]
        public void Selector()
        {
            var json = JsonHelper.ToJson(src);
            var fields = new string[] { "age", "null", "obj", "tmp" };
            var result = JsonHelper.Select(json, x => fields.Contains(x.Name));
            result.Is(JsonHelper.ToJson(expect));
        }

        static string array = @"
[
{ 'age' : 26, 'name' : 'test', },
{ 'age' : 30, 'name' : 'test2-4', },
{ 'age' : 30, 'name' : 'test2-1', },
{ 'age' : 1, 'name' : 'test3', },
{ 'age' : 99, 'name' : 'test4', },
{ 'age' : 10, 'name' : 'test5', },
{ 'age' : 5, 'name' : 'test6', },
]
";

        [TestMethod]
        public void OrderBy()
        {
            var aray = JsonHelper.ToJson(array) as JArray;
            var result = JsonHelper.OrderBy(aray, "age", "name");
        }
    }
}