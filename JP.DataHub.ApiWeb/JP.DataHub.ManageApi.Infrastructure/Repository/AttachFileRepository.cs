using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Infrastructure.Repository.Model;
using JP.DataHub.Infrastructure.Database.Authority;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class AttachFileRepository : AbstractRepository, IAttachFileRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("AttachFile"));
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }

        [Cache]
        [CacheEntity(AttachFileDatabase.TABLE_VENDORATTACHFILESTORAGE)]
        [CacheArg("vendorId")]
        public string GetAttachFileStorageIdByVendorId(string vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    attachfile_storage_id
FROM
    VENDOR_ATTACHFILESTORAGE
WHERE
    vendor_id = /*ds vendor_id*/'' 
and is_active = 1
and is_current = 1
";
            }
            else
            {
                sql = @"
SELECT
    attachfile_storage_id
FROM
    VendorAttachfilestorage
WHERE
    vendor_id = @vendor_id
and is_active = 1
and is_current = 1
";
            }
            var param = new { vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = _connection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
            if (string.IsNullOrEmpty(result))
            {
                throw new NotFoundException($"Not Found vendor_id={vendorId}");
            }
            return result;
        }

        [CacheIdFire("VendorId", "vendor_id")]
        [CacheEntityFire(AttachFileDatabase.TABLE_VENDORATTACHFILESTORAGE)]
        public void DeleteByVendorId(string vendor_id, string excluideVendorAttachFileStorageId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    VENDOR_ATTACHFILESTORAGE
SET
    is_current = 0
    ,is_active = 0
    ,upd_date=SYSDATE
    ,upd_username=/*ds open_id*/'' 
WHERE
    vendor_id=/*ds vendor_id*/'' AND
/*ds if exclude != null*/
    vendor_attachfilestorage_id=/*ds exclude*/'' AND
/*ds end if*/
    is_active=1

";
            }
            else
            {
                sql = @"
UPDATE
    VendorAttachfilestorage
SET
    is_current = 0
    ,is_active = 0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    vendor_id=@vendor_id
/*ds if exclude != null*/
    AND vendor_attachfilestorage_id=@exclude
/*ds end if*/
    AND is_active=1

";
            }
            var param = new { vendor_id = vendor_id, open_id = PerRequestDataContainer.OpenId, exclude = excluideVendorAttachFileStorageId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            if (_connection.Execute(twowaySql.Sql, dynParams) <= 0)
            {
                throw new NotFoundException($"Not Found vendor_id={vendor_id}");
            }
        }

        public IList<AttachFileStorageModel> GetAttachFileStorageList()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    attachfile_storage_id AS AttachFileStorageId
    ,attachfile_storage_name AS AttachFileStorageName
FROM
    ATTACH_FILE_STORAGE
WHERE
    is_active = 1
    AND is_full = 0
";
            }
            else
            {
                sql = @"
SELECT
    attachfile_storage_id AS AttachFileStorageId
    ,attachfile_storage_name AS AttachFileStorageName
FROM
    AttachFileStorage
WHERE
    is_active = 1
    AND is_full = 0
";
            }

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, null);
            return _connection.Query<AttachFileStorageModel>(twowaySql.Sql).ToList();
        }

        [CacheIdFire("VendorId", "vendor_id")]
        [CacheEntityFire(AttachFileDatabase.TABLE_VENDORATTACHFILESTORAGE)]
        public void RegisterVendorAttachFileStorage(string vendor_id, string attachfile_storage_id, bool is_current)
        {
            // 処理内容
            // 0. is_currentの値によってことなる
            // 1. is_current = true
            // 1-1. vendor_idに合致するものすべてをis_current = 0にする
            // 1-2. MERGE文でis_current=1のものをセットする
            // 2. is_current = false
            // 2-1. MERGE文でis_current=0のものをセットする
            // ※is_currentは最大１つでないといけないため
            if (is_current == true)
            {
            }
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO VENDOR_ATTACHFILESTORAGE a
    USING (SELECT /*ds vendor_id*/'' AS vendor_id, /*ds attachfile_storage_id*/'' AS attachfile_storage_id FROM DUAL) b
    ON (a.vendor_id=b.vendor_id)
    WHEN MATCHED THEN
        UPDATE SET
            is_active=1, upd_username=/*ds open_id*/'' , upd_date=SYSDATE,is_current=/*ds is_current*/1 , attachfile_storage_id=/*ds attachfile_storage_id*/'' 
    WHEN NOT MATCHED THEN
        INSERT (vendor_attachfilestorage_id,vendor_id,attachfile_storage_id,is_current,reg_date,reg_username,upd_date,upd_username,is_active)
        VALUES (NEWID(),/*ds vendor_id*/'' ,/*ds attachfile_storage_id*/'' ,/*ds is_current*/1 ,SYSDATE,/*ds open_id*/'' ,SYSDATE,/*ds open_id*/'' ,1)
";
            }
            else
            {
                sql = @"
MERGE INTO VendorAttachfilestorage AS a
    USING (SELECT @vendor_id AS vendor_id, @attachfile_storage_id AS attachfile_storage_id) AS b
    ON (a.vendor_id=b.vendor_id)
    WHEN MATCHED THEN
        UPDATE SET
            is_active=1, upd_username=@open_id, upd_date=GETDATE(),is_current=@current, attachfile_storage_id=@attachfile_storage_id
    WHEN NOT MATCHED THEN
        INSERT (vendor_attachfilestorage_id,vendor_id,attachfile_storage_id,is_current,reg_date,reg_username,upd_date,upd_username,is_active)
        VALUES (NEWID(),@vendor_id,@attachfile_storage_id,@current,GETDATE(),@open_id,GETDATE(),@open_id,1);
";
            }
            var param = new
            {
                attachfile_storage_id = attachfile_storage_id,
                vendor_id = vendor_id,
                open_id = PerRequestDataContainer.OpenId,
                current = is_current,
                is_current = is_current // currentというバインド変数はOracleでは使用できないので代わりにis_currentを使用する
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }
    }
}
