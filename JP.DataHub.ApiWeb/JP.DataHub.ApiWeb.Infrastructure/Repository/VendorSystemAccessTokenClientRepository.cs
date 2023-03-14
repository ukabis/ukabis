using Unity;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Infrastructure.Models.Database;
using JP.DataHub.Infrastructure.Database.Authority;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    [CacheKey]
    internal class VendorSystemAccessTokenClientRepository : IVendorSystemAccessTokenClientRepository
    {
        [Dependency("Authority")]
        public IJPDataHubDbConnection Connection { get; set; }

        public VendorSystemAccessTokenClientRepository()
        {
            this.AutoInjection();
        }

        /// <summary>
        /// 指定されたClientIdのClient情報を取得します。
        /// </summary>
        /// <param name="clientId">ClientId</param>
        /// <returns>Client情報</returns>
        public IEnumerable<ClientVendorSystem> GetByClientId(ClientId clientId)
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
    c.client_id = /*ds client_id*/'00000000-0000-0000-0000-000000000000' 
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
    c.client_id = /*ds client_id*/'00000000-0000-0000-0000-000000000000' 
    AND c.is_active = 1";
            }
            var param = new { client_id = clientId?.Value };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ClientVendorSystem>(twowaySql.Sql, dynParams);
        }
    }
}
