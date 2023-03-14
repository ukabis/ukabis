using JP.DataHub.Batch.TrailEventProcess.Models;
using JP.DataHub.Batch.TrailEventProcess.Repository.Interfaces;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.SQL;

namespace JP.DataHub.Batch.TrailEventProcess.Repository.Impls
{
    public class PhysicalRepositoryGroupRepository : IPhysicalRepositoryGroupRepository
    {
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));

        public List<PhysicalRepositoryResultModel> GetPhysicalRepository(string repositoryGroupId, TwowaySqlParser.DatabaseType dbType)
        {
            var sql = ""; 
            if (dbType.Equals(TwowaySqlParser.DatabaseType.Oracle))
            {
                sql = @"
SELECT 
    p.physical_repository_id AS PhysicalRepositoryId,
    p.repository_group_id AS RepositoryGroupId,
    p.connection_string AS ConnectionString,
    p.is_full AS IsFull
FROM 
    Physical_Repository p
INNER JOIN
    Repository_Group r ON r.repository_group_id = p.repository_group_id AND r.is_active = 1 AND r.is_enable = 1
WHERE 
    p.is_active = 1 
AND p.repository_group_id = :repository_group_id"; 
            }
            else
            {
                sql = @"
SELECT 
    p.physical_repository_id AS PhysicalRepositoryId,
    p.repository_group_id AS RepositoryGroupId,
    p.connection_string AS ConnectionString,
    p.is_full AS IsFull
FROM 
    PhysicalRepository p
INNER JOIN
    RepositoryGroup r ON r.repository_group_id = p.repository_group_id AND r.is_active = 1 AND r.is_enable = 1
WHERE 
    p.is_active = 1 
AND p.repository_group_id = @repository_group_id"; 
            }

            return Connection.Query<PhysicalRepositoryResultModel>(sql, new { repository_group_id = repositoryGroupId }).ToList();
        }
    }
}
