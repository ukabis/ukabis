using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    public class PhysicalRepositoryGroupRepository : IPhysicalRepositoryGroupRepository
    {
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));

        public List<PhysicalRepositoryResultModel> GetPhysicalRepository(string repositoryGroupId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
	p.physical_repository_id AS PhysicalRepositoryId,
	p.repository_group_id AS RepositoryGroupId,
	p.connection_string AS ConnectionString,
	p.is_full AS IsFull
FROM 
	PHYSICAL_REPOSITORY p
INNER JOIN
	REPOSITORY_GROUP r ON r.repository_group_id = p.repository_group_id AND r.is_active = 1 AND r.is_enable = 1
WHERE 
	p.is_active = 1 
AND p.repository_group_id = /*ds repository_group_id*/'1' 
";
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
            var param = new { repository_group_id = repositoryGroupId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<PhysicalRepositoryResultModel>(twowaySql.Sql, dynParams).ToList();
        }
    }
}
