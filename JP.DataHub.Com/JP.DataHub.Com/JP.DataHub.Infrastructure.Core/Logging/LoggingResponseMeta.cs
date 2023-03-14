using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.Logging
{
    internal class LoggingResponseMeta
    {
        public int status_code { get; set; }
        public Dictionary<string, List<string>> header { get; set; }
    }
}
