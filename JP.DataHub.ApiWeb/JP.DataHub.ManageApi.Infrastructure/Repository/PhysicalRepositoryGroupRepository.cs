using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class PhysicalRepositoryGroupRepository : IPhysicalRepositoryGroupRepository
    {
        [Dependency("DynamicApi")]
        public IJPDataHubDbConnection Connection { get; set; }

        public List<PhysicalRepositoryResultModel> GetPhysicalRepository(Guid repositoryGroupId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
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
AND p.repository_group_id = /*ds repository_group_id*/'00000000-0000-0000-0000-000000000000' 
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
AND p.repository_group_id = @repository_group_id
";
            }
            var param = new
            {
                repository_group_id = repositoryGroupId
            };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<PhysicalRepositoryResultModel>(twowaySql.Sql, dynParams).ToList();
        }
    }
}
