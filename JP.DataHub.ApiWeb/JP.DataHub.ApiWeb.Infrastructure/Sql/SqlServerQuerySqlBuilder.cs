using System.Text.RegularExpressions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Infrastructure.Database.Consts;
using JP.DataHub.Com.Json.Schema;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    internal class SqlServerQuerySqlBuilder : IQuerySqlBuilder
    {
        public string UserShareConditionString { get => "_UserSharing_Condition"; }
        public const string OrderByPattern = @"\s+order\s+by\s+(.*?)$";
        public const string WithoutOffsetPattern = @"^(?!.*\s+offset\s+).*$";

        private const int DEFAULT_TOP_COUNT = 100;
        private const string OffsetParam = "@r_offset";
        private const string FetchParam = "@r_fetch";

        private QueryParam QueryParam { get; }
        private RepositoryInfo RepositoryInfo { get; }
        private IResourceVersionRepository ResourceVersionRepository { get; }
        private IContainerDynamicSeparationRepository ContainerDynamicSeparationRepository { get; }
        private IDynamicApiRepository DynamicApiRepository { get; }
        private bool XRequestContinuationNeedsTopCount { get; }

        public string Sql { get; protected set; }
        public string CountSql { get; protected set; }
        public IRdbmsSqlParameterList SqlParameterList { get; protected set; }
        public bool IsNativeCountQuery { get; protected set; }
        public PagingInfo PagingInfo { get; protected set; }
        public bool IsCustomSql { get; protected set; } = false;

        private IPerRequestDataContainer PerRequestDataContainer => _requestDataContainer.Value;
        private readonly Lazy<IPerRequestDataContainer> _requestDataContainer = new Lazy<IPerRequestDataContainer>(() =>
        {
            try
            {
                return UnityCore.Resolve<IPerRequestDataContainer>();
            }
            catch
            {
                // ignored
                return UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
            }
        });



        public SqlServerQuerySqlBuilder(
            QueryParam queryParam,
            RepositoryInfo repositoryInfo,
            IResourceVersionRepository resourceVersionRepository,
            IContainerDynamicSeparationRepository containerDynamicSeparationRepository,
            IDynamicApiRepository dynamicApiRepository,
            bool xRequestContinuationNeedsTopCount)
        {
            QueryParam = queryParam;
            RepositoryInfo = repositoryInfo;
            ResourceVersionRepository = resourceVersionRepository;
            ContainerDynamicSeparationRepository = containerDynamicSeparationRepository;
            DynamicApiRepository = dynamicApiRepository;
            XRequestContinuationNeedsTopCount = xRequestContinuationNeedsTopCount;
        }


        public void BuildUp()
        {
            string baseSql;
            IRdbmsSqlParameterList parameters;
            var isNativeCountQuery = false;

            if (QueryParam.OperationInfo?.IsAttachFileOperation == true && 
                QueryParam.OperationInfo?.AttachFileOperation?.IsMetaQuery == true)
            {
                (baseSql, parameters) = GenerateAttachFileMetaQuerySqlAndParameters(QueryParam);
            }
            else
            {
                /* SQL生成 */
                if (QueryParam.IsNative == true && QueryParam.NativeQuery?.IsAllowAddCondition == false)
                {
                    // OData、バージョン情報取得の場合
                    // パラメータは既にSQLに埋め込み済のためパラメータ名を連番にはしない
                    baseSql = QueryParam.NativeQuery.Sql;
                    parameters = new SqlServerSqlParameterList();
                    parameters.AddRange(QueryParam.NativeQuery.Dic, false);
                    isNativeCountQuery = QueryParam.ODataQuery?.HasCountQuery ?? new ODataQuery(QueryParam.ApiQuery?.Value).HasCountQuery;
                }
                else
                {
                    // その他の場合
                    (baseSql, parameters) = GenerateQuerySqlAndParameters(QueryParam);
                }

                /* テーブル名置換 */
                if (QueryParam.OperationInfo?.IsVersionOperation != true &&
                    QueryParam.OperationInfo?.IsDocumentVersionOperation != true)
                {
                    baseSql = ReplaceTableName(baseSql, QueryParam);
                }

                /* 削除用SELECT句 */
                if ((QueryParam.ActionType?.Value == ActionType.DeleteData || 
                     QueryParam.ActionType?.Value == ActionType.ODataDelete) &&
                    QueryParam.IsDocumentHistory?.Value != true)
                {
                    var behind = baseSql.Substring(baseSql.ToLower().FindItemIndex("from"));
                    baseSql = $"SELECT \"{JsonPropertyConst.ID}\" {behind}";
                }
            }

            /* OFFSET/FETCH条件追加 */
            var querySql = baseSql;
            var pagingInfo = new PagingInfo();
            if (QueryParam.XRequestContinuation?.ContinuationString != null &&
                !(XRequestContinuationNeedsTopCount && QueryParam.SelectCount == null))
            {
                var firstPage = !long.TryParse(QueryParam.XRequestContinuation?.ContinuationString, out var requestContinuation);
                pagingInfo.IsPaging = true;
                pagingInfo.OffsetCount = firstPage ? (QueryParam.SkipCount?.Value ?? 0) : requestContinuation;
                pagingInfo.FetchCount = QueryParam.SelectCount?.Value ?? DEFAULT_TOP_COUNT;
            }
            else
            {
                pagingInfo.OffsetCount = QueryParam.SkipCount?.Value;
                pagingInfo.FetchCount = QueryParam.SelectCount?.Value;
            }

            // OFFSET条件
            if (pagingInfo.OffsetCount.HasValue || pagingInfo.FetchCount.HasValue)
            {
                // ORDER BY条件がない場合は強制付与
                // SQLServerはORDER BYなしでOFFSET/FETCHは使えないため
                if (!EndsWithOrderByPhrase(querySql))
                {
                    querySql += $" ORDER BY 1";
                }

                querySql += $" OFFSET {OffsetParam} ROWS";
                parameters.Add(OffsetParam, pagingInfo.OffsetCount ?? 0);
            }

            // FETCH条件
            if (pagingInfo.FetchCount.HasValue)
            {
                querySql += $" FETCH NEXT {FetchParam} ROWS ONLY";
                parameters.Add(FetchParam, pagingInfo.FetchCount.Value);
            }


            // 結果をプロパティにセット
            IsNativeCountQuery = isNativeCountQuery;
            Sql = querySql;
            CountSql = isNativeCountQuery ? querySql : $"SELECT COUNT(1) AS Count FROM ({AppendOrderByOffset(baseSql)}) x"; ;
            SqlParameterList = parameters;
            PagingInfo = pagingInfo;
        }

        /// <summary>
        /// クエリ用パラメータを準備する。
        /// </summary>
        public IRdbmsSqlParameterList PrepareQueryParameters(QueryParam param)
        {
            var parameters = new SqlServerSqlParameterList();

            /* Where条件 */
            bool isResourceSharingWith =
                param.XResourceSharingWith != null &&
                param.ApiResourceSharing != null &&
                param.ApiResourceSharing.ResourceSharingRuleList.Any();
            bool isResourceSharingPerson =
                !string.IsNullOrEmpty(param.XResourceSharingPerson?.Value) &&
                param.ResourceSharingPersonRules.Any();
            // 認可設定
            bool isUserSharing = param.XUserResourceSharing?.Value?.Any() == true;

            // バージョン
            if (param.OperationInfo?.IsVersionOperation != true &&
                param.OperationInfo?.IsDocumentVersionOperation != true)
            {
                var version = ResourceVersionRepository.GetRegisterVersion(param.RepositoryKey, param.XVersion);
                parameters.Add(JsonPropertyConst.VERSION_COLNAME, version.Value);
            }
            // ベンダー依存
            if (param.IsVendor?.Value == true && param.IsOverPartition?.Value != true && !isUserSharing)
            {
                parameters.Add(JsonPropertyConst.VENDORID, isResourceSharingWith ? param.XResourceSharingWith["VendorId"] : param.VendorId.Value);
                parameters.Add(JsonPropertyConst.SYSTEMID, isResourceSharingWith ? param.XResourceSharingWith["SystemId"] : param.SystemId.Value);
            }

            // 個人ユーザ依存
            if (param.IsPerson?.Value == true && param.IsOverPartition?.Value != true && !isUserSharing)
            {
                parameters.Add(JsonPropertyConst.OWNERID, isResourceSharingPerson ? param.XResourceSharingPerson?.Value : param.OpenId.Value);
            }
            // 認可
            if(isUserSharing)
            {
                parameters.Add(UserShareConditionString, param.XUserResourceSharing.Value.ToArray());
            }

            // データ共有
            if (isResourceSharingWith)
            {
                // *****************************************************
                // NIY: データ共有
                // *****************************************************
                //parameters.Add("_ResourceSharing_Condition", $"({ReplaceConditionBuild(parse.Condition, "^where ", "")})");
            }

            // KeyValue(UrlParameter)
            var schemas = new List<JSchema>() { QueryParam.UriSchema?.ToJSchema(), QueryParam.RequestSchema?.ToJSchema(), QueryParam.ResponseSchema?.ToJSchema(), QueryParam.ControllerSchema?.ToJSchema() };
            if (param.KeyValue != null)
            {
                foreach (var keyValue in param.KeyValue.Dic)
                {
                    if (!parameters.ContainsKey(keyValue.Key.Value))
                    {
                        parameters.Add(keyValue.Key.Value, Convert.ChangeType(keyValue.Value.Value, schemas.ToType(keyValue.Key.Value)));
                    }
                }
            }

            // QueryString
            if (param.QueryString?.HasValue == true &&
                param.ActionType.Value != ActionType.OData &&
                param.ActionType.Value != ActionType.ODataDelete &&
                (param.QueryType?.Value == null || param.QueryType?.Value != QueryTypes.ODataQuery))
            {
                foreach (var keyValue in param.QueryString.Dic)
                {
                    if (!parameters.ContainsKey(keyValue.Key.Value))
                    {
                        parameters.Add(keyValue.Key.Value, Convert.ChangeType(keyValue.Value.Value, schemas.ToType(keyValue.Key.Value)));
                    }
                }
            }

            if (param.Identification?.Value != null &&
                !parameters.ContainsKey(JsonPropertyConst.ID))
            {
                parameters.Add(JsonPropertyConst.ID, param.Identification.Value);
            }

            return parameters;
        }


        /// <summary>
        /// SQLクエリを生成する。
        /// </summary>
        private (string, IRdbmsSqlParameterList) GenerateQuerySqlAndParameters(QueryParam param)
        {
            var parameters = PrepareQueryParameters(param);
            var srcQuery = string.Empty;
            if (!string.IsNullOrWhiteSpace(param.ApiQuery?.Value))
            {
                IsCustomSql = true;
                srcQuery = param.ApiQuery?.Value;

                // QueryStringの埋め込みをパラメータに変換
                if (param.QueryString?.HasValue == true)
                {
                    foreach (var keyValue in param.QueryString.Dic)
                    {
                        srcQuery = srcQuery.Replace($"{{{keyValue.Key.Value}}}", parameters[keyValue.Key.Value].SqlParameter);
                    }
                }
                // KeyValueの埋め込みをパラメータに変換
                if (param.KeyValue?.Dic?.Any() == true)
                {
                    foreach (var keyValue in param.KeyValue.Dic)
                    {
                        srcQuery = srcQuery.Replace($"{{{keyValue.Key.Value}}}", parameters[keyValue.Key.Value].SqlParameter);
                    }
                }
            }

            string sql;
            if (string.IsNullOrWhiteSpace(srcQuery))
            {
                var where = CreateWheheString(parameters);
                sql = $"{CreateDefaultQuery(param)} {where}";
            }
            else
            {
                // SQLServerは複問い合わせ内にORDER BY句を単独で記述できないため
                // ORDER BY句があればOFFSETを付与する
                srcQuery = AppendOrderByOffset(srcQuery);

                var whereParams = parameters.Where(x => param.QueryString?.ContainKey(x.Key) != true);
                var where = CreateWheheString(whereParams);
                sql = $"SELECT * FROM ({srcQuery}) src {where}";
            }

            return (sql, parameters);
        }

        private string CreateWheheString(IEnumerable<KeyValuePair<string,RdbmsSqlParameter>> parameters)
        {
            string where = "";
            if (!parameters.Any())
            {
                return where;
            }
            var baseParameters = parameters.Where(x => x.Key != UserShareConditionString);
            if (baseParameters.Any())
            {
                where = $"WHERE {string.Join(" AND ", baseParameters.Select(x => $"\"{x.Key}\" = {x.Value.SqlParameter}"))}";
            }
            var userSharCondition = parameters.Where(x => x.Key == UserShareConditionString);
            if (userSharCondition.Any())
            {
                if(string.IsNullOrEmpty(where))
                {
                    where = $"WHERE \"{JsonPropertyConst.OWNERID}\" IN {userSharCondition.First().Value.SqlParameter}";
                }
                else
                {
                    where += $" AND \"{JsonPropertyConst.OWNERID}\" IN {userSharCondition.First().Value.SqlParameter}";
                }
            }
            return where;
        }

        /// <summary>
        /// スキーマに基づいて基本形のSELECT文を作成する。
        /// </summary>
        private string CreateDefaultQuery(QueryParam param)
        {
            // モデル項目追加
            var responseSchema = param.ResponseSchema?.ToJSchema();
            var controllerSchema = param.ControllerSchema.ToJSchema();
            var selectFields = (responseSchema ?? controllerSchema).Properties.Select(x => x.Key).ToList();
            // レスポンスモデルがAllowAdditionalPropertiesの場合はリソースモデルの項目も追加
            if (responseSchema != null && responseSchema.AllowAdditionalProperties)
            {
                selectFields.AddRange(controllerSchema.Properties.Where(x => !selectFields.Contains(x.Key)).Select(x => x.Key));
            }

            // 管理項目追加
            var managementFields = new List<string>();
            managementFields.AddRange(new string[] { JsonPropertyConst.ID, JsonPropertyConst.OWNERID });
            if (PerRequestDataContainer?.XgetInternalAllField == true)
            {
                managementFields.AddRange(new string[] {
                    JsonPropertyConst.ID, JsonPropertyConst.OWNERID, JsonPropertyConst.VERSION_COLNAME,
                    JsonPropertyConst.OPENID, JsonPropertyConst.REGDATE, JsonPropertyConst.UPDUSERID, JsonPropertyConst.UPDDATE
                });

                if (param.IsVendor?.Value == true)
                {
                    managementFields.AddRange(new string[] { JsonPropertyConst.VENDORID, JsonPropertyConst.SYSTEMID });
                }
            }
            if (param.IsOptimisticConcurrency?.Value == true)
            {
                managementFields.Add(JsonPropertyConst.ETAG);
            }
            selectFields.AddRange(managementFields.Where(x => !selectFields.Contains(x)));

            return $"SELECT {string.Join(",", selectFields.Select(x => $"\"{x}\""))} FROM {{{SqlServerConsts.TableNameVariable}}} WITH(NOLOCK)";
        }

        /// <summary>
        /// SQLのテーブル名パラメータを実際のテーブル名に置換する。
        /// </summary>
        private string ReplaceTableName(string sql, QueryParam param)
        {
            // テーブル名パラメータを抽出
            //   {TABLE_NAME} 当該リソースのテーブル
            //   {TABLE_NAME:リソースID} 結合テーブル
            var pattern = $"{{{SqlServerConsts.TableNameVariable}(:[-_0-9a-zA-Z]+)?}}";
            var tableParameters = new Regex(pattern).Matches(sql).Cast<Match>().Distinct();

            foreach (var tableParameter in tableParameters)
            {
                string tableName;
                var embeded = tableParameter.Value;

                if (param.OperationInfo?.IsAttachFileOperation == true)
                {
                    tableName = SqlServerConsts.AttachFileMetaTableName;
                }
                else
                {
                    var isOtherTable = embeded.Contains(":");
                    var controllerId = isOtherTable
                        ? new ControllerId(embeded.Substring(1, embeded.Length - 2).Split(new char[] { ':' })[1])
                        : param.ControllerId;
                    var phsycalRepositoryId = isOtherTable
                        ? GetPhysicalRepositoryId(controllerId)
                        : RepositoryInfo.PhysicalRepositoryId;

                    tableName = ContainerDynamicSeparationRepository.GetOrRegisterContainerName(
                        phsycalRepositoryId,
                        controllerId,
                        new VendorId(Guid.Empty.ToString()),
                        new SystemId(Guid.Empty.ToString()));
                }

                sql = sql.Replace(embeded, $"\"{tableName}\"");
            }

            return sql;
        }

        private PhysicalRepositoryId GetPhysicalRepositoryId(ControllerId controllerId)
        {
            var physicalRepositoryId = DynamicApiRepository.GetPhysicalRepositoryIdByControllerId(controllerId, RepositoryType.SQLServer2);

            if (string.IsNullOrEmpty(physicalRepositoryId?.Value))
            {
                throw new Rfc7807Exception(ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E50411));
            }

            return physicalRepositoryId;
        }


        /// <summary>
        /// ORDER BY句で終るか(含むか)
        /// </summary>
        private bool EndsWithOrderByPhrase(string sql)
        {
            return new Regex(OrderByPattern, RegexOptions.IgnoreCase).IsMatch(sql);
        }

        /// <summary>
        /// OFFSETなしのORDER BY句にOFFSETを付与する
        /// </summary>
        /// <remarks>
        /// 複問い合わせでCOUNTを取る場合にORDER BYのみだとエラーになるため
        /// </remarks>
        private string AppendOrderByOffset(string sql)
        {
            var matches = new Regex(OrderByPattern, RegexOptions.IgnoreCase).Matches(sql);
            if (matches.Count == 0)
            {
                return sql;
            }
            var orderBy = matches[0].ToString();

            // ORDER BY句にOFFSETがなければ付与
            if (new Regex(WithoutOffsetPattern, RegexOptions.IgnoreCase).IsMatch(orderBy))
            {
                return sql + $" OFFSET 0 ROWS";
            }
            else
            {
                return sql;
            }
        }

        /// <summary>
        /// 添付ファイルメタ検索用のSQLクエリを生成する。
        /// </summary>
        private (string, IRdbmsSqlParameterList) GenerateAttachFileMetaQuerySqlAndParameters(QueryParam param)
        {
            // パラメータ準備
            var parameters = PrepareQueryParameters(param);
            parameters.Add(JsonPropertyConst.TYPE, param.RepositoryKey.Type);

            // SQL組み立て
            var where = CreateWheheString(parameters);
            var sql = $"SELECT {param.OperationInfo.AttachFileOperation.QuerySelectFields} FROM {SqlServerConsts.AttachFileMetaTableName} afm WITH(NOLOCK) {where}";

            var i = 0;
            foreach (var q in param.QueryString.Dic)
            {
                if (q.Value?.Value == null)
                {
                    continue;
                }
                sql += $" AND EXISTS(SELECT 1 FROM {SqlServerConsts.AttachFileMetaSearchTableName} afs WITH(NOLOCK) WHERE afm.id = afs.AttachFileMetaId AND afs.MetaKey = @MetaKey{i} AND afs.MetaValue = @MetaValue{i})";
                parameters.Add($"MetaKey{i}", q.Key.Value, autoParameterName: false);
                parameters.Add($"MetaValue{i}", q.Value.Value, autoParameterName: false);
                i++;
            }

            return (sql, parameters);
        }
    }
}
