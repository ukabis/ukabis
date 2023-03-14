using JP.DataHub.Batch.DomainDataSync.Models;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.DomainDataSync.Usecase
{
    public interface ISyncUsecase
    {
        Task Sync(DomainDataSyncEventModel message);
        Task SyncAll(bool ignoreUpdateDate);
    }
}
