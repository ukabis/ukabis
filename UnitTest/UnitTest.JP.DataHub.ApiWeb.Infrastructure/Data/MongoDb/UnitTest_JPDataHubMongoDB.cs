using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.Infrastructure.Database.Data;
using JP.DataHub.Infrastructure.Database.Data.MongoDb;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Data.MongoDb
{
    [TestClass]
    public class UnitTest_JPDataHubMongoDB : UnitTestBase
    {
        public TestContext TestContext { get; set; }


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.CultureInfo = new CultureInfo("ja");
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            UnityContainer.RegisterType<IJPDataHubMongoDb, JPDataHubMongoDb>();
        }


        [TestMethod]
        public void QueryDocument_OK()
        {
            var dbResult = new List<BsonDocument>()
            {
                BsonDocument.Parse("{ '_id': 'hoge1', 'id': 'fuga1', 'key': 'foo1', 'value': NumberDecimal('1') }"),
                BsonDocument.Parse("{ '_id': 'hoge2', 'id': 'fuga2', 'key': 'foo2', 'value': NumberDecimal('2') }")
            };
            var expectedResult = new List<JToken>()
            {
                JToken.Parse("{ 'id': 'fuga1', 'key': 'foo1', 'value': 1 }"),
                JToken.Parse("{ 'id': 'fuga2', 'key': 'foo2', 'value': 2 }")
            };

            var mockCountResult = new Mock<IAsyncCursor<BsonDocument>>();
            mockCountResult.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(true).Returns(false);
            mockCountResult.SetupSequence(x => x.Current).Returns(dbResult.Take(1)).Returns(dbResult.Skip(1));

            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()))
                .Returns(mockCountResult.Object);

            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            // 対象メソッド実行
            var result = target.QueryDocument(new List<BsonDocument>()).ToList();
            result.IsStructuralEqual(expectedResult);

            // 呼ばれたか確認
            mockClient.Verify(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void QueryDocument_OK_0件()
        {
            var mockCountResult = new Mock<IAsyncCursor<BsonDocument>>();
            mockCountResult.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(false);

            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()))
                .Returns(mockCountResult.Object);

            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            // 対象メソッド実行
            var result = target.QueryDocument(new List<BsonDocument>()).ToList();
            result.Count.Is(0);

            // 呼ばれたか確認
            mockClient.Verify(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void QueryDocumentContinuation_OK_0件()
        {
            var mockCountResult = new Mock<IAsyncCursor<BsonDocument>>();
            mockCountResult.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(false);

            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()))
                .Callback<PipelineDefinition<BsonDocument, BsonDocument>, AggregateOptions>((x, y) =>
                {
                    x.ToBsonDocument().Where(a => a.Name == "$skip" && a.Value.ToInt64() == 0);
                })
                .Returns(mockCountResult.Object);

            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            // 対象メソッド実行
            var result = target.QueryDocumentContinuation("", 2, new List<BsonDocument>()).ToList();
            result.Count.Is(0);

            // 呼ばれたか確認
            mockClient.Verify(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void QueryDocumentContinuation_OK_先頭ページ次ページなし()
        {
            var dbResult = new List<BsonDocument>()
            {
                BsonDocument.Parse("{ '_id': 'hoge1', 'id': 'fuga1', 'key': 'foo1', 'value': NumberDecimal('1') }")
            };
            var expectedResult = new List<Tuple<JToken, string>>()
            {
                new Tuple<JToken, string>(JToken.Parse("{ 'id': 'fuga1', 'key': 'foo1', 'value': 1 }"), "")
            };

            var mockCountResult = new Mock<IAsyncCursor<BsonDocument>>();
            mockCountResult.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
            mockCountResult.SetupSequence(x => x.Current).Returns(dbResult);

            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()))
                .Callback<PipelineDefinition<BsonDocument, BsonDocument>, AggregateOptions>((x, y) =>
                {
                    x.ToBsonDocument().Where(a => a.Name == "$skip" && a.Value.ToInt64() == 0);
                })
                .Returns(mockCountResult.Object);

            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            // 対象メソッド実行
            var result = target.QueryDocumentContinuation("", 2, new List<BsonDocument>()).ToList();
            result.IsStructuralEqual(expectedResult);

            // 呼ばれたか確認
            mockClient.Verify(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void QueryDocumentContinuation_OK_先頭ページ次ページあり()
        {
            var dbResult = new List<BsonDocument>()
            {
                BsonDocument.Parse("{ '_id': 'hoge1', 'id': 'fuga1', 'key': 'foo1', 'value': NumberDecimal('1') }"),
                BsonDocument.Parse("{ '_id': 'hoge2', 'id': 'fuga2', 'key': 'foo2', 'value': NumberDecimal('2') }"),
                BsonDocument.Parse("{ '_id': 'hoge3', 'id': 'fuga3', 'key': 'foo3', 'value': NumberDecimal('3') }")
            };
            var expectedResult = new List<Tuple<JToken, string>>()
            {
                new Tuple<JToken, string>(JToken.Parse("{ 'id': 'fuga1', 'key': 'foo1', 'value': 1 }"), "1"),
                new Tuple<JToken, string>(JToken.Parse("{ 'id': 'fuga2', 'key': 'foo2', 'value': 2 }"), "2")
            };

            var mockCountResult = new Mock<IAsyncCursor<BsonDocument>>();
            mockCountResult.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(true).Returns(true).Returns(false);
            mockCountResult.SetupSequence(x => x.Current).Returns(dbResult.Take(1)).Returns(dbResult.Skip(1).Take(1)).Returns(dbResult.Skip(2));

            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()))
                .Callback<PipelineDefinition<BsonDocument, BsonDocument>, AggregateOptions>((x, y) =>
                {
                    x.ToBsonDocument().Where(a => a.Name == "$skip" && a.Value.ToInt64() == 0);
                })
                .Returns(mockCountResult.Object);

            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            // 対象メソッド実行
            var result = target.QueryDocumentContinuation("", 2, new List<BsonDocument>()).ToList();
            result.IsStructuralEqual(expectedResult);

            // 呼ばれたか確認
            mockClient.Verify(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void QueryDocumentContinuation_OK_途中ページ次ページあり()
        {
            var dbResult = new List<BsonDocument>()
            {
                BsonDocument.Parse("{ '_id': 'hoge1', 'id': 'fuga1', 'key': 'foo1', 'value': NumberDecimal('1') }"),
                BsonDocument.Parse("{ '_id': 'hoge2', 'id': 'fuga2', 'key': 'foo2', 'value': NumberDecimal('2') }"),
                BsonDocument.Parse("{ '_id': 'hoge3', 'id': 'fuga3', 'key': 'foo3', 'value': NumberDecimal('3') }")
            };
            var expectedResult = new List<Tuple<JToken, string>>()
            {
                new Tuple<JToken, string>(JToken.Parse("{ 'id': 'fuga1', 'key': 'foo1', 'value': 1 }"), "5"),
                new Tuple<JToken, string>(JToken.Parse("{ 'id': 'fuga2', 'key': 'foo2', 'value': 2 }"), "6")
            };

            var mockCountResult = new Mock<IAsyncCursor<BsonDocument>>();
            mockCountResult.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(true).Returns(false);
            mockCountResult.SetupSequence(x => x.Current).Returns(dbResult);

            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()))
                .Callback<PipelineDefinition<BsonDocument, BsonDocument>, AggregateOptions>((x, y) =>
                {
                    x.ToBsonDocument().Where(a => a.Name == "$skip" && a.Value.ToInt64() == 4);
                })
                .Returns(mockCountResult.Object);

            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            // 対象メソッド実行
            var result = target.QueryDocumentContinuation("4", 2, new List<BsonDocument>()).ToList();
            result.IsStructuralEqual(expectedResult);

            // 呼ばれたか確認
            mockClient.Verify(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void QueryDocumentContinuation_OK_途中ページ次ページなし()
        {
            var dbResult = new List<BsonDocument>()
            {
                BsonDocument.Parse("{ '_id': 'hoge1', 'id': 'fuga1', 'key': 'foo1', 'value': NumberDecimal('1') }"),
                BsonDocument.Parse("{ '_id': 'hoge2', 'id': 'fuga2', 'key': 'foo2', 'value': NumberDecimal('2') }")
            };
            var expectedResult = new List<Tuple<JToken, string>>()
            {
                new Tuple<JToken, string>(JToken.Parse("{ 'id': 'fuga1', 'key': 'foo1', 'value': 1 }"), "5"),
                new Tuple<JToken, string>(JToken.Parse("{ 'id': 'fuga2', 'key': 'foo2', 'value': 2 }"), "")
            };

            var mockCountResult = new Mock<IAsyncCursor<BsonDocument>>();
            mockCountResult.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(true).Returns(false);
            mockCountResult.SetupSequence(x => x.Current).Returns(dbResult.Take(1)).Returns(dbResult.Skip(1));

            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()))
                .Callback<PipelineDefinition<BsonDocument, BsonDocument>, AggregateOptions>((x, y) =>
                {
                    x.ToBsonDocument().Where(a => a.Name == "$skip" && a.Value.ToInt64() == 4);
                })
                .Returns(mockCountResult.Object);

            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            // 対象メソッド実行
            var result = target.QueryDocumentContinuation("4", 2, new List<BsonDocument>()).ToList();
            result.IsStructuralEqual(expectedResult);

            // 呼ばれたか確認
            mockClient.Verify(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void CountDocument_OK()
        {
            var expectedCount = 99;

            var mockCountResult = new Mock<IAsyncCursor<BsonDocument>>();
            mockCountResult.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
            mockCountResult.SetupGet(x => x.Current).Returns(new[] { new BsonDocument("count", expectedCount) });

            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()))
                .Returns(mockCountResult.Object);

            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            string message;
            var mockValidator = new Mock<MongoDbQuerySyntaxValidatior>();
            mockValidator.Setup(x => x.ValidateFindSyntax(It.IsAny<BsonDocument>(), out message)).Returns(true);
            UnityContainer.RegisterInstance<MongoDbQuerySyntaxValidatior>("mng", mockValidator.Object);

            // 対象メソッド実行
            var result = target.CountDocument(new List<BsonDocument>());

            // Mockの期待値が返却されること
            result.Value<long>().Is(expectedCount);

            // 呼ばれたか確認
            mockClient.Verify(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void CountDocument_OK_0件()
        {
            var mockCountResult = new Mock<IAsyncCursor<BsonDocument>>();
            mockCountResult.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(false);

            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()))
                .Returns(mockCountResult.Object);

            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            string message;
            var mockValidator = new Mock<MongoDbQuerySyntaxValidatior>();
            mockValidator.Setup(x => x.ValidateFindSyntax(It.IsAny<BsonDocument>(), out message)).Returns(true);
            UnityContainer.RegisterInstance<IQuerySyntaxValidator>("mng", mockValidator.Object);

            // 対象メソッド実行
            var result = target.CountDocument(new List<BsonDocument>());

            // Mockの期待値が返却されること
            result.Value<long>().Is(0);

            // 呼ばれたか確認
            mockClient.Verify(x => x.Aggregate(
                It.IsAny<PipelineDefinition<BsonDocument, BsonDocument>>(),
                It.IsAny<AggregateOptions>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void UpsertDocument_OK()
        {
            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            var targetDocument =
                @"{
                    'id': 'id',
                    'AreaUnitCode': 'AA',
                    'AreaUnitName': 'aaa',
                    'IntValue': 123,
                    'FloatValue': 0.0001,
                }";
            var expectedDocument =
                @"{
                    'id': 'id',
                    'AreaUnitCode': 'AA',
                    'AreaUnitName': 'aaa',
                    'IntValue': NumberDecimal('123'),
                    'FloatValue': NumberDecimal('0.0001'),
                }";
            var targetBson = BsonSerializer.Deserialize<BsonDocument>(targetDocument);
            var targetJson = JToken.FromObject(JsonConvert.DeserializeObject(targetDocument));

            // Mockでcollectionを上書き
            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.FindOneAndReplace(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<BsonDocument>(),
                It.IsAny<FindOneAndReplaceOptions<BsonDocument>>()))
                .Callback(
                    (FilterDefinition<BsonDocument> filter,
                    BsonDocument replacement,
                    FindOneAndReplaceOptions<BsonDocument, BsonDocument> options) =>
                    {
                        // 引数チェック
                        //filter.IsStructuralEqual(Builders<BsonDocument>.Filter.Eq("id", targetBson["id"].ToString()));
                        replacement.Is(targetBson);

                        // データチェック
                        //replacement.IsStructuralEqual(BsonDocument.Parse(expectedDocument));
                    })
                .Returns(targetBson);
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            // 対象メソッド実行
            var result = target.UpsertDocument(targetJson);

            // 登録・更新対象のidが返却されること
            result.Is(targetBson["id"].ToString());

            // 呼ばれたか確認
            mockClient.Verify(x => x.FindOneAndReplace(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<BsonDocument>(),
                It.IsAny<FindOneAndReplaceOptions<BsonDocument>>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void DeleteDocument_OK()
        {
            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            var expectCount = 1;
            var where = @"
                    { '$and' : [
                        { '_Type' : 'API~UnitTest~Mongo~DeleteDocument' },
                        { '_Version' : 1 },
                        { '_partitionkey' : 'API~UnitTest~Mongo~DeleteDocument~1' }
                    ]}";
            var expectedWhere = @"
                    { '$and' : [
                        { '_Type' : 'API~UnitTest~Mongo~DeleteDocument' },
                        { '_Version' : NumberDecimal('1') },
                        { '_partitionkey' : 'API~UnitTest~Mongo~DeleteDocument~1' }
                    ]}";

            var mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.Setup(x => x.DeletedCount).Returns(1);
            var mockClient = new Mock<IProfiledMongoDbClient<BsonDocument>>();
            mockClient.Setup(x => x.DeleteMany(
                It.IsAny<FilterDefinition<BsonDocument>>()))
                .Callback(
                (FilterDefinition<BsonDocument> filter) =>
                {
                    // 引数チェック
                    // filter.IsStructuralEqual((BsonDocumentFilterDefinition<BsonDocument>)BsonDocument.Parse(expectedWhere));
                })
                .Returns(mockDeleteResult.Object);
            target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(target, mockClient.Object);

            string message;
            var mockValidator = new Mock<MongoDbQuerySyntaxValidatior>();
            mockValidator.Setup(x => x.ValidateFindSyntax(It.IsAny<BsonDocument>(), out message)).Returns(true);
            UnityContainer.RegisterInstance<IQuerySyntaxValidator>("mng", mockValidator.Object);

            // 対象メソッド実行
            var result = target.DeleteDocument(where);

            // Mockの期待値が返却されること
            result.Is(expectCount);

            // 呼ばれたか確認
            mockClient.Verify(x => x.DeleteMany(
                It.IsAny<FilterDefinition<BsonDocument>>()),
                Times.Exactly(1));

            mockValidator.Verify(x => x.ValidateFindSyntax(
                It.IsAny<BsonDocument>(),
                out message),
                Times.Exactly(1));
        }

        [TestMethod]
        public void DeleteDocument_NG()
        {
            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            var where = @"
                    { '$and' : [
                        { '_Type' : 'API~UnitTest~Mongo~DeleteDocument' },
                        { '_Version' : 1 },
                        { '_partitionkey' : 'API~UnitTest~Mongo~DeleteDocument~1' }
                    ]}";

            var mockDeleteResult = new Mock<DeleteResult>();
            mockDeleteResult.Setup(x => x.DeletedCount).Returns(1);
            var mockCollection = new Mock<IMongoCollection<BsonDocument>>();

            string message;
            var mockValidator = new Mock<MongoDbQuerySyntaxValidatior>();
            mockValidator.Setup(x => x.ValidateFindSyntax(It.IsAny<BsonDocument>(), out message)).Returns(false);
            UnityContainer.RegisterInstance<IQuerySyntaxValidator>("mng", mockValidator.Object);

            // 対象メソッド実行
            AssertEx.Throws<QuerySyntaxErrorException>(() => target.DeleteDocument(where));

            // 呼ばれたか確認
            mockCollection.Verify(x => x.DeleteMany(
                It.IsAny<FilterDefinition<BsonDocument>>(),
                It.IsAny<CancellationToken>()),
                Times.Exactly(0));

            mockValidator.Verify(x => x.ValidateFindSyntax(
                It.IsAny<BsonDocument>(),
                out message),
                Times.Exactly(1));
        }

        [Ignore] // DevOpsでの実行でNGとなるため一旦無効化
        [TestMethod]
        public void ConnectionString_OK()
        {
            // 初期設定
            var userName = Guid.NewGuid().ToString();
            var password = Guid.NewGuid().ToString();
            var serverName1 = Guid.NewGuid().ToString();
            var port1 = 1234;
            var serverName2 = Guid.NewGuid().ToString();
            var port2 = 4321;
            var replicaSet = Guid.NewGuid().ToString();
            var useTls = true;
            var allowInsecureTls = false;

            var databaseName = Guid.NewGuid().ToString();
            var collectionName = Guid.NewGuid().ToString();

            var target = UnityContainer.Resolve<IJPDataHubMongoDb>();
            target.ConnectionString = $"endpoint=mongodb://{userName}:{password}@{serverName1}:{port1},{serverName2}:{port2}/?replicaSet={replicaSet}&tls={useTls}&sslverifycertificate={!allowInsecureTls};database={databaseName};collection={collectionName}";

            var clientField = target.GetType().GetField("_client", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
            var profiledMongoClient = (IProfiledMongoDbClient<BsonDocument>)clientField.GetValue(target);
            var mongoClient = profiledMongoClient.GetType().GetField("MongoClient", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profiledMongoClient) as MongoClient;
            mongoClient.Settings.Credential.Username.Is(userName);
            mongoClient.Settings.Servers.First().Host.Is(serverName1);
            mongoClient.Settings.Servers.First().Port.Is(port1);
            mongoClient.Settings.Servers.Last().Host.Is(serverName2);
            mongoClient.Settings.Servers.Last().Port.Is(port2);
            mongoClient.Settings.ReplicaSetName.Is(replicaSet);
            mongoClient.Settings.UseTls.Is(useTls);
            mongoClient.Settings.AllowInsecureTls.Is(allowInsecureTls);

            var database = (profiledMongoClient.GetType().GetField("Database", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profiledMongoClient) as IMongoDatabase);
            database.DatabaseNamespace.DatabaseName.Is(databaseName);

            var collection = (profiledMongoClient.GetType().GetField("Collection", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profiledMongoClient) as IMongoCollection<BsonDocument>);
            collection.CollectionNamespace.CollectionName.Is(collectionName);

            // 接続文字列変更
            userName = Guid.NewGuid().ToString();
            password = Guid.NewGuid().ToString();
            serverName1 = Guid.NewGuid().ToString();
            port1 = 12345;
            serverName2 = Guid.NewGuid().ToString();
            port2 = 54321;
            replicaSet = Guid.NewGuid().ToString();
            useTls = true;
            allowInsecureTls = false;

            databaseName = Guid.NewGuid().ToString();
            collectionName = Guid.NewGuid().ToString();

            target.ConnectionString = $"endpoint=mongodb://{userName}:{password}@{serverName1}:{port1},{serverName2}:{port2}/?replicaSet={replicaSet}&tls={useTls}&sslverifycertificate={!allowInsecureTls};database={databaseName};collection={collectionName}";

            profiledMongoClient = (IProfiledMongoDbClient<BsonDocument>)clientField.GetValue(target);
            mongoClient = profiledMongoClient.GetType().GetField("MongoClient", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profiledMongoClient) as MongoClient;
            mongoClient.Settings.Credential.Username.Is(userName);
            mongoClient.Settings.Servers.First().Host.Is(serverName1);
            mongoClient.Settings.Servers.First().Port.Is(port1);
            mongoClient.Settings.Servers.Last().Host.Is(serverName2);
            mongoClient.Settings.Servers.Last().Port.Is(port2);
            mongoClient.Settings.ReplicaSetName.Is(replicaSet);
            mongoClient.Settings.UseTls.Is(useTls);
            mongoClient.Settings.AllowInsecureTls.Is(allowInsecureTls);

            database = (profiledMongoClient.GetType().GetField("Database", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profiledMongoClient) as IMongoDatabase);
            database.DatabaseNamespace.DatabaseName.Is(databaseName);

            collection = (profiledMongoClient.GetType().GetField("Collection", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(profiledMongoClient) as IMongoCollection<BsonDocument>);
            collection.CollectionNamespace.CollectionName.Is(collectionName);
        }

        [Ignore] // DevOpsでの実行でNGとなるため一旦無効化
        [TestMethod]
        [TestCase("", typeof(ArgumentNullException))]
        [TestCase("endpoint=;database=databasename;collection=collectionname;", typeof(MongoConfigurationException))]
        [TestCase("endpoint=mongodb://servername;database=;collection=collectionname;", typeof(ArgumentException))]
        [TestCase("endpoint=mongodb://servername;database=databasename;collection=;", typeof(ArgumentException))]
        [TestCase("database=databasename;collection=collectionname;", typeof(ArgumentNullException))]
        [TestCase("endpoint=mongodb://servername;collection=collectionname;", typeof(ArgumentNullException))]
        [TestCase("endpoint=mongodb://servername;database=databasename;", typeof(ArgumentNullException))]
        public void ConnectionString_NG()
        {
            TestContext.Run((string connectionString, Type type) =>
            {
                bool thrown = false;

                try
                {
                    new ProfiledMongoDbClient<BsonDocument>(connectionString);
                }
                catch (Exception ex)
                {
                    ex.GetType().Is(type);
                    thrown = true;
                }
                finally
                {
                    thrown.IsTrue();
                }
            });
        }
    }
}