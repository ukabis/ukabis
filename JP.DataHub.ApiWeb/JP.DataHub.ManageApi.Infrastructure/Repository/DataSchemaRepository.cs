using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.DymamicApi;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class DataSchemaRepository : AbstractRepository, IDataSchemaRepository
    {
        private static readonly JPDataHubLogger logger = new JPDataHubLogger(typeof(DataSchemaRepository));

        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));


        /// <summary>
        /// 指定されたベンダーIDのDynamicAPIのスキーマの一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>
        /// 指定されたベンダーのDynamicAPIのスキーマの一覧。
        /// <paramref name="vendorId"/>がnullの場合は全ベンダーのDynamicAPIのスキーマの一覧。
        /// </returns>
        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_DATASCHEMA)]
        [CacheArg(allParam: true)]
        public IEnumerable<SchemaModel> GetSchemas(string vendorId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    data_schema_id as SchemaId
    ,schema_name as SchemaName
    ,vendor_id AS VendorId
    ,data_schema as JsonSchema
    ,schema_description as Description
    ,upd_date as UpdDate 
FROM
    DATA_SCHEMA 
WHERE
/*ds if vendor_id != null*/
    vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    is_active = 1 
ORDER BY
    SchemaName
";
            }
            else
            {
                sql = @"
SELECT
    data_schema_id as SchemaId
    ,schema_name as SchemaName
    ,vendor_id AS VendorId
    ,data_schema as JsonSchema
    ,schema_description as Description
    ,upd_date as UpdDate
FROM
    DataSchema
WHERE
/*ds if vendor_id != null*/
    vendor_id = @vendor_id AND
/*ds end if*/
    is_active = 1
ORDER BY
    SchemaName
";
            }
            var param = new { vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<SchemaModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// 指定されたベンダーIDのデータスキーマ情報を取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>データスキーマ情報の一覧</returns>
        public IEnumerable<DataSchemaInformationModel> GetDataSchemaImformationList(string vendorId)
        {
            //関連INDEX
            //SQLの修正をする際は関連INDEXの確認も行うこと
            //IDX_DataSchema_GetDataSchema

            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    ds.data_schema_id AS DataSchemaId
    ,ds.schema_name AS SchemaName
    ,ds.vendor_id AS VendorId
FROM 
    DATA_SCHEMA ds
    JOIN VENDOR v ON v.vendor_id = ds.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
/*ds if vendorId != null*/
    ds.vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    ds.is_active = 1
ORDER BY
    ds.schema_name
";
            }
            else
            {
                sql = @"
SELECT
    ds.data_schema_id AS DataSchemaId
    ,ds.schema_name AS SchemaName
    ,ds.vendor_id AS VendorId
FROM 
    DataSchema ds
    JOIN Vendor v ON v.vendor_id = ds.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
/*ds if vendorId != null*/
    ds.vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    ds.is_active = 1
ORDER BY
    ds.schema_name
";
            }

            var param = new { vendorId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<DataSchemaInformationModel>(twowaySql.Sql, dynParams).ToList();
        }

        /// <summary>
        /// 指定されたデータスキーマIDのデータスキーマ情報を取得します。
        /// </summary>
        /// <param name="dataSchemaId">データスキーマID</param>
        /// <returns>データスキーマ情報</returns>
        public DataSchemaInformationModel GetDataSchemaImformation(string dataSchemaId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    ds.data_schema_id AS DataSchemaId
    ,ds.schema_name AS SchemaName
    ,ds.vendor_id AS VendorId 
FROM 
    DATA_SCHEMA ds 
	JOIN VENDOR v ON v.vendor_id = ds.vendor_id AND v.is_enable = 1 AND v.is_active = 1 
WHERE
    ds.data_schema_id = /*ds dataSchemaId*/'00000000-0000-0000-0000-000000000000' 
    AND ds.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    ds.data_schema_id AS DataSchemaId
    ,ds.schema_name AS SchemaName
    ,ds.vendor_id AS VendorId
FROM 
    DataSchema ds
	JOIN Vendor v ON v.vendor_id = ds.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
    ds.data_schema_id = @dataSchemaId
    AND ds.is_active = 1
";
            }
            var param = new { dataSchemaId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingleOrDefault<DataSchemaInformationModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// 指定されたスキーマIDのDynamicAPIのスキーマの一件を取得します。
        /// </summary>
        /// <param name="dataSchemaId">スキーマID</param>
        /// <returns>DynamicAPIのスキーマ</returns>
        public SchemaModel GetDataSchemaById(string dataSchemaId)
        {
            if (string.IsNullOrEmpty(dataSchemaId))
            {
                return null;
            }
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    ds.data_schema_id as SchemaId
    ,ds.schema_name as SchemaName
    ,ds.vendor_id AS VendorId
    ,ds.data_schema as JsonSchema
    ,ds.schema_description as Description
    ,ds.upd_date as UpdDate
FROM 
    DATA_SCHEMA ds 
    INNER JOIN VENDOR v ON v.vendor_id = ds.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
    ds.data_schema_id = /*ds dataSchemaId*/'00000000-0000-0000-0000-000000000000' 
    AND ds.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    ds.data_schema_id as SchemaId
    ,ds.schema_name as SchemaName
    ,ds.vendor_id AS VendorId
    ,ds.data_schema as JsonSchema
    ,ds.schema_description as Description
    ,ds.upd_date as UpdDate
FROM
    DataSchema ds
    INNER JOIN Vendor v ON v.vendor_id = ds.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
    ds.data_schema_id = @dataSchemaId AND 
    ds.is_active = 1
";
            }
            var param = new { dataSchemaId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingleOrDefault<SchemaModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// 指定されたデータスキーマ名のデータスキーマ情報を取得します。
        /// </summary>
        /// <param name="dataSchemaId">データスキーマID</param>
        /// <param name="dataSchemaName">データスキーマ名</param>
        /// <returns>DynamicAPIのスキーマ</returns>
        public SchemaModel GetDataSchemaByName(string dataSchemaName)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    ds.data_schema_id as SchemaId
    ,ds.schema_name as SchemaName
    ,ds.vendor_id AS VendorId
    ,ds.data_schema as JsonSchema
    ,ds.schema_description as Description
    ,ds.upd_date as UpdDate 
FROM
    DATA_SCHEMA ds
    INNER JOIN VENDOR v ON v.vendor_id = ds.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
    ds.schema_name = /*ds dataSchemaName*/'a' AND 
    ds.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    ds.data_schema_id as SchemaId
    ,ds.schema_name as SchemaName
    ,ds.vendor_id AS VendorId
    ,ds.data_schema as JsonSchema
    ,ds.schema_description as Description
    ,ds.upd_date as UpdDate
FROM 
    DataSchema ds
    INNER JOIN Vendor v ON v.vendor_id = ds.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
    ds.schema_name = @dataSchemaName AND
    ds.is_active = 1
";
            }
            var param = new { dataSchemaName };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingleOrDefault<SchemaModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// データスキーマを取得します。
        /// </summary>
        /// <param name="dataSchemaId">データスキーマID</param>
        /// <returns>データスキーマ</returns>
        public DataSchema GetDataSchema(string dataSchemaId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    data_schema 
FROM
    DATA_SCHEMA d 
WHERE
    d.data_schema_id = /*ds dataSchemaId*/'00000000-0000-0000-0000-000000000000' 
AND d.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    data_schema
FROM
    DataSchema d
WHERE
    d.data_schema_id = @dataSchemaId
AND d.is_active = 1
";
            }
            var param = new { dataSchemaId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var schema = Connection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
            return new DataSchema(schema);
        }

        /// <summary>
        /// コントローラIDに紐づくデータスキーマIDを取得します。
        /// </summary>
        /// <param name="controllerId"></param>
        /// <returns>データスキーマID</returns>
        public string GetDataSchemaIdByControllerId(string controllerId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    c.controller_schema_id 
FROM
    CONTROLLER c 
WHERE
    c.controller_id = /*ds controllerId*/'00000000-0000-0000-0000-000000000000' 
    AND c.is_active = 1 
";
            }
            else
            {
                sql = @"
SELECT 
    c.controller_schema_id
FROM
    Controller c
WHERE
    c.controller_id = @controllerId
    AND c.is_active = 1 
";
            }
            var param = new { controllerId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// データスキーマ情報を登録または更新します。
        /// </summary>
        /// <param name="model">データスキーマ情報</param>
        /// <returns>データスキーマ情報</returns>
        [CacheIdFire("DataSchemaId", "model.DataSchemaId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_DATASCHEMA)]
        [DomainDataSync(DynamicApiDatabase.TABLE_DATASCHEMA, "model.DataSchemaId")]
        public DataSchemaInformationModel UpsertSchema(DataSchemaInformationModel model)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO DATA_SCHEMA target 
USING 
(
    SELECT 
        /*ds DataSchemaId*/'00000000-0000-0000-0000-000000000000' AS data_schema_id FROM DUAL
) source 
ON 
    (target.data_schema_id = source.data_schema_id ) 
WHEN MATCHED THEN 
    UPDATE 
    SET 
        schema_name = /*ds SchemaName*/'a' 
        ,vendor_id = /*ds VendorId*/'00000000-0000-0000-0000-000000000000' 
        ,data_schema = /*ds DataSchema*/'nclob' 
        ,schema_description = /*ds DataSchemaDescription*/'a' 
        ,upd_date = /*ds UpdDate*/SYSTIMESTAMP 
        ,upd_username = /*ds UpdUserName*/'a' 
        ,is_active = /*ds IsActive*/1 
WHEN NOT MATCHED THEN 
    INSERT 
    (
        data_schema_id
        ,schema_name
        ,vendor_id
        ,data_schema
        ,schema_description
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    ) 
    VALUES 
    (
        /*ds DataSchemaId*/'00000000-0000-0000-0000-000000000000' 
        ,/*ds SchemaName*/'a' 
        ,/*ds VendorId*/'00000000-0000-0000-0000-000000000000' 
        ,/*ds DataSchema*/'nclob' 
        ,/*ds DataSchemaDescription*/'a' 
        ,/*ds RegDate*/SYSTIMESTAMP 
        ,/*ds RegUserName*/'a' 
        ,/*ds UpdDate*/SYSTIMESTAMP 
        ,/*ds UpdUserName*/'a' 
        ,/*ds IsActive*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO DataSchema AS target
USING
(
    SELECT
        @DataSchemaId AS data_schema_id
) AS source
ON
    target.data_schema_id = source.data_schema_id
WHEN MATCHED THEN
    UPDATE
    SET
        schema_name = @SchemaName
        ,vendor_id = @VendorId
        ,data_schema = @DataSchema
        ,schema_description = @DataSchemaDescription
        ,upd_date = @UpdDate
        ,upd_username = @UpdUserName
        ,is_active = @IsActive
WHEN NOT MATCHED THEN
    INSERT
    (
        data_schema_id
        ,schema_name
        ,vendor_id
        ,data_schema
        ,schema_description
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        @DataSchemaId
        ,@SchemaName
        ,@VendorId
        ,@DataSchema
        ,@DataSchemaDescription
        ,@RegDate
        ,@RegUserName
        ,@UpdDate
        ,@UpdUserName
        ,@IsActive
    );
";
            }
            var param = new
            {
                model.DataSchemaId,
                model.SchemaName,
                model.VendorId,
                model.DataSchema,
                model.DataSchemaDescription,
                RegDate = UtcNow,
                RegUserName = PerRequestDataContainer.OpenId,
                UpdDate = UtcNow,
                UpdUserName = PerRequestDataContainer.OpenId,
                IsActive = true
            };

            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParam = dbsettings.GetParameters().AddDynamicParams(param).SetNClob(nameof(param.DataSchema));
            try
            {
                Connection.ExecutePrimaryKey(twowaySql.Sql, dynParam);
                return model;
            }
            catch (SqlException ex)
            {
                logger.Error($"Sql Error: {ex.Number} {ex.Message}", ex);
                // UNIQUE制約違反の場合、独自例外を返す
                throw ex.Number == 2627 ? new AlreadyExistsException(ex.Message) : new SqlDatabaseException(ex.Message);
            }
        }

        /// <summary>
        /// 指定されたデータスキーマIDのデータスキーマ情報を削除します。
        /// </summary>
        /// <param name="dataSchemaId">データスキーマID</param>
        [CacheIdFire("DataSchemaId", "dataSchemaId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_DATASCHEMA)]
        [DomainDataSync(DynamicApiDatabase.TABLE_DATASCHEMA, "dataSchemaId")]
        public void DeleteDataSchema(string dataSchemaId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
UPDATE DATA_SCHEMA 
   SET
       is_active = 0
      ,upd_date = /*ds UpdDate*/SYSTIMESTAMP 
      ,upd_username = /*ds UpdUserName*/'a' 
 WHERE 
    data_schema_id = /*ds dataSchemaId*/'00000000-0000-0000-0000-000000000000' 
 AND is_active = 1
";
            }
            else
            {
                sql = @"
UPDATE DataSchema
   SET
       is_active = 0
      ,upd_date = @UpdDate
      ,upd_username = @UpdUserName
 WHERE
    data_schema_id = @dataSchemaId
 AND is_active = 1
";
            }
            var param = new
            {
                dataSchemaId,
                UpdDate = UtcNow,
                UpdUserName = PerRequestDataContainer.OpenId
            };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// DataSchema名重複チェック
        /// </summary>
        /// <param name="schemaName">スキーマ名</param>
        /// <returns>true:登録済/false:未登録</returns>
        public bool ExistsSameSchemaName(string schemaName)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(data_schema_id)
FROM
    DATA_SCHEMA /*WITH(UPDLOCK)*/
WHERE
    schema_name = /*ds schemaName*/'a' 
    AND is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(data_schema_id)
FROM
    DataSchema WITH(UPDLOCK)
WHERE
    schema_name = @schemaName
    AND is_active = 1
";
            }
            var param = new { schemaName };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            return result != 0;
        }

        /// <summary>
        /// 引数のDataSchemeIdを使用しているApiが存在しているか
        /// </summary>
        /// <param name="dataSchemaId">DataSchemaId</param>
        /// <returns>使用している場合はtrue、していない場合はfalseを返す</returns>
        public bool IsUsedFromApi(string dataSchemaId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(api_id) AS api_count
FROM
    API a /*WITH(UPDLOCK)*/ ,
    CONTROLLER c /*WITH(UPDLOCK)*/ 
WHERE
    a.controller_id = c.controller_id
    AND a.is_active = 1
    AND c.is_active = 1
    AND
    (
        a.url_schema_id = /*ds dataSchemaId*/'00000000-0000-0000-0000-000000000000' 
    OR  a.request_schema_id = /*ds dataSchemaId*/'00000000-0000-0000-0000-000000000000' 
    OR  a.response_schema_id = /*ds dataSchemaId*/'00000000-0000-0000-0000-000000000000' 
    )
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(api_id) AS api_count
FROM
    Api a WITH(UPDLOCK),
    Controller c WITH(UPDLOCK)
WHERE
    a.controller_id = c.controller_id
    AND a.is_active = 1
    AND c.is_active = 1
    AND
    (
        a.url_schema_id = @dataSchemaId
    OR  a.request_schema_id = @dataSchemaId
    OR  a.response_schema_id = @dataSchemaId
    )
";
            }
            var param = new { dataSchemaId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var apiCount = Connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            return apiCount != 0;
        }

        /// <summary>
        /// 引数のDataSchemaIdを使用しているControllerが存在しているか
        /// </summary>
        /// <param name="dataSchemaId">DataSchemaId</param>
        /// <returns>使用している場合はtrue、していない場合はfalseを返す</returns>
        public bool IsUsedFromController(string dataSchemaId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(c.controller_id) AS controller_count
FROM
    CONTROLLER c /*WITH(UPDLOCK)*/ 
WHERE
    c.controller_schema_id = /*ds dataSchemaId*/'00000000-0000-0000-0000-000000000000' 
    AND c.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(c.controller_id) AS controller_count
FROM
    Controller c WITH(UPDLOCK)
WHERE
    c.controller_schema_id = @dataSchemaId
    AND c.is_active = 1
";
            }
            var param = new { dataSchemaId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var controllerCount = Connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            return controllerCount != 0;
        }
    }
}
