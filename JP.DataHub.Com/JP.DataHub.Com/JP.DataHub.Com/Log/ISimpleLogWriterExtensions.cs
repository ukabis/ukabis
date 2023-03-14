﻿using Newtonsoft.Json;
using NLog;
using System;
using System.Runtime.CompilerServices;

namespace JP.DataHub.Com.Log
{
    public static class SimpleLogWriterExtensions
    {
        private static LogEntry CreateLogEntry(LogLevel level, string message, System.Exception exception, object properties, string caller, int line)
        {
            var entity = new LogEntry(level)
            {
                BoundaryType = typeof(JPDataHubLoggerExtensions),
                Message = message,
                Line = line,
                Method = caller,
                Exception = exception,
            };

            //entity.Values.Add("_Message", message);
            entity.Values.Add("_Line", line);
            entity.Values.Add("_Method", caller);
            //entity.Values.Add("_Exception", exception?.ToString());

            var jsonProperties = JsonConvert.SerializeObject(properties, Formatting.None);
            entity.Values["_Values"] = jsonProperties;

            return entity;
        }

        public static void Trace(this ISimpleLogWriter me,
            object message,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Trace, message?.ToString(), null, properties, caller, line));

        public static void Trace_SetCallerInfo(this ISimpleLogWriter me,
          object message, string caller, int line,
           object properties = null) => me.Write(CreateLogEntry(LogLevel.Trace, message?.ToString(), null, properties, caller, line));

        public static void Trace_SetCallerInfo(this ISimpleLogWriter me,
            object message, Exception exception,
            string caller, int line, object properties = null) => me.Write(CreateLogEntry(LogLevel.Trace, message?.ToString(), exception, properties, caller, line));

        public static void Error_SetCallerInfo(this ISimpleLogWriter me,
            object message, Exception exception,
            string caller, int line, object properties = null) => me.Write(CreateLogEntry(LogLevel.Error, message?.ToString(), exception, properties, caller, line));

        public static void Trace(this ISimpleLogWriter me,
            object message, Exception exception,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Trace, message?.ToString(), exception, properties, caller, line));

        public static void TraceFormat(this ISimpleLogWriter me,
            string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Trace, string.Format(format, args), null, properties, caller, line));

        public static void TraceFormat(this ISimpleLogWriter me,
            string format, object arg0,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Trace, string.Format(format, arg0), null, properties, caller, line));

        public static void TraceFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Trace, string.Format(format, arg0, arg1), null, properties, caller, line));

        public static void TraceFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1, object arg2,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Trace, string.Format(format, arg0, arg1, arg2), null, properties, caller, line));

        public static void TraceFormat(this ISimpleLogWriter me,
            IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Trace, string.Format(format, args, provider), null, properties, caller, line));

        public static void Debug(this ISimpleLogWriter me,
            object message,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Debug, message?.ToString(), null, properties, caller, line));

        public static void Debug(this ISimpleLogWriter me,
            object message, Exception exception,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Debug, message?.ToString(), exception, properties, caller, line));

        public static void DebugFormat(this ISimpleLogWriter me,
            string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Debug, string.Format(format, args), null, properties, caller, line));

        public static void DebugFormat(this ISimpleLogWriter me,
            string format, object arg0,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Debug, string.Format(format, arg0), null, properties, caller, line));

        public static void DebugFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Debug, string.Format(format, arg0, arg1), null, properties, caller, line));

        public static void DebugFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1, object arg2,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Debug, string.Format(format, arg0, arg1, arg2), null, properties, caller, line));

        public static void DebugFormat(this ISimpleLogWriter me,
            IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Debug, string.Format(format, args, provider), null, properties, caller, line));

        public static void Info(this ISimpleLogWriter me,
              object message,
              object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Info, message?.ToString(), null, properties, caller, line));

        public static void Info(this ISimpleLogWriter me,
            object message, Exception exception,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Info, message?.ToString(), exception, properties, caller, line));

        public static void InfoFormat(this ISimpleLogWriter me,
            string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Info, string.Format(format, args), null, properties, caller, line));

        public static void InfoFormat(this ISimpleLogWriter me,
            string format, object arg0,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Info, string.Format(format, arg0), null, properties, caller, line));


        public static void InfoFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Info, string.Format(format, arg0, arg1), null, properties, caller, line));


        public static void InfoFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1, object arg2,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Info, string.Format(format, arg0, arg1, arg2), null, properties, caller, line));


        public static void InfoFormat(this ISimpleLogWriter me,
            IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Info, string.Format(format, args, provider), null, properties, caller, line));

        public static void Warn(this ISimpleLogWriter me,
            object message,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Warn, message?.ToString(), null, properties, caller, line));

        public static void Warn(this ISimpleLogWriter me,
            object message, Exception exception,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Warn, message?.ToString(), exception, properties, caller, line));


        public static void WarnFormat(this ISimpleLogWriter me,
            string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Warn, string.Format(format, args), null, properties, caller, line));


        public static void WarnFormat(this ISimpleLogWriter me,
            string format, object arg0,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Warn, string.Format(format, arg0), null, properties, caller, line));


        public static void WarnFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Warn, string.Format(format, arg0, arg1), null, properties, caller, line));


        public static void WarnFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1, object arg2,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Warn, string.Format(format, arg0, arg1, arg2), null, properties, caller, line));


        public static void WarnFormat(this ISimpleLogWriter me,
            IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Warn, string.Format(format, args, provider), null, properties, caller, line));

        public static void Error(this ISimpleLogWriter me,
            object message,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Error, message?.ToString(), null, properties, caller, line));

        public static void Error(this ISimpleLogWriter me,
            object message, Exception exception,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Error, message?.ToString(), exception, properties, caller, line));


        public static void ErrorFormat(this ISimpleLogWriter me,
            string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Error, string.Format(format, args), null, properties, caller, line));


        public static void ErrorFormat(this ISimpleLogWriter me,
            string format, object arg0,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Error, string.Format(format, arg0), null, properties, caller, line));


        public static void ErrorFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Error, string.Format(format, arg0, arg1), null, properties, caller, line));


        public static void ErrorFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1, object arg2,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Error, string.Format(format, arg0, arg1, arg2), null, properties, caller, line));


        public static void ErrorFormat(this ISimpleLogWriter me,
            IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Error, string.Format(format, args, provider), null, properties, caller, line));

        public static void Fatal(this ISimpleLogWriter me,
            object message,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Fatal, message?.ToString(), null, properties, caller, line));

        public static void Fatal(this ISimpleLogWriter me,
            object message, Exception exception,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Fatal, message?.ToString(), exception, properties, caller, line));


        public static void FatalFormat(this ISimpleLogWriter me,
            string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Fatal, string.Format(format, args), null, properties, caller, line));


        public static void FatalFormat(this ISimpleLogWriter me,
            string format, object arg0,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Fatal, string.Format(format, arg0), null, properties, caller, line));


        public static void FatalFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Fatal, string.Format(format, arg0, arg1), null, properties, caller, line));


        public static void FatalFormat(this ISimpleLogWriter me,
            string format, object arg0, object arg1, object arg2,
            object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0) => me.Write(CreateLogEntry(LogLevel.Fatal, string.Format(format, arg0, arg1, arg2), null, properties, caller, line));


        public static void FatalFormat(this ISimpleLogWriter me,
            IFormatProvider provider, string format, object properties = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = 0, params object[] args) => me.Write(CreateLogEntry(LogLevel.Fatal, string.Format(format, args, provider), null, properties, caller, line));
    }
}
