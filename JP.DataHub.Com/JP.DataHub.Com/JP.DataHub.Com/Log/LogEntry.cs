using NLog;
using System;
using System.Collections.Generic;

namespace JP.DataHub.Com.Log
{
    // .NET6
    public class LogEntry : ILogEntry
    {
        public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();

        public Type BoundaryType { get; set; }

        public Guid LogGuid { get; } = Guid.NewGuid();

        public DateTime UtcDateTime { get; } = DateTime.UtcNow;

        public DateTime LocalDateTime { get; } = DateTime.Now;

        public LogLevel Level { get; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public string Method { get; set; }

        public int Line { get; set; }

        public LogEntry(LogLevel level)
        {
            this.Level = level;
        }
   }
}