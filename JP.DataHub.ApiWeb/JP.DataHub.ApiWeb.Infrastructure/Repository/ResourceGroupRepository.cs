using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Service.Model;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    internal class ResourceGroupRepository : AbstractRepository, IResourceGroupRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }

        private class TmpResourceGroup
        {
            public string ResourceGroupId { get; set; }
            public string ControllerId { get; set; }
            public string ControllerUrl { get; set; }
            public bool IsPerson { get; set; }
        }

        public IList<ResourceGroupModel> GetList()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    rg.resource_group_id AS ResourceGroupId
    ,rg.resource_group_name AS ResourceGroupName
    ,rg.is_require_consent AS IsRequireConsent
    ,rg.terms_group_code AS TermsGroupCode
FROM
    RESOURCE_GROUP rg
WHERE
    rg.is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    rg.resource_group_id AS ResourceGroupId
    ,rg.resource_group_name AS ResourceGroupName
    ,rg.is_require_consent AS IsRequireConsent
    ,rg.terms_group_code AS TermsGroupCode
FROM
    ResourceGroup AS rg
WHERE
    rg.is_active=1
";
            }
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, null);
            var result = Connection.Query<ResourceGroupModel>(twowaySql.Sql).ToList();
            if (result.Count() == 0)
            {
                throw new NotFoundException($"Not Found ResourceGroup.");
            }
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    crg.resource_group_id AS ResourceGroupId
    ,c.controller_id AS ControllerId
    ,c.url AS ControllerUrl
    ,c.is_person AS IsPerson
FROM
    CONTROLLER_RESOURCE_GROUP crg
	INNER JOIN CONTROLLER c ON crg.controller_id=c.controller_id AND c.is_active=1
WHERE
    crg.is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    crg.resource_group_id AS ResourceGroupId
    ,c.controller_id AS ControllerId
    ,c.url AS ControllerUrl
    ,c.is_person AS IsPerson
FROM
    ControllerResourceGroup AS crg
	INNER JOIN Controller AS c ON crg.controller_id=c.controller_id AND c.is_active=1
WHERE
    crg.is_active=1
";
            }
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, null);
            var tmp = Connection.Query<TmpResourceGroup>(twowaySql.Sql).ToList();
            result.ForEach(x => x.Resources = tmp.Where(y => y.ResourceGroupId == x.ResourceGroupId).Select(z => new ResourceGroupInResourceModel { ControllerId = z.ControllerId, ControllerUrl = z.ControllerUrl ,IsPerson = z.IsPerson}).ToList());
            return result;
        }

        [DomainDataSync(DynamicApiDatabase.TABLE_RESOURCEGROUP, "model.ResourceGroupId")]
        public string Register(ResourceGroupModel model)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO RESOURCE_GROUP a
    USING (SELECT /*ds ResourceGroupId*/'1' as resource_group_id FROM DUAL) b
    ON ( a.resource_group_id = b.resource_group_id )
    WHEN MATCHED THEN
        UPDATE SET
            resource_group_name = /*ds ResourceGroupName*/'a' 
            ,terms_group_code = /*ds TermsGroupCode*/'a' 
            ,is_require_consent = /*ds IsRequireConsent*/1 
            ,upd_date = SYSTIMESTAMP
            ,upd_username = /*ds openid*/'1' 
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (resource_group_id,resource_group_name,terms_group_code,is_require_consent,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(/*ds ResourceGroupId*/'1' , /*ds ResourceGroupName*/'a' , /*ds TermsGroupCode*/'a' , /*ds IsRequireConsent*/1 , SYSTIMESTAMP, /*ds openid*/'1' , SYSTIMESTAMP, /*ds openid*/'1' ,1)
";
            }
            else
            {
                sql = @"
MERGE INTO ResourceGroup AS a
    USING (SELECT @ResourceGroupId as resource_group_id) AS b
    ON ( a.resource_group_id = b.resource_group_id )
    WHEN MATCHED THEN
        UPDATE SET
            resource_group_name = @ResourceGroupName
            ,terms_group_code = @TermsGroupCode
            ,is_require_consent = @IsRequireConsent
            ,upd_date = GETDATE()
            ,upd_username = @openid
            ,is_active = 1
    WHEN NOT MATCHED THEN
        INSERT (resource_group_id,resource_group_name,terms_group_code,is_require_consent,reg_date,reg_username,upd_date,upd_username,is_active)
            VALUES(@ResourceGroupId,@ResourceGroupName,@TermsGroupCode,@IsRequireConsent,GETDATE(),@openid,GETDATE(),@openid,1)
;";
            }
            var param = new { openid = PerRequestDataContainer.OpenId, resource_group_id = model.ResourceGroupId };
            var param2 = param.ObjectToDictionary().Merge(model);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param2);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param2);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);

            if (dbSettings.Type == "Oracle")
            {
                sql = @"DELETE CONTROLLER_RESOURCE_GROUP WHERE resource_group_id=/*ds resource_group_id*/'1' ";
            }
            else
            {
                sql = @"DELETE ControllerResourceGroup WHERE resource_group_id=@resource_group_id";
            }
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.Execute(twowaySql.Sql, dynParams);

            model.Resources?.ForEach(x => {
                var entity = new DB_ControllerResourceGroup()
                {
                    controller_resource_group_id = Guid.NewGuid(),
                    controller_id = x.ControllerId.To<Guid>(),
                    resource_group_id = model.ResourceGroupId.To<Guid>(),
                    reg_date = DateTime.UtcNow,
                    reg_username = PerRequestDataContainer.OpenId,
                    upd_date = DateTime.UtcNow,
                    upd_username = PerRequestDataContainer.OpenId,
                    is_active = true
                };
                Connection.Insert(entity);
            });

            return model.ResourceGroupId;
        }

        [DomainDataSync(DynamicApiDatabase.TABLE_RESOURCEGROUP, "resource_group_id")]
        public void Delete(string resource_group_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    RESOURCE_GROUP
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username=/*ds open_id*/'1' 
WHERE
    resource_group_id=/*ds resource_group_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    ResourceGroup
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    resource_group_id=@resource_group_id
    AND is_active=1
";
            }
            var param = new { resource_group_id = resource_group_id, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            if (result <= 0)
            {
                throw new NotFoundException($"Not Found resource_group_id={resource_group_id}");
            }
        }
    }
}
