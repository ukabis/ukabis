using Microsoft.Extensions.Logging;
using JP.DataHub.Batch.AsyncDynamicApi.Models;

namespace JP.DataHub.Batch.AsyncDynamicApi.Services.Interfaces
{
    public interface IDynamicApiService
    {
        Task<(string BlobPath, TimeSpan ExecutionTime, bool isError)> DynamicApiProc(ReciveMessageModel reciveMessage, ILogger log);
    }
}
