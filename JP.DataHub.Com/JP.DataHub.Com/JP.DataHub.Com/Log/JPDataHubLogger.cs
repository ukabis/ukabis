using NLog;
using System;
using System.Diagnostics;
using Unity.Interception.Utilities;
using ILogger = NLog.ILogger;

namespace JP.DataHub.Com.Log
{
    [DebuggerStepThrough]
    public class JPDataHubLogger : ISimpleLogWriter
    {
        public ILogger NLogLogger { get; }

        public JPDataHubLogger(Type type)
        {
            NLogLogger = LogManager.GetLogger(type.Name); 
        }

        public JPDataHubLogger(string name)
        {
            NLogLogger = NLog.LogManager.GetLogger(name);
        }


        // ～ログの構造化～
        // ログはデータであるべき。自由なテキスト行じゃダメ。
        // データ構造は決められた型(LogEntry)に限定してしまいたい。
        // とは言っても、不便なので ILog とほぼ互換性のある拡張メソッドで対応。
        public void Write(ILogEntry logEntry)
        {
            var loggingEvent = new LogEventInfo(logEntry.Level, NLogLogger.Name, logEntry.Message)
            {
                Exception = logEntry.Exception
            };
            logEntry.Values.ForEach(x => loggingEvent.Properties[x.Key] = x.Value);
            NLogLogger.Log(loggingEvent);
        }
    }
}
