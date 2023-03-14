using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.Infrastructure.Database.Data.MongoDb;
using JP.DataHub.ApiWeb.Infrastructure.Resources;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Data.MongoDb
{
    [TestClass]
    public class UnitTest_MongoDbQuerySyntaxValidatior : UnitTestBase
    {
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void Validate_正常系_Find()
        {
            var query = new Query(@"
{
    ""Select"": {
        ""Code"": 1,
        ""id"": 1,
        ""Name"": 1
    },
    ""Where"": {
        ""Code"": {Code1}
    },
    ""OrderBy"": {
        ""Code"": {Code2}
    },
    ""Top"": 2,
    ""Skip"": 3
})");

            var result = new MongoDbQuerySyntaxValidatior().Validate(query.Value, out string message);
            result.IsTrue();
            message.Is(string.Empty);
        }

        [TestMethod]
        public void Validate_正常系_Aggregate()
        {
            var query = new Query(@"
{
    ""Aggregate"": [
        { ""$unionWith"": { ""coll"": {COLLECTION_NAME}, ""pipeline"": [ { ""$match"": { ""Key1"": {Code1}, ""Key2"": {Code2} } } ] } }
    ]
}");

            var result = new MongoDbQuerySyntaxValidatior().Validate(query.Value, out string message);
            result.IsTrue();
            message.Is(string.Empty);
        }

        [TestMethod]
        [TestCase("Select")]
        [TestCase("Where")]
        [TestCase("OrderBy")]
        [TestCase("Top")]
        [TestCase("Skip")]
        public void Validate_正常系_混在()
        {
            TestContext.Run((string property) =>
            {
                var query = new Query($@"
{{
    ""{property}"": null,
    ""Aggregate"": [
        {{ ""$match"": {{ ""key"": ""value"" }} }}
    ]
}}");

                var result = new MongoDbQuerySyntaxValidatior().Validate(query.Value, out string message);
                result.IsTrue();
                message.Is(string.Empty);
            });
        }

        [TestMethod]
        [TestCase("Select")]
        [TestCase("Where")]
        [TestCase("OrderBy")]
        [TestCase("Top")]
        [TestCase("Skip")]
        public void Validate_異常系_混在()
        {
            TestContext.Run((string property) =>
            {
                var query = new Query($@"
{{
    ""{property}"": """",
    ""Aggregate"": [
        {{ ""$match"": {{ ""key"": ""value"" }} }}
    ]
}}");

                var result = new MongoDbQuerySyntaxValidatior().Validate(query.Value, out string message);
                result.IsFalse();
                message.Contains(property).IsTrue();
            });
        }

        [TestMethod]
        [TestCase("$currentOp")]
        [TestCase("$facet")]
        [TestCase("$indexStats")]
        [TestCase("$listLocalSessions")]
        [TestCase("$listSessions")]
        [TestCase("$planCacheStats")]
        [TestCase("$redact")]
        [TestCase("$graphLookup")]
        [TestCase("$lookup")]
        [TestCase("$merge")]
        [TestCase("$out")]
        public void Validate_異常系_禁止ステージ()
        {
            TestContext.Run((string stage) =>
            {
                var query = new Query($@"
{{
    ""Aggregate"": [
        {{ ""{stage}"": {{ ""key"": ""value"" }} }}
    ]
}}");

                var result = new MongoDbQuerySyntaxValidatior().Validate(query.Value, out string message);
                result.IsFalse();
                message.Contains(stage).IsTrue();
            });
        }

        [TestMethod]
        [TestCase("$currentOp")]
        public void Validate_異常系_禁止ステージ_ネスト()
        {
            TestContext.Run((string stage) =>
            {
                var query = new Query($@"
{{
    ""Aggregate"": [
        {{ ""$unionWith"": {{ ""coll"": ""COLLECTION_NAME"", ""pipeline"": [{{ ""{stage}"": {{ ""key"": ""value"" }} }}] }} }}
    ]
}}");

                var result = new MongoDbQuerySyntaxValidatior().Validate(query.Value, out string message);
                result.IsFalse();
                message.Contains(stage).IsTrue();
            });
        }

        [TestMethod]
        public void Validate_異常系_コレクション名指定()
        {
            var query = new Query($@"
{{
""Aggregate"": [
    {{ ""$unionWith"": {{ ""coll"": ""hogehoge"", ""pipeline"": [{{ ""$match"": {{ ""key"": ""value"" }} }}] }} }}
]
}}");

            var result = new MongoDbQuerySyntaxValidatior().Validate(query.Value, out string message);
            result.IsFalse();
            string.IsNullOrWhiteSpace(message).IsFalse();
        }

        [TestMethod]
        [TestCase("$where")]
        public void Validate_異常系_禁止オペレータ_Aggregate()
        {
            TestContext.Run((string @operator) =>
            {
                var query = new Query($@"
{{
""Aggregate"": [
    {{ ""$match"": {{ ""$where"": ""hogehoge"" }} }}
]
}}");

                var result = new MongoDbQuerySyntaxValidatior().Validate(query.Value, out string message);
                result.IsFalse();
                message.Contains(@operator).IsTrue();
            });
        }

        [TestMethod]
        [TestCase("$where")]
        public void Validate_異常系_禁止オペレータ_Aggregate_ネスト()
        {
            TestContext.Run((string @operator) =>
            {
                var query = new Query($@"
{{
""Aggregate"": [
    {{ ""$unionWith"": {{ ""coll"": ""COLLECTION_NAME"", ""pipeline"": [{{ ""$match"": {{ ""{@operator}"": ""value"" }} }}] }} }}
]
}}");

                var result = new MongoDbQuerySyntaxValidatior().Validate(query.Value, out string message);
                result.IsFalse();
                message.Contains(@operator).IsTrue();
            });
        }

        [TestMethod]
        [TestCase("$where")]
        public void Validate_異常系_禁止オペレータ_Find()
        {
            TestContext.Run((string @operator) =>
            {
                var query = new Query($@"
{{
""Where"": {{ ""$match"": {{ ""{@operator}"": ""value"" }} }}
}}");

                var result = new MongoDbQuerySyntaxValidatior().Validate(query.Value, out string message);
                result.IsFalse();
                message.Contains(@operator).IsTrue();
            });
        }

        [TestMethod]
        public void ValidatePipelineSyntax_正常系_Aggregate()
        {
            var collectionName = Guid.NewGuid().ToString();
            var aggregate = BsonSerializer.Deserialize<BsonArray>($@"
[
    {{ ""$unionWith"": {{ ""coll"": ""{collectionName}"", ""pipeline"": [ {{ ""$match"": {{ ""Key1"": 1, ""Key2"": 2 }} }} ] }} }}
]");

            var result = new MongoDbQuerySyntaxValidatior().ValidatePipelineSyntax(aggregate, collectionName, out string message);
            result.IsTrue();
            message.Is(string.Empty);
        }

        [TestMethod]
        public void ValidatePipelineSyntax_異常系_Aggregate()
        {
            var collectionName = Guid.NewGuid().ToString();
            var aggregate = BsonSerializer.Deserialize<BsonArray>($@"
[
    {{ ""$unionWith"": {{ ""coll"": ""hoge"", ""pipeline"": [ {{ ""$match"": {{ ""Key1"": 1, ""Key2"": 2 }} }} ] }} }}
]");

            var result = new MongoDbQuerySyntaxValidatior().ValidatePipelineSyntax(aggregate, collectionName, out string message);
            result.IsFalse();
            message.Is(InfrastructureMessages.MongoDbCollectionNameCantSpecified);
        }
    }
}