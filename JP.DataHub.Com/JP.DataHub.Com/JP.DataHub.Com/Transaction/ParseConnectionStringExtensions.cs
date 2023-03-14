using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Transaction
{
    public static class ParseConnectionStringExtensions
    {
        public static string ToServiceBusConnectionString(this ParseConnectionString cs)
            => cs.JoinConnectionString("Endpoint", "SharedAccessKeyName", "SharedAccessKey");
        public static string ToQueueName(this ParseConnectionString cs)
            => cs["QueueName"];
    }
}
