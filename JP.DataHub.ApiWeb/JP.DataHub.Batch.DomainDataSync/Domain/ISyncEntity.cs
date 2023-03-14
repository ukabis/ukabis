using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.DomainDataSync.Domain
{
    public interface ISyncEntity
    {
        int Sync(string eventName, string pkValue);
        bool SyncAll(bool ignoreUpdateDate);
        Task ClearCache(string eventName);
        Task ClearCacheAll();
    }
}
