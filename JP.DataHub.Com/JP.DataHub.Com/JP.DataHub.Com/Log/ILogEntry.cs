using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace JP.DataHub.Com.Log
{
    public interface ILogEntry
    {
        Dictionary<string, object> Values { get; }

        Type BoundaryType { get; set; }

        Guid LogGuid { get; }

        DateTime UtcDateTime { get; }

        DateTime LocalDateTime { get; }

        LogLevel Level { get; }

        string Message { get; set; }

        Exception Exception { get; set; }

        string Method { get; set; }

        int Line { get; set; }
    }
}
