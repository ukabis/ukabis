using JP.DataHub.Batch.TrailEventProcess.Models;
using JP.DataHub.Com.SQL;

namespace JP.DataHub.Batch.TrailEventProcess.Repository.Interfaces
{
    public interface IPhysicalRepositoryGroupRepository
    {
        List<PhysicalRepositoryResultModel> GetPhysicalRepository(string repositoryGroupId, TwowaySqlParser.DatabaseType dbType);
    }
}
