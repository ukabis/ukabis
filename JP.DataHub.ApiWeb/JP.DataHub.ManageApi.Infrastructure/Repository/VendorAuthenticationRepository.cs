using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.ManageApi.Service.Repository.Model;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class VendorAuthenticationRepository : IVendorAuthenticationRepository
    {
        [Dependency("Authority")]
        public IJPDataHubDbConnection Connection { get; set; }

        public IEnumerable<ClientVendorSystem> GetByClientId(string clientId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    c.client_id
    ,c.client_secret
    ,c.accesstoken_expiration_timespan
    ,c.is_active
    ,s.system_id
    ,v.vendor_id
FROM
    CLIENT c
    INNER JOIN SYSTEM s ON c.system_id = s.system_id AND s.is_active = 1 AND s.is_enable = 1
    INNER JOIN VENDOR v ON s.vendor_id = v.vendor_id AND v.is_active = 1 AND v.is_enable = 1
WHERE
    c.client_id = /*ds client_id*/'1' 
    AND c.is_active = 1";
            }
            else
            {
                sql = @"
SELECT
    c.client_id
    ,c.client_secret
    ,c.accesstoken_expiration_timespan
    ,c.is_active
    ,s.system_id
    ,v.vendor_id
FROM
    Client c
    INNER JOIN SYSTEM s ON c.system_id = s.system_id AND s.is_active = 1 AND s.is_enable = 1
    INNER JOIN VENDOR v ON s.vendor_id = v.vendor_id AND v.is_active = 1 AND v.is_enable = 1
WHERE
    c.client_id = @client_id
    AND c.is_active = 1";
            }

            var param = new { client_id = clientId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ClientVendorSystem>(twowaySql.Sql, dynParams);
        }
    }
}
