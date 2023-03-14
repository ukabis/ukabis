using JP.DataHub.Batch.TrailEventProcess.Models;
using Microsoft.Extensions.Logging;

namespace JP.DataHub.Batch.TrailEventProcess.Services.Interfaces
{
    public interface IProrcessMananagementService
    {
        public Task Execute(TrailEventModel trailEvent, string trailId, ILogger Log);
    }
}
