using AutoMapper;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class ResourceSharingRepository : AbstractRepository, IResourceSharingRepository
    {
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));

        public ResourceSharingRuleModel GetResourceSharingRule(string resourceSharingRuleId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
Select 
       resource_sharing_rule_id as ResourceSharingRuleId
      ,controller_id as ApiId
      ,sharing_from_vendor_id as SharingFromVendorId
      ,sharing_from_system_id as SharingFromSystemId
      ,resource_sharing_rule_name as ResourceSharingRuleName
      ,sharing_to_vendor_id as SharingToVendorId
      ,sharing_to_system_id as SharingToSystemId
      ,query as Query
      ,roslyn_script as RoslynScript
      ,is_enable as IsEnable
from RESOURCE_SHARING_RULE 
Where 
resource_sharing_rule_id = /*ds resource_sharing_rule_id*/'1' And
is_active = 1 
";
            }
            else
            {
                sql = @"
Select 
       resource_sharing_rule_id as ResourceSharingRuleId
      ,controller_id as ApiId
      ,sharing_from_vendor_id as SharingFromVendorId
      ,sharing_from_system_id as SharingFromSystemId
      ,resource_sharing_rule_name as ResourceSharingRuleName
      ,sharing_to_vendor_id as SharingToVendorId
      ,sharing_to_system_id as SharingToSystemId
      ,query as Query
      ,roslyn_script as RoslynScript
      ,is_enable as IsEnable
from ResourceSharingRule 
Where 
resource_sharing_rule_id = @resource_sharing_rule_id And
is_active = 1 
";
            }
            var param = new { resource_sharing_rule_id = resourceSharingRuleId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<ResourceSharingRuleModel>(twowaySql.Sql, dynParams).FirstOrDefault();
        }

        [Cache]
        [CacheArg(allParam: true)]
        [CacheEntity(DynamicApiDatabase.TABLE_RESOURCESHARINGRULE)]
        public IList<ResourceSharingRuleModel> GetResourceSharingRuleList(string apiId, string resourceSharingFromVendorId, string resourceSharingFromSystemId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
Select 
       resource_sharing_rule_id as ResourceSharingRuleId
      ,controller_id as ApiId
      ,sharing_from_vendor_id as SharingFromVendorId
      ,sharing_from_system_id as SharingFromSystemId
      ,resource_sharing_rule_name as ResourceSharingRuleName
      ,sharing_to_vendor_id as SharingToVendorId
      ,sharing_to_system_id as SharingToSystemId
      ,query as Query
      ,roslyn_script as RoslynScript
      ,is_enable as IsEnable 
from RESOURCE_SHARING_RULE 
Where 
controller_id = /*ds controller_id*/'1' And
sharing_from_vendor_id = /*ds sharing_from_vendor_id*/'1' And
sharing_from_system_id = /*ds sharing_from_system_id*/'1' And
is_active = 1 
order by reg_date
";
            }
            else
            {
                sql = @"
Select 
       resource_sharing_rule_id as ResourceSharingRuleId
      ,controller_id as ApiId
      ,sharing_from_vendor_id as SharingFromVendorId
      ,sharing_from_system_id as SharingFromSystemId
      ,resource_sharing_rule_name as ResourceSharingRuleName
      ,sharing_to_vendor_id as SharingToVendorId
      ,sharing_to_system_id as SharingToSystemId
      ,query as Query
      ,roslyn_script as RoslynScript
      ,is_enable as IsEnable 
from ResourceSharingRule 
Where 
controller_id = @controller_id And
sharing_from_vendor_id = @sharing_from_vendor_id And
sharing_from_system_id = @sharing_from_system_id And
is_active = 1 
order by reg_date
";
            }
            var param = new
            {
                controller_id = apiId,
                sharing_from_vendor_id = resourceSharingFromVendorId,
                sharing_from_system_id = resourceSharingFromSystemId
            };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<ResourceSharingRuleModel>
            (
                twowaySql.Sql,
                dynParams
            ).ToList();
        }

        [CacheIdFire("apiId", "resourceSharing.ApiId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_RESOURCESHARINGRULE)]
        public ResourceSharingRuleModel MargeResourceSharingRule(ResourceSharingRuleModel resourceSharing)
        {
            var updUserId = Convert.ToString(PerRequestDataContainer.OpenId);
            if (string.IsNullOrEmpty(resourceSharing.ResourceSharingRuleId))
            {
                resourceSharing.ResourceSharingRuleId = Guid.NewGuid().ToString();
            }

            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO RESOURCE_SHARING_RULE target
    USING
    (
        SELECT
            /*ds resource_sharing_rule_id*/'1' AS resource_sharing_rule_id
        FROM DUAL
    ) source
    ON
        (target.resource_sharing_rule_id = source.resource_sharing_rule_id)
    WHEN MATCHED THEN
        UPDATE
        SET
            controller_id = /*ds controller_id*/'1' 
            , resource_sharing_rule_name = /*ds resource_sharing_rule_name*/'1' 
            , sharing_from_vendor_id = /*ds sharing_from_vendor_id*/'1' 
            , sharing_from_system_id = /*ds sharing_from_system_id*/'1' 
            , sharing_to_vendor_id = /*ds sharing_to_vendor_id*/'1' 
            , sharing_to_system_id = /*ds sharing_to_system_id*/'1' 
            , query = /*ds query*/'1' 
            , roslyn_script = /*ds roslyn_script*/'1' 
            , is_enable = /*ds is_enable*/1 
            , is_active = /*ds is_active*/1 
            , upd_date = /*ds upd_date*/SYSTIMESTAMP 
            , upd_username = /*ds upd_username*/'1' 
    WHEN NOT MATCHED THEN
        INSERT
        (
            resource_sharing_rule_id,
            controller_id,
            resource_sharing_rule_name,
            sharing_from_vendor_id,
            sharing_from_system_id,
            sharing_to_vendor_id,
            sharing_to_system_id,
            query,
            roslyn_script,
            is_enable,
            reg_date,
            reg_username,
            upd_date,
            upd_username,
            is_active
        )
        VALUES
        (
            /*ds resource_sharing_rule_id*/'1' ,
            /*ds controller_id*/'1' ,
            /*ds resource_sharing_rule_name*/'1' ,
            /*ds sharing_from_vendor_id*/'1' ,
            /*ds sharing_from_system_id*/'1' ,
            /*ds sharing_to_vendor_id*/'1' ,
            /*ds sharing_to_system_id*/'1' ,
            /*ds query*/'1' ,
            /*ds roslyn_script*/'1' ,
            /*ds is_enable*/1 ,
            /*ds reg_date*/SYSTIMESTAMP ,
            /*ds reg_username*/'1' ,
            /*ds upd_date*/SYSTIMESTAMP ,
            /*ds upd_username*/'1' ,
            /*ds is_active*/1 
        )";
            }
            else
            {
                sql = @"
MERGE INTO ResourceSharingRule AS target
    USING
    (
        SELECT
            @resource_sharing_rule_id AS resource_sharing_rule_id
    ) AS source
    ON
        target.resource_sharing_rule_id = source.resource_sharing_rule_id
    WHEN MATCHED THEN
        UPDATE
        SET
            controller_id = @controller_id
            , resource_sharing_rule_name = @resource_sharing_rule_name
            , sharing_from_vendor_id = @sharing_from_vendor_id
            , sharing_from_system_id = @sharing_from_system_id
            , sharing_to_vendor_id = @sharing_to_vendor_id
            , sharing_to_system_id = @sharing_to_system_id
            , query = @query
            , roslyn_script = @roslyn_script
            , is_enable = @is_enable
            , is_active = @is_active
            , upd_date = @upd_date
            , upd_username = @upd_username
    WHEN NOT MATCHED THEN
        INSERT
        (
            resource_sharing_rule_id,
            controller_id,
            resource_sharing_rule_name,
            sharing_from_vendor_id,
            sharing_from_system_id,
            sharing_to_vendor_id,
            sharing_to_system_id,
            query,
            roslyn_script,
            is_enable,
            reg_date,
            reg_username,
            upd_date,
            upd_username,
            is_active
        )
        VALUES
        (
            @resource_sharing_rule_id,
            @controller_id,
            @resource_sharing_rule_name,
            @sharing_from_vendor_id,
            @sharing_from_system_id,
            @sharing_to_vendor_id,
            @sharing_to_system_id,
            @query,
            @roslyn_script,
            @is_enable,
            @reg_date,
            @reg_username,
            @upd_date,
            @upd_username,
            @is_active
        );";
            }

            var param = new
            {
                resource_sharing_rule_id = resourceSharing.ResourceSharingRuleId,
                controller_id = resourceSharing.ApiId,
                sharing_from_vendor_id = resourceSharing.SharingFromVendorId,
                sharing_from_system_id = resourceSharing.SharingFromSystemId,
                resource_sharing_rule_name = resourceSharing.ResourceSharingRuleName,
                sharing_to_vendor_id = resourceSharing.SharingToVendorId,
                sharing_to_system_id = resourceSharing.SharingToSystemId,
                query = resourceSharing.Query,
                roslyn_script = resourceSharing.RoslynScript,
                is_enable = resourceSharing.IsEnable,
                reg_date = UtcNow,
                reg_username = updUserId,
                upd_date = UtcNow,
                upd_username = updUserId,
                is_active = 1
            };

            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param)
               .SetNClob(nameof(param.query))
               .SetNClob(nameof(param.roslyn_script));
            _connection.Execute(twowaySql.Sql, dynParams);
            return resourceSharing;
        }
        [CacheEntityFire(DynamicApiDatabase.TABLE_RESOURCESHARINGRULE)]
        [CacheIdFire("apiId")]
        public void DeleteResourceSharingRule(string resourceSharingRuleId,string apiId)
        {
            var updUserId = Convert.ToString(PerRequestDataContainer.OpenId);
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
Update RESOURCE_SHARING_RULE Set 
    is_active = 0
    , upd_date = /*ds upd_date*/SYSTIMESTAMP 
    , upd_username = /*ds upd_username*/'1' 
Where 
    resource_sharing_rule_id = /*ds resource_sharing_rule_id*/'1' 
";
            }
            else
            {
                sql = @"
Update ResourceSharingRule Set 
    is_active = 0
    , upd_date = @upd_date
    , upd_username = @upd_username
Where 
    resource_sharing_rule_id = @resource_sharing_rule_id
";
            }
            var param = new
            {
                resource_sharing_rule_id = resourceSharingRuleId,
                upd_date = UtcNow,
                upd_username = updUserId,
            };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }
    }
}
