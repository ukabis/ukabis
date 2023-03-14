using System.Text;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// スクリプト実行時のログを出力するためのヘルパークラス
    /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
    /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
    /// </summary>
    public class ScriptHelper
    {
        private static Lazy<JPDataHubLogger> s_logger = new Lazy<JPDataHubLogger>(() => new JPDataHubLogger(typeof(ScriptHelper)));
        private JPDataHubLogger _logger = s_logger.Value;

        internal bool IsEnableScriptRuntimeLogService => UnityCore.Resolve<bool>("IsEnableScriptRuntimeLogService");

        internal ScriptHelper(Guid? providerVendorId)
        {
            ProviderVendorId = providerVendorId ?? throw new ArgumentNullException("ProviderVendorId");
            ScriptRuntimeLogId = Guid.NewGuid();
        }
        private Guid ProviderVendorId { get; }
        internal Guid ScriptRuntimeLogId { get; }
        internal bool IsExecutedPrintf { get; private set; }
        internal IDomainEventPublisher Publisher => UnityCore.Resolve<IDomainEventPublisher>();
        private const string logMediaType = "text/plain";


        /// <summary>
        /// <ja>引数に指定した文字列をログとして出力します。
        /// ログはAPI実行時に返却されるX-ScriptRuntimelog-Idを/API/ScriptRuntimeLog/Get?logId={value}のパラメータとして実行することで取得できます。</ja>
        /// <en>To be translated</en>
        /// </summary>
        /// <param name="src">
        /// <ja>書式指定文字列</ja>
        /// <en>To be translated</en>
        /// </param>
        /// <param name="args">
        /// <ja>src中の書式指定項目に対応するオブジェクト</ja>
        /// <en>To be translated</en>
        /// </param>
        public void Printf(string src, params object[] args)
        {
            if (string.IsNullOrEmpty(src)) { return; }
            if (!IsEnableScriptRuntimeLogService)
            {
                _logger.Error("Printf() Executed in unabling ScriptRuntimeLogService environment. Check relevant configuration.");
                return;
            }
            this.IsExecutedPrintf = true;
            var logString = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.ffff ") + string.Format(src, args) + Environment.NewLine;

            Publisher.Publish(new ScriptRuntimeLogWriteEventData(DateTime.UtcNow,
                ProviderVendorId, ScriptRuntimeLogId, "Runtime.log", logString,
                logMediaType));
        }


        /// <summary>
        /// <ja>引数に指定した例外とその内部例外をログとして出力します。
        /// ログはAPI実行時に返却されるX-ScriptRuntimelog-Idを/API/ScriptRuntimeLog/Get?logId={value}のパラメータとして実行することで取得できます。</ja>
        /// <en>To be translated</en>
        /// </summary>
        /// <param name="e">
        /// <ja>例外</ja>
        /// <en>To be translated</en>
        /// </param>
        public void PrintfException(Exception e)
        {
            if (!IsEnableScriptRuntimeLogService)
            {
                _logger.Error("PrintfException() Executed in unabling ScriptRuntimeLogService environment. Check relevant configuration.");
                return;
            }
            var exceptionMessage = new StringBuilder();
            exceptionMessage.AppendLine($"{ e.Message}");
            exceptionMessage.AppendLine($"{ e.StackTrace}");
            ExtractInnerExceptionRecursive(e, ref exceptionMessage);
            Printf(exceptionMessage.ToString());
        }

        private void ExtractInnerExceptionRecursive(Exception e, ref StringBuilder exceptionInfo)
        {
            if (e.InnerException != null)
            {
                exceptionInfo.AppendLine(e.InnerException.Message);
                exceptionInfo.AppendLine(e.InnerException.StackTrace);
                ExtractInnerExceptionRecursive(e.InnerException, ref exceptionInfo);
            }
        }
    }
}
