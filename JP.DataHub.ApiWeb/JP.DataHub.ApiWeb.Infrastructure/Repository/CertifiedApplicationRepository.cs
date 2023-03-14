using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Repository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.Infrastructure.Database.Document;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using System.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    internal class CertifiedApplicationRepository : AbstractRepository, ICertifiedApplicationRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }

        private Lazy<IJPDataHubDbConnection> _lazyAuthConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Authority"));
        private IJPDataHubDbConnection authConnection { get => _lazyAuthConnection.Value; }

        //[Cache]
        //[CacheEntity(DynamicApiDatabase.TABLE_CERTIFIEDAPPLICATION)]
        public IList<CertifiedApplicationModel> GetList()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    certified_application_id AS CertifiedApplicationId
    ,application_name AS ApplicationName
    ,vendor_id AS VendorId
    ,system_id AS SystemId
FROM
    CERTIFIED_APPLICATION
WHERE
    is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    certified_application_id AS CertifiedApplicationId
    ,application_name AS ApplicationName
    ,vendor_id AS VendorId
    ,system_id AS SystemId
FROM
    CertifiedApplication
WHERE
    is_active=1
";
            }
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, null);
            var result = Connection.Query<CertifiedApplicationModel>(twowaySql.Sql).ToList();
            if (result.Count() == 0)
            {
                throw new NotFoundException($"Not Found CertifiedApplication.");
            }
            return result;
        }

        //[Cache]
        //[CacheEntity(DynamicApiDatabase.TABLE_CERTIFIEDAPPLICATION)]
        //[CacheArg(allParam: true)]
        public CertifiedApplicationModel Get(string certified_application_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    certified_application_id AS CertifiedApplicationId
    ,application_name AS ApplicationName
    ,vendor_id AS VendorId
    ,system_id AS SystemId
FROM
    CERTIFIED_APPLICATION
WHERE
    certified_application_id=/*ds certified_application_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    certified_application_id AS CertifiedApplicationId
    ,application_name AS ApplicationName
    ,vendor_id AS VendorId
    ,system_id AS SystemId
FROM
    CertifiedApplication
WHERE
    certified_application_id=@certified_application_id
    AND is_active=1
";
            }
            var param = new { certified_application_id = certified_application_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingle<CertifiedApplicationModel>(twowaySql.Sql, dynParams);
        }

        //[CacheEntityFire(DynamicApiDatabase.TABLE_CERTIFIEDAPPLICATION)]
        //[CacheIdFire(DynamicApiDatabase.COLUMN_CERTIFIEDAPPLICATION_CERTIFIED_APPLICATION_ID, "model.CertifiedApplicationId")]
        [DomainDataSync(DynamicApiDatabase.TABLE_CERTIFIEDAPPLICATION, "model.CertifiedApplicationId")]
        public string Register(CertifiedApplicationModel model)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    VENDOR v
    INNER JOIN SYSTEM s ON v.vendor_id=s.vendor_id AND s.system_id=/*ds system_id*/'1' AND s.is_active=1
WHERE
    v.vendor_id=/*ds vendor_id*/'1' 
    AND v.is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    Vendor v
    INNER JOIN [System] s ON v.vendor_id=s.vendor_id AND s.system_id=@system_id AND s.is_active=1
WHERE
    v.vendor_id=@vendor_id
    AND v.is_active=1
";
            }
            var param = new { vendor_id = model.VendorId, system_id = model.SystemId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var count = authConnection.QuerySingleOrDefault<int>(twowaySql.Sql, dynParams);
            if (count == 0)
            {
                throw new NotFoundException("not found vendor_id and system_id.");
            }

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO CERTIFIED_APPLICATION a
    USING (SELECT /*ds CertifiedApplicationId*/'1' as certified_application_id FROM DUAL) b
    ON ( a.certified_application_id = b.certified_application_id )
    WHEN MATCHED THEN
        UPDATE SET
            application_name=/*ds ApplicationName*/'a' 
            ,vendor_id=/*ds VendorId*/'1' 
            ,system_id=/*ds SystemId*/'1' 
            ,upd_date = SYSTIMESTAMP
            ,upd_username = /*ds openid*/'1' 
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (certified_application_id,application_name,vendor_id,system_id,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(/*ds CertifiedApplicationId*/'1' , /*ds ApplicationName*/'a' , /*ds VendorId*/'1' , /*ds SystemId*/'1' , SYSTIMESTAMP, /*ds openid*/'1' , SYSTIMESTAMP , /*ds openid*/'1' , 1)
";
            }
            else
            {
                sql = @"
MERGE INTO CertifiedApplication AS a
    USING (SELECT @CertifiedApplicationId as certified_application_id) AS b
    ON ( a.certified_application_id = b.certified_application_id )
    WHEN MATCHED THEN
        UPDATE SET
            application_name=@ApplicationName
            ,vendor_id=@VendorId
            ,system_id=@SystemId
            ,upd_date = GETDATE()
            ,upd_username = @openid
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (certified_application_id,application_name,vendor_id,system_id,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(@CertifiedApplicationId,@ApplicationName,@VendorId,@SystemId,GETDATE(),@openid,GETDATE(),@openid,1)
;";
            }
            var param2 = new { openid = PerRequestDataContainer.OpenId }.ObjectToDictionary().Merge(model);
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param2);
            dynParams = dbSettings.GetParameters().AddDynamicParams(param2);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            return model.CertifiedApplicationId;
        }

        //[CacheEntityFire(DynamicApiDatabase.TABLE_CERTIFIEDAPPLICATION)]
        //[CacheIdFire(DynamicApiDatabase.COLUMN_CERTIFIEDAPPLICATION_CERTIFIED_APPLICATION_ID, "certified_application_id")]
        [DomainDataSync(DynamicApiDatabase.TABLE_CERTIFIEDAPPLICATION, "certified_application_id")]
        public void Delete(string certified_application_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    CERTIFIED_APPLICATION
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username=/*ds open_id*/'1' 
WHERE
    certified_application_id=/*ds certified_application_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    CertifiedApplication
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    certified_application_id=@certified_application_id
    AND is_active=1
";
            }
            var param = new { certified_application_id = certified_application_id, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            if (result <= 0)
            {
                throw new NotFoundException($"Not Found certified_application_id={certified_application_id}");
            }
        }
    }
}
