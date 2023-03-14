using Dapper.Oracle;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.DymamicApi;
using JP.DataHub.ManageApi.Service.DymamicApi.Actions.AllApi;
using JP.DataHub.ManageApi.Service.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    /// <summary>
    /// コントローラに関連するDynamicApiRepository
    /// </summary>
    internal partial class DynamicApiRepository
    {
        #region Controller
        /// <summary>
        /// コントローラとAPIのリストを取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="isAll">全てのベンダーのリソースを取得するか</param>
        /// <returns>リソースのリスト</returns>
        public IEnumerable<ControllerQueryModel> GetControllerApiList(string vendorId, bool isAll)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.controller_id as ControllerId
    ,c.url as RelativeUrl
    ,c.controller_description as ControllerDescription
    ,c.vendor_id as VendorId
    ,v.vendor_name as VendorName
    ,c.system_id as SystemId
    ,s.system_name as SystemName
    ,c.is_static_api as IsStaticApi
    ,c.is_vendor as IsVendor
    ,c.is_person as IsPerson
    ,c.is_enable as IsEnable
    ,c.is_toppage as IsTopPage
    ,c.controller_schema_id as ControllerSchemaId
    ,ds.schema_name as ControllerSchemaName
    ,c.controller_repository_key as RepositoryKey
    ,c.controller_partition_key as PartitionKey
    ,c.controller_name as ControllerName
    ,c.is_data as IsData
    ,c.is_businesslogic as IsBusinessLogic
    ,c.is_pay as IsPay
    ,c.fee_description as FeeDescription
    ,c.resource_create_user as ResourceCreateUser
    ,c.resource_maintainer as ResourceMaintainer
    ,c.resource_create_date as ResourceCreateDate
    ,c.resource_latest_date as ResourceLatestDate
    ,c.update_frequency as UpdateFrequency
    ,c.is_contract as IsContract
    ,c.contact_information as ContactInfomation
    ,c.version as Version
    ,c.agree_description as AgreeDescription
    ,c.is_visible_agreement as IsVisibleAgreement
    ,c.reg_date as RegDate
    ,c.reg_username as RegUserName
    ,c.upd_date as UpdDate
    ,c.upd_username as UpdUserName
    ,c.is_active as IsActive
    ,c.is_optimistic_concurrency as IsOptimisticConcurrency
    ,c.is_document_history as IsDocumentHistory
    ,c.is_enable_blockchain as IsEnableBlockchain
    ,c.is_enable_attachFile as IsEnableAttachFile
    ,c.is_enable_resource_version as IsEnableResourceVersion
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
    CONTROLLER c /*WITH(NOLOCK)*/
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    LEFT JOIN API a /*WITH(NOLOCK)*/ ON c.controller_id = a.controller_id AND a.is_active = 1
    LEFT JOIN REPOSITORY_GROUP rg /*WITH(NOLOCK)*/ ON a.repository_group_id=rg.repository_group_id AND rg.is_active= 1
    LEFT JOIN DATA_SCHEMA ds /*WITH(NOLOCK)*/ ON ds.data_schema_id = c.controller_schema_id
    LEFT JOIN DATA_SCHEMA dsreq /*WITH(NOLOCK)*/ ON a.request_schema_id=dsreq.data_schema_id AND dsreq.is_active = 1
    LEFT JOIN DATA_SCHEMA dsres /*WITH(NOLOCK)*/ ON a.response_schema_id=dsres.data_schema_id AND dsres.is_active = 1
    LEFT JOIN DATA_SCHEMA dsurl /*WITH(NOLOCK)*/ ON a.url_schema_id=dsurl.data_schema_id AND dsurl.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendor_id*/'1' AND
/*ds end if*/
    c.is_active = 1
ORDER BY
    c.url, a.is_transparent_api, a.url";
            }
            else
            {
                sql = @"
SELECT
    c.controller_id as ControllerId
    ,c.url as RelativeUrl
    ,c.controller_description as ControllerDescription
    ,c.vendor_id as VendorId
    ,v.vendor_name as VendorName
    ,c.system_id as SystemId
    ,s.system_name as SystemName
    ,c.is_static_api as IsStaticApi
    ,c.is_vendor as IsVendor
    ,c.is_person as IsPerson
    ,c.is_enable as IsEnable
    ,c.is_toppage as IsTopPage
    ,c.controller_schema_id as ControllerSchemaId
    ,ds.schema_name as ControllerSchemaName
    ,c.controller_repository_key as RepositoryKey
    ,c.controller_partition_key as PartitionKey
    ,c.controller_name as ControllerName
    ,c.is_data as IsData
    ,c.is_businesslogic as IsBusinessLogic
    ,c.is_pay as IsPay
    ,c.fee_description as FeeDescription
    ,c.resource_create_user as ResourceCreateUser
    ,c.resource_maintainer as ResourceMaintainer
    ,c.resource_create_date as ResourceCreateDate
    ,c.resource_latest_date as ResourceLatestDate
    ,c.update_frequency as UpdateFrequency
    ,c.is_contract as IsContract
    ,c.contact_information as ContactInfomation
    ,c.version as Version
    ,c.agree_description as AgreeDescription
    ,c.is_visible_agreement as IsVisibleAgreement
    ,c.reg_date as RegDate
    ,c.reg_username as RegUserName
    ,c.upd_date as UpdDate
    ,c.upd_username as UpdUserName
    ,c.is_active as IsActive
    ,c.is_optimistic_concurrency as IsOptimisticConcurrency
    ,c.is_document_history as IsDocumentHistory
    ,c.is_enable_blockchain as IsEnableBlockchain
    ,c.is_enable_attachFile as IsEnableAttachFile
    ,c.is_enable_resource_version as IsEnableResourceVersion
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
    Controller c WITH(NOLOCK)
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    LEFT JOIN Api a WITH(NOLOCK) ON c.controller_id = a.controller_id AND a.is_active = 1
    LEFT JOIN RepositoryGroup rg WITH(NOLOCK) ON a.repository_group_id=rg.repository_group_id AND rg.is_active= 1
    LEFT JOIN DataSchema ds WITH(NOLOCK) ON ds.data_schema_id = c.controller_schema_id
    LEFT JOIN DataSchema dsreq WITH(NOLOCK) ON a.request_schema_id=dsreq.data_schema_id AND dsreq.is_active = 1
    LEFT JOIN DataSchema dsres WITH(NOLOCK) ON a.response_schema_id=dsres.data_schema_id AND dsres.is_active = 1
    LEFT JOIN DataSchema dsurl WITH(NOLOCK) ON a.url_schema_id=dsurl.data_schema_id AND dsurl.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendor_id AND
/*ds end if*/
    c.is_active = 1
ORDER BY
    c.url, a.is_transparent_api, a.url
";
            }
            var param = new { vendor_id = vendorId };
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendor_id", vendorId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<ControllerQueryModel, ApiQueryModel, (ControllerQueryModel Controller, ApiQueryModel Api)>(
                sql: twowaySql.Sql,
                splitOn: "ApiId",
                map: (controller, api) => (controller, api),
                param: dynParams);

            // プロパティ一括取得
            var controllerTagList = GetControllerTag(vendorId, isAll);
            var controllerCategoryList = GetContollerCategory(vendorId, isAll);
            var controllerFieldList = GetControllerField(vendorId, isAll);
            var controllerCommonIpFilterGroupList = GetControllerCommonIpFilterGroupByVendorId(vendorId, isAll);
            var controllerIpFilterList = GetControllerIpFilterByVendorId(vendorId, isAll);
            var controllerOpenIdCAList = GetControllerOpenIdCA(vendorId, isAll);

            var secondaryRepositoryMapList = GetSecondaryRepositoryMap(vendorId, isAll);
            var sampleCodeList = GetSampleCode(vendorId, isAll);
            var apiLinkList = GetApiLink(vendorId, isAll);
            var apiAccessVendorList = GetApiAccessVendor(vendorId, isAll);
            var apiOpenIdCAList = GetApiOpenIdCa(vendorId, isAll);

            // ツリー形式に変換
            var list = new List<ControllerQueryModel>();
            foreach (var group in result.GroupBy(x => x.Controller.ControllerId))
            {
                var first = group.First();
                var controllerId = first.Controller.ControllerId;
                first.Controller.ControllerTagList = GetSingleOrEmpty(controllerTagList, controllerId);
                first.Controller.ControllerCategoryList = GetSingleOrEmpty(controllerCategoryList, controllerId);
                first.Controller.ControllerFieldList = GetSingleOrEmpty(controllerFieldList, controllerId);
                first.Controller.ControllerCommonIpFilterGroupList = GetSingleOrEmpty(controllerCommonIpFilterGroupList, controllerId);
                first.Controller.ControllerIpFilterList = GetSingleOrEmpty(controllerIpFilterList, controllerId);
                first.Controller.ControllerOpenIdCAList = GetSingleOrEmpty(controllerOpenIdCAList, controllerId);
                var apiList = new List<ApiQueryModel>();
                first.Controller.ApiList = apiList;
                list.Add(first.Controller);

                // APIなし
                if (first.Api?.ApiId == null)
                {
                    continue;
                }

                // APIあり
                foreach (var api in group)
                {
                    var apiId = api.Api.ApiId;
                    api.Api.SecondaryRepositoryMapList = GetSingleOrEmpty(secondaryRepositoryMapList, apiId);
                    api.Api.SampleCodeList = GetSingleOrEmpty(sampleCodeList, apiId);
                    api.Api.ApiLinkList = GetSingleOrEmpty(apiLinkList, apiId);
                    api.Api.ApiAccessVendorList = GetSingleOrEmpty(apiAccessVendorList, apiId);
                    api.Api.ApiOpenIdCAList = GetSingleOrEmpty(apiOpenIdCAList, apiId);
                    apiList.Add(api.Api);
                }
            }

            return list.Any() ? list : null;
        }

        /// <summary>
        /// キーと一致するものを取得します。
        /// </summary>
        /// <param name="source">ソースリスト</param>
        /// <param name="key">キー</param>
        /// <returns></returns>
        private IEnumerable<T> GetSingleOrEmpty<T>(Dictionary<string, IEnumerable<T>> source, string key)
        {
            return source.SingleOrDefault(x => x.Key == key).Value ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// コントローラとAPIのリストを取得します。（内容はシンプルなもの、管理画面のツリーを表示するための項目のみ）
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="isAll">全てのベンダーのリソースを取得するか</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソースのリスト</returns>
        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_CONTROLLER, DynamicApiDatabase.TABLE_API)]
        [CacheArg(allParam: true)]
        public IEnumerable<ControllerSimpleQueryModel> GetControllerApiSimpleList(string vendorId, bool isAll, bool isTransparent)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.controller_id as ControllerId
    ,c.vendor_id as VendorId
    ,c.url as RelativeUrl
    ,a.api_id as ApiId
    ,a.controller_id as ControllerId
    ,a.method_type as MethodType
    ,a.url as ApiUrl
    ,a.is_transparent_api as IsTransparent 
FROM
    CONTROLLER c /*WITH(NOLOCK)*/
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_active = 1
    LEFT JOIN API a /*WITH(NOLOCK)*/ ON
        c.controller_id = a.controller_id AND
/*ds if NoTransparent != null*/
        a.is_transparent_api = 0 AND
/*ds end if*/
        a.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    c.is_active = 1
ORDER BY
    c.url, a.is_transparent_api, a.url
";
            }
            else
            {
                sql = @"
SELECT
    c.controller_id as ControllerId
    ,c.vendor_id as VendorId
    ,c.url as RelativeUrl
    ,a.api_id as ApiId
    ,a.controller_id as ControllerId
    ,a.method_type as MethodType
    ,a.url as ApiUrl
    ,a.is_transparent_api as IsTransparent 
FROM
    Controller c WITH(NOLOCK)
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_active = 1
    LEFT JOIN Api a WITH(NOLOCK) ON 
        c.controller_id = a.controller_id AND
/*ds if NoTransparent != null*/
        a.is_transparent_api = 0 AND
/*ds end if*/
        a.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendor_id AND
/*ds end if*/
    c.is_active = 1
ORDER BY
    c.url, a.is_transparent_api, a.url
";
            }

            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("NoTransparent", isTransparent ? null : true);
            twowaySqlParam.Add("vendor_id", vendorId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var result = Connection.Query<ControllerSimpleQueryModel, ApiSimpleQueryModel, (ControllerSimpleQueryModel Controller, ApiSimpleQueryModel Api)>(
                sql: twowaySql.Sql,
                splitOn: "ApiId",
                map: (controller, api) => (controller, api),
                param: dynParams);

            // ツリー形式に変換
            var list = new List<ControllerSimpleQueryModel>();
            foreach (var group in result.GroupBy(x => x.Controller.ControllerId))
            {
                var first = group.First();
                list.Add(first.Controller);

                var apiList = new List<ApiSimpleQueryModel>();
                first.Controller.ApiList = apiList;
                // APIなし
                if (first.Api?.ApiId == null)
                {
                    continue;
                }
                // APIあり
                foreach (var api in group)
                {
                    apiList.Add(api.Api);
                }
            }
            return list.Any() ? list : null;
        }

        /// <summary>
        /// コントローラを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <param name="staticApiOnly">StaticApiだけか</param>
        /// <returnsコントローラモデル></returns>
        public ControllerInformationModel GetController(string controllerId, bool staticApiOnly = false)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.controller_id AS ControllerId
    ,c.url AS Url
    ,c.controller_description AS ControllerDescription
    ,c.vendor_id AS VendorId
    ,c.system_id AS SystemId
    ,c.is_vendor AS IsVendor
    ,c.is_person AS IsPerson
    ,c.is_enable AS IsEnable
    ,c.is_active AS IsActive
    ,c.is_toppage AS IsTopPage
    ,c.is_static_api AS IsStaticApi
    ,c.controller_schema_id AS ControllerSchemaId
    ,c.controller_repository_key AS RepositoryKey
    ,c.controller_partition_key AS PartitionKey
    ,c.controller_name AS ControllerName
    ,c.is_data AS IsData
    ,c.is_businesslogic AS IsBusinessLogic
    ,c.is_pay AS IsPay
    ,c.fee_description AS FeeDescription
    ,c.resource_create_user AS ResourceCreateUser
    ,c.resource_maintainer AS ResourceMaintainer
    ,c.resource_create_date AS ResourceCreateDate
    ,c.resource_latest_date AS ResourceLatestDate
    ,c.update_frequency AS UpdateFrequency
    ,c.is_contract AS IsContract
    ,c.contact_information AS ContactInformation
    ,c.version AS Version
    ,c.agree_description AS AgreeDescription
    ,c.is_visible_agreement AS IsVisibleAgreement
    ,c.is_enable_ipfilter AS IsEnableIpFilter
    ,c.is_enable_blockchain AS IsEnableBlockchain
    ,c.is_optimistic_concurrency AS IsOptimisticConcurrency
    ,c.is_container_dynamic_separation AS IsContainerDynamicSeparation
    ,c.is_use_blob_cache AS IsUseBlobCache
    ,c.is_container_dynamic_separation AS IsContainerDynamicSeparation
    ,c.is_enable_resource_version AS IsEnableResourceVersion
FROM
    CONTROLLER c
WHERE
/*ds if controllerId != null*/
    c.controller_id = /*ds controllerId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    c.is_active = 1 AND
    c.is_static_api > /*ds staticApiOnly*/1 
";
            }
            else
            {
                sql = @"
SELECT
    c.controller_id AS ControllerId
    ,c.url AS Url
    ,c.controller_description AS ControllerDescription
    ,c.vendor_id AS VendorId
    ,c.system_id AS SystemId
    ,c.is_vendor AS IsVendor
    ,c.is_person AS IsPerson
    ,c.is_enable AS IsEnable
    ,c.is_active AS IsActive
    ,c.is_toppage AS IsTopPage
    ,c.is_static_api AS IsStaticApi
    ,c.controller_schema_id AS ControllerSchemaId
    ,c.controller_repository_key AS RepositoryKey
    ,c.controller_partition_key AS PartitionKey
    ,c.controller_name AS ControllerName
    ,c.is_data AS IsData
    ,c.is_businesslogic AS IsBusinessLogic
    ,c.is_pay AS IsPay
    ,c.fee_description AS FeeDescription
    ,c.resource_create_user AS ResourceCreateUser
    ,c.resource_maintainer AS ResourceMaintainer
    ,c.resource_create_date AS ResourceCreateDate
    ,c.resource_latest_date AS ResourceLatestDate
    ,c.update_frequency AS UpdateFrequency
    ,c.is_contract AS IsContract
    ,c.contact_information AS ContactInformation
    ,c.version AS Version
    ,c.agree_description AS AgreeDescription
    ,c.is_visible_agreement AS IsVisibleAgreement
    ,c.is_enable_ipfilter AS IsEnableIpFilter
    ,c.is_enable_blockchain AS IsEnableBlockchain
    ,c.is_optimistic_concurrency AS IsOptimisticConcurrency
    ,c.is_container_dynamic_separation AS IsContainerDynamicSeparation
    ,c.is_use_blob_cache AS IsUseBlobCache
    ,c.is_container_dynamic_separation AS IsContainerDynamicSeparation
    ,c.is_enable_resource_version AS IsEnableResourceVersion
FROM
    Controller c
WHERE
/*ds if controllerId != null*/
    c.controller_id = /*ds controllerId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    c.is_active = 1 AND
    c.is_static_api > @staticApiOnly
";
            }

            var param = new { controllerId, staticApiOnly = staticApiOnly ? 0 : -1 };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingleOrDefault<ControllerInformationModel>(twowaySql.Sql, dynParams);

            if (result != null)
            {
                result.ControllerIpFilterList = GetControllerIpFilterList(controllerId).ToList();
                result.CategoryList = GetContollerCategoryListContainControllerIdNull(controllerId);
            }

            return result;
        }

        /// <summary>
        /// ControllerIdに紐づくリソースを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        public ControllerQueryModel GetControllerResourceFromControllerId(string controllerId, bool isTransparent)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.controller_id as ControllerId
    ,c.url as RelativeUrl
    ,c.controller_description as ControllerDescription
    ,c.vendor_id as VendorId
    ,v.vendor_name as VendorName
    ,c.system_id as SystemId
    ,s.system_name as SystemName
    ,c.is_static_api as IsStaticApi
    ,c.is_vendor as IsVendor
    ,c.is_person as IsPerson
    ,c.is_enable as IsEnable
    ,c.is_toppage as IsTopPage
    ,c.controller_schema_id as ControllerSchemaId
    ,ds.schema_name as ControllerSchemaName
    ,c.controller_repository_key as RepositoryKey
    ,c.controller_partition_key as PartitionKey
    ,c.controller_name as ControllerName
    ,c.is_data as IsData
    ,c.is_businesslogic as IsBusinessLogic
    ,c.is_pay as IsPay
    ,c.fee_description as FeeDescription
    ,c.resource_create_user as ResourceCreateUser
    ,c.resource_maintainer as ResourceMaintainer
    ,c.resource_create_date as ResourceCreateDate
    ,c.resource_latest_date as ResourceLatestDate
    ,c.update_frequency as UpdateFrequency
    ,c.is_contract as IsContract
    ,c.contact_information as ContactInfomation
    ,c.version as Version
    ,c.agree_description as AgreeDescription
    ,c.is_visible_agreement as IsVisibleAgreement
    ,c.reg_date as RegDate
    ,c.reg_username as RegUserName
    ,c.upd_date as UpdDate
    ,c.upd_username as UpdUserName
    ,c.is_active as IsActive
    ,c.is_optimistic_concurrency as IsOptimisticConcurrency
    ,c.is_document_history as IsDocumentHistory
    ,c.is_enable_blockchain as IsEnableBlockchain
    ,c.is_enable_attachFile as IsEnableAttachFile
    ,c.is_enable_resource_version as IsEnableResourceVersion
FROM
    CONTROLLER c
    LEFT OUTER JOIN VENDOR v ON v.vendor_id = c.vendor_id AND v.is_active = 1
    LEFT OUTER JOIN SYSTEM s ON s.system_id = c.system_id AND s.is_active = 1
    LEFT OUTER JOIN DATA_SCHEMA ds ON ds.data_schema_id = c.controller_schema_id AND ds.is_active = 1
WHERE
    controller_id = /*ds controllerId*/'1' 
    AND c.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    c.controller_id as ControllerId
    ,c.url as RelativeUrl
    ,c.controller_description as ControllerDescription
    ,c.vendor_id as VendorId
    ,v.vendor_name as VendorName
    ,c.system_id as SystemId
    ,s.system_name as SystemName
    ,c.is_static_api as IsStaticApi
    ,c.is_vendor as IsVendor
    ,c.is_person as IsPerson
    ,c.is_enable as IsEnable
    ,c.is_toppage as IsTopPage
    ,c.controller_schema_id as ControllerSchemaId
    ,ds.schema_name as ControllerSchemaName
    ,c.controller_repository_key as RepositoryKey
    ,c.controller_partition_key as PartitionKey
    ,c.controller_name as ControllerName
    ,c.is_data as IsData
    ,c.is_businesslogic as IsBusinessLogic
    ,c.is_pay as IsPay
    ,c.fee_description as FeeDescription
    ,c.resource_create_user as ResourceCreateUser
    ,c.resource_maintainer as ResourceMaintainer
    ,c.resource_create_date as ResourceCreateDate
    ,c.resource_latest_date as ResourceLatestDate
    ,c.update_frequency as UpdateFrequency
    ,c.is_contract as IsContract
    ,c.contact_information as ContactInfomation
    ,c.version as Version
    ,c.agree_description as AgreeDescription
    ,c.is_visible_agreement as IsVisibleAgreement
    ,c.reg_date as RegDate
    ,c.reg_username as RegUserName
    ,c.upd_date as UpdDate
    ,c.upd_username as UpdUserName
    ,c.is_active as IsActive
    ,c.is_optimistic_concurrency as IsOptimisticConcurrency
    ,c.is_document_history as IsDocumentHistory
    ,c.is_enable_blockchain as IsEnableBlockchain
    ,c.is_enable_attachFile as IsEnableAttachFile
    ,c.is_enable_resource_version as IsEnableResourceVersion
FROM
    Controller c
    LEFT OUTER JOIN Vendor v ON v.vendor_id = c.vendor_id AND v.is_active = 1
    LEFT OUTER JOIN System s ON s.system_id = c.system_id AND s.is_active = 1
    LEFT OUTER JOIN DataSchema ds ON ds.data_schema_id = c.controller_schema_id AND ds.is_active = 1
WHERE
    controller_id = @controllerId
    AND c.is_active = 1
";
            }

            var param = new { controllerId, };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            // Controllerを取得
            var controller = Connection.QuerySingle<ControllerQueryModel>(twowaySql.Sql, dynParams);

            controller.ControllerTagList = GetControllerTagList(controllerId);
            controller.ControllerCategoryList = GetContollerCategoryList(controllerId);
            controller.ControllerFieldList = GetControllerFieldList(controllerId);
            controller.ControllerCommonIpFilterGroupList = GetControllerCommonIpFilterGroupList(controllerId);
            controller.ControllerIpFilterList = GetControllerIpFilterList(controllerId);
            controller.ControllerOpenIdCAList = GetControllerOpenIdCAList(controllerId);
            controller.ApiList = GetApiList(controllerId, isTransparent);

            return controller;
        }

        /// <summary>
        /// ControllerIdに紐づくリソースを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>リソース</returns>
        public ControllerLightQueryModel GetControllerResourceLight(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string ctrlSql;
            if (dbSettings.Type == "Oracle")
            {
                ctrlSql = @"
SELECT
    ControllerId
    ,RelativeUrl
    ,VendorId
    ,SystemId
    ,IsVendor
    ,IsPerson
    ,ControllerSchemaId
    ,CASE WHEN COUNT(1) OVER () = 1 THEN RepositoryGroupId ELSE null END as RepositoryGroupId
FROM (
    SELECT
        DISTINCT
        c.controller_id as ControllerId
        ,c.url as RelativeUrl
        ,c.vendor_id as VendorId
        ,c.system_id as SystemId
        ,c.is_vendor as IsVendor
        ,c.is_person as IsPerson
        ,c.controller_schema_id as ControllerSchemaId
        ,a.repository_group_id as RepositoryGroupId
    FROM
        CONTROLLER c
        INNER JOIN VENDOR v ON v.vendor_id = c.vendor_id AND v.is_active = 1
        INNER JOIN SYSTEM s ON s.system_id = c.system_id AND s.is_active = 1
        LEFT JOIN API a ON a.controller_id = c.controller_id AND a.is_active = 1 AND a.is_transparent_api = 0 AND a.repository_group_id IS NOT NULL
    WHERE
        c.controller_id = /*ds controllerId*/'00000000-0000-0000-0000-000000000000' 
        AND c.is_active = 1
) z
FETCH FIRST 1 ROW ONLY
";
            }
            else
            {
                ctrlSql = @"
SELECT
    TOP 1
    ControllerId
    ,RelativeUrl
    ,VendorId
    ,SystemId
    ,IsVendor
    ,IsPerson
    ,ControllerSchemaId
    ,CASE WHEN COUNT(1) OVER () = 1 THEN RepositoryGroupId ELSE null END as RepositoryGroupId
FROM (
    SELECT
        DISTINCT
        c.controller_id as ControllerId
        ,c.url as RelativeUrl
        ,c.vendor_id as VendorId
        ,c.system_id as SystemId
        ,c.is_vendor as IsVendor
        ,c.is_person as IsPerson
        ,c.controller_schema_id as ControllerSchemaId
        ,a.repository_group_id as RepositoryGroupId
    FROM
        Controller c
        INNER JOIN Vendor v ON v.vendor_id = c.vendor_id AND v.is_active = 1
        INNER JOIN System s ON s.system_id = c.system_id AND s.is_active = 1
        LEFT JOIN API a ON a.controller_id = c.controller_id AND a.is_active = 1 AND a.is_transparent_api = 0 AND a.repository_group_id IS NOT NULL
    WHERE
        c.controller_id = @controllerId
        AND c.is_active = 1
) z
";
            }

            var param = new { controllerId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), ctrlSql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var controller = Connection.QuerySingle<ControllerLightQueryModel>(twowaySql.Sql, dynParams);

            return controller;
        }

        /// <summary>
        /// URLに紐づくリソースを取得します。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="isTransparent">透過Apiを含めるか(true:含める false:含めない)</param>
        /// <returns>リソース</returns>
        public ControllerQueryModel GetControllerResourceFromUrl(string url, bool isTransparent)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.controller_id as ControllerId
    ,c.url as RelativeUrl
    ,c.controller_description as ControllerDescription
    ,c.vendor_id as VendorId
    ,v.vendor_name as VendorName
    ,c.system_id as SystemId
    ,s.system_name as SystemName
    ,c.is_static_api as IsStaticApi
    ,c.is_vendor as IsVendor
    ,c.is_person as IsPerson
    ,c.is_enable as IsEnable
    ,c.is_toppage as IsTopPage
    ,c.controller_schema_id as ControllerSchemaId
    ,ds.schema_name as ControllerSchemaName
    ,c.controller_repository_key as RepositoryKey
    ,c.controller_partition_key as PartitionKey
    ,c.controller_name as ControllerName
    ,c.is_data as IsData
    ,c.is_businesslogic as IsBusinessLogic
    ,c.is_pay as IsPay
    ,c.fee_description as FeeDescription
    ,c.resource_create_user as ResourceCreateUser
    ,c.resource_maintainer as ResourceMaintainer
    ,c.resource_create_date as ResourceCreateDate
    ,c.resource_latest_date as ResourceLatestDate
    ,c.update_frequency as UpdateFrequency
    ,c.is_contract as IsContract
    ,c.contact_information as ContactInfomation
    ,c.version as Version
    ,c.agree_description as AgreeDescription
    ,c.is_visible_agreement as IsVisibleAgreement
    ,c.reg_date as RegDate
    ,c.reg_username as RegUserName
    ,c.upd_date as UpdDate
    ,c.upd_username as UpdUserName
    ,c.is_active as IsActive
    ,c.is_optimistic_concurrency as IsOptimisticConcurrency
    ,c.is_document_history as IsDocumentHistory
    ,c.is_enable_blockchain as IsEnableBlockchain
    ,c.is_enable_attachFile as IsEnableAttachFile
    ,c.is_enable_resource_version as IsEnableResourceVersion
FROM
    CONTROLLER c
    LEFT OUTER JOIN VENDOR v ON v.vendor_id = c.vendor_id AND v.is_active = 1
    LEFT OUTER JOIN SYSTEM s ON s.system_id = c.system_id AND s.is_active = 1
    LEFT OUTER JOIN DATA_SCHEMA ds ON ds.data_schema_id = c.controller_schema_id AND ds.is_active = 1
WHERE
    c.url = /*ds url*/'https://' 
    AND c.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    c.controller_id as ControllerId
    ,c.url as RelativeUrl
    ,c.controller_description as ControllerDescription
    ,c.vendor_id as VendorId
    ,v.vendor_name as VendorName
    ,c.system_id as SystemId
    ,s.system_name as SystemName
    ,c.is_static_api as IsStaticApi
    ,c.is_vendor as IsVendor
    ,c.is_person as IsPerson
    ,c.is_enable as IsEnable
    ,c.is_toppage as IsTopPage
    ,c.controller_schema_id as ControllerSchemaId
    ,ds.schema_name as ControllerSchemaName
    ,c.controller_repository_key as RepositoryKey
    ,c.controller_partition_key as PartitionKey
    ,c.controller_name as ControllerName
    ,c.is_data as IsData
    ,c.is_businesslogic as IsBusinessLogic
    ,c.is_pay as IsPay
    ,c.fee_description as FeeDescription
    ,c.resource_create_user as ResourceCreateUser
    ,c.resource_maintainer as ResourceMaintainer
    ,c.resource_create_date as ResourceCreateDate
    ,c.resource_latest_date as ResourceLatestDate
    ,c.update_frequency as UpdateFrequency
    ,c.is_contract as IsContract
    ,c.contact_information as ContactInfomation
    ,c.version as Version
    ,c.agree_description as AgreeDescription
    ,c.is_visible_agreement as IsVisibleAgreement
    ,c.reg_date as RegDate
    ,c.reg_username as RegUserName
    ,c.upd_date as UpdDate
    ,c.upd_username as UpdUserName
    ,c.is_active as IsActive
    ,c.is_optimistic_concurrency as IsOptimisticConcurrency
    ,c.is_document_history as IsDocumentHistory
    ,c.is_enable_blockchain as IsEnableBlockchain
    ,c.is_enable_attachFile as IsEnableAttachFile
    ,c.is_enable_resource_version as IsEnableResourceVersion
FROM
    Controller c
    LEFT OUTER JOIN Vendor v ON v.vendor_id = c.vendor_id AND v.is_active = 1
    LEFT OUTER JOIN System s ON s.system_id = c.system_id AND s.is_active = 1
    LEFT OUTER JOIN DataSchema ds ON ds.data_schema_id = c.controller_schema_id AND ds.is_active = 1
WHERE
    c.url = @url
    AND c.is_active = 1
";
            }

            var param = new { url, };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            // Controllerを取得
            var controller = Connection.QuerySingle<ControllerQueryModel>(twowaySql.Sql, dynParams);

            controller.ControllerTagList = GetControllerTagList(controller.ControllerId);
            controller.ControllerCategoryList = GetContollerCategoryList(controller.ControllerId);
            controller.ControllerFieldList = GetControllerFieldList(controller.ControllerId);
            controller.ControllerCommonIpFilterGroupList = GetControllerCommonIpFilterGroupList(controller.ControllerId);
            controller.ControllerIpFilterList = GetControllerIpFilterList(controller.ControllerId);
            controller.ControllerOpenIdCAList = GetControllerOpenIdCAList(controller.ControllerId);
            controller.ApiList = GetApiList(controller.ControllerId, isTransparent);

            return controller;
        }

        /// <summary>
        /// コントローラーのURLを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラーのID</param>
        /// <returns>URL</returns>
        public string GetControllerUrl(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.url
FROM
    CONTROLLER c
WHERE
    c.controller_id = /*ds controllerId*/'1' 
AND 
    c.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    c.url
FROM
    Controller c
WHERE
    c.controller_id = @controllerId
AND 
    c.is_active = 1
";
            }
            var param = new { controllerId, };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// URLが一致するAPIが存在するか
        /// </summary>
        /// <param name="outApiId">APIのID</param>
        /// <param name="controllerId">ContorollerId</param>
        /// <param name="apiUrl">APIの相対URL</param>
        /// <returns>true:存在する/false:存在しない</returns>
        public bool IsDuplicateUrl(out string outApiId, string controllerId, string apiUrl)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            if (!string.IsNullOrEmpty(controllerId) && !string.IsNullOrEmpty(apiUrl))
            {
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT
    a.api_id AS ApiId
FROM
    API a
    INNER JOIN CONTROLLER c ON a.controller_id = c.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v ON c.vendor_id = v.vendor_id AND v.is_active = 1
    INNER JOIN SYSTEM s ON c.system_id = s.system_id AND s.is_active = 1
WHERE
    a.controller_id = /*ds controllerId*/'1' 
    AND a.url = /*ds apiUrl*/'https://' 
    AND a.is_active = 1
";
                }
                else
                {
                    sql = @"
SELECT
    a.api_id AS ApiId
FROM
    Api a
    INNER JOIN Controller c ON a.controller_id = c.controller_id AND c.is_active = 1
    INNER JOIN Vendor v ON c.vendor_id = v.vendor_id AND v.is_active = 1
    INNER JOIN System s ON c.system_id = s.system_id AND s.is_active = 1
WHERE
    a.controller_id = @controllerId
    AND a.url = @apiUrl
    AND a.is_active = 1
";
                }
                var param = new { controllerId, apiUrl, };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                var result = Connection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
                if (!string.IsNullOrEmpty(result))
                {
                    outApiId = result;
                    return true;
                }
            }

            outApiId = null;
            return false;
        }

        /// <summary>
        /// URL（相対）が一致するコントローラが存在するか
        /// </summary>
        /// <param name="url">コントローラURL</param>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>true:存在する/false:存在しない</returns>
        public bool IsDuplicateController(string url, out string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var duplicateControllerSql = "";
            if (dbSettings.Type == "Oracle")
            {
                duplicateControllerSql = @"
SELECT
    c.controller_id
FROM
    CONTROLLER c
    INNER JOIN VENDOR v ON c.vendor_id = v.vendor_id AND v.is_active = 1
    INNER JOIN SYSTEM s ON c.system_id = s.system_id AND s.is_active = 1
WHERE
    c.url = /*ds url*/'https://' 
AND c.is_active = 1
";
            }
            else
            {
                duplicateControllerSql = @"
SELECT
    c.controller_id
FROM
    Controller c
    INNER JOIN Vendor v ON c.vendor_id = v.vendor_id AND v.is_active = 1
    INNER JOIN System s ON c.system_id = s.system_id AND s.is_active = 1
WHERE
    c.url = @url
AND c.is_active = 1
";
            }
            var param = new { url, };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), duplicateControllerSql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            controllerId = Connection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
            return controllerId != null;
        }

        /// <summary>
        /// コントローラの存在確認をします。
        /// </summary>
        /// <param name="controllerId"></param>
        /// <param name="vendorId"></param>
        /// <returns>bool</returns>
        public bool IsExists(string controllerId, string vendorId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(controller_id) AS Count
FROM
    CONTROLLER
WHERE
    controller_id = /*ds controller_id*/'00000000-0000-0000-0000-000000000000' AND
/*ds if vendor_id != null*/
    vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(controller_id) AS Count
FROM
    Controller
WHERE
    controller_id = @controller_id AND
/*ds if vendor_id != null*/
    vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    is_active = 1
";
            }
            var param = new { controller_id = controllerId, vendor_id = vendorId, };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return (Connection.QuerySingle<int>(twowaySql.Sql, dynParams) > 0);
        }

        /// <summary>
        /// コントローラを登録または更新します。
        /// </summary>
        /// <param name="model">コントローラモデル</param>
        /// <returns>コントローラモデル</returns>
        [CacheIdFire("ControllerId", "model.ControllerId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_CONTROLLER)]
        [DomainDataSync(ParameterType.Result, DynamicApiDatabase.TABLE_CONTROLLER, "ControllerId")]
        public ControllerInformationModel UpsertController(ControllerInformationModel model)
        {
            var param = new
            {
                model.ControllerId,
                model.Url,
                model.ControllerDescription,
                model.VendorId,
                model.SystemId,
                model.IsStaticApi,
                model.IsVendor,
                model.IsEnable,
                model.IsTopPage,
                model.ControllerSchemaId,
                model.RepositoryKey,
                model.PartitionKey,
                model.ControllerName,
                model.IsData,
                model.IsBusinessLogic,
                model.IsPay,
                model.FeeDescription,
                model.ResourceCreateUser,
                model.ResourceMaintainer,
                ResourceCreateDate = model.ResourceCreateDate?.ToDateTime(),
                ResourceLatestDate = model.ResourceLatestDate?.ToDateTime(),
                model.UpdateFrequency,
                model.IsContract,
                model.ContactInformation,
                model.Version,
                model.AgreeDescription,
                RegDate = UtcNow,
                RegUsername = PerRequestDataContainer.OpenId,
                UpdDate = UtcNow,
                UpdUsername = PerRequestDataContainer.OpenId,
                model.IsActive,
                model.IsPerson,
                IsEnableAttachfile = model.AttachFileSettings?.IsEnable ?? false,
                model.IsEnableIpFilter,
                IsDocumentHistory = model.DocumentHistorySettings?.IsEnable ?? false,
                model.IsEnableBlockchain,
                model.IsOptimisticConcurrency,
                model.IsUseBlobCache,
                model.IsVisibleAgreement,
                model.IsContainerDynamicSeparation,
                model.IsEnableResourceVersion
            };

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO CONTROLLER target
    USING (SELECT /*ds ControllerId*/'aa' AS controller_id FROM DUAL) source
ON (target.controller_id = source.controller_id)
WHEN MATCHED THEN
    UPDATE
    SET
        url = /*ds Url*/'www' 
        ,controller_description = /*ds ControllerDescription*/'aa' 
        ,vendor_id = /*ds VendorId*/'00000000-0000-0000-0000-000000000000' 
        ,system_id = /*ds SystemId*/'00000000-0000-0000-0000-000000000000' 
        ,is_static_api = /*ds IsStaticApi*/1 
        ,is_vendor = /*ds IsVendor*/1 
        ,is_enable = /*ds IsEnable*/1 
        -- 更新対象外 ,is_toppage = /*ds IsTopPage*/1 
        ,controller_schema_id = /*ds ControllerSchemaId*/'00000000-0000-0000-0000-000000000000' 
        ,controller_repository_key = /*ds RepositoryKey*/'00000000-0000-0000-0000-000000000000' 
        ,controller_partition_key = /*ds PartitionKey*/'00000000-0000-0000-0000-000000000000' 
        ,controller_name = /*ds ControllerName*/'aa' 
        ,is_data = /*ds IsData*/1 
        ,is_businesslogic = /*ds IsBusinessLogic*/1 
        ,is_pay = /*ds IsPay*/1 
        ,fee_description = /*ds FeeDescription*/'aa' 
        ,resource_create_user = /*ds ResourceCreateUser*/'aa' 
        ,resource_maintainer = /*ds ResourceMaintainer*/'aa' 
        ,resource_create_date = /*ds ResourceCreateDate*/'00-01-01' 
        ,resource_latest_date = /*ds ResourceLatestDate*/'00-01-01' 
        ,update_frequency = /*ds UpdateFrequency*/'a' 
        ,is_contract = /*ds IsContract*/1 
        ,contact_information = /*ds ContactInformation*/'aa' 
        ,version = /*ds Version*/'aa' 
        ,agree_description = /*ds AgreeDescription*/'aa' 
        ,upd_date = /*ds UpdDate*/'00-01-01' 
        ,upd_username = /*ds UpdUserName*/'aa' 
        ,is_person = /*ds IsPerson*/1 
        ,is_enable_attachfile = /*ds IsEnableAttachfile*/1 
        ,is_enable_ipfilter = /*ds IsEnableIpFilter*/1 
        ,is_document_history = /*ds IsDocumentHistory*/1 
        ,is_enable_blockchain = /*ds IsEnableBlockchain*/1 
        ,is_optimistic_concurrency = /*ds IsOptimisticConcurrency*/1 
        ,is_use_blob_cache = /*ds IsUseBlobCache*/1 
        ,is_visible_agreement = /*ds IsVisibleAgreement*/1 
        -- 更新対象外 ,is_container_dynamic_separation = /*ds IsContainerDynamicSeparation*/1 
        ,is_enable_resource_version = /*ds IsEnableResourceVersion*/1 
WHEN NOT MATCHED THEN
    INSERT
    (
        controller_id
        ,url
        ,controller_description
        ,vendor_id
        ,system_id
        ,is_static_api
        ,is_vendor
        ,is_enable
        -- 登録対象外 ,is_toppage
        ,controller_schema_id
        ,controller_repository_key
        ,controller_partition_key
        ,controller_name
        ,is_data
        ,is_businesslogic
        ,is_pay
        ,fee_description
        ,resource_create_user
        ,resource_maintainer
        ,resource_create_date
        ,resource_latest_date
        ,update_frequency
        ,is_contract
        ,contact_information
        ,version
        ,agree_description
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
        ,is_person
        ,is_enable_attachfile
        ,is_enable_ipfilter
        ,is_document_history
        ,is_enable_blockchain
        ,is_optimistic_concurrency
        ,is_use_blob_cache
        ,is_visible_agreement
        ,is_container_dynamic_separation
        ,is_enable_resource_version
    )
    VALUES
    (
        /*ds ControllerId*/'aa' 
        ,/*ds Url*/'www' 
        ,/*ds ControllerDescription*/'aa' 
        ,/*ds VendorId*/'00000000-0000-0000-0000-000000000000' 
        ,/*ds SystemId*/'00000000-0000-0000-0000-000000000000' 
        ,/*ds IsStaticApi*/1 
        ,/*ds IsVendor*/1 
        ,/*ds IsEnable*/1 
        -- 登録対象外 ,/*ds IsTopPage*/1 
        ,/*ds ControllerSchemaId*/'00000000-0000-0000-0000-000000000000' 
        ,/*ds RepositoryKey*/'00000000-0000-0000-0000-000000000000' 
        ,/*ds PartitionKey*/'00000000-0000-0000-0000-000000000000' 
        ,/*ds ControllerName*/'aa' 
        ,/*ds IsData*/1 
        ,/*ds IsBusinessLogic*/1 
        ,/*ds IsPay*/1 
        ,/*ds FeeDescription*/'aa' 
        ,/*ds ResourceCreateUser*/'aa' 
        ,/*ds ResourceMaintainer*/'aa' 
        ,/*ds ResourceCreateDate*/'00-01-01' 
        ,/*ds ResourceLatestDate*/'00-01-01' 
        ,/*ds UpdateFrequency*/'a' 
        ,/*ds IsContract*/1 
        ,/*ds ContactInformation*/'aa' 
        ,/*ds Version*/'aa' 
        ,/*ds AgreeDescription*/'aa' 
        ,/*ds RegDate*/'00-01-01' 
        ,/*ds RegUsername*/'aa' 
        ,/*ds UpdDate*/'00-01-01' 
        ,/*ds UpdUsername*/'aa' 
        ,1 --IsActive
        ,/*ds IsPerson*/1 
        ,/*ds IsEnableAttachfile*/1 
        ,/*ds IsEnableIpFilter*/1 
        ,/*ds IsDocumentHistory*/1 
        ,/*ds IsEnableBlockchain*/1 
        ,/*ds IsOptimisticConcurrency*/1 
        ,/*ds IsUseBlobCache*/1 
        ,/*ds IsVisibleAgreement*/1 
        ,/*ds IsContainerDynamicSeparation*/1 
        ,/*ds IsEnableResourceVersion*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO Controller AS target
USING
(
    SELECT
        @ControllerId AS controller_id
) AS source
ON
    target.controller_id = source.controller_id
WHEN MATCHED THEN
    UPDATE
    SET
        url = @Url
        ,controller_description = @ControllerDescription
        ,vendor_id = @VendorId
        ,system_id = @SystemId
        ,is_static_api = @IsStaticApi
        ,is_vendor = @IsVendor
        ,is_enable = @IsEnable
        -- 更新対象外 ,is_toppage = @IsTopPage
        ,controller_schema_id = @ControllerSchemaId
        ,controller_repository_key = @RepositoryKey
        ,controller_partition_key = @PartitionKey
        ,controller_name = @ControllerName
        ,is_data = @IsData
        ,is_businesslogic = @IsBusinessLogic
        ,is_pay = @IsPay
        ,fee_description = @FeeDescription
        ,resource_create_user = @ResourceCreateUser
        ,resource_maintainer = @ResourceMaintainer
        ,resource_create_date = @ResourceCreateDate
        ,resource_latest_date = @ResourceLatestDate
        ,update_frequency = @UpdateFrequency
        ,is_contract = @IsContract
        ,contact_information = @ContactInformation
        ,version = @Version
        ,agree_description = @AgreeDescription
        ,upd_date = @UpdDate
        ,upd_username = @UpdUserName
        ,is_person = @IsPerson
        ,is_enable_attachfile = @IsEnableAttachfile
        ,is_enable_ipfilter = @IsEnableIpFilter
        ,is_document_history = @IsDocumentHistory
        ,is_enable_blockchain = @IsEnableBlockchain
        ,is_optimistic_concurrency = @IsOptimisticConcurrency
        ,is_use_blob_cache = @IsUseBlobCache
        ,is_visible_agreement = @IsVisibleAgreement
        -- 更新対象外 ,is_container_dynamic_separation = @IsContainerDynamicSeparation
        ,is_enable_resource_version = @IsEnableResourceVersion
WHEN NOT MATCHED THEN
    INSERT
    (
        controller_id
        ,url
        ,controller_description
        ,vendor_id
        ,system_id
        ,is_static_api
        ,is_vendor
        ,is_enable
        -- 登録対象外 ,is_toppage
        ,controller_schema_id
        ,controller_repository_key
        ,controller_partition_key
        ,controller_name
        ,is_data
        ,is_businesslogic
        ,is_pay
        ,fee_description
        ,resource_create_user
        ,resource_maintainer
        ,resource_create_date
        ,resource_latest_date
        ,update_frequency
        ,is_contract
        ,contact_information
        ,version
        ,agree_description
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
        ,is_person
        ,is_enable_attachfile
        ,is_enable_ipfilter
        ,is_document_history
        ,is_enable_blockchain
        ,is_optimistic_concurrency
        ,is_use_blob_cache
        ,is_visible_agreement
        ,is_container_dynamic_separation
        ,is_enable_resource_version
    )
    VALUES
    (
        @ControllerId
        ,@Url
        ,@ControllerDescription
        ,@VendorId
        ,@SystemId
        ,@IsStaticApi
        ,@IsVendor
        ,@IsEnable
        -- 登録対象外 ,@IsTopPage
        ,@ControllerSchemaId
        ,@RepositoryKey
        ,@PartitionKey
        ,@ControllerName
        ,@IsData
        ,@IsBusinessLogic
        ,@IsPay
        ,@FeeDescription
        ,@ResourceCreateUser
        ,@ResourceMaintainer
        ,@ResourceCreateDate
        ,@ResourceLatestDate
        ,@UpdateFrequency
        ,@IsContract
        ,@ContactInformation
        ,@Version
        ,@AgreeDescription
        ,@RegDate
        ,@RegUsername
        ,@UpdDate
        ,@UpdUsername
        ,1 --IsActive
        ,@IsPerson
        ,@IsEnableAttachfile
        ,@IsEnableIpFilter
        ,@IsDocumentHistory
        ,@IsEnableBlockchain
        ,@IsOptimisticConcurrency
        ,@IsUseBlobCache
        ,@IsVisibleAgreement
        ,@IsContainerDynamicSeparation
        ,@IsEnableResourceVersion
    );
";
            }
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParam = dbSettings.GetParameters().AddDynamicParams(param)
                .SetNClob(nameof(param.AgreeDescription))
                .SetNClob(nameof(param.ContactInformation))
                .SetNClob(nameof(param.ControllerDescription))
                .SetNClob(nameof(param.ControllerName))
                .SetNClob(nameof(param.FeeDescription))
                .SetNClob(nameof(param.ResourceCreateUser))
                .SetNClob(nameof(param.ResourceMaintainer))
                .SetNClob(nameof(param.UpdateFrequency))
                .SetNClob(nameof(param.Version));
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParam);
            return model;
        }

        /// <summary>
        /// コントローラを削除します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        [CacheIdFire("ControllerId", "controllerId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_CONTROLLER)]
        [DomainDataSync(DynamicApiDatabase.TABLE_CONTROLLER, "controllerId")]
        public void DeleteController(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string delSql;
            if (dbSettings.Type == "Oracle")
            {
                delSql = @"
UPDATE CONTROLLER
    SET 
        is_active = 0
        ,upd_date = /*ds UpdDate*/'2000-01-01' 
        ,upd_username = /*ds UpdUserName*/'scott' 
WHERE
    controller_id = /*ds controllerId*/'1' 
    AND is_active = 1
";
            }
            else
            {
                delSql = @"
UPDATE Controller
    SET 
        is_active = 0
        ,upd_date = @UpdDate
        ,upd_username = @UpdUserName
WHERE
    controller_id = @controllerId
    AND is_active = 1
";
            }
            var param = new
            {
                controllerId,
                UpdDate = UtcNow,
                UpdUserName = PerRequestDataContainer.OpenId
            };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), delSql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// URLに紐づくコントローラを削除します。
        /// </summary>
        /// <param name="url">URL</param>
        [CacheIdFire("Url", "url")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_CONTROLLER)]
        [DomainDataSync(DynamicApiDatabase.TABLE_CONTROLLER, "controllerId")]
        public void DeleteControllerFromUrl(string url, string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string delSql;
            if (dbSettings.Type == "Oracle")
            {
                delSql = @"
UPDATE CONTROLLER
    SET 
        is_active = 0
        ,upd_date = /*ds UpdDate*/'2000-01-01' 
        ,upd_username = /*ds UpdUserName*/'scott' 
WHERE
    url = /*ds url*/'https://' 
    AND is_active = 1
";
            }
            else
            {
                delSql = @"
UPDATE Controller
    SET 
        is_active = 0
        ,upd_date = @UpdDate
        ,upd_username = @UpdUserName
WHERE
    url = @url
    AND is_active = 1
";
            }
            var param = new
            {
                url,
                UpdDate = UtcNow,
                UpdUserName = PerRequestDataContainer.OpenId
            };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), delSql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        #endregion

        #region ControllerCategory
        /// <summary>
        /// ControllerIdに紐づくContollerCategoryリストを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>ContollerCategoryリスト</returns>
        public IEnumerable<ControllerCategoryInfomationModel> GetContollerCategoryList(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    cc.controller_category_id as ControllerCategoryId
    ,cc.category_id as CategoryId
    ,c.category_name as CategoryName
    ,cc.is_active AS IsActive
FROM
    CONTROLLER_CATEGORY cc
    INNER JOIN CATEGORY c ON c.category_id = cc.category_id AND c.is_active = 1
WHERE
    cc.controller_id = /*ds controllerId*/'1' 
    AND cc.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    cc.controller_category_id as ControllerCategoryId
    ,cc.category_id as CategoryId
    ,c.category_name as CategoryName
    ,cc.is_active AS IsActive
FROM
    ControllerCategory cc
    INNER JOIN Category c ON c.category_id = cc.category_id AND c.is_active = 1
WHERE
    cc.controller_id = @controllerId
    AND cc.is_active = 1
";
            }

            var param = new { controllerId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ControllerCategoryInfomationModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// ControllerIdに紐づくContollerCategoryリストを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>ContollerCategoryリスト</returns>
        public IList<ControllerCategoryInfomationModel> GetContollerCategoryListContainControllerIdNull(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    cc.controller_category_id AS ControllerCategoryId
    ,cc.controller_id AS ControllerId
    ,c.category_id AS CategoryId
    ,c.category_name AS CategoryName
    ,cc.is_active AS IsActive
FROM
    CATEGORY c
    LEFT OUTER JOIN CONTROLLER_CATEGORY cc ON c.category_id = cc.category_id AND cc.controller_id = /*ds controllerId*/'1' AND c.is_active = 1 
WHERE
    c.is_active = 1
ORDER BY 
    c.sort_order
";
            }
            else
            {
                sql = @"
SELECT
    cc.controller_category_id AS ControllerCategoryId
    ,cc.controller_id AS ControllerId
    ,c.category_id AS CategoryId
    ,c.category_name AS CategoryName
    ,cc.is_active AS IsActive
FROM
    Category c
    LEFT OUTER JOIN ControllerCategory cc ON c.category_id = cc.category_id AND cc.controller_id = @controllerId AND c.is_active = 1 
WHERE
    c.is_active = 1
ORDER BY 
    c.sort_order
";
            }

            var param = new { controllerId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ControllerCategoryInfomationModel>(twowaySql.Sql, dynParams).ToList();
        }

        /// <summary>
        /// VendorIdに紐づくControllerのCategoryを取得します。
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのCategoryを取得する</param>
        /// <returns>カテゴリー</returns>
        private Dictionary<string, IEnumerable<ControllerCategoryInfomationModel>> GetContollerCategory(string vendorId, bool isAll)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    cc.controller_id as ControllerId
    ,cc.controller_category_id as ControllerCategoryId
    ,cc.category_id as CategoryId
    ,ca.category_name as CategoryName
FROM
    CONTROLLER_CATEGORY cc /*WITH(NOLOCK)*/
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = cc.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN CATEGORY ca /*WITH(NOLOCK)*/ ON ca.category_id = cc.category_id AND ca.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendorId*/'1' AND
/*ds end if*/
    cc.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    cc.controller_id as ControllerId
    ,cc.controller_category_id as ControllerCategoryId
    ,cc.category_id as CategoryId
    ,ca.category_name as CategoryName
FROM
    ControllerCategory cc WITH(NOLOCK)
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = cc.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN Category ca WITH(NOLOCK) ON ca.category_id = cc.category_id AND ca.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendorId AND
/*ds end if*/
    cc.is_active = 1
";
            }

            var param = new { IsAll = isAll ? null : "isAll", vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(new { vendorId });
            var result = Connection.Query<string, ControllerCategoryInfomationModel, (string ControllerId, ControllerCategoryInfomationModel Category)>(
                sql: twowaySql.Sql,
                splitOn: "ControllerCategoryId",
                map: (controllerId, category) => (controllerId, category),
                param: dynParams);

            return result.GroupBy(x => x.ControllerId).ToDictionary(x => x.Key, y => y.Select(z => z.Category));
        }

        /// <summary>
        /// コントローラカテゴリーリストを登録または更新します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <param name="controllerCategoryInfoList">コントローラカテゴリーリスト</param>
        /// <returns>コントローラカテゴリーリストモデル</returns>
        public IList<ControllerCategoryInfomationModel> UpsertControllerCategoryList(string controllerId, IList<ControllerCategoryInfomationModel> controllerCategoryInfoList)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var updsql = "";
            // 一旦全て無効にする
            if (dbSettings.Type == "Oracle")
            {
                updsql = "UPDATE CONTROLLER_CATEGORY SET is_active = 0 WHERE controller_id = /*ds controllerId*/'1' ";
            }
            else
            {
                updsql = "UPDATE ControllerCategory SET is_active = 0 WHERE controller_id = @controllerId";
            }
            var upd_twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), updsql, new { controllerId });

            Connection.Execute(upd_twowaySql.Sql, new { controllerId });

            List<object> paramList = new();
            foreach (var categoryInfo in controllerCategoryInfoList)
            {
                // 新規IDを発番する
                if (string.IsNullOrEmpty(categoryInfo.ControllerCategoryId)) categoryInfo.ControllerCategoryId = Guid.NewGuid().ToString();

                var param = new
                {
                    controller_category_id = categoryInfo.ControllerCategoryId,
                    controller_id = controllerId,
                    category_id = categoryInfo.CategoryId,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = categoryInfo.IsActive
                };
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                paramList.Add(dynParams);
            }

            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO CONTROLLER_CATEGORY target
USING
(
    SELECT
        /*ds category_id*/'id' AS category_id,
        /*ds controller_id*/'id' AS controller_id
    FROM DUAL
) source
ON (
    target.category_id = source.category_id AND target.controller_id = source.controller_id
)
WHEN MATCHED THEN
    UPDATE
    SET
        is_active = /*ds is_active*/1 
        ,upd_date = /*ds upd_date*/'2000-01-01' 
        ,upd_username = /*ds upd_username*/'name' 
WHEN NOT MATCHED THEN
    INSERT
    (
        controller_category_id
        ,controller_id
        ,category_id
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        /*ds controller_category_id*/'id' 
        ,/*ds controller_id*/'id' 
        ,/*ds category_id*/'id' 
        ,/*ds reg_date*/'2000-01-01' 
        ,/*ds reg_username*/'name' 
        ,/*ds upd_date*/'2000-01-01' 
        ,/*ds upd_username*/'name' 
        ,/*ds is_active*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO ControllerCategory AS target
USING
(
    SELECT
        @category_id AS category_id,
        @controller_id AS controller_id
) AS source
ON
    target.category_id = source.category_id AND target.controller_id = source.controller_id
WHEN MATCHED THEN
    UPDATE
    SET
        is_active = @is_active
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        controller_category_id
        ,controller_id
        ,category_id
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        @controller_category_id
        ,@controller_id
        ,@category_id
        ,@reg_date
        ,@reg_username
        ,@upd_date
        ,@upd_username
        ,@is_active
    );
";            }

            var twowayParam = new {
                category_id = true,
                controller_id = true,
                is_active =true,
                upd_date = true,
                controller_category_id = true,
                reg_date = true,
                reg_username = true,
                upd_username = true,
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowayParam);
            Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            return controllerCategoryInfoList;
        }

        #endregion

        #region ControllerCommonIpFilterGroup
        /// <summary>
        /// CommonIpFilterGroupId に紐づくControllerCommonIpFilterGroupが存在するか
        /// </summary>
        /// <param name="commonIpFilterGroupId">コモンIPフィルターグループID</param>
        /// <returns>true:存在する/false:存在しない</returns>
        private bool ExistsControllerCommonIpFilterGroup(string controllerId, string commonIpFilterGroupId)
        {
            if (string.IsNullOrEmpty(commonIpFilterGroupId))
                throw new ArgumentNullException(nameof(commonIpFilterGroupId));

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(cci.common_ip_filter_group_id)
FROM
    CONTROLLER_COMMON_IP_FILTER_GROUP cci
    INNER JOIN COMMON_IP_FILTER_GROUP ci ON ci.common_ip_filter_group_id = cci.common_ip_filter_group_id AND ci.is_active = 1
WHERE
    cci.common_ip_filter_group_id = /*ds commonIpFilterGroupId*/'id' 
    and cci.controller_id = /*ds controllerId*/'00000000-0000-0000-0000-000000000000' 
    and cci.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(cci.common_ip_filter_group_id)
FROM
    ControllerCommonIpFilterGroup cci
    INNER JOIN CommonIpFilterGroup ci ON ci.common_ip_filter_group_id = cci.common_ip_filter_group_id AND ci.is_active = 1
WHERE
    cci.common_ip_filter_group_id = @commonIpFilterGroupId
    and cci.controller_id = @controllerId
    and cci.is_active = 1
";
            }

            var param = new { controllerId, commonIpFilterGroupId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            return result == 1;
        }

        /// <summary>
        /// ControllerIdに紐づくCommonIpFilterGroupリストを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>CommonIpFilterGroupリスト</returns>
        public IEnumerable<ControllerCommonIpFilterGroupModel> GetControllerCommonIpFilterGroupList(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    cci.common_ip_filter_group_id as CommonIpFilterGroupId
    ,ci.common_ip_filter_group_name as CommonIpFilterGroupName
    ,cci.controller_id AS ControllerId
    ,cci.is_active AS IsActive
FROM
    CONTROLLER_COMMON_IP_FILTER_GROUP cci
    INNER JOIN COMMON_IP_FILTER_GROUP ci ON ci.common_ip_filter_group_id = cci.common_ip_filter_group_id AND ci.is_active = 1
WHERE
    cci.controller_id = /*ds controllerId*/'id' 
    AND cci.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    cci.common_ip_filter_group_id as CommonIpFilterGroupId
    ,ci.common_ip_filter_group_name as CommonIpFilterGroupName
    ,cci.controller_id AS ControllerId
    ,cci.is_active AS IsActive
FROM
    ControllerCommonIpFilterGroup cci
    INNER JOIN CommonIpFilterGroup ci ON ci.common_ip_filter_group_id = cci.common_ip_filter_group_id AND ci.is_active = 1
WHERE
    cci.controller_id = @controllerId
    AND cci.is_active = 1
";
            }
            var param = new { controllerId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ControllerCommonIpFilterGroupModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// ControllerIdに紐づくCommonIpFilterGroupリストを取得します。
        /// （コントローラIDがNULLのものも含む）
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>CommonIpFilterGroupリスト</returns>
        public IEnumerable<ControllerCommonIpFilterGroupModel> GetControllerCommonIpFilterGroupListContainControllerIdNull(string controllerId)
        {
            // CommonIpFilterGroupとControllerCommonIpFilterGroupとLEFT JOINして全てのCommonIpFilterGroupを取得(Controller新規登録時はControllerIdがNULL)
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    com.common_ip_filter_group_id AS CommonIpFilterGroupId
    ,com.common_ip_filter_group_name AS CommonIpFilterGroupName
    ,cont.controller_id AS ControllerId
    ,cont.is_active AS IsActive
FROM
    COMMON_IP_FILTER_GROUP com
    LEFT OUTER JOIN CONTROLLER_COMMON_IP_FILTER_GROUP cont ON com.common_ip_filter_group_id = cont.common_ip_filter_group_id AND cont.controller_id = /*ds controllerId*/'id' 
WHERE
    com.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    com.common_ip_filter_group_id AS CommonIpFilterGroupId
    ,com.common_ip_filter_group_name AS CommonIpFilterGroupName
    ,cont.controller_id AS ControllerId
    ,cont.is_active AS IsActive
FROM
    CommonIpFilterGroup com
    LEFT OUTER JOIN ControllerCommonIpFilterGroup cont ON com.common_ip_filter_group_id = cont.common_ip_filter_group_id AND cont.controller_id = @controllerId
WHERE
    com.is_active = 1
";
            }

            var param = new { controllerId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ControllerCommonIpFilterGroupModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// VendorIdに紐づくControllerCommonIpFilterGroupを取得します。
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのCommonIpFilterGroupを取得するか</param>
        /// <returns>IPフィルタグループ</returns>
        private Dictionary<string, IEnumerable<ControllerCommonIpFilterGroupModel>> GetControllerCommonIpFilterGroupByVendorId(string vendorId, bool isAll)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    cci.controller_id as ControllerId
    ,cci.common_ip_filter_group_id as CommonIpFilterGroupId
    ,ci.common_ip_filter_group_name as CommonIpFilterGroupName
FROM
    CONTROLLER_COMMON_IP_FILTER_GROUP cci /*WITH(NOLOCK)*/
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = cci.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN COMMON_IP_FILTER_GROUP ci /*WITH(NOLOCK)*/ ON ci.common_ip_filter_group_id = cci.common_ip_filter_group_id AND ci.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendorId*/'id' AND
/*ds end if*/
    cci.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    cci.controller_id as ControllerId
    ,cci.common_ip_filter_group_id as CommonIpFilterGroupId
    ,ci.common_ip_filter_group_name as CommonIpFilterGroupName
FROM
    ControllerCommonIpFilterGroup cci WITH(NOLOCK)
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = cci.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN CommonIpFilterGroup ci WITH(NOLOCK) ON ci.common_ip_filter_group_id = cci.common_ip_filter_group_id AND ci.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendorId AND
/*ds end if*/
    cci.is_active = 1
";
            }
            var param = new { vendorId };
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendorId", vendorId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<string, ControllerCommonIpFilterGroupModel, (string ControllerId, ControllerCommonIpFilterGroupModel CommonIpFilterGroup)>(
                sql: twowaySql.Sql,
                splitOn: "CommonIpFilterGroupId",
                map: (controllerId, commonIpFilterGroup) => (controllerId, commonIpFilterGroup),
                param: dynParams);

            return result.GroupBy(x => x.ControllerId).ToDictionary(x => x.Key, y => y.Select(z => z.CommonIpFilterGroup));
        }

        /// <summary>
        /// ControllerCommonIpFilterGroupを一括登録または更新します。
        /// </summary>
        /// <param name="modelList">ControllerCommonIpFilterGroupModel</param>
        /// <param name="controllerId">コントローラID</param>
        /// <returns></returns>
        public List<ControllerCommonIpFilterGroupModel> UpsertControllerCommonIpFilterGroupList(List<ControllerCommonIpFilterGroupModel> modelList, string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO CONTROLLER_COMMON_IP_FILTER_GROUP target
    USING
    (
        SELECT
            /*ds common_ip_filter_group_id*/'id' AS common_ip_filter_group_id
            ,/*ds controller_id*/'id' AS controller_id
        FROM DUAL
    ) source
    ON (
        target.common_ip_filter_group_id = source.common_ip_filter_group_id AND
        target.controller_id = source.controller_id
    )
    WHEN MATCHED THEN
        UPDATE
        SET
            is_active = /*ds is_active*/1 
            ,upd_date = /*ds upd_date*/'2000-01-01' 
            ,upd_username = /*ds upd_username*/'name' 
    WHEN NOT MATCHED THEN
        INSERT
        (
            controller_id
            ,common_ip_filter_group_id
            ,reg_date
            ,reg_username
            ,upd_date
            ,upd_username
            ,is_active
        )
        VALUES
        (
            /*ds controller_id*/'id' 
            ,/*ds common_ip_filter_group_id*/'id' 
            ,/*ds reg_date*/'2000-01-01' 
            ,/*ds reg_username*/'name' 
            ,/*ds upd_date*/'2000-01-01' 
            ,/*ds upd_username*/'name' 
            ,/*ds is_active*/1 
        )
";
            }
            else
            {
                sql = @"
MERGE INTO ControllerCommonIpFilterGroup AS target
    USING
    (
        SELECT
            @common_ip_filter_group_id AS common_ip_filter_group_id
            ,@controller_id AS controller_id
    ) AS source
    ON
        target.common_ip_filter_group_id = source.common_ip_filter_group_id AND
        target.controller_id = source.controller_id 
    WHEN MATCHED THEN
        UPDATE
        SET
            is_active = @is_active
            ,upd_date = @upd_date
            ,upd_username = @upd_username
    WHEN NOT MATCHED THEN
        INSERT
        (
            controller_id
            ,common_ip_filter_group_id
            ,reg_date
            ,reg_username
            ,upd_date
            ,upd_username
            ,is_active
        )
        VALUES
        (
            @controller_id
            ,@common_ip_filter_group_id
            ,@reg_date
            ,@reg_username
            ,@upd_date
            ,@upd_username
            ,@is_active
        );
";
            }
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                controller_id = true,
                common_ip_filter_group_id = true,
                reg_date = true,
                reg_username = true,
                upd_date = true,
                upd_username = true,
                is_active = true,
            });

            List<object> paramList = new();
            foreach (var model in modelList)
            {
                // 新規登録時はControllerIdを受け取る
                if (string.IsNullOrEmpty(model.ControllerId)) model.ControllerId = controllerId;

                // 新規IDを発番する
                if (string.IsNullOrEmpty(model.CommonIpFilterGroupId)) model.CommonIpFilterGroupId = Guid.NewGuid().ToString();

                // 登録値がfalseかつDB未登録の場合は処理しない
                if (!model.IsActive && !ExistsControllerCommonIpFilterGroup(model.ControllerId, model.CommonIpFilterGroupId))
                {
                    continue;
                }

                var param = new
                {
                    controller_id = model.ControllerId,
                    common_ip_filter_group_id = model.CommonIpFilterGroupId,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = model.IsActive
                };
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                paramList.Add(dynParams);
            }

            Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            return modelList;
        }

        #endregion

        #region ControllerIpFilter

        /// <summary>
        /// ControllerIpFilterが存在するか
        /// </summary>
        /// <param name="ipAddress">IPアドレス</param>
        /// <returns>true:存在する/false:存在しない</returns>
        private bool ExistsControllerIpFilter(string ipAddress)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    COUNT(controller_ip_filter_id)
FROM 
    CONTROLLER_IP_FILTER
WHERE
    ip_address = /*ds ipAddress*/'ip'
";
            }
            else
            {
                sql = @"
SELECT 
    COUNT(controller_ip_filter_id)
FROM 
    ControllerIpFilter
WHERE
    ip_address = @ipAddress
"
;
            }

            var param = new { ipAddress };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            return result > 1;
        }

        /// <summary>
        /// ControllerIdに紐づくIpFilterを取得します。
        /// </summary>
        /// <param name="controllerId">ControllerId</param>
        /// <returns>IPフィルタ</returns>
        private IList<ControllerIpFilterModel> GetControllerIpFilterList(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    ci.controller_ip_filter_id as ControllerIpFilterId
    ,ci.controller_id as ControllerId
    ,ci.ip_address as IpAddress
    ,ci.is_enable as IsEnable
    ,ci.is_active as IsActive
FROM
    CONTROLLER_IP_FILTER ci
WHERE
/*ds if controllerId != null*/
    ci.controller_id = /*ds controllerId*/'00000000-0000-0000-0000-000000000000' AND 
/*ds end if*/
    ci.is_active = 1
ORDER BY
    ci.ip_address
";
            }
            else
            {
                sql = @"
SELECT
    ci.controller_ip_filter_id as ControllerIpFilterId
    ,ci.controller_id as ControllerId
    ,ci.ip_address as IpAddress
    ,ci.is_enable as IsEnable
    ,ci.is_active as IsActive
FROM
    ControllerIpFilter ci
WHERE
/*ds if controllerId != null*/
    ci.controller_id = /*ds controllerId*/'00000000-0000-0000-0000-000000000000' AND 
/*ds end if*/
    ci.is_active = 1
ORDER BY
    ci.ip_address
";
            }

            var param = new { controllerId };
            var twowayIpFilterSql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ControllerIpFilterModel>(twowayIpFilterSql.Sql, dynParams).ToList();
        }

        /// <summary>
        /// VendorIdに紐づくControllerのIpFilterを取得します。
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのIpFilterを取得する</param>
        /// <returns>IPフィルタ</returns>
        private Dictionary<string, IEnumerable<ControllerIpFilterModel>> GetControllerIpFilterByVendorId(string vendorId, bool isAll)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    ci.controller_id as ControllerId
    ,ci.controller_ip_filter_id as ControllerIdFilterId
    ,ci.ip_address as IpAddress
    ,ci.is_enable as IsEnable
FROM
    CONTROLLER_IP_FILTER ci /*WITH(NOLOCK)*/
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = ci.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendorId*/'id' AND
/*ds end if*/
    ci.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    ci.controller_id as ControllerId
    ,ci.controller_ip_filter_id as ControllerIdFilterId
    ,ci.ip_address as IpAddress
    ,ci.is_enable as IsEnable
FROM
    ControllerIpFilter ci WITH(NOLOCK)
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = ci.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendorId AND
/*ds end if*/
    ci.is_active = 1
";
            }
            var param = new { vendorId };
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendorId", vendorId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<string, ControllerIpFilterModel, (string ControllerId, ControllerIpFilterModel IpFilter)>(
                sql: twowaySql.Sql,
                splitOn: "ControllerIdFilterId",
                map: (controllerId, ipFilterGroup) => (controllerId, ipFilterGroup),
                param: dynParams);

            return result.GroupBy(x => x.ControllerId).ToDictionary(x => x.Key, y => y.Select(z => z.IpFilter));
        }

        /// <summary>
        /// ControllerIpFilterを登録または更新します。
        /// </summary>
        /// <param name="modelList">ControllerIpFilterModel</param>
        /// <param name="controllerId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public List<ControllerIpFilterModel> UpsertControllerIpFilter(List<ControllerIpFilterModel> modelList, string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO CONTROLLER_IP_FILTER target
    USING
    (
        SELECT
            /*ds controller_id*/'id' AS controller_id
            ,/*ds ip_address*/'ip' AS ip_address
        FROM dual
    ) source
    ON (
        target.controller_id = source.controller_id AND target.ip_address = source.ip_address
    )
    WHEN MATCHED THEN
        UPDATE
        SET
            is_enable = CASE WHEN /*ds is_active*/1 = 1 THEN /*ds is_enable*/1 ELSE is_enable END,
            is_active = /*ds is_active*/1 
            ,upd_date = /*ds upd_date*/'2000-01-01' 
            ,upd_username = /*ds upd_username*/'name' 
    WHEN NOT MATCHED THEN
        INSERT
        (
            controller_ip_filter_id
            ,controller_id
            ,ip_address
            ,is_enable
            ,reg_date
            ,reg_username
            ,upd_date
            ,upd_username
            ,is_active
        )
        VALUES
        (
            NEWID()
            ,/*ds controller_id*/'id' 
            ,/*ds ip_address*/'ip' 
            ,/*ds is_enable*/1 
            ,/*ds reg_date*/'2000-01-01' 
            ,/*ds reg_username*/'name' 
            ,/*ds upd_date*/'2000-01-01' 
            ,/*ds upd_username*/'name' 
            ,/*ds is_active*/1 
        )
";
            }
            else
            {
                sql = @"
MERGE INTO ControllerIpFilter AS target
    USING
    (
        SELECT
            @controller_id AS controller_id
            ,@ip_address AS ip_address
    ) AS source
    ON
        target.controller_id = source.controller_id AND target.ip_address = source.ip_address
    WHEN MATCHED THEN
        UPDATE
        SET
            is_enable = CASE WHEN @is_active = 1 THEN @is_enable ELSE is_enable END,
            is_active = @is_active
            ,upd_date = @upd_date
            ,upd_username = @upd_username
    WHEN NOT MATCHED THEN
        INSERT
        (
            controller_ip_filter_id
            ,controller_id
            ,ip_address
            ,is_enable
            ,reg_date
            ,reg_username
            ,upd_date
            ,upd_username
            ,is_active
        )
        VALUES
        (
            NEWID()
            ,@controller_id
            ,@ip_address
            ,@is_enable
            ,@reg_date
            ,@reg_username
            ,@upd_date
            ,@upd_username
            ,@is_active
        );
";
            }

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                controller_ip_filter_id = true,
                controller_id = true,
                ip_address = true,
                is_enable = true,
                reg_date = true,
                reg_username = true,
                upd_date = true,
                upd_username = true,
                is_active = true,
            });

            List<object> paramList = new();
            foreach (var model in modelList)
            {
                if (string.IsNullOrEmpty(model.IpAddress)) throw new ArgumentNullException(model.IpAddress);

                // 新規登録時はControllerIdを受け取る
                if (string.IsNullOrEmpty(model.ControllerId)) model.ControllerId = controllerId;

                // 登録値がfalseかつDB未登録の場合は処理しない
                if (!model.IsActive && !ExistsControllerIpFilter(model.IpAddress))
                {
                    return modelList;
                }

                var param = new
                {
                    controller_id = model.ControllerId,
                    ip_address = model.IpAddress,
                    is_enable = model.IsEnable,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = model.IsActive
                };
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                paramList.Add(dynParams);
            }

            Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            return modelList;
        }

        #endregion

        #region ControllerTag
        /// <summary>
        /// ControllerTagリストを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>ControllerTagリスト</returns>
        public IEnumerable<ControllerTagInfoModel> GetControllerTagList(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.controller_tag_id AS ControllerTagId
    ,c.controller_id AS ControllerId
    ,c.tag_id AS TagId
    ,t.tag_name AS TagName
    ,t.is_active AS IsActive
FROM
    CONTROLLER_TAG c
    INNER JOIN TAG t ON c.tag_id = t.tag_id AND t.is_active = 1
WHERE
    c.controller_id = /*ds controllerId*/'id' 
    AND c.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    c.controller_tag_id AS ControllerTagId
    ,c.controller_id AS ControllerId
    ,c.tag_id AS TagId
    ,t.tag_name AS TagName
    ,t.is_active AS IsActive
FROM
    ControllerTag c
    INNER JOIN Tag t ON c.tag_id = t.tag_id AND t.is_active = 1
WHERE
    c.controller_id = @controllerId
    AND c.is_active = 1
";
            }

            var param = new { controllerId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ControllerTagInfoModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// VendorIdに紐づくControllerのTagを取得します。
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのTagを取得するか</param>
        /// <returns>タグ</returns>
        private Dictionary<string, IEnumerable<ControllerTagInfoModel>> GetControllerTag(string vendorId, bool isAll)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    ct.controller_id as ControllerId
    ,ct.controller_tag_id as ControllerTagId
    ,t.tag_name as TagName
    ,t.tag_id as TagId
    ,ct.is_active as IsActive
FROM
    CONTROLLER_TAG ct /*WITH(NOLOCK)*/
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = ct.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN TAG t /*WITH(NOLOCK)*/ ON ct.tag_id = t.tag_id AND t.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendorId*/'id' AND
/*ds end if*/
    ct.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    ct.controller_id as ControllerId
    ,ct.controller_tag_id as ControllerTagId
    ,t.tag_name as TagName
    ,t.tag_id as TagId
    ,ct.is_active as IsActive
FROM
    ControllerTag ct WITH(NOLOCK)
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = ct.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN Tag t WITH(NOLOCK) ON ct.tag_id = t.tag_id AND t.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendorId AND
/*ds end if*/
    ct.is_active = 1
";
            }
            var param = new { vendorId };
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendorId", vendorId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<string, ControllerTagInfoModel, (string ControllerId, ControllerTagInfoModel Tag)>(
                sql: twowaySql.Sql,
                splitOn: "ControllerTagId",
                map: (controllerId, tag) => (controllerId, tag),
                param: dynParams);

            return result.GroupBy(x => x.ControllerId).ToDictionary(x => x.Key, y => y.Select(z => z.Tag));
        }

        /// <summary>
        /// ControllerTagを登録または更新します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <param name="modelList">ControllerTagInfoModelリスト</param>
        /// <returns>ControllerTagInfoModelリスト</returns>
        public List<ControllerTagInfoModel> UpsertControllerTagInfoList(string controllerId, List<ControllerTagInfoModel> modelList)
        {
            // 一旦全て無効にする
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var updsql = "";
            if (dbSettings.Type == "Oracle")
            {
                updsql = "UPDATE CONTROLLER_TAG SET is_active = 0 WHERE controller_id = /*ds controllerId*/'id' ";
            }
            else
            {
                updsql = "UPDATE ControllerTag SET is_active = 0 WHERE controller_id = @controllerId";
            }
            var param2 = new { controllerId };
            var updtwowaySql = new TwowaySqlParser(dbSettings.GetDbType(), updsql, param2);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param2);
            Connection.Execute(updtwowaySql.Sql, dynParams);

            List<object> paramList = new();
            foreach (var model in modelList)
            {
                if (string.IsNullOrEmpty(model.ControllerId)) model.ControllerId = controllerId;
                if (string.IsNullOrEmpty(model.ControllerTagId)) model.ControllerTagId = Guid.NewGuid().ToString();

                var param = new
                {
                    controller_tag_id = model.ControllerTagId,
                    controller_id = model.ControllerId,
                    tag_id = model.TagId,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = model.IsActive
                };
                dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                paramList.Add(dynParams);
            }

            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO CONTROLLER_TAG target
USING
(
    SELECT
        /*ds tag_id*/'id' AS tag_id,
        /*ds controller_id*/'id' AS controller_id
    FROM dual
) source
ON (
    target.tag_id = source.tag_id AND target.controller_id = source.controller_id
)
WHEN MATCHED THEN
    UPDATE
    SET
        is_active = /*ds is_active*/1 
        ,upd_date = /*ds upd_date*/'2000-01-01' 
        ,upd_username = /*ds upd_username*/'name' 
WHEN NOT MATCHED THEN
    INSERT
    (
        controller_tag_id
        ,controller_id
        ,tag_id
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        /*ds controller_tag_id*/'id' 
        ,/*ds controller_id*/'id' 
        ,/*ds tag_id*/'id' 
        ,/*ds reg_date*/'2000-01-01' 
        ,/*ds reg_username*/'name' 
        ,/*ds upd_date*/'2000-01-01' 
        ,/*ds upd_username*/'name' 
        ,/*ds is_active*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO ControllerTag AS target
USING
(
    SELECT
        @tag_id AS tag_id,
        @controller_id AS controller_id
) AS source
ON
    target.tag_id = source.tag_id AND target.controller_id = source.controller_id
WHEN MATCHED THEN
    UPDATE
    SET
        is_active = @is_active
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        controller_tag_id
        ,controller_id
        ,tag_id
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        @controller_tag_id
        ,@controller_id
        ,@tag_id
        ,@reg_date
        ,@reg_username
        ,@upd_date
        ,@upd_username
        ,@is_active
    );
";
            }
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                controller_tag_id = true,
                controller_id = true,
                tag_id = true,
                reg_date = true,
                reg_username = true,
                upd_date = true,
                upd_username = true,
                is_active = true,
            });

            Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            return modelList;
        }

        #endregion

        #region ControllerField
        /// <summary>
        /// ControllerFieldリストを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>ControllerFieldリスト</returns>
        public IEnumerable<ControllerFieldInfoModel> GetControllerFieldList(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    cf.controller_field_id as ControllerFieldId
    ,cf.controller_id as ControllerId
    ,cf.field_id as FieldId
    ,f.field_name as FieldName
    ,cf.is_active as IsActive
FROM
    CONTROLLER_FIELD cf
    INNER JOIN FIELD f ON f.field_id = cf.field_id AND f.is_active = 1
WHERE
    cf.controller_id = /*ds controllerId*/'id' 
    AND cf.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    cf.controller_field_id as ControllerFieldId
    ,cf.controller_id as ControllerId
    ,cf.field_id as FieldId
    ,f.field_name as FieldName
    ,f.is_active as IsActive
FROM
    ControllerField cf
    INNER JOIN Field f ON f.field_id = cf.field_id AND f.is_active = 1
WHERE
    cf.controller_id = @controllerId
    AND cf.is_active = 1
";
            }

            var param = new { controllerId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ControllerFieldInfoModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// VendorIdに紐づくControllerのFieldを取得します。
        /// </summary>
        /// <param name="vendorId">VendorId</param>
        /// <param name="isAll">全てのベンダーのFieldを取得する</param>
        /// <returns>分野</returns>
        private Dictionary<string, IEnumerable<ControllerFieldInfoModel>> GetControllerField(string vendorId, bool isAll)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    cf.controller_id as ControllerId
    ,cf.controller_field_id as ControllerFieldId
    ,cf.field_id as FieldId
    ,f.field_name as FieldName
FROM
    CONTROLLER_FIELD cf /*WITH(NOLOCK)*/
    INNER JOIN CONTROLLER c /*WITH(NOLOCK)*/ ON c.controller_id = cf.controller_id AND c.is_active = 1
    INNER JOIN VENDOR v /*WITH(NOLOCK)*/ ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN SYSTEM s /*WITH(NOLOCK)*/ ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN FIELD f /*WITH(NOLOCK)*/ ON f.field_id = cf.field_id AND f.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = /*ds vendorId*/'id' AND
/*ds end if*/
    cf.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    cf.controller_id as ControllerId
    ,cf.controller_field_id as ControllerFieldId
    ,cf.field_id as FieldId
    ,f.field_name as FieldName
FROM
    ControllerField cf WITH(NOLOCK)
    INNER JOIN Controller c WITH(NOLOCK) ON c.controller_id = cf.controller_id AND c.is_active = 1
    INNER JOIN Vendor v WITH(NOLOCK) ON v.vendor_id = c.vendor_id AND v.is_enable = 1 AND v.is_active = 1
    INNER JOIN System s WITH(NOLOCK) ON s.system_id = c.system_id AND s.is_enable = 1 AND s.is_active = 1
    INNER JOIN Field f WITH(NOLOCK) ON f.field_id = cf.field_id AND f.is_active = 1
WHERE
/*ds if IsAll != null*/
    c.vendor_id = @vendorId AND
/*ds end if*/
    cf.is_active = 1
";
            }
            var param = new { vendorId };
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("IsAll", isAll ? null : "isAll");
            twowaySqlParam.Add("vendorId", vendorId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<string, ControllerFieldInfoModel, (string ControllerId, ControllerFieldInfoModel Field)>(
                sql: twowaySql.Sql,
                splitOn: "ControllerFieldId",
                map: (controllerId, field) => (controllerId, field),
                param: dynParams);

            return result.GroupBy(x => x.ControllerId).ToDictionary(x => x.Key, y => y.Select(z => z.Field));
        }

        /// <summary>
        /// ControllerFieldを登録または更新します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <param name="modelList">ControllerFieldInfoLリスト</param>
        /// <returns>ControllerFieldInfoLリスト</returns>
        public List<ControllerFieldInfoModel> UpsertControllerFieldInfoList(string controllerId, List<ControllerFieldInfoModel> modelList)
        {
            // 一旦全て無効にする
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var updsql = "";
            if (dbSettings.Type == "Oracle")
            {
                updsql = "UPDATE CONTROLLER_FIELD SET is_active = 0 WHERE controller_id = /*ds controllerId*/'id' ";
            }
            else
            {
                updsql = "UPDATE ControllerField SET is_active = 0 WHERE controller_id = @controllerId";
            }
            var param2 = new { controllerId };
            var upd_twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), updsql, param2);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param2);
            Connection.Execute(upd_twowaySql.Sql, dynParams);

            List<object> paramList = new();
            foreach (var model in modelList)
            {
                if (string.IsNullOrEmpty(model.ControllerId)) model.ControllerId = controllerId;
                if (string.IsNullOrEmpty(model.ControllerFieldId)) model.ControllerFieldId = Guid.NewGuid().ToString();
                var param = new
                {
                    controller_field_id = model.ControllerFieldId,
                    controller_id = model.ControllerId,
                    field_id = model.FieldId,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = model.IsActive
                };
                dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                paramList.Add(dynParams);
            }

            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO CONTROLLER_FIELD target
USING
(
    SELECT
        /*ds field_id*/'id' AS field_id,
        /*ds controller_id*/'id' AS controller_id
    FROM dual
) source
ON (
    target.field_id = source.field_id AND target.controller_id = source.controller_id
)
WHEN MATCHED THEN
    UPDATE
    SET
        is_active = /*ds is_active*/1 
        ,upd_date = /*ds upd_date*/'2000-01-01' 
        ,upd_username = /*ds upd_username*/'name' 
WHEN NOT MATCHED THEN
    INSERT
    (
        controller_field_id
        ,controller_id
        ,field_id
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        /*ds controller_field_id*/'id' 
        ,/*ds controller_id*/'id' 
        ,/*ds field_id*/'id' 
        ,/*ds reg_date*/'2000-01-01' 
        ,/*ds reg_username*/'name' 
        ,/*ds upd_date*/'2000-01-01' 
        ,/*ds upd_username*/'name' 
        ,/*ds is_active*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO ControllerField AS target
USING
(
    SELECT
        @field_id AS field_id,
        @controller_id AS controller_id
) AS source
ON
    target.field_id = source.field_id AND target.controller_id = source.controller_id
WHEN MATCHED THEN
    UPDATE
    SET
        is_active = @is_active
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        controller_field_id
        ,controller_id
        ,field_id
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        @controller_field_id
        ,@controller_id
        ,@field_id
        ,@reg_date
        ,@reg_username
        ,@upd_date
        ,@upd_username
        ,@is_active
    );
";
            }

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                controller_field_id = true,
                controller_id = true,
                field_id = true,
                reg_date = true,
                reg_username = true,
                upd_date = true,
                upd_username =true,
                is_active = true,
            });

            Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            return modelList;
        }

        #endregion

        #region ControllerMultiLanguage
        /// <summary>
        /// ControllerMultiLanguageを登録または更新します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <param name="modelList"></param>
        /// <returns></returns>
        public List<ControllerMultiLanguageModel> RegisterControllerMultiLanguage(string controllerId, List<ControllerMultiLanguageModel> modelList)
        {
            // 既存データを全て無効化
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var updsql = "";
            if (dbSettings.Type == "Oracle")
            {
                updsql = "UPDATE CONTROLLER_MULTI_LANGUAGE SET is_active = 0 WHERE controller_id = /*ds controllerId*/'id' ";
            }
            else
            {
                updsql = "UPDATE ControllerMultiLanguage SET is_active = 0 WHERE controller_id = @controllerId";
            }
            var param2 = new { controllerId };
            var upd_twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), updsql, param2);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param2);
            Connection.Execute(upd_twowaySql.Sql, dynParams);

            // 登録・更新
            List<object> paramList = new();
            foreach (var model in modelList)
            {
                if (string.IsNullOrEmpty(model.ControllerLangId)) model.ControllerLangId = Guid.NewGuid().ToString();
                var param = new
                {
                    controller_lang_id = model.ControllerLangId,
                    controller_id = controllerId,
                    controller_description = model.ControllerDescription,
                    controller_name = model.ControllerName,
                    fee_description = model.FeeDescription,
                    resource_create_user = model.ResourceCreateUser,
                    resource_maintainer = model.ResourceMaintainer,
                    update_frequency = model.UpdateFrequency,
                    contact_information = model.ContactInformation,
                    version = model.Version,
                    agree_description = model.AgreeDescription,
                    locale_code = model.LocaleCode,
                    reg_date = UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = model.IsActive
                };
                dynParams = dbSettings.GetParameters().AddDynamicParams(param)
                                    .SetNClob(nameof(param.agree_description))
                                    .SetNClob(nameof(param.contact_information))
                                    .SetNClob(nameof(param.controller_description))
                                    .SetNClob(nameof(param.controller_name))
                                    .SetNClob(nameof(param.fee_description))
                                    .SetNClob(nameof(param.resource_create_user))
                                    .SetNClob(nameof(param.resource_maintainer))
                                    .SetNClob(nameof(param.update_frequency))
                                    .SetNClob(nameof(param.version));
                paramList.Add(dynParams);
            }

            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO CONTROLLER_MULTI_LANGUAGE target
USING
(
    SELECT
        /*ds controller_id*/'id' AS controller_id,
        /*ds locale_code*/'cd' AS locale_code
    FROM dual
) source
ON (
    target.controller_id = source.controller_id AND target.locale_code = source.locale_code
)
WHEN MATCHED THEN
    UPDATE
    SET
        controller_description = /*ds controller_description*/'des' 
        ,controller_name = /*ds controller_name*/'name' 
        ,fee_description = /*ds fee_description*/'des' 
        ,resource_create_user = /*ds resource_create_user*/'us' 
        ,resource_maintainer = /*ds resource_maintainer*/'res' 
        ,update_frequency = /*ds update_frequency*/'up' 
        ,contact_information = /*ds contact_information*/'inf' 
        ,version = /*ds version*/'ver' 
        ,agree_description = /*ds agree_description*/'agr' 
        ,is_active = /*ds is_active*/1 
        ,upd_date = /*ds upd_date*/'2000-01-01' 
        ,upd_username = /*ds upd_username*/'name' 
WHEN NOT MATCHED THEN
    INSERT
    (
        controller_lang_id
        ,controller_id
        ,controller_description
        ,controller_name
        ,fee_description
        ,resource_create_user
        ,resource_maintainer
        ,update_frequency
        ,contact_information
        ,version
        ,agree_description
        ,locale_code
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        /*ds controller_lang_id*/'id' 
        ,/*ds controller_id*/'id' 
        ,/*ds controller_description*/'des' 
        ,/*ds controller_name*/'name' 
        ,/*ds fee_description*/'des' 
        ,/*ds resource_create_user*/'res' 
        ,/*ds resource_maintainer*/'res' 
        ,/*ds update_frequency*/'up' 
        ,/*ds contact_information*/'inf' 
        ,/*ds version*/'ver' 
        ,/*ds agree_description*/'agr' 
        ,/*ds locale_code*/'cd' 
        ,/*ds reg_date*/'2000-01-01' 
        ,/*ds reg_username*/'name' 
        ,/*ds upd_date*/'2000-01-01' 
        ,/*ds upd_username*/'name' 
        ,/*ds is_active*/1 
    );
";
            }
            else
            {
                sql = @"
MERGE INTO ControllerMultiLanguage AS target
USING
(
    SELECT
        @controller_id AS controller_id,
        @locale_code AS locale_code
) AS source
ON
    target.controller_id = source.controller_id AND target.locale_code = source.locale_code
WHEN MATCHED THEN
    UPDATE
    SET
        controller_description = @controller_description
        ,controller_name = @controller_name
        ,fee_description = @fee_description
        ,resource_create_user = @resource_create_user
        ,resource_maintainer = @resource_maintainer
        ,update_frequency = @update_frequency
        ,contact_information = @contact_information
        ,version = @version
        ,agree_description = @agree_description
        ,is_active = @is_active
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        controller_lang_id
        ,controller_id
        ,controller_description
        ,controller_name
        ,fee_description
        ,resource_create_user
        ,resource_maintainer
        ,update_frequency
        ,contact_information
        ,version
        ,agree_description
        ,locale_code
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        @controller_lang_id
        ,@controller_id
        ,@controller_description
        ,@controller_name
        ,@fee_description
        ,@resource_create_user
        ,@resource_maintainer
        ,@update_frequency
        ,@contact_information
        ,@version
        ,@agree_description
        ,@locale_code
        ,@reg_date
        ,@reg_username
        ,@upd_date
        ,@upd_username
        ,@is_active
    );
";
            }

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                controller_field_id = true,
                controller_id = true,
                controller_description = true,
                controller_name = true,
                fee_description = true,
                resource_create_user = true,
                resource_maintainer = true,
                update_frequency = true,
                contact_information = true,
                version = true,
                agree_description = true,
                locale_code = true,
                reg_date = true,
                reg_username = true,
                upd_date = true,
                upd_username = true,
                is_active = true,
            });

            Connection.ExecutePrimaryKeyList(twowaySql.Sql, paramList);
            return modelList;
        }

        #endregion

        #region Settings
        /// <summary>
        /// 添付ファイル設定を取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>添付ファイル設定</returns>
        public AttachFileSettingsModel GetAttachFileSettings(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.is_enable_attachfile AS IsEnable
    ,TO_CHAR(a.repository_group_id) AS MetaRepositoryId
    ,TO_CHAR(srm.repository_group_id) AS BlobRepositoryId
FROM
    CONTROLLER c
    INNER JOIN API a ON a.controller_id = c.controller_id and a.is_active = 1
    LEFT OUTER JOIN SECONDARY_REPOSITORY_MAP srm ON a.api_id = srm.api_id AND srm.is_active = 1
    INNER JOIN REPOSITORY_GROUP rg ON rg.repository_group_id = srm.repository_group_id AND rg.is_active = 1
WHERE
    a.is_transparent_api = 1
    AND c.controller_id = /*ds controllerId*/'id' 
    AND a.action_type_cd = 'aup'
    AND rg.repository_type_cd = 'afb'
ORDER BY
    c.reg_date DESC
FETCH FIRST 1 ROWS ONLY
";
            }
            else
            {
                sql = @"
SELECT TOP 1
    c.is_enable_attachfile AS IsEnable
    ,convert(nvarchar(36),a.repository_group_id) AS MetaRepositoryId
    ,convert(nvarchar(36),srm.repository_group_id) AS BlobRepositoryId
FROM
    Controller c
    INNER JOIN Api a ON a.controller_id = c.controller_id and a.is_active = 1
    LEFT OUTER JOIN SecondaryRepositoryMap srm ON a.api_id = srm.api_id AND srm.is_active = 1
    INNER JOIN RepositoryGroup rg ON rg.repository_group_id = srm.repository_group_id AND rg.is_active = 1
WHERE
    a.is_transparent_api = 1
    AND c.controller_id = @controllerId
    AND a.action_type_cd = 'aup'
    AND rg.repository_type_cd = 'afb'
ORDER BY
    c.upd_date DESC
";
            }

            var param = new { controllerId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingleOrDefault<AttachFileSettingsModel>(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// 履歴ドキュメント設定を取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>履歴ドキュメント設定</returns>
        public DocumentHistorySettingsModel GetDocumentHistorySettings(string controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.is_document_history AS IsEnable
    ,TO_CHAR(srm.repository_group_id) AS HistoryRepositoryId
FROM
    CONTROLLER c
    INNER JOIN API a ON a.controller_id = c.controller_id and a.is_active = 1
    INNER JOIN SECONDARY_REPOSITORY_MAP srm ON a.api_id = srm.api_id AND srm.is_active = 1
    INNER JOIN REPOSITORY_GROUP rg ON rg.repository_group_id = srm.repository_group_id AND rg.is_active = 1
WHERE
    a.is_transparent_api = 1
    AND c.controller_id = /*ds controllerId*/'id' 
    AND rg.repository_type_cd = 'dhs'
FETCH FIRST 1 ROWS ONLY
";
            }
            else
            {
                sql = @"
SELECT TOP 1
    c.is_document_history AS IsEnable
    ,convert(nvarchar(36),srm.repository_group_id) AS HistoryRepositoryId
FROM
    Controller c
    INNER JOIN Api a ON a.controller_id = c.controller_id and a.is_active = 1
    INNER JOIN SecondaryRepositoryMap srm ON a.api_id = srm.api_id AND srm.is_active = 1
    INNER JOIN RepositoryGroup rg ON rg.repository_group_id = srm.repository_group_id AND rg.is_active = 1
WHERE
    a.is_transparent_api = 1
    AND c.controller_id = @controllerId
    AND rg.repository_type_cd = 'dhs'
";
            }

            var param = new { controllerId = true };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(new { controllerId });
            return Connection.QuerySingleOrDefault<DocumentHistorySettingsModel>(twowaySql.Sql, dynParams);
        }

        #endregion

        #region AbstractDynamicApiRepositoryのなかにいたもの

        protected static IConfigurationSection AppConfig => s_lazyAppConfig.Value;
        protected static new Lazy<IConfigurationSection> s_lazyAppConfig = new(() => UnityCore.Resolve<IConfiguration>().GetSection("AppConfig"));

        /// <summary>
        /// リソースのバージョンを使用するかの全体設定
        /// </summary>
        protected bool EnableResourceVersion => AppConfig.GetValue<bool>("EnableResourceVersion", true);

        /// <summary>
        /// コントローラーとAPIを取得します。
        /// </summary>
        /// <param name="controllerId">コントローラID</param>
        /// <returns>コントローラモデル</returns>
        public List<AllApiModel> GetControllerApi(string controllerId)
        {
            var allApiEntitiyList = GetApiFullList(true, controllerId);
            var allApiRepositoryList = GetAllApiRepositoryModel(controllerId);

            allApiEntitiyList.ForEach(allApiEntity =>
            {
                allApiEntity.all_repository_model_list = GetAllRepositoryModelList(allApiEntity, allApiRepositoryList);
            });
            return allApiEntitiyList;
        }

        private List<AllApiRepositoryModel> GetAllRepositoryModelList(AllApiModel allApi, IEnumerable<AllApiRepositoryModel> allRepositoryList)
        {
            List<AllApiRepositoryModel> result = new List<AllApiRepositoryModel>();
            //対象APIのもののみ取得
            var apiAllRepositoryList = allRepositoryList.Where(r => allApi.api_id == r.api_id).ToList();

            //セカンダリのis_primary=trueがあれば、is_primary=trueのリポジトリ、apiEntityのリポジトリ、その他の順で詰める
            //セカンダリのプライマリ
            var first = apiAllRepositoryList.Where(x => !x.is_primary).Where(x => x.is_secondary_primary).FirstOrDefault();
            if (first != null)
            {
                result.Add(first);
            }
            //セカンダリのプライマリ
            var second = apiAllRepositoryList.Where(x => x.is_primary).FirstOrDefault();
            if (second != null)
            {
                result.Add(second);
            }
            //セカンダリのセカンダリ
            var third = apiAllRepositoryList.Where(x => !x.is_primary).Where(x => !x.is_primary);
            if (third.Any())
            {
                result.AddRange(third.ToList());
            }
            // 履歴用
            if (allApi.is_document_history == true && apiAllRepositoryList?.Count != 0)
            {
                allApi.history_repository_model = apiAllRepositoryList.Where(x => x.is_primary == false && x.api_id == allApi.api_id && x.repository_type_cd == "dhs").FirstOrDefault();
            }

            return result;
        }

        private List<AllApiRepositoryIncludePhysicalRepositoryModel> GetRepository()
        {
            return GetRepositoryFromChache();
        }

        private List<AllApiRepositoryIncludePhysicalRepositoryModel> GetRepositoryByControllerId(string controllerId)
        {
            return GetRepositoryFromDB<AllApiRepositoryIncludePhysicalRepositoryModel>(controllerId);
        }

        private List<AllApiRepositoryIncludePhysicalRepositoryModel> GetRepositoryFromChache()
        {
            return GetRepositoryFromDB<AllApiRepositoryIncludePhysicalRepositoryModel>(null);
            //return Cache.Get<List<AllApiRepositoryIncludePhysicalRepositoryModel>>(CACHE_KEY_GET_SECONDARY_REPOSITORY, CacheExpirationTimeSpan, () => GetRepositoryFromDB<AllApiRepositoryIncludePhysicalRepositoryModel>(null));
        }

        private List<T> GetRepositoryFromDB<T>(string? controllerId)
        {
            //[BUG]String.Format使用
            //リポジトリグループ、セカンダリリポジトリグループのPhysicalRepositoryを軸に取得する
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
        --GetRepository
        SELECT
            a.api_id               AS api_id
            ,a.repository_group_id AS repository_group_id
            ,pr.connection_string  AS repository_connection_string
            ,rg.repository_type_cd AS repository_type_cd
            ,pr.is_full            AS is_full
            ,1                     AS is_primary
            ,0                     AS is_secondary_primary
            ,pr.physical_repository_id AS physical_repository_id
            ,c.controller_id     AS controller_id
        FROM
            CONTROLLER c
            INNER JOIN API a ON c.controller_id=a.controller_id AND a.is_active=1
            INNER JOIN REPOSITORY_GROUP rg ON a.repository_group_id=rg.repository_group_id AND rg.is_active=1
            INNER JOIN PHYSICAL_REPOSITORY pr ON rg.repository_group_id=pr.repository_group_id AND pr.is_active=1
        WHERE
            c.is_active=1
/*ds if controllerId != null*/ 
            AND c.controller_id= /*ds controller_id*/'id' 
/*ds end if*/ 
        UNION ALL
        SELECT
            a.api_id               AS api_id
            ,secrg.repository_group_id AS repository_group_id
            ,pr.connection_string  AS repository_connection_string
            ,rg.repository_type_cd AS repository_type_cd
            ,pr.is_full            AS is_full
            ,0                     AS is_primary
            ,secrg.is_primary      AS is_secondary_primary
            ,pr.physical_repository_id AS physical_repository_id
            ,c.controller_id     AS controller_id
        FROM
            CONTROLLER c
            INNER JOIN API a ON c.controller_id=a.controller_id AND a.is_active=1
            INNER JOIN SECONDARY_REPOSITORY_MAP secrg ON a.api_id=secrg.api_id AND secrg.is_active=1
            INNER JOIN REPOSITORY_GROUP rg ON secrg.repository_group_id=rg.repository_group_id AND rg.is_active=1
            INNER JOIN PHYSICAL_REPOSITORY pr ON rg.repository_group_id=pr.repository_group_id AND pr.is_active=1
        WHERE
            c.is_active=1
/*ds if controllerId != null*/
            AND c.controller_id= /*ds controller_id*/'id' 
/*ds end if*/ 
        ";
            }
            else
            {
                sql = @"
        --GetRepository
        SELECT
            a.api_id               AS api_id
            ,a.repository_group_id AS repository_group_id
            ,pr.connection_string  AS repository_connection_string
            ,rg.repository_type_cd AS repository_type_cd
            ,pr.is_full            AS is_full
            ,1                     AS is_primary
            ,0                     AS is_secondary_primary
            ,pr.physical_repository_id AS physical_repository_id
            ,c.controller_id     AS controller_id
        FROM
            Controller c
            INNER JOIN Api a ON c.controller_id=a.controller_id AND a.is_active=1
            INNER JOIN RepositoryGroup rg ON a.repository_group_id=rg.repository_group_id AND rg.is_active=1
            INNER JOIN PhysicalRepository pr ON rg.repository_group_id=pr.repository_group_id AND pr.is_active=1
        WHERE
            c.is_active=1
/*ds if controllerId != null*/ 
            AND c.controller_id= /*ds controller_id*/'id' 
/*ds end if*/ 
        UNION ALL
        SELECT
            a.api_id               AS api_id
            ,secrg.repository_group_id AS repository_group_id
            ,pr.connection_string  AS repository_connection_string
            ,rg.repository_type_cd AS repository_type_cd
            ,pr.is_full            AS is_full
            ,0                     AS is_primary
            ,secrg.is_primary      AS is_secondary_primary
            ,pr.physical_repository_id AS physical_repository_id
            ,c.controller_id     AS controller_id
        FROM
            Controller c
            INNER JOIN Api a ON c.controller_id=a.controller_id AND a.is_active=1
            INNER JOIN SecondaryRepositoryMap secrg ON a.api_id=secrg.api_id AND secrg.is_active=1
            INNER JOIN RepositoryGroup rg ON secrg.repository_group_id=rg.repository_group_id AND rg.is_active=1
            INNER JOIN PhysicalRepository pr ON rg.repository_group_id=pr.repository_group_id AND pr.is_active=1
        WHERE
            c.is_active=1
/*ds if controllerId != null*/ 
            AND c.controller_id= /*ds controller_id*/'id' 
/*ds end if*/ 
        ";
            }
            var param = new { controller_id = controllerId };
            var twowaySql= new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<T>(twowaySql.Sql, dynParams).ToList();
        }

        private List<AllApiModel> GetApiFullList(bool isActiveOnly, string? controllerId = null)
        {
            return GetApiFullListFromDB(isActiveOnly, controllerId);
        }

        private List<AllApiModel> GetApiFullListFromDB(bool isActiveOnly, string? controllerId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
--GetApiFullList
SELECT
 *
FROM
 (
SELECT
         c.vendor_id              AS vendor_id
        ,c.system_id              AS system_id
        ,c.controller_id          AS controller_id
        ,c.controller_description AS controller_description
        ,c.url                    AS controller_relative_url
        ,c.is_vendor              AS is_vendor
        ,C.is_person              AS is_person
        ,c.is_enable              AS is_enable_controller
        ,c.controller_repository_key as controller_repository_key
        ,c.controller_schema_id   AS controller_schema_id
        ,c.controller_partition_key AS partition_key
        ,c.is_visible_agreement AS is_visible_agreement
        ,c.is_toppage           AS is_toppage
        ,c.reg_username AS controller_reg_username
        ,c.upd_username AS controller_upd_username
        ,c.reg_date     AS controller_reg_date
        ,c.upd_date     AS controller_upd_date
        ,c.is_active    AS controller_is_active
        ,c.is_enable_attachfile    AS is_enable_attachfile
        ,c.is_use_blob_cache       AS is_use_blob_cache
        ,c.is_optimistic_concurrency   AS is_optimistic_concurrency
        ,c.is_enable_blockchain AS is_enable_blockchain
        ,c.is_document_history AS is_document_history
        ,c.is_container_dynamic_separation AS is_container_dynamic_separation
        ,c.is_enable_resource_version AS is_enable_resource_version
        ,a.api_id                      AS api_id
        ,a.api_description             AS api_description
        ,a.url                         AS method_name
        ,a.method_type                 AS method_type
        ,a.is_admin_authentication     AS is_admin_authentication
        ,a.is_header_authentication    AS is_header_authentication
        ,a.is_vendor_system_authentication_allow_null    AS is_vendor_system_authentication_allow_null
        ,a.is_openid_authentication    AS is_openid_authentication
        ,a.post_data_type              AS post_data_type
        ,a.query                       AS query
        ,a.query_type_cd              AS query_type_cd
        ,a.is_enable                   AS is_enable_api
        ,a.is_hidden                    AS is_hidden
        ,a.gateway_url                 AS gateway_url
        ,a.gateway_credential_username AS gateway_credential_username
        ,a.gateway_credential_password AS gateway_credential_password
        ,a.is_over_partition           AS is_over_partition
        ,a.repository_group_id         AS repository_group_id
        ,a.script                      AS script
        ,a.action_type_cd              AS action_type_cd
        ,a.script_type_cd              AS script_type_cd
        ,a.actiontype_version
        ,a.reg_username                AS api_reg_username
        ,a.upd_username                AS api_upd_username
        ,a.reg_date                    AS api_reg_date
        ,a.upd_date                    AS api_upd_date
        ,a.is_active                   AS api_is_active
        ,a.is_transparent_api          AS is_transparent_api
        ,a.is_skip_jsonschema_validation AS is_skip_jsonschema_validation
        ,a.is_openid_authentication_allow_null AS is_openid_authentication_allow_null
        ,rg.repository_type_cd AS repository_type_cd
        ,rg.is_enable AS is_enable_repository
        ,a.is_cache         AS is_cache
        ,a.cache_minute AS cache_minute
        ,a.cache_key AS cache_key
        ,a.is_accesskey AS is_accesskey
        ,a.is_automatic_id AS is_automatic_id
        ,a.gateway_relay_header AS gateway_relay_header
        ,a.is_internal_call_only       AS is_internal_call_only
        ,a.internal_call_keyword       AS internal_call_keyword
        ,a.is_clientcert_authentication AS is_clientcert_authentication
        ,a.is_otherresource_sqlaccess   AS is_otherresource_sqlaccess
        ,dsreq.data_schema_id AS request_schema_id
        ,dsreq.data_schema    AS request_schema
        ,dsreq.schema_name    AS request_schema_name
        ,dsreq.vendor_id AS request_vendor_id
        ,dsreq.reg_date AS request_reg_date
        ,dsreq.upd_date AS request_upd_date
        ,dsreq.is_active AS request_is_active
        ,dsreq.schema_description AS request_schema_description
        ,dsres.data_schema_id AS response_schema_id
        ,dsres.data_schema    AS response_schema
        ,dsres.schema_name    AS response_schema_name
        ,dsres.vendor_id AS response_vendor_id
        ,dsres.reg_date AS response_reg_date
        ,dsres.upd_date AS response_upd_date
        ,dsres.is_active AS response_is_active
        ,dsres.schema_description AS response_schema_description
        ,dsurl.data_schema_id AS url_schema_id
        ,dsurl.data_schema    AS url_schema
        ,dsurl.schema_name    AS url_schema_name
        ,dsurl.vendor_id AS url_vendor_id
        ,dsurl.reg_date AS url_reg_date
        ,dsurl.upd_date AS url_upd_date
        ,dsurl.is_active AS url_is_active
        ,dsurl.schema_description AS url_schema_description
        ,dscontroller.data_schema    AS controller_schema
        ,dscontroller.reg_date AS controller_schema_reg_date
        ,dscontroller.upd_date AS controller_schema_upd_date
        ,dscontroller.is_active AS controller_schema_is_active
        ,dscontroller.schema_description AS controller_schema_description
        ,at.action_type_name
     ,TO_DATE(cp.public_start_datetime,'yyyy/MM/dd HH:mm:ss') AS public_start_datetime
     ,TO_DATE(cp.public_end_datetime,'yyyy/MM/dd HH:mm:ss') AS public_end_datetime
        ,(
            SELECT
                COUNT(*)
            FROM
                SECONDARY_REPOSITORY_MAP srm
                INNER JOIN REPOSITORY_GROUP rg ON rg.repository_group_id=srm.repository_group_id AND rg.is_active=1
            WHERE
                srm.api_id=a.api_id
                AND srm.is_active=1
                AND rg.is_enable=0
        ) AS secondary_repository_disable_count

        FROM
            CONTROLLER c
            INNER JOIN API a ON c.controller_id=a.controller_id AND a.is_active = /*ds isActiveList*/1 
            INNER JOIN ACTION_TYPE at ON a.action_type_cd=at.action_type_cd AND at.is_active=1
         INNER JOIN VENDOR vender ON c.vendor_id=vender.vendor_id AND vender.is_active  = /*ds isActiveList*/1 AND vender.is_enable = /*ds isActiveList*/1 
         INNER JOIN SYSTEM system ON c.system_id=system.system_id AND system.is_active  = /*ds isActiveList*/1 AND system.is_enable = /*ds isActiveList*/1 
            LEFT OUTER JOIN REPOSITORY_GROUP rg ON a.repository_group_id=rg.repository_group_id AND rg.is_active=1 AND rg.is_enable = 1
            LEFT OUTER JOIN DATA_SCHEMA dsreq ON a.request_schema_id=dsreq.data_schema_id AND dsreq.is_active = /*ds isActiveList*/1 
            LEFT OUTER JOIN DATA_SCHEMA dsres ON a.response_schema_id=dsres.data_schema_id AND dsres.is_active = /*ds isActiveList*/1 
            LEFT OUTER JOIN DATA_SCHEMA dsurl ON a.url_schema_id=dsurl.data_schema_id AND dsurl.is_active = /*ds isActiveList*/1 
            LEFT OUTER JOIN DATA_SCHEMA dscontroller ON c.controller_schema_id=dscontroller.data_schema_id AND dscontroller.is_active = /*ds isActiveList*/1 
            LEFT OUTER JOIN CONTROLLER_PRICIES cp ON c.controller_id=cp.controller_id AND cp.is_active = /*ds isActiveList*/1 
        WHERE
/*ds if ControllerId != null*/
            c.controller_id = /*ds ControllerId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
            c.is_active = /*ds isActiveList*/1 AND
           (a.action_type_cd = 'gtw'  OR is_transparent_api = 1 OR (a.action_type_cd <> 'gtw' AND rg.repository_group_id IS NOT NULL ))
    ) x
WHERE
 x.secondary_repository_disable_count=0
ORDER BY
    x.controller_relative_url
";
            }
            else
            {
                sql = @"
--GetApiFullList
SELECT
 *
FROM
 (
SELECT
         c.vendor_id              AS vendor_id
        ,c.system_id              AS system_id
        ,c.controller_id          AS controller_id
        ,c.controller_description AS controller_description
        ,c.url                    AS controller_relative_url
        ,c.is_vendor              AS is_vendor
        ,C.is_person              AS is_person
        ,c.is_enable              AS is_enable_controller
        ,c.controller_repository_key as controller_repository_key
        ,c.controller_schema_id   AS controller_schema_id
        ,c.controller_partition_key AS partition_key
        ,c.is_visible_agreement AS is_visible_agreement
        ,c.is_toppage           AS is_toppage
        ,c.reg_username AS controller_reg_username
        ,c.upd_username AS controller_upd_username
        ,c.reg_date     AS controller_reg_date
        ,c.upd_date     AS controller_upd_date
        ,c.is_active    AS controller_is_active
        ,c.is_enable_attachfile    AS is_enable_attachfile
        ,c.is_use_blob_cache       AS is_use_blob_cache
        ,c.is_optimistic_concurrency   AS is_optimistic_concurrency
        ,c.is_enable_blockchain AS is_enable_blockchain
        ,c.is_document_history AS is_document_history
        ,c.is_container_dynamic_separation AS is_container_dynamic_separation
        ,c.is_enable_resource_version AS is_enable_resource_version
        ,a.api_id                      AS api_id
        ,a.api_description             AS api_description
        ,a.url                         AS method_name
        ,a.method_type                 AS method_type
        ,a.is_admin_authentication     AS is_admin_authentication
        ,a.is_header_authentication    AS is_header_authentication
        ,a.is_vendor_system_authentication_allow_null    AS is_vendor_system_authentication_allow_null
        ,a.is_openid_authentication    AS is_openid_authentication
        ,a.post_data_type              AS post_data_type
        ,a.query                       AS query
        ,a.query_type_cd              AS query_type_cd
        ,a.is_enable                   AS is_enable_api
        ,a.is_hidden                    AS is_hidden
        ,a.gateway_url                 AS gateway_url
        ,a.gateway_credential_username AS gateway_credential_username
        ,a.gateway_credential_password AS gateway_credential_password
        ,a.is_over_partition           AS is_over_partition
        ,a.repository_group_id         AS repository_group_id
        ,a.script                      AS script
        ,a.action_type_cd              AS action_type_cd
        ,a.script_type_cd              AS script_type_cd
        ,a.actiontype_version
        ,a.reg_username                AS api_reg_username
        ,a.upd_username                AS api_upd_username
        ,a.reg_date                    AS api_reg_date
        ,a.upd_date                    AS api_upd_date
        ,a.is_active                   AS api_is_active
        ,a.is_transparent_api          AS is_transparent_api
        ,a.is_skip_jsonschema_validation AS is_skip_jsonschema_validation
        ,a.is_openid_authentication_allow_null AS is_openid_authentication_allow_null
        ,rg.repository_type_cd AS repository_type_cd
        ,rg.is_enable AS is_enable_repository
        ,a.is_cache         AS is_cache
        ,a.cache_minute AS cache_minute
        ,a.cache_key AS cache_key
        ,a.is_accesskey AS is_accesskey
        ,a.is_automatic_id AS is_automatic_id
        ,a.gateway_relay_header AS gateway_relay_header
        ,a.is_internal_call_only       AS is_internal_call_only
        ,a.internal_call_keyword       AS internal_call_keyword
        ,a.is_clientcert_authentication AS is_clientcert_authentication
        ,a.is_otherresource_sqlaccess   AS is_otherresource_sqlaccess
        ,dsreq.data_schema_id AS request_schema_id
        ,dsreq.data_schema    AS request_schema
        ,dsreq.schema_name    AS request_schema_name
        ,dsreq.vendor_id AS request_vendor_id
        ,dsreq.reg_date AS request_reg_date
        ,dsreq.upd_date AS request_upd_date
        ,dsreq.is_active AS request_is_active
        ,dsreq.schema_description AS request_schema_description
        ,dsres.data_schema_id AS response_schema_id
        ,dsres.data_schema    AS response_schema
        ,dsres.schema_name    AS response_schema_name
        ,dsres.vendor_id AS response_vendor_id
        ,dsres.reg_date AS response_reg_date
        ,dsres.upd_date AS response_upd_date
        ,dsres.is_active AS response_is_active
        ,dsres.schema_description AS response_schema_description
        ,dsurl.data_schema_id AS url_schema_id
        ,dsurl.data_schema    AS url_schema
        ,dsurl.schema_name    AS url_schema_name
        ,dsurl.vendor_id AS url_vendor_id
        ,dsurl.reg_date AS url_reg_date
        ,dsurl.upd_date AS url_upd_date
        ,dsurl.is_active AS url_is_active
        ,dsurl.schema_description AS url_schema_description
        ,dscontroller.data_schema    AS controller_schema
        ,dscontroller.reg_date AS controller_schema_reg_date
        ,dscontroller.upd_date AS controller_schema_upd_date
        ,dscontroller.is_active AS controller_schema_is_active
        ,dscontroller.schema_description AS controller_schema_description
        ,at.action_type_name
     ,FORMAT(cp.public_start_datetime,'yyyy/MM/dd HH:mm:ss') AS public_start_datetime
     ,FORMAT(cp.public_end_datetime,'yyyy/MM/dd HH:mm:ss') AS public_end_datetime
        ,(
            SELECT
                COUNT(*)
            FROM
                SecondaryRepositoryMap srm
                INNER JOIN RepositoryGroup rg ON rg.repository_group_id=srm.repository_group_id AND rg.is_active=1
            WHERE
                srm.api_id=a.api_id
                AND srm.is_active=1
                AND rg.is_enable=0
        ) AS secondary_repository_disable_count

        FROM
            Controller c
            INNER JOIN Api a ON c.controller_id=a.controller_id AND a.is_active IN @isActiveList
            INNER JOIN ActionType at ON a.action_type_cd=at.action_type_cd AND at.is_active=1
         INNER JOIN Vendor vender ON c.vendor_id=vender.vendor_id AND vender.is_active  IN @isActiveList AND vender.is_enable IN @isActiveList
         INNER JOIN System system ON c.system_id=system.system_id AND system.is_active  IN @isActiveList AND system.is_enable IN @isActiveList
            LEFT OUTER JOIN RepositoryGroup rg ON a.repository_group_id=rg.repository_group_id AND rg.is_active=1 AND rg.is_enable = 1
            LEFT OUTER JOIN DataSchema dsreq ON a.request_schema_id=dsreq.data_schema_id AND dsreq.is_active IN @isActiveList
            LEFT OUTER JOIN DataSchema dsres ON a.response_schema_id=dsres.data_schema_id AND dsres.is_active IN @isActiveList
            LEFT OUTER JOIN DataSchema dsurl ON a.url_schema_id=dsurl.data_schema_id AND dsurl.is_active IN @isActiveList
            LEFT OUTER JOIN DataSchema dscontroller ON c.controller_schema_id=dscontroller.data_schema_id AND dscontroller.is_active IN @isActiveList
            LEFT OUTER JOIN ControllerPricies cp ON c.controller_id=cp.controller_id AND cp.is_active IN @isActiveList
        WHERE
/*ds if ControllerId != null*/
            c.controller_id = /*ds ControllerId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
            c.is_active IN @isActiveList AND
           (a.action_type_cd = 'gtw'  OR is_transparent_api = 1 OR (a.action_type_cd <> 'gtw' AND rg.repository_group_id IS NOT NULL ))
    ) x
WHERE
 x.secondary_repository_disable_count=0
ORDER BY
    x.controller_relative_url
";
            }

            bool? isActiveListForOracle = true;
            var isActiveListForSQLServer = new List<bool>() { true };
            if (!isActiveOnly)
            {
                isActiveListForOracle = false;
                isActiveListForSQLServer.Add(false);
            }
            var dict = new Dictionary<string, object>
            {
                { "ControllerId", controllerId },
                { "EnableResourceVersion", EnableResourceVersion },
                { "isActiveList",  dbSettings.Type == "Oracle" ? isActiveListForOracle : isActiveListForSQLServer }
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, dict);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(dict);
            return Connection.Query<AllApiModel>(twowaySql.Sql, dynParams).ToList();
        }

        protected IEnumerable<AllApiRepositoryModel> GetAllApiRepositoryModel(string controllerId)
        {
            var allRepositoryList = GetRepositoryByControllerId(controllerId);
            List<AllApiRepositoryModel> allApiRepositoryModelList = new List<AllApiRepositoryModel>();
            foreach (var allRepositoryKey in allRepositoryList.GroupBy(x => new { x.api_id, x.repository_group_id }).Select(x => new { apiId = x.Key.api_id, repositoryGroupId = x.Key.repository_group_id }))
            {
                var repositoryGroups = allRepositoryList.Where(x => x.repository_group_id == allRepositoryKey.repositoryGroupId).Where(x => x.api_id == allRepositoryKey.apiId).ToList();
                AllApiRepositoryModel rg = new AllApiRepositoryModel()
                {
                    api_id = repositoryGroups[0].api_id,
                    repository_group_id = repositoryGroups[0].repository_group_id,
                    repository_type_cd = repositoryGroups[0].repository_type_cd,
                    is_primary = repositoryGroups[0].is_primary,
                    is_secondary_primary = repositoryGroups[0].is_secondary_primary,
                    physical_repository_list = new List<AllApiPhysicalRepositoryModel>()
                };
                foreach (var pr in repositoryGroups)
                {
                    rg.physical_repository_list.Add(new AllApiPhysicalRepositoryModel() { repository_connection_string = pr.repository_connection_string, is_full = pr.is_full, PhysicalRepositoryId = pr.physical_repository_id });
                }
                allApiRepositoryModelList.Add(rg);
            }
            return allApiRepositoryModelList;
        }
        #endregion
    }
}
