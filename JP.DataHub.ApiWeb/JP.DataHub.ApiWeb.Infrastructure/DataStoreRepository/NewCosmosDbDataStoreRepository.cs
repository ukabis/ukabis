using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;
using Cosmos=Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Sql;
using JP.DataHub.OData.CosmosDb.ODataToSqlTranslator;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.Data.CosmosDb;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    internal class NewCosmosDbDataStoreRepository : NewAbstractDynamicApiDataStoreRepository, INewDynamicApiDataStoreRepository
    {
        private static ConcurrentDictionary<string, IJPDataHubCosmosDb> connectionClient = new ConcurrentDictionary<string, IJPDataHubCosmosDb>();
        private static readonly string DEFAULT_QUERY = "SELECT * FROM c ";
        private static readonly string DELETE_DEFAULT_QUERY = "select c.id,c._self,c._partitionkey from c ";

        private static Lazy<IConfiguration> s_lasyConfiguration = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        protected static IConfiguration Configuration { get => s_lasyConfiguration.Value; }

        private const string TYPE = "_Type";
        private const string OPENID = "_Reguser_Id";
        private const string UPDUSERID = "_Upduser_Id";
        private const string UPDDATE = "_Upddate";
        private const string REGDATE = "_Regdate";
        private const string OWNERID = "_Owner_Id";
        private const string PARTITIONKEY = "_partitionkey";
        private const string VERSION_COLNAME = "_Version";
        private const string ID = "id";
        private const string ETAG = "_etag";
        private const string SELF = "_self";
        private const string RID = "_rid";
        private const string ATTACHMENTS = "_attachments";
        private const string TS = "_ts";

        private const int DEFAULT_TOP_COUNT = 100;

        public override bool CanQuery { get => true; }
        public override bool CanOptimisticConcurrency { get => true; }
        public override bool CanQueryAttachFileMetaByOData { get => true; }
        public override string VersionInfoQuery { get => "SELECT * FROM c WHERE c.id = @id AND c._partitionkey = @_partitionkey AND c._Type = @_Type"; }
        public override string DocumentVersionQuery { get => "SELECT * FROM c WHERE c.id = @id AND c._partitionkey = @_partitionkey AND c._Type = @_Type"; }
        public override string RepositoryName { get => "CosmosDB"; }
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
            JsonPropertyConst.VERSION_COLNAME,
            JsonPropertyConst.PARTITIONKEY
        };

        private JPDataHubLogger _log = new JPDataHubLogger(typeof(NewCosmosDbDataStoreRepository));
        private bool _avoidDocumentDBLibraryBugInPagingWithPartitionKey = Configuration.GetValue("CosmosDB:AvoidDocumentDBLibraryBugInPagingWithPartitionKey", false);


        [DataStoreRepositoryParamODataConvert]
        public override JsonDocument QueryOnce(QueryParam param)
        {
            var doc = Query(param.ToSingle()).FirstOrDefault();
            return doc == null ? null : new JsonDocument(doc.Item1);
        }

        [DataStoreRepositoryParamODataConvert]
        public override IEnumerable<JsonDocument> QueryEnumerable(QueryParam param)
        {
            foreach (var x in Query(param.ToSingle()))
            {
                yield return new JsonDocument(x.Item1);
            }
        }

        [DataStoreRepositoryParamODataConvert]
        public override IList<JsonDocument> Query(QueryParam param, out XResponseContinuation xResponseContinuation)
        {
            List<JsonDocument> result = new List<JsonDocument>();
            var queryResult = Query(param.ToSingle()).ToList();
            queryResult.ForEach(x => result.Add(new JsonDocument(x.Item1)));
            xResponseContinuation = queryResult.FirstOrDefault()?.Item2;
            return result;
        }

        public override RegisterOnceResult RegisterOnce(RegisterParam param)
        {
            if (param.IsOverrideId?.Value == true)
            {
                var version = ResourceVersionRepository.GetRegisterVersion(param.RepositoryKey, param.XVersion);
                // 個人共有のケース
                var isResourceSharingPerson = !string.IsNullOrEmpty(PerRequestDataContainer.XResourceSharingPerson) && param.ResourceSharingPersonRules.Any();
                AddManagementData(param.RepositoryKey,
                    param.IsVendor,
                    param.VendorId,
                    param.SystemId,
                    param.IsAutomaticId,
                    param.PartitionKey,
                    param.IsPerson,
                    isResourceSharingPerson ? new OpenId(this.PerRequestDataContainer.XResourceSharingPerson) : param.OpenId,
                    param.Json,
                    version);
            }

            var requestOptions = new Cosmos.ItemRequestOptions();
            if (param.IsOptimisticConcurrency?.Value == true)
            {
                requestOptions.IfMatchEtag = param.Json[JsonPropertyConst.ETAG] == null ? " " : param.Json[JsonPropertyConst.ETAG].ToString();
            }
            var connection = GetConnection();
            return new RegisterOnceResult(connection.UpsertDocument(param.Json, requestOptions));
        }

        public override void DeleteOnce(DeleteParam param)
        {
            DeleteDocument(param.Json);
            param.CallbackDelete?.Invoke(RemoveManagementField(param.Json), this.RepositoryInfo.Type);
        }

        public override IEnumerable<string> Delete(DeleteParam param)
        {
            throw new NotImplementedException();
        }

        private bool DeleteDocument(JToken document)
        {
            var connection = GetConnection();
            _log.Debug($"DeleteTarget={document}");
            return connection.DeleteDocument(document[ID], document[PARTITIONKEY]);
        }

        private IEnumerable<Tuple<JToken, XResponseContinuation>> Query(QueryParam param)
        {
            string sql = "";
            XResponseContinuation xResponseContinuation = null;
            if ((param?.ActionType?.Value == ActionType.DeleteData ||
                param?.ActionType?.Value == ActionType.ODataDelete)
                && param?.IsDocumentHistory?.Value == false)
            {
                //削除の場合かつ履歴を使ってない場合
                //履歴は変更前のデータを保存するために全項目を取得しないといけないため
                sql = DELETE_DEFAULT_QUERY;

                if (param?.IsNative == true)
                {
                    var index = FindItemIndex(param?.NativeQuery.Sql.ToLower(), "where");
                    if (index > 0)
                    {
                        sql += param?.NativeQuery.Sql.Substring(index);
                    }
                }
                else if (!string.IsNullOrEmpty(param.ApiQuery.Value))
                {
                    var index = FindItemIndex(param.ApiQuery.Value.ToLower(), "where");
                    if (index > 0)
                    {
                        sql += param.ApiQuery.Value.Substring(index);
                    }
                }
            }
            else if (param?.IsNative == true)
            {
                sql = param?.NativeQuery.Sql;
            }
            else
            {
                sql = string.IsNullOrEmpty(param.ApiQuery.Value) ? BuildSelect(param) : param.ApiQuery.Value;
            }

            var connection = GetConnection();
            List<SqlUnion> union = new List<SqlUnion>();
            if (UnionSplitter.SplitUnion(sql, union) == false)
            {
                union.Add(new SqlUnion() { IsUnionAll = false, Query = sql });
            }

            foreach (var sqlonce in union)
            {
                var condition = CreateQuery(sqlonce.Query, param);
                if (param?.IsNative == true)
                {
                    foreach (var item in param.NativeQuery.Dic)
                    {
                        condition.Item2.Add(item.Key, item.Value);
                    }
                }

                // ページング判定
                int GetRequestCount()
                {
                    // ページング指定なし
                    if (param.XRequestContinuation?.ContinuationString == null) return 0;
                    // top指定あり
                    if (param.SelectCount != null) return param.SelectCount.Value;
                    // top必須
                    if (XRequestContinuationNeedsTopCount)
                    {
                        return 0;
                    }
                    // top任意
                    return DEFAULT_TOP_COUNT;
                }
                int requestCount = GetRequestCount();

                IEnumerable<JToken> tmp;
                if (requestCount > 0)
                {
                    XResponseContinuation responseContinuation;
                    string executeSql;
                    if (_avoidDocumentDBLibraryBugInPagingWithPartitionKey && param.IsNative && !string.IsNullOrWhiteSpace(param.PartitionKey?.Value))
                    {
                        // パーティションキーを設定したAPIでページネーションを利用した一覧検索時に
                        // CosmosDBのライブラリレベルでエラーが発生する為、DISTINCTを削除して重複削除をApiFilter側で実装する。
                        executeSql = sql.Replace($"{Constants.SQLSelectSymbol} {Constants.SqlDistinctKeyWord}", Constants.SQLSelectSymbol);
                    }
                    else
                    {
                        executeSql = condition.Item1;
                    }
                    tmp = QueryDocumentDbContinuation(new CosmosDbQuery(executeSql, condition.Item2), condition.Item3,
                        new CosmosDbQueryItemCount(requestCount), param.XRequestContinuation,
                        out responseContinuation).ToList();
                    xResponseContinuation = new XResponseContinuation(responseContinuation.ContinuationString);
                }
                else
                {
                    // OdataでAnyを含むCount時はパーティションキーを指定しない
                    IsOverPartition isOverPartition = new IsOverPartition(
                        (param.ODataQuery != null && param.ODataQuery.HasAnyQuery && param.ODataQuery.HasCountQuery) ? true : condition.Item3.Value);
                    tmp = QueryDocumentDb(new CosmosDbQuery(condition.Item1, condition.Item2), isOverPartition).ToList();
                }
                var samelist = new List<string>();
                if (sqlonce.IsUnionAll == true)
                {
                    foreach (var x in tmp)
                    {
                        var id = x.GetPropertyValue(JsonPropertyConst.ID)?.ToString();
                        yield return new Tuple<JToken, XResponseContinuation>(x, xResponseContinuation);
                        if (id != null)
                        {
                            samelist.Add(id);
                        }
                    }
                }
                else
                {
                    foreach (var x in tmp)
                    {
                        var id = x.GetPropertyValue(JsonPropertyConst.ID)?.ToString();
                        if (id == null || samelist.Contains(id) == false)
                        {
                            yield return new Tuple<JToken, XResponseContinuation>(x, xResponseContinuation);
                        }
                        if (samelist.Contains(id) == false)
                        {
                            samelist.Add(id);
                        }
                    }
                }
            }
        }

        private IJPDataHubCosmosDb GetConnection()
        {
            if (!connectionClient.ContainsKey(RepositoryInfo.ConnectionString))
            {
                var client = UnityCore.Resolve<IJPDataHubCosmosDb>();
                client.ConnectionString = RepositoryInfo.ConnectionString;
                connectionClient.GetOrAdd(RepositoryInfo.ConnectionString, client);
                return client;
            }
            return connectionClient[RepositoryInfo.ConnectionString];
        }

        private IEnumerable<JToken> QueryDocumentDb(CosmosDbQuery query, IsOverPartition overPartition)
        {
            var connection = GetConnection();
            foreach (var ret in connection.QueryDocument(query.Query, query.QueryParams, overPartition.Value))
            {
                yield return ret;
            };
            yield break;
        }

        private IEnumerable<JToken> QueryDocumentDbContinuation(CosmosDbQuery query, IsOverPartition overPartition, CosmosDbQueryItemCount requestItemCount, XRequestContinuation requestContinuation,
            out XResponseContinuation responseContinuation)
        {
            var connection = GetConnection();
            var cosmosDbContinuation = new CosmosDbQueryContinuation(requestContinuation?.ContinuationString);
            var result = connection.QueryDocumentContinuation(query.Query, query.QueryParams, overPartition.Value, requestItemCount.Count, cosmosDbContinuation.ContinuationString, out var continuation);
            responseContinuation = new XResponseContinuation(continuation);
            return result;
        }

        private Tuple<string, Dictionary<string, object>, IsOverPartition> CreateQuery(string srcQuery, QueryParam param) => CreateQuery(srcQuery, param, KeyManagement, ResourceVersionRepository);

        public static Tuple<string, Dictionary<string, object>, IsOverPartition> CreateQuery(string srcQuery, QueryParam param, IKeyManagement keyman, IResourceVersionRepository resourceVersionRepository)
        {
            var query = string.IsNullOrEmpty(srcQuery) || Regex.IsMatch(srcQuery, "^(\r\n)+$") ? DEFAULT_QUERY : srcQuery;
            var parse = new ParseSql(query);
            // データ共有のケース
            bool isResourceSharingWith = param.XResourceSharingWith != null && param.ApiResourceSharing != null;
            if (isResourceSharingWith)
            {
                // 元の条件全体が追加の条件に適応されるようにする
                parse.Condition = string.IsNullOrEmpty(parse.Condition) ? parse.Condition : $"where ({ReplaceConditionBuild(parse.Condition, "^where ", "")})";

                var appendCondition = new StringBuilder("");
                param.ApiResourceSharing.ResourceSharingRuleList.ToList().ForEach(x => appendCondition.Append(AppendConditionBuild(parse.Condition, appendCondition.ToString(), x.Query)));
                var appendConditionString = appendCondition.ToString();
                parse.Condition += appendConditionString.ToLower().StartsWith("where") || string.IsNullOrEmpty(appendConditionString) ? appendConditionString : $" and ({ReplaceConditionBuild(appendConditionString, "^ and ", "")})";
                if (!param.ApiResourceSharing.ResourceSharingRuleList.Any())
                {
                    isResourceSharingWith = false;
                }
            }

            // 個人データ共有のケース
            bool isResourceSharingPerson = !string.IsNullOrEmpty(param.XResourceSharingPerson?.Value) && param.ResourceSharingPersonRules.Any();

            //認可設定
            bool isUserSharing = param.XUserResourceSharing?.Value?.Any() == true;

            var conditionString = new StringBuilder(parse.Condition);
            var condition = keyman.GetGenerateKey(param, conditionString, resourceVersionRepository);
            var isOverPartition = true;

            if (query.Contains("{VersionType}"))
            {
                condition.Clear();
                ResourceVersionKey verKey = new ResourceVersionKey(param.RepositoryKey);
                conditionString.Replace("{VersionType}", "@_Type");
                condition.Add("_Type", verKey.Type);
                if (query.Contains("{Version}"))
                {
                    conditionString.Replace("{Version}", "@_partitionkey");
                    condition.Add("_partitionkey", "version");
                }
                isOverPartition = true;
            }
            else
            {
                if(isUserSharing)
                {
                    //認可設定されている場合はOverPartitionをTrueにする
                    isOverPartition = true;
                }
                foreach (var key in condition)
                {
                    if (conditionString.ToString().Contains($"{{{key.Key}}}"))
                    {
                        conditionString.Replace($"{{{key.Key}}}", $"@{key.Key}");
                    }
                    else
                    {
                        conditionString.AppendLine(QueryConditionBuild(conditionString.ToString(), key.Key));
                    }
                }
                if (param.IsVendor?.Value == true && param.IsOverPartition?.Value == false)
                {
                    if (!conditionString.ToString().Contains($"c.{JsonPropertyConst.VENDORID}"))
                    {
                        conditionString.AppendLine(QueryConditionBuild(conditionString.ToString(), JsonPropertyConst.VENDORID));
                        condition.Add(JsonPropertyConst.VENDORID, isResourceSharingWith ? param.XResourceSharingWith["VendorId"] : param.VendorId.Value);
                    }
                    else if (!condition.ContainsKey(JsonPropertyConst.VENDORID))
                    {
                        condition.Add(JsonPropertyConst.VENDORID, isResourceSharingWith ? param.XResourceSharingWith["VendorId"] : param.VendorId.Value);
                    }

                    if (!conditionString.ToString().Contains($"c.{JsonPropertyConst.SYSTEMID}"))
                    {
                        conditionString.AppendLine(QueryConditionBuild(conditionString.ToString(), JsonPropertyConst.SYSTEMID));
                        condition.Add(JsonPropertyConst.SYSTEMID, isResourceSharingWith ? param.XResourceSharingWith["SystemId"] : param.SystemId.Value);
                    }
                    else if (!condition.ContainsKey(JsonPropertyConst.SYSTEMID))
                    {
                        condition.Add(JsonPropertyConst.SYSTEMID, isResourceSharingWith ? param.XResourceSharingWith["SystemId"] : param.SystemId.Value);
                    }

                }
                if (param.IsPerson?.Value == true && param.IsOverPartition?.Value == false && isUserSharing == false)
                {
                    if (!conditionString.ToString().Contains($"c.{JsonPropertyConst.OWNERID}"))
                    {
                        conditionString.AppendLine(QueryConditionBuild(conditionString.ToString(), JsonPropertyConst.OWNERID));
                        condition.Add(JsonPropertyConst.OWNERID, isResourceSharingPerson ? param.XResourceSharingPerson?.Value : param.OpenId.Value);
                    }
                    else if (!condition.ContainsKey(JsonPropertyConst.OWNERID))
                    {
                        condition.Add(JsonPropertyConst.OWNERID, isResourceSharingPerson ? param.XResourceSharingPerson?.Value : param.OpenId.Value);
                    }
                }
                if (param.QueryString?.HasValue == true && (param.QueryType == null || param.QueryType.Value != QueryTypes.ODataQuery))
                {
                    List<JSchema> schemas = new List<JSchema>() { param.UriSchema?.ToJSchema(), param.RequestSchema?.ToJSchema(), param.ResponseSchema?.ToJSchema() };
                    foreach (var key in param.QueryString.Dic)
                    {
                        if (key.Key.Value == JsonPropertyConst.ID)
                        {
                            if (!condition.ContainsKey(key.Key.Value))
                            {
                                conditionString.AppendLine(QueryConditionBuild(conditionString.ToString(), key.Key.Value));
                                condition.Add(key.Key.Value, key.Value.Value);
                            }
                        }
                        else if (condition.Where(x => x.Key == key.Key.Value).Count() == 0)
                        {
                            if (conditionString.ToString().Contains($"{{{key.Key.Value}}}"))
                            {
                                conditionString.Replace($"{{{key.Key.Value}}}", $"@{key.Key.Value}");
                            }
                            condition.Add(key.Key.Value, Convert.ChangeType(key.Value.Value, schemas.ToType(key.Key.Value)));
                        }
                    }
                }
                if (param.Identification != null)
                {
                    conditionString.AppendLine(QueryConditionBuild(conditionString.ToString(), JsonPropertyConst.ID));
                    var idKey = condition.Where(x => x.Key == JsonPropertyConst.ID);
                    if (!idKey.Any())
                    {
                        condition.Add(JsonPropertyConst.ID, param.Identification.Value);
                    }
                }
                //認可設定
                if(isUserSharing)
                {
                    var tmpOpenIds = new List<string>();
                    param.XUserResourceSharing?.Value?.ForEach(x => tmpOpenIds.Add($"'{x}'"));
                    string appedCondition = $" c.{JsonPropertyConst.OWNERID} IN({string.Join(", ", tmpOpenIds)}) ";
                    string sqlOperator = conditionString.ToString().ToLower().Contains("where") ? " and " : " where ";
                    conditionString.AppendLine($" {sqlOperator} {appedCondition}");
                    condition.Add("_UserSharing_Condition", appedCondition);
                }

                DocumentDbPartitionKey partitionKey = null;
                var versionNum = default(int);
                var canCreatePartitionKey = condition.ContainsKey(JsonPropertyConst.VERSION_COLNAME) && int.TryParse(condition[JsonPropertyConst.VERSION_COLNAME].ToString(), out versionNum);
                if (canCreatePartitionKey &&
                    DocumentDbPartitionKey.CreateQueryPartition(
                    param.PartitionKey,
                    param.RepositoryKey,
                    param.IsVendor,
                    isResourceSharingWith ? new VendorId(param.XResourceSharingWith["VendorId"]) : param.VendorId,
                    isResourceSharingWith ? new SystemId(param.XResourceSharingWith["SystemId"]) : param.SystemId,
                    param.IsPerson,
                    isResourceSharingPerson ? new OpenId(param.XResourceSharingPerson?.Value) : param.OpenId,
                    new ResourceVersion(versionNum),
                    param.QueryString,
                    param.KeyValue,
                    param.IsOverPartition,
                    out partitionKey)
                    && (!conditionString.ToString().Contains($"c.{JsonPropertyConst.PARTITIONKEY}") || (param?.IsNative == true && param?.NativeQuery.IsAllowAddCondition == false)) 
                    && isUserSharing == false)
                {
                    conditionString.AppendLine(QueryConditionBuild(conditionString.ToString(), JsonPropertyConst.PARTITIONKEY));
                    condition.Add(JsonPropertyConst.PARTITIONKEY, partitionKey.Value);
                    isOverPartition = false;
                }
            }
            if (!string.IsNullOrEmpty(parse.Condition) && isResourceSharingWith)
            {
                condition.Add("_ResourceSharing_Condition", $"({ReplaceConditionBuild(parse.Condition, "^where ", "")})");
            }
            //Queryを復元する。
            var retQuery = $"{parse.Select} {parse.From} {conditionString.ToString()} {parse.Order}";
            //NativeQueryが完成されている場合はクエリの上書きは行わない
            if (param?.IsNative == true && param?.NativeQuery.IsAllowAddCondition == false)
            {
                retQuery = param.NativeQuery.Sql;
            }
            return new Tuple<string, Dictionary<string, object>, IsOverPartition>(retQuery, condition, new IsOverPartition(isOverPartition));
        }

        private static string AppendConditionBuild(string originCondition, string condition, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return string.Empty;
            }
            var parse = new ParseSql(query);
            // conditionにすでに条件があればandで接続。なければwhereごと付加。
            return !string.IsNullOrEmpty(originCondition) || !string.IsNullOrEmpty(condition) ? ReplaceConditionBuild(parse.Condition, "^where ", " and ") : parse.Condition;
        }

        private static string ReplaceConditionBuild(string query, string target, string replaceStr)
        {
            Regex reg = new Regex(target, RegexOptions.IgnoreCase);
            return reg.Replace(query, replaceStr);
        }

        internal static void AddManagementData(RepositoryKey repositoryKey, IsVendor isVendor, VendorId vendorId, SystemId systemId, IsAutomaticId isAutomaticId, PartitionKey partitionKey, IsPerson isPerson, OpenId openId, JToken json, ResourceVersion version)
        {
            if (json.Count() > 0)
            {
                if (!json.IsExistProperty(JsonPropertyConst.TYPE))
                {
                    json.First.AddAfterSelf(new JProperty(JsonPropertyConst.TYPE, repositoryKey.Type));
                }
            }
            else
            {
                json = new JObject(new JProperty(JsonPropertyConst.TYPE, repositoryKey.Type));
            }
            if (json[JsonPropertyConst.PARTITIONKEY] == null)
            {
                json.First.AddAfterSelf(new JProperty(JsonPropertyConst.PARTITIONKEY, DocumentDbPartitionKey.CreateRegisterPartition(partitionKey, repositoryKey, isVendor, vendorId, systemId, isPerson, openId, version, json).Value));
            }
            if (!json.IsExistProperty(JsonPropertyConst.VERSION_COLNAME))
            {
                json.First.AddAfterSelf(new JProperty(JsonPropertyConst.VERSION_COLNAME, version.Value));
            }
        }

        private static string QueryConditionBuild(string query, string conditionKey)
        {
            bool whereExist = query.ToLower().Contains("where");
            bool andExitst = query.ToLower().Contains("and");
            if (whereExist == true && andExitst == false && query.Trim().ToLower().Split(new string[] { "where" }, StringSplitOptions.RemoveEmptyEntries).Length > 0)
            {
                andExitst = true;
            }

            var condition = new StringBuilder();
            condition.Append(whereExist ? "" : " where");
            condition.Append(andExitst ? " and " : "");
            // 予約語の場合はエスケープする
            if (Constants.SelectEscapeKeywords.Contains(conditionKey.ToUpper()))
            {
                condition.Append($" c[\"{conditionKey}\"] = @_{conditionKey}");
            }
            else
            {
                condition.Append($" c.{conditionKey} = @{conditionKey}");
            }

            return condition.ToString();
        }

        private string BuildSelect(QueryParam paramn)
        {
            if (paramn.ResponseSchema != null && paramn.ResponseSchema.Value != null)
            {
                if (!paramn.ResponseSchema.ToJSchema().AllowAdditionalProperties)
                {
                    var sb = new StringBuilder();
                    sb.Append("SELECT ");
                    foreach (var prop in paramn.ResponseSchema.ToJSchema().Properties)
                    {
                        var item = prop.Key;

                        // 予約語の場合はエスケープする
                        if (Constants.SelectEscapeKeywords.Contains(item.ToUpper()))
                        {
                            sb.Append($"c[\"{item}\"],");
                        }
                        else
                        {
                            sb.Append("c.").Append(item).Append(",");
                        }
                    }
                    if (PerRequestDataContainer.XgetInternalAllField == true)
                    {
                        sb.AppendFormat($"c.{OWNERID},c.{ID},c.{TYPE},c.{REGDATE},c.{OPENID},c.{UPDDATE},c.{UPDUSERID},c.{VERSION_COLNAME},c.{PARTITIONKEY},c.{JsonPropertyConst.VENDORID},c.{JsonPropertyConst.SYSTEMID} FROM c ");
                    }
                    else
                    {
                        sb.AppendFormat($"c.{OWNERID},c.{ID} FROM c ");
                    }
                    return sb.ToString();
                }
            }
            return DEFAULT_QUERY;
        }

        /// <summary>
        /// 対象の文字列から文字列検索し、ヒットした文字位置を返す。
        /// </summary>
        /// <param name="target">検索対象</param>
        /// <param name="searchWord">検索文字列</param>
        /// <param name="startIdx">検索開始位置</param>
        /// <returns>ヒットした場合は文字位置、ヒットしない場合は-1を返す。</returns>
        private int FindItemIndex(string target, string searchWord, int startIdx = 0)
        {
            if (target.Length <= searchWord.Length)
            {
                return -1;
            }

            var idx = target.IndexOf(searchWord, startIdx, StringComparison.OrdinalIgnoreCase);

            if (idx == -1)
            {
                return -1;
            }

            // 対象の前後を取得
            var before = target.Substring(startIdx, idx);
            var after = target.Substring(before.Length);

            // 前後が空白か
            if (after != searchWord &&
                string.IsNullOrEmpty(before.Substring(before.Length - 1).Trim()) &&
                string.IsNullOrEmpty(after.Substring(searchWord.Length, 1).Trim()))
            {
                return idx;
            }
            // 前後空白でない場合/after=searchWordの場合は次を探す
            else
            {
                idx = FindItemIndex(after, searchWord, searchWord.Length + 1);
                if (idx == -1)
                {
                    return -1;
                }
                return idx;
            }
        }

        private JToken RemoveManagementField(JToken json)
        {
            json.RemoveField(RID);
            json.RemoveField(SELF);
            json.RemoveField(ETAG);
            json.RemoveField(ATTACHMENTS);
            json.RemoveField(TS);
            return json;
        }

        /// <summary>
        /// 内部で追加するクエリストリングを作成する
        /// </summary>
        /// <param name="param">条件</param>
        /// <returns>cosmosDBのwhere文</returns>
        public override string GetInternalAddWhereString(QueryParam param, out IDictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();

            var condition = CreateQuery("", param);
            var sqlWhere2 = new StringBuilder($"c.{TYPE}=@{TYPE} ");
            if (condition.Item2.ContainsKey(VERSION_COLNAME))
            {
                sqlWhere2.Append($"AND c.{VERSION_COLNAME}=@{VERSION_COLNAME} ");
            }

            if (condition.Item2.ContainsKey(PARTITIONKEY))
            {
                sqlWhere2.Append($"AND c.{PARTITIONKEY}=@{PARTITIONKEY} ");
            }

            if (condition.Item2.ContainsKey(OWNERID))
            {
                sqlWhere2.Append($"AND c.{OWNERID}=@{OWNERID} ");
            }
            if (condition.Item2.ContainsKey("_ResourceSharing_Condition"))
            {
                sqlWhere2.Append($"AND {condition.Item2["_ResourceSharing_Condition"]} ");
            }
            if (condition.Item2.ContainsKey("_UserSharing_Condition"))
            {
                sqlWhere2.Append($"AND {condition.Item2["_UserSharing_Condition"]} ");
            }
            if (condition.Item2.ContainsKey("_UserSharing_Condition"))
            {
                sqlWhere2.Append($"AND {condition.Item2["_UserSharing_Condition"]} ");
            }
            return sqlWhere2.ToString();
        }
    }
}
