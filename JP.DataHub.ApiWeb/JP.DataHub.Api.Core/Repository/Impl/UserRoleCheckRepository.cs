using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    internal class UserRoleCheckRepository : IUserRoleCheckRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Authority"));
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }

        [Cache]
        public IList<AccessRight> GetAllAccessRights(string openId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
	af.admin_name AS FunctionName
	,afr.is_read AS IsRead
	,afr.is_write AS IsWrite
FROM
	staff s
	INNER JOIN staff_role sr ON s.staff_id=sr.staff_id  AND sr.is_active=1
	INNER JOIN role r ON sr.role_id=r.role_id AND r.is_active=1
	INNER JOIN admin_func_role afr ON r.role_id=afr.role_id AND afr.is_active=1
	INNER JOIN admin_func af ON afr.admin_func_id=af.admin_func_id AND af.is_active=1
WHERE
	s.account=/*ds openId*/'00000000-0000-0000-0000-000000000000' 
	AND s.is_active=1
";
            }
            else
            {
                sql = @"
SELECT
	af.admin_name AS FunctionName
	,afr.is_read AS IsRead
	,afr.is_write AS IsWrite
FROM
	Staff s
	INNER JOIN StaffRole sr ON s.staff_id=sr.staff_id  AND sr.is_active=1
	INNER JOIN Role r ON sr.role_id=r.role_id AND r.is_active=1
	INNER JOIN AdminFuncRole afr ON r.role_id=afr.role_id AND afr.is_active=1
	INNER JOIN AdminFunc af ON afr.admin_func_id=af.admin_func_id AND af.is_active=1
WHERE
	s.account=@openId
	AND s.is_active=1
";
            }
            var param = new { openId = openId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<AccessRight>(twowaySql.Sql, dynParams).ToList();
        }
    }
}
