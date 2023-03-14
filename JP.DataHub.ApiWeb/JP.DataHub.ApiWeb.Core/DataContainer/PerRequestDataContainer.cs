using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using JP.DataHub.Com.TimeZone;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.DataContainer;

namespace JP.DataHub.ApiWeb.Core.DataContainer
{
    public class PerRequestDataContainer : ApiDataContainer, IPerRequestDataContainer
    {
        private Lazy<DateTimeUtil> dateTimeUtil = new Lazy<DateTimeUtil>(() => new DateTimeUtil("yyyy/MM/dd",
            new string[] { "yyyy/MM/dd hh:mm:ss tt", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd h:mm:ss" }, "yyyy/M/d"));

        private static Lazy<IMapper> _mapper = new Lazy<IMapper>(() =>
        {
            var mapping = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IPerRequestDataContainer, IPerRequestDataContainer>();
            });
            return mapping.CreateMapper();
        });

        private static IMapper Mapper => _mapper.Value;

        private IConfiguration _configuration { get; } = UnityCore.Resolve<IConfiguration>();

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string ContainerGuid { get; } = Guid.NewGuid().ToString();

        public string ClientName { get; set; }
        public string OriginalAccessToken { get; set; }

        public string VendorId { get; set; }

        public string SystemId { get; set; }

        public string FunctionGroup { get; set; }

        public string OpenId { get; set; }

        public bool VendorSystemAuthenticated { get; set; }

        public Dictionary<string, string> Claims { get; set; }

        public bool IsUserAuthenticated { get; set; } = false;
        public bool IsDeveloper { get; set; }
        public Guid UserId { get; set; } = Guid.Empty;
        public string UserAccount { get; set; } = null;
        [Obsolete("APIでは使用しません。")]
        public string UserName { get; set; } = null;
        public string DateInputFormat { get; set; } = null;
        public CultureInfo CultureInfo { get; set; } = null;
        public IList<EventLogType> SuppressAudit { get; } = new List<EventLogType>();

        public string Xadmin { get => this._Xadmin.GetValue(); set => this._Xadmin.SetValue(value); }

        public int? Xversion { get => this._Xversion.GetValue(); set => this._Xversion.SetValue(value); }

        public bool XgetInternalAllField { get => this._XgetInternalAllField.GetValue(); set => this._XgetInternalAllField.SetValue(value); }

        public bool XNotAuthenticationRequest { get; set; } = false;

        public string XRequestContinuation { get => this._XRequestContinuation.GetValue(); set => this._XRequestContinuation.SetValue(value); }
        public string ClientIpAddress { get; set; }

        public string AccessBeyondVendorKey { get => this._AccessBeyondVendorKey.GetValue(); set => this._AccessBeyondVendorKey.SetValue(value); }

        public bool XAsync { get => this._XAsync.GetValue(); set => this._XAsync.SetValue(value); }
        public bool XScriptRuntimeLogException { get => this._XScriptRuntimeLogException.GetValue(); set => this._XScriptRuntimeLogException.SetValue(value); }

        public ConcurrentDictionary<string, string> XResourceSharingWith { get; set; }

        public string XResourceSharingPerson { get => this._XResourceSharingPerson.GetValue(); set => this._XResourceSharingPerson.SetValue(value); }

        public bool XVendorSystemCertificateAuthenticated { get; set; }

        public string CommonKeyId { get => this._CommonKeyId.GetValue(); set => this._CommonKeyId.SetValue(value); }

        public string AuthorizationError { get; set; }

        public bool IsInternalCall { get; set; }
        public string InternalCallKeyword { get; set; }

        public bool IsSkipJsonFormatProtect { get; set; } = false;

        public Dictionary<string, List<string>> RequestHeaders
        {
            get => this._requestHeaders;
            set
            {
                this._requestHeaders = value == null ? null : new Dictionary<string, List<string>>(value, StringComparer.OrdinalIgnoreCase);
                // プロパティ同期フィールドを更新
                this.SwitchRequestHeaders();
            }
        }

        public ConcurrentDictionary<string, string> LoggingIdUrlList { get; set; } = new ConcurrentDictionary<string, string>();

        public bool IsDynamicApiRequest { get; set; } = false;

        public DateTimeUtil GetDateTimeUtil() => dateTimeUtil.Value;

        /// <summary>
        /// 運営会社ユーザか？
        /// </summary>
        public bool IsOperatingVendorUser => (_configuration.GetSection("AppConfig:OperatingVendorVendorId").Get<string[]>()).Select(x => x.ToLower()).Contains(VendorId?.ToLower());

        /// <summary>
        /// X-InternalRepositoryが指定されたか？
        /// </summary>
        public bool XInternalRepository { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> RepositoryInfo { get; } = new Dictionary<string, string>();

        /// <summary>
        /// 更新処理時にコンフリクトが発生したときに処理を止めるか
        /// True コンフリクトが発生時た時点で処理をやめる False コンフリクトが発生しても処理を最後まで続行する
        /// </summary>
        public bool XRegisterConflictStop { get => this._XRegisterConflictStop.GetValue(); set => this._XRegisterConflictStop.SetValue(value); }

        /// <summary>
        /// リソースの楽観排他設定を無視するか
        /// </summary>
        public bool XNoOptimistic { get => this._XNoOptimistic.GetValue(); set => this._XNoOptimistic.SetValue(value); }

        /// <summary>
        /// 履歴の$Referenceを解決させるための指定
        /// </summary>
        public string XReferenceHistory { get => this._XReferenceHistory.GetValue(); set => this._XReferenceHistory.SetValue(value); }

        /// <summary>
        /// すべてのProfilerを返すか？
        /// </summary>
        public bool XProfiler { get; set; }

        /// <summary>
        /// 全てのキャッシュの状態を返すか？
        /// </summary>
        public bool XProfilerCache { get; set; }

        /// <summary>
        /// ProfileのQueryを返すか？
        /// </summary>
        public bool XProfilerQuery { get; set; }

        /// <summary>
        /// APIツリー情報
        /// </summary>
        public object ApiTreeCache { get; set; }

        /// <summary>
        /// リソースに対するAPIキャッシュ
        /// </summary>
        public ConcurrentDictionary<Guid, object> ResourceApiCache { get; set; }

        public string XValidateWithBlockchainVersion { get => this._XValidateWithBlockchainVersion.GetValue(); set => this._XValidateWithBlockchainVersion.SetValue(value); }

        /// <summary>
        /// JsonValidatorでValidationErrorをOFF（返さない）場合に、このヘッダーが指定された場合には返すようにする
        /// </summary>
        public bool ReturnNeedsJsonValidatorErrorDetail { get; set; }

        public bool ProfilerDisabled { get; set; }

        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public object Argument { get; set; }

        public DateTime Time { get; set; }
        public List<string> XUserResourceSharing { get; set; }

        private Dictionary<string, List<string>> _requestHeaders;

        #region RequestHeaderSync
        // RequestHeadersと各プロパティを同期するためのオブジェクト
        // FromRequestHeaderAttributeで指定したヘッダー項目に対応する
        // リクエストヘッダーを新しく追加する場合は属性付与と同期オブジェクトも作成すること
        private RequestHeaderSync<int?> _Xversion = null;
        private RequestHeaderSync<string> _Xadmin = null;
        private RequestHeaderSync<string> _AccessBeyondVendorKey = null;
        private RequestHeaderSync<bool> _XgetInternalAllField = null;
        private RequestHeaderSync<string> _XRequestContinuation = null;
        private RequestHeaderSync<bool> _XAsync = null;
        private RequestHeaderSync<bool> _XScriptRuntimeLogException = null;
        private RequestHeaderSync<string> _XResourceSharingPerson = null;
        private RequestHeaderSync<string> _CommonKeyId = null;
        private RequestHeaderSync<string> _XValidateWithBlockchainVersion = null;
        private RequestHeaderSync<bool> _XRegisterConflictStop = null;
        private RequestHeaderSync<bool> _XNoOptimistic = null;
        private RequestHeaderSync<string> _XReferenceHistory = null;

        #endregion

        public PerRequestDataContainer()
        {
            // リクエストヘッダーと同期オブジェクトを作成
            this.RequestHeaders = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }


        // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// 自身をディープコピーします。
        /// </summary>
        /// <returns>コピーしたIPerRequestDataContainerインスタンス</returns>
        public IPerRequestDataContainer DeepCopy()
        {
            // まず簡易コピー
            var clone = (PerRequestDataContainer)MemberwiseClone();
            // 参照型は新しくオブジェクトを生成する
            clone.XResourceSharingWith = this.XResourceSharingWith == null ? null : new ConcurrentDictionary<string, string>(this.XResourceSharingWith);
            clone.LoggingIdUrlList = this.LoggingIdUrlList == null ? null : new ConcurrentDictionary<string, string>(this.LoggingIdUrlList);
            clone.ResourceApiCache = this.ResourceApiCache == null ? null : new ConcurrentDictionary<Guid, object>(this.ResourceApiCache);
            return clone;
        }

        public IPerRequestDataContainer DeepCopy(IPerRequestDataContainer clone)
        {
            Mapper.Map(this, clone);
            // 参照型は新しくオブジェクトを生成する
            clone.XResourceSharingWith = this.XResourceSharingWith == null ? null : new ConcurrentDictionary<string, string>(this.XResourceSharingWith);
            clone.LoggingIdUrlList = this.LoggingIdUrlList == null ? null : new ConcurrentDictionary<string, string>(this.LoggingIdUrlList);
            clone.ResourceApiCache = this.ResourceApiCache == null ? null : new ConcurrentDictionary<Guid, object>(this.ResourceApiCache);
            return clone;
        }

        public void MergeRequestHeaders(Dictionary<string, List<string>> headers)
        {
            if (headers == null)
            {
                return;
            }
            if (this.RequestHeaders == null)
            {
                // リクエストヘッダーと同期オブジェクトを作成
                this.RequestHeaders = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
            }
            headers.ToList().ForEach(x => this.RequestHeaders[x.Key] = x.Value);
        }

        private void SwitchRequestHeaders()
        {
            // RequestHeadersの変更をプロパティ同期フィールド(RequestHeaderSync)に反映するため
            // プロパティ同期フィールドにRequestHeaderSyncを再設定する
            GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                     .Where(x => x.Name.StartsWith("_") && x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(RequestHeaderSync<>))
                     .ToList()
                     .ForEach(x =>
                     {
                         // フィールド名から先頭アンダースコアを除外した文字列でプロパティを取得
                         var prop = this.GetType().GetProperty(x.Name.Substring(1), BindingFlags.Instance | BindingFlags.Public);
                         // プロパティに付与された属性からリクエストヘッダー名を取得
                         var headerName = typeof(IPerRequestDataContainer).GetProperty(prop.Name)
                                                                          .GetCustomAttribute<FromRequestHeaderAttribute>()
                                                                          .HeaderName;
                         // RequestHeaderSyncインスタンスを作成してフィールドに設定
                         var syncObj = Activator.CreateInstance(typeof(RequestHeaderSync<>)
                                                .MakeGenericType(new Type[] { prop.PropertyType }), new object[] { this.RequestHeaders, headerName });
                         x.SetValue(this, syncObj);
                     });
        }
    }
}
