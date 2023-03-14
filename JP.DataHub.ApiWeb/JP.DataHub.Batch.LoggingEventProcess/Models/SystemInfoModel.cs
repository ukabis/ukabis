using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingEventProcess.Models
{
    public class SystemInfoModel
    {
        public string SystemId { get; }

        public string SystemName { get; }

        public SystemInfoModel(string systemId, string systemName)
        {
            SystemId = systemId;
            SystemName = systemName;
        }

    }
}