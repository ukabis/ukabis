using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    internal class ApiCoreSystemRepository : IApiCoreSystemRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Authority"));
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }

        public string SystemIdToVendorId(string systemId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = @"
SELECT
    vendor_id
FROM
    System
WHERE
    system_id=/*ds system_id*/'00000000-0000-0000-0000-000000000000' 
    AND is_active=1
";
            var param = new { system_id = systemId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
        }
    }
}
