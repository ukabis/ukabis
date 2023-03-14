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
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Settings;
using System.Data.SqlClient;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    internal class TermsRepository : AbstractRepository, ITermsRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }

        private Lazy<ICacheManager> _lazyCacheManager = new Lazy<ICacheManager>(() => UnityCore.Resolve<ICacheManager>());
        private ICacheManager _cacheManager { get => _lazyCacheManager.Value; }
        private Lazy<ICache> _lazyDynamicApiCache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>("DynamicApi"));

        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_TERMSGROUP)]
        public IList<TermsGroupModel> GroupGetList()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    terms_group_code AS TermsGroupCode
    ,terms_group_name AS TermsGroupName
    ,resource_group_type_code AS ResourceGroupTypeCode
FROM
    TERMS_GROUP
WHERE
    is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    terms_group_code AS TermsGroupCode
    ,terms_group_name AS TermsGroupName
    ,resource_group_type_code AS ResourceGroupTypeCode
FROM
    TermsGroup
WHERE
    is_active=1
";
            }
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, null);
            var result = Connection.Query<TermsGroupModel>(twowaySql.Sql).ToList();
            if (result.Count() == 0)
            {
                throw new NotFoundException($"Not Found TermsGroup.");
            }
            return result;
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_TERMSGROUP)]
        [CacheIdFire(DynamicApiDatabase.COLUMN_TERMSGROUP_TERM_GROUP_CODE, "model.TermsGroupCode")]
        [DomainDataSync(DynamicApiDatabase.TABLE_TERMSGROUP, "model.TermsGroupCode")]
        public string GroupRegister(TermsGroupModel model)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO TERMS_GROUP a
    USING (SELECT /*ds TermsGroupCode*/'a' as terms_group_code FROM DUAL) b
    ON ( a.terms_group_code = b.terms_group_code )
    WHEN MATCHED THEN
        UPDATE SET
            terms_group_name = /*ds TermsGroupName*/'a' 
            ,resource_group_type_code = /*ds ResourceGroupTypeCode*/'a' 
            ,upd_date = SYSTIMESTAMP
            ,upd_username = /*ds openid*/'1' 
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (terms_group_code,terms_group_name,resource_group_type_code,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(/*ds TermsGroupCode*/'a' , /*ds TermsGroupName*/'a' , /*ds ResourceGroupTypeCode*/'a' , SYSTIMESTAMP, /*ds openid*/'1' , SYSTIMESTAMP, /*ds openid*/'1' ,1)
";
            }
            else
            {
                sql = @"
MERGE INTO TermsGroup AS a
    USING (SELECT @TermsGroupCode as terms_group_code) AS b
    ON ( a.terms_group_code = b.terms_group_code )
    WHEN MATCHED THEN
        UPDATE SET
            terms_group_name = @TermsGroupName
            ,resource_group_type_code = @ResourceGroupTypeCode
            ,upd_date = GETDATE()
            ,upd_username = @openid
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (terms_group_code,terms_group_name,resource_group_type_code,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(@TermsGroupCode,@TermsGroupName,@ResourceGroupTypeCode,GETDATE(),@openid,GETDATE(),@openid,1)
;";
            }
            try
            {
                var param = new { openid = PerRequestDataContainer.OpenId }.ObjectToDictionary().Merge(model);
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            }
            catch (SqlException ex)
            {
                throw (ex.Number == 547) ? new ForeignKeyException(ex.Message, ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return model.TermsGroupCode;
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_TERMSGROUP)]
        [CacheIdFire(DynamicApiDatabase.COLUMN_TERMSGROUP_TERM_GROUP_CODE, "terms_group_code")]
        [DomainDataSync(DynamicApiDatabase.TABLE_TERMSGROUP, "terms_group_code")]
        public void GroupDelete(string terms_group_code)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    TERMS_GROUP
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username=/*ds open_id*/'1' 
WHERE
    terms_group_code=/*ds terms_group_code*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    TermsGroup
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    terms_group_code=@terms_group_code
    AND is_active=1
";
            }
            var param = new { terms_group_code = terms_group_code, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_TERMS)]
        [CacheArg(allParam: true)]
        public TermsModel TermsGet(string terms_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    terms_id AS TermsId
    ,version_no AS VersionNo
    ,from_date AS FromDate
    ,terms_text AS TermsText
    ,terms_group_code AS TermsGroupCode
FROM
    TERMS
WHERE
    terms_id=/*ds terms_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    terms_id AS TermsId
    ,version_no AS VersionNo
    ,from_date AS FromDate
    ,terms_text AS TermsText
    ,terms_group_code AS TermsGroupCode
FROM
    Terms
WHERE
    terms_id=@terms_id
    AND is_active=1
";
            }
            var param = new { terms_id = terms_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.QueryPrimaryKey<TermsModel>(twowaySql.Sql, dynParams);
        }

        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_TERMS)]
        public IList<TermsModel> TermsGetList()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    terms_id AS TermsId
    ,version_no AS VersionNo
    ,from_date AS FromDate
    ,terms_text AS TermsText
    ,terms_group_code AS TermsGroupCode
FROM
    TERMS
WHERE
    is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    terms_id AS TermsId
    ,version_no AS VersionNo
    ,from_date AS FromDate
    ,terms_text AS TermsText
    ,terms_group_code AS TermsGroupCode
FROM
    Terms
WHERE
    is_active=1
";
            }
            var result = Connection.Query<TermsModel>(sql).ToList();
            if (result.Count() == 0)
            {
                throw new NotFoundException($"Not Found Terms.");
            }
            return result;
        }

        [Cache]
        [CacheEntity(DynamicApiDatabase.TABLE_TERMS)]
        [CacheArg(allParam: true)]
        public IList<TermsModel> TermsGetListByTermGroupCode(string terms_group_code)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    terms_id AS TermsId
    ,version_no AS VersionNo
    ,from_date AS FromDate
    ,terms_text AS TermsText
    ,terms_group_code AS TermsGroupCode
FROM
    TERMS
WHERE
    terms_group_code=/*ds terms_group_code*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    terms_id AS TermsId
    ,version_no AS VersionNo
    ,from_date AS FromDate
    ,terms_text AS TermsText
    ,terms_group_code AS TermsGroupCode
FROM
    Terms
WHERE
    terms_group_code=@terms_group_code 
    AND is_active=1
";
            }
            var param = new { terms_group_code = terms_group_code };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<TermsModel>(twowaySql.Sql, dynParams).ToList();
            if (result.Count() == 0)
            {
                throw new NotFoundException($"Not Found Terms.");
            }
            return result;
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_TERMS)]
        [CacheIdFire(DynamicApiDatabase.COLUMN_TERMS_TERM_GROUP_CODE, "model.TermsGroupCode")]
        [DomainDataSync(ParameterType.Result, DynamicApiDatabase.TABLE_TERMS, ".")]
        public string TermsRegister(TermsModel model)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO TERMS a
    USING (SELECT /*ds TermsId*/'1' as terms_id FROM DUAL) b
    ON ( a.terms_id = b.terms_id )
    WHEN MATCHED THEN
        UPDATE SET
            version_no = /*ds VersionNo*/1 
            ,from_date = /*ds FromDate*/SYSTIMESTAMP 
            ,terms_text = /*ds TermsText*/'a' 
            ,terms_group_code = /*ds TermsGroupCode*/'1' 
            ,upd_date = SYSTIMESTAMP
            ,upd_username = /*ds openid*/'1' 
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (terms_id,version_no,from_date,terms_text,terms_group_code,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(/*ds TermsId*/'1' , /*ds VersionNo*/1 , /*ds FromDate*/SYSTIMESTAMP , /*ds TermsText*/'a' , /*ds TermsGroupCode*/'1' , SYSTIMESTAMP, /*ds openid*/'1' , SYSTIMESTAMP, /*ds openid*/'1' ,1)
";
            }
            else
            {
                sql = @"
MERGE INTO Terms AS a
    USING (SELECT @TermsId as terms_id) AS b
    ON ( a.terms_id = b.terms_id )
    WHEN MATCHED THEN
        UPDATE SET
            version_no = @VersionNo
            ,from_date = @FromDate
            ,terms_text = @TermsText
            ,terms_group_code = @TermsGroupCode
            ,upd_date = GETDATE()
            ,upd_username = @openid
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (terms_id,version_no,from_date,terms_text,terms_group_code,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(@TermsId,@VersionNo,@FromDate,@TermsText,@TermsGroupCode,GETDATE(),@openid,GETDATE(),@openid,1)
;";
            }
            try
            {
                var param = new { openid = PerRequestDataContainer.OpenId }.Merge(model);
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            }
            catch (SqlException ex)
            {
                throw (ex.Number == 547) ? new ForeignKeyException(ex.Message, ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return model.TermsId;
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_TERMS)]
        [CacheIdFire(DynamicApiDatabase.COLUMN_TERMS_TERMS_ID, "terms_id")]
        [DomainDataSync(DynamicApiDatabase.TABLE_TERMS, "terms_id")]
        public void TermsDelete(string terms_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    TERMS
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username=/*ds open_id*/'1' 
WHERE
    terms_id=/*ds terms_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    Terms
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    terms_id=@terms_id
    AND is_active=1
";
            }
            var param = new { terms_id = terms_id, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        [CacheIdFire(DynamicApiDatabase.COLUMN_TERMS_TERMS_ID, "terms_id")]
        public void Agreement(string open_id, string terms_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    terms_group_code
FROM
    TERMS
WHERE
    terms_id=/*ds terms_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    terms_group_code
FROM
    Terms
WHERE
    terms_id=@terms_id
    AND is_active=1
";
            }
            var param = new { open_id = open_id, terms_id = terms_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var termsGroupCode = Connection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams);
            if (string.IsNullOrEmpty(termsGroupCode))
            {
                throw new NotFoundException("not found terms_id.");
            }

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    USER_TERMS
WHERE
    terms_id=/*ds terms_id*/'1' 
    AND open_id=/*ds open_id*/'1' 
    AND revoke_date IS NULL
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    UserTerms
WHERE
    terms_id=@terms_id
    AND open_id=@open_id
    AND revoke_date IS NULL
";
            }
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var usertermCount = Connection.QuerySingleOrDefault<int>(twowaySql.Sql, dynParams);
            if (usertermCount == 0)
            {
                var now = DateTime.UtcNow;
                DB_UserTerm userterms = new DB_UserTerm()
                {
                    user_terms_id = Guid.NewGuid(),
                    open_id = open_id.To<Guid>(),
                    terms_id = terms_id.To<Guid>(),
                    agreement_date = now,
                    revoke_date = null,
                    reg_date = now,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = now,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = true
                };
                this.Connection.Insert(userterms);
                //キャッシュ削除
                _cacheManager.FireKey(CacheManager.CreateKey(DynamicApiRepository.CACHE_KEY_USERTERMS, termsGroupCode, open_id));
            }
            else
            {
                throw new AlreadyExistsException("すでに同意済です");
            }
        }

        public void Revoke(string open_id, string terms_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    t.terms_group_code AS TermsGroupCode,
    ut.user_terms_id AS UserTermsId,
	rg.resource_group_id AS ResourceGroupId
FROM
    TERMS t
    INNER JOIN USER_TERMS ut ON ut.terms_id = t.terms_id AND ut.is_active=1
	INNER JOIN TERMS_GROUP tg ON tg.terms_group_code = t.terms_group_code AND tg.is_active = 1
	INNER JOIN RESOURCE_GROUP rg ON rg.terms_group_code = tg.terms_group_code AND rg.is_active = 1
WHERE
    t.terms_id=/*ds terms_id*/'1' 
    AND t.is_active=1
    AND ut.open_id=/*ds open_id*/'1' 
    AND ut.revoke_date IS NULL
ORDER BY
    ut.agreement_date DESC
";
            }
            else
            {
                sql = @"
SELECT
    t.terms_group_code AS TermsGroupCode,
    ut.user_terms_id AS UserTermsId,
	rg.resource_group_id AS ResourceGroupId
FROM
    Terms t
    INNER JOIN UserTerms ut ON ut.terms_id = t.terms_id AND ut.is_active=1
	INNER JOIN TermsGroup tg ON tg.terms_group_code = t.terms_group_code AND tg.is_active = 1
	INNER JOIN ResourceGroup rg ON rg.terms_group_code = tg.terms_group_code AND rg.is_active = 1
WHERE
    t.terms_id=@terms_id
    AND t.is_active=1
    AND ut.open_id=@open_id
    AND ut.revoke_date IS NULL
ORDER BY
    ut.agreement_date DESC
";
            }
            var param = new { open_id = open_id, terms_id = terms_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var userTermsRevokeInfo = Connection.QuerySingleOrDefault<UserTermsRevokeModel>(twowaySql.Sql, dynParams);
            if (userTermsRevokeInfo == null || userTermsRevokeInfo.TermsGroupCode == null)
            {
                throw new NotFoundException("not found terms_id.");
            }

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    USER_TERMS
SET
    revoke_date=/*ds now*/SYSTIMESTAMP 
    ,upd_date=/*ds now*/SYSTIMESTAMP 
    ,upd_username=/*ds open_id*/'1' 
WHERE
    open_id=/*ds open_id*/'1' 
    AND terms_id=/*ds terms_id*/'1' 
    AND is_active=1
    AND revoke_date IS NULL
";
            }
            else
            {
                sql = @"
UPDATE
    UserTerms
SET
    revoke_date=@now
    ,upd_date=@now
    ,upd_username=@open_id
WHERE
    open_id=@open_id
    AND terms_id=@terms_id
    AND is_active=1
    AND revoke_date IS NULL
";
            }
            var param2 = new { open_id = open_id, terms_id = terms_id, now = DateTime.UtcNow };
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param2);
            dynParams = dbSettings.GetParameters().AddDynamicParams(param2);
            int count = Connection.Execute(twowaySql.Sql, dynParams);
            if (count <= 0)
            {
                throw new NotFoundException("同意した規約がありません");
            }
            //キャッシュ削除
            _cacheManager.FireKey(CacheManager.CreateKey(DynamicApiRepository.CACHE_KEY_USERTERMS, userTermsRevokeInfo.TermsGroupCode, open_id));
            var notify = UnityCore.Resolve<INotificationService>("Revoke");
            notify.SendAsync(new RevokeNotification(open_id, userTermsRevokeInfo.UserTermsId,userTermsRevokeInfo.ResourceGroupId).ToJsonString());
        }

        internal class RevokeNotification
        {
            public string OpenId { get; set; }
            public string UserTermsId { get; set; }

            public string ResourceGroupId { get; set; }

            public RevokeNotification(string open_id,string user_terms_id,string resource_group_id)
            {
                this.OpenId = open_id;
                this.UserTermsId = user_terms_id;
                this.ResourceGroupId = resource_group_id;
            }
        }
    }
}
