using AutoMapper;
using Dapper.Oracle;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using System.Data.SqlClient;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class CommonIpFilterRepository : AbstractRepository, ICommonIpFilterRepository
    {
        private static readonly JPDataHubLogger s_logger = new(typeof(RepositoryGroupRepository));

        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CommonIpFilterGroupModel, DB_CommonIpFilterGroup>()
                    .ForMember(d => d.common_ip_filter_group_id, o => o.MapFrom(s => Guid.Parse(s.CommonIpFilterGroupId)))
                    .ForMember(d => d.common_ip_filter_group_name, o => o.MapFrom(s => s.CommonIpFilterGroupName))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());

                cfg.CreateMap<CommonIpFilterModel, DB_CommonIpFilter>()
                    .ForMember(d => d.common_ip_filter_id, o => o.MapFrom(s => s.CommonIpFilterId))
                    .ForMember(d => d.ip_address, o => o.MapFrom(s => s.IpAddress))
                    .ForMember(d => d.is_enable, o => o.MapFrom(s => s.IsEnable))
                    .ForMember(d => d.is_active, o => o.MapFrom(s => s.IsActive))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());

                cfg.CreateMap<CommonIpFilterGroupWithIpAddressModel, CommonIpFilterModel>();
            });
            return mappingConfig.CreateMapper();
        });

        private static IMapper Mapper { get { return s_lazyMapper.Value; } }

        [CacheEntity(DynamicApiDatabase.TABLE_COMMONIPFILTERGROUP, DynamicApiDatabase.TABLE_COMMONIPFILTER)]
        [Cache]
        [CacheArg("commonIpFilterGroupNames")]
        public IList<CommonIpFilterGroupInfoModel> GetCommonIPFilter(List<string> commonIpFilterGroupNames)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    cifg.common_ip_filter_group_name AS CommonIpFilterGroupName,
    cif.ip_address AS IpAddress
FROM
    COMMON_IP_FILTER_GROUP cifg 
    INNER JOIN COMMON_IP_FILTER cif ON cifg.common_ip_filter_group_id = cif.common_ip_filter_group_id AND cif.is_enable = 1 AND cif.is_active=1
WHERE    
    cifg.common_ip_filter_group_name IN (/*ds commonIpFilterGroupNames*/'1' )
    AND cifg.is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    cifg.common_ip_filter_group_name AS CommonIpFilterGroupName,
    cif.ip_address AS IpAddress
FROM
    CommonIpFilterGroup AS cifg 
    INNER JOIN CommonIpFilter AS cif ON cifg.common_ip_filter_group_id = cif.common_ip_filter_group_id AND cif.is_enable = 1 AND cif.is_active=1
WHERE    
    cifg.common_ip_filter_group_name IN @commonIpFilterGroupNames
    AND cifg.is_active=1
";
            }
            var param = new { commonIpFilterGroupNames = commonIpFilterGroupNames };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            // dynamicParameterで配列がうまく扱えない為、dynamicParameter化未対応
            var list = Connection.Query<CommonIpFilterGroupResultModel>(twowaySql.Sql, param).ToList();
            List<CommonIpFilterGroupInfoModel> resultModel = new List<CommonIpFilterGroupInfoModel>();
            foreach (var g in list.GroupBy(x => x.CommonIpFilterGroupName))
            {
                resultModel.Add(new CommonIpFilterGroupInfoModel(g.Key, list.Where(y => y.CommonIpFilterGroupName == g.Key).Select(z => z.IpAddress).ToList()));
            }
            return resultModel;
        }

        public CommonIpFilterGroupModel GetCommonIpFilterGroup(string commonIpFilterGroupId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    common_ip_filter_group_id AS CommonIpFilterGroupId
    ,common_ip_filter_group_name AS CommonIpFilterGroupName
FROM
    COMMON_IP_FILTER_GROUP
WHERE
    common_ip_filter_group_id = /*ds commonIpFilterGroupId*/'1' 
AND
    is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    common_ip_filter_group_id AS CommonIpFilterGroupId
    ,common_ip_filter_group_name AS CommonIpFilterGroupName
FROM
    CommonIpFilterGroup
WHERE
    common_ip_filter_group_id = @commonIpFilterGroupId
AND
    is_active=1
";
            }
            var param = new { commonIpFilterGroupId = commonIpFilterGroupId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingle<CommonIpFilterGroupModel>(twowaySql.Sql, dynParams);
        }

        public IList<CommonIpFilterGroupModel> GetCommonIpFilterGroups(IList<CommonIpFilterGroupModel> list)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    common_ip_filter_id AS CommonIpFilterId
    ,ip_address AS IpAddress
    ,is_enable AS IsEnable
    ,common_ip_filter_group_id AS CommonIpFilterGroupId
FROM
    COMMON_IP_FILTER
WHERE
    common_ip_filter_group_id in /*ds commonIpFilterGroupId*/('8B9D7A57-C462-49E7-AD57-20BF941E64BA') AND
    is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    common_ip_filter_id AS CommonIpFilterId
    ,ip_address AS IpAddress
    ,is_enable AS IsEnable
    ,common_ip_filter_group_id AS CommonIpFilterGroupId
FROM
    CommonIpFilter
WHERE
    common_ip_filter_group_id in /*ds commonIpFilterGroupId*/('8B9D7A57-C462-49E7-AD57-20BF941E64BA') AND
    is_active=1
";
            }
            var param = new { commonIpFilterGroupId = list.Select(x => x.CommonIpFilterGroupId).ToList() };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            // dynamicParameterで配列がうまく扱えない為、dynamicParameter化未対応
            var tmp = Connection.Query<CommonIpFilterGroupWithIpAddressModel>(twowaySql.Sql, param);
            list.ForEach(x => x.IpList = Mapper.Map<IList<CommonIpFilterModel>>(tmp.Where(y => y.CommonIpFilterGroupId == x.CommonIpFilterGroupId).ToList()));
            return list;
        }

        public IList<CommonIpFilterModel> GetCommonIpFilterList(string commonIpFilterGroupId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    common_ip_filter_id AS CommonIpFilterId
    ,ip_address AS IpAddress
    ,is_enable AS IsEnable
FROM
    COMMON_IP_FILTER
WHERE
    common_ip_filter_group_id = /*ds commonIpFilterGroupId*/'1' 
 AND
    is_active=1
ORDER BY
    reg_date
";
            }
            else
            {
                sql = @"
SELECT
    common_ip_filter_id AS CommonIpFilterId
    ,ip_address AS IpAddress
    ,is_enable AS IsEnable
FROM
    CommonIpFilter
WHERE
    common_ip_filter_group_id = @commonIpFilterGroupId    
AND
    is_active=1
ORDER BY
    reg_date
";
            }
            var param = new { commonIpFilterGroupId = commonIpFilterGroupId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<CommonIpFilterModel>(twowaySql.Sql, dynParams).ToList();
        }

        public IList<CommonIpFilterGroupNameModel> GetCommonIpFilterGroupList()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    common_ip_filter_group_id AS CommonIpFilterGroupId
    ,common_ip_filter_group_name AS CommonIpFilterGroupName
FROM
    COMMON_IP_FILTER_GROUP
WHERE
    is_active=1
ORDER BY
    reg_date
";
            }
            else
            {
                sql = @"
SELECT
    common_ip_filter_group_id AS CommonIpFilterGroupId
    ,common_ip_filter_group_name AS CommonIpFilterGroupName
FROM
    CommonIpFilterGroup
WHERE
    is_active=1
ORDER BY
    reg_date
";
            }

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, null);
            return Connection.Query<CommonIpFilterGroupNameModel>(twowaySql.Sql).ToList();
        }

        [DomainDataSync(DynamicApiDatabase.TABLE_COMMONIPFILTERGROUP, "CommonIpFilterGroupId")]
        [CacheIdFire("commonIpFilterGroupId", "ipFilterGroup.commonIpFilterGroupId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_COMMONIPFILTERGROUP, DynamicApiDatabase.TABLE_COMMONIPFILTER)]
        public CommonIpFilterGroupModel RegistrationCommonIpFilterGroup(CommonIpFilterGroupModel ipFilterGroup)
        {
            try
            {
                var ipFilterGroupDbModel = Mapper.Map<DB_CommonIpFilterGroup>(ipFilterGroup);
                // CommonIpFilterGroupの登録
                Connection.Insert(ipFilterGroupDbModel);

                // CommonIpFilterの登録
                foreach (var ip in ipFilterGroup.IpList)
                {
                    DB_CommonIpFilter ipFilter = Mapper.Map<DB_CommonIpFilter>(ip);
                    ipFilter.common_ip_filter_group_id = ipFilterGroupDbModel.common_ip_filter_group_id;
                    Connection.Insert(ipFilter);
                }
            }
            catch (SqlException ex)
            {
                // UNIQUE制約違反の場合、独自例外を返す
                if (ex.Number == 2627) throw new AlreadyExistsException(ex.Message);
                // 外部キー制約違反の場合、独自例外を返す
                if (ex.Number == 547) throw new ForeignKeyException(ex.Message);
                throw new SqlDatabaseException(ex.Message);
            }
            return ipFilterGroup;
        }

        [DomainDataSync(DynamicApiDatabase.TABLE_COMMONIPFILTERGROUP, "CommonIpFilterGroupId")]
        [CacheIdFire("commonIpFilterGroupId", "ipFilterGroup.commonIpFilterGroupId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_COMMONIPFILTERGROUP, DynamicApiDatabase.TABLE_COMMONIPFILTER)]
        public CommonIpFilterGroupModel UpdateCommonIpFilterGroup(CommonIpFilterGroupModel ipFilterGroup)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();

            // CommonIpFilterGroupの更新
            var groupSql = "";
            if (dbSettings.Type == "Oracle")
            {
                groupSql = @"
UPDATE
    COMMON_IP_FILTER_GROUP
SET
    common_ip_filter_group_name = /*ds commonIpFilterGroupName*/'1' 
    ,upd_date = SYSTIMESTAMP
    ,upd_username = /*ds openId*/'1' 
    ,is_active = 1
WHERE
    common_ip_filter_group_id = /*ds commonIpFilterGroupId*/'1' 
 AND
    is_active = 1
";
            }
            else
            {
                groupSql = @"
UPDATE
    CommonIpFilterGroup
SET
    common_ip_filter_group_name = @commonIpFilterGroupName
    ,upd_date = GETDATE()
    ,upd_username = @openId
    ,is_active = 1
WHERE
    common_ip_filter_group_id = @commonIpFilterGroupId
AND
    is_active = 1
";
            }


            // CommonIpFilterの更新
            var ipFilterSql = "";
            if (dbSettings.Type == "Oracle")
            {
                ipFilterSql = @"
MERGE INTO COMMON_IP_FILTER target
USING
(
    SELECT
        /*ds commonIpFilterId*/'1'  AS common_ip_filter_id
    FROM DUAL
) source
ON
    (target.common_ip_filter_id = source.common_ip_filter_id)
WHEN MATCHED THEN
    UPDATE
    SET
        ip_address = CASE WHEN /*ds isActive*/1 = 1 THEN TO_NCHAR(/*ds ipAddress*/'1' ) ELSE ip_address END
        ,is_enable = CASE WHEN /*ds isActive*/1 = 1 THEN /*ds isEnable*/1 ELSE is_enable END
        ,upd_date = SYSTIMESTAMP
        ,upd_username = /*ds openId*/'1' 
        ,is_active= 1
WHEN NOT MATCHED THEN
    INSERT
    (
        common_ip_filter_id
        ,ip_address
        ,is_enable
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
        ,common_ip_filter_group_id
    )
    VALUES
    (
        /*ds commonIpFilterId*/'1' 
        ,/*ds ipAddress*/'1' 
        ,/*ds isEnable*/1 
        ,SYSTIMESTAMP
        ,/*ds openId*/'1' 
        ,SYSTIMESTAMP
        ,/*ds openId*/'1' 
        ,1
        ,/*ds commonIpFilterGroupId*/'1' 
    )
";
            }
            else
            {
                ipFilterSql = @"
MERGE INTO CommonIpFilter AS target
USING
(
    SELECT
        @commonIpFilterId AS common_ip_filter_id
) AS source
ON
    target.common_ip_filter_id = source.common_ip_filter_id
WHEN MATCHED THEN
    UPDATE
    SET
        ip_address = CASE WHEN @isActive = 1 THEN @ipAddress ELSE ip_address END
        ,is_enable = CASE WHEN @isActive = 1 THEN @isEnable ELSE is_enable END
        ,upd_date = GETDATE()
        ,upd_username = @openId
        ,is_active= 1
WHEN NOT MATCHED THEN
    INSERT
    (
        common_ip_filter_id
        ,ip_address
        ,is_enable
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
        ,common_ip_filter_group_id
    )
    VALUES
    (
        @commonIpFilterId
        ,@ipAddress
        ,@isEnable
        ,GETDATE()
        ,@openId
        ,GETDATE()
        ,@openId
        ,1
        ,@commonIpFilterGroupId
    );
";
            }

            try
            {
                // CommonIpFilterGroupの更新
                var groupParam = new
                {
                    commonIpFilterGroupName = ipFilterGroup.CommonIpFilterGroupName,
                    openId = PerRequestDataContainer.OpenId,
                    commonIpFilterGroupId = ipFilterGroup.CommonIpFilterGroupId
                };
                var groupTwowaySql = new TwowaySqlParser(dbSettings.GetDbType(), groupSql, groupParam);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(groupParam);
                Connection.ExecutePrimaryKey(groupTwowaySql.Sql, dynParams);
                // CommonIpFilterの更新
                foreach (var ip in ipFilterGroup.IpList)
                {
                    var param = new
                    {
                        commonIpFilterId = ip.CommonIpFilterId,
                        ipAddress = ip.IpAddress,
                        isEnable = ip.IsEnable,
                        isActive = true,
                        openId = PerRequestDataContainer.OpenId,
                        commonIpFilterGroupId = ipFilterGroup.CommonIpFilterGroupId
                    };
                    var ipFilterTwowaySql = new TwowaySqlParser(dbSettings.GetDbType(), ipFilterSql, param);
                    dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                    Connection.ExecutePrimaryKey(ipFilterTwowaySql.Sql, dynParams);
                }
            }
            catch (SqlException ex)
            {
                // UNIQUE制約違反の場合、独自例外を返す
                if (ex.Number == 2627) throw new AlreadyExistsException(ex.Message);
                // 外部キー制約違反の場合、独自例外を返す
                if (ex.Number == 547) throw new ForeignKeyException(ex.Message);
                throw new SqlDatabaseException(ex.Message);
            }
            return ipFilterGroup;
        }

        [DomainDataSync(DynamicApiDatabase.TABLE_COMMONIPFILTERGROUP, "commonIpFilterGroupId")]
        [CacheIdFire("commonIpFilterGroupId", "commonIpFilterGroupId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_COMMONIPFILTERGROUP, DynamicApiDatabase.TABLE_COMMONIPFILTER)]
        public void DeleteCommonIpFilterGroup(string commonIpFilterGroupId)
        {

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();

            // CommonIpFilterGroupの更新
            var groupSql = "";
            if (dbSettings.Type == "Oracle")
            {
                groupSql = @"
UPDATE
    COMMON_IP_FILTER_GROUP
SET
    upd_date = SYSTIMESTAMP
    ,upd_username = /*ds openId*/'1' 
    ,is_active = 0
WHERE
    common_ip_filter_group_id = /*ds commonIpFilterGroupId*/'1' 
";
            }
            else
            {
                groupSql = @"
UPDATE
    CommonIpFilterGroup
SET
    upd_date = GETDATE()
    ,upd_username = @openId
    ,is_active = 0
WHERE
    common_ip_filter_group_id = @commonIpFilterGroupId
";
            }
            var groupParam = new { openId = PerRequestDataContainer.OpenId, commonIpFilterGroupId = commonIpFilterGroupId };
            var groupTwowaySql = new TwowaySqlParser(dbSettings.GetDbType(), groupSql, groupParam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(groupParam);
            Connection.ExecutePrimaryKey(groupTwowaySql.Sql, dynParams);
            // CommonIpFilterの更新
            var ipFilterSql = "";
            if (dbSettings.Type == "Oracle")
            {
                ipFilterSql = @"
UPDATE
    COMMON_IP_FILTER
SET
    upd_date = SYSTIMESTAMP
    ,upd_username = /*ds openId*/'1' 
    ,is_active = 0
WHERE
    common_ip_filter_group_id = /*ds commonIpFilterGroupId*/'1' 
";
            }
            else
            {
                ipFilterSql = @"
UPDATE
    CommonIpFilter
SET
    upd_date = GETDATE()
    ,upd_username = @openId
    ,is_active = 0
WHERE
    common_ip_filter_group_id = @commonIpFilterGroupId
";
            }
            var ipFilterParam = new { openId = PerRequestDataContainer.OpenId, commonIpFilterGroupId = commonIpFilterGroupId };
            var ipFilterTwowaySql = new TwowaySqlParser(dbSettings.GetDbType(), ipFilterSql, ipFilterParam);
            dynParams = dbSettings.GetParameters().AddDynamicParams(ipFilterParam);
            Connection.Execute(ipFilterTwowaySql.Sql, dynParams);
        }
    }
}