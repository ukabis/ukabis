using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.DymamicApi;
using JP.DataHub.ManageApi.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JP.DataHub.Infrastructure.Database.Data;
using JP.DataHub.Infrastructure.Database.Data.MongoDb;
using JP.DataHub.Infrastructure.Database.Consts;
using System.Data.SqlClient;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    /// <summary>
    /// APIに関連するDynamicApiRepository
    /// </summary>
    internal partial class DynamicApiRepository
    {
        #region Api
        /// <summary>
        /// API情報をすべて取得します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns>API情報</returns>
        public ApiInformationModel GetAllFieldApiInformation(string apiId)
        {
            var api = GetApiInformation(apiId);
            api.ApiLinkList = GetApiLinkList(api.ApiId).ToList();
            api.SampleCodeList = GetSampleCodeList(api.ApiId).ToList();
            api.OpenIdCaList = GetApiOpenIdCaList(api.ApiId);
            api.SecondaryRepositoryMapList = GetSecondaryRepositoryMapList(api.ApiId, api.VendorId);
            return api;
        }

        /// <summary>
        /// API情報を取得します。
        /// </summary>
        /// <param name="apiId">APIID</param>
        /// <returns>API情報</returns>
        public ApiInformationModel GetApiInformation(string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.controller_id AS ControllerId
    ,c.url AS ControllerUrl
    ,c.vendor_id AS VendorId
    ,c.is_static_api AS IsStaticApi
    ,a.api_id AS ApiId
    ,a.api_description AS ApiDescription
    ,a.method_type AS MethodType
    ,a.url AS Url
    ,a.repository_key AS RepositoryKey
    ,a.partition_key AS PartitionKey
    ,a.request_schema_id AS RequestSchemaId
    ,a.response_schema_id AS ResponseSchemaId
    ,a.url_schema_id AS UrlSchemaId
    ,a.post_data_type AS PostDataType
    ,a.query AS Query
    ,a.query_type_cd AS QueryType
    ,a.repository_group_id AS RepositoryGroupId
    ,a.is_enable AS IsEnable
    ,a.is_header_authentication AS IsHeaderAuthentication
    ,a.is_vendor_system_authentication_allow_null AS IsVendorSystemAuthenticationAllowNull
    ,a.is_openid_authentication AS IsOpenIdAuthentication
    ,a.is_admin_authentication AS IsAdminAuthentication
    ,a.is_over_partition AS IsOverPartition
    ,a.is_otherresource_sqlaccess AS IsOtherResourceSqlAccess
    ,a.gateway_url AS GatewayUrl
    ,a.gateway_credential_username AS GatewayCredentialUserName
    ,a.gateway_credential_password AS GatewayCredentialPassword
    ,a.gateway_relay_header AS GatewayRelayHeader
    ,a.action_type_cd AS ActionType
    ,a.script_type_cd AS ScriptType
    ,a.script AS Script
    ,a.is_hidden AS IsHidden
    ,a.is_visible_signinuser_only AS IsVisibleSigninuserOnly
    ,a.is_active AS IsActive
    ,a.is_cache AS IsCache
    ,a.cache_minute AS CacheMinute
    ,a.cache_key AS CacheKey
    ,a.is_accesskey AS IsAccessKey
    ,a.is_automatic_id AS IsAutomaticId
    ,a.is_transparent_api AS IsTransparentApi
    ,a.is_internal_call_only AS IsInternalOnly
    ,a.internal_call_keyword AS InternalOnlyKeyword
    ,a.is_skip_jsonschema_validation AS IsSkipJsonSchemaValidation
    ,aav.api_access_vendor_id AS ApiAccessVendorId
    ,aav.api_id AS ApiId 
    ,aav.vendor_id AS VendorId
    ,aav.system_id AS SystemId
    ,aav.access_key AS SystemName
    ,aav.is_enable AS IsEnable
FROM
    API a
    INNER JOIN CONTROLLER c ON a.controller_id = c.controller_id AND c.is_active = 1
    LEFT OUTER JOIN API_ACCESS_VENDOR aav ON a.api_id = aav.api_id AND aav.is_active = 1
WHERE
/*ds if apiId != null*/
    a.api_id = /*ds apiId*/'00000000-0000-0000-0000-000000000000' AND 
/*ds end if*/
    a.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    c.controller_id AS ControllerId
    ,c.url AS ControllerUrl
    ,c.vendor_id AS VendorId
    ,c.is_static_api AS IsStaticApi
    ,a.api_id AS ApiId
    ,a.api_description AS ApiDescription
    ,a.method_type AS MethodType
    ,a.url AS Url
    ,a.repository_key AS RepositoryKey
    ,a.partition_key AS PartitionKey
    ,a.request_schema_id AS RequestSchemaId
    ,a.response_schema_id AS ResponseSchemaId
    ,a.url_schema_id AS UrlSchemaId
    ,a.post_data_type AS PostDataType
    ,a.query AS Query
    ,a.query_type_cd AS QueryType
    ,a.repository_group_id AS RepositoryGroupId
    ,a.is_enable AS IsEnable
    ,a.is_header_authentication AS IsHeaderAuthentication
    ,a.is_vendor_system_authentication_allow_null AS IsVendorSystemAuthenticationAllowNull
    ,a.is_openid_authentication AS IsOpenIdAuthentication
    ,a.is_admin_authentication AS IsAdminAuthentication
    ,a.is_over_partition AS IsOverPartition
    ,a.is_otherresource_sqlaccess AS IsOtherResourceSqlAccess
    ,a.gateway_url AS GatewayUrl
    ,a.gateway_credential_username AS GatewayCredentialUserName
    ,a.gateway_credential_password AS GatewayCredentialPassword
    ,a.gateway_relay_header AS GatewayRelayHeader
    ,a.action_type_cd AS ActionType
    ,a.script_type_cd AS ScriptType
    ,a.script AS Script
    ,a.is_hidden AS IsHidden
    ,a.is_visible_signinuser_only AS IsVisibleSigninuserOnly
    ,a.is_active AS IsActive
    ,a.is_cache AS IsCache
    ,a.cache_minute AS CacheMinute
    ,a.cache_key AS CacheKey
    ,a.is_accesskey AS IsAccessKey
    ,a.is_automatic_id AS IsAutomaticId
    ,a.is_transparent_api AS IsTransparentApi
    ,a.is_internal_call_only AS IsInternalOnly
    ,a.internal_call_keyword AS InternalOnlyKeyword
    ,a.is_skip_jsonschema_validation AS IsSkipJsonSchemaValidation
    ,aav.api_access_vendor_id AS ApiAccessVendorId
    ,aav.api_id AS ApiId 
    ,aav.vendor_id AS VendorId
    ,aav.system_id AS SystemId
    ,aav.access_key AS SystemName
    ,aav.is_enable AS IsEnable
FROM
    Api a
    INNER JOIN Controller c ON a.controller_id = c.controller_id AND c.is_active = 1
    LEFT OUTER JOIN ApiAccessVendor aav ON a.api_id = aav.api_id AND aav.is_active = 1
WHERE
/*ds if apiId != null*/
    a.api_id = /*ds apiId*/'00000000-0000-0000-0000-000000000000' AND 
/*ds end if*/
    a.is_active = 1
";
            }

            var param = new { apiId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var apiInfoDic = new Dictionary<string, ApiInformationModel>();
            var resultList = Connection.Query<ApiInformationModel, ApiAccessVendorModel, ApiInformationModel>(
                sql: twowaySql.Sql,
                map: (api, apiAccessVendor) =>
                {
                    if(!apiInfoDic.TryGetValue(apiId, out ApiInformationModel? result))
                    {
                        // 辞書に新規のAPIを追加
                        apiInfoDic.Add(api.ApiId, api);
                        result = api;
                    }

                    // APIにApiAccessVendorを追加
                    result.ApiAccessVendorList.Add(apiAccessVendor);

                    return result;
                },
                dynParams,
                splitOn: "ApiAccessVendorId"
                );

            return resultList?.FirstOrDefault();
        }



        private string apiBaseSql
        {
            get {

                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT
    c.vendor_id as VendorId
    ,a.api_id as ApiId
    ,a.api_description as ApiDescription
    ,a.controller_id as ControllerId
    ,a.method_type as MethodType
    ,a.url as ApiUrl
    ,a.repository_key as RepositoryKey
    ,a.request_schema_id as RequestSchemaId
    ,dsreq.schema_name as RequestSchemaName
    ,a.response_schema_id as ResponseSchemaId
    ,dsres.schema_name as ResponseSchemaName
    ,a.url_schema_id as UrlSchemaId
    ,dsurl.schema_name as UrlSchemaName
    ,a.post_data_type as PostDataType
    ,a.query as Query
    ,a.repository_group_id as RepositoryGroupId
    ,rg.repository_group_name as RepositoryGroupName
    ,a.is_enable as IsEnable
    ,a.is_header_authentication as IsHeaderAuthentication
    ,a.is_openid_authentication as IsOpenIdAuthentication
    ,a.is_admin_authentication as IsAdminAuthentication
    ,a.is_over_partition as IsOverPartition
    ,a.gateway_url as GatewayUrl
    ,a.gateway_credential_username as GatewayCredentialUserName
    ,a.is_hidden as IsHidden
    ,a.script as Script
    ,a.action_type_cd as ActionTypeCd
    ,a.script_type_cd as ScriptTypeCd
    ,a.is_cache as IsCache
    ,a.cache_minute as CacheMinute
    ,a.cache_key as CacheKey
    ,CASE a.is_accesskey WHEN 1 THEN 'True' ELSE 'False' END as AccessKey
    ,CASE a.is_automatic_id WHEN 1 THEN 'True' ELSE 'False' END as Automatic
    ,a.actiontype_version as ActionTypeVersion
    ,a.partition_key as PartitionKey
    ,a.gateway_relay_header as GatewayRelayHeader
    ,a.reg_date as RegDate
    ,a.reg_username as RegUsername
    ,a.upd_date as UpdDate
    ,a.upd_username as UpdUsername
    ,a.is_active as IsActive
    ,a.is_transparent_api as IsTransparent 
    ,a.is_vendor_system_authentication_allow_null as IsVendorSystemAuthenticationAllowNull
    ,a.is_visible_signinuser_only as IsVisibleSigninUserOnly
    ,a.query_type_cd as QueryTypeCd
    ,a.is_internal_call_only as IsInternalOnly
    ,a.internal_call_keyword as InternalOnlyKeyword
    ,a.is_skip_jsonschema_validation as IsSkipJsonSchemaValidation
    ,a.is_otherresource_sqlaccess as IsOtherResourceSqlAccess
    ,a.is_clientcert_authentication as IsClientCertAuthentication
FROM 
    API a
    LEFT OUTER JOIN REPOSITORY_GROUP rg ON a.repository_group_id=rg.repository_group_id AND rg.is_active= 1
    LEFT OUTER JOIN DATA_SCHEMA dsreq ON a.request_schema_id=dsreq.data_schema_id AND dsreq.is_active = 1
    LEFT OUTER JOIN DATA_SCHEMA dsres ON a.response_schema_id=dsres.data_schema_id AND dsres.is_active = 1
    LEFT OUTER JOIN DATA_SCHEMA dsurl ON a.url_schema_id=dsurl.data_schema_id AND dsurl.is_active = 1
    LEFT OUTER JOIN CONTROLLER c ON  a.controller_id = c.controller_id AND c.is_active = 1
WHERE
    a.is_active = 1
";
                }
                else
                {
                    sql = @"
SELECT
    c.vendor_id as VendorId
    ,a.api_id as ApiId
    ,a.api_description as ApiDescription
    ,a.controller_id as ControllerId
    ,a.method_type as MethodType
    ,a.url as ApiUrl
    ,a.repository_key as RepositoryKey
    ,a.request_schema_id as RequestSchemaId
    ,dsreq.schema_name as RequestSchemaName
    ,a.response_schema_id as ResponseSchemaId
    ,dsres.schema_name as ResponseSchemaName
    ,a.url_schema_id as UrlSchemaId
    ,dsurl.schema_name as UrlSchemaName
    ,a.post_data_type as PostDataType
    ,a.query as Query
    ,a.repository_group_id as RepositoryGroupId
    ,rg.repository_group_name as RepositoryGroupName
    ,a.is_enable as IsEnable
    ,a.is_header_authentication as IsHeaderAuthentication
    ,a.is_openid_authentication as IsOpenIdAuthentication
    ,a.is_admin_authentication as IsAdminAuthentication
    ,a.is_over_partition as IsOverPartition
    ,a.gateway_url as GatewayUrl
    ,a.gateway_credential_username as GatewayCredentialUserName
    ,a.gateway_credential_password as GatewayCredentialPassword
    ,a.is_hidden as IsHidden
    ,a.script as Script
    ,a.action_type_cd as ActionTypeCd
    ,a.script_type_cd as ScriptTypeCd
    ,a.is_cache as IsCache
    ,a.cache_minute as CacheMinute
    ,a.cache_key as CacheKey
    ,a.is_accesskey as AccessKey
    ,a.is_automatic_id as Automatic
    ,a.actiontype_version as ActionTypeVersion
    ,a.partition_key as PartitionKey
    ,a.gateway_relay_header as GatewayRelayHeader
    ,a.reg_date as RegDate
    ,a.reg_username as RegUsername
    ,a.upd_date as UpdDate
    ,a.upd_username as UpdUsername
    ,a.is_active as IsActive
    ,a.is_transparent_api as IsTransparent 
    ,a.is_vendor_system_authentication_allow_null as IsVendorSystemAuthenticationAllowNull
    ,a.is_visible_signinuser_only as IsVisibleSigninUserOnly
    ,a.query_type_cd as QueryTypeCd
    ,a.is_internal_call_only as IsInternalOnly
    ,a.internal_call_keyword as InternalOnlyKeyword
    ,a.is_skip_jsonschema_validation as IsSkipJsonSchemaValidation
    ,a.is_otherresource_sqlaccess as IsOtherResourceSqlAccess
    ,a.is_clientcert_authentication as IsClientCertAuthentication
FROM 
    Api a
    LEFT OUTER JOIN RepositoryGroup rg ON a.repository_group_id=rg.repository_group_id AND rg.is_active= 1
    LEFT OUTER JOIN DataSchema dsreq ON a.request_schema_id=dsreq.data_schema_id AND dsreq.is_active = 1
    LEFT OUTER JOIN DataSchema dsres ON a.response_schema_id=dsres.data_schema_id AND dsres.is_active = 1
    LEFT OUTER JOIN DataSchema dsurl ON a.url_schema_id=dsurl.data_schema_id AND dsurl.is_active = 1
    LEFT OUTER JOIN Controller c ON  a.controller_id = c.controller_id AND c.is_active = 1
WHERE
    a.is_active = 1
";
                }

                return sql; }
        }

        /// <summary>
        /// ControllerIdに紐づくApiを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <param name="ContainsTransparentApi">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>Api</returns>
        private IEnumerable<ApiQueryModel> GetApiList(string controllerId, bool ContainsTransparentApi)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            bool? isTransparentApi = true;
            if (ContainsTransparentApi)
            {
                isTransparentApi = false;
            }
            var sql = apiBaseSql + " AND a.is_transparent_api = /*ds isTransparentApi*/'1' AND a.controller_id = /*ds controllerId*/'1' ";
            var param = new
            {
                controllerId,
                isTransparentApi
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var ret = Connection.Query<ApiQueryModel>(twowaySql.Sql, dynParams);

            // Apiの子要素を取得
            ret.ToList().ForEach(x => SetApiChidrenData(x));

            return ret;
        }

        /// <summary>
        /// apiIdに紐づくAPIを取得します。
        /// </summary>
        /// <returns>API</returns>
        public ApiQueryModel GetApi(string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = apiBaseSql + " AND a.api_id = /*ds apiId*/'1' ";
            var param = new { apiId };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var ret = Connection.QuerySingle<ApiQueryModel>(twowaySql.Sql, dynParams);
            SetApiChidrenData(ret);
            return ret;
        }

        /// <summary>
        /// URLに紐づくAPIを取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns>API</returns>
        public ApiQueryModel GetApiFromUrl(string controllerUrl, string apiUrl)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = apiBaseSql + "AND c.url = /*ds controllerUr*/'1' AND a.url = /*ds apiUrl*/'1' ";
            var param = new { controllerUrl, apiUrl };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var ret = Connection.QuerySingle<ApiQueryModel>(twowaySql.Sql, dynParams);
            SetApiChidrenData(ret);
            return ret;
        }

        /// <summary>
        /// Apiの子要素を取得します。
        /// </summary>
        /// <param name="api">ApiQueryModel</param>
        private void SetApiChidrenData(ApiQueryModel api)
        {
            api.SecondaryRepositoryMapList = GetSecondaryRepositoryMapList(api.ApiId);
            api.SampleCodeList = GetSampleCodeList(api.ApiId);
            api.ApiLinkList = GetApiLinkList(api.ApiId);
            api.ApiAccessVendorList = GetApiAccessVendorList(api.ApiId);
            api.ApiOpenIdCAList = GetApiOpenIdCaList(api.ApiId);
        }

        /// <summary>
        /// メソッドタイプ一致かつコントローラーURL前方一致のAPIを取得します。
        /// </summary>
        /// <param name="httpMethodType">メソッドタイプ</param>
        /// <param name="controllerUrl">コントローラーURL</param>
        /// <returns>重複API有無</returns>
        public IEnumerable<ApiUrlIdentifier> GetApiUrlIdentifier(HttpMethodType httpMethodType, string controllerUrl)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    a.controller_id AS ControllerId,
    a.api_id AS ApiId,
    a.action_type_cd AS ActionType,
    a.method_type AS MethodType,
    c.url AS ControllerUrl,
    a.url AS ApiUrl
FROM
    API a
    JOIN CONTROLLER c ON a.controller_id = c.controller_id AND c.is_active = 1
    JOIN VENDOR v ON c.vendor_id = v.vendor_id AND v.is_active = 1
    JOIN SYSTEM s ON c.system_id = s.system_id AND s.is_active = 1
WHERE
    c.url || '/' || a.url LIKE /*ds url*/'/API/Master' || '%'
    AND a.method_type = /*ds method_type*/'GET'
    AND a.is_active = 1
";
                }
                else
                {
                    sql = @"
SELECT
    a.controller_id AS ControllerId,
    a.api_id AS ApiId,
    a.action_type_cd AS ActionType,
    a.method_type AS MethodType,
    c.url AS ControllerUrl,
    a.url AS ApiUrl
FROM
    Api a
    JOIN Controller c ON a.controller_id = c.controller_id AND c.is_active = 1
    JOIN Vendor v ON c.vendor_id = v.vendor_id AND v.is_active = 1
    JOIN System s ON c.system_id = s.system_id AND s.is_active = 1
WHERE
    c.url + '/' + a.url LIKE @url + '%'
    AND a.method_type = @method_type
    AND a.is_active = 1
";
            }

            var param = new
            {
                url = controllerUrl,
                method_type = httpMethodType.Value.ToString()
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            // メソッドタイプ一致かつコントローラーURL前方一致のAPIを取得
            return Connection.Query<ApiUrlIdentifier>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// ApiIdに紐づく有効なAPIが存在するかを返します。
        /// </summary>
        /// <returns>true:存在する/false:存在しない</returns>
        public bool ExistsEnableApi(string apiId)
        {
            if (string.IsNullOrEmpty(apiId))
            {
                return false;
            }

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(a.api_id)
FROM
    Api a
    JOIN Controller c ON c.controller_Id = a.controller_id AND c.is_active = 1 AND c.is_enable = 1
WHERE
    a.api_id = /*ds apiId*/'id' 
    AND a.is_active = 1
    AND a.is_enable = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(a.api_id)
FROM
    API a
    JOIN Controller c ON c.controller_Id = a.controller_id AND c.is_active = 1 AND c.is_enable = 1
WHERE
    a.api_id = @apiId
    AND a.is_active = 1
    AND a.is_enable = 1
";
            }
            var param = new { apiId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            return result > 0;
        }

        /// <summary>
        /// APIが実行可能か取得します。
        /// </summary>
        /// <param name="actionTypeCd">アクションタイプコード</param>
        /// <param name="httpMethodTypeCd">メソッドタイプコード</param>
        /// <param name="repositoryGroupId">リポジトリグループID</param>
        /// <returns></returns>
        public bool IsExcuseApiCombinationConstraints(ActionType actionTypeCd, HttpMethodType httpMethodTypeCd, string? repositoryGroupId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (string.IsNullOrEmpty(repositoryGroupId))
            {
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT
    ACC.is_excuse
FROM 
    API_COMBINATION_CONSTRAINTS ACC
WHERE
    ACC.repository_type_cd is null
    AND ACC.action_type_cd = /*ds actionTypeCd*/'upd' 
    AND ACC.http_method_type_code = /*ds httpMethodTypeCd*/'PUT' 
    AND ACC.is_active = 1
";
                }
                else
                {
                    sql = @"
SELECT
    ACC.is_excuse
FROM 
    ApiCombinationConstraints AS ACC
WHERE
    ACC.repository_type_cd is null
    AND ACC.action_type_cd = @actionTypeCd
    AND ACC.http_method_type_code = @httpMethodTypeCd
    AND ACC.is_active = 1
";
                }
            }
            else
            {
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT
    ACC.is_excuse
FROM 
    API_COMBINATION_CONSTRAINTS ACC
    LEFT JOIN REPOSITORY_GROUP  RG ON ACC.repository_type_cd = RG.repository_type_cd
WHERE
    RG.repository_group_id = /*ds repositoryGroupId*/'id' 
    AND ACC.action_type_cd = /*ds actionTypeCd*/'upd' 
    AND ACC.http_method_type_code = /*ds httpMethodTypeCd*/'PUT' 
    AND ACC.is_active = 1
";
                }
                else
                {
                    sql = @"
SELECT
    ACC.is_excuse
FROM 
    ApiCombinationConstraints AS ACC
    LEFT JOIN RepositoryGroup AS RG ON ACC.repository_type_cd = RG.repository_type_cd
WHERE
    RG.repository_group_id = @repositoryGroupId
    AND ACC.action_type_cd = @actionTypeCd
    AND ACC.http_method_type_code = @httpMethodTypeCd
    AND ACC.is_active = 1
";
                }
            }

            var param = new
            {
                repositoryGroupId,
                actionTypeCd = actionTypeCd.Value.GetCode(),
                httpMethodTypeCd = Enum.GetName(typeof(HttpMethodType.MethodType), httpMethodTypeCd.Value),
            };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingleOrDefault<bool>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// APIを登録または更新します。
        /// </summary>
        /// <param name="api">APIモデル</param>
        /// <returns>APIモデル</returns>
        [CacheIdFire("ApiId", "model.ApiId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_API)]
        [DomainDataSync(ParameterType.Result, DynamicApiDatabase.TABLE_API, "ApiId")]
        public ApiInformationModel UpsertApi(ApiInformationModel model)
        {
            ValidateQuerySyntax(model.Query);

            if (string.IsNullOrEmpty(model.ApiId)) model.ApiId = Guid.NewGuid().ToString();

            var param = new
            {
                model.ApiId,
                model.ApiDescription,
                model.ControllerId,
                MethodType = model.MethodType.ToUpper(),
                model.Url,
                model.RepositoryKey,
                model.RequestSchemaId,
                model.ResponseSchemaId,
                model.UrlSchemaId,
                PostDataType = model.PostDataType ?? string.Empty,
                model.Query,
                model.RepositoryGroupId,
                model.IsEnable,
                model.IsHeaderAuthentication,
                model.IsOpenIdAuthentication,
                model.IsAdminAuthentication,
                model.IsOverPartition,
                model.GatewayUrl,
                model.GatewayCredentialUserName,
                model.GatewayCredentialPassword,
                model.IsHidden,
                model.Script,
                ActionType = ActionType.Parse(model.ActionType).Value.GetCode(),
                model.ScriptType,
                model.IsCache,
                CacheMinute = model.CacheMinute ?? 0,
                model.CacheKey,
                model.IsAccessKey,
                model.IsAutomaticId,
                ActiontypeVersion = 1,
                model.PartitionKey,
                model.GatewayRelayHeader,
                RegDate = UtcNow,
                RegUsername = PerRequestDataContainer.OpenId,
                UpdDate = UtcNow,
                UpdUsername = PerRequestDataContainer.OpenId,
                IsActive = true,
                model.IsTransparentApi,
                model.IsVendorSystemAuthenticationAllowNull,
                model.IsVisibleSigninuserOnly,
                QueryTypeCd = model.QueryType ?? DefaultQueryType,
                model.IsSkipJsonSchemaValidation,
                IsInternalCallOnly = model.IsInternalOnly,
                InternalCallKeyword = model.InternalOnlyKeyword ?? string.Empty,
                IsOpenidAuthenticationAllowNull = false,
                model.IsClientCertAuthentication,
                model.IsOtherResourceSqlAccess
            };

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO API target
USING
(
    SELECT
        /*ds ApiId*/'1' AS api_id
    FROM DUAL
) source
ON
    (target.api_id = source.api_id)
WHEN MATCHED THEN
    UPDATE
    SET
        api_description = /*ds ApiDescription*/'1' 
        ,controller_id = /*ds ControllerId*/'1' 
        ,method_type = /*ds MethodType*/'1' 
        ,url = /*ds Url*/'1' 
        ,repository_key = /*ds RepositoryKey*/'1' 
        ,request_schema_id = /*ds RequestSchemaId*/'1' 
        ,response_schema_id = /*ds ResponseSchemaId*/'1' 
        ,url_schema_id = /*ds UrlSchemaId*/'1' 
        ,post_data_type = /*ds PostDataType*/'1' 
        ,query = /*ds Query*/'1' 
        ,repository_group_id = /*ds RepositoryGroupId*/'1' 
        ,is_enable = /*ds IsEnable*/'1' 
        ,is_header_authentication = /*ds IsHeaderAuthentication*/'1' 
        ,is_openid_authentication = /*ds IsOpenIdAuthentication*/'1' 
        ,is_admin_authentication = /*ds IsAdminAuthentication*/'1' 
        ,is_over_partition = /*ds IsOverPartition*/'1' 
        ,gateway_url = /*ds GatewayUrl*/'1' 
        ,gateway_credential_username = /*ds GatewayCredentialUserName*/'1' 
        ,gateway_credential_password = /*ds GatewayCredentialPassword*/'1' 
        ,is_hidden = /*ds IsHidden*/1 
        ,script = /*ds Script*/'1' 
        ,action_type_cd = /*ds ActionType*/'1' 
        ,script_type_cd = /*ds ScriptType*/'1' 
        ,is_cache = /*ds IsCache*/1 
        ,cache_minute = /*ds CacheMinute*/'1' 
        ,cache_key = /*ds CacheKey*/'1' 
        ,is_accesskey = /*ds IsAccessKey*/1  
        ,is_automatic_id = /*ds IsAutomaticId*/1  
        ,actiontype_version = /*ds ActiontypeVersion*/'1' 
        ,partition_key = /*ds PartitionKey*/'1' 
        ,gateway_relay_header = /*ds GatewayRelayHeader*/'1' 
        ,upd_date = /*ds UpdDate*/systimestamp 
        ,upd_username = /*ds UpdUsername*/'1' 
        ,is_active = /*ds IsActive*/1 
        ,is_transparent_api = /*ds IsTransparentApi*/1 
        ,is_vendor_system_authentication_allow_null = /*ds IsVendorSystemAuthenticationAllowNull*/1 
        ,is_visible_signinuser_only = /*ds IsVisibleSigninuserOnly*/1 
        ,query_type_cd = /*ds QueryTypeCd*/'1' 
        ,is_skip_jsonschema_validation = /*ds IsSkipJsonSchemaValidation*/1  
        ,is_internal_call_only = /*ds IsInternalCallOnly*/1 
        ,internal_call_keyword = /*ds InternalCallKeyword*/'1' 
        ,is_openid_authentication_allow_null = /*ds IsOpenidAuthenticationAllowNull*/1  
        ,is_clientcert_authentication = /*ds IsClientCertAuthentication*/1 
        ,is_otherresource_sqlaccess = /*ds IsOtherResourceSqlAccess*/1 
 WHEN NOT MATCHED THEN
    INSERT
    (
        api_id
        ,api_description
        ,controller_id
        ,method_type
        ,url
        ,repository_key
        ,request_schema_id
        ,response_schema_id
        ,url_schema_id
        ,post_data_type
        ,query
        ,repository_group_id
        ,is_enable
        ,is_header_authentication
        ,is_openid_authentication
        ,is_admin_authentication
        ,is_over_partition
        ,gateway_url
        ,gateway_credential_username
        ,gateway_credential_password
        ,is_hidden
        ,script
        ,action_type_cd
        ,script_type_cd
        ,is_cache
        ,cache_minute
        ,cache_key
        ,is_accesskey
        ,is_automatic_id
        ,actiontype_version
        ,partition_key
        ,gateway_relay_header
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
        ,is_transparent_api
        ,is_vendor_system_authentication_allow_null
        ,is_visible_signinuser_only
        ,query_type_cd
        ,is_skip_jsonschema_validation
        ,is_internal_call_only
        ,internal_call_keyword
        ,is_openid_authentication_allow_null
        ,is_clientcert_authentication
        ,is_otherresource_sqlaccess
    )
    VALUES
    (
        /*ds ApiId*/'1' 
        ,/*ds ApiDescription*/'1' 
        ,/*ds ControllerId*/'1' 
        ,/*ds MethodType*/'1' 
        ,/*ds Url*/'1' 
        ,/*ds RepositoryKey*/'1' 
        ,/*ds RequestSchemaId*/'1' 
        ,/*ds ResponseSchemaId*/'1' 
        ,/*ds UrlSchemaId*/'1' 
        ,/*ds PostDataType*/'1' 
        ,/*ds Query*/'1' 
        ,/*ds RepositoryGroupId*/'1' 
        ,/*ds IsEnable*/1 
        ,/*ds IsHeaderAuthentication*/1 
        ,/*ds IsOpenIdAuthentication*/1 
        ,/*ds IsAdminAuthentication*/1 
        ,/*ds IsOverPartition*/1 
        ,/*ds GatewayUrl*/'1' 
        ,/*ds GatewayCredentialUserName*/'1' 
        ,/*ds GatewayCredentialPassword*/'1' 
        ,/*ds IsHidden*/1 
        ,/*ds Script*/'1' 
        ,/*ds ActionType*/'1' 
        ,/*ds ScriptType*/'1' 
        ,/*ds IsCache*/1 
        ,/*ds CacheMinute*/'1' 
        ,/*ds CacheKey*/'1' 
        ,/*ds IsAccessKey*/1 
        ,/*ds IsAutomaticId*/1 
        ,/*ds ActiontypeVersion*/'1' 
        ,/*ds PartitionKey*/'1' 
        ,/*ds GatewayRelayHeader*/'1' 
        ,/*ds RegDate*/systimestamp 
        ,/*ds RegUsername*/'1' 
        ,/*ds UpdDate*/systimestamp 
        ,/*ds UpdUsername*/'1' 
        ,/*ds IsActive*/1 
        ,/*ds IsTransparentApi*/1 
        ,/*ds IsVendorSystemAuthenticationAllowNull*/1 
        ,/*ds IsVisibleSigninuserOnly*/1 
        ,/*ds QueryTypeCd*/'1' 
        ,/*ds IsSkipJsonSchemaValidation*/1 
        ,/*ds IsInternalCallOnly*/1 
        ,/*ds InternalCallKeyword*/'1' 
        ,/*ds IsOpenidAuthenticationAllowNull*/1 
        ,/*ds IsClientCertAuthentication*/1 
        ,/*ds IsOtherResourceSqlAccess*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO Api AS target
USING
(
    SELECT
        @ApiId AS api_id
) AS source
ON
    target.api_id = source.api_id
WHEN MATCHED THEN
    UPDATE
    SET
        api_description = @ApiDescription
        ,controller_id = @ControllerId
        ,method_type = @MethodType
        ,url = @Url
        ,repository_key = @RepositoryKey
        ,request_schema_id = @RequestSchemaId
        ,response_schema_id = @ResponseSchemaId
        ,url_schema_id = @UrlSchemaId
        ,post_data_type = @PostDataType
        ,query = @Query
        ,repository_group_id = @RepositoryGroupId
        ,is_enable = @IsEnable
        ,is_header_authentication = @IsHeaderAuthentication
        ,is_openid_authentication = @IsOpenIdAuthentication
        ,is_admin_authentication = @IsAdminAuthentication
        ,is_over_partition = @IsOverPartition
        ,gateway_url = @GatewayUrl
        ,gateway_credential_username = @GatewayCredentialUserName
        ,gateway_credential_password = @GatewayCredentialPassword
        ,is_hidden = @IsHidden
        ,script = @Script
        ,action_type_cd = @ActionType
        ,script_type_cd = @ScriptType
        ,is_cache = @IsCache
        ,cache_minute = @CacheMinute
        ,cache_key = @CacheKey
        ,is_accesskey = @IsAccessKey
        ,is_automatic_id = @IsAutomaticId
        ,actiontype_version = @ActiontypeVersion
        ,partition_key = @PartitionKey
        ,gateway_relay_header = @GatewayRelayHeader
        ,upd_date = @UpdDate
        ,upd_username = @UpdUsername
        ,is_active = @IsActive
        ,is_transparent_api = @IsTransparentApi
        ,is_vendor_system_authentication_allow_null = @IsVendorSystemAuthenticationAllowNull
        ,is_visible_signinuser_only = @IsVisibleSigninuserOnly
        ,query_type_cd = @QueryTypeCd
        ,is_skip_jsonschema_validation = @IsSkipJsonSchemaValidation
        ,is_internal_call_only = @IsInternalCallOnly
        ,internal_call_keyword = @InternalCallKeyword
        ,is_openid_authentication_allow_null = @IsOpenidAuthenticationAllowNull
        ,is_clientcert_authentication = @IsClientCertAuthentication
        ,is_otherresource_sqlaccess = @IsOtherResourceSqlAccess
WHEN NOT MATCHED THEN
    INSERT
    (
        api_id
        ,api_description
        ,controller_id
        ,method_type
        ,url
        ,repository_key
        ,request_schema_id
        ,response_schema_id
        ,url_schema_id
        ,post_data_type
        ,query
        ,repository_group_id
        ,is_enable
        ,is_header_authentication
        ,is_openid_authentication
        ,is_admin_authentication
        ,is_over_partition
        ,gateway_url
        ,gateway_credential_username
        ,gateway_credential_password
        ,is_hidden
        ,script
        ,action_type_cd
        ,script_type_cd
        ,is_cache
        ,cache_minute
        ,cache_key
        ,is_accesskey
        ,is_automatic_id
        ,actiontype_version
        ,partition_key
        ,gateway_relay_header
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
        ,is_transparent_api
        ,is_vendor_system_authentication_allow_null
        ,is_visible_signinuser_only
        ,query_type_cd
        ,is_skip_jsonschema_validation
        ,is_internal_call_only
        ,internal_call_keyword
        ,is_openid_authentication_allow_null
        ,is_clientcert_authentication
        ,is_otherresource_sqlaccess
    )
    VALUES
    (
        @ApiId
        ,@ApiDescription
        ,@ControllerId
        ,@MethodType
        ,@Url
        ,@RepositoryKey
        ,@RequestSchemaId
        ,@ResponseSchemaId
        ,@UrlSchemaId
        ,@PostDataType
        ,@Query
        ,@RepositoryGroupId
        ,@IsEnable
        ,@IsHeaderAuthentication
        ,@IsOpenIdAuthentication
        ,@IsAdminAuthentication
        ,@IsOverPartition
        ,@GatewayUrl
        ,@GatewayCredentialUserName
        ,@GatewayCredentialPassword
        ,@IsHidden
        ,@Script
        ,@ActionType
        ,@ScriptType
        ,@IsCache
        ,@CacheMinute
        ,@CacheKey
        ,@IsAccessKey
        ,@IsAutomaticId
        ,@ActiontypeVersion
        ,@PartitionKey
        ,@GatewayRelayHeader
        ,@RegDate
        ,@RegUsername
        ,@UpdDate
        ,@UpdUserName
        ,@IsActive
        ,@IsTransparentApi
        ,@IsVendorSystemAuthenticationAllowNull
        ,@IsVisibleSigninuserOnly
        ,@QueryTypeCd
        ,@IsSkipJsonSchemaValidation
        ,@IsInternalCallOnly
        ,@InternalCallKeyword
        ,@IsOpenidAuthenticationAllowNull
        ,@IsClientCertAuthentication
        ,@IsOtherResourceSqlAccess
    );
";
            }
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param)
                .SetNClob(nameof(param.Query))
                .SetNClob(nameof(param.Script))
                .SetNClob(nameof(param.InternalCallKeyword));
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            return model;
        }

        /// <summary>
        /// クエリ内の構文の検証
        /// </summary>
        private void ValidateQuerySyntax(string query)
        {
            // クエリが空の場合は検証しない
            if (string.IsNullOrEmpty(query)) return;

            var validator = UnityCore.Resolve<IQuerySyntaxValidatorFactory>().Create(query);
            if (validator != null)
            {
                if (!validator.Validate(query, out string message))
                {
                    throw new QuerySyntaxErrorException(message);
                }
            }
        }

        /// <summary>
        /// APIを削除します。
        /// </summary>
        /// <param name="apiId">APIID</param>
        [CacheIdFire("ApiId", "apiId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_API)]
        [DomainDataSync(DynamicApiDatabase.TABLE_API, "apiId")]
        public void DeleteApi(string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var updSql = "";
            if (dbSettings.Type == "Oracle")
            {
                updSql = @"
UPDATE API
   SET
       is_active = 0
      ,upd_date = /*ds UpdDate*/systimestamp 
      ,upd_username = /*ds UpdUserName*/'1' 
 WHERE
    api_id = /*ds apiId*/'1' 
 AND is_active = 1
";
            }
            else
            {
                updSql = @"
UPDATE Api
   SET
       is_active = 0
      ,upd_date = @UpdDate
      ,upd_username = @UpdUserName
 WHERE
    api_id = @apiId
 AND is_active = 1
";
            }
            var param = new
            {
                apiId,
                UpdDate = UtcNow,
                UpdUsername = PerRequestDataContainer.OpenId
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), updSql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// URL紐づくAPIを削除します。
        /// </summary>
        /// <param name="controllerUrl">コントローラURL</param>
        /// <param name="apiUrl">APIURL</param>
        [CacheIdFire("controllerUrl", "apiUrl")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_API)]
        [DomainDataSync(DynamicApiDatabase.TABLE_API, "apiId")]
        public void DeleteApiFromUrl(string controllerUrl, string apiUrl, string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var updSql = "";

            if (dbSettings.Type == "Oracle")
            {
                updSql = @"
UPDATE API a
    SET
        a.is_active = 0
        ,a.upd_date = /*ds UpdDate*/systimestamp
        ,a.upd_username = /*ds UpdUserName*/'1' 
WHERE
    a.url = /*ds apiUrl*/'1' 
    AND a.is_active = 1
    AND EXISTS(
        SELECT c.controller_id FROM CONTROLLER c
        WHERE
            c.is_active = 1
            AND c.url = /*ds controllerUrl*/'1' 
            AND c.controller_id = a.controller_id
    )
";
            }
            else
            {
                updSql = @"
UPDATE a
    SET
        a.is_active = 0
        ,a.upd_date = @UpdDate
        ,a.upd_username = @UpdUserName
    FROM Api AS a
        JOIN Controller c ON c.controller_id = a.controller_id AND c.is_active = 1
WHERE
    c.url = @controllerUrl
    AND a.url = @apiUrl
    AND a.is_active = 1
";
            }

            var param = new
            {
                controllerUrl,
                apiUrl,
                UpdDate = UtcNow,
                UpdUsername = PerRequestDataContainer.OpenId
            };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), updSql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }
        #endregion

        #region StaticApi
        /// <summary>
        /// StaticApiの一覧を取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns></returns>
        public IEnumerable<StaticApiId> GetStaticApiList(string controllerId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    c.controller_id AS ControllerId
    ,a.api_id AS ApiId
FROM 
    CONTROLLER c
    LEFT JOIN API a ON c.controller_id = a.controller_id AND a.is_active = 1
WHERE
/*ds if IsControllerId != null*/
    c.controller_id = /*ds controllerId*/'1' AND
/*ds end if*/
    c.is_static_api = 1
AND c.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT 
    c.controller_id AS ControllerId
    ,a.api_id AS ApiId
FROM 
    Controller c
    LEFT JOIN Api a ON c.controller_id = a.controller_id AND a.is_active = 1
WHERE
/*ds if IsControllerId != null*/
    c.controller_id = @controllerId AND
/*ds end if*/
    c.is_static_api = 1
AND c.is_active = 1
";
            }
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsControllerId", string.IsNullOrEmpty(controllerId) ? null : "IsControllerId");
            twowaySqlParam.Add("controllerId", controllerId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            return Connection.Query<StaticApiId>(twowaySql.Sql, dynParams);
        }
        #endregion

        #region ApiAccessVendor
        /// <summary>
        /// APIアクセスベンダーを取得します。
        /// </summary>
        /// <param name="model">モデル</param>
        /// <param name="apiId">APIID</param>
        /// <returns>モデル</returns>
        private IEnumerable<ApiAccessVendorModel> GetApiAccessVendor(ApiAccessVendorModel model, string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    api_access_vendor_id AS ApiAccessVendorId
    ,vendor_id AS VendorId
    ,system_id AS SystemId
    ,is_enable AS IsEnable
    ,access_key AS AccessKey
FROM
    API_ACCESS_VENDOR
WHERE
    vendor_id = /*ds VendorId*/'1' 
    AND system_id = /*ds SystemId*/'1' 
    AND api_id = /*ds apiId*/'1' 
    AND is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    api_access_vendor_id AS ApiAccessVendorId
    ,vendor_id AS VendorId
    ,system_id AS SystemId
    ,is_enable AS IsEnable
    ,access_key AS AccessKey
FROM
    ApiAccessVendor
WHERE
    vendor_id = @VendorId
    AND system_id = @SystemId
    AND api_id = @apiId
    AND is_active = 1
";
            }
            var param = new
            {
                model.VendorId,
                model.SystemId,
                apiId
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ApiAccessVendorModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// ApiIdに紐づくApiAccessVendorを取得する
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns>ApiAccessVendor</returns>
        private IEnumerable<ApiAccessVendorModel> GetApiAccessVendorList(string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    a.api_access_vendor_id as ApiAccessVendorId
    ,a.vendor_id as VendorId
    ,v.vendor_name as VendorName
    ,a.system_id as SystemId
    ,s.system_name as SystemName
    ,a.is_enable as IsEnable
    ,a.access_key as AccessKey
FROM
    API_ACCESS_VENDOR a
    INNER JOIN VENDOR v ON a.vendor_id = v.vendor_id AND v.is_active = 1
    INNER JOIN SYSTEM s ON a.system_id = s.system_id AND s.is_active = 1
WHERE
    a.api_id = /*ds apiId*/'1' 
    AND a.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    a.api_access_vendor_id as ApiAccessVendorId
    ,a.vendor_id as VendorId
    ,v.vendor_name as VendorName
    ,a.system_id as SystemId
    ,s.system_name as SystemName
    ,a.is_enable as IsEnable
    ,a.access_key as AccessKey
FROM
    ApiAccessVendor a
    INNER JOIN Vendor v ON a.vendor_id = v.vendor_id AND v.is_active = 1
    INNER JOIN System s ON a.system_id = s.system_id AND s.is_active = 1
WHERE
    a.api_id = @apiId
    AND a.is_active = 1
";
            }

            var param = new { apiId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ApiAccessVendorModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// VendorIdに紐づくApiAccessVendorを取得する
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのApiAccessVendorを取得する</param>
        /// <returns>ApiAccessVendor</returns>
        private Dictionary<string, IEnumerable<ApiAccessVendorModel>> GetApiAccessVendor(string vendorId, bool isAll)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    aa.api_id as ApiId
    ,aa.api_access_vendor_id as ApiAccessVendorId
    ,aa.vendor_id as VendorId
    ,aav.vendor_name as VendorName
    ,aa.system_id as SystemId
    ,aas.system_name as SystemName
    ,aa.is_enable as IsEnable
    ,aa.access_key as AccessKey
FROM
    API_ACCESS_VENDOR aa /*WITH(NOLOCK)*/
    INNER JOIN VENDOR aav /*WITH(NOLOCK)*/ ON aav.vendor_id = aa.vendor_id AND aav.is_active = 1
    INNER JOIN SYSTEM aas /*WITH(NOLOCK)*/ ON aas.system_id = aa.system_id AND aas.is_active = 1
    INNER JOIN API a /*WITH(NOLOCK)*/ ON a.api_id = aa.api_id AND a.is_active = 1
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = a.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /* vendor_id*/'1' AND
/*ds end if*/
    aa.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    aa.api_id as ApiId
    ,aa.api_access_vendor_id as ApiAccessVendorId
    ,aa.vendor_id as VendorId
    ,aav.vendor_name as VendorName
    ,aa.system_id as SystemId
    ,aas.system_name as SystemName
    ,aa.is_enable as IsEnable
    ,aa.access_key as AccessKey
FROM
    ApiAccessVendor aa WITH(NOLOCK)
    INNER JOIN Vendor aav WITH(NOLOCK) ON aav.vendor_id = aa.vendor_id AND aav.is_active = 1
    INNER JOIN System aas WITH(NOLOCK) ON aas.system_id = aa.system_id AND aas.is_active = 1
    INNER JOIN Api a WITH(NOLOCK) ON a.api_id = aa.api_id AND a.is_active = 1
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = a.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendor_id AND
/*ds end if*/
    aa.is_active = 1
";
            }

            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendor_id", vendorId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var result = Connection.Query<string, ApiAccessVendorModel, (string ApiId, ApiAccessVendorModel ApiAccessVendor)>(
                sql: twowaySql.Sql,
                splitOn: "ApiAccessVendorId",
                map: (apiId, apiAccessVendor) => (apiId, apiAccessVendor),
                param: dynParams);

            return result.GroupBy(x => x.ApiId).ToDictionary(x => x.Key, y => y.Select(z => z.ApiAccessVendor));
        }

        /// <summary>
        /// APIアクセスベンダーを登録または更新します。
        /// </summary>
        /// <param name="modelList">APIアクセスベンダーモデルリスト</param>
        /// <param name="apiId">APIID</param>
        /// <returns>APIアクセスベンダーモデル</returns>
        public List<ApiAccessVendorModel> UpsertApiAccessVendor(List<ApiAccessVendorModel> modelList, string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();

            List<object> paramList = new();
            foreach (var model in modelList)
            {
                // IsEnableがfalseかつDB未登録の場合は処理しない
                if (model == null || (model.IsEnable == false && !GetApiAccessVendor(model, apiId).Any()))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(model.ApiAccessVendorId)) model.ApiAccessVendorId = Guid.NewGuid().ToString();

                var param = new
                {
                    api_access_vendor_id = model.ApiAccessVendorId,
                    api_id = apiId,
                    vendor_id = model.VendorId,
                    system_id = model.SystemId,
                    is_enable = model.IsEnable,
                    is_active = true,
                    access_key = string.IsNullOrEmpty(model.AccessKey) == true ? null : model.AccessKey,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId
                };
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                paramList.Add(dynParams);
            }

            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO API_ACCESS_VENDOR target
USING
(
    SELECT
        /*ds api_access_vendor_id*/'1' AS api_access_vendor_id,
        /*ds api_id*/'1' AS api_id,
        /*ds vendor_id*/'1' AS vendor_id,
        /*ds system_id*/'1' AS system_id
    FROM DUAL
) source
ON(    target.vendor_id = source.vendor_id
   AND target.system_id = source.system_id
   AND target.api_id = source.api_id
  )
WHEN MATCHED THEN
UPDATE
    SET
        access_key = /*ds access_key*/'1' ,
        is_enable = /*ds is_enable*/1 ,
        is_active = /*ds is_active*/1 ,
        upd_date = /*ds upd_date*/systimestamp ,
        upd_username = /*ds upd_username*/'1' 
WHEN NOT MATCHED THEN
    INSERT
    (
        api_access_vendor_id,
        api_id,
        vendor_id,
        system_id,
        is_enable,
        access_key,
        reg_date,
        reg_username,
        upd_date,
        upd_username,
        is_active
    )
    VALUES
    (
        /*ds api_access_vendor_id*/'1' ,
        /*ds api_id*/'1' ,
        /*ds vendor_id*/'1' ,
        /*ds system_id*/'1' ,
        /*ds is_enable*/1 ,
        /*ds access_key*/'1' ,
        /*ds reg_date*/systimestamp ,
        /*ds reg_username*/'1' ,
        /*ds upd_date*/systimestamp ,
        /*ds upd_username*/'1' ,
        /*ds is_active*/1
    )
";
            }
            else
            {
                sql = @"
MERGE INTO ApiAccessVendor AS target
USING
(
    SELECT
        @api_access_vendor_id AS api_access_vendor_id,
        @api_id AS api_id,
        @vendor_id AS vendor_id,
        @system_id AS system_id
) AS source
ON(    target.vendor_id = source.vendor_id
   AND target.system_id = source.system_id
   AND target.api_id = source.api_id
  )
WHEN MATCHED THEN
UPDATE
    SET
        access_key = @access_key,
        is_enable = @is_enable,
        is_active = @is_active,
        upd_date = @upd_date,
        upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        api_access_vendor_id,
        api_id,
        vendor_id,
        system_id,
        is_enable,
        access_key,
        reg_date,
        reg_username,
        upd_date,
        upd_username,
        is_active
    )
    VALUES
    (
        @api_access_vendor_id,
        @api_id,
        @vendor_id,
        @system_id,
        @is_enable,
        @access_key,
        @reg_date,
        @reg_username,
        @upd_date,
        @upd_username,
        @is_active
    )
;
";
            }
            if (paramList?.Count > 0)
            {
                var paramNames = new
                {
                    api_access_vendor_id = 1,
                    api_id = 1,
                    vendor_id = 1,
                    system_id = 1,
                    is_enable = true,
                    is_active = true,
                    access_key = 1,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId
                };

                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, paramNames);
                Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            }
            return modelList;
        }

        #endregion

        #region ApiLink
        /// <summary>
        /// ApiIdに紐づくApiLinkを取得します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns>ApiLink</returns>
        private IEnumerable<ApiLinkModel> GetApiLinkList(string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    a.api_link_id as ApiLinkId
    ,a.title as LinkTitle
    ,a.url as LinkUrl
    ,a.detail as LinkDetail
    ,a.is_visible as IsVisible
    ,is_active AS IsActive
FROM
    API_LINK a
WHERE
    a.api_id = /*ds apiId*/'1' 
    AND a.is_active = 1

";
            }
            else
            {
                sql = @"
SELECT
    a.api_link_id as ApiLinkId
    ,a.title as LinkTitle
    ,a.url as LinkUrl
    ,a.detail as LinkDetail
    ,a.is_visible as IsVisible
    ,is_active AS IsActive
FROM
    ApiLink a
WHERE
    a.api_id = @apiId
    AND a.is_active = 1

";
            }

            var param = new { apiId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ApiLinkModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// VendorIdに紐づくApiLinkを取得する
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのSampleCodeを取得する</param>
        /// <returns>ApiLink</returns>
        private Dictionary<string, IEnumerable<ApiLinkModel>> GetApiLink(string vendorId, bool isAll)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    al.api_id as ApiId
    ,al.api_link_id as ApiLinkId
    ,al.title as LinkTitle
    ,al.url as LinkUrl
    ,al.detail as LinkDetail
    ,al.is_visible as IsVisible
FROM
    API_LINK al /*WITH(NOLOCK)*/
    INNER JOIN API a /*WITH(NOLOCK)*/ ON a.api_id = al.api_id AND a.is_active = 1
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = a.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendor_id*/'1' AND
/*ds end if*/
    al.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    al.api_id as ApiId
    ,al.api_link_id as ApiLinkId
    ,al.title as LinkTitle
    ,al.url as LinkUrl
    ,al.detail as LinkDetail
    ,al.is_visible as IsVisible
FROM
    ApiLink al WITH(NOLOCK)
    INNER JOIN Api a WITH(NOLOCK) ON a.api_id = al.api_id AND a.is_active = 1
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = a.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendor_id AND
/*ds end if*/
    al.is_active = 1
";
            }

            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendor_id", vendorId);

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var result = Connection.Query<string, ApiLinkModel, (string ApiId, ApiLinkModel ApiLink)>(
                sql: twowaySql.Sql,
                splitOn: "ApiLinkId",
                map: (apiId, apiLink) => (apiId, apiLink),
                param: dynParams);

            return result.GroupBy(x => x.ApiId).ToDictionary(x => x.Key, y => y.Select(z => z.ApiLink));
        }

        /// <summary>
        /// ApiLinkを登録または更新します。
        /// </summary>
        /// <param name="model">APIリンクモデル</param>
        /// <param name="apiId">APIID</param>
        /// <returns>APIリンクモデル</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<ApiLinkModel> UpsertApiLink(List<ApiLinkModel> modelList, string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();

            List<object> paramList = new();
            foreach (var model in modelList)
            {
                if (string.IsNullOrEmpty(model.LinkDetail))
                {
                    throw new ArgumentNullException(model.LinkDetail);
                }

                if (string.IsNullOrEmpty(model.LinkTitle))
                {
                    throw new ArgumentNullException(model.LinkTitle);
                }

                if (string.IsNullOrEmpty(model.LinkUrl))
                {
                    throw new ArgumentNullException(model.LinkUrl);
                }

                if (string.IsNullOrEmpty(model.ApiLinkId)) model.ApiLinkId = Guid.NewGuid().ToString();

                var param = new
                {
                    api_id = apiId,
                    api_link_id = model.ApiLinkId,
                    title = model.LinkTitle,
                    detail = model.LinkDetail,
                    url = model.LinkUrl,
                    is_visible = model.IsVisible,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = model.IsActive
                };
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                paramList.Add(dynParams);
            }

            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO API_LINK target
USING
(
    SELECT
        /*ds api_link_id*/'1' AS api_link_id
    FROM DUAL
) source
ON
    (target.api_link_id = source.api_link_id)
WHEN MATCHED THEN
    UPDATE
    SET
        title = /*ds title*/'1' 
        ,detail = /*ds detail*/'1' 
        ,url = /*ds url*/'1' 
        ,is_visible = /*ds is_visible*/1 
        ,is_active = /*ds is_active*/1 
        ,upd_date = /*ds upd_date*/systimestamp 
        ,upd_username = /*ds upd_username*/'1' 
WHEN NOT MATCHED THEN
    INSERT
    (
        api_id
        ,api_link_id
        ,title
        ,detail
        ,url
        ,is_visible
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        /*ds api_id*/'1' 
        ,/*ds api_link_id*/'1' 
        ,/*ds title*/'1' 
        ,/*ds detail*/'1' 
        ,/*ds url*/'1' 
        ,/*ds is_visible*/1 
        ,/*ds reg_date*/systimestamp 
        ,/*ds reg_username*/'1' 
        ,/*ds upd_date*/systimestamp 
        ,/*ds upd_username*/'1' 
        ,/*ds is_active*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO ApiLink AS target
USING
(
    SELECT
        @api_link_id AS api_link_id
) AS source
ON
    target.api_link_id = source.api_link_id
WHEN MATCHED THEN
    UPDATE
    SET
        title = @title
        ,detail = @detail
        ,url = @url
        ,is_visible = @is_visible
        ,is_active = @is_active
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        api_id
        ,api_link_id
        ,title
        ,detail
        ,url
        ,is_visible
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        @api_id
        ,@api_link_id
        ,@title
        ,@detail
        ,@url
        ,@is_visible
        ,@reg_date
        ,@reg_username
        ,@upd_date
        ,@upd_username
        ,@is_active
    );
";
            }

            var paramNames = new
            {
                api_id = 1,
                api_link_id = 1,
                title = 1,
                detail = 1,
                url = 1,
                is_visible = true,
                reg_date = UtcNow,
                reg_username = PerRequestDataContainer.OpenId,
                upd_date = UtcNow,
                upd_username = PerRequestDataContainer.OpenId,
                is_active = true
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, paramNames);
            Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            return modelList;
        }
        #endregion

        #region SampleCode
        /// <summary>
        /// ApiIdに紐づくSampleCodeを取得します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns>SampleCode</returns>
        private IEnumerable<SampleCodeModel> GetSampleCodeList(string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    s.sample_code_id as SampleCodeId
    ,s.language_id as LanguageId
    ,l.language_name as Language
    ,s.code as Code
FROM
    SAMPLE_CODE s
    INNER JOIN LANGUAGE l ON l.language_id = s.language_id AND l.is_active = 1
WHERE
    s.is_active = 1
    AND s.api_id = /*ds apiId*/'1' 
";
            }
            else
            {
                sql = @"
SELECT
    s.sample_code_id as SampleCodeId
    ,s.language_id as LanguageId
    ,l.language_name as Language
    ,s.code as Code
FROM
    SampleCode s
    INNER JOIN Language l ON l.language_id = s.language_id AND l.is_active = 1
WHERE
    s.is_active = 1
    AND s.api_id = @apiId
";
            }

            var param = new { apiId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<SampleCodeModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// VendorIdに紐づくSampleCodeを取得します。
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのSampleCodeを取得する</param>
        /// <returns>SampleCode</returns>
        private Dictionary<string, IEnumerable<SampleCodeModel>> GetSampleCode(string vendorId, bool isAll)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    sc.api_id as ApiId
    ,sc.sample_code_id as SampleCodeId
    ,sc.language_id as LanguageId
    ,l.language_name as Language
    ,sc.code as Code
FROM
    SAMPLE_CODE sc /*WITH(NOLOCK)*/
    INNER JOIN API a /*WITH(NOLOCK)*/ ON a.api_id = sc.api_id AND a.is_active = 1
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = a.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN LANGUAGE l /*WITH(NOLOCK)*/ ON l.language_id = sc.language_id
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendorId*/'1' AND
/*ds end if*/
    sc.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    sc.api_id as ApiId
    ,sc.sample_code_id as SampleCodeId
    ,sc.language_id as LanguageId
    ,l.language_name as Language
    ,sc.code as Code
FROM
    SampleCode sc WITH(NOLOCK)
    INNER JOIN Api a WITH(NOLOCK) ON a.api_id = sc.api_id AND a.is_active = 1
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = a.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN Language l WITH(NOLOCK) ON l.language_id = sc.language_id
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendorId AND
/*ds end if*/
    sc.is_active = 1
";
            }

            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendorId", vendorId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var result = Connection.Query<string, SampleCodeModel, (string ApiId, SampleCodeModel SampleCode)>(
                sql: twowaySql.Sql,
                splitOn: "SampleCodeId",
                map: (apiId, sampleCode) => (apiId, sampleCode),
                param: dynParams);

            return result.GroupBy(x => x.ApiId).ToDictionary(x => x.Key, y => y.Select(z => z.SampleCode));
        }

        /// <summary>
        /// サンプルコードを登録または更新します。
        /// </summary>
        /// <param name="model">サンプルコードモデル</param>
        /// <param name="apiId">APIID</param>
        /// <returns>サンプルコードモデル</returns>
        public List<SampleCodeModel> UpsertSampleCode(List<SampleCodeModel> modelList, string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();

            List<object> paramList = new();
            foreach (var model in modelList)
            {
                if (string.IsNullOrEmpty(model.LanguageId))
                {
                    throw new ArgumentNullException(model.LanguageId);
                }

                if (string.IsNullOrEmpty(model.SampleCodeId)) model.SampleCodeId = Guid.NewGuid().ToString();

                var param = new
                {
                    api_id = apiId,
                    sample_code_id = model.SampleCodeId,
                    code = model.Code ?? " ",
                    language_id = model.LanguageId,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = model.IsActive
                };
                var dynParam = dbSettings.GetParameters().AddDynamicParams(param).SetNClob(nameof(param.code));
                paramList.Add(dynParam);
            }

            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO SAMPLE_CODE target
    USING
    (
        SELECT
            /*ds sample_code_id*/'1' AS sample_code_id
        FROM DUAL
    ) source
    ON
        (target.sample_code_id = source.sample_code_id)
    WHEN MATCHED THEN
        UPDATE
        SET
            code = /*ds code*/'1' 
            ,language_id = /*ds language_id*/'1' 
            ,is_active = /*ds is_active*/1 
            ,upd_date = /*ds upd_date*/systimestamp 
            ,upd_username = /*ds upd_username*/'1' 
    WHEN NOT MATCHED THEN
        INSERT
        (
            api_id
            ,sample_code_id
            ,code
            ,language_id
            ,reg_date
            ,reg_username
            ,upd_date
            ,upd_username
            ,is_active
        )
        VALUES
        (
            /*ds api_id*/'1' 
            ,/*ds sample_code_id*/'1' 
            ,/*ds code*/'1' 
            ,/*ds language_id*/'1' 
            ,/*ds reg_date*/systimestamp 
            ,/*ds reg_username*/'1' 
            ,/*ds upd_date*/systimestamp 
            ,/*ds upd_username*/'1' 
            ,/*ds is_active*/1 
        )
";
            }
            else
            {
                sql = @"
MERGE INTO SampleCode AS target
    USING
    (
        SELECT
            @sample_code_id AS sample_code_id
    ) AS source
    ON
        target.sample_code_id = source.sample_code_id
    WHEN MATCHED THEN
        UPDATE
        SET
            code = @code
            ,language_id = @language_id
            ,is_active = @is_active
            ,upd_date = @upd_date
            ,upd_username = @upd_username
    WHEN NOT MATCHED THEN
        INSERT
        (
            api_id
            ,sample_code_id
            ,code
            ,language_id
            ,reg_date
            ,reg_username
            ,upd_date
            ,upd_username
            ,is_active
        )
        VALUES
        (
            @api_id
            ,@sample_code_id
            ,@code
            ,@language_id
            ,@reg_date
            ,@reg_username
            ,@upd_date
            ,@upd_username
            ,@is_active
        );
";
            }

            var parserParam = new
            {
                api_id = 1,
                sample_code_id = 1,
                code = 1,
                language_id = 1,
                reg_date = UtcNow,
                reg_username = PerRequestDataContainer.OpenId,
                upd_date = UtcNow,
                upd_username = PerRequestDataContainer.OpenId,
                is_active = true
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, parserParam);

            Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            return modelList;
        }
        #endregion

        #region SecondaryRepositoryMap
        /// <summary>
        /// ApiIdに紐づくSecondaryRepositoryMapを取得します。
        /// </summary>
        /// <param name="apiId">ApiId</param>
        /// <returns>SecondaryRepositoryMap</returns>
        private IEnumerable<SecondaryRepositoryMapQueryModel> GetSecondaryRepositoryMapList(string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    s.secondary_repository_map_id as SecondaryRepositoryMapId
    ,s.repository_group_id as RepositoryGroupId
    ,r.repository_group_name as RepositoryGroupName
    ,s.is_primary as IsPrimary
FROM
    SECONDARY_REPOSITORY_MAP s
    INNER JOIN REPOSITORY_GROUP r ON r.repository_group_id = s.repository_group_id AND r.is_active = 1
WHERE
    s.api_id = /*ds apiId*/'1' 
    AND s.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    s.secondary_repository_map_id as SecondaryRepositoryMapId
    ,s.repository_group_id as RepositoryGroupId
    ,r.repository_group_name as RepositoryGroupName
    ,s.is_primary as IsPrimary
FROM
    SecondaryRepositoryMap s
    INNER JOIN RepositoryGroup r ON r.repository_group_id = s.repository_group_id AND r.is_active = 1
WHERE
    s.api_id = @apiId
    AND s.is_active = 1
";
            }
            var param = new { apiId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<SecondaryRepositoryMapQueryModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// セカンダリリポジトリが重複しているか
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="AlreadyExistsException"></exception>
        public bool IsDuplicateSecondaryRepositoryMap(IEnumerable<SecondaryRepositoryMapModel> model)
        {
            if (model.GroupBy(p => p?.RepositoryGroupId?.ToLower()).Any(p => p.Count() > 1))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// ApiIDに紐づくSecondaryRepositoryMapリストを取得します。
        /// </summary>
        /// <param name="apiId">ApiID</param>
        /// <param name="vendorId">VendorID</param>
        /// <returns>SecondaryRepositoryMapリスト</returns>
        public IList<SecondaryRepositoryMapModel> GetSecondaryRepositoryMapList(string apiId, string vendorId)
        {
            if (string.IsNullOrEmpty(vendorId))
                throw new ArgumentNullException(nameof(vendorId));

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    rgt.repository_group_id RepositoryGroupId,
    rgt.repository_group_name RepositoryGroupName,
    rgt.is_container_separation IsContainerDynamicSeparation,
    rgt.repository_type_cd RepositoryTypeCd,
    rgt.repository_type_name RepositoryTypeName,
    sr.secondary_repository_map_id SecondaryRepositoryMapId,
    sr.is_active IsActive
FROM
(
    SELECT 
        rg.*,
        rt.is_container_separation,
        rt.repository_type_name
    FROM 
        REPOSITORY_GROUP rg INNER JOIN REPOSITORY_TYPE rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.is_active = 1
    WHERE 
        rg.repository_group_id IN
        (
            SELECT
                rg.repository_group_id
            FROM
                vendor_repository_group rg
            WHERE
                rg.vendor_id = /*ds vendorId*/'1' 
                AND rg.is_active = 1
        )
    AND
        rg.is_active = 1
) rgt
    LEFT JOIN SECONDARY_REPOSITORY_MAP sr ON rgt.repository_group_id = sr.repository_group_id AND sr.api_id = /*ds apiId*/'1' AND sr.is_active = 1
ORDER BY
    rgt.sort_no
";
            }
            else
            {
                sql = @"
SELECT
    rgt.repository_group_id AS RepositoryGroupId,
    rgt.repository_group_name AS RepositoryGroupName,
    rgt.is_container_separation AS IsContainerDynamicSeparation,
    rgt.repository_type_cd AS RepositoryTypeCd,
    rgt.repository_type_name AS RepositoryTypeName,
    sr.secondary_repository_map_id AS SecondaryRepositoryMapId,
    sr.is_active AS IsActive
FROM
(
    SELECT 
        rg.*,
        rt.is_container_separation,
        rt.repository_type_name
    FROM 
        RepositoryGroup rg INNER JOIN RepositoryType rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.is_active = 1
    WHERE 
        rg.repository_group_id IN
        (
            SELECT
                rg.repository_group_id
            FROM
                VendorRepositoryGroup rg
            WHERE
                rg.vendor_id = @vendorId
                AND rg.is_active = 1
        )
    AND
        rg.is_active = 1
) AS rgt
    LEFT JOIN SecondaryRepositoryMap sr ON rgt.repository_group_id = sr.repository_group_id AND sr.api_id = @apiId AND sr.is_active = 1
ORDER BY
    rgt.sort_no
";
            }

            var param = new { apiId = apiId ?? null, vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<SecondaryRepositoryMapModel>(twowaySql.Sql, dynParams)?.ToList();
        }

        /// <summary>
        /// コンテナ分離対応されたセカンダリリポジトリを持っているか
        /// </summary>
        /// <param name="apiId">ApiID</param>
        /// <param name="vendorId">VendorID</param>
        /// <returns>SecondaryRepositoryMapリスト</returns>
        public bool HasContainerDynamicSeparationSecondaryRepositoryMap(string apiId, string vendorId, string repositoryGroupId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(rgt.repository_group_id)
FROM
(
    SELECT 
        rg.repository_group_id,
        rt.is_container_separation
    FROM 
        REPOSITORY_GROUP rg INNER JOIN REPOSITORY_TYPE rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.is_active = 1
    WHERE 
        rg.repository_group_id IN
        (
            SELECT
                rg.repository_group_id
            FROM
                vendor_repository_group rg
            WHERE
                rg.vendor_id = /*ds vendorId*/'1' 
                AND rg.is_active = 1
        )
    AND rg.is_active = 1
) rgt
    LEFT JOIN SECONDARY_REPOSITORY_MAP sr ON rgt.repository_group_id = sr.repository_group_id AND sr.api_id = /*ds apiId*/'1' 
 WHERE
    rgt.repository_group_id = /*ds repositoryGroupId*/'1' 
    AND rgt.is_container_separation = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(rgt.repository_group_id),
FROM
(
    SELECT 
        rg.repository_group_id,
        rt.is_container_separation,
    FROM 
        RepositoryGroup rg INNER JOIN RepositoryType rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.is_active = 1
    WHERE 
        rg.repository_group_id IN
        (
            SELECT
                rg.repository_group_id
            FROM
                VendorRepositoryGroup rg
            WHERE
                rg.vendor_id = @vendorId
                AND rg.is_active = 1
        )
    AND rg.is_active = 1
) AS rgt
    LEFT JOIN SecondaryRepositoryMap sr ON rgt.repository_group_id = sr.repository_group_id AND sr.api_id = @apiId
WHERE
    rgt.repository_group_id = @repositoryGroupId
    AND rgt.is_container_separation = 1
";
            }
            var param = new { apiId = apiId ?? null, vendorId, repositoryGroupId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            return result > 0;
        }

        /// <summary>
        /// VendorIdに紐づくSecondaryRepositoryMapを取得します。
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのSecondaryRepositoryMapを取得するか</param>
        /// <returns>SecondaryRepositoryMap</returns>
        private Dictionary<string, IEnumerable<SecondaryRepositoryMapQueryModel>> GetSecondaryRepositoryMap(string vendorId, bool isAll)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    sr.api_id as ApiId
    ,sr.secondary_repository_map_id as SecondaryRepositoryMapId
    ,sr.repository_group_id as RepositoryGroupId
    ,r.repository_group_name as RepositoryGroupName
    ,sr.is_primary as IsPrimary
FROM
    SECONDARY_REPOSITORY_MAP sr /*WITH(NOLOCK)*/
    INNER JOIN API a /*WITH(NOLOCK)*/ ON a.api_id = sr.api_id AND a.is_active = 1
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = a.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN REPOSITORY_GROUP r /*WITH(NOLOCK)*/ ON r.repository_group_id = sr.repository_group_id AND r.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendor_id*/'1' AND
/*ds end if*/
    sr.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    sr.api_id as ApiId
    ,sr.secondary_repository_map_id as SecondaryRepositoryMapId
    ,sr.repository_group_id as RepositoryGroupId
    ,r.repository_group_name as RepositoryGroupName
    ,sr.is_primary as IsPrimary
FROM
    SecondaryRepositoryMap sr WITH(NOLOCK)
    INNER JOIN Api a WITH(NOLOCK) ON a.api_id = sr.api_id AND a.is_active = 1
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = a.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN RepositoryGroup r WITH(NOLOCK) ON r.repository_group_id = sr.repository_group_id AND r.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendor_id AND
/*ds end if*/
    sr.is_active = 1
";
            }

            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendor_id", vendorId);

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var result = Connection.Query<string, SecondaryRepositoryMapQueryModel, (string ApiId, SecondaryRepositoryMapQueryModel SecondaryRepositoryMap)>(
                sql: twowaySql.Sql,
                splitOn: "SecondaryRepositoryMapId",
                map: (apiId, secondaryRepositoryMap) => (apiId, secondaryRepositoryMap),
                param: dynParams);

            return result.GroupBy(x => x.ApiId).ToDictionary(x => x.Key, y => y.Select(z => z.SecondaryRepositoryMap));
        }

        /// <summary>
        /// SecondaryRepositoryMapListを登録または更新します。
        /// </summary>
        /// <param name="modelList">セカンダリーリポジトリマップモデル</param>
        /// <param name="apiId">APIID</param>
        /// <returns>セカンダリーリポジトリマップモデル</returns>
        public IEnumerable<SecondaryRepositoryMapModel> UpsertSecondaryRepositoryMapList(IEnumerable<SecondaryRepositoryMapModel> modelList, string apiId)
        {
            modelList.ToList().Where(x => string.IsNullOrEmpty(x.SecondaryRepositoryMapId)).ToList().ForEach(x => x.SecondaryRepositoryMapId ??= Guid.NewGuid().ToString());
            modelList.ToList().Where(x => string.IsNullOrEmpty(x.ApiId)).ToList().ForEach(x => x.ApiId = apiId);

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO SECONDARY_REPOSITORY_MAP Target
USING
(
    SELECT
        /*ds repository_group_id*/'1' AS repository_group_id
        ,/*ds api_id*/'1' AS api_id
    FROM DUAL
) Source
ON
    (target.repository_group_id = source.repository_group_id
    AND target.api_id = source.api_id)
WHEN MATCHED THEN
UPDATE
    SET
        is_active = /*ds is_active*/'1' 
        ,upd_date = /*ds upd_date*/systimestamp 
        ,upd_username = /*ds upd_username*/'1' 
 WHEN NOT MATCHED THEN
    INSERT
    (
        secondary_repository_map_id
        ,api_id
        ,repository_group_id
        ,is_primary
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        /*ds secondary_repository_map_id*/'1' 
        ,/*ds api_id*/'1' 
        ,/*ds repository_group_id*/'1' 
        ,0
        ,/*ds reg_date*/systimestamp 
        ,/*ds reg_username*/'1' 
        ,/*ds upd_date*/systimestamp  
        ,/*ds upd_username*/'1' 
        ,/*ds is_active*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO SecondaryRepositoryMap AS Target  
USING
(
    SELECT
        @repository_group_id AS repository_group_id
        ,@api_id AS api_id
) AS Source
ON
    target.repository_group_id = source.repository_group_id
    AND target.api_id = source.api_id
WHEN MATCHED THEN
UPDATE
    SET
        is_active = @is_active
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        secondary_repository_map_id
        ,api_id
        ,repository_group_id
        ,is_primary
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        @secondary_repository_map_id
        ,@api_id
        ,@repository_group_id
        ,0
        ,@reg_date
        ,@reg_username
        ,@upd_date
        ,@upd_username
        ,@is_active
    );
";
            }
            List<object> paramList = new();
            var paramNames = new
            {
                secondary_repository_map_id = 1,
                api_id = 1,
                repository_group_id = 1,
                reg_date = UtcNow,
                reg_username = PerRequestDataContainer.OpenId,
                upd_date = UtcNow,
                upd_username = PerRequestDataContainer.OpenId,
                is_active = true
            };
            foreach (var model in modelList)
            {
                var param = new
                {
                    secondary_repository_map_id = model.SecondaryRepositoryMapId,
                    api_id = model.ApiId,
                    repository_group_id = model.RepositoryGroupId,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = model.IsActive
                };
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                paramList.Add(dynParams);
            }

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, paramNames);
            try
            {
                Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            }
            catch (SqlException ex)
            {
                // 外部キー制約違反の場合、独自例外を返す
                if (ex.Number == 547) throw new ForeignKeyException(ex.Message);
                throw;
            }
            return modelList;
        }

        /// <summary>
        /// ApiIdに紐づくSecondaryRepositoryMapをすべて削除します。
        /// </summary>
        /// <param name="apiId">APIID</param>
        public void DeleteSecondaryRepositoryMap(string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    SECONDARY_REPOSITORY_MAP
SET
    is_active = 0,
    upd_date = /*ds upd_date*/systimestamp ,
    upd_username = /*ds upd_username*/'1' 
 WHERE
    api_id = /*ds apiId*/'1' 
";
            }
            else
            {
                sql = @"
UPDATE
    SecondaryRepositoryMap
SET
    is_active = 0,
    upd_date = @upd_date,
    upd_username = @upd_username
WHERE
    api_id = @apiId
";
            }
            var param = new
            {
                apiId,
                upd_date = UtcNow,
                upd_username = PerRequestDataContainer.OpenId
            };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.Execute(twowaySql.Sql, dynParams);
        }
        #endregion

        #region ApiMultiLanguage
        /// <summary>
        /// ApiMultiLanguageを登録または更新します。
        /// </summary>
        /// <param name="model">ApiMultiLanguageModel</param>
        /// <param name="apiId">APIID</param>
        /// <returns>ApiMultiLanguageModelリスト</returns>
        public List<ApiMultiLanguageModel> UpsertApiMultiLanguage(List<ApiMultiLanguageModel> model, string apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();

            // 既存データを全て無効化
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"UPDATE API_MULTI_LANGUAGE SET is_active = 0 WHERE api_id = /*ds api_id*/'1' ";
            }
            else
            {
                sql = @"UPDATE ApiMultiLanguage SET is_active = 0 WHERE api_id = @api_id";
            }

            var param = new { api_id = apiId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.Execute(twowaySql.Sql, dynParams);
            if (model == null || !model.Any())
            {
                return model;
            }

            // 登録・更新
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO API_MULTI_LANGUAGE target
USING
(
    SELECT
        /*ds api_id*/'1' AS api_id,
        /*ds locale_code*/'1' AS locale_code
    FROM DUAL
) source
ON
    (target.api_id = source.api_id AND target.locale_code = source.locale_code)
WHEN MATCHED THEN
    UPDATE
    SET
        api_description = /*ds api_description*/'1' 
        ,is_active = /*ds is_active*/1 
        ,upd_date = /*ds upd_date*/systimestamp 
        ,upd_username = /*ds upd_username*/'1' 
 WHEN NOT MATCHED THEN
    INSERT
    (
        api_lang_id
        ,api_id
        ,locale_code
        ,api_description
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        /*ds api_lang_id*/'1' 
        ,/*ds api_id*/'1' 
        ,/*ds locale_code*/'1' 
        ,/*ds api_description*/'1' 
        ,/*ds reg_date*/systimestamp 
        ,/*ds reg_username*/'1' 
        ,/*ds upd_date*/systimestamp 
        ,/*ds upd_username*/'1' 
        ,/*ds is_active*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO ApiMultiLanguage AS target
USING
(
    SELECT
        @api_id AS api_id,
        @locale_code AS locale_code
) AS source
ON
    target.api_id = source.api_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN
    UPDATE
    SET
        api_description = @api_description
        ,is_active = @is_active
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        api_lang_id
        ,api_id
        ,locale_code
        ,api_description
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        @api_lang_id
        ,@api_id
        ,@locale_code
        ,@api_description
        ,@reg_date
        ,@reg_username
        ,@upd_date
        ,@upd_username
        ,@is_active
    );
";
            }

            List<object> paramList = new();
            foreach (var multiLanguage in model)
            {
                var paramTmp = new
                {
                    api_lang_id = multiLanguage.ApiLangId,
                    api_id = apiId,
                    locale_code = multiLanguage.LocaleCode,
                    api_description = multiLanguage.ApiDescription,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = multiLanguage.IsActive
                };
                var dynParam = dbSettings.GetParameters().AddDynamicParams(paramTmp).SetNClob(nameof(paramTmp.api_description));
                paramList.Add(dynParam);
            }

            var paramNames = new
            {
                api_lang_id = 1,
                api_id = apiId,
                locale_code = 1,
                api_description = 1,
                reg_date = UtcNow,
                reg_username = PerRequestDataContainer.OpenId,
                upd_date = UtcNow,
                upd_username = PerRequestDataContainer.OpenId,
                is_active = true
            };

            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, paramNames);
            Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            return model;
        }
        #endregion

        #region ApiAccessOpenId
        /// <summary>
        /// ApiIdとOpenIdに紐づく有効なApiAccessOpenIdを一件取得します。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <param name="openId">openId</param>
        /// <param name="isActiveOnly">有効なもののみ取得するかどうか</param>
        /// <returns>ApiAccessOpenId</returns>
        public ApiAccessOpenIdInfoModel GetApiAccessOpenId(string apiId, string openId, bool isActiveOnly = true)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    api_access_open_id as ApiAccessOpenId
    ,api_id as ApiId
    ,open_id as OpenId
    ,is_enable as IsEnable
    ,access_key as AccessKey
    ,is_active as IsActive
FROM
    API_ACCESS_OPEN_ID
WHERE
    api_id = /*ds apiId*/'1' 
    AND open_id = /*ds openId*/'1' 
    AND is_enable = /*ds isActiveList*/1 
    AND is_active = /*ds isActiveList*/1 
";
            }
            else
            {
                sql = @"
SELECT
    api_access_openId as ApiAccessOpenId
    ,api_id as ApiId
    ,open_id as OpenId
    ,is_enable as IsEnable
    ,access_key as AccessKey
    ,is_active as IsActive
FROM
    ApiAccessOpenId
WHERE
    api_id = @apiId
    AND open_id = @openId
    AND is_enable IN @isActiveList
    AND is_active IN @isActiveList
";
            }

            bool? isActiveListForOracle = true;
            var isActiveListForSQLServer = new List<bool>() { true };
            if (!isActiveOnly)
            {
                isActiveListForOracle = false;
                isActiveListForSQLServer.Add(false);
            }
            var dict = new Dictionary<string, object>()
            {
                { "apiId", apiId },
                { "openId", openId },
                { "isActiveList",  dbSettings.Type == "Oracle" ? isActiveListForOracle : isActiveListForSQLServer }
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, dict);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(dict);
            return Connection.QuerySingleOrDefault<ApiAccessOpenIdInfoModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// ApiAccessOpenIdを登録します。
        /// </summary>
        /// <param name="model">ApiAccessOpenIdInfoModel</param>
        /// <returns>ApiAccessOpenIdInfoModel</returns>
        public ApiAccessOpenIdInfoModel RegisterApiAccessOpenId(ApiAccessOpenIdInfoModel model)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO API_ACCESS_OPEN_ID target
USING
(
    SELECT
        /*ds api_id*/'1' AS api_id,
        /*ds open_id*/'1' AS open_id
    FROM DUAL
) source
ON
    (target.api_id = source.api_id
AND target.open_id = source.open_id)
WHEN MATCHED THEN
UPDATE
    SET
        access_key = /*ds access_key*/'1' ,
        is_enable = /*ds is_enable*/1 ,
        is_active = /*ds is_active*/1 ,
        upd_date = /*ds upd_date*/systimestamp ,
        upd_username = /*ds upd_username*/'1' 
 WHEN NOT MATCHED THEN
    INSERT
    (
        api_access_open_id,
        api_id,
        open_id,
        is_enable,
        access_key,
        reg_date,
        reg_username,
        upd_date,
        upd_username,
        is_active
    )
    VALUES
    (
        /*ds api_access_openId*/'1' ,
        /*ds api_id*/'1' ,
        /*ds open_id*/'1' ,
        /*ds is_enable*/1 ,
        /*ds access_key*/'1' ,
        /*ds reg_date*/systimestamp ,
        /*ds reg_username*/'1' ,
        /*ds upd_date*/systimestamp ,
        /*ds upd_username*/'1' ,
        /*ds is_active*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO ApiAccessOpenId AS target
USING
(
    SELECT
        @api_id AS api_id,
        @open_id AS open_id
) AS source
ON
    target.api_id = source.api_id
AND target.open_id = source.open_id
WHEN MATCHED THEN
UPDATE
    SET
        access_key = @access_key,
        is_enable = @is_enable,
        is_active = @is_active,
        upd_date = @upd_date,
        upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        api_access_openId,
        api_id,
        open_id,
        is_enable,
        access_key,
        reg_date,
        reg_username,
        upd_date,
        upd_username,
        is_active
    )
    VALUES
    (
        @api_access_openId,
        @api_id,
        @open_id,
        @is_enable,
        @access_key,
        @reg_date,
        @reg_username,
        @upd_date,
        @upd_username,
        @is_active
    )
;
";
            }
            var param = new
            {
                api_access_openId = model.ApiAccessOpenId,
                api_id = model.ApiId,
                is_enable = model.IsEnable,
                is_active = true,
                access_key = model.AccessKey,
                open_id = model.OpenId,
                reg_date = UtcNow,
                reg_username = PerRequestDataContainer.OpenId,
                upd_date = UtcNow,
                upd_username = PerRequestDataContainer.OpenId
            };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.Execute(twowaySql.Sql, dynParams);

            return model;
        }

        /// <summary>
        /// ApiAccessOpenIdを削除します。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <param name="openId">openId</param>
        public void DeleteApiAccessOpenId(string apiId, string openId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE API_ACCESS_OPEN_ID
    SET 
        is_active = 0
        ,upd_date = /*ds upd_date*/systimestamp 
        ,upd_username = /*ds upd_username*/'1' 
 WHERE
    api_id = /*ds apiId*/'1' AND
    open_id = /*ds openId*/'1' AND
    is_active = 1
";
            }
            else
            {
                sql = @"
UPDATE ApiAccessOpenId
    SET 
        is_active = 0
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHERE
    api_id = @apiId
    AND open_id = @openId
    AND is_active = 1
";
            }
            var param = new { apiId, openId, upd_date = UtcNow, upd_username = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// ApiのOpenIdアクセスコントロールの有効/無効を設定します。
        /// </summary>
        /// <param name="apiId">apiId</param>
        /// <param name="isEnable">有効な場合true、無効な場合falseを指定する</param>
        public void UseApiAccessOpenId(string apiId, bool isEnable)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE API
    SET 
        is_openid_authentication_allow_null = /*ds is_openid_authentication_allow_null*/1
        ,upd_date = /*ds upd_date*/systimestamp
        ,upd_username = /*ds upd_username*/'1' 
 WHERE
    api_id = /*ds apiId*/'1'
    AND is_active = 1
";
            }
            else
            {
                sql = @"
UPDATE Api
    SET 
        is_openid_authentication_allow_null = @is_openid_authentication_allow_null
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHERE
    api_id = @apiId
    AND is_active = 1
";
            }
            var param = new
            {
                apiId,
                is_openid_authentication_allow_null = isEnable,
                upd_date = UtcNow,
                upd_username = PerRequestDataContainer.OpenId
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }
        #endregion

        /// <summary>
        /// パラメータを取得します。
        /// </summary>
        /// <param name="parameters">パラメータ</param>
        /// <param name="isQuery"></param>
        /// <returns></returns>
        public IEnumerable<string> GetParameters(IEnumerable<string> parameters)
            => parameters.Where(x => x != MongoDbConstants.CollectionNameVariable && x != SqlServerConsts.TableNameVariable);
    }
}
