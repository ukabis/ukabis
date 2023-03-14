using JP.DataHub.Batch.AsyncDynamicApi.Models;
using Microsoft.Extensions.Logging;

namespace JP.DataHub.Batch.AsyncDynamicApi.Services.Interfaces
{
    public interface IProrcessMananagementService
    {
        public Task Execute(ReciveMessageModel message, ILogger Log);
    }
}
