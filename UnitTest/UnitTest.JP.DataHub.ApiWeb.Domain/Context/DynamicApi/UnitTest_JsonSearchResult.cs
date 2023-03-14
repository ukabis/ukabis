using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessagePack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    [TestClass]
    public class UnitTest_JsonSearchResult : UnitTestBase
    {
        [TestMethod]
        public void JsonSearchResult_AddSingle()
        {
            var test = JToken.FromObject(new { col1 = "aaaa", col2 = "bbbb" });
            var target = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            target.AddString(test.ToString(Formatting.None));
            target.EndData();
            target.Value.Is(test.ToString(Formatting.None));
            target.JToken.IsStructuralEqual(test);
            using (var reader = new StreamReader(target.Stream))
            {
                reader.ReadToEnd().Is(test.ToString(Formatting.None));
            }
        }

        [TestMethod]
        public void JsonSearchResult_Serialize()
        {
            var test = JToken.FromObject(new { col1 = "aaaa", col2 = "bbbb" });
            var target = new JsonSearchResult(new ApiQuery(""), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            target.AddString(test.ToString(Formatting.None));
            target.EndData();
            var serialize = MessagePackSerializer.Serialize(target);
            var deserialize = MessagePackSerializer.Deserialize<JsonSearchResult>(serialize);
            deserialize.Count.Is(target.Count);
            deserialize.Value.Is(target.Value);
            deserialize.JToken.IsStructuralEqual(target.JToken);
        }

        [TestMethod]
        public void JsonSearchResult_SerializeNull()
        {
            JsonSearchResult target = null;
            var serialize = MessagePackSerializer.Serialize(target);
            var deserialize = MessagePackSerializer.Deserialize<JsonSearchResult>(serialize);
            deserialize.IsNull();
        }

        [TestMethod]
        public void JsonSearchResult_AddSingleQuery()
        {
            var test = JToken.FromObject(new { col1 = "aaaa", col2 = "bbbb" });
            var target = new JsonSearchResult(new ApiQuery("aaaa"), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            target.AddString(test.ToString(Formatting.None));
            target.EndData();
            target.Value.Is(test.ToString(Formatting.None));
            target.JToken.IsStructuralEqual(test);
            using (var reader = new StreamReader(target.Stream))
            {
                reader.ReadToEnd().Is(test.ToString(Formatting.None));
            }
        }

        [TestMethod]
        public void JsonSearchResult_AddDoubleQuery()
        {
            var test = JToken.FromObject(new[] { new { col1 = "aaaa", col2 = "bbbb" }, new { col1 = "ccccc", col2 = "ddddd" } });
            var target = new JsonSearchResult(new ApiQuery("select *"), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            foreach (var singleData in test as JArray)
            {
                target.AddString(singleData.ToString(Formatting.None));

            }
            target.EndData();
            target.Value.Is(test.ToString(Formatting.None));
            target.JToken.IsStructuralEqual(test);
            using (var reader = new StreamReader(target.Stream))
            {
                reader.ReadToEnd().Is(test.ToString(Formatting.None));
            }
        }

        [TestMethod]
        public void JsonSearchResult_AddThreeQuery()
        {
            var test = JToken.FromObject(new[] { new { col1 = "aaaa", col2 = "bbbb" }, new { col1 = "ccccc", col2 = "ddddd" }, new { col1 = "scccc", col2 = "ddddd" } });
            var target = new JsonSearchResult(new ApiQuery("select *"), new PostDataType(""), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            foreach (var singleData in test as JArray)
            {
                target.AddString(singleData.ToString(Formatting.None));

            }
            target.EndData();
            target.Value.Is(test.ToString(Formatting.None));
            target.JToken.IsStructuralEqual(test);
            using (var reader = new StreamReader(target.Stream))
            {
                reader.ReadToEnd().Is(test.ToString(Formatting.None));
            }
        }
        [TestMethod]
        public void JsonSearchResult_AddSinglePostDataType()
        {
            var test = JToken.FromObject(new[] { new { col1 = "aaaa", col2 = "bbbb" } });
            var target = new JsonSearchResult(new ApiQuery(""), new PostDataType("array"), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            foreach (var singleData in test as JArray)
            {
                target.AddString(singleData.ToString(Formatting.None));

            }
            target.EndData();
            target.Value.Is(test.ToString(Formatting.None));
            target.JToken.IsStructuralEqual(test);
            using (var reader = new StreamReader(target.Stream))
            {
                reader.ReadToEnd().Is(test.ToString(Formatting.None));
            }
        }

        [TestMethod]
        public void JsonSearchResult_AddDoublePostDataType()
        {
            var test = JToken.FromObject(new[] { new { col1 = "aaaa", col2 = "bbbb" }, new { col1 = "ccccc", col2 = "ddddd" } });
            var target = new JsonSearchResult(new ApiQuery(""), new PostDataType("array"), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            foreach (var singleData in test as JArray)
            {
                target.AddString(singleData.ToString(Formatting.None));
            }
            target.EndData();
            target.Value.Is(test.ToString(Formatting.None));
            target.JToken.IsStructuralEqual(test);
            using (var reader = new StreamReader(target.Stream))
            {
                reader.ReadToEnd().Is(test.ToString(Formatting.None));
            }
        }

        [TestMethod]
        public void JsonSearchResult_AddThreePostDataType()
        {
            var test = JToken.FromObject(new[] { new { col1 = "aaaa", col2 = "bbbb" }, new { col1 = "ccccc", col2 = "ddddd" }, new { col1 = "scccc", col2 = "ddddd" } });
            var target = new JsonSearchResult(new ApiQuery(""), new PostDataType("array"), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            foreach (var singleData in test as JArray)
            {
                target.AddString(singleData.ToString(Formatting.None));
            }
            target.EndData();
            target.Value.Is(test.ToString(Formatting.None));
            target.JToken.IsStructuralEqual(test);
            using (var reader = new StreamReader(target.Stream))
            {
                reader.ReadToEnd().Is(test.ToString(Formatting.None));
            }
        }

        [TestMethod]
        public void JsonSearchResult_AddSinglePostDataTypeAndQuery()
        {
            var test = JToken.FromObject(new[] { new { col1 = "aaaa", col2 = "bbbb" } });
            var target = new JsonSearchResult(new ApiQuery("aaaa"), new PostDataType("array"), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            foreach (var singleData in test as JArray)
            {
                target.AddString(singleData.ToString(Formatting.None));
            }
            target.EndData();
            target.Value.Is(test.ToString(Formatting.None));
            target.JToken.IsStructuralEqual(test);
            using (var reader = new StreamReader(target.Stream))
            {
                reader.ReadToEnd().Is(test.ToString(Formatting.None));
            }
        }

        [TestMethod]
        public void JsonSearchResult_AddDoublePostDataTypeAndQuery()
        {
            var test = JToken.FromObject(new[] { new { col1 = "aaaa", col2 = "bbbb" }, new { col1 = "ccccc", col2 = "ddddd" } });
            var target = new JsonSearchResult(new ApiQuery("aaaa"), new PostDataType("array"), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            foreach (var singleData in test as JArray)
            {
                target.AddString(singleData.ToString(Formatting.None));
            }
            target.EndData();
            target.Value.Is(test.ToString(Formatting.None));
            target.JToken.IsStructuralEqual(test);
            using (var reader = new StreamReader(target.Stream))
            {
                reader.ReadToEnd().Is(test.ToString(Formatting.None));
            }
        }

        [TestMethod]
        public void JsonSearchResult_AddThreePostDataTypeAndQuery()
        {
            var test = JToken.FromObject(new[] { new { col1 = "aaaa", col2 = "bbbb" }, new { col1 = "ccccc", col2 = "ddddd" }, new { col1 = "scccc", col2 = "ddddd" } });
            var target = new JsonSearchResult(new ApiQuery("aaaa"), new PostDataType("array"), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            foreach (var singleData in test as JArray)
            {
                target.AddString(singleData.ToString(Formatting.None));
            }
            target.EndData();
            target.Value.Is(test.ToString(Formatting.None));
            target.JToken.IsStructuralEqual(test);
            using (var reader = new StreamReader(target.Stream))
            {
                reader.ReadToEnd().Is(test.ToString(Formatting.None));
            }
        }

        [TestMethod]
        public async Task JsonSearchResult_UsingCounter()
        {
            var target = new JsonSearchResult(new ApiQuery("aaaa"), new PostDataType("array"), new ActionTypeVO(ActionType.Query));
            target.BeginData();
            var test = JToken.FromObject(new[]
            {
                new {col1 = "aaaa", col2 = "bbbb"},
                new {col1 = "ccccc", col2 = "ddddd"},
                new {col1 = "scccc", col2 = "ddddd"}
            });
            foreach (var singleData in test as JArray)
            {
                target.AddString(singleData.ToString(Formatting.None));
            }

            target.EndData();

            target.InUse(true);
            var task1 = Task.Run(() => usingCounterTask(target, test));
            target.InUse(true);
            var task2 = Task.Run(() => usingCounterTask(target, test));
            target.InUse(true);
            var task3 = Task.Run(() => usingCounterTask(target, test));
            target.Dispose();

            await Task.WhenAll(task1, task2, task3);

            //UsingCounterが0になるとDisposeされる
            //Disposeされても取得可能
            target.Value.Is(test.ToString(Formatting.None));
        }

        private void usingCounterTask(JsonSearchResult target, JToken test)
        {
            target.Value.Is(test.ToString(Formatting.None));
            target.InUse(false);
        }
    }
}
