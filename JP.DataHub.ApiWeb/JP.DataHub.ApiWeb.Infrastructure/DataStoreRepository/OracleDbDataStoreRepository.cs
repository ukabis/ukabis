using System.Data.SqlClient;
using System.Text;
using Unity;
using Unity.Injection;
using Unity.Resolution;
using Polly;
using Polly.Retry;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Extensions.OracleDb;
using JP.DataHub.ApiWeb.Infrastructure.Sql;
using JP.DataHub.Infrastructure.Database.Consts;
using JP.DataHub.Infrastructure.Database.Data;
using JP.DataHub.Infrastructure.Database.Data.SqlServer;
using JP.DataHub.Infrastructure.Database.Data.OracleDb;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    // .NET6
    /// <summary>
    /// OracleDbを使用してデータを永続化するためのリポジトリです。
    /// </summary>
    internal class OracleDbDataStoreRepository : NewAbstractDynamicApiDataStoreRepository, INewDynamicApiDataStoreRdbmsRepository
    {
        private static readonly JPDataHubLogger s_log = new JPDataHubLogger(typeof(OracleDbDataStoreRepository));

        private Lazy<IConfigurationSection> _lasyConfiguration = new Lazy<IConfigurationSection>(() => UnityCore.Resolve<IConfiguration>().GetSection("OracleDbDataStoreRepository"));
        protected IConfigurationSection Configuration { get => _lasyConfiguration.Value; }

        public override bool CanQuery { get => true; }
        public override bool CanOptimisticConcurrency { get => true; }
        public override string VersionInfoQuery { get => $"SELECT \"{OracleDbVersionInfo.VersionInfoColumnName}\", \"{JsonPropertyConst.ETAG}\" FROM \"{OracleDbVersionInfo.VersionTableName}\" WHERE \"id\" = {ToSqlParameter("id")}"; }
        public override string DocumentVersionQuery { get => $"SELECT * FROM \"{OracleDbDocumentVersion.DocumentVersionTableName}\" WHERE \"id\" = {ToSqlParameter("id")}"; }
        public override string RepositoryName => "Oracle";
        public override IEnumerable<string> AttachFileMetaManagementFields => new string[]
        {
            JsonPropertyConst.TYPE,
            JsonPropertyConst.VENDORID,
            JsonPropertyConst.SYSTEMID,
            JsonPropertyConst.OPENID,
            JsonPropertyConst.REGDATE,
            JsonPropertyConst.UPDUSERID,
            JsonPropertyConst.UPDDATE,
            JsonPropertyConst.OWNERID,
            JsonPropertyConst.VERSION_COLNAME
        };


        /// <summary>
        /// マルチバイト文字列の最大文字数
        /// </summary>
        public int? NCharTableColumnMaxLength => OracleDbConsts.NCharTableColumnMaxLength;

        /// <summary>
        /// テーブル列名の最大バイト数
        /// </summary>
        public int? TableColumnNameMaxBytes => OracleDbConsts.TableColumnNameMaxLength;

        /// <summary>
        /// テーブル最大列数
        /// </summary>
        public int? TableMaxColumns => OracleDbConsts.TableMaxColumns;

        /// <summary>
        /// テーブル列名パターン
        /// </summary>
        public string TableColumnNamePattern => OracleDbConsts.TableColumnNamePattern;


        [Dependency]
        public IContainerDynamicSeparationRepository ContainerDynamicSeparationRepository { get; set; }
        [Dependency]
        public IDynamicApiRepository DynamicApiRepository { get; set; }

        /// <summary>
        /// OracleDbへの操作リトライ回数
        /// </summary>
        private int RetryCount { get; } = 5;
        /// <summary>
        /// OracleDbへの操作リトライ秒数(type:double)。指数関数的バックオフで秒を指定しているので秒数=待ち時間ではない
        /// </summary>
        private double RetryPowerBaseSec { get; } = 2;

        private readonly RetryPolicy _retryPolicy;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public OracleDbDataStoreRepository()
        {
            RetryCount = Configuration.GetValue<int>("RetryCount");

            RetryPowerBaseSec = Configuration.GetValue<double>("RetryPowerBaseSecond");

            _retryPolicy = Policy
                .Handle<Exception>(OracleDbTransientExceptionDetector.ShouldRetryOn)
                .WaitAndRetry(
                    RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(RetryPowerBaseSec, retryAttempt)),
                    (ex, timespan, retryCount, context) =>
                        s_log.Warn($"Exception Occurred. Error: {ex.Message},  retryCount: {retryCount}", ex)
                );
        }


        #region Connection

        /// <summary>
        /// OracleDbへのコネクションを取得する。同時に接続のオープンも行う。
        /// </summary>
        private IJPDataHubRdbms GetAndOpenConnection()
        {
            IJPDataHubRdbms client;
            client = UnityCore.Resolve<IJPDataHubRdbms>(RepositoryName);
            client.ConnectionString = RepositoryInfo.ConnectionString;
            return client;
        }

        #endregion

        #region Query

        /// <summary>
        /// クエリを実行する。(1件取得)
        /// </summary>
        [DataStoreRepositoryParamODataConvert]
        public override JsonDocument QueryOnce(QueryParam param)
        {
            var doc = Query(param.ToSingle()).FirstOrDefault();

            JsonDocument json;
            if (doc != null && param.OperationInfo?.IsVersionOperation == true)
            {
                // バージョン情報の場合はversioninfo列のJSONを返す(ETAGは個別項目より補完)
                var versionInfo = doc.Item1[OracleDbVersionInfo.VersionInfoColumnName];
                versionInfo[JsonPropertyConst.ETAG] = doc.Item1[JsonPropertyConst.ETAG].Value<string>();
                json = new JsonDocument(versionInfo);
            }
            else
            {
                json = (doc == null ? null : new JsonDocument(doc.Item1));
            }

            return json;
        }

        /// <summary>
        /// 検索パラメータで指定されたSQL文とクエリパラメータで検索を行います。
        /// </summary>
        [DataStoreRepositoryParamODataConvert]
        public override IEnumerable<JsonDocument> QueryEnumerable(QueryParam param)
        {
            foreach (var x in Query(param.ToSingle()).ToList())
            {
                yield return new JsonDocument(x.Item1);
            }
        }

        /// <summary>
        /// クエリを実行する。(Listで取得)
        /// </summary>
        [DataStoreRepositoryParamODataConvert]
        public override IList<JsonDocument> Query(QueryParam param, out XResponseContinuation xResponseContinuation)
        {
            List<JsonDocument> result = new List<JsonDocument>();
            var queryResult = Query(param.ToSingle()).ToList();
            queryResult.ForEach(x => result.Add(new JsonDocument(x.Item1)));
            xResponseContinuation = queryResult.FirstOrDefault()?.Item2;
            return result;
        }


        /// <summary>
        /// クエリを実行する。
        /// </summary>
        private IEnumerable<Tuple<JToken, XResponseContinuation>> Query(QueryParam param)
        {
            var sqlBuilder = CreateQuerySqlBuilder(param);
            sqlBuilder.BuildUp();

            // クエリ発行
            if (sqlBuilder.IsNativeCountQuery)
            {
                yield return new Tuple<JToken, XResponseContinuation>(CountQuery(sqlBuilder.Sql, sqlBuilder.SqlParameterList, sqlBuilder.IsCustomSql), null);
            }
            else
            {
                XResponseContinuation responseContinuation = null;
                if (sqlBuilder.PagingInfo.IsPaging)
                {
                    var readCount = sqlBuilder.PagingInfo.ReadCount;
                    var remains = readCount < CountQuery(sqlBuilder.CountSql, sqlBuilder.SqlParameterList, sqlBuilder.IsCustomSql);

                    responseContinuation = new XResponseContinuation(remains ? readCount.ToString() : string.Empty);
                }

                IDictionary<string, JSchema> propertySchemaList = null;
                IList<string> required = null;
                switch (param.OperationInfo?.Type)
                {
                    case OperationInfo.OperationType.VersionInfo:
                        propertySchemaList = OracleDbVersionInfo.VersionTableSchema.Properties;
                        break;
                    case OperationInfo.OperationType.DocumentVersion:
                        propertySchemaList = OracleDbDocumentVersion.DocumentVersionTableSchema.Properties;
                        break;
                    default:
                        var schema = (param.ResponseSchema?.ToJSchema() ?? param.ControllerSchema?.ToJSchema());
                        propertySchemaList = schema?.Properties;
                        propertySchemaList?.Remove(JsonPropertyConst.ETAG);
                        required = schema?.Required;
                        break;
                }

                foreach (var json in ExecuteQuery(sqlBuilder.Sql, sqlBuilder.SqlParameterList, sqlBuilder.IsCustomSql))
                {
                    // DB格納用に変換されているデータをスキーマに基づいて元の形式に復元
                    if (propertySchemaList != null)
                    {
                        var removeList = new List<string>();
                        foreach (JProperty property in json.Children().Where(x => x is JProperty))
                        {
                            propertySchemaList.TryGetValue(property.Name, out var propertySchema);
                            if (propertySchema == null)
                            {
                                // _etagは内部的には数値だが文字列として返す
                                if (property.Name == JsonPropertyConst.ETAG)
                                {
                                    json[property.Name] = property.Value.Value<string>();
                                }
                                continue;
                            }

                            // 配列、オブジェクトをJSONに展開
                            if (!propertySchema.Type.IsFlatType())
                            {
                                var value = property.Value.Value<string>();
                                json[property.Name] = value == null ? JValue.CreateNull() : JToken.Parse(value);
                            }

                            // 任意項目かつnull不許可項目の値がnullであれば項目削除
                            // nullのまま返してしまうとnull不許可項目がnullの不正なデータとなり、
                            // UPDATEの場合はバリデーションエラーになる場合があるため
                            if (!(param.OperationInfo?.IsVersionOperation ?? false) &&
                                !(required?.Contains(property.Name) ?? false) &&
                                (propertySchema.Type & JSchemaType.Null) != JSchemaType.Null &&
                                property.Value.Type == JTokenType.Null)
                            {
                                removeList.Add(property.Name);
                            }
                        }

                        removeList.ForEach(x => json.RemoveField(x));
                    }

                    yield return new Tuple<JToken, XResponseContinuation>(json, responseContinuation);
                }
            }
        }

        /// <summary>
        /// SQLクエリを発行する。
        /// </summary>
        private IEnumerable<JToken> ExecuteQuery(string sql, IRdbmsSqlParameterList sqlParameters, bool isCustomQuery = false)
        {
            IEnumerable<JToken> result;
            try
            {
                result = _retryPolicy
                    .Execute(() =>
                    {
                        using (var connection = GetAndOpenConnection())
                        {
                            return connection.QueryDocument(sql, sqlParameters.AsParameterObject).ToList();
                        }
                    }
                    );
            }
            // 【暫定実装】
            // APIクエリやODataで存在しない項目が指定された場合はSQLの構文エラーとなる。
            // SQLを解析してクエリを実行せずにハンドリングすることが望ましいが、
            // SQLの完全な解析は難しいため一旦はSqlServerのエラーコード(207: 列名が無効です)でハンドリングする。
            // NoSQL系リポジトリでWhereに存在しない項目が指定された場合の動作に合せて通常はNotFoundを返す。
            // ただし、APIクエリに起因するエラーの場合は作成者が構文エラーを認識できるようBadRequestを返す。
            // 不具合によるSQLの不備などの可能性もあるためWarningでログ出力する。
            catch (SqlException ex) when (ex.Number == 207 && isCustomQuery)
            {
                s_log.Warn($"OracleDb SQL Error: {ex.Message}, SQL: {sql}", ex);
                throw new QuerySyntaxErrorException(ex.Message);
            }
            catch (SqlException ex) when (ex.Number == 207)
            {
                s_log.Warn($"OracleDb SQL Error: {ex.Message}, SQL: {sql}", ex);
                yield break;
            }
            catch (SqlException ex)
            {
                s_log.Warn($"OracleDb SQL Error: {ex.Message}, SQL: {sql}", ex);
                throw;
            }

            foreach (var ret in result)
            {
                yield return ret;
            }
            yield break;
        }


        /// <summary>
        /// 指定クエリのレコード件数を取得する。
        /// </summary>
        private long CountQuery(string sql, IRdbmsSqlParameterList sqlParameters, bool isCustomQuery = false)
        {
            try
            {
                var result = _retryPolicy
                    .Execute(() =>
                    {
                        using (var connection = GetAndOpenConnection())
                        {
                            return (connection.QueryDocument(sql, sqlParameters.AsParameterObject).First().Children().First() as JProperty).Value.Value<long>();
                        }
                    }
                    );
                return result;
            }
            // 【暫定実装】
            // APIクエリやODataで存在しない項目が指定された場合はSQLの構文エラーとなる。
            // SQLを解析してクエリを実行せずにハンドリングすることが望ましいが、
            // SQLの完全な解析は難しいため一旦はSqlServerのエラーコード(207: 列名が無効です)でハンドリングする。
            // NoSQL系リポジトリでWhereに存在しない項目が指定された場合の動作に合せて通常はNotFoundを返す。
            // ただし、APIクエリに起因するエラーの場合は作成者が構文エラーを認識できるようBadRequestを返す。
            // 不具合によるSQLの不備などの可能性もあるためWarningでログ出力する。
            catch (SqlException ex) when (ex.Number == 207 && isCustomQuery)
            {
                s_log.Warn($"OracleDb SQL Error: {ex.Message}, SQL: {sql}", ex);
                throw new QuerySyntaxErrorException(ex.Message);
            }
            catch (SqlException ex) when (ex.Number == 207)
            {
                s_log.Warn($"OracleDb SQL Error: {ex.Message}, SQL: {sql}", ex);
                return 0;
            }
            catch (SqlException ex)
            {
                s_log.Warn($"OracleDb SQL Error: {ex.Message}, SQL: {sql}", ex);
                throw;
            }
        }

        #endregion

        #region Register

        /// <summary>
        /// 1件のデータをOracleDbに登録する。
        /// </summary>
        public override RegisterOnceResult RegisterOnce(RegisterParam param)
        {
            // 管理項目を調整
            FormatManagementData(param.Json, param);

            var sqlBuilder = CreateUpsertSqlBuilder(param);
            sqlBuilder.BuildUp();

            // SQL実行
            var result = _retryPolicy
                .Execute(() =>
                {
                    using (var connection = GetAndOpenConnection())
                    {
                        var count = connection.UpsertDocument(sqlBuilder.Sql, sqlBuilder.SqlParameterList.AsParameterObject);
                        if (count <= 0)
                        {
                            throw new ConflictException("");
                        }

                        // 追加のSQLがあれば続けて実行する
                        if (sqlBuilder.HasPostAdditionalSqls)
                        {
                            foreach (var additionalSql in sqlBuilder.PostAdditionalSqls)
                            {
                                connection.UpsertDocument(additionalSql.Sql, additionalSql.SqlParameterList.AsParameterObject);
                            }
                        }

                        return new RegisterOnceResult(param.Json[JsonPropertyConst.ID].Value<string>(), null);
                    }
                });
            return result;
        }

        /// <summary>
        /// データの管理用フィールドを追加・削除する。
        /// </summary>
        private void FormatManagementData(JToken json, RegisterParam param)
        {
            var isAny = json.Any();

            // _Type/_partitionkeyがあれば削除(CosmosDBがプライマリの場合など)
            if (isAny && json.IsExistProperty(JsonPropertyConst.TYPE))
            {
                json.RemoveField(JsonPropertyConst.TYPE);
            }
            if (isAny && json.IsExistProperty(JsonPropertyConst.PARTITIONKEY))
            {
                json.RemoveField(JsonPropertyConst.PARTITIONKEY);
            }

            if (param.IsOverrideId?.Value == true)
            {
                if (isAny && json.IsExistProperty(JsonPropertyConst.VERSION_COLNAME))
                {
                    return;
                }

                var version = ResourceVersionRepository.GetRegisterVersion(param.RepositoryKey, param.XVersion);
                if (isAny)
                {
                    json.First.AddAfterSelf(new JProperty(JsonPropertyConst.VERSION_COLNAME, version.Value));
                }
                else
                {
                    json = new JObject(new JProperty(JsonPropertyConst.VERSION_COLNAME, version.Value));
                }
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// OracleDbからデータを削除する。
        /// </summary>
        public override void DeleteOnce(DeleteParam param)
        {
            s_log.Debug($"DeleteTarget={param.Json}");

            var sqlBuilder = CreateDeleteSqlBuilder(param);
            sqlBuilder.BuildUp();

            // SQL実行
            _retryPolicy
                .Execute(() =>
                {
                    using (var connection = GetAndOpenConnection())
                    {
                        // 追加のSQLがあれば先に実行する
                        if (sqlBuilder.HasPreAdditionalSqls)
                        {
                            foreach (var additionalSql in sqlBuilder.PreAdditionalSqls)
                            {
                                connection.DeleteDocument(additionalSql.Sql, additionalSql.SqlParameterList.AsParameterObject);
                            }
                        }

                        connection.DeleteDocument(sqlBuilder.Sql, sqlBuilder.SqlParameterList.AsParameterObject);
                        param.CallbackDelete?.Invoke(RemoveManagementField(param.Json), this.RepositoryInfo.Type);
                    }
                });
        }

        /// <summary>
        /// OracleDbからデータを削除する。
        /// </summary>
        public override IEnumerable<string> Delete(DeleteParam param)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// データから管理用フィールドを削除する。
        /// </summary>
        private static JToken RemoveManagementField(JToken json)
        {
            json.RemoveField(JsonPropertyConst.ETAG);
            return json;
        }

        #endregion

        #region ODataPatch

        [DataStoreRepositoryParamODataConvert]
        public override int ODataPatch(QueryParam queryParam, JToken patchData)
        {
            var sqlBuilder = CreateODataPatchSqlBuilder(queryParam, patchData);
            sqlBuilder.BuildUp();

            // SQL実行
            try
            {
                var result = _retryPolicy
                    .Execute(() =>
                    {
                        using (var connection = GetAndOpenConnection())
                        {
                            return connection.UpsertDocument(sqlBuilder.Sql, sqlBuilder.SqlParameterList.AsParameterObject);
                        }
                    }
                    );
                return result;
            }
            // 【暫定実装】
            // 存在しない項目が指定された場合はNotFoundとする。(NoSQL系リポジトリとの共通仕様)
            // SQLを解析してクエリを実行せずにNotFoundを返すことが望ましいが、
            // SQLの完全な解析は難しいため一旦はSqlServerのエラーコードで判断する。
            // (207: 列名が無効です)
            // 不具合によるSQLの不備などの可能性もあるためWarningでログ出力する。
            catch (SqlException ex) when (ex.Number == 207)
            {
                s_log.Warn($"OracleDb SQL Error: {ex.Message}, SQL: {sqlBuilder.Sql}", ex);
                return 0;
            }
            catch (SqlException ex)
            {
                s_log.Warn($"OracleDb SQL Error: {ex.Message}, SQL: {sqlBuilder.Sql}", ex);
                throw;
            }
        }

        #endregion

        #region ODataConvert

        /// <summary>
        /// ODataクエリから変換したネイティブクエリに内部的に追加するWHERE条件を作成する。
        /// </summary>
        public override string GetInternalAddWhereString(QueryParam param, out IDictionary<string, object> parameters)
        {
            // パラメータ取得
            var sqlBuilder = CreateQuerySqlBuilder(param);
            var baseParameters = sqlBuilder.PrepareQueryParameters(param);
            parameters = baseParameters.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue);

            var whereString = string.Join(" AND ", baseParameters.Where(x => x.Key != sqlBuilder.UserShareConditionString).Select(x => $"\"{x.Key}\" = {x.Value.SqlParameter}"));
            var userSharCondition = baseParameters.Where(x => x.Key == sqlBuilder.UserShareConditionString).FirstOrDefault();
            if (userSharCondition.Value != null)
            {
                whereString += $" AND \"{JsonPropertyConst.OWNERID}\" IN {userSharCondition.Value.SqlParameter}";
            }

            return whereString;
        }

        #endregion

        #region DDL

        /// <summary>
        /// 指定したテーブルの列定義を取得する。
        /// </summary>
        public IEnumerable<RdbmsTableColumn> GetTableColumns(string tableName)
        {
            const string tableNameParam = "TABLENAME";

            var sql = $"SELECT column_name AS COLUMN_NAME, data_type AS DATA_TYPE FROM user_tab_columns WHERE table_name = {SqlBuilderUtil.ToSqlParameter(tableNameParam, RepositoryInfo.Type)}";
            var parameters = new Dictionary<string, object>() { [tableNameParam] = tableName };
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.AdaptResourceSchema), new NativeQuery(sql, parameters, false));
            var result = Query(queryParam, out _);

            return result.Select(x => new RdbmsTableColumn(x.Value["COLUMN_NAME"].ToString(), x.Value["DATA_TYPE"].ToString())).ToList();
        }

        /// <summary>
        /// テーブルを作成する。
        /// </summary>
        public void CreateTable(string tableName, IEnumerable<RdbmsTableColumn> columns, IEnumerable<string> primaryKeyColumns = null, string primaryKeyName = null)
        {
            var createTable = new StringBuilder();
            createTable.AppendLine($"CREATE TABLE \"{tableName}\" (");
            createTable.AppendLine(string.Join(",", columns.Select(x => x.GetColumnDeclaration())));
            if (primaryKeyColumns != null && primaryKeyColumns.Any())
            {
                if (string.IsNullOrWhiteSpace(primaryKeyName))
                {
                    primaryKeyName = $"PK_{tableName}";
                }
                createTable.AppendLine($",CONSTRAINT \"{primaryKeyName}\" PRIMARY KEY ({string.Join(",", primaryKeyColumns.Select(x => $"\"{x}\""))})");
            }
            createTable.AppendLine($")");

            ExecuteSql(createTable.ToString());
        }

        /// <summary>
        /// テーブルに列を追加する。
        /// </summary>
        public void AddTableColumns(string tableName, IEnumerable<RdbmsTableColumn> columns)
        {
            var addColumns = string.Join(",", columns.Select(x => x.GetColumnDeclaration()));
            var ddl = $"ALTER TABLE \"{tableName}\" ADD ({addColumns})";

            ExecuteSql(ddl);
        }

        /// <summary>
        /// 指定したテーブル列のインデックスを作成する。
        /// </summary>
        public void CreateIndex(string tableName, string columnName, string indexName = null)
        {
            if (string.IsNullOrWhiteSpace(indexName))
            {
                indexName = $"IX_{tableName}_{columnName}";
            }
            var ddl = $"CREATE INDEX \"{indexName}\" ON \"{tableName}\" (\"{columnName}\")";

            ExecuteSql(ddl);
        }


        private void ExecuteSql(string sql)
        {
            s_log.Info($"ExecuteSql={sql}");

            _retryPolicy
                .Execute(() =>
                {
                    using (var connection = GetAndOpenConnection())
                    {
                        connection.Execute(sql);
                    }
                }
                );
        }

        #endregion

        #region SqlBuilder

        private IQuerySqlBuilder CreateQuerySqlBuilder(QueryParam param)
        {
            return UnityCore.Resolve<IQuerySqlBuilder>(
                RepositoryName,
                new ParameterOverride("queryParam", new InjectionParameter<QueryParam>(param)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(RepositoryInfo)),
                new ParameterOverride("resourceVersionRepository", new InjectionParameter<IResourceVersionRepository>(ResourceVersionRepository)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(ContainerDynamicSeparationRepository)),
                new ParameterOverride("dynamicApiRepository", new InjectionParameter<IDynamicApiRepository>(DynamicApiRepository)),
                new ParameterOverride("xRequestContinuationNeedsTopCount", new InjectionParameter<bool>(XRequestContinuationNeedsTopCount)));
        }

        private IUpsertSqlBuilder CreateUpsertSqlBuilder(RegisterParam param)
        {
            return UnityCore.Resolve<IUpsertSqlBuilder>(
                RepositoryName,
                new ParameterOverride("registerParam", new InjectionParameter<RegisterParam>(param)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(RepositoryInfo)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(ContainerDynamicSeparationRepository)));
        }

        private IDeleteSqlBuilder CreateDeleteSqlBuilder(DeleteParam param)
        {
            return UnityCore.Resolve<IDeleteSqlBuilder>(
                RepositoryName,
                new ParameterOverride("deleteParam", new InjectionParameter<DeleteParam>(param)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(RepositoryInfo)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(ContainerDynamicSeparationRepository)));
        }

        private IODataPatchSqlBuilder CreateODataPatchSqlBuilder(QueryParam param, JToken patchData)
        {
            return UnityCore.Resolve<IODataPatchSqlBuilder>(
                RepositoryName,
                new ParameterOverride("queryParam", new InjectionParameter<QueryParam>(param)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(RepositoryInfo)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(ContainerDynamicSeparationRepository)),
                new ParameterOverride("patchData", new InjectionParameter<JToken>(patchData)));
        }

        #endregion


        /// <summary>
        /// SQLパラメータ用の文字列に変換
        /// </summary>
        private static string ToSqlParameter(string parameterName) => SqlBuilderUtil.ToSqlParameter(parameterName, RepositoryType.OracleDb);
    }
}
