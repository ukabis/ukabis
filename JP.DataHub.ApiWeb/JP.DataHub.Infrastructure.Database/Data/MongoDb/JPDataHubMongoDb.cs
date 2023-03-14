using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.Infrastructure.Database.Data.MongoDb
{
    public class JPDataHubMongoDb : IJPDataHubMongoDb
    {
        private static object lockobj = new object();

        private IProfiledMongoDbClient<BsonDocument> _client;

        private string _connectionString;
        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                lock (lockobj)
                {
                    //設定してある接続文字列と違う場合再接続する。
                    if (_connectionString != value)
                    {
                        _connectionString = value;
                        InitConnection();
                    }
                }
            }
        }

        public string CollectionName => _client.CollectionName;


        private void InitConnection()
        {
            _client = new ProfiledMongoDbClient<BsonDocument>(ConnectionString);
        }

        public string UpsertDocument(JToken json)
        {
            // 浮動小数点の誤差を回避するため数値をDecimal型に変換
            var data = json.ToDecimalizedBsonDocument();

            var filter = Builders<BsonDocument>.Filter.Eq(JsonPropertyConst.ID, json[JsonPropertyConst.ID].ToString());
            var bson = _client.FindOneAndReplace(filter, data, new FindOneAndReplaceOptions<BsonDocument> { ReturnDocument = ReturnDocument.After, IsUpsert = true });
            return bson[JsonPropertyConst.ID].AsString; // 登録・更新対象のidを返却
        }

        public long DeleteDocument(string query)
        {
            // 数値データはDecimalで保存されているためクエリの数値もDecimal化
            var bsonQuery = query.ToDecimalizedBsonDocument();

            // セキュリティ観点から実行時にも構文チェックを実施
            if (!GetQuerySyntaxValidatior().ValidateFindSyntax(bsonQuery, out var message))
            {
                throw new QuerySyntaxErrorException(message);
            }

            return _client.DeleteMany(bsonQuery).DeletedCount;
        }

        public void CreateWildcardIndex()
        {
            var model = new CreateIndexModel<BsonDocument>(Builders<BsonDocument>.IndexKeys.Wildcard());
            _client.CreateIndex(model);
        }

        public JToken CountDocument(IEnumerable<BsonDocument> pipeline)
        {
            var pipelineDefinition = PipelineDefinition<BsonDocument, BsonDocument>.Create(pipeline);

            // Aggregateで0件の場合は結果なしになる
            var result = _client.Aggregate(pipelineDefinition, GetAggregateOptions()).FirstOrDefault();
            var count = result == null ? 0 : result["count"].ToInt64();
            return $"{count}";
        }

        public IEnumerable<JToken> QueryDocument(IEnumerable<BsonDocument> pipeline)
        {
            var pipelineDefinition = PipelineDefinition<BsonDocument, BsonDocument>.Create(pipeline);
            foreach (var doc in _client.Aggregate(pipelineDefinition, GetAggregateOptions()).ToEnumerable<BsonDocument>())
            {
                // _idで「ObjectId(guid??)」形式の場合JsonのParseで失敗してしまうので_idは削除しておく
                doc.Remove(JsonPropertyConst._ID);
                var json = doc.ToJsonWithReverseDecimalization();
                yield return json;
            }
        }

        public IEnumerable<Tuple<JToken, string>> QueryDocumentContinuation(string requestContinuation, int top, IEnumerable<BsonDocument> pipeline)
        {
            // X-RequestContinuationの指定に応じて返却済みページをスキップ
            int? continuationSkipCount = null;
            if (int.TryParse(requestContinuation, out var continuationValue))
            {
                continuationSkipCount = continuationValue;
            }

            // クエリ実行
            var pipelineDefinition = PipelineDefinition<BsonDocument, BsonDocument>.Create(pipeline);
            pipelineDefinition = pipelineDefinition.Skip(continuationSkipCount ?? 0).Limit(top + 1);
            var enumerator = _client.Aggregate(pipelineDefinition, GetAggregateOptions()).ToEnumerable().GetEnumerator();

            // データ取得
            var hasNext = enumerator.MoveNext();
            for (var i = 0; hasNext && i < top; i++)
            {
                var doc = enumerator.Current;
                hasNext = enumerator.MoveNext();

                // 次のデータがある場合のみ次位置を返却
                var responseContinuation = hasNext ? ((continuationSkipCount ?? 0) + i + 1).ToString() : string.Empty;

                // _idで「ObjectId(guid??)」形式の場合JsonのParseで失敗してしまうので_idは削除しておく
                doc.Remove(JsonPropertyConst._ID);
                yield return new Tuple<JToken, string>(doc.ToJsonWithReverseDecimalization(), responseContinuation);
            }
        }


        private MongoDbQuerySyntaxValidatior GetQuerySyntaxValidatior()
        {
            return (MongoDbQuerySyntaxValidatior)UnityCore.Resolve<IQuerySyntaxValidator>("mng");
            //return (MongoDbQuerySyntaxValidatior)UnityCore.Resolve<IQuerySyntaxValidator>(RepositoryType.MomgoDB.ToCode());
        }

        private AggregateOptions GetAggregateOptions()
        {
            // MongoDBはソートに100MB以上メモリを必要とするクエリはエラーとなる仕様
            // AllowDiskUse=trueとすることで100MB以上の場合はディスクを使用するようになる(低速だがエラーにならない)
            return new AggregateOptions() { AllowDiskUse = true };
        }
    }
}
