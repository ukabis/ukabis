using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.Revoke
{
    public class RevokeSettings
    {
        public int MaxNumberOfAttempts { get; set; }
        public int RetryDelaySec { get; set; }
        public int EventHubMaxBatchSize { get; set; }
        public string EventHubName { get; set; }

    }
}
