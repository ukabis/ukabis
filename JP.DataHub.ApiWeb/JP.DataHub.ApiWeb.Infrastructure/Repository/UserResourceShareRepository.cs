using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.Api.Core.Exceptions;
using System.Data.SqlClient;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    internal class UserResourceShareRepository : AbstractRepository, IUserResourceShareRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<ICacheManager> _lazyCacheManager = new Lazy<ICacheManager>(() => UnityCore.Resolve<ICacheManager>());
        private ICacheManager _cacheManager { get => _lazyCacheManager.Value; }
        private Lazy<ICache> _lazyDynamicApiCache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>("DynamicApi"));

        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_USERRESOURCESHARE)]
        [CacheArg("open_id")]
        public IList<UserResourceShareModel> GetList(string open_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    user_resource_group_id AS UserResourceGroupId
    ,open_id AS OpenId
    ,resource_group_id AS ResourceGroupId
    ,user_shared_type_code AS UserShareTypeCode
    ,user_group_id AS UserGroupId
FROM
    USER_RESOURCE_SHARE
WHERE
    open_id=/*ds open_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    user_resource_group_id AS UserResourceGroupId
    ,open_id AS OpenId
    ,resource_group_id AS ResourceGroupId
    ,user_shared_type_code AS UserShareTypeCode
    ,user_group_id AS UserGroupId
FROM
    UserResourceShare
WHERE
    open_id=@open_id
    AND is_active=1
";
            }
            var param = new { open_id = open_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<UserResourceShareModel>(twowaySql.Sql, dynParams).ToList();
            if (result.Count() == 0)
            {
                throw new NotFoundException($"Not Found UserResourceShare.");
            }
            return result;
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_USERRESOURCESHARE)]
        [CacheIdFire(DynamicApiDatabase.COLUMN_USERRESOURCESHARE_USER_RESOURCE_GROUP_ID, "model.UserResourceGroupId")]
        [DomainDataSync(DynamicApiDatabase.TABLE_USERRESOURCESHARE, "model.UserResourceGroupId")]
        public string Register(UserResourceShareModel model)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO USER_RESOURCE_SHARE a
    USING (SELECT /*ds UserResourceGroupId*/'1' as user_resource_group_id FROM DUAL) b
    ON (a.user_resource_group_id=b.user_resource_group_id)
    WHEN MATCHED THEN
        UPDATE SET
            open_id=/*ds OpenId*/'1' 
            ,user_group_id=/*ds UserGroupId*/'1' 
            ,resource_group_id=/*ds ResourceGroupId*/'1' 
            ,user_shared_type_code=/*ds UserShareTypeCode*/'a' 
            ,upd_date = SYSTIMESTAMP
            ,upd_username = /*ds openid*/'1' 
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (user_resource_group_id,open_id,user_group_id,resource_group_id,user_shared_type_code,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(/*ds UserResourceGroupId*/'1' , /*ds OpenId*/'1' , /*ds UserGroupId*/'1' , /*ds ResourceGroupId*/'1' , /*ds UserShareTypeCode*/'1' , SYSTIMESTAMP, /*ds openid*/'1' , SYSTIMESTAMP, /*ds openid*/'1' , 1)
";
            }
            else
            {
                sql = @"
MERGE INTO UserResourceShare AS a
    USING (SELECT @UserResourceGroupId as user_resource_group_id) AS b
    ON (a.user_resource_group_id=b.user_resource_group_id)
    WHEN MATCHED THEN
        UPDATE SET
            user_resource_group_id=@UserResourceGroupId
            ,open_id=@OpenId
            ,user_group_id=@UserGroupId
            ,resource_group_id=@ResourceGroupId
            ,user_shared_type_code=@UserShareTypeCode
            ,upd_date = GETDATE()
            ,upd_username = @openid
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (user_resource_group_id,open_id,user_group_id,resource_group_id,user_shared_type_code,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(@UserResourceGroupId,@OpenId,@UserGroupId,@ResourceGroupId,@UserShareTypeCode,GETDATE(),@openid,GETDATE(),@openid,1)
;";
            }
            var param = new { openid = PerRequestDataContainer.OpenId }.ObjectToDictionary().Merge(model);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            try
            {
                Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new ForeignKeyException($"user_share_type_code({model.UserShareTypeCode})が不正です。");
            }
           _cacheManager.FireKey(CacheManager.CreateKey(DynamicApiRepository.CACHE_KEY_USER_RESOURCE_SHARE, model.ResourceGroupId));

            return model.UserResourceGroupId;
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_USERRESOURCESHARE)]
        [CacheIdFire(DynamicApiDatabase.COLUMN_USERGROUP_USER_GROUP_ID, "user_resource_group_id")]
        [DomainDataSync(DynamicApiDatabase.TABLE_USERRESOURCESHARE, "user_resource_group_id")]
        public void Delete(string user_resource_group_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    USER_RESOURCE_SHARE
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username=/*ds open_id*/'1' 
WHERE
    user_resource_group_id=/*ds user_resource_group_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    UserResourceShare
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    user_resource_group_id=@user_resource_group_id
    AND is_active=1
";
            }
            var param = new { user_resource_group_id = user_resource_group_id, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }
    }
}
