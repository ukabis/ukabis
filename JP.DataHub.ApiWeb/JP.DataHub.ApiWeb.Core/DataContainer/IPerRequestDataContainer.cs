using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.TimeZone;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Api.Core.DataContainer;

namespace JP.DataHub.ApiWeb.Core.DataContainer
{
    // .NET6
    /// <summary>
    /// Interface of PerRequestDataContainer.
    /// </summary>
    public interface IPerRequestDataContainer : IApiDataContainer, IDisposable
    {
        /// <summary>リクエストごとに生成される乱数値。</summary>
        string ContainerGuid { get; }

        /// <summary>
        /// クライアント名称 基本的には管理画面由来のリクエストの識別目的
        /// </summary>
        string ClientName { get; set; }

        /// <summary>
        /// 機能グループ
        /// </summary>
        string FunctionGroup { get; set; }

        /// <summary>ユーザー認証済みであるかどうかを示す。</summary>
        bool IsUserAuthenticated { get; set; }

        /// <summary>認証済みユーザーのID(Guid)値</summary>
        Guid UserId { get; set; }

        /// <summary>認証済みユーザーのAccount</summary>
        string UserAccount { get; set; }

        /// <summary>認証済みユーザーの名前(First+Middle+Famiry)</summary>
        string UserName { get; set; }

        /// <summary>
        /// Xadmin
        /// </summary>
        [FromRequestHeader("X-Admin")]
        string Xadmin { get; set; }

        [FromRequestHeader("X-AccessBeyondVendorKey")]
        string AccessBeyondVendorKey { get; set; }

        /// <summary>
        /// Xversion
        /// </summary>
        [FromRequestHeader("X-Version")]
        int? Xversion { get; set; }

        bool XVendorSystemCertificateAuthenticated { get; set; }

        /// <summary>
        /// X-GetInternalAllField
        /// </summary>
        [FromRequestHeader("X-GetInternalAllField")]
        bool XgetInternalAllField { get; set; }

        /// <summary>
        /// X-XRequestContinuation
        /// </summary>
        [FromRequestHeader("X-RequestContinuation")]
        string XRequestContinuation { get; set; }

        /// <summary>
        /// 非同期実行するか否か
        /// True 非同期 False 同期実行
        /// </summary>
        [FromRequestHeader("X-IsAsync")]
        bool XAsync { get; set; }

        /// <summary>
        /// RoslynScript実行時に使用
        /// 例外発生時にBlobへログ出力するかどうか
        /// True 出力する False 出力しない
        /// </summary>
        [FromRequestHeader("X-ScriptRuntimeLog-Exception")]
        bool XScriptRuntimeLogException { get; set; }

        /// <summary>
        /// 共有データの取得情報
        /// </summary>
        ConcurrentDictionary<string, string> XResourceSharingWith { get; set; }

        /// <summary>
        /// 個人の共有データの取得情報
        /// </summary>
        [FromRequestHeader("X-ResourceSharingPerson")]
        string XResourceSharingPerson { get; set; }

        /// <summary>
        /// Not Authentication
        /// </summary>
        bool XNotAuthenticationRequest { get; set; }

        /// <summary>共通鍵ID</summary>
        [FromRequestHeader("X-CommonKeyId")]
        string CommonKeyId { get; set; }

        /// <summary>証跡ログ発行を抑止。</summary>
        IList<EventLogType> SuppressAudit { get; }

        /// <summary>
        /// 運営会社ユーザか？
        /// </summary>
        bool IsOperatingVendorUser { get; }

        bool XInternalRepository { get; set; }

        Dictionary<string, string> RepositoryInfo { get; }

        /// <summary>
        /// ブロックチェーンの検証APIで使用 履歴のバージョンを指定する
        /// </summary>
        [FromRequestHeader("X-ValidateWithBlockchain-Version")]
        string XValidateWithBlockchainVersion { get; set; }
        bool IsInternalCall { get; set; }
        string InternalCallKeyword { get; set; }
        bool IsSkipJsonFormatProtect { get; set; }
        Dictionary<string, List<string>> RequestHeaders { get; set; }
        ConcurrentDictionary<string, string> LoggingIdUrlList { get; set; }
        bool IsDynamicApiRequest { get; set; }
        /// <summary>
        /// 更新処理時にコンフリクトが発生したときに処理を止めるか
        /// True コンフリクトが発生時た時点で処理をやめる False コンフリクトが発生しても処理を最後まで続行する
        /// </summary>
        [FromRequestHeader("X-RegisterConflictStop")]
        bool XRegisterConflictStop { get; set; }

        /// <summary>
        /// リソースの楽観排他設定を無視するか
        /// </summary>
        [FromRequestHeader("X-NoOptimistic")]
        bool XNoOptimistic { get; set; }

        /// <summary>
        /// 履歴の$Referenceを解決させるための指定
        /// </summary>
        [FromRequestHeader("X-ReferenceHistory")]
        string XReferenceHistory { get; set; }

        /// <summary>
        /// すべてのProfilerを返すか？
        /// </summary>
        bool XProfiler { get; set; }

        /// <summary>
        /// 全てのキャッシュの状態を返すか？
        /// </summary>
        bool XProfilerCache { get; set; }

        /// <summary>
        /// ProfileのQueryを返すか？
        /// </summary>
        bool XProfilerQuery { get; set; }

        /// <summary>
        /// APIツリー情報
        /// </summary>
        object ApiTreeCache { get; set; }

        /// <summary>
        /// リソースに対するAPIキャッシュ
        /// </summary>
        ConcurrentDictionary<Guid, object> ResourceApiCache { get; set; }

        /// <summary>
        /// JsonValidatorでValidationErrorをOFF（返さない）場合に、このヘッダーが指定された場合には返すようにする
        /// </summary>
        bool ReturnNeedsJsonValidatorErrorDetail { get; set; }

        /// <summary>
        /// 認可されたデータの取得情報
        /// </summary>
        List<string> XUserResourceSharing { get; set; }

        /// <summary>
        /// 自身をディープコピーします。
        /// </summary>
        /// <returns>コピーしたIPerRequestDataContainerインスタンス</returns>
        IPerRequestDataContainer DeepCopy();

        /// <summary>
        /// dstに対しディープコピーする
        /// </summary>
        /// <returns>コピーしたIPerRequestDataContainerインスタンス</returns>
        IPerRequestDataContainer DeepCopy(IPerRequestDataContainer dst);

        /// <summary>
        /// 指定したリクエストヘッダーをマージする。
        /// </summary>
        /// <param name="headers">マージされるリクエストヘッダー。</param>
        void MergeRequestHeaders(Dictionary<string, List<string>> headers);
    }
}
