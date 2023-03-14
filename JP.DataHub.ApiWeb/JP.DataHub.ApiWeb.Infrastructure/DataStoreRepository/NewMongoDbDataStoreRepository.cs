using System.Collections.Concurrent;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Sql;
using JP.DataHub.Infrastructure.Database.Data;
using JP.DataHub.Infrastructure.Database.Data.MongoDb;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    internal class NewMongoDbDataStoreRepository : NewAbstractDynamicApiDataStoreRepository
    {
        private const int _defaultTopCount = 100;
        private const string _resourceSharingCondition = "_ResourceSharing_Condition";

        private static ConcurrentDictionary<string, IJPDataHubMongoDb> s_connectionClient = new ConcurrentDictionary<string, IJPDataHubMongoDb>();
        private static Lazy<IMongoDbODataSqlManager> s_oDataSqlManager = new Lazy<IMongoDbODataSqlManager>(() => UnityCore.Resolve<IMongoDbODataSqlManager>());

        /// <summary>
        /// クエリが可能かどうか
        /// </summary>
        public override bool CanQuery { get => true; }
        /// <summary>
        /// 楽観排他が可能かどうか
        /// </summary>
        public override bool CanOptimisticConcurrency { get => true; }
        /// <summary>
        /// ODataのanyが使用可能かどうか
        /// </summary>
        public override bool CanQueryAttachFileMetaByOData { get => true; }
        /// <summary>
        /// バージョン情報取得用クエリ
        /// </summary>
        public override string VersionInfoQuery { get => "DummyMongoDBVersionInfoQuery"; }
        /// <summary>
        /// リポジトリ名
        /// </summary>
        public override string RepositoryName { get => "MongoDB"; }

        private IMongoDbODataSqlManager _oDataSqlManager { get => s_oDataSqlManager.Value; }
        private IContainerDynamicSeparationRepository _containerDynamicSeparationRepository = UnityCore.Resolve<IContainerDynamicSeparationRepository>();


        #region Query

        /// <summary>
        /// クエリを実行する。
        /// </summary>
        //[DataStoreRepositoryParamODataConvert]
        public override JsonDocument QueryOnce(QueryParam param)
        {
            var doc = query(param.ToSingle()).FirstOrDefault();
            return doc == null ? null : new JsonDocument(doc.Item1);
        }

        /// <summary>
        /// クエリを実行する。
        /// </summary>
        [DataStoreRepositoryParamODataConvert]
        public override IEnumerable<JsonDocument> QueryEnumerable(QueryParam param)
        {
            foreach (var x in query(param.ToSingle()))
            {
                yield return new JsonDocument(x.Item1);
            }
        }

        /// <summary>
        /// クエリを実行する。
        /// </summary>
        [DataStoreRepositoryParamODataConvert]
        public override IList<JsonDocument> Query(QueryParam param, out XResponseContinuation xResponseContinuation)
        {
            List<JsonDocument> result = new List<JsonDocument>();
            var queryResult = query(param.ToSingle()).ToList();
            queryResult.ForEach(x => result.Add(new JsonDocument(x.Item1)));
            xResponseContinuation = queryResult.LastOrDefault()?.Item2; // 最後尾のインデックスを次ページの開始位置として取得
            return result;
        }

        #endregion


        #region Register

        /// <summary>
        /// データを登録する。
        /// </summary>
        public override RegisterOnceResult RegisterOnce(RegisterParam param)
        {
            var isResourceSharingPerson = IsResourceSharingPerson(param.XResourceSharingPerson, param.ResourceSharingPersonRules, out var sharingFromOpenId);

            if (param.IsOverrideId?.Value == true)
            {
                var version = ResourceVersionRepository.GetRegisterVersion(param.RepositoryKey, param.XVersion);

                NewCosmosDbDataStoreRepository.AddManagementData(param.RepositoryKey,
                    param.IsVendor,
                    param.VendorId,
                    param.SystemId,
                    param.IsAutomaticId,
                    param.PartitionKey,
                    param.IsPerson,
                    // 個人共有の場合は共有元のデータとして登録
                    isResourceSharingPerson ? new OpenId(this.PerRequestDataContainer.XResourceSharingPerson) : param.OpenId,
                    param.Json,
                    version);
            }

            var client = GetConnection(RepositoryInfo, param.IsContainerDynamicSeparation, param.ControllerId, param.IsVendor, param.IsPerson, null, null, sharingFromOpenId);
            return new RegisterOnceResult(client.UpsertDocument(param.Json));
        }

        #endregion


        #region Delete

        /// <summary>
        /// データを削除する。
        /// </summary>
        public override void DeleteOnce(DeleteParam param)
        {
            _ = IsResourceSharingPerson(param.XResourceSharingPerson, param.ResourceSharingPersonRules, out var sharingFromOpenId);

            var client = GetConnection(RepositoryInfo, param.IsContainerDynamicSeparation, param.ControllerId, param.IsVendor, param.IsPerson, null, null, sharingFromOpenId);
            var query = _oDataSqlManager.CreateSqlQueryEx($"$filter={JsonPropertyConst.ID} eq '{param.Json[JsonPropertyConst.ID]}'", string.Empty);
            client.DeleteDocument(query.Where);
            param.CallbackDelete?.Invoke(param.Json, RepositoryInfo.Type);
        }

        /// <summary>
        /// データを削除する。
        /// </summary>
        public override IEnumerable<string> Delete(DeleteParam param)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Connection

        private IJPDataHubMongoDb GetConnection(
            RepositoryInfo repositoryInfo, IsContainerDynamicSeparation isContainerDynamicSeparation, ControllerId controllerId, IsVendor isVendor, IsPerson isPerson,
            VendorId sharingFromVendorId = null, SystemId sharingFromSystemId = null, OpenId sharingFromOpenId = null)
        {
            var connectionString = repositoryInfo.ConnectionString;
            var isRegistered = false;

            // コレクション分離の場合は接続文字列を編集(依存なしはコレクション分離非対応)
            if (isContainerDynamicSeparation?.Value == true && (isVendor?.Value == true || isPerson?.Value == true))
            {
                const string ENDPOINT = "endpoint";
                const string DATABASE = "database";
                const string COLLECTION = "collection";

                var collectionName = GetCollectionName(repositoryInfo, controllerId, isVendor, isPerson, sharingFromVendorId, sharingFromSystemId, sharingFromOpenId, out isRegistered);
                var pcs = new ParseConnectionString(connectionString);
                if (pcs.Parameters.ContainsKey(COLLECTION))
                {
                    pcs.Parameters[COLLECTION] = collectionName;
                }
                else
                {
                    pcs.Parameters.Add(COLLECTION, collectionName);
                }

                connectionString = pcs.CreateConnectionString(new string[] { ENDPOINT, DATABASE, COLLECTION });
            }

            IJPDataHubMongoDb client;
            if (!s_connectionClient.ContainsKey(connectionString))
            {
                client = UnityCore.Resolve<IJPDataHubMongoDb>();
                client.ConnectionString = connectionString;
                s_connectionClient.GetOrAdd(connectionString, client);
            }
            else
            {
                client = s_connectionClient[connectionString];
            }

            // 新規コンテナの場合はワイルドカードインデックスを設定
            // コレクションはインデックス作成時(またはデータ操作時)にMongoDBにより自動作成される
            // ContainerDynamicSeparationのデータを移行する場合、コレクションは自動作成されるがインデックスは自動設定されないため注意
            if (isRegistered)
            {
                client.CreateWildcardIndex();
            }

            return client;
        }

        private string GetCollectionName(
            RepositoryInfo repositoryInfo, ControllerId controllerId, IsVendor isVendor, IsPerson isPerson,
            VendorId sharingFromVendorId, SystemId sharingFromSystemId, OpenId sharingFromOpenId,
            out bool isRegistered)
        {
            var vendorId = isVendor?.Value == true ? (sharingFromVendorId ?? new VendorId(PerRequestDataContainer.VendorId)) : new VendorId(Guid.Empty.ToString());
            var systemId = isVendor?.Value == true ? (sharingFromSystemId ?? new SystemId(PerRequestDataContainer.SystemId)) : new SystemId(Guid.Empty.ToString());
            var opendId = isPerson?.Value == true ? (sharingFromOpenId ?? new OpenId(PerRequestDataContainer.OpenId)) : new OpenId(Guid.Empty.ToString());

            return _containerDynamicSeparationRepository.GetOrRegisterContainerName(
                repositoryInfo.PhysicalRepositoryId,
                controllerId,
                vendorId,
                systemId,
                opendId,
                out isRegistered);
        }

        #endregion


        /// <summary>
        /// 内部で追加するクエリストリングを作成する。
        /// </summary>
        public override string GetInternalAddWhereString(QueryParam param, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();
            return string.Empty;
        }


        private IEnumerable<Tuple<JToken, XResponseContinuation>> query(QueryParam param)
        {
            SeparatedQuerySyntax syntax = null;

            if (param.OperationInfo?.IsVersionOperation == true)
            {
                // バージョン操作:パラメータからクエリ生成
                var where = ConvertToLogicalAndExpression(param.NativeQuery.Dic).ToString();
                syntax = new SeparatedQuerySyntax(null, where, null, null, null);
            }
            else
            {
                if (param?.IsNative == true)
                {
                    // OData、APIクエリ(ODataクエリ)
                    syntax = JsonConvert.DeserializeObject<SeparatedQuerySyntax>(param.NativeQuery.Sql);
                    syntax.Where = CreateWhereCondition(param, syntax.Where);
                }
                else if (!string.IsNullOrWhiteSpace(param.ApiQuery?.Value))
                {
                    // APIクエリ(既定のクエリ)
                    var apiQuery = param.ApiQuery.Value;

                    // QueryStringの値を置換する
                    if (param.QueryString?.HasValue == true)
                    {
                        List<JSchema> schemas = new List<JSchema>() { param.UriSchema?.ToJSchema(), param.RequestSchema?.ToJSchema(), param.ResponseSchema?.ToJSchema() };
                        foreach (var key in param.QueryString.Dic)
                        {
                            // QueryStringの値の型を取得し、文字列型の場合はダブルクォーテーションで囲む
                            var valueType = Convert.ChangeType(key.Value.Value, schemas.ToType(key.Key.Value)).GetType();
                            apiQuery = apiQuery.Replace($"{{{key.Key.Value}}}", valueType == typeof(string) ? $@"""{key.Value.Value}""" : $"{key.Value.Value}");
                        }
                    }

                    // コレクション名変数を置換する(一時的にBson化可能な形式に変換し、実行時に実際のコレクション名に置換)
                    apiQuery = apiQuery.Replace($"{{{MongoDbConstants.CollectionNameVariable}}}", $"\"{MongoDbConstants.CollectionNameVariable}\"");

                    // ApiQueryが「既定のクエリ」の値の場合Select, Where, OrderByプロパティの型がBsonDocumentになるため、
                    // SeparatedQuerySyntaxクラスにデシリアライズするためStringに変換する
                    var bson = apiQuery.ToDecimalizedBsonDocument();
                    bson.Elements.Where(x => x.Value.IsBsonDocument || x.Value.IsBsonArray).ToList().ForEach(x => bson[x.Name] = x.Value.ToString());

                    // Where条件をマージしてsyntaxに設定する
                    syntax = bson.ToObject<SeparatedQuerySyntax>();
                    syntax.Where = CreateWhereCondition(param, syntax.Where, true);

                    // Aggregateの場合、パイプライン定義を編集
                    if (syntax.IsAggregation)
                    {
                        EditAndValidateAggregationPipeline(syntax);
                    }
                }
                else
                {
                    // APIクエリなし
                    var select = BuildSelect(param);
                    syntax = new SeparatedQuerySyntax(select, null, null, null, null);
                    syntax.Where = CreateWhereCondition(param);
                }

                // 削除の場合は削除に必要な項目のみ取得する
                // (履歴は変更前のデータを保存するために全項目を取得しないといけないため通常通り)
                if ((param?.ActionType?.Value == Domain.Context.DynamicApi.ActionType.DeleteData || param?.ActionType?.Value == Domain.Context.DynamicApi.ActionType.ODataDelete) &&
                    !syntax.IsAggregation)
                {
                    // _idは不要なため除外
                    syntax.Select = $"{{ \"{JsonPropertyConst.ID}\": 1, \"{JsonPropertyConst._ID}\": 0 }}";
                }
            }

            // コネクション取得(コンテナ分離かつデータ共有の場合は共有元のコンテナに接続)
            _ = IsResourceSharingWith(param.XResourceSharingWith, param.ApiResourceSharing, out var sharingFromVendorId, out var sharingFromSystemId);
            _ = IsResourceSharingPerson(param.XResourceSharingPerson, param.ResourceSharingPersonRules, out var sharingFromOpenId);
            var client = GetConnection(
                RepositoryInfo, param.IsContainerDynamicSeparation, param.ControllerId, param.IsVendor, param.IsPerson,
                sharingFromVendorId, sharingFromSystemId, sharingFromOpenId);

            // Aggregateパイプライン作成
            var isCountQuery = syntax.IsCountQuery();
            var isPaging = !isCountQuery && param.XRequestContinuation?.ContinuationString != null && !(XRequestContinuationNeedsTopCount && syntax.Top == null);
            var pipelineBsonArray = ConvertToAggregatePipeline(param, syntax, client.CollectionName, isPaging);

            // MongoDBへのクエリを実行
            if (isCountQuery)
            {
                var count = client.CountDocument(pipelineBsonArray);
                yield return new Tuple<JToken, XResponseContinuation>(count, null);
            }
            else if (isPaging)
            {
                var top = syntax.Top ?? _defaultTopCount;
                foreach (var json in client.QueryDocumentContinuation(param.XRequestContinuation?.ContinuationString, top, pipelineBsonArray))
                {
                    yield return new Tuple<JToken, XResponseContinuation>(json.Item1, new XResponseContinuation(json.Item2));
                }
            }
            else
            {
                foreach (var json in client.QueryDocument(pipelineBsonArray))
                {
                    yield return new Tuple<JToken, XResponseContinuation>(json, null);
                }
            }
        }


        private string CreateWhereCondition(QueryParam param, string apiQueryWhere = null, bool excludeQueryStringParameters = false)
        {
            var queryCondition = CreateBasicQueryCondition(param);
            if (param?.IsNative == true && queryCondition.Count == 0)
            {
                param.NativeQuery.Dic.ToList().ForEach(x => queryCondition.Add(x.Key, x.Value));
            }

            // 既定の条件を追加(除外指定ありの場合はQueryStringの項目を除外)
            var baseConditions = !excludeQueryStringParameters
                ? new Dictionary<string, object>(queryCondition).Where(x => x.Key != _resourceSharingCondition)
                : new Dictionary<string, object>(queryCondition).Where(x => x.Key != _resourceSharingCondition && !(param.QueryString?.ContainKey(x.Key) ?? false));

            // クエリ組み立て
            var conditions = "{ '$and' : [] }".ToDecimalizedBsonDocument();
            var andArray = conditions.GetElement("$and");
            baseConditions.ToList().ForEach(condition =>
            {
                if (andArray.Value.AsBsonArray.All(x => !x.AsBsonDocument.Contains(condition.Key)))
                {
                    andArray.Value.AsBsonArray.Add(new BsonDocument(condition.Key, condition.Value.ToDecimalizedBsonValue()));
                }
            });

            // データ共有のクエリを追加
            if (queryCondition.ContainsKey(_resourceSharingCondition))
            {
                andArray.Value.AsBsonArray.Add(queryCondition[_resourceSharingCondition] as BsonValue);
            }

            // APIクエリの条件を追加
            if (!string.IsNullOrEmpty(apiQueryWhere))
            {
                var additionalConditions = apiQueryWhere.ToDecimalizedBsonDocument();

                // $and/$or/$norの空配列はFind実行エラーとなるため無視
                // (現状システム的に発生するトップレベルの単一項目のみ対応)
                var element = additionalConditions.First();
                if (!(additionalConditions.ElementCount == 1 &&
                    element.IsLogicalOperator() &&
                    element.IsEmptyBsonArray()))
                {
                    andArray.Value.AsBsonArray.Add(additionalConditions);
                }
            }

            return conditions.ToString();
        }

        private Dictionary<string, object> CreateBasicQueryCondition(QueryParam param)
        {
            var condition = KeyManagement.GetGenerateKey(param, new StringBuilder(), ResourceVersionRepository);

            // データ共有
            var isResourceSharingWith = IsResourceSharingWith(param.XResourceSharingWith, param.ApiResourceSharing, out var sharingFromVendorId, out var sharingFromSystemId);
            if (isResourceSharingWith)
            {
                var queries = new List<BsonDocument>();
                foreach (var rule in param.ApiResourceSharing.ResourceSharingRuleList)
                {
                    if (string.IsNullOrWhiteSpace(rule.Query))
                    {
                        continue;
                    }
                    if (rule.Query.TryToDecimalizedBsonDocument(out var bsonQuery))
                    {
                        if (bsonQuery.ElementCount != 0)
                        {
                            queries.Add(bsonQuery);
                        }
                    }
                    else
                    {
                        throw new QuerySyntaxErrorException("Resource sharing is invalid.");
                    }
                }

                if (queries.Count > 0)
                {
                    var ruleQueries = "{ '$and' : [] }".ToDecimalizedBsonDocument();
                    ruleQueries.GetElement("$and").Value.AsBsonArray.AddRange(queries);
                    condition.Add(_resourceSharingCondition, ruleQueries.AsBsonValue);
                }
            }

            // データ共有(個人)
            var isResourceSharingPerson = IsResourceSharingPerson(param.XResourceSharingPerson, param.ResourceSharingPersonRules, out var sharingFromOpenId);

            // ベンダー依存
            if (param.IsVendor?.Value == true && !(param.IsOverPartition?.Value ?? false))
            {
                if (!condition.ContainsKey(JsonPropertyConst.VENDORID))
                {
                    condition.Add(JsonPropertyConst.VENDORID, isResourceSharingWith ? sharingFromVendorId.Value : param.VendorId.Value);
                }
                if (!condition.ContainsKey(JsonPropertyConst.SYSTEMID))
                {
                    condition.Add(JsonPropertyConst.SYSTEMID, isResourceSharingWith ? sharingFromSystemId.Value : param.SystemId.Value);
                }
            }

            // 個人依存
            if (param.IsPerson?.Value == true && !(param.IsOverPartition?.Value ?? false))
            {
                if (!condition.ContainsKey(JsonPropertyConst.OWNERID))
                {
                    condition.Add(JsonPropertyConst.OWNERID, isResourceSharingPerson ? sharingFromOpenId.Value : param.OpenId.Value);
                }
            }

            // URLパラメータ
            if (param.QueryString?.HasValue == true && (param.QueryType == null || param.QueryType.Value != QueryTypes.ODataQuery))
            {
                var schemas = new List<JSchema>() { param.UriSchema?.ToJSchema(), param.RequestSchema?.ToJSchema(), param.ResponseSchema?.ToJSchema() };
                foreach (var key in param.QueryString.Dic)
                {
                    if (key.Key.Value == JsonPropertyConst.ID)
                    {
                        if (!condition.ContainsKey(key.Key.Value))
                        {
                            condition.Add(key.Key.Value, key.Value.Value);
                        }
                    }
                    else if (!condition.ContainsKey(key.Key.Value))
                    {
                        condition.Add(key.Key.Value, Convert.ChangeType(key.Value.Value, schemas.ToType(key.Key.Value)));
                    }
                }
            }

            if (param.Identification?.Value != null)
            {
                if (!condition.ContainsKey(JsonPropertyConst.ID))
                {
                    condition.Add(JsonPropertyConst.ID, param.Identification.Value);
                }
            }

            return condition;
        }

        private string BuildSelect(QueryParam param)
        {
            var responseSchema = param.ResponseSchema?.ToJSchema();
            if (responseSchema == null || responseSchema.AllowAdditionalProperties)
            {
                return string.Empty;
            }

            // データモデルが設定されている場合、フィールド名を指定するクエリ(Select)を作成
            var select = new Dictionary<string, int>();
            select.Add(JsonPropertyConst.ID, 1);
            select.Add(JsonPropertyConst.OWNERID, 1);
            responseSchema.Properties.ToList().ForEach(x => select.Add(x.Key, 1));

            if (PerRequestDataContainer.XgetInternalAllField == true)
            {
                // 追加で表示する内部フィールド
                select.Add(JsonPropertyConst.TYPE, 1);
                select.Add(JsonPropertyConst.REGDATE, 1);
                select.Add(JsonPropertyConst.OPENID, 1);
                select.Add(JsonPropertyConst.UPDDATE, 1);
                select.Add(JsonPropertyConst.UPDUSERID, 1);
                select.Add(JsonPropertyConst.VERSION_COLNAME, 1);
                select.Add(JsonPropertyConst.PARTITIONKEY, 1);
                select.Add(JsonPropertyConst.VENDORID, 1);
                select.Add(JsonPropertyConst.SYSTEMID, 1);
            }

            return JsonConvert.SerializeObject(select);
        }

        private void EditAndValidateAggregationPipeline(SeparatedQuerySyntax syntax)
        {
            var pipeline = syntax.Aggregate.ToBsonArray();
            if (pipeline.Count <= 0)
            {
                return;
            }

            MergeDefaultConditions(pipeline, syntax.Where);
            syntax.Aggregate = pipeline.ToString();
        }

        private void MergeDefaultConditions(BsonArray pipeline, string queryCondition)
        {
            var firstStage = pipeline.FirstOrDefault().AsBsonDocument;
            if (!firstStage.Contains("$geoNear"))
            {
                // パーティションキー等のデフォルト条件を先頭に挿入する
                // $geoNearの場合は挿入しない($geoNearは先頭のみ可のため)
                pipeline.Insert(0, $"{{ $match: {queryCondition} }}".ToDecimalizedBsonDocument());
            }
            else
            {
                // $geoNearの場合はqueryに条件を追加する
                var geoNear = firstStage["$geoNear"].AsBsonDocument;
                if (geoNear.Contains("query"))
                {
                    var query = geoNear["query"].AsBsonDocument;
                    var newQuery = queryCondition.ToDecimalizedBsonDocument();
                    newQuery["$and"].AsBsonArray.Add(query);
                    geoNear.Set("query", newQuery.AsBsonValue);
                }
                else
                {
                    geoNear.Add("query", queryCondition.ToDecimalizedBsonDocument());
                }
            }

            // ネスト系のステージがあれば再帰的に処理
            // 現状では$unionWithのみ対応
            foreach (var item in pipeline)
            {
                var stage = item.AsBsonDocument;

                if (!stage.Contains("$unionWith"))
                {
                    continue;
                }

                var childPipeline = stage["$unionWith"]["pipeline"].AsBsonArray;
                if (childPipeline.Count > 0)
                {
                    MergeDefaultConditions(childPipeline, queryCondition);
                }
            }
        }


        private BsonDocument ConvertToLogicalAndExpression(IDictionary<string, object> keyValuePairs)
        {
            var conditions = "{ '$and' : [] }".ToDecimalizedBsonDocument();
            var andArray = conditions.GetElement("$and").Value.AsBsonArray;

            keyValuePairs.ToList().ForEach(condition =>
            {
                if (andArray.All(x => !x.AsBsonDocument.Contains(condition.Key)))
                {
                    andArray.Add(new BsonDocument(condition.Key, condition.Value.ToDecimalizedBsonValue()));
                }
            });

            return conditions;
        }


        private IList<BsonDocument> ConvertToAggregatePipeline(QueryParam param, SeparatedQuerySyntax syntax, string mainContainerName, bool isPaging)
        {
            var isCountQuery = syntax.IsCountQuery();
            string pipelineStr = null;

            // ベースとなるステージを追加
            if (string.IsNullOrWhiteSpace(syntax.Aggregate))
            {
                // パイプラインで処理されるためステージの処理順は$match⇒$sort⇒$project⇒$limit
                // 領域越えの場合は$unionWith後に$sort以降のステージを配置する必要があるためここでは$matchのみ追加
                var tmpPipeline = new List<BsonDocument>();
                if (!string.IsNullOrWhiteSpace(syntax.Where))
                {
                    tmpPipeline.Add(new BsonDocument("$match", syntax.Where.ToDecimalizedBsonDocument()));
                }
                pipelineStr = PipelineDefinition<BsonDocument, BsonDocument>.Create(tmpPipeline).ToString();
            }
            else
            {
                pipelineStr = syntax.Aggregate;
            }

            // コレクション名を実際の値に置換
            // 数値データはDecimalで保存されているためクエリの数値もDecimal化
            var pipeline = ReplaceContainerName(pipelineStr, mainContainerName).ToDecimalizedBsonArray().Select(x => x.AsBsonDocument).ToList();

            // セキュリティ観点から実行前にも構文チェックを実施
            if (!GetQuerySyntaxValidatior().ValidatePipelineSyntax(new BsonArray(pipeline), mainContainerName, out var message))
            {
                throw new QuerySyntaxErrorException(message);
            }

            // コンテナ分離の領域越えの場合は$unionWithで複数コンテナを検索するクエリに変更
            if (!isCountQuery &&
                param.IsOverPartition?.Value == true &&
                param.IsContainerDynamicSeparation?.Value == true &&
                (param.IsVendor?.Value == true || param.IsPerson?.Value == true))
            {
                var containerNames = _containerDynamicSeparationRepository.GetAllContainerNames(RepositoryInfo.PhysicalRepositoryId, param.ControllerId);
                foreach (var containerName in containerNames.Where(x => x != mainContainerName))
                {
                    var elements = new List<BsonElement>()
                    {
                        new BsonElement("coll", containerName),
                        new BsonElement("pipeline", ReplaceContainerName(pipelineStr, containerName).ToDecimalizedBsonArray())
                    };
                    pipeline.Add(new BsonDocument("$unionWith", new BsonDocument(elements)));
                }
            }

            // 後続ステージを追加
            if (isCountQuery)
            {
                pipeline.Add(new BsonDocument("$count", "count"));
            }
            else if (string.IsNullOrWhiteSpace(syntax.Aggregate) && !isCountQuery)
            {
                // パイプラインで処理されるためステージの処理順は$match⇒$sort⇒$project⇒$limit
                // 領域越えの場合は$unionWith後に$sort以降のステージを配置する必要があるためここで$sort以降を追加
                if (!string.IsNullOrWhiteSpace(syntax.OrderBy))
                {
                    pipeline.Add(new BsonDocument("$sort", syntax.OrderBy.ToDecimalizedBsonDocument()));
                }
                if (!string.IsNullOrWhiteSpace(syntax.Select))
                {
                    pipeline.Add(new BsonDocument("$project", syntax.Select.ToDecimalizedBsonDocument()));
                }
                if (syntax.Top.HasValue && !isPaging)
                {
                    pipeline.Add(new BsonDocument("$limit", syntax.Top.Value));
                }
            }

            return pipeline;
        }

        private string ReplaceContainerName(string pipelineStr, string containerName)
        {
            if (pipelineStr.Contains($"\"{MongoDbConstants.CollectionNameVariable}\""))
            {
                pipelineStr = pipelineStr.Replace($"\"{MongoDbConstants.CollectionNameVariable}\"", $"\"{containerName}\"");
            }
            return pipelineStr;
        }


        private bool IsResourceSharingWith(XResourceSharingWith xResourceSharingWith, ApiResourceSharing apiResourceSharing, out VendorId sharingFromVendorId, out SystemId sharingFromSystemId)
        {
            var isResourceSharing = xResourceSharingWith?.Value != null && (apiResourceSharing?.ResourceSharingRuleList?.Any() ?? false);
            sharingFromVendorId = isResourceSharing ? new VendorId(xResourceSharingWith["VendorId"]) : null;
            sharingFromSystemId = isResourceSharing ? new SystemId(xResourceSharingWith["SystemId"]) : null;

            return isResourceSharing;
        }

        private bool IsResourceSharingPerson(XResourceSharingPerson xResourceSharingPerson, List<ResourceSharingPersonRule> resourceSharingPersonRules, out OpenId sharingFromOpenId)
        {
            var isResourceSharingPerson = !string.IsNullOrEmpty(xResourceSharingPerson?.Value) && (resourceSharingPersonRules?.Any() ?? false);
            sharingFromOpenId = isResourceSharingPerson ? new OpenId(xResourceSharingPerson.Value) : null;

            return isResourceSharingPerson;
        }

        private MongoDbQuerySyntaxValidatior GetQuerySyntaxValidatior()
        {
            return (MongoDbQuerySyntaxValidatior)UnityCore.Resolve<IQuerySyntaxValidator>(RepositoryType.MomgoDB.ToCode());
        }
    }
}
