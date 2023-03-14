using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.DomainDataSync.Models
{
    public class DomainDataSyncEventModel
    {
        public string EventName { get; set; }

        public string PkValue { get; set; }
    }
}
