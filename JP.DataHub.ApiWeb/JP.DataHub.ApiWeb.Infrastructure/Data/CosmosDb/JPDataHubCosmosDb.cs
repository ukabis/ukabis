using System.Text;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.OData.CosmosDb.ODataToSqlTranslator;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.CosmosDb
{
    internal class JPDataHubCosmosDb : IJPDataHubCosmosDb, IDisposable
    {
        private static object s_lockobj = new object();
        private static Lazy<IConfiguration> s_lasyConfiguration = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        protected static IConfiguration Configuration { get => s_lasyConfiguration.Value; }

        private CosmosClient _client;
        private Container _container;
        private string _connectionString;

        private bool _outputSqlLog = false;
        private double _minLogRu = 10;
        private double _alertRu = 15;
        private int _feedOptionsMaxDegreeOfParallelism = 0;

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                lock (s_lockobj)
                {

                    // 設定してある接続文字列と違う場合再接続する。
                    if (_connectionString != value)
                    {
                        _connectionString = value;
                        InitConnection();
                    }
                }
            }
        }

        private JPDataHubLogger _logger = new JPDataHubLogger(typeof(JPDataHubCosmosDb));


        public JPDataHubCosmosDb()
        {
            _outputSqlLog = Configuration.GetValue<bool>("AppConfig:OutPutSqlLog", false);
            _minLogRu = Configuration.GetValue<double>("CosmosDB:MinimumLogRu", 10d);
            _alertRu = Configuration.GetValue<double>("CosmosDB:AlertRu", 15d);
            _feedOptionsMaxDegreeOfParallelism = Configuration.GetValue<int>("CosmosDB:FeedOptionsMaxDegreeOfParallelism", 0);
        }

        private void InitConnection()
        {
            const string PCSKEY_ACCOUNTENDPOINT = "AccountEndpoint";
            const string PCSKEY_ACCOUNTKEY = "AccountKey";
            const string PCSKEY_CONNECTIONTIMEOUT = "Connection Timeout";
            const string PCSKEY_DATABASEID = "DatabaseId";
            const string PCSKEY_COLLECTIONID = "CollectionId";

            DisposeDocumentClient();
            var pcs = new ParseConnectionString(ConnectionString);
            _client = new CosmosClient(new Uri(pcs[PCSKEY_ACCOUNTENDPOINT]).ToString(), pcs[PCSKEY_ACCOUNTKEY], GetConnectionPolicy(pcs[PCSKEY_CONNECTIONTIMEOUT]));
            _container = _client.GetContainer(pcs[PCSKEY_DATABASEID], pcs[PCSKEY_COLLECTIONID]);

            //未処理の例外が発生した場合の対策
            OpenClient().ContinueWith(task =>
            {
                _logger.Error(task.Exception.Message);
                foreach (var ex in task.Exception.InnerExceptions)
                {
                    _logger.Error($">>{ex.Message}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private async Task OpenClient()
        {
            //v3 sdkではOpenClient()がなくなり 初回のDB操作で初めて接続するようになっている
            //以下issueの指摘に従い 存在しないデータを読み込もうとすることで接続操作の代替とする
            //https://github.com/Azure/azure-cosmos-dotnet-v3/issues/1436#issuecomment-619953780
            using (await _container.ReadItemStreamAsync("DoesNotExist", new PartitionKey("DoesNotExist"))) { };
        }

        private static TimeSpan ParseTimespan(string tpRequestTimeout)
        {
            if (int.TryParse(tpRequestTimeout, out int sec) == false)
            {
                sec = 60;
            }
            TimeSpan result = TimeSpan.FromSeconds(sec);
            return result;
        }

        private CosmosClientOptions GetConnectionPolicy(string requestTimeout)
        {
            var policy = new CosmosClientOptions();
            policy.ConnectionMode = Enum.TryParse(Configuration.GetValue<string>("CosmosDB:CosmosDBConnectionMode", "Direct"), out ConnectionMode mode) ? mode : ConnectionMode.Direct;
            policy.OpenTcpConnectionTimeout = TimeSpan.FromSeconds(Configuration.GetValue("CosmosDB:OpenTcpConnectionTimeout", 5));
            policy.MaxRequestsPerTcpConnection = Configuration.GetValue("CosmosDB:MaxRequestsPerTcpConnection", 30);
            policy.RequestTimeout = ParseTimespan(requestTimeout);

            return policy;
        }

        public string UpsertDocument(JToken token, ItemRequestOptions opt)
        {
            var itemResponse = RuLargeRetryHelper(() => _container.UpsertItemAsync(token, requestOptions: opt).Result);
            WriteLog(itemResponse, () => $"UpsertDocument: {token.ToString().Replace("\r\n", "")}");
            return itemResponse.Resource["id"].ToString();
        }

        /// <summary>
        /// CosmosDBからデータを取得する
        /// </summary>
        /// <param name="query">クエリ</param>
        /// <param name="conditions">SQLパラメータ</param>
        /// <param name="isOverPartition">領域越えする</param>
        /// <returns></returns>
        public IEnumerable<JToken> QueryDocument(string query, IDictionary<string, object> conditions, bool isOverPartition)
        {
            var sqlQueryDefinition = GenerateQueryDefinition(query, CreateParameter(conditions));
            var queryRequestOptions = CreateQueryRequestOptions(conditions, isOverPartition);

            var queryStringExpand = new StringBuilder();

            if (_outputSqlLog)
            {
                queryStringExpand.Append($"Query={query.Replace("\r\n", " ")} params = {string.Join(",", sqlQueryDefinition.GetQueryParameters().Select(x => $"{x.Name}={x.Value}").ToArray())}");
                _logger.Debug(queryStringExpand.ToString());
            }

            using (var feedIterator = RuLargeRetryHelper(() => _container.GetItemQueryIterator<JToken>(sqlQueryDefinition, requestOptions: queryRequestOptions)))
            {
                var metrics = new DocumentQueryMetrics();
                while (feedIterator.HasMoreResults)
                {
                    var feedResponse = RuLargeRetryHelper(() => feedIterator.ReadNextAsync().Result);
                    foreach (var token in feedResponse.Select(x => x))
                    {
                        yield return token;
                    }

                    // 性能情報を集計
                    metrics.AddMetrics(feedResponse);
                }

                // 性能情報をログ出力
                WriteQueryLog(metrics, queryStringExpand.ToString());
            }
            yield break;
        }

        private static QueryDefinition GenerateQueryDefinition(string query, IDictionary<string, object> conditions)
        {
            var definition = new QueryDefinition(query);
            if (conditions?.Any() == true)
            {
                conditions.ToList().ForEach(x => definition.WithParameter(x.Key, x.Value));
            }

            return definition;
        }

        public List<JToken> QueryDocumentContinuation(string query, IDictionary<string, object> conditions, bool isOverPartition, int maxItemCount, string requestContinuation, out string responseContinuation)
        {
            var resultList = new List<JToken>();
            responseContinuation = null;

            var sqlQueryDefinition = GenerateQueryDefinition(query, CreateParameter(conditions));
            var queryStringExpand = new StringBuilder();

            if (_outputSqlLog)
            {
                queryStringExpand.Append($"Query={query.Replace("\r\n", " ")} params = {string.Join(",", sqlQueryDefinition.GetQueryParameters().Select(x => $"{x.Name}={x.Value}").ToArray())}");
                _logger.Debug(queryStringExpand.ToString());
            }

            var queryRequestOptions = CreateQueryRequestOptions(conditions, isOverPartition);
            queryRequestOptions.MaxItemCount = maxItemCount;

            using (var feedIterator = RuLargeRetryHelper(() => _container.GetItemQueryIterator<JToken>(sqlQueryDefinition, requestContinuation, queryRequestOptions)))
            {
                long ItemCount = 0;
                var metrics = new DocumentQueryMetrics();
                while (feedIterator.HasMoreResults && ItemCount < maxItemCount)
                {
                    var feedResponse = RuLargeRetryHelper(() => feedIterator.ReadNextAsync().Result);
                    responseContinuation = feedResponse.ContinuationToken;

                    resultList.AddRange(feedResponse.Select(x => x));

                    ItemCount += feedResponse.Count;
                    // 性能情報を集計
                    metrics.AddMetrics(feedResponse);
                }

                // 性能情報をログ出力
                WriteQueryLog(metrics, queryStringExpand.ToString());
            }

            return resultList;
        }

        /// <summary>
        /// DocumentDBからデータを削除する
        /// </summary>
        /// <param name="query">クエリ</param>
        /// <param name="conditions">SQLパラメータ</param>
        /// <param name="isOverPartition">領域越えする</param>
        /// <returns></returns>
        public IEnumerable<string> DeleteDocument(string query, IDictionary<string, object> conditions, bool isOverPartition)
        {
            var sqlQueryDefinition = GenerateQueryDefinition(query, CreateParameter(conditions));

            var requestOptions = new QueryRequestOptions();
            bool condition_partition = false;
            if (conditions.ContainsKey("_partitionkey"))
            {
                requestOptions.PartitionKey = new PartitionKey(conditions["_partitionkey"].ToString());
                condition_partition = true;
            }

            var deletedIds = new List<string>();
            foreach (var doc in QueryDocument(query, conditions, isOverPartition))
            {
                if (!condition_partition)
                {
                    requestOptions.PartitionKey = new PartitionKey(doc["_partitionkey"].ToString());
                }

                try
                {
                    var x = RuLargeRetryHelper(() => _container.DeleteItemAsync<JToken>(doc["id"].ToString(), requestOptions.PartitionKey.Value).Result);
                    deletedIds.Add(doc["id"].ToString());
                    WriteLog(x, () => $"DeleteDocument: {doc.ToString().Replace("\r\n", "")}");
                }
                catch (AggregateException ex)
                {
                    foreach (var ie in ex.InnerExceptions)
                    {
                        if (ie is CosmosException)
                        {
                            if (((CosmosException)ie).StatusCode != System.Net.HttpStatusCode.NotFound)
                            {
                                // NotFound以外はそのままExceptionを投げる
                                throw;
                            }
                        }
                        else
                        {
                            // CosmosException以外はそのままthrowする
                            throw;
                        }
                    }
                }
            }

            return deletedIds.Any() ? deletedIds : null;
        }

        /// <summary>
        /// JToken 指定でのDocumentDelete
        /// </summary>
        /// <param name="deltarget"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public bool DeleteDocument(JToken deltarget, JToken partitionKey)
        {
            try
            {
                var requestOptions = new QueryRequestOptions();
                requestOptions.PartitionKey = new PartitionKey(partitionKey.ToString());

                var itemResponse = RuLargeRetryHelper(() => _container.DeleteItemAsync<JToken>(deltarget.ToString(), requestOptions.PartitionKey.Value).Result);
                if (itemResponse != null) WriteLog(itemResponse, () => $"DeleteDocument: {deltarget.ToString().Replace("\r\n", "")}");
            }
            catch (AggregateException ex)
            {
                foreach (var ie in ex.InnerExceptions)
                {
                    if (ie is CosmosException)
                    {
                        if (((CosmosException)ie).StatusCode != System.Net.HttpStatusCode.NotFound)
                        {
                            // NotFound以外はそのままExceptionを投げる
                            throw;
                        }
                    }
                    else
                    {
                        // CosmosException以外はそのままthrowする
                        throw;
                    }
                }
            }

            return true;
        }

        private IDictionary<string, object> CreateParameter(IDictionary<string, object> conditions)
        {
            var param = new Dictionary<string, object>();
            if (conditions == null)
            {
                return param;
            }
            foreach (var condition in conditions)
            {
                // 予約語の場合はエスケープする
                if (Constants.SelectEscapeKeywords.Contains(condition.Key.ToUpper()))
                {
                    param.Add($"@_{condition.Key}", condition.Value);
                }
                else
                {
                    param.Add($"@{condition.Key}", condition.Value);
                }
            }
            return param;
        }

        private T RuLargeRetryHelper<T>(Func<T> execute)
        {
            bool finish = false;
            TimeSpan delay = TimeSpan.MinValue;
            T result = default(T);
            while (!finish)
            {
                if (delay != TimeSpan.MinValue)
                {
                    var sleepTask = Task.Delay(delay);
                    sleepTask.Wait();
                }
                try
                {
                    result = execute.Invoke();
                    finish = true;
                }
                catch (AggregateException ex)
                {
                    if (ex.InnerExceptions.Count == 1 && ex.InnerExceptions[0] is CosmosException)
                    {
                        var innerEx = ex.InnerExceptions[0] as CosmosException;
                        _logger?.Error("CosmosDB Error", innerEx);
                        if ((int)innerEx.StatusCode == 429)
                        {
                            delay = innerEx.RetryAfter.Value;
                            continue;
                        }
                        if (innerEx.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            string errorMessage = string.Empty;

                            /*
                             * HACK 環境ごとにcosmosクライアントが返却するメッセージが違う
                               local環境でのcosmosdb v3のエラーメッセージの書式は以下のような感じ
                               Reason の後にあるMessageを取得して エラーメッセージを返却する
                               Response status code does not indicate success: BadRequest (400); Substatus: 0; ActivityId: 0bdbbadf-1425-4425-a07e-5e7070399bf5; Reason: (Response status code does not indicate success: BadRequest (400); Substatus: 0; ActivityId: 0bdbbadf-1425-4425-a07e-5e7070399bf5; Reason: (Response status code does not indicate success: BadRequest (400); Substatus: 0; ActivityId: 0bdbbadf-1425-4425-a07e-5e7070399bf5; Reason: (Message: {"errors":[{"severity":"Error","location":{"start":24,"end":32},"code":"SC2001","message":"Identifier 'hogehoge' could not be resolved."}]}
                               ActivityId: 0bdbbadf-1425-4425-a07e-5e7070399bf5, Request URI: /apps/be83c8b6-69d9-4737-b7d1-e6cd7df307ca/services/05c3c6e7-2842-4fc9-8294-4c5442028874/partitions/9debd5b4-d843-4b09-8f03-4669003a2cee/replicas/132751331856894470s/, RequestStats: Microsoft.Azure.Cosmos.Tracing.TraceData.ClientSideRequestStatisticsTraceDatum, SDK: Windows/10.0.19043 cosmos-netstandard-sdk/3.19.3);););

                               stg-wgrd環境でのメッセージの書式は以下のような感じ
                               --->でExceptionの後にあるMessageを取得して エラーメッセージを返却する
                               Response status code does not indicate success: BadRequest (400); Substatus: 0; ActivityId: ; Reason: (Microsoft.Azure.Cosmos.Query.Core.Monads.ExceptionWithStackTraceException: TryCatch resulted in an exception. ---> Microsoft.Azure.Cosmos.Query.Core.Monads.ExceptionWithStackTraceException: TryCatch resulted in an exception. ---> Microsoft.Azure.Cosmos.Query.Core.Exceptions.ExpectedQueryPartitionProviderException: {\"errors\":[{\"severity\":\"Error\",\"location\":{\"start\":24,\"end\":32},\"code\":\"SC2001\",\"message\":\"Identifier 'hogehoge' could not be resolved.\"}]} ---> System.Runtime.InteropServices.COMException: Exception from HRESULT: 0x800A0B00\r\n   --- End of inner exception stack trace ---\r\n   --- End of inner exception stack trace ---\r\n
                             */

                            if (string.IsNullOrEmpty(innerEx.Message) == false)
                            {
                                if (innerEx.Message.Contains(" ---> "))
                                {
                                    errorMessage = innerEx.Message
                                                .Split(new string[] { " ---> " }, StringSplitOptions.None)?
                                                .ToList().FirstOrDefault(x => x.Contains("errors"))?
                                                .Split(new string[] { "Exception:" }, StringSplitOptions.None)[1];
                                    errorMessage = "Message:" + errorMessage;

                                }
                                else
                                {
                                    errorMessage = innerEx.Message.Split(new string[] { "Reason: (" }, StringSplitOptions.None)?
                                                .ToList()?
                                                .FirstOrDefault(x => x.StartsWith("Message:"))?
                                                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];

                                }
                            }

                            throw new QuerySyntaxErrorException(errorMessage);
                        }

                        if (innerEx.StatusCode == System.Net.HttpStatusCode.PreconditionFailed)
                        {
                            throw new ConflictException("");
                        }
                    }
                    throw;
                }
            }
            return result;
        }

        private QueryRequestOptions CreateQueryRequestOptions(IDictionary<string, object> conditions, bool isOverPartition)
        {
            var queryRequestOptions = new QueryRequestOptions();
            if (conditions.ContainsKey("_partitionkey") && !isOverPartition)
            {
                queryRequestOptions.PartitionKey = new PartitionKey(conditions["_partitionkey"].ToString());
            }
            if (isOverPartition)
            {
                //v3 ではパーテイションキーをつけなければ勝手にクロスパーティションクエリになる
                queryRequestOptions.PartitionKey = null;
            }
            queryRequestOptions.MaxConcurrency = _feedOptionsMaxDegreeOfParallelism;
            return queryRequestOptions;
        }

        /// <summary>
        /// パフォーマンスログを出力します。
        /// </summary>
        /// <param name="response">ドキュメント操作のレスポンス</param>
        /// <param name="func">メッセージ生成Func</param>
        private void WriteLog<T>(ItemResponse<T> response, Func<string> func)
        {
            if (_outputSqlLog || response.RequestCharge > _minLogRu)
            {
                string message = $"{func()}, RU: {response.RequestCharge}";

                if (response.RequestCharge > _alertRu)
                {
                    _logger.Warn(message);
                }
                else
                {
                    _logger.Info(message);
                }
            }
        }

        /// <summary>
        /// Queryのパフォーマンスログを出力します。
        /// </summary>
        /// <param name="metrics">DocumentQueryMetrics</param>
        /// <param name="documentQuery">DocumentQuery</param>
        private void WriteQueryLog(DocumentQueryMetrics metrics, string documentQuery)
        {
            if (metrics.RequestCharge > _minLogRu || _outputSqlLog)
            {
                string message = $"QueryDocument: {documentQuery}, RU: {metrics.RequestCharge}, RetrievedDocumentCount: {metrics.RetrievedDocumentCount:#,0}, TotalExecutionTime: {metrics.TotalExecutionTime:hh\\:mm\\:ss\\.fff}";

                if (metrics.RequestCharge > _alertRu)
                {
                    _logger.Warn(message);
                }
                else
                {
                    _logger.Info(message);
                }
            }
        }

        /// <summary>
        /// Query実行時の性能情報
        /// </summary>
        private struct DocumentQueryMetrics
        {
            public double RequestCharge;
            public long RetrievedDocumentCount;
            public TimeSpan TotalExecutionTime;
            private JPDataHubLogger logger;
            /// <summary>
            /// Metricsを追加します。
            /// </summary>
            /// <param name="feedResponse">FeedResponse</param>
            public void AddMetrics(FeedResponse<JToken> feedResponse)
            {
                try
                {
                    RequestCharge += feedResponse.RequestCharge;
                    RetrievedDocumentCount += feedResponse.Count;
                    TotalExecutionTime += feedResponse.Diagnostics.GetClientElapsedTime();
                }
                catch (Exception ex)
                {
                    if (logger == null)
                        logger = new JPDataHubLogger(typeof(JPDataHubCosmosDb));

                    logger.Error($"AddMetrics error : {ex}");
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeDocumentClient();

                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void DisposeDocumentClient()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }
        #endregion
    }
}
