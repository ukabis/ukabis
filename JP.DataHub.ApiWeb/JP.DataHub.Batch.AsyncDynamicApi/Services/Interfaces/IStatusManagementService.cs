using Microsoft.Extensions.Logging;
using JP.DataHub.Batch.AsyncDynamicApi.Models;

namespace JP.DataHub.Batch.AsyncDynamicApi.Services.Interfaces
{
    public interface IStatusManagementService
    {
        Task<AsyncDynamicApiStatusModel> GetStatus(string requestId, ILogger log);
        Task SaveStatus(AsyncDynamicApiStatusModel status, ILogger log);
        DateTime DeleteRequest(string requestId, ILogger log);
        Task<bool> ErrorStatus(string requestId, ILogger log);
        Task<bool> UpdateStatus(StatusManagementModel statusArgs, ILogger log);
        Task<bool> IsRetryOver(string requestId, ILogger log);
    }
}
