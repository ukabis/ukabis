using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.ApiWeb.Core.Cache;
using JP.DataHub.ApiWeb.Core.Cache.Attributes;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.Infrastructure.Database.Authority;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [CacheKey]
    internal class ClientCertificateRepository : IClientCertificateRepository
    {
        [CacheKey(CacheKeyType.Entity)]
        public static string CACHE_KEY_CLIENTCERTIFICATES = "ClientCertificateRepository-ClientCertificates";

        protected IJPDataHubDbConnection DbConnection => _lazyDbConnection.Value;
        private Lazy<IJPDataHubDbConnection> _lazyDbConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Authority"));

        protected ICache Cache => _lazyCache.Value;
        private Lazy<ICache> _lazyCache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>());

        protected readonly TimeSpan CacheExpireTime = TimeSpan.Parse("01:00:00");


        public ClientCertificate GetClientCertificateByThumbPrint(string thumbprint)
        {
            return GetAllClientCertificates().Where(x => x.ClientCertificateObject.ValueX509.Thumbprint == thumbprint).FirstOrDefault();
        }

        private List<ClientCertificate> GetAllClientCertificates()
        {
            //関連INDEX
            //IXFK_ClientCertificate_Vendor
            //IXFK_ClientCertificate_System
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
 SELECT 
     c.vendor_id
    ,c.system_id
    ,c.client_certificate_string
 FROM
    client_certificate c
 WHERE
    c.is_active=1
";
            }
            else
            {
                sql = @"
 SELECT 
     c.vendor_id
    ,c.system_id
    ,c.client_certificate_string
 FROM
    ClientCertificate as c
 WHERE
    c.is_active=1
";
            }

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, null);
            return Cache.Get<List<ClientCertificate>>(CACHE_KEY_CLIENTCERTIFICATES, CacheExpireTime, () =>
            {
                var dbret = DbConnection.Query<DB_ClientCertificate>(twowaySql.Sql);
                if (dbret == null) return null;
                var retLst = new List<ClientCertificate>();
                dbret.Where(x => !string.IsNullOrEmpty(x.client_certificate_string)).ToList().ForEach(xx =>
                {
                    retLst.Add(new ClientCertificate
                    {
                        VendorId = new VendorId(xx.vendor_id.ToString()),
                        SystemId = new SystemId(xx.system_id.ToString()),
                        ClientCertificateObject = new ClientCertificateVO(xx.client_certificate_string)
                    });
                });
                return retLst;
            });
        }
    }

}
