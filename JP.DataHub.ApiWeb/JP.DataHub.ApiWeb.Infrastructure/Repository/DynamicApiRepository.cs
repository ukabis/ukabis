using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Misc;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.Cache;
using JP.DataHub.ApiWeb.Core.Cache.Attributes;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Infrastructure.Models.Database;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using Accord.Math.Distances;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    internal class DynamicApiRepository : AbstractDynamicApiRepository, IDynamicApiRepository
    {
        private JPDataHubLogger log = new JPDataHubLogger(typeof(DynamicApiRepository));

        #region Cache
        [CacheKey(CacheKeyType.EntityWithKey, "vendor", "system", "controller")]
        public static string CACHE_KEY_IPFILTER_CONTROLLER = "DynamicApiRepository-IPFilter-Contoroller";

        [CacheKey(CacheKeyType.Id, "vendor_id", "system_id", "controller_id", "api_id")]
        public static string CACHE_KEY_IPFILTER_API = "DynamicApiRepository-IPFilter-Api";

        [CacheKey(CacheKeyType.Id, "vendor_id", "system_id", "controller_id", "api_id", "from_vendor_id", "from_system_id")]
        public static string CACHE_KEY_APIACCESSVENDOR = "DynamicApiRepository-ApiAccessVendor";

        [CacheKey(CacheKeyType.Id, "api_id", "open_id")]
        public static string CACHE_KEY_APIACCESSOPENID = "DynamicApiRepository-ApiAccessOpenid";

        [CacheKey(CacheKeyType.Id, "vendor_id", "controller_id", "api_id")]
        public static string CACHE_KEY_OPENID_ALLOWED_APPLICATIONS = "OpenId-Allowed-Applications";

        [CacheKey(CacheKeyType.Id, "vendor_id", "system_id", "controller_id")]
        public static string CACHE_KEY_RESOURCE_SHARING_RULE = "Resource-Sharing-Rule";

        [CacheKey(CacheKeyType.Id, "vendor_id", "controller_id")]
        public static string CACHE_KEY_API_MAILTEMPLATE = "DynamicApi:MailTemplate";

        [CacheKey(CacheKeyType.Id, "vendor_id", "controller_id")]
        public static string CACHE_KEY_API_WEBHOOK = "DynamicApi:Webhook";

        [CacheKey(CacheKeyType.Entity, "vendor")]
        public static string CACHE_KEY_GET_VENDOR = "DynamicApiRepository-GetVendor";

        [CacheKey(CacheKeyType.Id, "vendor_id", "controller_id", "vendor_controller_agreement_id")]
        public static string CACHE_KEY_IS_APPROVEDAGREEMENT = "DynamicApi:IsApprovedAgreement";

        [CacheKey(CacheKeyType.Id, "controller_id_for_physicalrepository")]
        public static string CACHE_KEY_CONTROLLER_PHYSICALREPOSITORY = "DynamicApi:Controller-PysicalRepository";

        [CacheKey(CacheKeyType.Id, "dataschema_for_controllerurl")]
        public static string CACHE_KEY_DATASCHEMA_CONTROLLERURL = "DynamicApi:DataSchema-ControllerUrl";

        [CacheKey(CacheKeyType.Id, "data_schema_id")]
        public static string CACHE_KEY_DATASCHEMA_SCHEMAID = "DynamicApi:DataSchema-SchemaId";

        [CacheKey(CacheKeyType.Id, "dataschema_for_schemaname")]
        public static string CACHE_KEY_DATASCHEMA_SCHEMANAME = "DynamicApi:DataSchema-SchemaName";

        [CacheKey(CacheKeyType.Id, "controller_id")]
        public static string CACHE_KEY_CONSENTREQUIRED_TERMASGROUP = "DynamicApi:ConsentRequiredTermasGroup";

        [CacheKey(CacheKeyType.Id, "terms_group_code", "open_id")]
        public static string CACHE_KEY_USERTERMS = "DynamicApi:UserTerms";

        [CacheKey(CacheKeyType.Id, "user_group_id", "open_id")]
        public static string CACHE_KEY_USER_RESOURCE_SHARE = "DynamicApi:UserResourceShare";

        #endregion

        #region LAZY
        protected IEnumerable<OpenIdAllowedApplication> FixedOpenIdAllowedApplication { get => _fixedOpenIdAllowedApplication.Value; }
        protected Lazy<IEnumerable<OpenIdAllowedApplication>> _fixedOpenIdAllowedApplication = new Lazy<IEnumerable<OpenIdAllowedApplication>>(() => FixedOpenIdAllowApplication());
        private static Lazy<IConfiguration> s_lazyConfig = new Lazy<IConfiguration>(() => UnityCore.Resolve<IConfiguration>());
        private static IConfiguration Config { get => s_lazyConfig.Value; }
        #endregion

        /// <summary>
        /// HttpメソッドタイプやURL（相対）から、合致するAPIを取得する
        /// </summary>
        /// <param name="httpMethodType">メソッドタイプ</param>
        /// <param name="requestRelativeUri">相対URL</param>
        /// <param name="getQuery">QUERY文字列（URLの?以降）</param>
        /// <returns></returns>
        public IMethod FindApi(HttpMethodType httpMethodType, RequestRelativeUri requestRelativeUri, GetQuery getQuery = null)
        {
            return _FindApi(httpMethodType, requestRelativeUri, getQuery, null);
        }

        /// <summary>
        /// ControllerId, ApiIdから合致するAPIを検索、API実行情報からデータ取得用Methodを作成する
        /// </summary>
        /// <param name="controllerId">Controller Id</param>
        /// <param name="apiId">Api Id</param>
        /// <param name="dataId">Data Id</param>
        /// <param name="contents">Content Body</param>
        /// <param name="queryString">QueryString</param>
        /// <param name="keyValue">KeyValue</param>
        /// <returns></returns>
        public IMethod FindApiForGetExecuteApiInfo(ControllerId controllerId, ApiId apiId, DataId dataId, Contents contents, QueryStringVO queryString, UrlParameter keyValue)
        {
            var method = _FindApiForGetExecuteApiInfo(controllerId, apiId);
            if (method == null)
            {
                return null;
            }
            if (method.ActionType.Value == ActionType.Regist)
            {
                // 登録なので、パーティションキーに必要なフィールドとidをkeyValueに設定、モデルにidを設定
                var keyValueForRegister = new Dictionary<UrlParameterKey, UrlParameterValue>();
                var data = JObject.Parse(contents.ReadToString());
                method.PartitionKey.LogicalKeys.ToList().ForEach(logicalKey =>
                {
                    if (data[logicalKey] != null)
                    {
                        keyValueForRegister.Add(
                            new UrlParameterKey(logicalKey),
                            new UrlParameterValue(data[logicalKey].ToObject<string>())
                        );
                    }
                });

                // Idに設定
                keyValueForRegister.Add(
                    new UrlParameterKey("id"),
                    new UrlParameterValue(dataId.Value)
                );
                method.ControllerSchema = new DataSchema(@"
{
  'properties': {
    id: {
      'type': 'string',
    },
  },
  'type': 'object'
}
");
                method.KeyValue = new UrlParameter(keyValueForRegister);
            }
            else
            {
                // 更新、削除なのでQueryとkeyValueの復元でよいはず
                method.Query = queryString;
                method.KeyValue = keyValue;
            }

            // データ取得を設定
            method.ActionType = new ActionTypeVO(ActionType.Query);
            // スクリプトは動かさない
            method.Script = new Script("");
            method.ScriptType = null;

            return method;
        }

        /// <summary>
        /// HttpメソッドタイプやURL（相対）から、合致するAPIを取得、遅延読込用APIとして返す
        /// </summary>
        /// <param name="httpMethodType">メソッドタイプ</param>
        /// <param name="requestRelativeUri">相対URL</param>
        /// <param name="getQuery">QUERY文字列（URLの?以降）</param>
        /// <returns></returns>
        public IMethod FindEnumerableApi(HttpMethodType httpMethodType, RequestRelativeUri requestRelativeUri, GetQuery getQuery = null)
        {
            var method = _FindApi(httpMethodType, requestRelativeUri, getQuery, null);
            if (method?.ActionType.Value != ActionType.Query)
            {
                return null;
            }
            method.ActionType = new ActionTypeVO(ActionType.EnumerableQuery);
            return method;
        }

        #region FindApiから呼ばれるメソッド
        /// <summary>
        /// HttpメソッドタイプやURL（相対）から、合致するAPIを取得する
        /// </summary>
        /// <param name="httpMethodType">メソッドタイプ</param>
        /// <param name="requestRelativeUri">相対URL</param>
        /// <param name="getQuery">QUERY文字列（URLの?以降）</param>
        /// <param name="exclusiveApiId">APIリストから除外するAPIID</param>
        /// <returns>見つけた場合はMethodモデルを返す</returns>
        private IMethod _FindApi(
            HttpMethodType httpMethodType,
            RequestRelativeUri requestRelativeUri,
            GetQuery getQuery = null,
            List<Guid> exclusiveApiId = null)
        {
            // リソース毎のAdaptResourceSchemが呼び出された場合は、データ構造が変わって呼び出されているので
            // キャッシュを消す（それによって/API/xxx/AdaptResourceSchemaのAPIが見つかることになり処理が継続できる）
            if (requestRelativeUri.Value.EndsWith("/AdaptResourceSchema"))
            {
                RefreshStaticCache(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff")).Wait();
            }

            var api = FindApiEntityByUrlFast(httpMethodType, requestRelativeUri, getQuery, exclusiveApiId);
            if (api == null)
            {
                return null;
            }
            string normalizedRelativeUri = requestRelativeUri.Value.NormalizeUrlRelative();
            string[] splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);
            return ApiEntryToMethod(api, httpMethodType, normalizedRelativeUri, getQuery?.Value, splitUrl);
        }

        private IMethod _FindApiForGetExecuteApiInfo(ControllerId controllerId, ApiId apiId)
        {
            if (controllerId.ToGuid == null)
            {
                return null;
            }
            var targetControllerId = (Guid)controllerId.ToGuid;

            if (apiId.ToGuid == null)
            {
                return null;
            }
            var targetApiId = (Guid)apiId.ToGuid;

            List<AllApiEntity> api_list = GetControllerApiWithVersionApi(targetControllerId);
            var result = api_list.FirstOrDefault(x => (x.api_id == targetApiId));
            if (result == null)
            {
                return null;
            }

            return ApiEntryToMethod(result, new HttpMethodType(HttpMethodType.MethodTypeEnum.GET), "", null, new string[] { });
        }

        private IMethod ApiEntryToMethod(AllApiEntity api, HttpMethodType httpMethodType, string urlRelative, string query, string[] splitRelative)
        {
            Dictionary<string, string> keyValue = UriUtil.IsMatchUrlWithoutQueryString(splitRelative, api.controller_relative_url, api.method_name);
            if (keyValue == null)
            {
                keyValue = UriUtil.IsMatchUrlWithoutQueryString(splitRelative, api.controller_relative_url, api.alias_method_name);
            }
            if (keyValue == null)
            {
                keyValue = new Dictionary<string, string>();
            }

            IMethod method = UnityCore.Resolve<IMethod>();

            method.VendorId = new VendorId(api.vendor_id.ToString());
            method.SystemId = new SystemId(api.system_id.ToString());
            method.ApiId = new ApiId(api.api_id.ToString());
            method.ControllerId = new ControllerId(api.controller_id.ToString());
            method.ControllerRelativeUrl = new ControllerUrl(api.controller_relative_url);
            method.IsUseBlobCache = new IsUseBlobCache(api.is_use_blob_cache);
            method.MethodType = httpMethodType;
            method.RequestSchema = new DataSchema(api.request_schema);
            method.UriSchema = new DataSchema(api.url_schema);
            method.ControllerSchema = new DataSchema(api.controller_schema);
            method.ResponseSchema = new DataSchema(api.response_schema);
            method.IsHeaderAuthentication = new IsHeaderAuthentication(api.is_header_authentication);
            method.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(api.all_repository_model_list.Select(
                allRepositoryModel => new RepositoryInfo(allRepositoryModel.repository_group_id, allRepositoryModel.repository_type_cd,
                allRepositoryModel.ToTuple())).ToList());
            method.IsHeaderAuthentication = new IsHeaderAuthentication(api.is_header_authentication);
            method.IsVendorSystemAuthenticationAllowNull = new IsVendorSystemAuthenticationAllowNull(api.is_vendor_system_authentication_allow_null);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(api.is_clientcert_authentication);
            method.IsOpenIdAuthentication = new IsOpenIdAuthentication(api.is_openid_authentication);
            method.IsAdminAuthentication = new IsAdminAuthentication(api.is_admin_authentication);
            method.RepositoryKey = new RepositoryKey(api.controller_repository_key);
            method.ControllerRepositoryKey = new RepositoryKey(api.controller_repository_key);
            method.KeyValue = new UrlParameter(keyValue.Select(x => new { Key = new UrlParameterKey(x.Key), Value = new UrlParameterValue(x.Value) }).ToDictionary(key => key.Key, value => value.Value));
            method.PostDataType = new PostDataType(api.post_data_type);
            method.RelativeUri = new RelativeUri(urlRelative);
            method.ApiUri = new ApiUri(api.method_name);
            method.IsVendor = new IsVendor(api.is_vendor);
            method.IsPerson = new IsPerson(api.is_person);
            method.GatewayInfo = new GatewayInfo(api.gateway_url, api.gateway_credential_username, api.gateway_credential_password, api.gateway_relay_header);
            var container = UnityCore.Resolve<IPerRequestDataContainer>();
            Guid tempSystemId = Guid.Empty;
            Guid tempVendorId = Guid.Empty;
            Guid.TryParse(container.VendorId, out tempVendorId);
            Guid.TryParse(container.SystemId, out tempSystemId);

            method.IsOverPartition = new IsOverPartition(api.is_over_partition);
            method.ApiQuery = new ApiQuery(api.query);
            method.QueryType = QueryType.Parse(api.query_type_cd);
            method.Script = new Script(api.script);
            method.ActionType = api.action_type_cd.ToActionTypeVO();
            method.ScriptType = api.script_type_cd.ToScriptTypeVO();
            method.CacheInfo = new CacheInfo(api.is_cache, api.cache_minute, api.cache_key);

            // データ共有ありの場合。参照のみ許可
            if (container.XResourceSharingWith != null && !tempVendorId.Equals(Guid.Empty) && !tempSystemId.Equals(Guid.Empty) && IsAllowedActionTypeForResourseSharing(method.ActionType.Value))
            {
                // 非同期呼び出しの際に、PerRequestDataContainerがAutoMapperでMapした際にDictionaryがnullでなくなるときの回避
                if (container.XResourceSharingWith.ContainsKey("VendorId") && container.XResourceSharingWith.ContainsKey("SystemId"))
                {
                    method.ApiResourceSharing = GetResourceSharingRules(new Guid(container.XResourceSharingWith["VendorId"]), new Guid(container.XResourceSharingWith["SystemId"]), api.controller_id, tempVendorId, tempSystemId);
                }
            }

            // プライベートデータ共有の場合
            if (!string.IsNullOrEmpty(container.XResourceSharingPerson))
            {
                var apiUrl = api.controller_relative_url + "/" + api.method_name;
                method.ResourceSharingPersonRules = GetPersonResourceSharingRules(apiUrl, container.XResourceSharingPerson, container.OpenId, container.VendorId, container.SystemId);
            }


            method.IsAccesskey = new IsAccesskey(api.is_accesskey);
            method.IsAutomaticId = new IsAutomaticId(api.is_automatic_id);
            method.ActionTypeVersion = new ActionTypeVersion(api.actiontype_version);
            method.ActionInjectorHandler = api.ActionInjector == null ? null : new ActionInjectorHandler(api.ActionInjector);
            method.PartitionKey = new PartitionKey(api.partition_key);

            Dictionary<QueryStringKey, QueryStringValue> q = new Dictionary<QueryStringKey, QueryStringValue>();
            if (method.ActionType?.Value == ActionType.OData || method.ActionType?.Value == ActionType.ODataDelete || method.ActionType?.Value == ActionType.ODataPatch)
            {
                q = UriUtil.SplitODataQuery(query).Select(x => new { Key = new QueryStringKey(x.Key), Value = new QueryStringValue(x.Value) }).ToDictionary(key => key.Key, value => value.Value);
            }
            else if (query != null && string.IsNullOrEmpty(query) == false)
            {
                q = UriUtil.ParseQueryString(query).Select(x => new { Key = new QueryStringKey(x.Key), Value = new QueryStringValue(x.Value) }).ToDictionary(key => key.Key, value => value.Value);
            }
            if (keyValue != null && keyValue.Count > 0)
            {
                foreach (var k in keyValue)
                {
                    if (q.Where(x => x.Key.Value == k.Key).Count() == 0)
                    {
                        q.Add(new QueryStringKey(k.Key), new QueryStringValue(k.Value));
                    }
                }
            }
            method.Query = q.Count() == 0 ? null : new QueryStringVO(q, query);

            bool allowAsync = false;
            bool.TryParse(Config.GetValue<string>("AppConfig:AllowAsync"), out allowAsync);
            if (allowAsync && container.XAsync)
            {
                method.AsyncOriginalActionType = new ActionTypeVO(method.ActionType.Value);
                method.ActionType = new ActionTypeVO(ActionType.Async);
                method.ActionTypeVersion = new ActionTypeVersion(1);
            }
            method.IsEnableAttachFile = new IsEnableAttachFile(api.is_enable_attachfile);
            method.InternalOnly = new InternalOnly(api.is_internal_call_only, api.internal_call_keyword);
            var attachfileStorageRepository = api.attachfile_blob_repository_model;
            if (attachfileStorageRepository != null)
            {
                method.AttachFileBlobRepositoryInfo = new RepositoryInfo(attachfileStorageRepository.repository_type_cd, attachfileStorageRepository.physical_repository_list.ToDictionary(x => x.repository_connection_string, x => x.is_full));
                method.AttachFileBlobRepositoryGroupId = new RepositoryGroupId(attachfileStorageRepository.repository_group_id?.ToString());
            }
            method.IsSkipJsonSchemaValidation = new IsSkipJsonSchemaValidation(api.is_skip_jsonschema_validation);
            method.IsOpenidAuthenticationAllowNull = new IsOpenidAuthenticationAllowNull(api.is_openid_authentication_allow_null);
            var dtu = container.GetDateTimeUtil();
            method.PublicDate = new PublicDate(dtu.ParseDateTimeNull(api.public_start_datetime), dtu.ParseDateTimeNull(api.public_end_datetime));
            method.IsOptimisticConcurrency = new IsOptimisticConcurrency(api.is_optimistic_concurrency);
            method.IsEnableBlockchain = new IsEnableBlockchain(api.is_enable_blockchain);
            method.IsDocumentHistory = new IsDocumentHistory(api.is_document_history);
            method.IsVisibleAgreement = new IsVisibleAgreement(api.is_visible_agreement);
            method.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(api.is_container_dynamic_separation);
            method.IsOtherResourceSqlAccess = new IsOtherResourceSqlAccess(api.is_otherresource_sqlaccess);
            if (api.history_repository_model != null)
            {
                //method.DocumentHistoryRepositoryInfo = new RepositoryInfo(api.history_repository_model.repository_type_cd, api.history_repository_model.physical_repository_list.ToDictionary(x => x.repository_connection_string, x => x.is_full), api.history_repository_model.repository_group_id);
                method.DocumentHistoryRepositoryInfo = new RepositoryInfo(api.history_repository_model.repository_group_id.Value, api.history_repository_model.repository_type_cd, api.history_repository_model.ToTuple());
            }
            method.IsTransparentApi = new IsTransparentApi(api.is_transparent_api);
            method.IsEnableResourceVersion = new IsEnableResourceVersion(api.is_enable_resource_version && EnableResourceVersion);
            method.IsRequireConsent = new IsRequireConsent(api.is_require_consent);
            method.TermsGroupCode = new TermsGroupCode(api.terms_group_code);
            method.ResourceGroupId = new ResourceGroupId(api.resource_group_id);

            return method;
        }
        #endregion

        #region IpFilter関連
        public IEnumerable<IpAddress> GetIpFilter(VendorId vendorId, SystemId systemId, ControllerId controllerId, ApiId apiId)
        {
            var enableIpFilter = UnityCore.Resolve<bool>("EnableIpFilter");
            if (enableIpFilter == false)
            {
                yield break;
            }

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
    SELECT
        ip_address
    FROM
        CONTROLLER_IP_FILTER contipfil
        INNER JOIN CONTROLLER cont on contipfil.controller_id = cont.controller_id
    WHERE
        contipfil.controller_id = /*ds controller_id*/'id' and cont.is_enable_ipfilter = 1 and contipfil.is_active= 1 and contipfil.is_enable= 1
UNION
    SELECT
        ip_address
    FROM
        COMMON_IP_FILTER cf 
        INNER JOIN COMMON_IP_FILTER_GROUP cifg on cf.common_ip_filter_group_id = cifg.common_ip_filter_group_id and cifg.is_active = 1
        INNER JOIN CONTROLLER_COMMON_IP_FILTER_GROUP ccifg on ccifg.common_ip_filter_group_id = cifg.common_ip_filter_group_id and ccifg.is_active = 1 and ccifg.controller_id = /*ds controller_id*/'id' 
        INNER JOIN CONTROLLER cont on ccifg.controller_id = cont.controller_id
    WHERE
        cont.is_enable_ipfilter = 1 and cf.is_active = 1 and cf.is_enable = 1
UNION
    SELECT
        ip_address
    FROM
        API_IP_FILTER
    WHERE
        api_id = /*ds api_id*/'id' and is_active = 1 and is_enable= 1
";
            }
            else
            {
                sql = @"
    SELECT
        ip_address
    FROM
        ControllerIpFilter as contipfil
        INNER JOIN Controller as cont on contipfil.controller_id = cont.controller_id
    WHERE
        contipfil.controller_id = @controller_id and cont.is_enable_ipfilter = 1 and contipfil.is_active= 1 and contipfil.is_enable= 1
UNION
    SELECT
        ip_address
    FROM
        CommonIpFilter cf 
        INNER JOIN CommonIpFilterGroup cifg on cf.common_ip_filter_group_id = cifg.common_ip_filter_group_id and cifg.is_active = 1
        INNER JOIN ControllerCommonIpFilterGroup ccifg on ccifg.common_ip_filter_group_id = cifg.common_ip_filter_group_id and ccifg.is_active = 1 and ccifg.controller_id = @controller_id
        INNER JOIN Controller as cont on ccifg.controller_id = cont.controller_id
    WHERE
        cont.is_enable_ipfilter = 1 and cf.is_active = 1 and cf.is_enable = 1
UNION
    SELECT
        ip_address
    FROM
        ApiIpFilter
    WHERE
        api_id = @api_id and is_active = 1 and is_enable= 1
";
            }

            string cacheKey = CacheManager.CreateKey(CACHE_KEY_IPFILTER_CONTROLLER, vendorId.Value, systemId.Value, controllerId.Value, apiId.Value);
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("controller_id", controllerId.Value);
            twowaySqlParam.Add("api_id", apiId.Value);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var result = Cache.Get<IEnumerable<string>>(cacheKey, CacheExpirationTimeSpan, () =>
                DbConnection.Query<string>(twowaySql.Sql, dynParams));
            foreach (var val in result.Select(x => new IpAddress(x)))
            {
                yield return val;
            }
        }
        #endregion

        public ApiAccessVendor GetApiAccessVendor(VendorId vendorId, SystemId systemId, ControllerId controllerId, ApiId apiId, VendorId targetVendorId, SystemId targetSystemId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = "select vendor_id,system_id,access_key from api_access_vendor where api_id = /*ds apiId*/'id' and vendor_id = /*ds vendorid*/'id' and system_id = /*ds systemid*/'id' and is_active = 1 and is_enable = 1";
            }
            else
            {
                sql = "select vendor_id,system_id,access_key from ApiAccessVendor where api_id = @apiId and vendor_id = @vendorid and system_id = @systemid and is_active = 1 and is_enable = 1";
            }

            string cacheKey = CacheManager.CreateKey(CACHE_KEY_APIACCESSVENDOR, vendorId.Value, systemId.Value, controllerId.Value, apiId.Value, targetVendorId.Value, targetSystemId.Value);
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("apiId", apiId.Value);
            twowaySqlParam.Add("vendorid", targetVendorId.Value);
            twowaySqlParam.Add("systemid", targetSystemId.Value);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var result = Cache.Get<DB_ApiAccessVendorSelect>(cacheKey, CacheExpirationTimeSpan, () =>
                DbConnection.Query<DB_ApiAccessVendorSelect>(twowaySql.Sql, dynParams).FirstOrDefault());

            if (result != null)
            {
                return new ApiAccessVendor()
                {
                    AccessKey = new ApiAccessKey(result.access_key),
                    VendorId = new VendorId(result.vendor_id.ToString()),
                    SystemId = new SystemId(result.system_id.ToString()),
                };
            }

            return null;
        }

        /// <summary>
        /// OpenId認証を許可するアプリケーションの一覧を取得します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="controllerId">コントローラーID</param>
        /// <param name="apiId">アプリケーションID</param>
        /// <returns>OpenId認証を許可するアプリケーションの一覧</returns>
        public IEnumerable<OpenIdAllowedApplication> GetOpenIdAllowedApplications(VendorId vendorId, ControllerId controllerId, ApiId apiId)
        {
            if (FixedOpenIdAllowedApplication?.Any() == true)
            {
                return FixedOpenIdAllowedApplication;
            }
            return GetOpenIdAllowedApplicationsFromDB(vendorId, controllerId, apiId);
        }

        private static IEnumerable<OpenIdAllowedApplication> FixedOpenIdAllowApplication()
        {
            var list = UnityCore.Resolve<string>("FixedOpenIdAllowedApplication")?.Split(',').Select(x => x.To<Guid?>()?.ToString()).Where(x => x != null).ToList() ?? new List<string>();
            return list.Select(x => new OpenIdAllowedApplication(x)).ToList();
        }

        public IEnumerable<OpenIdAllowedApplication> GetOpenIdAllowedApplicationsFromDB(VendorId vendorId, ControllerId controllerId, ApiId apiId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"SELECT application_id FROM (
SELECT DISTINCT application_id, FIRST_VALUE(access_control)
  OVER (PARTITION BY application_id ORDER BY priority) AS access_control FROM (
SELECT application_id, access_control, 1 AS priority FROM API_OPEN_ID_CA
 WHERE api_id = /*ds apiId*/'id' and is_active = 1 and access_control != 'inh'
UNION
SELECT application_id, access_control, 2 AS priority FROM CONTROLLER_OPEN_ID_CA
 WHERE controller_id = /*ds controllerId*/'id' and is_active = 1 and access_control != 'inh'
UNION
SELECT application_id, access_control, 3 AS priority FROM VENDOR_OPEN_ID_CA
 WHERE vendor_id = /*ds vendorId*/'id' and is_active = 1 and access_control != 'inh'
) a) b
WHERE b.access_control = 'alw'";
            }
            else
            {
                sql = @"SELECT application_id FROM (
SELECT DISTINCT application_id, FIRST_VALUE(access_control)
  OVER (PARTITION BY application_id ORDER BY priority) AS access_control FROM (
SELECT application_id, access_control, 1 AS priority FROM ApiOpenIdCA
 WHERE api_id = @apiId and is_active = 1 and access_control != 'inh'
UNION
SELECT application_id, access_control, 2 AS priority FROM ControllerOpenIdCA
 WHERE controller_id = @controllerId and is_active = 1 and access_control != 'inh'
UNION
SELECT application_id, access_control, 3 AS priority FROM VendorOpenIdCA
 WHERE vendor_id = @vendorId and is_active = 1 and access_control != 'inh'
) a) b
WHERE b.access_control = 'alw'";
            }

            string cacheKey = CacheManager.CreateKey(CACHE_KEY_OPENID_ALLOWED_APPLICATIONS, vendorId.Value, controllerId.Value, apiId.Value);
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("apiId", apiId.Value);
            twowaySqlParam.Add("controllerId", controllerId.Value);
            twowaySqlParam.Add("vendorId", vendorId.Value);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var result = Cache.Get<IEnumerable<Guid>>(cacheKey, CacheExpirationTimeSpan, () =>
            {
                var param = new { vendorId = vendorId.Value, controllerId = controllerId.Value, apiId = apiId.Value };
                return DbConnection.Query<Guid>(twowaySql.Sql, dynParams);
            });

            return result.Count() > 0 ? result.Select(appId => new OpenIdAllowedApplication(appId.ToString())).ToList() : null;
        }

        #region ResourceSharing関連
        private ApiResourceSharing GetResourceSharingRules(Guid sharingFromVendorId, Guid sharingFromSystemId, Guid controllerId, Guid sharingToVendorId, Guid sharingToSystemId)
        {
            var dbResult = _GetResourceSharingRules(sharingFromVendorId, sharingFromSystemId, controllerId);

            var resoureSharing = new ApiResourceSharing(new ControllerId(controllerId.ToString()),
                new VendorId(sharingFromVendorId.ToString()),
                new SystemId(sharingFromSystemId.ToString()),
                new List<ApiResourceSharingRule>() { });

            if (dbResult.Any())
            {
                var ruleList = new List<ApiResourceSharingRule>() { };
                dbResult.Where(x => x.sharing_to_vendor_id.Equals(sharingToVendorId) && x.sharing_to_system_id.Equals(sharingToSystemId) && x.is_enable == true && !string.IsNullOrEmpty(x.query)).ToList().ForEach(x => ruleList.Add(new ApiResourceSharingRule(
                    sharingToVendorId: x.sharing_to_vendor_id,
                    sharingToSystemId: x.sharing_to_system_id,
                    resourceSharingRuleName: x.resource_sharing_rule_name,
                    query: x.query,
                    roslynScript: x.roslyn_script,
                    isEnable: x.is_enable)));
                resoureSharing.ResourceSharingRuleList = ruleList;
            }
            return resoureSharing;
        }

        private List<DB_ResourceSharingRule> _GetResourceSharingRules(Guid vendorId, Guid systemId, Guid controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = $"Select * from RESOURCE_SHARING_RULE Where controller_id = /*ds controller_id*/'id' And sharing_from_vendor_id = /*ds sharing_from_vendor_id*/'id' And sharing_from_system_id = /*ds sharing_from_system_id*/'id' And is_active = 1 order by reg_date";
            }
            else
            {
                sql = $"Select * from ResourceSharingRule Where controller_id = @controller_id And sharing_from_vendor_id = @sharing_from_vendor_id And sharing_from_system_id = @sharing_from_system_id And is_active = 1 order by reg_date";
            }
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("controller_id", controllerId);
            twowaySqlParam.Add("sharing_from_vendor_id", vendorId);
            twowaySqlParam.Add("sharing_from_system_id", systemId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            return Cache.Get<List<DB_ResourceSharingRule>>(GetResourceSharingRule_Key(vendorId, systemId, controllerId), CacheExpirationTimeSpan,
                () => DbConnection.Query<DB_ResourceSharingRule>(twowaySql.Sql, dynParams).ToList());
        }

        private string GetResourceSharingRule_Key(Guid vendorId, Guid systemId, Guid controllerId) => CacheManager.CreateKey(CACHE_KEY_RESOURCE_SHARING_RULE, vendorId, systemId, controllerId);

        private bool IsAllowedActionTypeForResourseSharing(ActionType type)
        {
            return type == ActionType.Query || type == ActionType.OData || type == ActionType.AttachFileDownload;
        }
        #endregion

        #region PersonResourceSharing関連
        private List<ResourceSharingPersonRule> GetPersonResourceSharingRules(string api_url, string sharingFromUserId, string sharingToUserId, string sharingVendorId = null, string sharingSystemId = null)
        {
            var dbResult = _GetResourceSharingPersonRule(sharingFromUserId, sharingToUserId, sharingVendorId, sharingSystemId);
            var resourceSharingPerson = new List<ResourceSharingPersonRule> { };

            if (dbResult.Any())
            {
                dbResult.Where(x => !string.IsNullOrEmpty(x.resource_path) && CheckUrlPattern(api_url, x.resource_path))
                    .ToList().ForEach(x => resourceSharingPerson.Add(new ResourceSharingPersonRule(
                        new ResourceSharingPersonRuleId(x.resource_sharing_person_rule_id.ToString()),
                        x.resource_sharing_rule_name,
                        new RelativeUri(x.resource_path),
                        x.sharing_from_user_id.HasValue ? new OpenId(x.sharing_from_user_id.ToString()) : null,
                        x.sharing_from_mail_address,
                        x.sharing_to_user_id.HasValue ? new OpenId(x.sharing_to_user_id.ToString()) : null,
                        x.sharing_to_mail_address,
                        new ApiQuery(x.query),
                        new Script(x.script),
                        new IsActive(x.is_active),
                        x.sharing_to_vendor_id,
                        x.sharing_to_system_id)));
            }
            return resourceSharingPerson;
        }

        private bool CheckUrlPattern(string url, string pattern)
        {
            var sb = new System.Text.StringBuilder("^");
            sb.Append(pattern.Replace("*", ".*"));
            sb.Append("$");
            return System.Text.RegularExpressions.Regex.IsMatch(url, sb.ToString());
        }

        private List<DB_ResourceSharingPersonRule> _GetResourceSharingPersonRule(string sharingFromUserId, string sharingToUserId, string sharingVendorId = null, string sharingSystemId = null)
        {

            /*
             * 取得する対象は以下2件
             * 1. Person to Person (openidがfrom-toで完全一致)
             * 2. Any to Vendor (openidに関する指定がない AND toベンダーが指定されている)
             */
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    resource_sharing_person_rule_id,
    resource_sharing_rule_name,
    resource_path,
    sharing_from_user_id,
    sharing_from_mail_address,
    sharing_to_user_id,
    sharing_to_mail_address,
    query,
    script,
    is_enable,
    reg_date,
    reg_username,
    upd_date,
    upd_username,
    is_active,
    sharing_to_vendor_id,
    sharing_to_system_id
FROM RESOURCE_SHARING_PERSON_RULE
WHERE
( 
  ( sharing_from_user_id = /*ds sharing_from_user_id*/'id'  AND sharing_to_user_id = /*ds sharing_to_user_id*/'id' )
  OR ( 
        (sharing_from_user_id is null AND sharing_from_mail_address = '*')
        AND (sharing_to_user_id is null AND sharing_to_mail_address = '*')
        AND sharing_to_vendor_id = /*ds sharing_to_vendor_id*/'id'  
        AND sharing_to_system_id = /*ds sharing_to_system_id*/'id' 
    )
)
AND is_active = 1 
AND is_enable = 1 
ORDER BY reg_date";
            }
            else
            {
                sql = @"
SELECT 
    resource_sharing_person_rule_id,
    resource_sharing_rule_name,
    resource_path,
    sharing_from_user_id,
    sharing_from_mail_address,
    sharing_to_user_id,
    sharing_to_mail_address,
    query,
    script,
    is_enable,
    reg_date,
    reg_username,
    upd_date,
    upd_username,
    is_active,
    sharing_to_vendor_id,
    sharing_to_system_id
FROM ResourceSharingPersonRule
WHERE
( 
  ( sharing_from_user_id = @sharing_from_user_id  AND sharing_to_user_id = @sharing_to_user_id )
  OR ( 
        (sharing_from_user_id is null AND sharing_from_mail_address = '*')
        AND (sharing_to_user_id is null AND sharing_to_mail_address = '*')
        AND sharing_to_vendor_id = @sharing_to_vendor_id  
        AND sharing_to_system_id = @sharing_to_system_id 
    )
)
AND is_active = 1 
AND is_enable = 1 
ORDER BY reg_date";
            }

            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("sharing_from_user_id", sharingFromUserId);
            twowaySqlParam.Add("sharing_to_user_id", sharingToUserId);
            twowaySqlParam.Add("sharing_to_vendor_id", sharingVendorId);
            twowaySqlParam.Add("sharing_to_system_id", sharingSystemId);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            return DbConnection.Query<DB_ResourceSharingPersonRule>(twowaySql.Sql, dynParams)
                .ToList();
        }

        #endregion

        #region MailTemplate関連
        /// <summary>
        /// 指定されたAPI、ベンダーにメールテンプレートが設定されているか返します。
        /// </summary>
        /// <param name="controllerId">コントローラーID</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>メールテンプレートが設定されているかどうか</returns>
        public bool HasMailTemplate(ControllerId controllerId, VendorId vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"--HasMailTemplate
SELECT COUNT(*) FROM CONTROLLER_MAIL_TEMPLATE cmt
JOIN VENDOR v ON v.vendor_id = cmt.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE cmt.controller_id = /*ds controllerId*/'id' AND cmt.is_active = 1
AND cmt.vendor_id = /*ds vendorId*/'id' ";
            }
            else
            {
                sql = @"--HasMailTemplate
SELECT COUNT(*) FROM ControllerMailTemplate cmt
JOIN Vendor v ON v.vendor_id = cmt.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE cmt.controller_id = @controllerId AND cmt.is_active = 1
AND cmt.vendor_id = @vendorId";
            }
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("controllerId", controllerId.Value);
            twowaySqlParam.Add("vendorId", vendorId.Value);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            try
            {
                string cacheKey = CreateApiMailTemplateCacheKey(vendorId, controllerId);
                var result = Cache.Get<bool?>(cacheKey, CacheExpirationTimeSpan, () =>
                {
                    object param = new { controllerId = controllerId.Value, vendorId = vendorId.Value };
                    return DbConnection.QuerySingle<int>(twowaySql.Sql, dynParams) > 0;
                });
                // 結果を返却
                return result.Value;
            }
            catch (SqlException ex)
            {
                log.Error($"Sql Error: {ex.Number} {ex.Message}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        /// <summary>
        /// APIメールテンプレートのキャッシュキーを作成します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="controllerId">コントローラーID</param>
        /// <returns>キャッシュキー</returns>
        public static string CreateApiMailTemplateCacheKey(VendorId vendorId, ControllerId controllerId)
            => CacheManager.CreateKey(CACHE_KEY_API_MAILTEMPLATE, vendorId.Value, controllerId.Value);
        #endregion

        #region Webhook関連
        /// <summary>
        /// 指定されたAPI、ベンダーにWebhookが設定されているか返します。
        /// </summary>
        /// <param name="controllerId">コントローラーID</param>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>Webhookが設定されているかどうか</returns>
        public bool HasWebhook(ControllerId controllerId, VendorId vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"--HasWebhook
SELECT COUNT(*) FROM CONTROLLER_WEBHOOK cwf
JOIN VENDOR v ON v.vendor_id = cwf.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE cwf.controller_id = /*ds controllerId*/'id' AND cwf.is_active = 1
AND cwf.vendor_id = /*ds vendorId*/'id' ";
            }
            else
            {
                sql = @"--HasWebhook
SELECT COUNT(*) FROM ControllerWebhook cwf
JOIN Vendor v ON v.vendor_id = cwf.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE cwf.controller_id = @controllerId AND cwf.is_active = 1
AND cwf.vendor_id = @vendorId";
            }
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("controllerId", controllerId.Value);
            twowaySqlParam.Add("vendorId", vendorId.Value);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            try
            {
                string cacheKey = CreateApiWebhookCacheKey(vendorId, controllerId);
                var result = Cache.Get<bool?>(cacheKey, CacheExpirationTimeSpan, () =>
                {
                    object param = new { controllerId = controllerId.Value, vendorId = vendorId.Value };
                    return DbConnection.QuerySingle<int>(twowaySql.Sql, dynParams) > 0;
                });
                // 結果を返却
                return result.Value;
            }
            catch (SqlException ex)
            {
                log.Error($"Sql Error: {ex.Number} {ex.Message}", ex);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        /// <summary>
        /// API Webhookのキャッシュキーを作成します。
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <param name="controllerId">コントローラーID</param>
        /// <returns>キャッシュキー</returns>
        public static string CreateApiWebhookCacheKey(VendorId vendorId, ControllerId controllerId)
            => CacheManager.CreateKey(CACHE_KEY_API_WEBHOOK, vendorId.Value, controllerId.Value);
        #endregion

        public bool HasApiAccessOpenid(ApiId apiId, OpenId openId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = "SELECT COUNT(*) FROM API_ACCESS_OPEN_ID WHERE api_id=/*ds apiId*/'id' AND open_id=/*ds openId*/'id'  AND is_enable=1AND is_active=1";
            }
            else
            {
                sql = "SELECT COUNT(*) FROM ApiAccessOpenid WHERE api_id=@apiId AND open_id=@openId  AND is_enable=1AND is_active=1";
            }
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("apiId", apiId.Value);
            twowaySqlParam.Add("openId", openId.Value);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            string cacheKey = CacheManager.CreateKey(CACHE_KEY_APIACCESSOPENID, apiId.Value, openId.Value);
            var result = Cache.Get<int?>(cacheKey, CacheExpirationTimeSpan, () =>
                DbConnection.QuerySingle<int>(twowaySql.Sql, dynParams));
            return result > 0;
        }

        /// <summary>
        /// 指定された時間のデータが存在するブロックチェーンノードを取得する
        /// 引数がない または対象のノードが存在しない場合は最新のブロックチェーンノードを取得する
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public RepositoryInfo GetBlockchainNodeRepositoryInfoByTimeStamp(DateTime? timestamp = null)
        {
            //引数の時刻から取得できなくても 少なくとも最新のブロックチェーンノードを返却する
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
 repository_connection_string
 ,is_full
FROM (
    select * from (
    SELECT 
    pr.connection_string AS repository_connection_string
    ,pr.is_full AS is_full
    ,pr.blockchain_latest_datetime AS latest_datetime
    FROM REPOSITORY_GROUP rg 
    INNER JOIN PHYSICAL_REPOSITORY pr 
    ON rg.repository_group_id = pr.repository_group_id and rg.is_active = 1 and pr.is_active = 1
    WHERE rg.repository_type_cd = 'bcn'
    AND pr.blockchain_latest_datetime > /*ds targettime*/'2000-01-01' 
    ORDER BY latest_datetime ASC
    FETCH FIRST 1 ROWS ONLY)
    UNION 
    SELECT 
    pr.connection_string AS repository_connection_string
    ,pr.is_full AS is_full
    ,pr.blockchain_latest_datetime AS latest_datetime
    FROM REPOSITORY_GROUP rg 
    INNER JOIN PHYSICAL_REPOSITORY pr 
    ON rg.repository_group_id = pr.repository_group_id and rg.is_active = 1 and pr.is_active = 1
    WHERE rg.repository_type_cd = 'bcn'
    AND pr.blockchain_latest_datetime is null
    FETCH FIRST 1 ROWS ONLY
) tbl
ORDER BY latest_datetime DESC
";
            }
            else
            {
                sql = @"
SELECT 
 repository_connection_string
 ,is_full
FROM (
    SELECT TOP 1
    pr.connection_string AS repository_connection_string
    ,pr.is_full AS is_full
    ,pr.blockchain_latest_datetime AS latest_datetime
    FROM RepositoryGroup rg 
    INNER JOIN PhysicalRepository pr 
    ON rg.repository_group_id = pr.repository_group_id and rg.is_active = 1 and pr.is_active = 1
    WHERE rg.repository_type_cd = 'bcn'
    AND pr.blockchain_latest_datetime > @targettime
    ORDER BY latest_datetime ASC
    UNION 
    SELECT TOP 1
    pr.connection_string AS repository_connection_string
    ,pr.is_full AS is_full
    ,pr.blockchain_latest_datetime AS latest_datetime
    FROM RepositoryGroup rg 
    INNER JOIN PhysicalRepository pr 
    ON rg.repository_group_id = pr.repository_group_id and rg.is_active = 1 and pr.is_active = 1
    WHERE rg.repository_type_cd = 'bcn'
    AND pr.blockchain_latest_datetime is null
) AS tbl
ORDER BY latest_datetime DESC
";
            }

            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("targettime", timestamp);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            string cacheKey = CacheManager.CreateKey("DynamicApiRepository-GetBlockchainNodeRepositoryInfoByTimeStamp");
            var sqlResult = Cache.Get<AllApiPhysicalRepositoryModel>(cacheKey, CacheExpirationTimeSpan, () =>
            {
                return DbConnection.Query<AllApiPhysicalRepositoryModel>(twowaySql.Sql, dynParams).First();
            });
            if (sqlResult == null)
            {
                return null;
            }
            return new RepositoryInfo(RepositoryType.BlockchainNode.ToCode(), new Dictionary<string, bool>() { { sqlResult.repository_connection_string, sqlResult.is_full } });
        }

        public VendorVO GetVendor(VendorId vendorId)
        {
            if (IsDynamicApiStaticCache == true && scVendor.TryGet(out var list))
            {
                return list.Where(x => x.vendor_id == Guid.Parse(vendorId.Value)).FirstOrDefault();
            }
            else
            {
                return GetVendorFromCache(vendorId);
            }
        }

        /// <summary>
        /// ベンダー情報の取得
        /// </summary>
        /// <param name="vendorId">ベンダーID</param>
        /// <returns>ベンダー情報</returns>
        public VendorVO GetVendorFromCache(VendorId vendorId) => Cache.Get<VendorVO>(CacheManager.CreateKey(CACHE_KEY_GET_VENDOR, vendorId.Value), CacheExpirationTimeSpan, () =>
        {
            if (string.IsNullOrEmpty(vendorId?.Value))
            {
                throw new ArgumentNullException(nameof(vendorId));
            }

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
--GetVendor
SELECT
    v.vendor_id
    ,v.vendor_name
    ,v.is_data_offer
    ,v.is_data_use
    ,v.is_enable
FROM
    VENDOR v /*WITH(NOLOCK)*/
WHERE
    v.vendor_id = /*ds vendor_id*/'id' 
    AND v.is_active = 1
";
            }
            else
            {
                sql = @"
--GetVendor
SELECT
    v.vendor_id
    ,v.vendor_name
    ,v.is_data_offer
    ,v.is_data_use
    ,v.is_enable
FROM
    Vendor v WITH(NOLOCK)
WHERE
    v.vendor_id = @vendor_id
    AND v.is_active = 1
";
            }
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("vendor_id", vendorId.Value);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var vendor = DbConnection.Query<DB_Vendor>(twowaySql.Sql, dynParams).FirstOrDefault();
            return vendor == null ? null : new VendorVO(vendor.vendor_id, vendor.vendor_name, vendor.is_data_offer, vendor.is_data_use, vendor.is_enable);
        });

        public bool IsApprovedAgreement(VendorId vendorId, ControllerId controllerId)
        {
            return Cache.Get<bool?>(CacheManager.CreateKey(CACHE_KEY_IS_APPROVEDAGREEMENT, vendorId.Value, controllerId.Value), CacheExpirationTimeSpan, () =>
            {
                //関連INDEX
                //SQLの修正をする際は関連INDEXの確認も行うこと
                //IDX_VendorControllerAgreement_VendorIdControllerId
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT
    result
FROM
    VENDOR_CONTROLLER_AGREEMENT vca
WHERE
    vendor_id=/*ds vendor_id*/'id' 
    AND controller_id=/*ds controller_id*/'id' 
    AND is_active=1
";
                }
                else
                {
                    sql = @"
SELECT
    result
FROM
    VendorControllerAgreement vca
WHERE
    vendor_id=@vendor_id
    AND controller_id=@controller_id
    AND is_active=1
";
                }
                var twowaySqlParam = new Dictionary<string, object>();
                twowaySqlParam.Add("vendor_id", vendorId.Value);
                twowaySqlParam.Add("controller_id", controllerId.Value);
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
                var result = DbConnection.Query<string>(twowaySql.Sql, dynParams).FirstOrDefault();
                return result == "apl" ? true : false;
            }) ?? false;
        }


        public PhysicalRepositoryId GetPhysicalRepositoryIdByControllerId(ControllerId controllerId, RepositoryType repositoryType)
        {
            if (!Guid.TryParse(controllerId?.Value, out var guidControllerId))
            {
                throw new ArgumentException($"{nameof(controllerId)}: {controllerId?.Value}");
            }

            var physicalRepositoryId = Cache.Get<Guid?>(CacheManager.CreateKey(CACHE_KEY_CONTROLLER_PHYSICALREPOSITORY, controllerId.Value, repositoryType.ToString()), CacheExpirationTimeSpan, () =>
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT 
    x.physical_repository_id
FROM (
    SELECT
        pr.physical_repository_id AS physical_repository_id
        ,1                     AS is_primary
    FROM
        CONTROLLER c
        INNER JOIN API a ON c.controller_id = a.controller_id AND a.is_active = 1
        INNER JOIN REPOSITORY_GROUP rg ON a.repository_group_id = rg.repository_group_id AND rg.is_active = 1
        INNER JOIN PHYSICAL_REPOSITORY pr ON rg.repository_group_id = pr.repository_group_id AND pr.is_active = 1
    WHERE
        c.is_active=1
        AND c.controller_id = /*ds controller_id*/'id' 
        AND rg.repository_type_cd = /*ds repository_type_cd*/'cd' 
    UNION ALL
    SELECT
        pr.physical_repository_id AS physical_repository_id
        ,0                     AS is_primary
    FROM
        CONTROLLER c
        INNER JOIN API a ON c.controller_id = a.controller_id AND a.is_active = 1
        INNER JOIN SECONDARY_REPOSITORY_MAP secrg ON a.api_id = secrg.api_id AND secrg.is_active = 1
        INNER JOIN REPOSITORY_GROUP rg ON secrg.repository_group_id = rg.repository_group_id AND rg.is_active = 1
        INNER JOIN PHYSICAL_REPOSITORY pr ON rg.repository_group_id = pr.repository_group_id AND pr.is_active = 1
    WHERE
        c.is_active = 1
        AND c.controller_id = /*ds controller_id*/'id' 
        AND rg.repository_type_cd = /*ds repository_type_cd*/'cd' 
) x
ORDER BY x.is_primary DESC
FETCH FIRST 1 ROWS ONLY
";
                }
                else
                {
                    sql = @"
SELECT 
    TOP 1 x.physical_repository_id
FROM (
    SELECT
        pr.physical_repository_id AS physical_repository_id
        ,1                     AS is_primary
    FROM
        Controller c
        INNER JOIN Api a ON c.controller_id = a.controller_id AND a.is_active = 1
        INNER JOIN RepositoryGroup rg ON a.repository_group_id = rg.repository_group_id AND rg.is_active = 1
        INNER JOIN PhysicalRepository pr ON rg.repository_group_id = pr.repository_group_id AND pr.is_active = 1
    WHERE
        c.is_active=1
        AND c.controller_id = @controller_id
        AND rg.repository_type_cd = @repository_type_cd
    UNION ALL
    SELECT
        pr.physical_repository_id AS physical_repository_id
        ,0                     AS is_primary
    FROM
        Controller c
        INNER JOIN Api a ON c.controller_id = a.controller_id AND a.is_active = 1
        INNER JOIN SecondaryRepositoryMap secrg ON a.api_id = secrg.api_id AND secrg.is_active = 1
        INNER JOIN RepositoryGroup rg ON secrg.repository_group_id = rg.repository_group_id AND rg.is_active = 1
        INNER JOIN PhysicalRepository pr ON rg.repository_group_id = pr.repository_group_id AND pr.is_active = 1
    WHERE
        c.is_active = 1
        AND c.controller_id = @controller_id
        AND rg.repository_type_cd = @repository_type_cd
) x
ORDER BY x.is_primary DESC
";
                }
                var twowaySqlParam = new Dictionary<string, object>();
                twowaySqlParam.Add("controller_id", controllerId.Value);
                twowaySqlParam.Add("repository_type_cd", repositoryType.ToCode());
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
                return DbConnection.QuerySingleOrDefault<Guid?>(twowaySql.Sql, dynParams);
            });

            if (physicalRepositoryId == null)
            {
                return null;
            }
            else
            {
                return new PhysicalRepositoryId(physicalRepositoryId.ToString());
            }
        }

        public DataSchema GetControllerSchemaByUrl(ControllerUrl url)
        {
            if (url == null || string.IsNullOrEmpty(url.Value))
            {
                throw new ArgumentNullException(nameof(url));
            }
            return Cache.Get<DataSchema>(CacheManager.CreateKey(CACHE_KEY_DATASCHEMA_CONTROLLERURL, url.Value), CacheExpirationTimeSpan, () =>
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT 
    ds.data_schema
FROM
    CONTROLLER c
    INNER JOIN DATA_SCHEMA ds ON c.controller_schema_id=ds.data_schema_id AND ds.is_active=1
WHERE
    c.url=/*ds url*/'u' 
    AND c.is_active=1
";
                }
                else
                {
                    sql = @"
SELECT 
    ds.data_schema
FROM
    Controller c
    INNER JOIN DataSchema ds ON c.controller_schema_id=ds.data_schema_id AND ds.is_active=1
WHERE
    c.url=@url
    AND c.is_active=1
";
                }
                var twowaySqlParam = new Dictionary<string, object>();
                twowaySqlParam.Add("url", url.Value);
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
                var result = DbConnection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
                return result == null ? null : new DataSchema(result);
            });
        }

        public DataSchema GetSchemaModelById(DataSchemaId id)
        {
            if (id == null || string.IsNullOrEmpty(id.Value))
            {
                throw new ArgumentNullException(nameof(id));
            }
            return Cache.Get<DataSchema>(CacheManager.CreateKey(CACHE_KEY_DATASCHEMA_SCHEMAID, id.Value), CacheExpirationTimeSpan, () =>
            {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT 
    ds.data_schema
FROM
    DATA_SCHEMA ds
WHERE
    ds.data_schema_id=/*ds id*/'1' 
    AND ds.is_active=1
";
                }
                else
                {
                    sql = @"
SELECT 
    ds.data_schema
FROM
    DataSchema ds
WHERE
    ds.data_schema_id=@id
    AND ds.is_active=1
";
                }
                var param = new { id = id.Value };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                var result = DbConnection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
                return result == null ? null : new DataSchema(result);
            });
        }

        public DataSchema GetSchemaModelByName(DataSchemaName name)
        {
            if (name == null || string.IsNullOrEmpty(name.Value))
            {
                throw new ArgumentNullException(nameof(name));
            }
            return Cache.Get<DataSchema>(CacheManager.CreateKey(CACHE_KEY_DATASCHEMA_SCHEMANAME, name.Value), CacheExpirationTimeSpan, () =>
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT 
    ds.data_schema
FROM
    DATA_SCHEMA ds
WHERE
    ds.schema_name=/*ds name*/'n' 
    AND ds.is_active=1
";
                }
                else
                {
                    sql = @"
SELECT 
    ds.data_schema
FROM
    DataSchema ds
WHERE
    ds.schema_name=@name
    AND ds.is_active=1
";
                }
                var twowaySqlParam = new Dictionary<string, object>();
                twowaySqlParam.Add("name", name.Value);
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
                var result = DbConnection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
                return result == null ? null : new DataSchema(result);
            });
        }

        public bool IsAgreeToTerms(TermsGroupCode termsGroupCode, OpenId openid)
        {
            //同意が必要なAPIは最後に同意した規約が取り下げされていないか確認する
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    count(1) 
FROM 
    USER_TERMS ut
INNER JOIN 
    (
        SELECT 
            open_id,max(agreement_date) AS agreement_date
        FROM 
            USER_TERMS sut
        INNER JOIN TERMS t ON t.terms_id = sut.terms_id 
            AND t.is_active = 1
            AND t.terms_group_code = /*ds TermsGroupCode*/'1' 
        WHERE sut.open_id = /*ds OpenId*/'1' 
            AND sut.is_active = 1
        GROUP BY open_id) LATEST_AGREE
    ON ut.open_id = LATEST_AGREE.open_id 
    AND ut.agreement_date = LATEST_AGREE.agreement_date 
WHERE
    ut.is_active = 1
    AND ut.revoke_date is null
";
            }
            else
            {
                sql = @"
SELECT 
    count(1) 
FROM 
    UserTerms ut
INNER JOIN 
    (
        SELECT 
            open_id,max(agreement_date) AS agreement_date
        FROM 
            UserTerms sut
        INNER JOIN Terms t ON t.terms_id = sut.terms_id 
			AND t.is_active = 1
			AND t.terms_group_code = @TermsGroupCode
        WHERE sut.open_id =  @OpenId
            AND sut.is_active = 1
        GROUP BY open_id) AS LatestAgree
    ON ut.open_id = LatestAgree.open_id 
    AND ut.agreement_date = LatestAgree.agreement_date 
WHERE
    ut.is_active = 1
    AND ut.revoke_date is null
";
            }
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("OpenId", openid.Value);
            twowaySqlParam.Add("TermsGroupCode", termsGroupCode.Value);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var result = Cache.Get<int?>(CacheManager.CreateKey(CACHE_KEY_USERTERMS, termsGroupCode.Value, openid.Value), CacheExpirationTimeSpan, () =>
                 DbConnection.Query<int?>(twowaySql.Sql, dynParams).First()
            );
            return result > 0;
        }

        public IEnumerable<string> GetResourceSharedOpenId(ResourceGroupId resourceGroupId, OpenId openid)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
	urs.open_id 
FROM 
	USER_RESOURCE_SHARE urs
	INNER JOIN USER_GROUP ug ON ug.user_group_id = urs.user_group_id AND ug.is_active = 1
	INNER JOIN USER_GROUP_MAP ugm ON ugm.user_group_id = urs.user_group_id AND ugm.is_active = 1
WHERE
	urs.resource_group_id = /*ds ResourceGroupId*/'1' 
	AND urs.user_shared_type_code = 'stg'
	AND urs.is_active = 1
	AND ugm.open_id = /*ds OpenId*/'1' 
UNION 
	SELECT urs.open_id FROM USER_RESOURCE_SHARE urs
WHERE
	urs.resource_group_id = /*ds ResourceGroupId*/'1' 
	AND urs.user_shared_type_code = 'uls'
	AND urs.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT 
	urs.open_id 
FROM 
	UserResourceShare urs
	INNER JOIN UserGroup ug ON ug.user_group_id = urs.user_group_id AND ug.is_active = 1
	INNER JOIN UserGroupMap ugm ON ugm.user_group_id = urs.user_group_id AND ugm.is_active = 1
WHERE
	urs.resource_group_id = @ResourceGroupId
	AND urs.user_shared_type_code = 'stg'
	AND urs.is_active = 1
	AND ugm.open_id = @OpenId
UNION 
	SELECT urs.open_id FROM UserResourceShare urs
WHERE
	urs.resource_group_id = @ResourceGroupId
	AND urs.user_shared_type_code = 'uls'
	AND urs.is_active = 1
";
            }
            var twowaySqlParam = new Dictionary<string, object>();
            twowaySqlParam.Add("OpenId", openid.Value);
            twowaySqlParam.Add("ResourceGroupId", resourceGroupId.Value);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, twowaySqlParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(twowaySqlParam);
            var result = Cache.Get<List<string>>(CacheManager.CreateKey(CACHE_KEY_USER_RESOURCE_SHARE, resourceGroupId.Value, openid.Value), CacheExpirationTimeSpan, () =>
                 DbConnection.Query<string>(twowaySql.Sql, dynParams)
            );

            return result;
        }

    }
}
