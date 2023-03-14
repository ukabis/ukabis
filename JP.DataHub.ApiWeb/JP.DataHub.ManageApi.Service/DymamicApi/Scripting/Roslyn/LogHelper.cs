using System.Runtime.CompilerServices;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public class LogHelper
    {
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

        public static void Info(object message, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0)
        {
        }

        public static void Info(object message, Exception exception, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0)
        {
        }

        //[StringFormatMethod("format")]
        //public static void InfoFormat(string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args)
        //{
        //}

        //[StringFormatMethod("format")]
        //public static void InfoFormat(IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args)
        //{
        //}

        public static void Warn(object message, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0)
        {
        }

        public static void Warn(object message, Exception exception, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0)
        {
        }

        //[StringFormatMethod("format")]
        //public static void WarnFormat(string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args)
        //{
        //}

        //[StringFormatMethod("format")]
        //public static void WarnFormat(IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args)
        //{
        //}

        public static void Error(object message, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0)
        {
        }

        public static void Error(object message, Exception exception, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0)
        {
        }

        //[StringFormatMethod("format")]
        //public static void ErrorFormat(string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args)
        //{
        //}

        //[StringFormatMethod("format")]
        //public static void ErrorFormat(IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args)
        //{
        //}

        public static void Fatal(object message, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0)
        {
        }

        public static void Fatal(object message, Exception exception, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0)
        {
        }

        //[StringFormatMethod("format")]
        //public static void FatalFormat(string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args)
        //{
        //}

        //[StringFormatMethod("format")]
        //public static void FatalFormat(IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args)
        //{
        //}
    }
}