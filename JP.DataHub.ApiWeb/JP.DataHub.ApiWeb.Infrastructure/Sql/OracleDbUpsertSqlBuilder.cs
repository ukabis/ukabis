using System.Text;
using JP.DataHub.Com.Extensions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Infrastructure.Database.Consts;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    internal class OracleDbUpsertSqlBuilder : IUpsertSqlBuilder
    {
        #region SQL(FIXME)

        private const string SqlAttachFileMetaSearch = @$"
INSERT INTO ""{OracleDbConsts.AttachFileMetaSearchTableName}""
(
    ""id"",
    ""AttachFileMetaId"",
    ""MetaKey"",
    ""MetaValue"",
    ""_Reguser_Id"",
    ""_Regdate"",
    ""_Upduser_Id"",
    ""_Upddate""
)
SELECT
    NEWID(),
    ""id"",
    ""MetaKey"",
    ""MetaValue"",
    ""_Reguser_Id"",
    ""_Regdate"",
    ""_Upduser_Id"",
    ""_Upddate""
FROM ""{OracleDbConsts.AttachFileMetaTableName}"",
JSON_TABLE(""{OracleDbConsts.AttachFileMetaTableName}"".""MetaList"", '$[*]'
    COLUMNS (
        ""MetaKey""   VARCHAR(400) PATH '$.MetaKey',
        ""MetaValue"" VARCHAR(400) PATH '$.MetaValue'
    )
)
WHERE ""FileId"" = :FileId";

        #endregion

        private RegisterParam RegisterParam { get; }
        private RepositoryInfo RepositoryInfo { get; }
        private IContainerDynamicSeparationRepository ContainerDynamicSeparationRepository { get; }

        public string Sql { get; protected set; }
        public IRdbmsSqlParameterList SqlParameterList { get; protected set; }

        public bool HasPostAdditionalSqls { get; private set; } = false;
        public IEnumerable<(string Sql, IRdbmsSqlParameterList SqlParameterList)> PostAdditionalSqls { get; private set; }

        public OracleDbUpsertSqlBuilder(
            RegisterParam registerParam,
            RepositoryInfo repositoryInfo,
            IContainerDynamicSeparationRepository containerDynamicSeparationRepository)
        {
            RegisterParam = registerParam;
            RepositoryInfo = repositoryInfo;
            ContainerDynamicSeparationRepository = containerDynamicSeparationRepository;
        }


        public void BuildUp()
        {
            // テーブル取得
            string tableName;
            if (RegisterParam.OperationInfo?.IsNormalOperation ?? true)
            {
                tableName = ContainerDynamicSeparationRepository.GetOrRegisterContainerName(
                    RepositoryInfo.PhysicalRepositoryId,
                    RegisterParam.ControllerId,
                    new VendorId(Guid.Empty.ToString()),
                    new SystemId(Guid.Empty.ToString()));
            }
            else
            {
                switch (RegisterParam.OperationInfo.Type)
                {
                    case OperationInfo.OperationType.VersionInfo:
                        tableName = SqlServerVersionInfo.VersionTableName;
                        break;
                    case OperationInfo.OperationType.AttachFileMeta:
                        tableName = SqlServerConsts.AttachFileMetaTableName;
                        break;
                    case OperationInfo.OperationType.DocumentVersion:
                        tableName = SqlServerDocumentVersion.DocumentVersionTableName;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            // SQL生成
            (Sql, SqlParameterList) = GenerateRegisterSqlAndParameters(RegisterParam, tableName);

            // 添付ファイルメタの場合は検索用のテーブルの登録SQLを追加
            if (RegisterParam.OperationInfo?.IsAttachFileOperation == true)
            {
                SetAttachFileMetaAdditionalSql();
            }
        }


        /// <summary>
        /// JSONをMERGE文に変換する。
        /// </summary>
        private (string, IRdbmsSqlParameterList) GenerateRegisterSqlAndParameters(RegisterParam param, string tableName)
        {
            const string etagCurrent = "_etag_current";

            // パラメータ準備
            var parameters = PrepareRegisterParameters(param);

            // SQL生成
            var select = string.Join(", ", parameters.Select(x => $"{x.Value.SqlParameter} AS \"{x.Value.ParameterName}\""));
            var update = $"UPDATE SET {string.Join(", ", parameters.Where(x => x.Key != JsonPropertyConst.ID && x.Key != JsonPropertyConst.ETAG).Select(x => $"\"{x.Key}\" = X.\"{x.Value.ParameterName}\""))}, \"{JsonPropertyConst.ETAG}\" = \"{JsonPropertyConst.ETAG}\" + 1";
            var insert = $"INSERT ({string.Join(", ", parameters.Select(x => $"\"{x.Key}\""))}) VALUES ({string.Join(", ", parameters.Select(x => $"X.\"{x.Value.ParameterName}\""))})";

            var sql = new StringBuilder();
            sql.AppendLine($"MERGE INTO \"{tableName}\"");
            sql.AppendLine($"USING (");
            sql.AppendLine($"    SELECT {select} FROM dual");
            sql.AppendLine($") X ON (\"{tableName}\".\"{JsonPropertyConst.ID}\" = {parameters[JsonPropertyConst.ID].SqlParameter})");

            sql.Append($"WHEN MATCHED ");
            sql.AppendLine($"THEN");
            sql.AppendLine($"{update}");
            if (param.IsOptimisticConcurrency?.Value == true ||
                param.OperationInfo?.IsVersionOperation == true)
            {
                // 楽観排他、バージョン情報の場合は入力データのETAGをマッチング条件に追加
                long etagValue = 0;
                if (param.Json[JsonPropertyConst.ETAG] != null)
                {
                    _ = long.TryParse(param.Json[JsonPropertyConst.ETAG].Value<string>(), out etagValue);
                }
                parameters.Add(etagCurrent, etagValue);
                sql.AppendLine($"WHERE \"{tableName}\".\"{JsonPropertyConst.ETAG}\" = {parameters[etagCurrent].SqlParameter} ");
            }

            sql.AppendLine($"WHEN NOT MATCHED THEN");
            sql.AppendLine($"{insert}");

            return (sql.ToString(), parameters);
        }

        /// <summary>
        /// データ登録用パラメータを準備する。
        /// </summary>
        private IRdbmsSqlParameterList PrepareRegisterParameters(RegisterParam param)
        {
            JSchema schema;
            switch (param.OperationInfo?.Type)
            {
                case OperationInfo.OperationType.VersionInfo:
                    schema = OracleDbVersionInfo.VersionTableSchema;
                    break;
                case OperationInfo.OperationType.DocumentVersion:
                    schema = OracleDbDocumentVersion.DocumentVersionTableSchema;
                    break;
                default:
                    schema = param.ControllerSchema?.ToJSchema();
                    break;
            }
            var parameters = new OracleDbSqlParameterList();
            parameters.Add(JsonPropertyConst.ETAG, 1L);

            var dataProperties = param.Json.Children().Where(x => x.Type == JTokenType.Property).Select(x => (JProperty)x).ToList();
            foreach (var property in dataProperties)
            {
                // ETAGは個別に処理するため無視
                if (property.Name == JsonPropertyConst.ETAG)
                {
                    continue;
                }

                // スキーマに基づく変換処理
                var propertySchema = schema?.Properties.FirstOrDefault(x => x.Key == property.Name).Value;
                if (propertySchema != null)
                {
                    // 配列、オブジェクトは文字列として格納
                    if (!propertySchema.Type.IsFlatType())
                    {
                        parameters.Add(property.Name, property.Value.Type == JTokenType.Null ? null : property.Value.ToString());
                        continue;
                    }

                    // タイムゾーン指定ありのDateTimeはUTCに変換して格納
                    // タイムゾーン指定なしならそのまま格納
                    if (propertySchema.IsDateTimeFormat())
                    {
                        if (((JValue)property.Value).Value is DateTime dateTimeValue)
                        {
                            if (dateTimeValue.Kind == DateTimeKind.Local)
                            {
                                dateTimeValue = dateTimeValue.ToUniversalTime();
                            }

                            parameters.Add(property.Name, dateTimeValue);
                            continue;
                        }
                        else
                        {
                            parameters.Add(property.Name, (DateTime?)null);
                            continue;
                        }
                    }
                }
                // バージョン情報、履歴情報の場合はテーブル項目以外無視
                else if (param.OperationInfo?.IsVersionOperation == true ||
                    param.OperationInfo?.IsDocumentVersionOperation == true)
                {
                    continue;
                }

                parameters.Add(property.Name, ((JValue)property.Value).Value);
            }

            // 登録の場合、データに含まれない項目はnullに更新
            if (param.ActionType?.Value == ActionType.Regist)
            {
                foreach (var property in schema.Properties.Where(x => x.Key != JsonPropertyConst.ETAG && !dataProperties.Any(y => y.Name == x.Key)))
                {
                    parameters.Add(property.Key, null);
                }
            }

            // バージョン情報は全体をJSONとしてversioninfo列に格納
            if (param.OperationInfo?.IsVersionOperation == true)
            {
                // ETAGは個別の列で管理するため削除
                var versionInfo = param.Json.DeepClone();
                versionInfo.RemoveField(JsonPropertyConst.ETAG);

                parameters.Add(OracleDbVersionInfo.VersionInfoColumnName, versionInfo.ToString());
            }

            // 添付ファイル、履歴情報の場合はリソース識別用に_Typeを設定
            if (param.OperationInfo?.IsAttachFileOperation == true ||
                param.OperationInfo?.IsDocumentVersionOperation == true)
            {
                parameters.Add(JsonPropertyConst.TYPE, param.RepositoryKey.Type);
            }

            return parameters;
        }


        /// <summary>
        /// 添付ファイルメタ用の追加SQLを設定する。
        /// </summary>
        private void SetAttachFileMetaAdditionalSql()
        {
            var fileId = SqlParameterList.First(x => x.Key == "FileId").Value.ParameterValue;
            var parameters = new SqlServerSqlParameterList();
            parameters.Add("FileId", fileId, autoParameterName: false);

            HasPostAdditionalSqls = true;
            PostAdditionalSqls = new List<(string, IRdbmsSqlParameterList)>()
            {
                new (SqlAttachFileMetaSearch, parameters)
            };
        }
    }
}
