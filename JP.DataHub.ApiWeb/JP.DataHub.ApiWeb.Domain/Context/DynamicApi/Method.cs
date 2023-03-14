using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Net;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Unity;
using JP.DataHub.Aop;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Attributes;
using JP.DataHub.ApiWeb.Domain.ApiFilter;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    // .NET6
    /// <summary>
    /// Dynamic API用のメソッド（処理）をつかさどるクラス
    /// </summary>
    class Method : IEntity, IMethod
    {
        private JPDataHubLogger Logger => s_lazyLogger.Value;
        private static Lazy<JPDataHubLogger> s_lazyLogger => new Lazy<JPDataHubLogger>(() => new JPDataHubLogger(typeof(Method)));


        private IAuthenticationRepository AuthenticationRepository => _lazyAuthenticationRepository.Value;
        private readonly Lazy<IAuthenticationRepository> _lazyAuthenticationRepository = new Lazy<IAuthenticationRepository>(() => UnityCore.Resolve<IAuthenticationRepository>());

        private IDynamicApiRepository DynamicApiRepository => _lazyDynamicApiRepository.Value;
        private readonly Lazy<IDynamicApiRepository> _lazyDynamicApiRepository = new Lazy<IDynamicApiRepository>(() => UnityCore.Resolve<IDynamicApiRepository>());

        private IFilterManager FilterManager => _lazyFilterManager.Value;
        private readonly Lazy<IFilterManager> _lazyFilterManager = new Lazy<IFilterManager>(() => UnityCore.Resolve<IFilterManager>());

        private bool _isInternalServerErrorDetailResponse => UnityCore.Resolve<bool>("IsInternalServerErrorDetailResponse");
        private bool _isHeaderAuthentication => UnityCore.Resolve<bool>("HeaderAuthentication");
        private static bool UseClientCertAuthentication => UnityCore.Resolve<bool>("UseClientCertificateAuth");

        private static bool UseTerms => UnityCore.Resolve<bool>("UseTerms");

        private IPerRequestDataContainer PerRequestDataContainer => _lazyPerRequestDataContainer.Value;
        private readonly Lazy<IPerRequestDataContainer> _lazyPerRequestDataContainer = new Lazy<IPerRequestDataContainer>(
            () =>
            {
                try
                {
                    return UnityCore.Resolve<IPerRequestDataContainer>();
                }
                catch
                {
                    // ignored
                    return UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                }
            });


        /// <summary>API所有ベンダー</summary>
        public VendorId VendorId { get; set; }

        /// <summary>API所有システム</summary>
        public SystemId SystemId { get; set; }

        /// <summary>
        /// ApiId
        /// </summary>
        public ApiId ApiId { get; set; }

        /// <summary>
        /// ControllerId
        /// </summary>
        public ControllerId ControllerId { get; set; }

        public ControllerUrl ControllerRelativeUrl { get; set; }

        public IsUseBlobCache IsUseBlobCache { get; set; }

        /// <summary>
        /// Httpのメソッドタイプ
        /// </summary>
        public HttpMethodType MethodType { get; set; }

        /// <summary>
        /// POSTするとき、主にバリデーションチェック用のスキーマ
        /// （このスキーマが定義されているときはバリデーションを行う）
        /// </summary>
        public DataSchema RequestSchema { get; set; }

        /// <summary>
        /// 対象Controllerの登録すべきスキーマ
        /// </summary>
        public DataSchema ControllerSchema { get; set; }

        public DataSchema ResponseSchema { get; set; }

        /// <summary>
        /// Uriスキーマ
        /// </summary>
        public DataSchema UriSchema { get; set; }

        /// <summary>
        /// Reposiory（永続化層）の情報
        /// </summary>
        public ReadOnlyCollection<RepositoryInfo> RepositoryInfo { get; set; }

        /// <summary>
        /// X-VendorId,X-SystemIdなどの独自認証を行うか？
        /// true : 独自認証を行う
        /// false : 独自認証を行わない
        /// </summary>
        public IsHeaderAuthentication IsHeaderAuthentication { get; set; }
        public IsVendorSystemAuthenticationAllowNull IsVendorSystemAuthenticationAllowNull { get; set; }

        /// <summary>
        /// クライアント証明書認証をするか？
        /// </summary>
        public IsClientCertAuthentication IsClientCertAuthentication { get; set; }

        /// <summary>
        /// OpenID認証を行うか？
        /// true : OpenID認証を行う
        /// false : OpenID認証を行わない
        /// </summary>
        public IsOpenIdAuthentication IsOpenIdAuthentication { get; set; }

        /// <summary>
        /// X-Admin認証を行うか？（特にマスターなどの登録系APIで設定し、勝手にマスターを触れないようにするため）
        /// true : X-Admin認証を行う
        /// false : X-Admin認証を行わない
        /// </summary>
        public IsAdminAuthentication IsAdminAuthentication { get; set; }

        /// <summary>
        /// Repository（永続化層）へ保存する際のキーを指定する
        /// これは登録と取得でAPI(URL)で異なった場合に、このキーを合わせることによって登録と取得がペアになるように設定する
        /// </summary>
        public RepositoryKey RepositoryKey { get; set; }

        public RepositoryKey ControllerRepositoryKey { get; set; }

        /// <summary>
        /// URLの{xxx}のキーと値のペアを保持する
        /// </summary>
        public UrlParameter KeyValue { get; set; }

        /// <summary>
        /// GETパラメータのQuery文字列のキーと値のペアを保持する
        /// </summary>
        public QueryStringVO Query { get; set; }
        public QueryType QueryType { get; set; }

        /// <summary>
        /// HttpMethodTypeがPostの場合、そのデータをどう扱うかを指定する
        /// block : データ（主にjson）を１つの塊として登録する
        /// array : データが配列の場合、それぞれの要素を分解して登録する。（データをいっぺんに登録することが可能になる）
        /// </summary>
        public PostDataType PostDataType { get; set; }

        /// <summary>
        /// 相対URL
        /// </summary>
        public RelativeUri RelativeUri { get; set; }

        /// <summary>
        /// APIとして定義しているURL
        /// </summary>
        public ApiUri ApiUri { get; set; }

        /// <summary>
        /// VendorId、SystemId単位にデータの登録・参照をするか
        /// </summary>
        public IsVendor IsVendor { get; set; }

        /// <summary>
        /// 個人単位にデータの登録・参照をするか
        /// </summary>
        public IsPerson IsPerson { get; set; }

        /// <summary>
        /// Gatewayする先のURL
        /// </summary>
        public GatewayInfo GatewayInfo { get; set; }

        /// <summary>
        /// APiの検索用Query
        /// </summary>
        public ApiQuery ApiQuery { get; set; }

        public IsOverPartition IsOverPartition { get; set; }

        public Script Script { get; set; }

        public ScriptTypeVO ScriptType { get; set; }

        public ActionTypeVO ActionType { get; set; }

        /// <summary>
        /// 非同期の元のActionType(非同期の場合は元のActionTypeがAyncActionに上書きされてしまうため非同期の場合は元のActionTypeを保持する)
        /// </summary>
        public ActionTypeVO AsyncOriginalActionType { get; set; }

        public CacheInfo CacheInfo { get; set; }

        public IsAccesskey IsAccesskey { get; set; }

        public IsAutomaticId IsAutomaticId { get; set; }

        public ActionTypeVersion ActionTypeVersion { get; set; }

        public ActionInjectorHandler ActionInjectorHandler { get; set; }

        public PartitionKey PartitionKey { get; set; }

        public ApiResourceSharing ApiResourceSharing { get; set; }

        public List<ResourceSharingPersonRule> ResourceSharingPersonRules { get; set; }

        public HasMailTemplate HasMailTemplate { get; set; }

        public HasWebhook HasWebhook { get; set; }

        public IsEnableAttachFile IsEnableAttachFile { get; set; }

        /// <summary>
        /// AttachFileBlobRepositoryの情報
        /// </summary>
        public RepositoryInfo AttachFileBlobRepositoryInfo { get; set; }
        public RepositoryGroupId AttachFileBlobRepositoryGroupId { get; set; }

        public InternalOnly InternalOnly { get; set; }

        public IsSkipJsonSchemaValidation IsSkipJsonSchemaValidation { get; set; }

        public IsOpenidAuthenticationAllowNull IsOpenidAuthenticationAllowNull { get; set; }

        public PublicDate PublicDate { get; set; }

        public IsOptimisticConcurrency IsOptimisticConcurrency { get; set; }
        public IsEnableBlockchain IsEnableBlockchain { get; set; }
        public IsDocumentHistory IsDocumentHistory { get; set; }
        public RepositoryInfo DocumentHistoryRepositoryInfo { get; set; }
        public IsVisibleAgreement IsVisibleAgreement { get; set; }

        /// <summary>
        /// コンテナ分離を行うか
        /// </summary>
        public IsContainerDynamicSeparation IsContainerDynamicSeparation { get; set; }
        /// <summary>
        /// リソース間のSQLによるアクセスを許可するか
        /// </summary>
        public IsOtherResourceSqlAccess IsOtherResourceSqlAccess { get; set; }

        /// <summary>
        /// 透過APIか
        /// </summary>
        public IsTransparentApi IsTransparentApi { get; set; }

        /// <summary>
        /// リソースのバージョンを使用するかどうか
        /// </summary>
        public IsEnableResourceVersion IsEnableResourceVersion { get; set; }

        /// <summary>
        /// 規約の同意を必要とするか
        /// </summary>
        public IsRequireConsent IsRequireConsent { get; set; }
        /// <summary>
        /// 規約グループコード
        /// </summary>
        public TermsGroupCode TermsGroupCode { get; set; }
        /// <summary>
        /// リソースグループID
        /// </summary>
        public ResourceGroupId ResourceGroupId { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Method()
        {
        }

        /// <summary>
        /// WebAPIのリクエスト処理
        /// </summary>
        /// <param name="actionId">要求のID</param>
        /// <param name="mediaType">要求のMediaType</param>
        /// <param name="notAuthentication">すべて認証を行わないか</param>
        /// <param name="contents">要求のメッセージのBodyStream</param>
        /// <param name="accept">リクエストのAccept</param>
        /// <param name="contentRange">ContentsRange</param>
        /// <returns>WEBレスポンス</returns>

        //AllExceptionHandlingは一番上
        [AllExceptionHandling(ErrorCodeMessage.Code.E99999)]
        [ExceptionHandling(typeof(HttpResponseException), ErrorCodeMessage.Code.E50401, ConvertType.HttpResponseDirect)]
        [ExceptionHandling(typeof(RoslynScriptRuntimeException), ErrorCodeMessage.Code.E50402, ConvertType.RoslynScriptRuntimeError)]
        [ExceptionHandling(typeof(AggregateException), ErrorCodeMessage.Code.E50403, ConvertType.AddInnerExeptionMessage)]
        [ExceptionHandling(typeof(XVersionNotFoundException), ErrorCodeMessage.Code.E50404, ConvertType.ExeptionMsgToDetail)]
        [ExceptionHandling(typeof(QuerySyntaxErrorException), ErrorCodeMessage.Code.E50405, ConvertType.ExeptionMsgToDetailWithLog)]
        [ExceptionHandling(typeof(ODataException), ErrorCodeMessage.Code.E50406, ConvertType.ExeptionMsgToDetailWithLog)]
        [ExceptionHandling(typeof(ApiException), ErrorCodeMessage.Code.E50407, ConvertType.HttpResponseDirect)]
        [ExceptionHandling(typeof(NotParseCsvException), ErrorCodeMessage.Code.E50408, ConvertType.ExeptionMsgToDetail)]
        [RFC7807ExceptionHandling(ConvertType.HttpResponseDirect)]
        public HttpResponseMessage Request(ActionId actionId, MediaType mediaType, NotAuthentication notAuthentication, Contents contents, Accept accept, ContentRange contentRange = null)
        {
            var httpContext = HttpContextAccessor.Current;

            bool skipAuth = PerRequestDataContainer.XNotAuthenticationRequest | notAuthentication.Value;
            if (!skipAuth)
            {
                // APIの認証を実行
                var response = Authenticate();
                if (!response.IsSuccessStatusCode) return response;
            }

            //ベンダー依存確認
            if (!IsVendorDependencyValid())
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Vendor Or System Unspecified", Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            }

            // ユーザー依存確認
            //if (!IsUserDependencyValid())
            //{
            //    return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("openId Unspecified", Encoding.UTF8, MediaTypeConst.ApplicationJson) };
            //}

            // ユーザーのベンダーIDが取得できる場合
            if (IsHeaderAuthentication.Value)
            {
                if (UnityCore.Resolve<bool>("EnableWebHookAndMailTemplate") == false)
                {
                    HasMailTemplate = new HasMailTemplate(false);
                    HasWebhook = new HasWebhook(false);
                }
                else
                {
                    HasMailTemplate = new HasMailTemplate(DynamicApiRepository.HasMailTemplate(ControllerId, new VendorId(PerRequestDataContainer.VendorId)));
                    HasWebhook = new HasWebhook(DynamicApiRepository.HasWebhook(ControllerId, new VendorId(PerRequestDataContainer.VendorId)));
                }
            }
            // 認可の確認
            if(PerRequestDataContainer.XUserResourceSharing?.Any() == true)
            {
                if (IsOtherResourceSqlAccess?.Value == true)
                {
                    // テーブル結合許可が設定されている場合はエラーを返す
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E50414, this.RelativeUri?.Value);
                }
                if ((ActionType.Value != DynamicApi.ActionType.Query && ActionType.Value != DynamicApi.ActionType.OData))
                {
                    // クエリ系以外は指定できない
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E50415, this.RelativeUri?.Value);
                }
                // 取得OpenIdに対して認可されているOpenIdを取得する
                var openIds = DynamicApiRepository.GetResourceSharedOpenId(ResourceGroupId, new OpenId(PerRequestDataContainer.OpenId)).ToList();
                if (!openIds.Contains(PerRequestDataContainer.OpenId))
                {
                    // 自身が含まれていいない場合は追加してあげる
                    openIds.Add(PerRequestDataContainer.OpenId);
                }
                var except = PerRequestDataContainer.XUserResourceSharing.Except(openIds).ToList();
                if (PerRequestDataContainer.XUserResourceSharing.Any(x => x == "ALL"))
                {
                    // 許可されているもの全て取得の場合は取得したOpenIDを全て指定する
                    PerRequestDataContainer.XUserResourceSharing = openIds;
                }
                else if(except.Any())
                {
                    // 認可されていないOpenIdを指定したのでエラーを返す
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E50413, this.RelativeUri?.Value, detail: $"共有が許可されていないOpenId={string.Join(",", except)}");
                }
            }

            // APIがInternalCallOnlyの場合
            if (this.InternalOnly?.IsInternalOnly == true)
            {
                if (PerRequestDataContainer.IsInternalCall == false)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10415, this.RelativeUri?.Value);
                }
                if (string.IsNullOrEmpty(InternalOnly.InternalOnlyKeyword) == false && InternalOnly.InternalOnlyKeyword != PerRequestDataContainer.InternalCallKeyword)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10416, this.RelativeUri?.Value);
                }
            }

            if (this.PublicDate?.IsPublicNow(PerRequestDataContainer.GetDateTimeUtil().UtcNow) == false)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10410, this.RelativeUri?.Value);
            }

            Func<IApiFilterActionParam, bool, HttpResponseMessage> action = (param, isChange) =>
            {
                if (isChange)
                {
                    // ApiFilter内でQueryStringが変更された場合はQueryを置き換える
                    if (Query != null && Query.GetQueryString() != param.QueryString ||
                        Query == null && !string.IsNullOrEmpty(param.QueryString))
                    {
                        var nvc = System.Web.HttpUtility.ParseQueryString(param.QueryString);
                        var dic = nvc.AllKeys.ToDictionary(k => new QueryStringKey(k), k => new QueryStringValue(nvc[k]));
                        Query = new QueryStringVO(dic);
                    }
                }
                var executeMethod = DynamicApiActionFactory.CreateDynamicApiAction(this, new MediaType(param.MediaType), contents, new Accept(param.Accept), PerRequestDataContainer, actionId, RequestSchema, new ContentRange(param.ContentRange));
                executeMethod.Initialize();
                return executeMethod.ExecuteAction();
            };

            var queryString = Query?.GetQueryString();
            IApiFilterActionParam actionParam = new ApiFilterActionParam()
            {
                Action = ActionType == null ? "xxx" : ActionType.Code,
                MediaType = mediaType?.Value,
                Accept = accept?.Value,
                ContentRange = contentRange?.SourceValue,
                ResourceUrl = ControllerRelativeUrl?.Value,
                ApiUrl = ApiUri?.Value,
                VendorId = VendorId?.Value,
                SystemId = SystemId?.Value,
                QueryString = queryString,
                QueryStringDic = string.IsNullOrEmpty(queryString) ? null : new QueryStringDictionary(queryString),
                HttpMethodType = MethodType?.Value.ToString(),
                OpenId = PerRequestDataContainer.OpenId,
                ControllerSchema = ControllerSchema?.Value,
                UriSchema = UriSchema?.Value,
                RequestSchema = RequestSchema?.Value,
                ResponseSchema = ResponseSchema?.Value,
                RequestVendorId = PerRequestDataContainer.VendorId,
                RequestSystemId = PerRequestDataContainer.SystemId,
                Headers = PerRequestDataContainer.RequestHeaders.DeepCopy<List<string>>(UnityCore.Resolve<bool>("HeaderIgnoreCase") == true ? StringComparer.InvariantCultureIgnoreCase : StringComparer.InvariantCulture),
                Gateway = GatewayInfo == null ? null : new ApiFilterGateway() { Url = GatewayInfo.Url, CredentialUsername = GatewayInfo.CredentialUsername, CredentialPassword = GatewayInfo.CredentialPassword, GatewayRelayHeader = GatewayInfo.GatewayRelayHeader },
                TransparentApi = IsTransparentApi == null ? false : IsTransparentApi.Value,
                IsOverPartition = IsOverPartition?.Value == true,
                PostDataType = PostDataType?.Value,
                LanguageInfo = PerRequestDataContainer.CultureInfo,
                IsInternalCall = PerRequestDataContainer.IsInternalCall,
                //通常は、明示的にnull にしておく２
                ImpersonateRequestVendorId = null,
                ImpersonateRequestSystemId = null
            };
            var filters = FilterManager?.GetApiFilter(actionParam);
            if (filters != null)
            {
                actionParam.ContentsStream = contents.Value;
                //Contentsは必要になった場合に読み込み-

                //BeforeAction
                foreach (var filter in filters)
                {
                    Logger.Trace($"ApiFilter BeforeAction Start Type:{filter.GetType().Name}");
                    HttpResponseMessage msg = null;
                    try
                    {
                        msg = filter.BeforeAction(actionParam);
                    }
                    catch (AopResponseException e)
                    {
                        msg = e.Response;
                    }
                    Logger.Trace($"ApiFilter BeforeAction End Type:{filter.GetType().Name}");
                    if (!string.IsNullOrEmpty(actionParam.ImpersonateRequestVendorId) && !string.IsNullOrEmpty(actionParam.ImpersonateRequestSystemId))
                    {
                        PerRequestDataContainer.VendorId = actionParam.ImpersonateRequestVendorId;
                        PerRequestDataContainer.SystemId = actionParam.ImpersonateRequestSystemId;
                    }
                    if (!string.IsNullOrEmpty(actionParam.OpenId))
                    {
                        // BeforeAction内で上書きされたOpenIdをPerRequestDataContainerに反映(上書きされていない場合は同じ値)
                        PerRequestDataContainer.OpenId = actionParam.OpenId;
                    }
                    if (msg != null)
                    {
                        return msg;
                    }
                }
                ActionType = actionParam.Action.ToActionTypeVO();
                IsOverPartition = new IsOverPartition(actionParam.IsOverPartition);
                PostDataType = new PostDataType(actionParam.PostDataType);
                GatewayInfo = actionParam.Gateway == null ? null : new GatewayInfo(actionParam.Gateway.Url, actionParam.Gateway.CredentialUsername, actionParam.Gateway.CredentialPassword, actionParam.Gateway.GatewayRelayHeader);
                // BeforeAction内でActionInjectorの無効化が指定された場合はAction実行前にActionInjectorをクリア
                // (OpenDataAOPでの透過APIのGateway時に使用)
                if (actionParam.DisableActionInjector)
                {
                    Logger.Info($"ActionInjector disabled: url={actionParam.ResourceUrl}/{actionParam.ApiUrl}");
                    ActionInjectorHandler = null;
                }
                ResetSchema(actionParam);
                PerRequestDataContainer.RequestHeaders = actionParam.Headers;
                var result = action(actionParam, true);

                //AfterActions

                for (int i = filters.Count - 1; i >= 0; i--)
                {
                    Logger.Trace($"ApiFilter AfterAction Start Type:{filters[i].GetType().Name}");
                    HttpResponseMessage msg = null;
                    try
                    {
                        msg = filters[i].AfterAction(result);
                    }
                    catch (AopResponseException e)
                    {
                        msg = e.Response;
                    }
                    Logger.Trace($"ApiFilter AfterAction End Type:{filters[i].GetType().Name}");
                    if (msg != null)
                    {
                        return msg;
                    }
                }
                return result;
            }
            else
            {
                return action(actionParam, false);
            }
        }

        private void ResetSchema(IApiFilterActionParam actionParam)
        {
            ControllerSchema = ResetSchema(ControllerSchema, actionParam.ControllerSchema);
            UriSchema = ResetSchema(UriSchema, actionParam.UriSchema);
            RequestSchema = ResetSchema(RequestSchema, actionParam.RequestSchema);
            ResponseSchema = ResetSchema(ResponseSchema, actionParam.ResponseSchema);
        }

        private DataSchema ResetSchema(DataSchema schema, string newschema)
        {
            var oldschema = schema?.Value;
            if (string.IsNullOrEmpty(newschema))
            {
                return new DataSchema(null);
            }
            else if (oldschema == newschema)
            {
                return schema;
            }
            return new DataSchema(newschema);
        }

        /// <summary>
        /// APIの認証を行います。
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        public HttpResponseMessage Authenticate()
        {
            // 独自アクセストークン認証 と ベンダーID、システムID認証が可能な場合
            if (IsHeaderAuthentication.Value)
            {
                // 設定によるヘッダー認証の制御
                bool enable_header_authentication = _isHeaderAuthentication;

                // ヘッダ認証以外で且つSetUserInfoによるトークンの検証がされていない場合、トークンの検証を行う。
                if (!enable_header_authentication && !PerRequestDataContainer.VendorSystemAuthenticated)
                {
                    if (IsVendorSystemAuthenticationAllowNull.Value)
                    {
                        //省略を許可されている場合はデフォルトのベンダーシステムを設定する
                        PerRequestDataContainer.VendorId = UnityCore.Resolve<string>("VendorSystemAuthenticationDefaultVendorId")?.ToLower();
                        PerRequestDataContainer.SystemId = UnityCore.Resolve<string>("VendorSystemAuthenticationDefaultSystemId")?.ToLower();
                    }
                    else return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E02402, this.RelativeUri?.Value);
                }

                //X-Vendor,X-System トークンベースの認証に関わらず　Funcの検証を行う。
                if (!IsOriginalAuthentication())
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E02403, this.RelativeUri?.Value);
            }
            //クライアント証明書認証をするAPI
            else if (IsClientCertAuthentication.Value)
            {
                //AppSettingsが、クライアント証明書使う設定になっていて、且つ
                //クライアント証明書が検証されていない場合は、NG
                if (UseClientCertAuthentication && !PerRequestDataContainer.XVendorSystemCertificateAuthenticated)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E02411, this.RelativeUri?.Value);
                }
                //Funcの検証
                if (!IsOriginalAuthentication())
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E02403, this.RelativeUri?.Value);
            }

            // X-Admin認証
            if (IsAdminAuthentication.Value)
            {
                if (!IsAdminValid())
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E02404, this.RelativeUri?.Value);
            }

            // OpenID認証
            if (IsOpenIdAuthentication.Value)
            {
                var openIdAllowedApplications = DynamicApiRepository.GetOpenIdAllowedApplications(VendorId, ControllerId, ApiId);

                var validationStatus = OpenIdAuthorizationValidator.Validate(PerRequestDataContainer.OpenId, PerRequestDataContainer.IsDeveloper,
                    PerRequestDataContainer.Claims, openIdAllowedApplications);

                if (validationStatus == OpenIdAuthorizationValidator.ValidationStatus.AuthorizationInvalid)
                {
                    if (!string.IsNullOrEmpty(PerRequestDataContainer.AuthorizationError))
                    {
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                        {
                            Content = new StringContent(PerRequestDataContainer.AuthorizationError, Encoding.UTF8, MediaTypeConst.ApplicationJson)
                        };
                    }
                    else return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E01401, this.RelativeUri?.Value);

                }
                else if (validationStatus == OpenIdAuthorizationValidator.ValidationStatus.AudienceInvalid)
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E01402, this.RelativeUri?.Value);
            }

            // APIのOpenID認証
            if (IsOpenidAuthenticationAllowNull != null && IsOpenidAuthenticationAllowNull.Value == true)
            {
                if (DynamicApiRepository.HasApiAccessOpenid(ApiId, new OpenId(PerRequestDataContainer.OpenId)) == false)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E01403, this.RelativeUri?.Value);
                }
            }

            //ControllerFilter
            string clientIpAddress = PerRequestDataContainer.ClientIpAddress;
            if (clientIpAddress != "127.0.0.1")
            {
                var ipFilter = DynamicApiRepository.GetIpFilter(VendorId, SystemId, ControllerId, ApiId);
                if (!IsIpFilterValid(clientIpAddress, ipFilter))
                {
                    return new HttpResponseMessage(HttpStatusCode.Unauthorized) { Content = new StringContent("IP-Filter", Encoding.UTF8, MediaTypeConst.ApplicationJson) };
                }
            }

            //VendorKeyの認証
            if (IsAccesskey.Value)
            {
                var targetVendorId = new VendorId(PerRequestDataContainer.VendorId);
                var targetSystemId = new SystemId(PerRequestDataContainer.SystemId);
                var apiAccessVendor = DynamicApiRepository.GetApiAccessVendor(VendorId, SystemId, ControllerId, ApiId, targetVendorId, targetSystemId);

                if (!IsAccessBeyondVendorKeyValid(apiAccessVendor))
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E02401, this.RelativeUri?.Value);
            }

            // データ利用者の判定
            // Openベンダーの場合は判定しない
            // APIが自ベンダーが作ったもの：データ利用者でない場合は、APIの呼び出しができない
            // APIが他ベンダーが作ったもの：データ提供者でない場合は、APIの呼び出しができない
            // configでEnableVendorDataUseAndOffer=falseの場合は判定しない(デフォルトtrue)
            var enableVendorDataUseAndOffer = UnityCore.Resolve<bool>("EnableVendorDataUseAndOffer");
            var vendorId = new VendorId(PerRequestDataContainer.VendorId);
            if (vendorId.IsOpenVendor == false && enableVendorDataUseAndOffer == true)
            {
                var vendor = DynamicApiRepository.GetVendor(vendorId);
                // 自社で作ったAPIの場合
                if (vendor != null && this.VendorId.Value == vendorId.Value && vendor.is_data_offer == false)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10411, this.RelativeUri?.Value);
                }
                // 他社のAPIの場合
                if (vendor != null && this.VendorId.Value != vendorId.Value && vendor.is_data_use == false)
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10412, this.RelativeUri?.Value);
                }
                // vendorがnullの場合はチェックしない（基本的にはそのケースは存在しないが、MethodのUTで発生する）
            }

            // 同意と承認
            if (IsVisibleAgreement?.Value == true)
            {
                var callerVendorId = new VendorId(PerRequestDataContainer.VendorId);
                if (this.VendorId == callerVendorId)
                {
                    //所有ベンダー＝ユーザーの所属ベンダーなら同意と承認は不要
                }
                else if (DynamicApiRepository.IsApprovedAgreement(callerVendorId, ControllerId) == false)
                {

                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10413, this.RelativeUri?.Value);
                }
            }
            //同意確認
            if (UseTerms && IsRequireConsent.Value)
            {
                if (PerRequestDataContainer.OpenId == null || !DynamicApiRepository.IsAgreeToTerms(TermsGroupCode,new OpenId(PerRequestDataContainer.OpenId)))
                {
                    return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E50412, this.RelativeUri?.Value);
                }
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        /// <summary>
        /// HttpヘッダーにX-Adminの指定があるか？。それが合致しているか？
        /// </summary>
        /// <returns></returns>
        private bool IsAdminValid()
        {
            return AuthenticationRepository.IsAdmin(new AdminKeyword(PerRequestDataContainer.Xadmin), new SystemId(PerRequestDataContainer.SystemId)).Value;
        }

        /// <summary>
        /// 独自認証として認証可能か？
        /// </summary>
        /// <returns></returns>
        private bool IsOriginalAuthentication()
        {
            // 独自認証を有効にするか？
            // 無効なら、無条件にこの認証を成功として返す
            if (UnityCore.Resolve<bool>("EnableOriginalAuthentication") == false)
            {
                return true;
            }

            User user = null;
            try
            {
                if (string.IsNullOrEmpty(ApiUri.Value) == true)
                {
                    return false;
                }

                string vendorid = PerRequestDataContainer.VendorId;
                string systemId = PerRequestDataContainer.SystemId;
                user = AuthenticationRepository.Login(vendorid == null ? null : new VendorId(vendorid), systemId == null ? null : new SystemId(systemId), null);
            }
            catch (NotFoundException)
            {
                return false;
            }
            if (user == null)
            {
                return false;
            }
            return true;
        }

        private bool IsIpFilterValid(string ipAddress, IEnumerable<IpAddress> filterList)
        {
            if (!filterList.Any()) return true;

            var ipFilter = new IPFilterList();
            foreach (var filter in filterList)
            {
                if (ipFilter.Contain(ipAddress, filter.Value))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsAccessBeyondVendorKeyValid(ApiAccessVendor apiAccessVendor)
        {
            // ApiAccessVendorが存在しないということは、Apiのアクセスキーに、そのベンダーが存在しないということ。その場合はエラーとする
            if (apiAccessVendor == null) return false;

            if (!string.IsNullOrEmpty(PerRequestDataContainer.AccessBeyondVendorKey))
            {
                if (Guid.TryParse(PerRequestDataContainer.AccessBeyondVendorKey, out Guid guid))
                {
                    // GUIDのアクセスキーが存在するということは、それが合致しているかチェックする
                    return guid == apiAccessVendor.AccessKey?.Value;
                }

                return false;
            }

            // ApiAccessVendorは存在した。しかしAccessKeyが存在しなということは、GUIDキーが設定されていない（チェックボックスがされてない）ということで、認可OKとなる
            return apiAccessVendor.AccessKey?.Value == Guid.Empty;
        }

        private bool IsVendorDependencyValid()
        {
            if (IsVendor.Value)
            {
                if (string.IsNullOrEmpty(PerRequestDataContainer.VendorId) || string.IsNullOrEmpty(PerRequestDataContainer.SystemId))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsUserDependencyValid()
        {
            if (IsPerson.Value)
            {
                if (string.IsNullOrEmpty(PerRequestDataContainer.OpenId))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
