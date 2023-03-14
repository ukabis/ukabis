using System.Runtime.CompilerServices;
using JP.DataHub.Com.Log;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
    /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
    /// </summary>
    [RoslynScriptHelp]
    public class LogHelper
    {
        private static JPDataHubLogger logger
        {
            get
            {
                lock (_obj)
                {
                    if (_logger == null)
                    {
                        _logger = new Lazy<JPDataHubLogger>(() => new JPDataHubLogger(typeof(LogHelper)));
                    }
                }
                return _logger.Value;
            }
        }
        private static Lazy<JPDataHubLogger> _logger;
        private static object _obj = new object();

        //---------------------------------------------------------------------
        // Trace及びDebugは禁止クラスのチェックでエラーとなるため一旦割愛
        //---------------------------------------------------------------------
        //public static void Trace(object message, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Trace(message, properties, caller, line);

        //public static void Trace(object message, Exception exception, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Trace(message, exception, properties, caller, line);

        //[StringFormatMethod("format")]
        //public static void TraceFormat(string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.TraceFormat(format, properties, caller, line, args);

        //[StringFormatMethod("format")]
        //public static void TraceFormat(IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.TraceFormat(provider, format, properties, caller, line, args);

        //public static void Debug(object message, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Debug(message, properties, caller, line);

        //public static void Debug(object message, Exception exception, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Debug(message, exception, properties, caller, line);

        //[StringFormatMethod("format")]
        //public static void DebugFormat(string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.DebugFormat(format, properties, caller, line, args);

        //[StringFormatMethod("format")]
        //public static void DebugFormat(IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.DebugFormat(provider, format, properties, caller, line, args);

        public static void Info(object message, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Info(message, properties, caller, line);

        public static void Info(object message, Exception exception, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Info(message, exception, properties, caller, line);

        //[StringFormatMethod("format")]
        //public static void InfoFormat(string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.InfoFormat(format, properties, caller, line, args);

        //[StringFormatMethod("format")]
        //public static void InfoFormat(IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.InfoFormat(provider, format, properties, caller, line, args);

        public static void Warn(object message, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Warn(message, properties, caller, line);

        public static void Warn(object message, Exception exception, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Warn(message, exception, properties, caller, line);

        //[StringFormatMethod("format")]
        //public static void WarnFormat(string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.WarnFormat(format, properties, caller, line, args);

        //[StringFormatMethod("format")]
        //public static void WarnFormat(IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.WarnFormat(provider, format, properties, caller, line, args);

        public static void Error(object message, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Error(message, properties, caller, line);

        public static void Error(object message, Exception exception, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Error(message, exception, properties, caller, line);

        //[StringFormatMethod("format")]
        //public static void ErrorFormat(string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.ErrorFormat(format, properties, caller, line, args);

        //[StringFormatMethod("format")]
        //public static void ErrorFormat(IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.ErrorFormat(provider, format, properties, caller, line, args);

        public static void Fatal(object message, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Fatal(message, properties, caller, line);

        public static void Fatal(object message, Exception exception, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => logger.Fatal(message, exception, properties, caller, line);

        //[StringFormatMethod("format")]
        //public static void FatalFormat(string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.FatalFormat(format, properties, caller, line, args);

        //[StringFormatMethod("format")]
        //public static void FatalFormat(IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => logger.FatalFormat(provider, format, properties, caller, line, args);
    }
}