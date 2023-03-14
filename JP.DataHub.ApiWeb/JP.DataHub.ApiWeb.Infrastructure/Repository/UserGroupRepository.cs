using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using Dapper.Oracle;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    internal class UserGroupRepository : AbstractRepository, IUserGroupRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<ICacheManager> _lazyCacheManager = new Lazy<ICacheManager>(() => UnityCore.Resolve<ICacheManager>());
        private ICacheManager _cacheManager { get => _lazyCacheManager.Value; }
        private Lazy<ICache> _lazyDynamicApiCache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>("DynamicApi"));

        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_USERGROUP)]
        [CacheArg("open_id")]
        public IList<UserGroupModel> GetList(string open_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    user_group_id AS UserGroupId
    ,user_group_name AS UserGroupName
    ,open_id AS OpenId
FROM
    USER_GROUP
WHERE
    open_id=/*ds open_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    user_group_id AS UserGroupId
    ,user_group_name AS UserGroupName
    ,open_id AS OpenId
FROM
    UserGroup
WHERE
    open_id=@open_id
    AND is_active=1
";
            }
            var param = new { open_id = open_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<UserGroupModel>(twowaySql.Sql, dynParams).ToList();
            if (result.Count() == 0)
            {
                throw new NotFoundException($"Not Found UserGroup.");
            }

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    user_group_id
    ,open_id
FROM
    USER_GROUP_MAP
WHERE
    user_group_id in /*ds user_group_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    user_group_id
    ,open_id
FROM
    UserGroupMap
WHERE
    user_group_id in @user_group_id
    AND is_active=1
";
            }
            var param2 = new { user_group_id = result.Select(x => x.UserGroupId).ToList() };
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param2);
            // dynamicParameterで配列がうまく扱えない為、dynamicParameter化未対応
            var memberList = Connection.Query<DB_UserGroupMap>(twowaySql.Sql, param2).ToList();
            result.ForEach(x => x.Members = memberList.Where(y => y.user_group_id.ToString() == x.UserGroupId).Select(y => y.open_id.ToString()).ToList());
            return result;
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_USERGROUP)]
        [CacheIdFire(DynamicApiDatabase.COLUMN_USERGROUP_USER_GROUP_ID, "model.UserGroupId")]
        [DomainDataSync(DynamicApiDatabase.TABLE_USERGROUP, "model.UserGroupId")]
        public string Register(UserGroupModel model)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO USER_GROUP a
    USING (SELECT /*ds UserGroupId*/'1' as user_group_id FROM DUAL) b
    ON (a.user_group_id=b.user_group_id)
    WHEN MATCHED THEN
        UPDATE SET
            user_group_name=/*ds UserGroupName*/'a' 
            ,open_id = /*ds OpenId*/'1' 
            ,upd_date = SYSTIMESTAMP
            ,upd_username = /*ds openid*/'1' 
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (user_group_id,user_group_name,open_id,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(/*ds UserGroupId*/'1' , /*ds UserGroupName*/'a' , /*ds openid*/'1' , SYSTIMESTAMP, /*ds openid*/'1' , SYSTIMESTAMP, /*ds openid*/'1' , 1)
";
            }
            else
            {
                sql = @"
MERGE INTO UserGroup AS a
    USING (SELECT @UserGroupId as user_group_id) AS b
    ON (a.user_group_id=b.user_group_id)
    WHEN MATCHED THEN
        UPDATE SET
            user_group_name=@UserGroupName
            ,open_id = @OpenId
            ,upd_date = GETDATE()
            ,upd_username = @openid
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (user_group_id,user_group_name,open_id,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(@UserGroupId,@UserGroupName,@openid,GETDATE(),@openid,GETDATE(),@openid,1)
;";
            }
            var param = new { openid = PerRequestDataContainer.OpenId }.ObjectToDictionary().Merge(model);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);

            if (dbSettings.Type == "Oracle")
            {
                sql = @"DELETE USER_GROUP_MAP WHERE user_group_id=/*ds UserGroupId*/'1' ";
            }
            else
            {
                sql = @"DELETE UserGroupMap WHERE user_group_id=@UserGroupId";
            }
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            Connection.Execute(twowaySql.Sql, dynParams);

            model.Members?.ForEach(x => {
                var entity = new DB_UserGroupMap()
                {
                    user_group_map_id = Guid.NewGuid(),
                    user_group_id = model.UserGroupId.To<Guid>(),
                    open_id = x.To<Guid>(),
                    reg_date = DateTime.UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = DateTime.UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = true
                };
                Connection.Insert(entity);
            });
            if (dbSettings.Type == "Oracle")
            {
                sql = "SELECT resource_group_id FROM USER_RESOURCE_SHARE WHERE user_group_id=/*ds user_group_id*/'1' AND is_active = 1";
            }
            else
            {
                sql = "SELECT resource_group_id FROM UserResourceShare WHERE user_group_id=@user_group_id AND is_active = 1";
            }
            var param2 = new { user_group_id = model.UserGroupId };
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param2);
            dynParams = dbSettings.GetParameters().AddDynamicParams(param2);
            var resourceGroupId = Connection.Query<string>(twowaySql.Sql, dynParams).ToList().FirstOrDefault();
            if(!string.IsNullOrEmpty(resourceGroupId))
            {
                _cacheManager.FireKey(CacheManager.CreateKey(DynamicApiRepository.CACHE_KEY_USER_RESOURCE_SHARE, resourceGroupId));
            }
            return model.UserGroupId;
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_USERGROUP)]
        [CacheIdFire(DynamicApiDatabase.COLUMN_USERGROUP_USER_GROUP_ID, "user_group_id")]
        [DomainDataSync(DynamicApiDatabase.TABLE_USERGROUP, "user_group_id")]
        public void Delete(string user_group_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    USER_GROUP
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username=/*ds open_id*/'1' 
WHERE
    user_group_id=/*ds user_group_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    UserGroup
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    user_group_id=@user_group_id
    AND is_active=1
";
            }
            var param = new { user_group_id = user_group_id, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }
    }
}
