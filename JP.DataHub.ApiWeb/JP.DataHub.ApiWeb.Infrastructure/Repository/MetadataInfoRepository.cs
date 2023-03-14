using JP.DataHub.Com.Cache;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.MetadataInfo;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    /// <summary>
    /// APIの説明を取得するクエリーです。
    /// </summary>
    class MetadataInfoRepository : AbstractDynamicApiRepository, IMetadataInfoRepository
    {
        #region sql
        private string ApiColumns
        {
            get
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
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
    CASE WHEN api.is_active != 0 AND COALESCE(vdr.is_active, 0) != 0 AND COALESCE(vdr.is_enable, 0) != 0 AND COALESCE(sys.is_active, 0) != 0 AND COALESCE(sys.is_enable, 0) != 0 THEN 1 ELSE 0 END as IsActive,
    controller_schema_id as ApiSchemaId,
    controller_repository_key as RepositoryKey,";
                }
                else
                {
                    sql = @"
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
    controller_repository_key as RepositoryKey,";
                }
                return sql;
            }
        }

        private string MethodColumns
        {
            get
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
    method.api_id as MethodId,
    method.method_type as HttpMethod,
    method.url as RelativePath,
    COALESCE(TO_NCHAR(methodml.api_description), method.api_description) as Description,
    method.is_header_authentication as AuthVendorSystem,
    method.is_openid_authentication as AuthOpenId,
    method.is_admin_authentication as AuthAdmin,
    method.is_vendor_system_authentication_allow_null as IsVendorSystemAuthenticationAllowNull,
    CASE WHEN post_data_type = 'array' THEN 1 ELSE 0 END as PostArray,
    method.url_schema_id as UrlSchemaId,
    method.request_schema_id as RequestSchemaId,
    method.response_schema_id as ResponseSchemaId,
    method.is_visible_signinuser_only as IsVisibleSigninuserOnly,
	CASE 
		WHEN method.is_enable is null THEN 0
		WHEN rg.is_enable is null THEN method.is_enable 
		ELSE (CASE WHEN method.is_enable != 0 AND rg.is_enable != 0 THEN 1 ELSE 0 END)
	END as IsEnable,
    method.is_hidden as IsHidden,
	CASE WHEN method.is_active is null THEN 0
		WHEN rg.is_active is null THEN method.is_active 
		ELSE (CASE WHEN method.is_enable != 0 AND rg.is_enable != 0 THEN 1 ELSE 0 END)
	END as IsActive,
    method.upd_date as UpdDate,
";
                }
                else
                {
                    sql = @"
    method.api_id as MethodId,
    method.method_type as HttpMethod,
    method.url as RelativePath,
    ISNULL(methodml.api_description, method.api_description) as Description,
    method.is_header_authentication as AuthVendorSystem,
    method.is_openid_authentication as AuthOpenId,
    method.is_admin_authentication as AuthAdmin,
    method.is_vendor_system_authentication_allow_null as IsVendorSystemAuthenticationAllowNull,
    IIF(method.post_data_type = 'array', CONVERT(BIT, 1), CONVERT(BIT, 0)) as PostArray,
    method.url_schema_id as UrlSchemaId,
    method.request_schema_id as RequestSchemaId,
    method.response_schema_id as ResponseSchemaId,
    method.is_visible_signinuser_only as IsVisibleSigninuserOnly,
	CASE 
		WHEN method.is_enable is null THEN CONVERT(BIT, 0)
		WHEN rg.is_enable is null THEN method.is_enable 
		ELSE method.is_enable & rg.is_enable 
	END as IsEnable,
    method.is_hidden as IsHidden,
	CASE WHEN method.is_active is null THEN CONVERT(BIT, 0)
		WHEN rg.is_active is null THEN method.is_active 
		ELSE method.is_active & rg.is_active
	END as IsActive,
    method.upd_date as UpdDate,
";
                }
                return sql;
            }
        }

        private string SelectSchemaSql
        {
            get
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT
    ds.data_schema_id as SchemaId,
    ds.schema_name as SchemaName,
    ds.data_schema as JsonSchema,
    COALESCE(TO_NCHAR(dsml.schema_description), ds.schema_description) as Description,
    ds.is_active as IsActive,
    ds.upd_date as UpdDate
FROM data_schema ds
LEFT JOIN locale loc ON 
    loc.locale_code = /*ds localeCode*/'' AND 
    loc.is_active = 1
LEFT JOIN data_schema_multi_language dsml ON 
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


        private IJPDataHubDbConnection conn = UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi");
        private string DefaultLocaleCode = UnityCore.Resolve<string>("DefaultCulture");


        /// <summary>
        /// API情報の一覧を取得します。
        /// </summary>
        /// <param name="noChildren">子の情報なしフラグ</param>
        /// <param name="localeCode">ロケール</param>
        /// <param name="isActiveOnly">未削除のみ</param>
        /// <param name="isEnableOnly">有効のみ</param>
        /// <param name="isNotHiddenOnly">公開のみ</param>
        /// <returns>API情報の一覧</returns>
        public IList<ApiDescription> GetApiDescription(bool noChildren, string localeCode = null, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            return Cache.Get<IList<ApiDescription>>(CacheManager.CreateKey(CACHE_KEY_API_LIST_DESCRIPTION, noChildren, localeCode, isActiveOnly, isEnableOnly, isNotHiddenOnly), CacheExpirationTimeSpan, () =>
            {
                IList<ApiDescription> apiDescriptions;

                if (string.IsNullOrWhiteSpace(localeCode))
                {
                    localeCode = DefaultLocaleCode;
                }

                if (noChildren)
                {
                    var sql = "";
                    if (dbSettings.Type == "Oracle")
                    {
                        sql = $@"
SELECT
    {ApiColumns}
    GREATEST(api.upd_date, vdr.upd_date, sys.upd_date) UpdDate
FROM controller api
LEFT JOIN locale loc ON 
    loc.locale_code = /*ds localeCode*/'' AND 
    loc.is_active = 1
LEFT JOIN controller_multi_language apiml ON 
    api.controller_id = apiml.controller_id AND 
    apiml.locale_code = loc.locale_code AND 
    apiml.is_active = 1
INNER JOIN vendor vdr ON 
    vdr.vendor_id = api.vendor_id
LEFT JOIN vendor_multi_language vdrml ON 
    vdr.vendor_id = vdrml.vendor_id AND 
    vdrml.locale_code = loc.locale_code AND 
    vdrml.is_active = 1
INNER JOIN system sys ON 
    sys.system_id = api.system_id
LEFT JOIN system_multi_language sysml ON 
    sys.system_id = sysml.system_id AND 
    sysml.locale_code = loc.locale_code AND 
    sysml.is_active = 1
WHERE 
/*ds if isActiveOnly != null*/
    api.is_active = 1 AND
/*ds end if*/
/*ds if isEnableOnly != null*/
    api.is_enable = 1 AND
/*ds end if*/
    vdr.is_active = 1 AND 
    vdr.is_enable = 1 AND 
    sys.is_active = 1 AND 
    sys.is_enable = 1";
                    }
                    else
                    {
                        sql = $@"
SELECT
    {ApiColumns}
    (SELECT MAX(upd_date) FROM (VALUES (api.upd_date), (vdr.upd_date), (sys.upd_date)) as LIST(upd_date)) as UpdDate
FROM Controller api
LEFT JOIN Locale loc ON 
    loc.locale_code = @localeCode AND 
    loc.is_active = 1
LEFT JOIN ControllerMultiLanguage apiml ON 
    api.controller_id = apiml.controller_id AND 
    apiml.locale_code = loc.locale_code AND 
    apiml.is_active = 1
INNER JOIN Vendor vdr ON 
    vdr.vendor_id = api.vendor_id
LEFT JOIN VendorMultiLanguage vdrml ON 
    vdr.vendor_id = vdrml.vendor_id AND 
    vdrml.locale_code = loc.locale_code AND 
    vdrml.is_active = 1
INNER JOIN System sys ON 
    sys.system_id = api.system_id
LEFT JOIN SystemMultiLanguage sysml ON 
    sys.system_id = sysml.system_id AND 
    sysml.locale_code = loc.locale_code AND 
    sysml.is_active = 1
WHERE 
/*ds if isActiveOnly != null*/
    api.is_active = 1 AND
/*ds end if*/
/*ds if isEnableOnly != null*/
    api.is_enable = 1 AND
/*ds end if*/
    vdr.is_active = 1 AND 
    vdr.is_enable = 1 AND 
    sys.is_active = 1 AND 
    sys.is_enable = 1";
                    }

                    // API情報を取得
                    var dict = new Dictionary<string, object>();
                    dict.Add("localeCode", localeCode);
                    dict.Add("isActiveOnly", isActiveOnly ? "1" : null);
                    dict.Add("isEnableOnly", isEnableOnly ? "1" : null);
                    var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, dict);
                    var dynParams = dbSettings.GetParameters().AddDynamicParams(dict);
                    apiDescriptions = conn.Query<ApiDescription>(twowaySql.Sql, dynParams).ToList();
                }
                else
                {
                    // 子要素を含めてAPI情報を取得
                    apiDescriptions = GetApiDescriptionWithChildren(localeCode, isActiveOnly, isEnableOnly, isNotHiddenOnly).ToList();
                }

                return apiDescriptions;
            });
        }

        /// <summary>
        /// スキーマ情報の一覧を取得します。
        /// </summary>
        /// <param name="localeCode">ロケール</param>
        /// <returns>スキーマ情報の一覧</returns>
        public IList<SchemaDescription> GetSchemaDescription(string localeCode = null)
        {
            if (string.IsNullOrWhiteSpace(localeCode))
            {
                localeCode = DefaultLocaleCode;
            }

            return Cache.Get<IList<SchemaDescription>>(CacheManager.CreateKey(CACHE_KEY_API_LIST_SCHEMA_DESCRIPTION, localeCode), CacheExpirationTimeSpan, () =>
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                string sql = SelectSchemaSql;
                var param = new
                {
                    localeCode = localeCode,
                };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                return conn.Query<SchemaDescription>(twowaySql.Sql, dynParams);
            });
        }


        /// <summary>
        /// 子要素を含めたAPI情報の一覧を取得します。
        /// </summary>
        /// <param name="localeCode">ロケール</param>
        /// <param name="isActiveOnly">未削除のみ</param>
        /// <param name="isEnableOnly">有効のみ</param>
        /// <param name="isNotHiddenOnly">公開のみ</param>
        /// <returns>API情報の一覧</returns>
        private IEnumerable<ApiDescription> GetApiDescriptionWithChildren(string localeCode, bool isActiveOnly = false, bool isEnableOnly = false, bool isNotHiddenOnly = false)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();

            #region sql

            string sql = "";
            string getFieldSql = "";
            string getTagSql = "";
            string getCategoryeSql = "";
            string getMethodLinkSql = "";
            string getSampleCodeSql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = $@"
SELECT
    {ApiColumns}
    GREATEST(api.upd_date, vdr.upd_date, vl.upd_date, sys.upd_date, sl.upd_date) as UpdDate,
    vl.url as VendorUrl,
    sl.url as SystemUrl,
    {MethodColumns}
    at.action_type_name AS ActionType
FROM controller api
LEFT JOIN locale loc ON 
    loc.locale_code = /*ds localeCode*/'' AND 
    loc.is_active = 1
LEFT JOIN controller_multi_language apiml ON 
    api.controller_id = apiml.controller_id AND 
    apiml.locale_code = loc.locale_code AND 
    apiml.is_active = 1
INNER JOIN vendor vdr ON 
    vdr.vendor_id = api.vendor_id
LEFT JOIN vendor_multi_language vdrml ON 
    vdr.vendor_id = vdrml.vendor_id AND 
    vdrml.locale_code = loc.locale_code AND 
    vdrml.is_active = 1
INNER JOIN system sys ON 
    sys.system_id = api.system_id
LEFT JOIN system_multi_language sysml ON 
    sys.system_id = sysml.system_id AND 
    sysml.locale_code = loc.locale_code AND 
    sysml.is_active = 1
LEFT JOIN vendor_link vl ON 
    vl.vendor_id = api.vendor_id AND 
    vl.is_default = 1 AND 
    vl.is_visible = 1 AND 
    vl.is_active = 1 
LEFT JOIN system_link sl ON 
    sl.system_id = api.system_id AND 
    sl.is_default = 1 AND 
    sl.is_visible = 1 AND 
    sl.is_active = 1
LEFT JOIN api method ON 
    method.controller_id = api.controller_id AND 
    method.is_transparent_api = 0
LEFT JOIN api_multi_language methodml ON 
    method.api_id = methodml.api_id AND 
    methodml.locale_code = loc.locale_code AND 
    methodml.is_active = 1
LEFT JOIN action_type at ON 
    at.action_type_cd = method.action_type_cd
LEFT JOIN repository_group rg ON 
    method.repository_group_id=rg.repository_group_id
WHERE
/*ds if isActiveOnly != null*/
    api.is_active = 1 AND
    method.is_active = 1 AND
/*ds end if*/
/*ds if isEnableOnly != null*/
    api.is_enable = 1 AND
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
    method.url
";

                getFieldSql = @"
SELECT
    cf.controller_id as ApiId,
    f.field_id as FieldId,
    f.parent_field_id as ParentFieldId,
    COALESCE(ml.field_name, f.field_name) as FieldName,
    CASE WHEN f.is_active != 0 AND cf.is_active != 0 THEN 1 ELSE 0 END as IsActive,
    CASE WHEN f.upd_date > cf.upd_date THEN f.upd_date ELSE cf.upd_date END as UpdDate
FROM controller_field cf
INNER JOIN field f on f.field_id = cf.field_id
LEFT JOIN locale loc ON 
    loc.locale_code = /*ds localeCode*/'' AND 
    loc.is_active = 1
LEFT JOIN field_multi_language ml ON 
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
    CASE WHEN COALESCE(tt.is_active, 0) != 0 AND COALESCE(t.is_active, 0) != 0 AND COALESCE(ct.is_active, 0) != 0 THEN 1 ELSE 0 END as IsActive,
    GREATEST(ct.upd_date, t.upd_date, tt.upd_date) as UpdDate
FROM controller_tag ct
INNER JOIN tag t on t.tag_id = ct.tag_id
INNER JOIN tag_type tt on tt.tag_type_id = t.tag_type_id
LEFT JOIN locale loc ON 
    loc.locale_code = /*ds localeCode*/'' AND 
    loc.is_active = 1
LEFT JOIN tag_multi_language ml ON 
    t.tag_id = ml.tag_id AND 
    ml.locale_code = loc.locale_code AND 
    ml.is_active = 1";

                getCategoryeSql = @"
SELECT
    cc.controller_id as ApiId,
    c.category_id as CategoryId,
    COALESCE(ml.category_name, c.category_name) as CategoryName,
    c.sort_order as DisplayOrder,
    CASE WHEN c.is_active != 0 AND cc.is_active != 0 THEN 1 ELSE 0 END as IsActive,
    CASE WHEN c.upd_date > cc.upd_date THEN c.upd_date ELSE cc.upd_date END as UpdDate
FROM controller_category cc 
INNER JOIN category c on c.category_id = cc.category_id
LEFT JOIN locale loc ON 
    loc.locale_code = /*ds localeCode*/'' AND 
    loc.is_active = 1
LEFT JOIN category_multi_language ml ON 
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
FROM api_link";

                getSampleCodeSql = @"
SELECT 
    sc.api_id as MethodId,
    sc.sample_code_id as SampleCodeId,
    l.language_name as Language,
    l.order_no as DisplayOrder,
    sc.code,
    CASE WHEN l.is_active != 0 AND sc.is_active != 0 THEN 1 ELSE 0 END as IsActive,
    CASE WHEN l.upd_date > sc.upd_date THEN l.upd_date ELSE sc.upd_date END as UpdDate
FROM sample_code sc
JOIN language l on sc.language_id = l.language_id";
            }
            else
            {
                sql = $@"
SELECT
    {ApiColumns}
    (SELECT MAX(upd_date) FROM (VALUES (api.upd_date), (vdr.upd_date), (vl.upd_date), (sys.upd_date), (sl.upd_date)) as LIST(upd_date)) as UpdDate,
    vl.url as VendorUrl,
    sl.url as SystemUrl,
    {MethodColumns}
    at.action_type_name AS ActionType
FROM Controller api
LEFT JOIN Locale loc ON 
    loc.locale_code = @localeCode AND 
    loc.is_active = 1
LEFT JOIN ControllerMultiLanguage apiml ON 
    api.controller_id = apiml.controller_id AND 
    apiml.locale_code = loc.locale_code AND 
    apiml.is_active = 1
INNER JOIN Vendor vdr ON 
    vdr.vendor_id = api.vendor_id
LEFT JOIN VendorMultiLanguage vdrml ON 
    vdr.vendor_id = vdrml.vendor_id AND 
    vdrml.locale_code = loc.locale_code AND 
    vdrml.is_active = 1
INNER JOIN System sys ON 
    sys.system_id = api.system_id
LEFT JOIN SystemMultiLanguage sysml ON 
    sys.system_id = sysml.system_id AND 
    sysml.locale_code = loc.locale_code AND 
    sysml.is_active = 1
LEFT JOIN VendorLink vl ON 
    vl.vendor_id = api.vendor_id AND 
    vl.is_default = 1 AND 
    vl.is_visible = 1 AND 
    vl.is_active = 1 
LEFT JOIN SystemLink sl ON 
    sl.system_id = api.system_id AND 
    sl.is_default = 1 AND 
    sl.is_visible = 1 AND 
    sl.is_active = 1
LEFT JOIN Api method ON 
    method.controller_id = api.controller_id AND 
    method.is_transparent_api = 0
LEFT JOIN ApiMultiLanguage methodml ON 
    method.api_id = methodml.api_id AND 
    methodml.locale_code = loc.locale_code AND 
    methodml.is_active = 1
LEFT JOIN ActionType at ON 
    at.action_type_cd = method.action_type_cd
LEFT JOIN RepositoryGroup rg ON 
    method.repository_group_id=rg.repository_group_id
WHERE
/*ds if isActiveOnly != null*/
    api.is_active = 1 AND
    method.is_active = 1 AND
/*ds end if*/
/*ds if isEnableOnly != null*/
    api.is_enable = 1 AND
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

                getCategoryeSql = @"
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
            var param = new
            {
                localeCode = localeCode,
            };
            var apiDic = new Dictionary<Guid, ApiDescription>();
            var isHidden = !isNotHiddenOnly;
            var dict = new Dictionary<string, object>();
            dict.Add("localeCode", localeCode);
            dict.Add("isActiveOnly", isActiveOnly ? "1" : null);
            dict.Add("isEnableOnly", isEnableOnly ? "1" : null);
            dict.Add("isHidden", isNotHiddenOnly ? "0" : null);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, dict);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var apiDescriptionList = conn.Query<ApiDescription, MethodDescription, ApiDescription>(
                twowaySql.Sql,
                (api, method) =>
                {
                    ApiDescription apiDescription = null;
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
                        if (api.Methods == null) { api.Methods = new List<MethodDescription>(); }
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
                        row.Methods = new List<MethodDescription>();
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
            var categoryeSqlParam = new Dictionary<string, object>();
            categoryeSqlParam.Add("localeCode", localeCode);
            var twowayCategoryeSql = new TwowaySqlParser(dbSettings.GetDbType(), getCategoryeSql, categoryeSqlParam);
            var categories = conn.Query<Category>(twowayCategoryeSql.Sql, dynParams);
            // 分野を取得
            var fieldSqlParam = new Dictionary<string, object>();
            fieldSqlParam.Add("localeCode", localeCode);
            var twowayFieldSql = new TwowaySqlParser(dbSettings.GetDbType(), getFieldSql, fieldSqlParam);
            var fields = conn.Query<Field>(twowayFieldSql.Sql, dynParams);
            // タグを取得
            var tagSqlParam = new Dictionary<string, object>();
            tagSqlParam.Add("localeCode", localeCode);
            var twowayTagSql = new TwowaySqlParser(dbSettings.GetDbType(), getTagSql, tagSqlParam);
            var tags = conn.Query<Tag>(twowayTagSql.Sql, dynParams);
            // メソッドのリンク情報を取得
            var methodLinkSqlParam = new Dictionary<string, object>();
            var twowayMethodLinkSql = new TwowaySqlParser(dbSettings.GetDbType(), getMethodLinkSql, methodLinkSqlParam);
            var methodLinks = conn.Query<MethodLink>(twowayMethodLinkSql.Sql);
            // サンプルコードを取得
            var sampleCodeSqlParam = new Dictionary<string, object>();
            var twowaySampleCodeSql = new TwowaySqlParser(dbSettings.GetDbType(), getSampleCodeSql, sampleCodeSqlParam);
            var sampleCodes = conn.Query<SampleCode>(twowaySampleCodeSql.Sql);

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
        /// 引数のメソッドIDとセカンダリリポジトリグループ一覧をつきあわせ、有効なメソッドかを判定します。
        /// </summary>
        /// <param name="methodId">メソッドID</param>
        /// <param name="secRep">セカンダリリポジトリグループ一覧</param>
        /// <returns>有効な場合はtrue、無効な場合はfalse</returns>
        private bool IsSecondaryRepositoryGroupEnabled(Guid methodId, IEnumerable<SecondaryRepositoryGroup> secRep)
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
        private IEnumerable<SecondaryRepositoryGroup> GetSecondaryRepositoryGroupList()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string SelectSecondaryRepositorySql = "";
            if (dbSettings.Type == "Oracle")
            {
                SelectSecondaryRepositorySql = @"
SELECT
    a.api_id     AS MethodId, 
    rg.is_enable AS IsEnable
FROM
    controller c
    INNER JOIN api a ON c.controller_id= a.controller_id AND a.is_active= 1
    INNER JOIN secondary_repository_map secrg ON a.api_id= secrg.api_id AND secrg.is_active= 1
    INNER JOIN repository_group rg ON secrg.repository_group_id= rg.repository_group_id AND rg.is_active= 1
WHERE
    c.is_active= 1
";
            }
            else
            {
                SelectSecondaryRepositorySql = @"
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
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), SelectSecondaryRepositorySql, null);
            return conn.Query<SecondaryRepositoryGroup>(twowaySql.Sql);
        }
    }
}