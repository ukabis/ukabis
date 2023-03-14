using System;
using System.Text;
using System.Text.RegularExpressions;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.Consts;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.ResourceSchemaAdapter;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Extensions.SqlServer;

namespace JP.DataHub.ApiWeb.Infrastructure.ResourceSchemaAdapter
{
    //    ドメイン：スキーマバリデーションのIF、変更対象列の抽出、抽象化された列定義
    //    インフラ：スキーマバリデーションの実装、SQL生成、列定義のデータソース固有型への変換
    internal class SqlServerResourceSchemaAdapter : IResourceSchemaAdapter
    {
        /// <summary>
        /// PK列
        /// </summary>
        private static readonly ColumnInfo PrimaryKey =
            new ColumnInfo() { Column = new RdbmsTableColumn(JsonPropertyConst.ID, $"nvarchar({SqlServerConsts.PrimaryKeyTableColumnMaxLength})", false), Index = false };

        /// <summary>
        /// デフォルト列
        /// </summary>
        private static readonly List<ColumnInfo> ManagementColumns = new List<ColumnInfo>()
        {
            new ColumnInfo() { Column = new RdbmsTableColumn(JsonPropertyConst.VERSION_COLNAME, "int", false), Index = true },
            new ColumnInfo() { Column = new RdbmsTableColumn(JsonPropertyConst.VENDORID, $"uniqueidentifier", true), Index = true },
            new ColumnInfo() { Column = new RdbmsTableColumn(JsonPropertyConst.SYSTEMID, $"uniqueidentifier", true), Index = true },
            new ColumnInfo() { Column = new RdbmsTableColumn(JsonPropertyConst.OWNERID, $"uniqueidentifier", true), Index = true },
            new ColumnInfo() { Column = new RdbmsTableColumn(JsonPropertyConst.OPENID, $"uniqueidentifier", true), Index = true },
            new ColumnInfo() { Column = new RdbmsTableColumn(JsonPropertyConst.REGDATE, "datetime", false), Index = true },
            new ColumnInfo() { Column = new RdbmsTableColumn(JsonPropertyConst.UPDUSERID, $"uniqueidentifier", true), Index = true },
            new ColumnInfo() { Column = new RdbmsTableColumn(JsonPropertyConst.UPDDATE, "datetime", false), Index = true },
            new ColumnInfo() { Column = new RdbmsTableColumn(JsonPropertyConst.ETAG, "int", false), Index = false }
        };

        private readonly JPDataHubLogger Log = new JPDataHubLogger(typeof(SqlServerResourceSchemaAdapter));


        protected IContainerDynamicSeparationRepository ContainerDynamicSeparationRepository => s_containerDynamicSeparationRepository.Value;
        private Lazy<IContainerDynamicSeparationRepository> s_containerDynamicSeparationRepository = 
            new Lazy<IContainerDynamicSeparationRepository>(() => UnityCore.Resolve<IContainerDynamicSeparationRepository>());

        private INewDynamicApiDataStoreRdbmsRepository DataStoreRepository { get; }

        private ControllerId ControllerId { get; }
        private JSchema ControllerSchema { get; }

        private string TableName { get; set; }
        private List<RdbmsTableColumn> TableColumns { get; set; }


        public SqlServerResourceSchemaAdapter(RepositoryInfo repositoryInfo, ControllerId controllerId, DataSchema controllerSchema)
        {
            ControllerId = controllerId;
            ControllerSchema = controllerSchema?.ToJSchema();
            DataStoreRepository = UnityCore.Resolve<INewDynamicApiDataStoreRdbmsRepository>(repositoryInfo.Type.ToCode());
            DataStoreRepository.RepositoryInfo = repositoryInfo;
        }


        /// <summary>
        /// リソーススキーマに対応できるかどうか。
        /// </summary>
        public bool IsAdaptable(out RFC7807ProblemDetailExtendErrors problemDetail)
        {
            problemDetail = null;
            var errors = new List<(string PropertyName, string Message)>();

            // リソースモデル必須
            if (ControllerSchema == null)
            {
                problemDetail = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E50409);
                Log.Info($"Resource schema is not adaptable. (ControllerId: {ControllerId.Value}, Error: {problemDetail.ErrorCode})");
                return false;
            }

            // AllowAdditionalProperties不可
            if (ControllerSchema.AllowAdditionalProperties)
            {
                errors.Add((string.Empty, ErrorCodeMessage.GetString(nameof(DynamicApiMessages.ResourceSchemaNotAdaptable_AllowAdditionalProperties))));
            }

            // 複合スキーマチェック
            if (ControllerSchema.AllOf.Count > 0 ||
                ControllerSchema.AnyOf.Count > 0 ||
                ControllerSchema.OneOf.Count > 0)
            {
                errors.Add((string.Empty, ErrorCodeMessage.GetString(nameof(DynamicApiMessages.ResourceSchemaNotAdaptable_CombiningSchemas))));
            }

            // 列数チェック
            var propertiesCount = ControllerSchema.Properties.Count();
            if (propertiesCount <= 0)
            {
                errors.Add((string.Empty, ErrorCodeMessage.GetString(nameof(DynamicApiMessages.ResourceSchemaNotAdaptable_NoProperties))));
            }
            if (propertiesCount > DataStoreRepository.TableMaxColumns)
            {
                errors.Add((string.Empty, ErrorCodeMessage.GetString(nameof(DynamicApiMessages.ResourceSchemaNotAdaptable_TooMatchProperties))));
            }

            // 列別チェック
            var columnNamePattern = new Regex(DataStoreRepository.TableColumnNamePattern);
            foreach (var property in ControllerSchema.Properties)
            {
                if (Encoding.UTF8.GetByteCount(property.Key) > DataStoreRepository.TableColumnNameMaxBytes)
                {
                    errors.Add((property.Key, ErrorCodeMessage.GetString(nameof(DynamicApiMessages.ResourceSchemaNotAdaptable_PropertyNameTooLong))));
                }
                if (!(columnNamePattern.IsMatch(property.Key)))
                {
                    errors.Add((property.Key, ErrorCodeMessage.GetString(nameof(DynamicApiMessages.ResourceSchemaNotAdaptable_InvalidPropertyName))));
                }
                if (property.Value.Type == null || property.Value.Type == JSchemaType.Null || property.Value.Type == JSchemaType.None)
                {
                    errors.Add((property.Key, ErrorCodeMessage.GetString(nameof(DynamicApiMessages.ResourceSchemaNotAdaptable_PropertyTypeRequired))));
                }
            }

            // 既存プロパティ変更不可
            TableName = GetOrRegisterTableName(DataStoreRepository.RepositoryInfo.PhysicalRepositoryId, ControllerId);
            TableColumns = DataStoreRepository.GetTableColumns(TableName).ToList();
            var modifiedColumns = TableColumns.Where(x =>
                !(PrimaryKey.Column.Name == x.Name) &&
                !ManagementColumns.Any(y => x.Name == y.Column.Name) &&
                !ControllerSchema.Properties.Any(y => x.Name == y.Key && x.DataType.ToUpper() == y.Value.GetSqlServerDataType(true).ToUpper())).ToList();
            if (modifiedColumns.Any())
            {
                var msg = ErrorCodeMessage.GetString(nameof(DynamicApiMessages.ResourceSchemaNotAdaptable_ModifyExistingProperties));
                modifiedColumns.ForEach(x => errors.Add((x.Name, msg)));
            }

            if (errors.Count > 0)
            {
                problemDetail = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E50410);
                problemDetail.Errors = errors.GroupBy(x => x.PropertyName).ToDictionary(x => x.Key, y => y.Select(z => z.Message).ToArray() as dynamic);
                Log.Info($"Resource schema is not adaptable. (ControllerId: {ControllerId.Value}, Error: {problemDetail.ErrorCode}, Details: {string.Join("/", errors.Select(x => $"{x.PropertyName}={x.Message}"))})");
                return false;
            }

            return true;
        }

        /// <summary>
        /// リソーススキーマを適用する。
        /// </summary>
        public void Adapt()
        {
            if (TableName == null)
            {
                TableName = GetOrRegisterTableName(DataStoreRepository.RepositoryInfo.PhysicalRepositoryId, ControllerId);
            }
            if (TableColumns == null)
            {
                TableColumns = DataStoreRepository.GetTableColumns(TableName).ToList();
            }

            // 管理項目が含まれる場合は除去(管理項目として扱う)
            var schema = ControllerSchema;
            ManagementColumns.ForEach(x =>
            {
                if (schema.Properties.Any(y => y.Key == x.Column.Name))
                {
                    schema.Properties.Remove(x.Column.Name);
                }
            });

            // DDL実行
            if (TableColumns.Count == 0)
            {
                CreateTable(TableName, schema);
            }
            else
            {
                AlterTable(TableName, TableColumns, schema);
            }
        }


        /// <summary>
        /// テーブル名を取得する。未登録の場合は登録する。
        /// </summary>
        private string GetOrRegisterTableName(PhysicalRepositoryId physicalRepositoryId, ControllerId controllerId)
        {
            return ContainerDynamicSeparationRepository.GetOrRegisterContainerName(
                physicalRepositoryId, controllerId, new VendorId(Guid.Empty.ToString()), new SystemId(Guid.Empty.ToString()));
        }

        /// <summary>
        /// テーブル作成、インデックス作成を行う。
        /// </summary>
        private void CreateTable(string tableName, JSchema resourceSchema)
        {
            Log.Info($"CREATE TABLE and INDEX: {ControllerId.Value}");

            // CREATE TABLE
            var columns = new List<RdbmsTableColumn>();
            columns.Add(PrimaryKey.Column);
            columns.AddRange(resourceSchema.Properties.Select(x => new RdbmsTableColumn(x.Key, x.Value.GetSqlServerDataType(), true)));
            columns.AddRange(ManagementColumns.Select(x => x.Column));
            DataStoreRepository.CreateTable(tableName, columns, new string[] { PrimaryKey.Column.Name });

            // CREATE INDEX(nvarchar(MAX)はインデックス作成不可のため除外)
            var indexingColumns = ManagementColumns.Where(x => x.Index).Select(x => x.Column.Name).ToList();
            indexingColumns.AddRange(resourceSchema.Properties.Where(x => !x.Value.GetSqlServerDataType().Contains("MAX")).Select(x => x.Key).ToList());
            indexingColumns.ForEach(x => DataStoreRepository.CreateIndex(tableName, x));
        }

        /// <summary>
        /// テーブル更新、インデックス作成を行う。
        /// </summary>
        private void AlterTable(string tableName, List<RdbmsTableColumn> tableColumns, JSchema resourceSchema)
        {
            // 追加列抽出
            var additionalColumns = resourceSchema.Properties
                .Where(x =>
                    PrimaryKey.Column.Name != x.Key &&
                    !ManagementColumns.Any(y => x.Key == y.Column.Name) &&
                    !tableColumns.Any(y => x.Key == y.Name && x.Value.GetSqlServerDataType(true).ToLower() == y.DataType.ToLower()))
                .Select(x => new RdbmsTableColumn(x.Key, x.Value.GetSqlServerDataType(), true)).ToList();
            if (!additionalColumns.Any())
            {
                return;
            }

            Log.Info($"ALTER TABLE and INDEX: {ControllerId.Value}");

            // ALTER TABLE
            DataStoreRepository.AddTableColumns(tableName, additionalColumns);

            // CREATE INDEX(nvarchar(MAX)はインデックス作成不可のため除外)
            var indexingColumns = additionalColumns.Where(x => !x.DataType.Contains("MAX")).Select(x => x.Name).ToList();
            indexingColumns.ForEach(x => DataStoreRepository.CreateIndex(tableName, x));
        }


        #region inner classes

        private class ColumnInfo
        {
            public RdbmsTableColumn Column { get; set; }
            public bool Index { get; set; }
        }

        #endregion
    }
}