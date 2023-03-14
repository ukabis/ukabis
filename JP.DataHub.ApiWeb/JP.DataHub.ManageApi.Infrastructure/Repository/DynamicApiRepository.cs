using System.Data.SqlClient;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.Consts;
using JP.DataHub.Infrastructure.Database.Data.MongoDb;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.ManageApi.Infrastructure.Repository.Model;
using JP.DataHub.ManageApi.Service.DymamicApi.Actions.AllApi;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.CodeAnalysis;
using JP.DataHub.Com.Sql;
using Dapper.Oracle;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal partial class DynamicApiRepository : AbstractRepository, IDynamicApiRepository
    {
        private static readonly JPDataHubLogger logger = new(typeof(DynamicApiRepository));

        private const string RepositoryTypeCdDataLakeStore = "dls";
        private const string DefaultQueryType = "cdb";

        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));

        private string DefaultLocaleCode { get; } = UnityCore.Resolve<string>("DefaultCulture");

        /// <summary>
        /// DynamicAPIのカテゴリーの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIのカテゴリーの一覧</returns>
        public IEnumerable<CategoryQueryModel> GetCategories()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    category_id as CategoryId
    ,category_name as CategoryName
FROM
    CATEGORY 
WHERE
    is_active = 1 
ORDER BY
    sort_order
";
            }
            else
            {
                sql = @"
SELECT
    category_id as CategoryId
    ,category_name as CategoryName
FROM
    Category 
WHERE
    is_active = 1
ORDER BY
    sort_order
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            return Connection.Query<CategoryQueryModel>(twowaySql.Sql);
        }

        /// <summary>
        /// 分野の一覧を取得します。
        /// </summary>
        /// <returns>分野の一覧</returns>
        public IList<FieldQueryModel> GetFields()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    field_id as FieldId
    ,parent_field_id as ParentFieldId
    ,field_name as FieldName 
FROM
    FIELD 
WHERE
    is_active = 1 
ORDER BY
    field_id
";
            }
            else
            {
                sql = @"
SELECT 
    field_id as FieldId
    ,parent_field_id as ParentFieldId
    ,field_name as FieldName
FROM
    Field
WHERE
    is_active = 1 
ORDER BY
    field_id
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            return Connection.Query<FieldQueryModel>(twowaySql.Sql).ToList();
        }

        /// <summary>
        /// Tagの一覧を取得します。
        /// </summary>
        /// <returns>Tagの一覧</returns>
        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_TAG, DynamicApiDatabase.TABLE_TAGTYPE)]
        public IEnumerable<TagQueryModel> GetTags()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    tg.tag_id as TagId
    ,tg.tag_code as TagCode
    ,tg.tag_code2 as TagCode2
    ,tg.tag_name as TagName
    ,tg.parent_tag_id as ParentTagId
    ,tt.tag_type_id as TagTypeId
    ,tt.tag_type_name as TagTypeName
    ,tt.detail as TagTypeDetail
FROM
    TAG tg
    INNER JOIN TAG_TYPE tt ON tg.tag_type_id = tt.tag_type_id AND tt.is_active = 1
WHERE
    tg.is_active = 1 
ORDER BY
    tg.tag_code, tg.tag_code2
";
            }
            else
            {
                sql = @"
SELECT 
    tg.tag_id as TagId
    ,tg.tag_code as TagCode
    ,tg.tag_code2 as TagCode2
    ,tg.tag_name as TagName
    ,tg.parent_tag_id as ParentTagId
    ,tt.tag_type_id as TagTypeId
    ,tt.tag_type_name as TagTypeName
    ,tt.detail as TagTypeDetail
FROM
    Tag tg
    INNER JOIN TagType tt ON tg.tag_type_id = tt.tag_type_id AND tt.is_active = 1
WHERE
    tg.is_active = 1 
ORDER BY
    tg.tag_code, tg.tag_code2
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            return Connection.Query<TagQueryModel>(twowaySql.Sql);
        }

        /// <summary>
        /// タグ情報リストを取得します。
        /// </summary>
        /// <returns>タグ情報リスト</returns>
        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_TAG)]
        public IList<TagInfoModel> GetTagInfoList()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    tag_id AS TagId
    ,tag_code AS TagCode1
    ,tag_code2 AS TagCode2
    ,tag_name AS TagName
    ,parent_tag_id AS ParentTagId
FROM
    TAG
WHERE
    is_active = 1
ORDER BY
    tag_code,
    tag_code2
";
            }
            else
            {
                sql = @"
SELECT
    tag_id AS TagId
    ,tag_code AS TagCode1
    ,tag_code2 AS TagCode2
    ,tag_name AS TagName
    ,parent_tag_id AS ParentTagId
FROM
    Tag
WHERE
    is_active = 1
ORDER BY
    tag_code,
    tag_code2
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            return Connection.Query<TagInfoModel>(twowaySql.Sql)?.ToList();
        }

        /// <summary>
        /// ActionTypeの一覧を取得します。
        /// </summary>
        /// <returns>HttpMethodTypeの一覧</returns>
        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_ACTIONTYPE)]
        public IEnumerable<ActionTypeModel> GetActionTypes()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    action_type_cd as ActionTypeCd
    ,action_type_name as ActionTypeName
    ,is_visible as IsVisible
FROM
    ACTION_TYPE 
WHERE
    is_active = 1 
ORDER BY
    reg_date
";
            }
            else
            {
                sql = @"
SELECT
    action_type_cd as ActionTypeCd
    ,action_type_name as ActionTypeName
    ,is_visible as IsVisible
FROM
    ActionType 
WHERE
    is_active = 1 
ORDER BY
    reg_date
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            return Connection.Query<ActionTypeModel>(twowaySql.Sql);
        }

        /// <summary>
        /// HttpMethodTypeの一覧を取得します。
        /// </summary>
        /// <returns>HttpMethodTypeの一覧</returns>
        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_HTTPMETHODTYPE)]
        public IEnumerable<HttpMethodTypeModel> GetHttpMethodTypes()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    http_method_type_code as HttpMethodTypeCode
FROM
    HTTP_METHOD_TYPE 
WHERE
    is_enable = 1
    AND is_active = 1 
ORDER BY
    sort_no
";
            }
            else
            {
                sql = @"
SELECT
    http_method_type_code as HttpMethodTypeCode
FROM
    HttpMethodType 
WHERE
    is_enable = 1
    AND is_active = 1 
ORDER BY
    sort_no
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            return Connection.Query<HttpMethodTypeModel>(twowaySql.Sql);
        }

        /// <summary>
        /// 言語リストを取得します。
        /// </summary>
        /// <returns>言語リスト</returns>
        [CacheEntity(DynamicApiDatabase.TABLE_LANGUAGE)]
        [Cache]
        public IList<LanguageModel> GetLanguageList()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {

                sql = @"
SELECT
    language_id AS LanguageId
    ,language_name AS LanguageName
FROM
    LANGUAGE
WHERE
    is_active = 1
ORDER BY
    order_no
";
            }
            else
            {
                sql = @"
SELECT
    language_id AS LanguageId
    ,language_name AS LanguageName
FROM
    Language
WHERE
    is_active = 1
ORDER BY
    order_no
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            var result = Connection.Query<LanguageModel>(twowaySql.Sql);
            if (result == null || !result.Any())
                throw new NotFoundException("Not Found Language");

            return result.ToList();
        }

        /// <summary>
        /// スクリプトタイプリストを取得します。
        /// </summary>
        /// <returns>スクリプトタイプリスト</returns>
        [CacheEntity(DynamicApiDatabase.TABLE_SCRIPTTYPE)]
        [Cache]
        public IList<ScriptTypeModel> GetScriptTypeList()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    script_type_cd AS ScriptTypeCd
    ,script_type_name AS ScriptTypeName
FROM
    SCRIPT_TYPE
WHERE
    is_active=1
ORDER BY
    sort_no
";
            }
            else
            {
                sql = @"
SELECT
    script_type_cd AS ScriptTypeCd
    ,script_type_name AS ScriptTypeName
FROM
    ScriptType
WHERE
    is_active=1
ORDER BY
    sort_no
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            var result = Connection.Query<ScriptTypeModel>(twowaySql.Sql);
            if (result == null || !result.Any())
                throw new NotFoundException("Not Found Script Type");

            return result.ToList();
        }

        /// <summary>
        /// クエリタイプリストを取得します。
        /// </summary>
        /// <returns>クエリタイプリスト</returns>
        [CacheEntity(DynamicApiDatabase.TABLE_QUERYTYPE)]
        [Cache]
        public IList<QueryTypeModel> GetQueryTypeList()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    query_type_cd AS QueryTypeCd
    ,query_type_name AS QueryTypeName
FROM
    QUERY_TYPE
WHERE
    is_active=1
ORDER BY
    sort_no
";
            }
            else
            {
                sql = @"
SELECT
    query_type_cd AS QueryTypeCd
    ,query_type_name AS QueryTypeName
FROM
    QueryType
WHERE
    is_active=1
ORDER BY
    sort_no
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            var result = Connection.Query<QueryTypeModel>(twowaySql.Sql);
            if (result == null || !result.Any())
                throw new NotFoundException("Not Found Query Type");

            return result.ToList();
        }

        #region RepositoryGroup
        /// <summary>
        /// RepositoryGroupの一覧を取得します。
        /// </summary>
        /// <returns>RepositoryGroupの一覧</returns>
        public IEnumerable<RepositoryGroupModel> GetRepositoryGroups()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT 
     rg.repository_group_id as RepositoryGroupId
     ,rg.repository_group_name as RepositoryGroupName
     ,rg.repository_type_cd as RepositoryTypeCd
     ,rt.repository_type_name as RepositoryTypeName
     ,rg.is_enable as IsEnable
FROM
    REPOSITORY_GROUP rg
    INNER JOIN REPOSITORY_TYPE rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.is_active = 1
WHERE 
    rg.is_active = 1
ORDER BY
    sort_no
";
            }
            else
            {
                sql = @"
SELECT 
     rg.repository_group_id as RepositoryGroupId
     ,rg.repository_group_name as RepositoryGroupName
     ,rg.repository_type_cd as RepositoryTypeCd
     ,rt.repository_type_name as RepositoryTypeName
     ,rg.is_enable as IsEnable
FROM
    RepositoryGroup rg
    INNER JOIN RepositoryType rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.is_active = 1
WHERE 
    rg.is_active = 1
ORDER BY
    sort_no
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            return Connection.Query<RepositoryGroupModel>(twowaySql.Sql);
        }

        /// <summary>
        /// DynamicAPIの指定の添付ファイルストレージの一覧を取得します。
        /// </summary>
        /// <returns>DynamicAPIの添付ファイルストレージの一覧</returns>
        [CacheEntity(DynamicApiDatabase.TABLE_REPOSITORYGROUP)]
        [CacheArg(allParam: true)]
        [Cache]
        public IList<RepositoryGroupModel> GetAttachFileStorage(IEnumerable<string> repositoryTypeCd)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId
    ,rg.repository_group_name AS RepositoryGroupName
FROM
    REPOSITORY_GROUP rg
WHERE
    rg.is_active = 1
    AND rg.repository_type_cd = /*ds repositoryTypeCd*/'aaa' 
ORDER BY
    rg.sort_no
";
            }
            else
            {
                sql = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId
    ,rg.repository_group_name AS RepositoryGroupName
FROM
    RepositoryGroup rg
WHERE
    rg.is_active = 1
    AND rg.repository_type_cd IN @repositoryTypeCd
ORDER BY
    rg.sort_no
";
            }
            var param = new { repositoryTypeCd };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<RepositoryGroupModel>(twowaySql.Sql, dynParams);
            if (result == null || !result.Any())
                throw new NotFoundException($"Not Found AttachFileStorage. RepositoryTypeCd: {string.Join(",", repositoryTypeCd)}");

            return result.ToList();
        }

        /// <summary>
        /// コントローラに紐づくメソッドのプライマリまたはセカンダリのリポジトリグループに
        /// 「DataLakeStore」がある場合、かつ、「代表的なモデル」が未入力の場合はエラーとする
        /// </summary>
        /// <param name="controllerId">controllerId</param>
        /// <param name="dataSchemaId">dataSchemaId</param>
        /// <returns></returns>
        public bool CheckDataLakeStoreMethod(string controllerId, string dataSchemaId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    ApiRep.CNT + SecRep.CNT
FROM
(
    SELECT
        COUNT(*) AS CNT
    FROM
        API a
        INNER JOIN REPOSITORY_GROUP rg ON a.repository_group_id = rg.repository_group_id AND rg.is_active = 1
        INNER JOIN REPOSITORY_TYPE rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.repository_type_cd ='dls' AND rt.is_active = 1
    WHERE
        a.controller_id = /*ds controller_id*/'00000000-0000-0000-0000-000000000000' 
    AND a.is_active = 1
) ApiRep,
(
    SELECT
        COUNT(*) AS CNT
    FROM
        REPOSITORY_GROUP rg
        INNER JOIN REPOSITORY_TYPE rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.repository_type_cd = /*ds repository_type_cd*/'aaa' AND rt.is_active = 1
    WHERE
        repository_group_id IN
        (
            SELECT
                repository_group_id
            FROM
                SECONDARY_REPOSITORY_MAP secRep
            WHERE
                secRep.is_active = 1
                AND secRep.api_id IN (SELECT api_id FROM API WHERE controller_id = /*ds controller_id*/'00000000-0000-0000-0000-000000000000' AND is_active = 1)
        )
) SecRep
";
            }
            else
            {
                sql = @"
SELECT 
    ApiRep.CNT + SecRep.CNT
FROM
(
    SELECT
        COUNT(*) AS CNT
    FROM
        Api a
        INNER JOIN RepositoryGroup rg ON a.repository_group_id = rg.repository_group_id AND rg.is_active = 1
        INNER JOIN RepositoryType rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.repository_type_cd ='dls' AND rt.is_active = 1
    WHERE
        a.controller_id = @controller_id
    AND a.is_active = 1
) ApiRep,
(
    SELECT
        COUNT(*) AS CNT
    FROM
        RepositoryGroup rg
        INNER JOIN RepositoryType rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.repository_type_cd = @repository_type_cd AND rt.is_active = 1
    WHERE
        repository_group_id IN
        (
            SELECT
                repository_group_id
            FROM
                SecondaryRepositoryMap secRep
            WHERE
                secRep.is_active = 1
                AND secRep.api_id IN (SELECT api_id FROM Api WHERE controller_id = @controller_id AND is_active = 1)
        )
) SecRep
";
            }
            var param = new { repository_type_cd = RepositoryTypeCdDataLakeStore, controller_id = controllerId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingle<int>(twowaySql.Sql, dynParams);

            if (result > 0 && string.IsNullOrEmpty(dataSchemaId))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 対象のリポジトリグループIDのリポジトリタイプがDataLakeStoreと一致するか
        /// </summary>
        /// <param name="repositoryGroupIdList">リポジトリグループIDリスト</param>
        /// <returns>true:一致する/false:一致しない</returns>
        public bool MatchDataLakeStoreId(IEnumerable<string> repositoryGroupIdList)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(repository_group_id)
FROM
    REPOSITORY_GROUP rg
    INNER JOIN REPOSITORY_TYPE rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.repository_type_cd = /*ds repositoryTypeCd*/'aaa' AND rt.is_active = 1
WHERE
    rg.repository_group_id IN /*ds repositoryGroupIdList*/'aaa' 
    AND rg.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(repository_group_id)
FROM
    RepositoryGroup rg
    INNER JOIN RepositoryType rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.repository_type_cd = @repositoryTypeCd AND rt.is_active = 1
WHERE
    rg.repository_group_id IN @repositoryGroupIdList
    AND rg.is_active = 1
";
            }
            var param = new { repositoryGroupIdList = repositoryGroupIdList, repositoryTypeCd = RepositoryTypeCdDataLakeStore };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            // dynamicParameterで配列がうまく扱えない為、dynamicParameter化未対応
            var result = Connection.QuerySingle<int>(twowaySql.Sql, param);
            return result > 0;
        }



        #endregion

        #region Vendor
        /// <summary>
        /// 有効なベンダー/システムの一覧を取得します。
        /// ベンダーIDが指定された場合は、指定されたベンダーのみ取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>有効なベンダー/システムの一覧</returns>
        public IEnumerable<VendorNameSystemNameModel> GetVendorList(string? vendorId = null)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    v.vendor_id as VendorId,
    v.vendor_name as VendorName,
    v.is_data_use as IsDataUse,
    s.system_id as SystemId,
    s.system_name as SystemName
FROM
    VENDOR v
    LEFT JOIN SYSTEM s ON s.vendor_id = v.vendor_id AND s.is_enable = 1 AND s.is_active = 1
WHERE
/*ds if vendorId != null*/
    v.vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    v.is_active = 1
    AND v.is_enable = 1
    
ORDER BY
    VendorName, SystemName
";
            }
            else
            {
                sql = @"
SELECT
    v.vendor_id as VendorId,
    v.vendor_name as VendorName,
    v.is_data_use as IsDataUse,
    s.system_id as SystemId,
    s.system_name as SystemName
FROM
    Vendor v
    LEFT JOIN System s ON s.vendor_id = v.vendor_id AND s.is_enable = 1 AND s.is_active = 1
WHERE
/*ds if vendorId != null*/
    v.vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    v.is_active = 1
    AND v.is_enable = 1
    
ORDER BY
    VendorName, SystemName
";
            }

            try
            {
                var param = !string.IsNullOrEmpty(vendorId) ? new { vendorId } : null;
                var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
                var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
                var vendorDic = new Dictionary<string, VendorNameSystemNameModel>();
                // 検索実行
                Connection.Query<VendorNameSystemNameModel, SystemNameSystemIdModel, VendorNameSystemNameModel>(twowaySql.Sql,
                    (vendor, system) =>
                    {
                        if (!vendorDic.TryGetValue(vendor.VendorId, out VendorNameSystemNameModel? result))
                        {
                            // 辞書に新規のベンダーを追加
                            vendorDic.Add(vendor.VendorId, vendor);
                            result = vendor;
                        }

                        // システムを追加
                        if (system != null) result.Systems.Add(system);
                        return result;
                    },
                    dynParams,
                    splitOn: "SystemId");
                // 結果を返却
                return vendorDic.Values;
            }
            catch (SqlException ex)
            {
                logger.Error($"Sql Error: {ex.Number} {ex.Message}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        /// <summary>
        /// ベンダーが存在するか
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>true:存在する/false:存在しない</returns>
        public bool ExistsVendor(string vendorId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(vendor_id)
FROM
    VENDOR
WHERE
    vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' 
    AND is_active = 1
    AND is_enable = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(vendor_id)
FROM
    Vendor
WHERE
    vendor_id = @vendorId
    AND is_active = 1
    AND is_enable = 1
";
            }
            var param = new { vendorId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            return result == 1;
        }

        /// <summary>
        /// APIで使用中のVendorSystemの組み合わせが正しいかどうか
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="systemId">システムID</param>
        /// <returns>true:正しい/false:正しくない</returns>
        public bool CheckVendorSystemCombination(string vendorId, string systemId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    VENDOR v /*WITH(NOLOCK)*/
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON v.vendor_id = s.vendor_id AND s.is_active = 1
WHERE
    v.vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' 
    AND s.system_id = /*ds systemId*/'00000000-0000-0000-0000-000000000000' 
    AND v.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    Vendor v WITH(NOLOCK)
    INNER JOIN System s WITH(NOLOCK) ON v.vendor_id = s.vendor_id AND s.is_active = 1
WHERE
    v.vendor_id = @vendorId
    AND s.system_id = @systemId
    AND v.is_active = 1
";
            }
            var param = new { systemId, vendorId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            // 1件以上あればTrue
            return result > 0;
        }

        #endregion

        #region OpenIdCa
        private enum RelationTableType
        {
            Vendor = 1,
            Controller,
            Api
        }

        /// <summary>
        /// ベンダーに紐づくOpenID認証局リストを取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>OpenID認証局リスト</returns>
        [CacheEntity(DynamicApiDatabase.TABLE_VENDOROPENIDCA, DynamicApiDatabase.TABLE_OPENIDCERTIFICATIONAUTHORITY)]
        [CacheArg(allParam: true)]
        [Cache]
        public IList<OpenIdCaModel> GetVendorOpenIdCaList(string vendorId)
            => GetOpenIdCaList(vendorId, RelationTableType.Vendor);

        /// <summary>
        /// コントローラーに紐づくOpenID認証局リストを取得します。（OUTER JOIN）
        /// </summary>
        /// <param name="controllerId">コントローラーID</param>
        /// <returns>OpenID認証局リスト</returns>
        [CacheEntity(DynamicApiDatabase.TABLE_CONTROLLEROPENIDCA, DynamicApiDatabase.TABLE_OPENIDCERTIFICATIONAUTHORITY)]
        [CacheArg(allParam: true)]
        [Cache]
        public IList<OpenIdCaModel> GetControllerOpenIdCaList(string controllerId)
            => GetOpenIdCaList(controllerId, RelationTableType.Controller);

        /// <summary>
        /// ControllerIdに紐づくOpenID認証局リストを取得します。（INNER JOIN）
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>OpenIdCA</returns>
        public IEnumerable<OpenIdCaModel> GetControllerOpenIdCAList(string controllerId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    co.controller_openid_ca_id as Id
    ,co.application_id as ApplicationId
    ,o.application_name as ApplicationName
    ,co.access_control as AccessControl
FROM
    CONTROLLER_OPEN_ID_CA co
    INNER JOIN OPEN_ID_CERTIFICATION_AUTHORITY o ON o.application_id = co.application_id AND o.is_active = 1
WHERE
    co.controller_id = /*ds controllerId*/'00000000-0000-0000-0000-000000000000' 
    AND co.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    co.controller_openid_ca_id as Id
    ,co.application_id as ApplicationId
    ,o.application_name as ApplicationName
    ,co.access_control as AccessControl
FROM
    ControllerOpenIdCA co
    INNER JOIN OpenIdCertificationAuthority o ON o.application_id = co.application_id AND o.is_active = 1
WHERE
    co.controller_id = @controllerId
    AND co.is_active = 1
";
            }
            var param = new { controllerId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<OpenIdCaModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// APIに紐づくOpenID認証局リストを取得します。（OUTER JOIN）
        /// </summary>
        /// <param name="apiId">API ID</param>
        /// <returns>OpenID認証局リスト</returns>
        [CacheEntity(DynamicApiDatabase.TABLE_APIOPENIDCA, DynamicApiDatabase.TABLE_OPENIDCERTIFICATIONAUTHORITY)]
        [CacheArg(allParam: true)]
        [Cache]
        public IList<OpenIdCaModel> GetApiOpenIdCaList(string apiId)
            => GetOpenIdCaList(apiId, RelationTableType.Api);

        /// <summary>
        /// APIに紐づくOpenID認証局リストを取得します。（INNER JOIN）
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns>ApiOpenIdCA</returns>
        public IEnumerable<OpenIdCaModel> GetApiOpenIdCAList(string apiId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    a.api_openid_ca_id as Id
    ,a.application_id as ApplicationId
    ,o.application_name as ApplicationName
    ,a.access_control as AccessControl
FROM
    API_OPEN_ID_CA a
    INNER JOIN OPEN_ID_CERTIFICATION_AUTHORITY o ON o.application_id = a.application_id AND o.is_active = 1
WHERE
    a.api_id = /*ds apiId*/'00000000-0000-0000-0000-000000000000' 
    AND a.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    a.api_openid_ca_id as Id
    ,a.application_id as ApplicationId
    ,o.application_name as ApplicationName
    ,a.access_control as AccessControl
FROM
    ApiOpenIdCA a
    INNER JOIN OpenIdCertificationAuthority o ON o.application_id = a.application_id AND o.is_active = 1
WHERE
    a.api_id = @apiId
    AND a.is_active = 1
";
            }
            var param = new { apiId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<OpenIdCaModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// OpenID認証局リストを取得する
        /// </summary>
        /// <param name="relationId">ベンダーIDやApiID等のID</param>
        /// <param name="type">取得するテーブルの種別</param>
        /// <returns>OpenID認証局リスト</returns>
        private IList<OpenIdCaModel> GetOpenIdCaList(string relationId, RelationTableType type)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            string tableName = string.Empty;
            string idColumnName = string.Empty;
            string relationIdColumnName = string.Empty;

            if (dbsettings.Type == "Oracle")
            {
                switch (type)
                {
                    case RelationTableType.Vendor:
                        tableName = "VENDOR_OPEN_ID_CA";
                        idColumnName = "vendor_openid_ca_id";
                        relationIdColumnName = "vendor_id";
                        break;
                    case RelationTableType.Controller:
                        tableName = "CONTROLLER_OPEN_ID_CA";
                        idColumnName = "controller_openid_ca_id";
                        relationIdColumnName = "controller_id";
                        break;
                    case RelationTableType.Api:
                        tableName = "API_OPEN_ID_CA";
                        idColumnName = "api_openid_ca_id";
                        relationIdColumnName = "api_id";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case RelationTableType.Vendor:
                        tableName = "VendorOpenIdCA";
                        idColumnName = "vendor_openid_ca_id";
                        relationIdColumnName = "vendor_id";
                        break;
                    case RelationTableType.Controller:
                        tableName = "ControllerOpenIdCA";
                        idColumnName = "controller_openid_ca_id";
                        relationIdColumnName = "controller_id";
                        break;
                    case RelationTableType.Api:
                        tableName = "ApiOpenIdCA";
                        idColumnName = "api_openid_ca_id";
                        relationIdColumnName = "api_id";
                        break;
                    default:
                        break;
                }
            }

            if (dbsettings.Type == "Oracle")
            {
                sql = string.Format(@"
SELECT
    ca.application_id AS ApplicationId
    ,ca.application_name  AS ApplicationName
    ,COALESCE(t.is_active, 1) AS IsActive 
    ,t.{0} AS Id
    ,COALESCE(t.access_control, 'inh') AS AccessControl
FROM
    OPEN_ID_CERTIFICATION_AUTHORITY ca
    LEFT OUTER JOIN {1} t ON ca.application_id = t.application_id AND t.{2} = /*ds relationId*/'00000000-0000-0000-0000-000000000000' AND t.is_active = 1
WHERE
    ca.is_active = 1
ORDER BY
    ca.application_id
", idColumnName, tableName, relationIdColumnName);
            }
            else
            {
                sql = string.Format(@"
SELECT
    ca.application_id AS ApplicationId
    ,ca.application_name  AS ApplicationName
    ,ISNULL(t.is_active, '1') AS IsActive 
    ,t.{0} AS Id
    ,ISNULL(t.access_control, 'inh') AS AccessControl
FROM
    OpenIdCertificationAuthority ca
    LEFT OUTER JOIN {1} t ON ca.application_id = t.application_id AND t.{2} = @relationId AND t.is_active = 1
WHERE
    ca.is_active = 1
ORDER  BY
    ca.application_id
", idColumnName, tableName, relationIdColumnName);
            }
            var param = new { relationId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<OpenIdCaModel>(twowaySql.Sql, dynParams).ToList();
        }

        /// <summary>
        /// VendorIdに紐づくControllerのOpenIdCAを取得します。
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのOpenIdCAを取得する</param>
        /// <returns>OpenIdCA</returns>
        private Dictionary<string, IEnumerable<OpenIdCaModel>> GetControllerOpenIdCA(string vendorId, bool isAll)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    co.controller_id as ControllerId
    ,co.controller_openid_ca_id as ControllerOpenIdCaId
    ,co.application_id as ApplicationId
    ,o.application_name as ApplicationName
    ,co.access_control as AccessControl
FROM
    CONTROLLER_OPEN_ID_CA co /*WITH(NOLOCK)*/
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = co.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN OPEN_ID_CERTIFICATION_AUTHORITY o /*WITH(NOLOCK)*/ ON o.application_id = co.application_id AND o.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    co.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    co.controller_id as ControllerId
    ,co.controller_openid_ca_id as ControllerOpenIdCaId
    ,co.application_id as ApplicationId
    ,o.application_name as ApplicationName
    ,co.access_control as AccessControl
FROM
    ControllerOpenIdCA co WITH(NOLOCK)
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = co.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN OpenIdCertificationAuthority o WITH(NOLOCK) ON o.application_id = co.application_id AND o.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendorId AND
/*ds end if*/
    co.is_active = 1
";
            }
            var param = new { vendorId };
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendorId", vendorId);
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<string, OpenIdCaModel, (string ControllerId, OpenIdCaModel OpenIdCA)>(
                sql: twowaySql.Sql,
                splitOn: "ControllerOpenIdCaId",
                map: (controllerId, openIdCA) => (controllerId, openIdCA),
                param: dynParams);

            return result.GroupBy(x => x.ControllerId).ToDictionary(x => x.Key, y => y.Select(z => z.OpenIdCA));
        }

        /// <summary>
        /// VendorIdに紐づくAPIのOpenIdCaを取得します。
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのApiOpenIdCAを取得する</param>
        /// <returns>ApiOpenIdCA</returns>
        private Dictionary<string, IEnumerable<OpenIdCaModel>> GetApiOpenIdCa(string vendorId, bool isAll)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {

                sql = @"
SELECT
    ao.api_id as ApiId
    ,ao.api_openid_ca_id as Id
    ,ao.application_id as ApplicationId
    ,o.application_name as ApplicationName
    ,ao.access_control as AccessControl
FROM
    API_OPEN_ID_CA ao /*WITH(NOLOCK)*/
    INNER JOIN API a /*WITH(NOLOCK)*/ ON a.api_id = ao.api_id AND a.is_active = 1
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = a.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN OPEN_ID_CERTIFICATION_AUTHORITY o /*WITH(NOLOCK)*/ ON o.application_id = ao.application_id AND o.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    ao.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    ao.api_id as ApiId
    ,ao.api_openid_ca_id as Id
    ,ao.application_id as ApplicationId
    ,o.application_name as ApplicationName
    ,ao.access_control as AccessControl
FROM
    ApiOpenIdCA ao WITH(NOLOCK)
    INNER JOIN Api a WITH(NOLOCK) ON a.api_id = ao.api_id AND a.is_active = 1
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = a.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN OpenIdCertificationAuthority o WITH(NOLOCK) ON o.application_id = ao.application_id AND o.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendor_id AND
/*ds end if*/
    ao.is_active = 1
";
            }
            var param = new { vendorId };
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendorId", vendorId);
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<string, OpenIdCaModel, (string ApiId, OpenIdCaModel ApiOpenIdCA)>(
                sql: twowaySql.Sql,
                splitOn: "Id",
                map: (apiId, apiOpenIdCA) => (apiId, apiOpenIdCA),
                param: dynParams);

            return result.GroupBy(x => x.ApiId).ToDictionary(x => x.Key, y => y.Select(z => z.ApiOpenIdCA));
        }

        /// <summary>
        /// ベンダーに紐づくOpenID認証局リストを更新します。
        /// </summary>
        /// <param name="model">OpenID認証局リスト</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>OpenID認証局リスト</returns>
        [CacheIdFire("vendorId", "model.Id")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_VENDOROPENIDCA)]
        public OpenIdCaModel UpsertVendorOpenIdCa(OpenIdCaModel model, string vendorId)
            => UpsertOpenIdCa(model, vendorId, RelationTableType.Vendor);

        /// <summary>
        /// コントローラーに紐づくOpenID認証局リストを更新します。
        /// </summary>
        /// <param name="model">OpenID認証局リスト</param>
        /// <param name="controllerId">コントローラーID</param>
        /// <returns>OpenID認証局リスト</returns>
        [CacheIdFire("controllerId", "model.Id")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_CONTROLLEROPENIDCA)]
        public OpenIdCaModel UpsertControllerOpenIdCa(OpenIdCaModel model, string controllerId)
            => UpsertOpenIdCa(model, controllerId, RelationTableType.Controller);

        /// <summary>
        /// Apiに紐づくOpenID認証局リストを更新します。
        /// </summary>
        /// <param name="model">OpenID認証局リスト</param>
        /// <param name="apiId">API ID</param>
        /// <returns>OpenID認証局リスト</returns>
        [CacheIdFire("apiId", "model.Id")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_APIOPENIDCA)]
        public  OpenIdCaModel UpsertApiOpenIdCa(OpenIdCaModel model, string apiId)
            => UpsertOpenIdCa(model, apiId, RelationTableType.Api);

        /// <summary>
        /// OpenID認証局リストを更新します。
        /// </summary>
        /// <param name="model">OpenID認証局リスト</param>
        /// <param name="relationId">ベンダーIDやApiID等のID</param>
        /// <param name="type">更新するするテーブルの種別</param>
        /// <returns>OpenID認証局リスト</returns>
        private OpenIdCaModel UpsertOpenIdCa(OpenIdCaModel model, string relationId, RelationTableType type)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(model.AccessControl))
            {
                throw new ArgumentNullException(model.AccessControl);
            }

            if (string.IsNullOrEmpty(relationId))
            {
                throw new ArgumentNullException(relationId);
            }

            if (!Guid.TryParse(relationId, out _))
            {
                throw new InvalidCastException(relationId);
            }

            // 新規で未設定の場合はデータを作らない
            if ((model.Id == null || model.Id.ToString() == "00000000-0000-0000-0000-000000000000") && model.AccessControl == "inh")
            {
                return model;
            }
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            string tableName = string.Empty;
            string idColumnName = string.Empty;
            string relationIdColumnName = string.Empty;
            if (dbsettings.Type == "Oracle")
            {
                switch (type)
                {
                    case RelationTableType.Vendor:
                        tableName = "VENDOR_OPEN_ID_CA";
                        idColumnName = "vendor_openid_ca_id";
                        relationIdColumnName = "vendor_id";
                        break;
                    case RelationTableType.Controller:
                        tableName = "CONTROLLER_OPEN_ID_CA";
                        idColumnName = "controller_openid_ca_id";
                        relationIdColumnName = "controller_id";
                        break;
                    case RelationTableType.Api:
                        tableName = "API_OPEN_ID_CA";
                        idColumnName = "api_openid_ca_id";
                        relationIdColumnName = "api_id";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case RelationTableType.Vendor:
                        tableName = "VendorOpenIdCA";
                        idColumnName = "vendor_openid_ca_id";
                        relationIdColumnName = "vendor_id";
                        break;
                    case RelationTableType.Controller:
                        tableName = "ControllerOpenIdCA";
                        idColumnName = "controller_openid_ca_id";
                        relationIdColumnName = "controller_id";
                        break;
                    case RelationTableType.Api:
                        tableName = "ApiOpenIdCA";
                        idColumnName = "api_openid_ca_id";
                        relationIdColumnName = "api_id";
                        break;
                    default:
                        break;
                }
            }

            if (model.Id == null || model.Id.ToString() == "00000000-0000-0000-0000-000000000000")
            {
                model.Id = Guid.NewGuid();
            }


            if (dbsettings.Type == "Oracle")
            {
                sql = string.Format(@"
MERGE INTO {0} target
USING
(
    SELECT
        /*ds id*/'00000000-0000-0000-0000-000000000000' AS id FROM DUAL
) source
ON
    (target.{1} = source.id)
WHEN MATCHED THEN
    UPDATE
    SET
        {2} = /*ds relation_id*/'00000000-0000-0000-0000-000000000000' 
        ,application_id = /*ds application_id*/'00000000-0000-0000-0000-000000000000' 
        ,upd_date = /*ds upd_date*/SYSTIMESTAMP 
        ,upd_username = /*ds upd_username*/'a' 
        ,is_active= /*ds is_active*/1 
        ,access_control= /*ds access_control*/'aaa' 
WHEN NOT MATCHED THEN
    INSERT
    (
        {1}
        ,{2}
        ,application_id
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
        ,access_control
    )
    VALUES
    (
        /*ds id*/'00000000-0000-0000-0000-000000000000' 
        ,/*ds relation_id*/'00000000-0000-0000-0000-000000000000' 
        ,/*ds application_id*/'00000000-0000-0000-0000-000000000000' 
        ,/*ds reg_date*/SYSTIMESTAMP 
        ,/*ds reg_username*/'a' 
        ,/*ds upd_date*/SYSTIMESTAMP 
        ,/*ds upd_username*/'a' 
        ,/*ds is_active*/1 
        ,/*ds access_control*/'aaa'
    )
", tableName, idColumnName, relationIdColumnName);
            }
            else
            {
                sql = string.Format(@"
MERGE INTO {0} AS target
USING
(
    SELECT
        @id AS id
) AS source
ON
    target.{1} = source.id
WHEN MATCHED THEN
    UPDATE
    SET
        {2} = @relation_id
        ,application_id = @application_id
        ,upd_date = @upd_date
        ,upd_username = @upd_username
        ,is_active= @is_active
        ,access_control= @access_control
WHEN NOT MATCHED THEN
    INSERT
    (
        {1}
        ,{2}
        ,application_id
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
        ,access_control
    )
    VALUES
    (
        @id
        ,@relation_id
        ,@application_id
        ,@reg_date
        ,@reg_username
        ,@upd_date
        ,@upd_username
        ,@is_active
        ,@access_control
    );
", tableName, idColumnName, relationIdColumnName);
            }

            var param = new
            {
                id = model.Id,
                relation_id = relationId,
                application_id = model.ApplicationId,
                access_control = model.AccessControl,
                reg_date = UtcNow,
                reg_username = PerRequestDataContainer.OpenId,
                upd_date = UtcNow,
                upd_username = PerRequestDataContainer.OpenId,
                is_active = model.IsActive,
            };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            Connection.Execute(twowaySql.Sql, dynParams);
            return model;
        }
        #endregion

        #region APIDescription
        #region sql
        string SelectSchemaSql {
            get {
                var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql="";
                if (dbsettings.Type == "Oracle")
                {
                    sql = @"
SELECT
    ds.data_schema_id as SchemaId,
    ds.schema_name as SchemaName,
    ds.data_schema as JsonSchema,
    COALESCE(TO_NCHAR(dsml.schema_description), ds.schema_description) as Description,
    ds.is_active as IsActive,
    ds.upd_date as UpdDate
FROM DATA_SCHEMA ds
LEFT JOIN LOCALE loc ON 
    loc.locale_code = /*ds localeCode*/'1' AND 
    loc.is_active = 1
LEFT JOIN DATA_SCHEMA_MULTI_LANGUAGE dsml ON 
    ds.data_schema_id = dsml.data_schema_id AND 
    dsml.locale_code = loc.locale_code AND 
    dsml.is_active = 1";
                }
                else
                {
                    sql = @"
SELECT
    ds.data_schema_id as SchemaId,
    ds.schema_name as SchemaName,
    ds.data_schema as JsonSchema,
    ISNULL(dsml.schema_description, ds.schema_description) as Description,
    ds.is_active as IsActive,
    ds.upd_date as UpdDate
FROM DataSchema ds
LEFT JOIN Locale loc ON 
    loc.locale_code = @localeCode AND 
    loc.is_active = 1
LEFT JOIN DataSchemaMultiLanguage dsml ON 
    ds.data_schema_id = dsml.data_schema_id AND 
    dsml.locale_code = loc.locale_code AND 
    dsml.is_active = 1";
                }
                return sql;
            }
        }
        #endregion

        /// <summary>
        /// ベンダーのリンク情報の一覧を取得します。
        /// </summary>
        /// <returns>ベンダーのリンク情報</returns>
        [CacheEntity(DynamicApiDatabase.TABLE_VENDORLINK)]
        [Cache]
        public IEnumerable<VendorLinkModel> GetVendorLink()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    vendor_link_id as VendorLinkId,
    vendor_id as VendorId,
    title,
    detail,
    url,
    is_visible as IsVisible,
    is_active as IsActive,
    upd_date as UpdDate
FROM VENDOR_LINK
WHERE is_default = 0";
            }
            else
            {
                sql = @"
SELECT
    vendor_link_id as VendorLinkId,
    vendor_id as VendorId,
    title,
    detail,
    url,
    is_visible as IsVisible,
    is_active as IsActive,
    upd_date as UpdDate
FROM VendorLink
WHERE is_default = 0";
            }

            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            return Connection.Query<VendorLinkModel>(twowaySql.Sql);
        }

        /// <summary>
        /// システムのリンク情報の一覧を取得します。
        /// </summary>
        /// <returns>システムのリンク情報</returns>
        [CacheEntity(DynamicApiDatabase.TABLE_SYSTEMLINK)]
        [Cache]
        public IEnumerable<SystemLinkModel> GetSystemLink()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    system_link_id as SystemLinkId,
    system_id as SystemId,
    title,
    detail,
    url,
    is_visible as IsVisible,
    is_active as IsActive,
    upd_date as UpdDate
FROM SYSTEM_LINK
WHERE is_default = 0";
            }
            else
            {
                sql = @"
SELECT
    system_link_id as SystemLinkId,
    system_id as SystemId,
    title,
    detail,
    url,
    is_visible as IsVisible,
    is_active as IsActive,
    upd_date as UpdDate
FROM SystemLink
WHERE is_default = 0";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            return Connection.Query<SystemLinkModel>(twowaySql.Sql);
        }

        /// <summary>
        /// API情報の一覧を取得します。
        /// </summary>
        /// <param name="noChildren">子の情報なしフラグ</param>
        /// <param name="localeCode">ロケール</param>
        /// <param name="isActiveOnly">未削除のみ</param>
        /// <param name="isEnableOnly">有効のみ</param>
        /// <param name="isNotHiddenOnly">公開のみ</param>
        /// <returns>API情報の一覧</returns>
        [CacheArg(allParam: true)]
        [CacheEntity(DynamicApiDatabase.TABLE_CONTROLLER, DynamicApiDatabase.TABLE_VENDOR, DynamicApiDatabase.TABLE_SYSTEM, DynamicApiDatabase.TABLE_API, DynamicApiDatabase.TABLE_REPOSITORYGROUP)]
        [Cache]
        public IEnumerable<ApiDescriptionModel> GetApiDescription(bool noChildren, string localeCode = null, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false)
        {
            IEnumerable<ApiDescriptionModel> apiDescriptions;

            if (string.IsNullOrWhiteSpace(localeCode))
            {
                localeCode = DefaultLocaleCode;
            }

            if (noChildren)
            {
                var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbsettings.Type == "Oracle")
                {
                    sql = $@"
SELECT
    (SELECT TO_TIMESTAMP(MAX(column_value), 'YY-MM-DD HH24:MI:SS.FF9') FROM table(sys.odcivarchar2list(api.upd_date, vdr.upd_date, vl.upd_date, sys.upd_date, sl.upd_date))) as UpdDate,
    vl.url as VendorUrl,
    sl.url as SystemUrl,
    api.controller_id as ApiId,
    api.vendor_id as VendorId,
    COALESCE(vdrml.vendor_name, vdr.vendor_name) as VendorName,
    api.system_id as SystemId,
    COALESCE(sysml.system_name, sys.system_name) as SystemName,
    COALESCE(apiml.controller_name, api.controller_name) as ApiName,
    api.url as RelativePath,
    COALESCE(apiml.controller_description, api.controller_description) as Description,
    api.is_vendor as PartitionByVendor,
    api.is_person as PartitionByPerson,
    api.is_static_api as IsStaticApi,
    api.is_data as IsData,
    api.is_businesslogic as IsBusinesslogic,
    api.is_pay as IsPay,
    COALESCE(apiml.fee_description, api.fee_description) as FeeDescription,
    COALESCE(apiml.resource_create_user, api.resource_create_user) as ResourceCreateUser,
    COALESCE(apiml.resource_maintainer, api.resource_maintainer) as ResourceMaintainer,
    api.resource_create_date as ResourceCreateDate,
    api.resource_latest_date as ResourceLatestDate,
    COALESCE(apiml.update_frequency, api.update_frequency) as UpdateFrequency,
    api.is_contract as NeedContract,
    COALESCE(apiml.contact_information, api.contact_information) as ContactInformation,
    COALESCE(apiml.version, api.version) as version,
    COALESCE(apiml.agree_description, api.agree_description) as AgreeDescription,
    api.is_visible_agreement as IsVisibleAgreement,
    api.is_enable as IsEnable,
    CASE WHEN api.is_active = 1 AND COALESCE(vdr.is_active, 0) =1 AND COALESCE(vdr.is_enable, 0) =1 AND COALESCE(sys.is_active, 0) =1 AND COALESCE(sys.is_enable, 0) =1 THEN 1 ELSE 0 END as IsActive,
    controller_schema_id as ApiSchemaId,
    controller_repository_key as RepositoryKey,
    at.action_type_name AS ActionType
FROM
    CONTROLLER api
    LEFT JOIN LOCALE loc ON  loc.locale_code = /*ds localeCode*/'a' AND  loc.is_active = 1
    LEFT JOIN CONTROLLER_MULTI_LANGUAGE apiml ON api.controller_id = apiml.controller_id AND apiml.locale_code = loc.locale_code AND apiml.is_active = 1
    INNER JOIN VENDOR vdr ON vdr.vendor_id = api.vendor_id AND vdr.is_active=1
    LEFT JOIN VENDOR_MULTI_LANGUAGE vdrml ON api.vendor_id = vdrml.vendor_id AND vdrml.locale_code = loc.locale_code AND vdrml.is_active = 1
    INNER JOIN SYSTEM sys ON sys.system_id = api.system_id AND sys.is_active=1
    LEFT JOIN SYSTEM_MULTI_LANGUAGE sysml ON api.system_id = sysml.system_id AND sysml.locale_code = loc.locale_code AND sysml.is_active = 1
    LEFT JOIN VENDOR_LINK vl ON vl.vendor_id = api.vendor_id AND vl.is_default = 1 AND vl.is_visible = 1 AND vl.is_active = 1 
    LEFT JOIN SYSTEM_LINK sl ON sl.system_id = api.system_id AND sl.is_default = 1 AND sl.is_visible = 1 AND sl.is_active = 1
    LEFT JOIN API method ON method.controller_id = api.controller_id AND method.is_transparent_api = 0
    LEFT JOIN API_MULTI_LANGUAGE methodml ON method.api_id = methodml.api_id AND methodml.locale_code = loc.locale_code AND methodml.is_active = 1
    LEFT JOIN ACTION_TYPE at ON at.action_type_cd = method.action_type_cd
    LEFT JOIN REPOSITORY_GROUP rg ON method.repository_group_id=rg.repository_group_id
WHERE
/*ds if isEnableOnly != null*/
    api.is_enable = 1 AND
/*ds end if*/
/*ds if isActiveOnly != null*/
    api.is_active = 1 AND
    method.is_active = 1 AND
/*ds end if*/
/*ds if isEnableOnly != null*/
    method.is_enable = 1 AND
/*ds end if*/
/*ds if isHidden != null*/
    method.is_hidden = 0 AND
/*ds end if*/
    vdr.is_active = 1 AND 
    vdr.is_enable = 1 AND 
    sys.is_active = 1 AND 
    sys.is_enable = 1
ORDER BY
    api.url,
    method.url";
                }
                else
                {
                    sql = $@"
SELECT
    (SELECT MAX(upd_date) FROM (VALUES (api.upd_date), (vdr.upd_date), (vl.upd_date), (sys.upd_date), (sl.upd_date)) as LIST(upd_date)) as UpdDate,
    vl.url as VendorUrl,
    sl.url as SystemUrl,
    api.controller_id as ApiId,
    api.vendor_id as VendorId,
    ISNULL(vdrml.vendor_name, vdr.vendor_name) as VendorName,
    api.system_id as SystemId,
    ISNULL(sysml.system_name, sys.system_name) as SystemName,
    ISNULL(apiml.controller_name, api.controller_name) as ApiName,
    api.url as RelativePath,
    ISNULL(apiml.controller_description, api.controller_description) as Description,
    api.is_vendor as PartitionByVendor,
    api.is_person as PartitionByPerson,
    api.is_static_api as IsStaticApi,
    api.is_data as IsData,
    api.is_businesslogic as IsBusinesslogic,
    api.is_pay as IsPay,
    ISNULL(apiml.fee_description, api.fee_description) as FeeDescription,
    ISNULL(apiml.resource_create_user, api.resource_create_user) as ResourceCreateUser,
    ISNULL(apiml.resource_maintainer, api.resource_maintainer) as ResourceMaintainer,
    api.resource_create_date as ResourceCreateDate,
    api.resource_latest_date as ResourceLatestDate,
    ISNULL(apiml.update_frequency, api.update_frequency) as UpdateFrequency,
    api.is_contract as NeedContract,
    ISNULL(apiml.contact_information, api.contact_information) as ContactInformation,
    ISNULL(apiml.version, api.version) as version,
    ISNULL(apiml.agree_description, api.agree_description) as AgreeDescription,
    api.is_visible_agreement as IsVisibleAgreement,
    api.is_enable as IsEnable,
    api.is_active & ISNULL(vdr.is_active, 0) & ISNULL(vdr.is_enable, 0) & ISNULL(sys.is_active, 0) & ISNULL(sys.is_enable, 0) as IsActive,
    controller_schema_id as ApiSchemaId,
    controller_repository_key as RepositoryKey,
    at.action_type_name AS ActionType
FROM
    Controller api
    LEFT JOIN Locale loc ON  loc.locale_code = @localeCode AND  loc.is_active = 1
    LEFT JOIN ControllerMultiLanguage apiml ON api.controller_id = apiml.controller_id AND apiml.locale_code = loc.locale_code AND apiml.is_active = 1
    INNER JOIN Vendor vdr ON vdr.vendor_id = api.vendor_id AND vdr.is_active=1
    LEFT JOIN VendorMultiLanguage vdrml ON api.vendor_id = vdrml.vendor_id AND vdrml.locale_code = loc.locale_code AND vdrml.is_active = 1
    INNER JOIN System sys ON sys.system_id = api.system_id AND sys.is_active=1
    LEFT JOIN SystemMultiLanguage sysml ON api.system_id = sysml.system_id AND sysml.locale_code = loc.locale_code AND sysml.is_active = 1
    LEFT JOIN VendorLink vl ON vl.vendor_id = api.vendor_id AND vl.is_default = 1 AND vl.is_visible = 1 AND vl.is_active = 1 
    LEFT JOIN SystemLink sl ON sl.system_id = api.system_id AND sl.is_default = 1 AND sl.is_visible = 1 AND sl.is_active = 1
    LEFT JOIN Api method ON method.controller_id = api.controller_id AND method.is_transparent_api = 0
    LEFT JOIN ApiMultiLanguage methodml ON method.api_id = methodml.api_id AND methodml.locale_code = loc.locale_code AND methodml.is_active = 1
    LEFT JOIN ActionType at ON at.action_type_cd = method.action_type_cd
    LEFT JOIN RepositoryGroup rg ON method.repository_group_id=rg.repository_group_id
WHERE
/*ds if isEnableOnly != null*/
    api.is_enable = 1 AND
/*ds end if*/
/*ds if isActiveOnly != null*/
    api.is_active = 1 AND
    method.is_active = 1 AND
/*ds end if*/
/*ds if isEnableOnly != null*/
    method.is_enable = 1 AND
/*ds end if*/
/*ds if isHidden != null*/
    method.is_hidden = 0 AND
/*ds end if*/
    vdr.is_active = 1 AND 
    vdr.is_enable = 1 AND 
    sys.is_active = 1 AND 
    sys.is_enable = 1
ORDER BY
    api.url,
    method.url";
                }
                var param = new { localeCode };
                // API情報を取得
                var dict = new Dictionary<string, object>();
                dict.Add("isActiveOnly", isActiveOnly ? "1" : null);
                dict.Add("isEnableOnly", isEnableOnly ? "1" : null);
                dict.Add("isHidden", isNotHiddenOnly ? "1" : null);
                dict.Add("localeCode", localeCode);
                var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, dict);
                var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
                apiDescriptions = Connection.Query<ApiDescriptionModel>(twowaySql.Sql, dynParams);
            }
            else
            {
                // 子要素を含めてAPI情報を取得
                apiDescriptions = GetApiDescriptionWithChildren(localeCode, isActiveOnly, isEnableOnly, isNotHiddenOnly);
            }
            return apiDescriptions;
        }

        /// <summary>
        /// 子要素を含めたAPI情報の一覧を取得します。
        /// </summary>
        /// <param name="localeCode">ロケール</param>
        /// <param name="isActiveOnly">未削除のみ</param>
        /// <param name="isEnableOnly">有効のみ</param>
        /// <param name="isNotHiddenOnly">公開のみ</param>
        /// <returns>API情報の一覧</returns>
        private IEnumerable<ApiDescriptionModel> GetApiDescriptionWithChildren(string localeCode, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false)
        {
            #region sql
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            var getFieldSql = "";
            var getTagSql = "";
            var getCategorySql = "";
            var getMethodLinkSql = "";
            var getSampleCodeSql = "";

            if (dbsettings.Type == "Oracle")
            {

                sql = $@"
SELECT
    api.controller_id as ApiId,
    api.vendor_id as VendorId,
    COALESCE(vdrml.vendor_name, vdr.vendor_name) as VendorName,
    api.system_id as SystemId,
    COALESCE(sysml.system_name, sys.system_name) as SystemName,
    COALESCE(apiml.controller_name, api.controller_name) as ApiName,
    api.url as RelativePath,
    COALESCE(apiml.controller_description, api.controller_description) as Description,
    api.is_vendor as PartitionByVendor,
    api.is_person as PartitionByPerson,
    api.is_static_api as IsStaticApi,
    api.is_data as IsData,
    api.is_businesslogic as IsBusinesslogic,
    api.is_pay as IsPay,
    COALESCE(apiml.fee_description, api.fee_description) as FeeDescription,
    COALESCE(apiml.resource_create_user, api.resource_create_user) as ResourceCreateUser,
    COALESCE(apiml.resource_maintainer, api.resource_maintainer) as ResourceMaintainer,
    api.resource_create_date as ResourceCreateDate,
    api.resource_latest_date as ResourceLatestDate,
    COALESCE(apiml.update_frequency, api.update_frequency) as UpdateFrequency,
    api.is_contract as NeedContract,
    COALESCE(apiml.contact_information, api.contact_information) as ContactInformation,
    COALESCE(apiml.version, api.version) as version,
    COALESCE(apiml.agree_description, api.agree_description) as AgreeDescription,
    api.is_visible_agreement as IsVisibleAgreement,
    api.is_enable as IsEnable,
    CASE WHEN api.is_active = 1 AND COALESCE(vdr.is_active, 0) =1 AND COALESCE(vdr.is_enable, 0) = 1 AND COALESCE(sys.is_active, 0) =1 AND COALESCE(sys.is_enable, 0) =1 THEN 1 ELSE 0 END as IsActive,
    controller_schema_id as ApiSchemaId,
    controller_repository_key as RepositoryKey,
    (SELECT TO_TIMESTAMP(MAX(column_value), 'YY-MM-DD HH24:MI:SS.FF9') FROM table(sys.odcivarchar2list(api.upd_date, vdr.upd_date, vl.upd_date, sys.upd_date, sl.upd_date))) as UpdDate,
    vl.url as VendorUrl,
    sl.url as SystemUrl,
    method.api_id as MethodId,
    method.method_type as HttpMethod,
    method.url as RelativePath,
    COALESCE(TO_NCHAR(methodml.api_description), method.api_description) as Description,
    method.is_header_authentication as AuthVendorSystem,
    method.is_openid_authentication as AuthOpenId,
    method.is_admin_authentication as AuthAdmin,
    method.is_vendor_system_authentication_allow_null as IsVendorSystemAuthenticationAllowNull,
    CASE method.post_data_type WHEN N'array' THEN 1 ELSE 0 END as PostArray,
    method.url_schema_id as UrlSchemaId,
    method.request_schema_id as RequestSchemaId,
    method.response_schema_id as ResponseSchemaId,
    method.is_visible_signinuser_only as IsVisibleSigninuserOnly,
    CASE
        WHEN method.is_enable = 1 AND COALESCE(rg.is_enable, 1) = 1 THEN 1
        ELSE 0
    END as IsEnable,
    method.is_hidden as IsHidden,
    CASE
        WHEN method.is_active = 1 AND COALESCE(rg.is_active, 1) = 1 THEN 1
        ELSE 0
    END as IsActive,
    method.upd_date as UpdDate,
    at.action_type_name AS ActionType
FROM
    CONTROLLER api
    LEFT JOIN LOCALE loc ON loc.locale_code = /*ds localeCode*/'aaa' AND loc.is_active = 1
    LEFT JOIN CONTROLLER_MULTI_LANGUAGE apiml ON api.controller_id = apiml.controller_id AND apiml.locale_code = loc.locale_code AND apiml.is_active = 1
    INNER JOIN VENDOR vdr ON vdr.vendor_id = api.vendor_id
    LEFT JOIN VENDOR_MULTI_LANGUAGE vdrml ON vdr.vendor_id = vdrml.vendor_id AND vdrml.locale_code = loc.locale_code AND vdrml.is_active = 1
    INNER JOIN SYSTEM sys ON sys.system_id = api.system_id
    LEFT JOIN SYSTEM_MULTI_LANGUAGE sysml ON sys.system_id = sysml.system_id AND sysml.locale_code = loc.locale_code AND sysml.is_active = 1
    LEFT JOIN VENDOR_LINK vl ON vl.vendor_id = api.vendor_id AND vl.is_default = 1 AND vl.is_visible = 1 AND vl.is_active = 1 
    LEFT JOIN SYSTEM_LINK sl ON sl.system_id = api.system_id AND sl.is_default = 1 AND sl.is_visible = 1 AND sl.is_active = 1
    LEFT JOIN API method ON method.controller_id = api.controller_id AND method.is_transparent_api = 0
    LEFT JOIN API_MULTI_LANGUAGE methodml ON method.api_id = methodml.api_id AND methodml.locale_code = loc.locale_code AND methodml.is_active = 1
    LEFT JOIN ACTION_TYPE at ON at.action_type_cd = method.action_type_cd
    LEFT JOIN REPOSITORY_GROUP rg ON method.repository_group_id=rg.repository_group_id
WHERE
/*ds if isEnableOnly != null*/
    api.is_enable = 1 AND
    method.is_enable = 1 AND
/*ds end if*/
/*ds if isActiveOnly != null*/
    api.is_active = 1 AND
    method.is_active = 1 AND
/*ds end if*/
/*ds if isHidden != null*/
    method.is_hidden = 0 AND
/*ds end if*/
    vdr.is_active = 1 AND 
    vdr.is_enable = 1 AND 
    sys.is_active = 1 AND 
    sys.is_enable = 1
ORDER BY
    api.url,
    method.url
";

                getFieldSql = @"
SELECT
    cf.controller_id as ApiId,
    f.field_id as FieldId,
    f.parent_field_id as ParentFieldId,
    COALESCE(ml.field_name, f.field_name) as FieldName,
    CASE WHEN COALESCE(f.is_active,0)=1 AND COALESCE(cf.is_active,0)=1 THEN 1 ELSE 0 END as IsActive,
    CASE WHEN f.upd_date > cf.upd_date THEN f.upd_date ELSE cf.upd_date END as UpdDate
FROM CONTROLLER_FIELD cf
INNER JOIN FIELD f on f.field_id = cf.field_id
LEFT JOIN LOCALE loc ON 
    loc.locale_code = /*ds localeCode*/'aaa' AND 
    loc.is_active = 1
LEFT JOIN FIELD_MULTI_LANGUAGE ml ON 
    f.field_id = ml.field_id AND 
    ml.locale_code = loc.locale_code AND 
    ml.is_active = 1";

                getTagSql = @"
SELECT
    ct.controller_id as ApiId,
    t.tag_id as TagId,
    t.parent_tag_id as ParentTagId,
    t.tag_type_id as TagTypeId,
    tt.tag_type_name as TagTypeName,
    COALESCE(ml.tag_name, t.tag_name) as TagName,
    t.tag_code as Code,
    t.tag_code2 as Code2,
    CASE WHEN COALESCE(tt.is_active,0) = 1 AND COALESCE(t.is_active,0) = 1 AND COALESCE(ct.is_active,0) = 1 THEN 1 ELSE 0 END as IsActive,
    (SELECT TO_TIMESTAMP(MAX(column_value), 'YY-MM-DD HH24:MI:SS.FF9') FROM table(sys.odcivarchar2list(ct.upd_date, t.upd_date, tt.upd_date))) as UpdDate
FROM CONTROLLER_TAG ct
INNER JOIN TAG t on t.tag_id = ct.tag_id
INNER JOIN TAG_TYPE tt on tt.tag_type_id = t.tag_type_id
LEFT JOIN LOCALE loc ON 
    loc.locale_code = /*ds localeCode*/'aaa' AND 
    loc.is_active = 1
LEFT JOIN TAG_MULTI_LANGUAGE ml ON 
    t.tag_id = ml.tag_id AND 
    ml.locale_code = loc.locale_code AND 
    ml.is_active = 1";

                getCategorySql = @"
SELECT
    cc.controller_id as ApiId,
    c.category_id as CategoryId,
    COALESCE(ml.category_name, c.category_name) as CategoryName,
    c.sort_order as DisplayOrder,
    CASE WHEN COALESCE(c.is_active,0) = 1 AND COALESCE(cc.is_active,0) = 1 THEN 1 ELSE 0 END as IsActive,
    CASE WHEN c.upd_date > cc.upd_date THEN c.upd_date ELSE cc.upd_date END as UpdDate
FROM CONTROLLER_CATEGORY cc 
INNER JOIN CATEGORY c on c.category_id = cc.category_id
LEFT JOIN LOCALE loc ON 
    loc.locale_code = /*ds localeCode*/'aaa' AND 
    loc.is_active = 1
LEFT JOIN CATEGORY_MULTI_LANGUAGE ml ON 
    c.category_id = ml.category_id AND 
    ml.locale_code = loc.locale_code AND 
    ml.is_active = 1";

                getMethodLinkSql = @"
SELECT 
    api_id as MethodId,
    api_link_id as MethodLinkId,
    title,
    detail,
    url,
    is_visible as IsVisible,
    is_active as IsActive,
    upd_date as UpdDate
FROM API_LINK";

                getSampleCodeSql = @"
SELECT 
    sc.api_id as MethodId,
    sc.sample_code_id as SampleCodeId,
    l.language_name as Language,
    l.order_no as DisplayOrder,
    sc.code,
    CASE WHEN COALESCE(l.is_active,0) = 1 AND COALESCE(sc.is_active,0) = 1 THEN 1 ELSE 0 END as IsActive,
    CASE WHEN l.upd_date > sc.upd_date THEN l.upd_date ELSE sc.upd_date END as UpdDate
FROM SAMPLE_CODE sc
JOIN LANGUAGE l on sc.language_id = l.language_id";
            }
            else
            {
                sql = $@"
SELECT
    api.controller_id as ApiId,
    api.vendor_id as VendorId,
    ISNULL(vdrml.vendor_name, vdr.vendor_name) as VendorName,
    api.system_id as SystemId,
    ISNULL(sysml.system_name, sys.system_name) as SystemName,
    ISNULL(apiml.controller_name, api.controller_name) as ApiName,
    api.url as RelativePath,
    ISNULL(apiml.controller_description, api.controller_description) as Description,
    api.is_vendor as PartitionByVendor,
    api.is_person as PartitionByPerson,
    api.is_static_api as IsStaticApi,
    api.is_data as IsData,
    api.is_businesslogic as IsBusinesslogic,
    api.is_pay as IsPay,
    ISNULL(apiml.fee_description, api.fee_description) as FeeDescription,
    ISNULL(apiml.resource_create_user, api.resource_create_user) as ResourceCreateUser,
    ISNULL(apiml.resource_maintainer, api.resource_maintainer) as ResourceMaintainer,
    api.resource_create_date as ResourceCreateDate,
    api.resource_latest_date as ResourceLatestDate,
    ISNULL(apiml.update_frequency, api.update_frequency) as UpdateFrequency,
    api.is_contract as NeedContract,
    ISNULL(apiml.contact_information, api.contact_information) as ContactInformation,
    ISNULL(apiml.version, api.version) as version,
    ISNULL(apiml.agree_description, api.agree_description) as AgreeDescription,
    api.is_visible_agreement as IsVisibleAgreement,
    api.is_enable as IsEnable,
    api.is_active & ISNULL(vdr.is_active, 0) & ISNULL(vdr.is_enable, 0) & ISNULL(sys.is_active, 0) & ISNULL(sys.is_enable, 0) as IsActive,
    controller_schema_id as ApiSchemaId,
    controller_repository_key as RepositoryKey,
    (SELECT MAX(upd_date) FROM (VALUES (api.upd_date), (vdr.upd_date), (vl.upd_date), (sys.upd_date), (sl.upd_date)) as LIST(upd_date)) as UpdDate,
    vl.url as VendorUrl,
    sl.url as SystemUrl,
    method.api_id as MethodId,
    method.method_type as HttpMethod,
    method.url as RelativePath,
    ISNULL(methodml.api_description, method.api_description) as Description,
    method.is_header_authentication as AuthVendorSystem,
    method.is_openid_authentication as AuthOpenId,
    method.is_admin_authentication as AuthAdmin,
    method.is_vendor_system_authentication_allow_null as IsVendorSystemAuthenticationAllowNull,
    IIF(method.post_data_type = 'array', 1, 0) as PostArray,
    method.url_schema_id as UrlSchemaId,
    method.request_schema_id as RequestSchemaId,
    method.response_schema_id as ResponseSchemaId,
    method.is_visible_signinuser_only as IsVisibleSigninuserOnly,
    CASE 
        WHEN method.is_enable is null THEN 0
        WHEN rg.is_enable is null THEN method.is_enable 
        ELSE method.is_enable & rg.is_enable 
    END as IsEnable,
    method.is_hidden as IsHidden,
    CASE WHEN method.is_active is null THEN 0
        WHEN rg.is_active is null THEN method.is_active 
        ELSE method.is_active & rg.is_active
    END as IsActive,
    method.upd_date as UpdDate,
    at.action_type_name AS ActionType
FROM
    Controller AS api
    LEFT JOIN Locale loc ON loc.locale_code = @localeCode AND loc.is_active = 1
    LEFT JOIN ControllerMultiLanguage apiml ON api.controller_id = apiml.controller_id AND apiml.locale_code = loc.locale_code AND apiml.is_active = 1
    INNER JOIN Vendor vdr ON vdr.vendor_id = api.vendor_id
    LEFT JOIN VendorMultiLanguage vdrml ON vdr.vendor_id = vdrml.vendor_id AND vdrml.locale_code = loc.locale_code AND vdrml.is_active = 1
    INNER JOIN System sys ON sys.system_id = api.system_id
    LEFT JOIN SystemMultiLanguage sysml ON sys.system_id = sysml.system_id AND sysml.locale_code = loc.locale_code AND sysml.is_active = 1
    LEFT JOIN VendorLink vl ON vl.vendor_id = api.vendor_id AND vl.is_default = 1 AND vl.is_visible = 1 AND vl.is_active = 1 
    LEFT JOIN SystemLink sl ON sl.system_id = api.system_id AND sl.is_default = 1 AND sl.is_visible = 1 AND sl.is_active = 1
    LEFT JOIN Api method ON method.controller_id = api.controller_id AND method.is_transparent_api = 0
    LEFT JOIN ApiMultiLanguage methodml ON method.api_id = methodml.api_id AND methodml.locale_code = loc.locale_code AND methodml.is_active = 1
    LEFT JOIN ActionType at ON at.action_type_cd = method.action_type_cd
    LEFT JOIN RepositoryGroup rg ON method.repository_group_id=rg.repository_group_id
WHERE
/*ds if isEnableOnly != null*/
    api.is_enable = 1 AND
    method.is_enable = 1 AND
/*ds end if*/
/*ds if isActiveOnly != null*/
    api.is_active = 1 AND
    method.is_active = 1 AND
/*ds end if*/
/*ds if isHidden != null*/
    method.is_hidden = 0 AND
/*ds end if*/
    vdr.is_active = 1 AND 
    vdr.is_enable = 1 AND 
    sys.is_active = 1 AND 
    sys.is_enable = 1
ORDER BY
    api.url,
    method.url
";

                getFieldSql = @"
SELECT
    cf.controller_id as ApiId,
    f.field_id as FieldId,
    f.parent_field_id as ParentFieldId,
    ISNULL(ml.field_name, f.field_name) as FieldName,
    f.is_active & cf.is_active as IsActive,
    IIF(f.upd_date > cf.upd_date, f.upd_date, cf.upd_date) UpdDate
FROM ControllerField cf
INNER JOIN Field f on f.field_id = cf.field_id
LEFT JOIN Locale loc ON 
    loc.locale_code = @localeCode AND 
    loc.is_active = 1
LEFT JOIN FieldMultiLanguage ml ON 
    f.field_id = ml.field_id AND 
    ml.locale_code = loc.locale_code AND 
    ml.is_active = 1";

                getTagSql = @"
SELECT
    ct.controller_id as ApiId,
    t.tag_id as TagId,
    t.parent_tag_id as ParentTagId,
    t.tag_type_id as TagTypeId,
    tt.tag_type_name as TagTypeName,
    ISNULL(ml.tag_name, t.tag_name) as TagName,
    t.tag_code as Code,
    t.tag_code2 as Code2,
    tt.is_active & t.is_active & ct.is_active as IsActive,
    (SELECT MAX(upd_date) FROM (VALUES (ct.upd_date), (t.upd_date), (tt.upd_date)) as LIST(upd_date)) as UpdDate
FROM ControllerTag ct
INNER JOIN Tag t on t.tag_id = ct.tag_id
INNER JOIN TagType tt on tt.tag_type_id = t.tag_type_id
LEFT JOIN Locale loc ON 
    loc.locale_code = @localeCode AND 
    loc.is_active = 1
LEFT JOIN TagMultiLanguage ml ON 
    t.tag_id = ml.tag_id AND 
    ml.locale_code = loc.locale_code AND 
    ml.is_active = 1";

                getCategorySql = @"
SELECT
    cc.controller_id as ApiId,
    c.category_id as CategoryId,
    ISNULL(ml.category_name, c.category_name) as CategoryName,
    c.sort_order as DisplayOrder,
    c.is_active & cc.is_active as IsActive,
    IIF(c.upd_date > cc.upd_date, c.upd_date, cc.upd_date) UpdDate
FROM ControllerCategory cc 
INNER JOIN Category c on c.category_id = cc.category_id
LEFT JOIN Locale loc ON 
    loc.locale_code = @localeCode AND 
    loc.is_active = 1
LEFT JOIN CategoryMultiLanguage ml ON 
    c.category_id = ml.category_id AND 
    ml.locale_code = loc.locale_code AND 
    ml.is_active = 1";

                getMethodLinkSql = @"
SELECT 
    api_id as MethodId,
    api_link_id as MethodLinkId,
    title,
    detail,
    url,
    is_visible as IsVisible,
    is_active as IsActive,
    upd_date as UpdDate
FROM ApiLink";

                getSampleCodeSql = @"
SELECT 
    sc.api_id as MethodId,
    sc.sample_code_id as SampleCodeId,
    l.language_name as Language,
    l.order_no as DisplayOrder,
    sc.code,
    l.is_active & sc.is_active as IsActive,
    IIF(l.upd_date > sc.upd_date, l.upd_date, sc.upd_date) as UpdDate
FROM SampleCode sc
JOIN Language l on sc.language_id = l.language_id";
            }
            #endregion

            // セカンダリリポジトリグループ一覧取得
            var secRep = this.GetSecondaryRepositoryGroupList();
            var param = new { localeCode };
            var apiDic = new Dictionary<Guid, ApiDescriptionModel>();
            var isHidden = !isNotHiddenOnly;
            var dict = new Dictionary<string, object>();
            dict.Add("localeCode", localeCode);
            dict.Add("isActiveOnly", isActiveOnly ? "1" : null);
            dict.Add("isEnableOnly", isEnableOnly ? "1" : null);
            dict.Add("isHidden", isNotHiddenOnly ? "0" : null);
            dict.Add("localeCode", localeCode);
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, dict);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var apiDescriptionList = Connection.Query<ApiDescriptionModel, MethodDescriptionModel, ApiDescriptionModel>(
                twowaySql.Sql,
                (api, method) =>
                {
                    ApiDescriptionModel apiDescription = null;
                    // 子要素を追加
                    if (method != null && method.MethodId != Guid.Empty && this.IsSecondaryRepositoryGroupEnabled(method.MethodId, secRep))
                    {
                        // isActiveとisEnableはSQLの結果からしか判断できないパターンがあるのでここでやる
                        if (isActiveOnly && method.IsActive != isActiveOnly)
                        {
                            return null;
                        }
                        if (isEnableOnly && method.IsEnable != isEnableOnly)
                        {
                            return null;
                        }

                        apiDescription = api;
                        if (api.Methods == null) { api.Methods = new List<MethodDescriptionModel>(); }
                        apiDescription.Methods.Add(method);
                    }
                    else if (!isActiveOnly && !isEnableOnly)
                    {
                        // 全部取得するパターンはメソッドがなくても追加
                        apiDescription = api;
                    }
                    return apiDescription;
                },
                splitOn: "MethodId",
                param: dynParams);

            foreach (var row in apiDescriptionList)
            {
                if (row == null) continue;
                if (row.ApiId == null) continue;
                if (!apiDic.TryGetValue(row.ApiId, out var apiDescription))
                {
                    if (row.Methods == null)
                    {
                        row.Methods = new List<MethodDescriptionModel>();
                    }
                    // 親を辞書に登録
                    apiDic.Add(row.ApiId, row);
                    continue;
                }
                if (row.Methods != null && row.Methods.FirstOrDefault() != null)
                {
                    apiDescription.Methods.Add(row.Methods.FirstOrDefault());
                }
            }

            // カテゴリーを取得
            var getCategoryTwowaySql = new TwowaySqlParser(dbsettings.GetDbType(), getCategorySql, param);
            var categories = Connection.Query<CategoryModel>(getCategoryTwowaySql.Sql, dynParams);


            // 分野を取得
            var getFieldTwowaySql = new TwowaySqlParser(dbsettings.GetDbType(), getFieldSql, param);
            var fields = Connection.Query<FieldModel>(getFieldTwowaySql.Sql, dynParams);


            // タグを取得
            var getTagTwowaySql = new TwowaySqlParser(dbsettings.GetDbType(), getTagSql, param);
            var tags = Connection.Query<TagModel>(getTagTwowaySql.Sql, dynParams);


            // メソッドのリンク情報を取得
            var getMethodLinkTwowaySql = new TwowaySqlParser(dbsettings.GetDbType(), getMethodLinkSql, null);
            var methodLinks = Connection.Query<MethodLinkModel>(getMethodLinkTwowaySql.Sql);


            // サンプルコードを取得
            var getSampleCodeTwowaySql = new TwowaySqlParser(dbsettings.GetDbType(), getSampleCodeSql, null);
            var sampleCodes = Connection.Query<SampleCode>(getSampleCodeTwowaySql.Sql);

            foreach (var kvp in apiDic)
            {
                // カテゴリーを設定
                kvp.Value.Categories = categories.Where(c => c.ApiId == kvp.Key);
                // 分野を設定
                kvp.Value.Fields = fields.Where(f => f.ApiId == kvp.Key);
                // タグを設定
                kvp.Value.Tags = tags.Where(f => f.ApiId == kvp.Key);

                foreach (var method in kvp.Value.Methods)
                {
                    // メソッドのリンク情報を設定
                    method.MethodLinks = methodLinks.Where(m => m.MethodId == method.MethodId);
                    // サンプルコードを設定
                    method.SampleCodes = sampleCodes.Where(s => s.MethodId == method.MethodId);
                }
            }
            return apiDic.Values;
        }

        /// <summary>
        /// スキーマ情報の一覧を取得します。
        /// </summary>
        /// <param name="localeCode">ロケール</param>
        /// <returns>スキーマ情報の一覧</returns>
        [CacheArg(allParam: true)]
        [CacheEntity(DynamicApiDatabase.TABLE_DATASCHEMA)]
        [Cache]
        public IEnumerable<SchemaDescriptionModel> GetSchemaDescription(string localeCode = null)
        {
            if (string.IsNullOrWhiteSpace(localeCode))
            {
                localeCode = DefaultLocaleCode;
            }

            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var param = new { localeCode };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), SelectSchemaSql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<SchemaDescriptionModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// カテゴリーの一覧を取得します。
        /// </summary>
        /// <returns>カテゴリーの一覧</returns>
        [CacheArg(allParam: true)]
        [CacheEntity(DynamicApiDatabase.TABLE_CATEGORY)]
        [Cache]
        public IEnumerable<CategoryModel> GetCategoryList(string localeCode = null)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.category_id AS categoryId,
    COALESCE(ml.category_name, c.category_name) AS categoryName,
    c.sort_order AS displayOrder,
    c.is_active AS isActive,
    c.upd_date AS updDate
FROM CATEGORY c 
LEFT JOIN LOCALE loc ON 
    loc.locale_code = /*ds localeCode*/'a' AND 
    loc.is_active = 1
LEFT JOIN CATEGORY_MULTI_LANGUAGE ml ON 
    c.category_id = ml.category_id AND 
    ml.locale_code = loc.locale_code AND 
    ml.is_active = 1
WHERE c.is_active = 1
ORDER BY sort_order";
            }
            else
            {
                sql = @"
SELECT
    c.category_id AS categoryId,
    ISNULL(ml.category_name, c.category_name) AS categoryName,
    c.sort_order AS displayOrder,
    c.is_active AS isActive,
    c.upd_date AS updDate
FROM Category c 
LEFT JOIN Locale loc ON 
    loc.locale_code = @localeCode AND 
    loc.is_active = 1
LEFT JOIN CategoryMultiLanguage ml ON 
    c.category_id = ml.category_id AND 
    ml.locale_code = loc.locale_code AND 
    ml.is_active = 1
WHERE c.is_active = 1
ORDER BY sort_order";
            }

            if (string.IsNullOrWhiteSpace(localeCode))
            {
                localeCode = DefaultLocaleCode;
            }
            var param = new { localeCode };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<CategoryModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// 引数のメソッドIDとセカンダリリポジトリグループ一覧をつきあわせ、有効なメソッドかを判定します。
        /// </summary>
        /// <param name="methodId">メソッドID</param>
        /// <param name="secRep">セカンダリリポジトリグループ一覧</param>
        /// <returns>有効な場合はtrue、無効な場合はfalse</returns>
        private bool IsSecondaryRepositoryGroupEnabled(Guid methodId, IEnumerable<SecondaryRepositoryGroupModel> secRep)
        {
            if (secRep.Count() == 0)
            {
                return true;
            }

            return secRep.Where(x => x.MethodId == methodId && !x.IsEnable).Count() == 0;
        }

        /// <summary>
        /// セカンダリリポジトリグループ一覧を取得します。
        /// </summary>
        /// <returns>セカンダリリポジトリグループ一覧</returns>
        private IEnumerable<SecondaryRepositoryGroupModel> GetSecondaryRepositoryGroupList()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {

                sql = @"
SELECT
    a.api_id     AS MethodId, 
    rg.is_enable AS IsEnable
FROM
    CONTROLLER c
    INNER JOIN API a ON c.controller_id= a.controller_id AND a.is_active= 1
    INNER JOIN SECONDARY_REPOSITORY_MAP secrg ON a.api_id= secrg.api_id AND secrg.is_active= 1
    INNER JOIN REPOSITORY_GROUP rg ON secrg.repository_group_id= rg.repository_group_id AND rg.is_active= 1
WHERE
    c.is_active= 1
";
            }
            else
            {
                sql = @"
SELECT
    a.api_id     AS MethodId, 
    rg.is_enable AS IsEnable
FROM
    Controller c
    INNER JOIN Api a ON c.controller_id= a.controller_id AND a.is_active= 1
    INNER JOIN SecondaryRepositoryMap secrg ON a.api_id= secrg.api_id AND secrg.is_active= 1
    INNER JOIN RepositoryGroup rg ON secrg.repository_group_id= rg.repository_group_id AND rg.is_active= 1
WHERE
    c.is_active= 1
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            return Connection.Query<SecondaryRepositoryGroupModel>(twowaySql.Sql);
        }
        #endregion
    }
}
