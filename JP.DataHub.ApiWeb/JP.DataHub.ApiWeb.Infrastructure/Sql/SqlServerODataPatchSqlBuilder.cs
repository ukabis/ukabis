using System.Text;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Infrastructure.Database.Consts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    internal class SqlServerODataPatchSqlBuilder : IODataPatchSqlBuilder
    {
        private const string AdditionalConditionPropertyName = "_Where";

        private QueryParam QueryParam { get; }
        private RepositoryInfo RepositoryInfo { get; }
        private IContainerDynamicSeparationRepository ContainerDynamicSeparationRepository { get; }
        private JToken PatchData { get; }

        public string Sql { get; protected set; }
        public IRdbmsSqlParameterList SqlParameterList { get; protected set; }


        public SqlServerODataPatchSqlBuilder(
            QueryParam queryParam,
            RepositoryInfo repositoryInfo,
            IContainerDynamicSeparationRepository containerDynamicSeparationRepository,
            JToken patchData)
        {
            QueryParam = queryParam;
            RepositoryInfo = repositoryInfo;
            ContainerDynamicSeparationRepository = containerDynamicSeparationRepository;
            PatchData = patchData;
        }


        public void BuildUp()
        {
            // テーブル取得
            var tableName = ContainerDynamicSeparationRepository.GetOrRegisterContainerName(
                RepositoryInfo.PhysicalRepositoryId,
                QueryParam.ControllerId,
                new VendorId(Guid.Empty.ToString()),
                new SystemId(Guid.Empty.ToString()));

            // SQL生成
            (Sql, SqlParameterList) = GenerateUpdateSqlAndParameters(QueryParam, tableName, PatchData);
        }


        /// <summary>
        /// 一括UPDATE文を生成する。
        /// </summary>
        private (string, IRdbmsSqlParameterList) GenerateUpdateSqlAndParameters(QueryParam param, string tableName, JToken patchData)
        {
            // パラメータ準備
            var parameters = PrepareUpdateParameters(param, patchData);

            // SQL生成
            var sql = new StringBuilder();
            sql.AppendLine($"UPDATE \"{tableName}\"");
            sql.AppendLine($"SET {string.Join(", ", parameters.Select(x => $"\"{x.Key}\" = {x.Value.SqlParameter}"))}, \"{JsonPropertyConst.ETAG}\" = \"{JsonPropertyConst.ETAG}\" + 1");

            // WHERE条件
            var whereCondition = param.NativeQuery.Sql.Split(new string[] { " WHERE " }, StringSplitOptions.None)[1];
            parameters.AddRange(param.NativeQuery.Dic);

            // Ocha専用の隠し機能としてリクエストBodyで追加条件を指定可能
            // ODataのfilterでIN句が使用できないためその代替手段
            if (patchData.IsExistProperty(AdditionalConditionPropertyName))
            {
                try
                {
                    var (additionalConditionStr, additionalConditionParams) = BuildAdditionalWhereCondition(param, patchData[AdditionalConditionPropertyName]);
                    if (!string.IsNullOrEmpty(additionalConditionStr))
                    {
                        whereCondition += $" AND {additionalConditionStr}";
                        parameters.AddRange(additionalConditionParams);
                    }
                }
                catch (Exception ex)
                {
                    var error = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10440);
                    error.Errors = new Dictionary<string, dynamic>() { { AdditionalConditionPropertyName, new List<string> { ex.Message.Replace("\r", "").Replace("\n", "") } } };
                    throw new Rfc7807Exception(error);
                }
            }

            sql.AppendLine($"WHERE {whereCondition}");

            return (sql.ToString(), parameters);
        }

        /// <summary>
        /// データ更新用パラメータを準備する。
        /// </summary>
        private IRdbmsSqlParameterList PrepareUpdateParameters(QueryParam param, JToken patchData)
        {
            var parameters = new SqlServerSqlParameterList("odp");
            var schema = param.ControllerSchema?.ToJSchema();

            // リクエストBodyの項目
            var dataProperties = patchData.Children().Where(x => x.Type == JTokenType.Property).Select(x => (JProperty)x).ToList();
            foreach (var property in dataProperties)
            {
                if (property.Name == AdditionalConditionPropertyName)
                {
                    continue;
                }

                var propertySchema = schema?.Properties.FirstOrDefault(x => x.Key == property.Name).Value;
                if (propertySchema != null)
                {
                    if (!propertySchema.Type.IsFlatType())
                    {
                        // 配列、オブジェクトは文字列として格納
                        parameters.Add(property.Name, property.Value.Type == JTokenType.Null ? null : property.Value.ToString());
                        continue;
                    }
                    else if (propertySchema.IsDateTimeFormat())
                    {
                        // タイムゾーン指定ありのDateTimeはUTCに変換して格納
                        // タイムゾーン指定なしならそのまま格納
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

                parameters.Add(property.Name, ((JValue)property.Value).Value);
            }

            return parameters;
        }

        /// <summary>
        /// リクエストBodyの追加条件をWHERE句の条件に変換する。
        /// </summary>
        /// <remarks>
        /// Ocha向けにIN条件のみ対応
        /// </remarks>
        private (string, Dictionary<string, object>) BuildAdditionalWhereCondition(QueryParam param, JToken conditionJson)
        {
            var condition = JsonConvert.DeserializeObject<ODataPatchAdditionalCondition>(conditionJson?.ToString());
            var parameters = new Dictionary<string, object>();
            for (var i = 1; i <= condition.Object.Count; i++)
            {
                parameters.Add($"@odpex_{i}", condition.Object[i - 1]);
            }
            var sql = $"\"{condition.ColumnName}\" {condition.Operator} ({string.Join(",", parameters.Keys)})";

            return (sql, parameters);
        }
    }
}
