using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using AutoMapper;
using System.Data.SqlClient;
using JP.DataHub.Api.Core.Repository;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using Dapper.Oracle;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class RepositoryGroupRepository : AbstractRepository, IRepositoryGroupRepository
    {
        private static readonly JPDataHubLogger s_logger = new(typeof(RepositoryGroupRepository));

        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get => s_lazyMapper.Value; }

        public RepositoryGroupModel GetRepositoryGroup(string repositoryGroupId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId
    ,rg.repository_group_name AS RepositoryGroupName
    ,rg.repository_type_cd AS RepositoryTypeCd
    ,sort_no AS SortNo
    ,is_default AS IsDefault
    ,is_enable AS IsEnable
FROM
    REPOSITORY_GROUP rg
WHERE
    rg.is_active = 1
   AND rg.repository_group_id = /*ds repository_group_id*/'00000000-0000-0000-0000-000000000000' 
";
            }
            else
            {
                sql = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId
    ,rg.repository_group_name AS RepositoryGroupName
    ,rg.repository_type_cd AS RepositoryTypeCd
    ,sort_no AS SortNo
    ,is_default AS IsDefault
    ,is_enable AS IsEnable
FROM
    RepositoryGroup rg
WHERE
    rg.is_active = 1
   AND rg.repository_group_id = @repository_group_id
";
            }

            var param = new { repository_group_id = repositoryGroupId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingleOrDefault<RepositoryGroupModel>(twowaySql.Sql, dynParams);
            if (result == null)
                throw new NotFoundException($"Not Found Repository Group id={repositoryGroupId}");

            return result;
        }

        /// <summary>
        /// リポジトリグループリストを取得します。
        /// </summary>
        /// <returns>リポジトリグループリスト</returns>
        [CacheArg(allParam: true)]
        [CacheEntity(DynamicApiDatabase.TABLE_REPOSITORYGROUP)]
        [Cache]
        public IList<RepositoryGroupModel> GetRepositoryGroupList(string vendorId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sqlAllVendor = "";
            var sqlVendor = "";


            if (dbSettings.Type == "Oracle")
            {
                sqlAllVendor = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId
    ,rg.repository_group_name AS RepositoryGroupName
    ,rg.repository_type_cd AS RepositoryTypeCd
    ,sort_no AS SortNo
    ,is_default AS IsDefault
    ,is_enable AS IsEnable
    ,rt.is_container_separation
    ,rt.repository_type_name
FROM
    REPOSITORY_GROUP rg
    INNER JOIN REPOSITORY_TYPE rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.is_active = 1
WHERE
    rg.is_active = 1
ORDER BY
    rg.sort_no
";
                sqlVendor = @"
SELECT
	rg.repository_group_id AS RepositoryGroupId
	,rg.repository_group_name AS RepositoryGroupName
	,rg.repository_type_cd AS RepositoryTypeCd
	,rg.sort_no AS SortNo
    ,rg.is_default AS IsDefault
    ,rg.is_enable AS IsEnable
    ,rt.is_container_separation
    ,rt.repository_type_name  AS RepositoryTypeName
FROM
    REPOSITORY_GROUP rg
    INNER JOIN REPOSITORY_TYPE rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.is_active = 1
    INNER JOIN VENDOR_REPOSITORY_GROUP vr ON rg.repository_group_id = vr.repository_group_id AND vr.is_active = 1
WHERE
    vr.vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' AND 
    rg.is_active = 1
ORDER BY
	rg.sort_no
";
            }
            else
            {
                sqlAllVendor = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId
    ,rg.repository_group_name AS RepositoryGroupName
    ,rg.repository_type_cd AS RepositoryTypeCd
    ,sort_no AS SortNo
    ,is_default AS IsDefault
    ,is_enable AS IsEnable
    ,rt.is_container_separation
    ,rt.repository_type_name
FROM
    RepositoryGroup rg
    INNER JOIN RepositoryType rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.is_active = 1
WHERE
    rg.is_active = 1
ORDER BY
    rg.sort_no
";
                sqlVendor = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId
    ,rg.repository_group_name AS RepositoryGroupName
    ,rg.repository_type_cd AS RepositoryTypeCd
    ,rg.sort_no AS SortNo
    ,rg.is_default AS IsDefault
    ,rg.is_enable AS IsEnable
    ,rt.is_container_separation
    ,rt.repository_type_name  AS RepositoryTypeName
FROM
    RepositoryGroup rg
    INNER JOIN RepositoryType rt ON rg.repository_type_cd = rt.repository_type_cd AND rt.is_active = 1
    INNER JOIN VendorRepositoryGroup vr ON rg.repository_group_id = vr.repository_group_id AND vr.is_active = 1
WHERE
    vr.vendor_id = @vendor_id AND 
    rg.is_active = 1
ORDER BY
    rg.sort_no
";
            }

            var sql = string.IsNullOrEmpty(vendorId) == true ? sqlAllVendor : sqlVendor;
            var param = new { vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<RepositoryGroupModel>(twowaySql.Sql, dynParams);
            return result?.ToList();
        }

        public List<PhysicalRepositoryModel> GetPhysicalRepository(string repositoryGroupId)
        {
            if (string.IsNullOrEmpty(repositoryGroupId))
            {
                throw new ArgumentNullException(nameof(repositoryGroupId));
            }

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    physical_repository_id AS PhysicalRepositoryId
    ,repository_group_id AS RepositoryGroupId
    ,connection_string AS ConnectionString
    ,is_full AS IsFull
    ,is_active AS IsActive
FROM
    PHYSICAL_REPOSITORY
WHERE
    is_active = 1
   AND repository_group_id = /*ds repository_group_id*/'00000000-0000-0000-0000-000000000000' 
";
            }
            else
            {
                sql = @"
SELECT
    physical_repository_id AS PhysicalRepositoryId
    ,repository_group_id AS RepositoryGroupId
    ,connection_string AS ConnectionString
    ,is_full AS IsFull
    ,is_active AS IsActive
FROM
    PhysicalRepository
WHERE
    is_active = 1
   AND repository_group_id = @repository_group_id
";
            }

            var param = new { repository_group_id = repositoryGroupId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<PhysicalRepositoryModel>(twowaySql.Sql, dynParams).ToList();
        }

        [CacheEntity(DynamicApiDatabase.TABLE_REPOSITORYTYPE)]
        [Cache]
        public IList<RepositoryTypeModel> GetRepositoryTypeList()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    repository_type_cd AS RepositoryTypeCd
    ,repository_type_name AS RepositoryTypeName
FROM
    REPOSITORY_TYPE
WHERE
    is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    repository_type_cd AS RepositoryTypeCd
    ,repository_type_name AS RepositoryTypeName
FROM
    RepositoryType
WHERE
    is_active = 1
";
            }

            var result = Connection.Query<RepositoryTypeModel>(sql).ToList();
            if (result.Count == 0)
                throw new NotFoundException($"Not Found Repository Group List");

            return result;
        }

        [CacheIdFire("repositoryGroupId", "repositoryGroup.RepositoryGroupId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_REPOSITORYGROUP)]
        public RepositoryGroupModel MergeRepositoryGroup(RepositoryGroupModel repositoryGroup)
        {
            if (repositoryGroup == null)
            {
                throw new ArgumentNullException(nameof(repositoryGroup));
            }

            if (!Guid.TryParse(repositoryGroup.RepositoryGroupId, out Guid guid))
            {
                throw new InvalidCastException(nameof(repositoryGroup.RepositoryGroupId));
            }

            if (string.IsNullOrEmpty(repositoryGroup.RepositoryGroupName))
            {
                throw new ArgumentNullException(nameof(repositoryGroup.RepositoryGroupName));
            }

            if (string.IsNullOrEmpty(repositoryGroup.RepositoryTypeCd))
            {
                throw new ArgumentNullException(nameof(repositoryGroup.RepositoryTypeCd));
            }

            var now = this.PerRequestDataContainer.GetDateTimeUtil().GetUtc(this.PerRequestDataContainer.GetDateTimeUtil().LocalNow);
            var updUserId = Convert.ToString(this.PerRequestDataContainer.OpenId);

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
MERGE INTO REPOSITORY_GROUP target
USING
(
    SELECT
        /*ds repository_group_id*/'1' AS repository_group_id
    FROM DUAL
) source
ON
    (target.repository_group_id = source.repository_group_id)
WHEN MATCHED THEN
    UPDATE
    SET
        repository_group_name = /*ds repository_group_name*/'1' 
        ,repository_type_cd = /*ds repository_type_cd*/'1' 
        ,sort_no = /*ds sort_no*/1 
        ,is_active = /*ds is_active*/1 
        ,is_default = /*ds is_default*/1 
        ,is_enable = /*ds is_enable*/1 
        ,upd_date = /*ds upd_date*/systimestamp 
        ,upd_username = /*ds upd_username*/'1' 
WHEN NOT MATCHED THEN
    INSERT
    (
        repository_group_id
        ,repository_group_name
        ,repository_type_cd
        ,sort_no
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
        ,is_default
        ,is_enable
    )
    VALUES
    (
        /*ds repository_group_id*/'1' 
        ,/*ds repository_group_name*/'1' 
        ,/*ds repository_type_cd*/'1' 
        ,/*ds sort_no*/1 
        ,/*ds reg_date*/systimestamp 
        ,/*ds reg_username*/'1' 
        ,/*ds upd_date*/systimestamp 
        ,/*ds upd_username*/'1' 
        ,/*ds is_active*/1 
        ,/*ds is_default*/1 
        ,/*ds is_enable*/1 
    )
";
            }
            else
            {
                sql = @"
MERGE INTO RepositoryGroup AS target
USING
(
    SELECT
        @repository_group_id AS repository_group_id
) AS source
ON
    target.repository_group_id = source.repository_group_id
WHEN MATCHED THEN
    UPDATE
    SET
        repository_group_name = @repository_group_name
        ,repository_type_cd = @repository_type_cd
        ,sort_no = @sort_no
        ,is_active = @is_active
        ,is_default = @is_default
        ,is_enable = @is_enable
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        repository_group_id
        ,repository_group_name
        ,repository_type_cd
        ,sort_no
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
        ,is_default
        ,is_enable
    )
    VALUES
    (
        @repository_group_id
        ,@repository_group_name
        ,@repository_type_cd
        ,@sort_no
        ,@reg_date
        ,@reg_username
        ,@upd_date
        ,@upd_username
        ,@is_active
        ,@is_default
        ,@is_enable
    );
";
            }

            var param = new
            {
                repository_group_id = repositoryGroup.RepositoryGroupId,
                repository_group_name = repositoryGroup.RepositoryGroupName,
                repository_type_cd = repositoryGroup.RepositoryTypeCd,
                sort_no = repositoryGroup.SortNo,
                reg_date = now,
                reg_username = updUserId,
                upd_date = now,
                upd_username = updUserId,
                is_active = true,
                is_default = repositoryGroup.IsDefault,
                is_enable = repositoryGroup.IsEnable.Value
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            try
            {
                // 登録実行
                Connection.Execute(twowaySql.Sql, dynParams);
            }
            catch (SqlException ex)
            {
                s_logger.Error($"Sql Error: {ex.Number} {ex.Message}", ex);
                if (ex.Number == 547) throw new ForeignKeyException(ex.Message);
                else throw new SqlDatabaseException(ex.Message);
            }

            repositoryGroup.PhysicalRepositoryList?.ForEach(x =>
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
MERGE INTO PHYSICAL_REPOSITORY target
USING
(
    SELECT
        /*ds physical_repository_id*/'1' AS physical_repository_id
    FROM DUAL
) source
ON
    (target.physical_repository_id = source.physical_repository_id)
WHEN MATCHED THEN
    UPDATE
    SET
        repository_group_id = /*ds repository_group_id*/'1' 
        ,connection_string = /*ds connection_string*/'1' 
        ,is_full = /*ds is_full*/1 
        ,is_active = /*ds is_active*/1 
        ,upd_date = /*ds upd_date*/systimestamp 
        ,upd_username = /*ds upd_username*/'1'  
WHEN NOT MATCHED THEN
    INSERT
    (
        physical_repository_id
        ,repository_group_id
        ,connection_string
        ,is_full
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        /*ds physical_repository_id*/'1' 
        ,/*ds repository_group_id*/'1' 
        ,/*ds connection_string*/'1' 
        ,/*ds is_full*/1 
        ,/*ds reg_date*/systimestamp 
        ,/*ds reg_username*/'1' 
        ,/*ds upd_date*/systimestamp 
        ,/*ds upd_username*/'1' 
        ,/*ds is_active*/1 
    )
";
                }
                else
                {
                    sql = @"
MERGE INTO PhysicalRepository AS target
USING
(
    SELECT
        @physical_repository_id AS physical_repository_id
) AS source
ON
    target.physical_repository_id = source.physical_repository_id
WHEN MATCHED THEN
    UPDATE
    SET
        repository_group_id = @repository_group_id
        ,connection_string = @connection_string
        ,is_full = @is_full
        ,is_active = @is_active
        ,upd_date = @upd_date
        ,upd_username = @upd_username
WHEN NOT MATCHED THEN
    INSERT
    (
        physical_repository_id
        ,repository_group_id
        ,connection_string
        ,is_full
        ,reg_date
        ,reg_username
        ,upd_date
        ,upd_username
        ,is_active
    )
    VALUES
    (
        @physical_repository_id
        ,@repository_group_id
        ,@connection_string
        ,@is_full
        ,@reg_date
        ,@reg_username
        ,@upd_date
        ,@upd_username
        ,@is_active
    );
";
                }

                var param = new
                {
                    physical_repository_id = x.PhysicalRepositoryId,
                    repository_group_id = repositoryGroup.RepositoryGroupId,
                    connection_string = x.ConnectionString,
                    is_full = x.IsFull,
                    reg_date = now,
                    reg_username = updUserId,
                    upd_date = now,
                    upd_username = updUserId,
                    is_active = x.IsActive,
                };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                try
                {
                    // 登録実行
                    Connection.Execute(twowaySql.Sql, dynParams);
                }
                catch (SqlException ex)
                {
                    s_logger.Error($"Sql Error: {ex.Number} {ex.Message}", ex);
                    if (ex.Number == 547) throw new ForeignKeyException(ex.Message);
                    else throw new SqlDatabaseException(ex.Message);
                }
            });

            return repositoryGroup;
        }

        [CacheIdFire("repositoryGroupId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_REPOSITORYGROUP)]
        public void DeleteReposigoryGroup(string repositoryGroupId)
        {
            if (string.IsNullOrEmpty(repositoryGroupId))
            {
                throw new ArgumentNullException(nameof(repositoryGroupId));
            }

            if (!Guid.TryParse(repositoryGroupId, out Guid guid))
            {
                throw new InvalidCastException(nameof(repositoryGroupId));
            }

            // API等、他で使用しているか
            if (!this.ValidateUsedReposigoryGroup(repositoryGroupId))
            {
                throw new InUseException("指定されたリポジトリグループは他で使用されている為削除できません。");
            }

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    REPOSITORY_GROUP
SET
    is_active = 0
    ,upd_date = /*ds upd_date*/systimestamp 
    ,upd_username = /*ds upd_username*/'1' 
WHERE
    repository_group_id = /*ds repository_group_id*/'1' 
";
            }
            else
            {
                sql = @"
UPDATE
    RepositoryGroup
SET
    is_active = 0
    ,upd_date = @upd_date
    ,upd_username = @upd_username
WHERE
    repository_group_id = @repository_group_id
";
            }

            var now = this.PerRequestDataContainer.GetDateTimeUtil().GetUtc(this.PerRequestDataContainer.GetDateTimeUtil().LocalNow);
            var updUserId = Convert.ToString(this.PerRequestDataContainer.OpenId);

            var param = new { repository_group_id = repositoryGroupId, upd_date = now, upd_username = updUserId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.Execute(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// 対象のリポジトリタイプと一致するか
        /// </summary>
        /// <param name="repositoryGroupId">リポジトリグループコード</param>
        /// <param name="repositoryTypeList">対象リポジトリタイプリスト</param>
        /// <returns>true:一致している/false:一致していない</returns>
        public bool MatchRepositoryType(string repositoryGroupId, string[] repositoryTypeList)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(rg.repository_group_id)
FROM
    REPOSITORY_GROUP rg
WHERE
    rg.repository_group_id = /*ds repositoryGroupId*/'1' 
    AND rg.repository_type_cd in /*ds repositoryTypeList*/('ssd')
    AND rg.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(rg.repository_group_id)
FROM
    RepositoryGroup rg
WHERE
    rg.repository_group_id = @repositoryGroupId
    AND rg.repository_type_cd in /*ds repositoryTypeList*/('ssd')
    AND rg.is_active = 1
";
            }

            var param = new { repositoryGroupId, repositoryTypeList = repositoryTypeList };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            // dynamicParameterで配列がうまく扱えない為、dynamicParameter化未対応
            var result = Connection.QuerySingle<int>(twowaySql.Sql, param);
            return result != 0;
        }

        private bool ValidateUsedReposigoryGroup(string repositoryGroupId)
        {
            // ControllerとAPIで使用しているか（Controllerが消えているとAPIが消せないので、Controllerが消えていたら削除許可する）

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var apiSql = "";
            if (dbSettings.Type == "Oracle")
            {
                apiSql = @"
SELECT
    COUNT(api_id)
FROM
    API
    INNER JOIN Controller ON Api.controller_id = Controller.controller_id
WHERE
    Api.is_active = 1
    AND Controller.is_active = 1
    AND repository_group_id = /*ds repository_group_id*/'1' 
";
            }
            else
            {
                apiSql = @"
SELECT
    COUNT(api_id)
FROM
    Api
    INNER JOIN Controller ON Api.controller_id = Controller.controller_id
WHERE
    Api.is_active = 1
    AND Controller.is_active = 1
    AND repository_group_id = @repository_group_id
";
            }
            var apiSqlparam = new { repository_group_id = repositoryGroupId };
            var twowaySqlForApiSql = new TwowaySqlParser(dbSettings.GetDbType(), apiSql, apiSqlparam);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(apiSqlparam);
            if (Connection.QuerySingle<int>(twowaySqlForApiSql.Sql, dynParams) > 0)
            {
                return false;
            }

            // SecondRepositoryで使用しているか
            var secRepSql = "";
            if (dbSettings.Type == "Oracle")
            {
                secRepSql = @"
SELECT
    COUNT(s.secondary_repository_map_id)
FROM
    SECONDARY_REPOSITORY_MAP s
    INNER JOIN API a ON s.api_id = a.api_id AND a.is_active = 1
    INNER JOIN CONTROLLER c ON a.controller_id = c.controller_id AND c.is_active = 1
WHERE
    s.is_active = 1
    AND s.repository_group_id = /*ds repository_group_id*/'1' 
";
            }
            else
            {
                secRepSql = @"
SELECT
    COUNT(s.secondary_repository_map_id)
FROM
    SecondaryRepositoryMap s
    INNER JOIN Api a ON s.api_id = a.api_id AND a.is_active = 1
    INNER JOIN Controller c ON a.controller_id = c.controller_id AND c.is_active = 1
WHERE
    s.is_active = 1
    AND s.repository_group_id = @repository_group_id
";
            }

            var secReqSqlParam = new { repository_group_id = repositoryGroupId };
            var twowaySqlForSecRepSql = new TwowaySqlParser(dbSettings.GetDbType(), secRepSql, secReqSqlParam);
            dynParams = dbSettings.GetParameters().AddDynamicParams(secReqSqlParam);
            if (Connection.QuerySingle<int>(twowaySqlForSecRepSql.Sql, dynParams) > 0)
            {
                return false;
            }

            return true;
        }

        public void ActivateVendorRepositoryGroupList(string vendorId, IList<string> repositoryGroupIds)
        {
            if (!(repositoryGroupIds?.Count() != 0))
            {
                return;
            }
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    VENDOR_REPOSITORY_GROUP
SET
    is_active=0
    ,upd_date=/*ds now*/systimestamp 
    ,upd_username=/*ds open_id*/'1' 
WHERE
    vendor_id=/*ds vendor_id*/'1'
";
            }
            else
            {
                sql = @"
UPDATE
    VendorRepositoryGroup
SET
    is_active=0
    ,upd_date=@now
    ,upd_username=@open_id
WHERE
    vendor_id=@vendor_id
";
            }
            var param = new { vendor_id = vendorId, now = UtcNow, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.Execute(twowaySql.Sql, dynParams);
            repositoryGroupIds.ToList().ForEach(x => {
                var model = new DB_VendorRepositoryGroup()
                {
                    vendor_repositorygroup_id = Guid.NewGuid(),
                    vendor_id = vendorId.To<Guid>(),
                    repository_group_id = x.To<Guid>()
                };
                model.UpdateFiveColumn();
                Connection.Insert(model);
            });
        }

        [CacheIdFire("repositoryGroupId")]
        [CacheEntityFire(DynamicApiDatabase.TABLE_VENDORREPOSITORYGROUP)]
        public void ActivateVendorRepositoryGroup(string vendorId, string repositoryGroupId, bool isActive)
        {
            if (string.IsNullOrEmpty(vendorId))
            {
                throw new ArgumentNullException(nameof(vendorId));
            }
            if (string.IsNullOrEmpty(repositoryGroupId))
            {
                throw new ArgumentNullException(nameof(repositoryGroupId));
            }
            ExecuteUpsertVendorRepositoryGroup(vendorId, repositoryGroupId, isActive);
        }

        public bool CheckUsedVendorRepositoryGroup(string repositoryGroupId, string vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    CONTROLLER c
    INNER JOIN API a ON c.controller_id = a.controller_id AND c.is_active = 1
WHERE
    a.is_active = 1
    AND a.repository_group_id = /*ds repository_group_id*/'1' 
    AND c.vendor_id = /*ds vendor_id*/'1' 
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    Controller c
    INNER JOIN Api a ON c.controller_id = a.controller_id AND c.is_active = 1
WHERE
    a.is_active = 1
    AND a.repository_group_id = @repository_group_id
    AND c.vendor_id = @vendor_id
";
            }
            var param = new { repository_group_id = repositoryGroupId, vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            // 1件以上あればTrue
            if (Connection.QuerySingle<int>(twowaySql.Sql, dynParams) > 0)
            {
                return true;
            }

            //アタッチファイル（Blob、Meta）で使用されているかどうかも見る
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    SECONDARY_REPOSITORY_MAP s
    INNER JOIN API a ON s.api_id = a.api_id AND s.is_active = 1
    INNER JOIN CONTROLLER c ON c.controller_id = a.controller_id
WHERE
    c.is_active = 1
    AND a.repository_group_id = /*ds repository_group_id*/'1' 
    AND c.vendor_id = /*ds vendor_id*/'1' 
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    SecondaryRepositoryMap s
    INNER JOIN Api a ON s.api_id = a.api_id AND s.is_active = 1
    INNER JOIN Controller c ON c.controller_id = a.controller_id
WHERE
    c.is_active = 1
    AND s.repository_group_id = @repository_group_id
    AND c.vendor_id = @vendor_id
";
            }

            param = new { repository_group_id = repositoryGroupId, vendor_id = vendorId };
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return (Connection.QuerySingle<int>(twowaySql.Sql, dynParams) > 0);
        }

        public VendorRepositoryGroupModel GetVendorRepositoryGroupInfo(string vendorId, string repositoryGroupId)
        {
            if (string.IsNullOrEmpty(vendorId))
            {
                throw new ArgumentNullException(nameof(vendorId));
            }

            if (string.IsNullOrEmpty(repositoryGroupId))
            {
                throw new ArgumentNullException(nameof(repositoryGroupId));
            }

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId,
    rg.repository_group_name AS RepositoryGroupName,
    vrg.vendor_id AS VendorId
FROM 
    REPOSITORY_GROUP rg 
    INNER JOIN VENDOR_REPOSITORY_GROUP vrg ON rg.repository_group_id = vrg.repository_group_id
        AND vrg.repository_group_id = /*ds repository_group_id*/'1' 
        AND vrg.vendor_id = /*ds vendor_id*/'1' 
        AND vrg.is_active = 1
WHERE 
    rg.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId,
    rg.repository_group_name AS RepositoryGroupName,
    vrg.vendor_id AS VendorId
FROM 
    RepositoryGroup rg 
    INNER JOIN VendorRepositoryGroup vrg ON rg.repository_group_id = vrg.repository_group_id
        AND vrg.repository_group_id = @repository_group_id
        AND vrg.vendor_id = @vendor_id
        AND vrg.is_active = 1
WHERE 
    rg.is_active = 1
";
            }
            var param = new { vendor_id = vendorId, repository_group_id = repositoryGroupId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingleOrDefault<VendorRepositoryGroupModel>(twowaySql.Sql, dynParams);

            if (result == null)
                throw new NotFoundException($"Not Found Vendor id={vendorId}, Repository Group id = {repositoryGroupId}");

            return result;
        }

        public IList<VendorRepositoryGroupModel> GetVendorRepositoryGroupInfoExList()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId,
    rg.repository_group_name AS RepositoryGroupName,
    vrg.vendor_id AS VendorId,
    CASE
    (
        SELECT
            count(*)
        FROM
            CONTROLLER c
        WHERE 
            c.vendor_id = vrg.vendor_id and c.is_active = 1
        AND
            (
                EXISTS (SELECT * FROM API a where a.controller_id = c.controller_id and a.repository_group_id = rg.repository_group_id and a.is_active = 1)
                or 
                EXISTS (SELECT * FROM
                                     SECONDARY_REPOSITORY_MAP s
                                     INNER JOIN  API a ON s.api_id = a.api_id AND a.is_active = 1
                                 WHERE
                                     a.controller_id = c.controller_id and s.is_active = 1
                                     AND s.repository_group_id = rg.repository_group_id)
            )
    )
WHEN 0 THEN 0 ELSE 1 END AS Used
FROM
    REPOSITORY_GROUP rg 
    INNER JOIN VENDOR_REPOSITORY_GROUP vrg on rg.repository_group_id = vrg.repository_group_id  and vrg.is_active = 1
    INNER JOIN VENDOR v on v.vendor_id = vrg.vendor_id  and v.is_active = 1 and v.is_enable = 1
WHERE 
    rg.is_active = 1
ORDER BY
    rg.reg_date
";
            }
            else
            {
                sql = @"
SELECT
    rg.repository_group_id AS RepositoryGroupId,
    rg.repository_group_name AS RepositoryGroupName,
    vrg.vendor_id AS VendorId,
    CASE
    (
        SELECT
            count(*)
        FROM
            Controller c
        WHERE 
            c.vendor_id = vrg.vendor_id and c.is_active = 1
        AND
            (
                EXISTS (SELECT * FROM api a where a.controller_id = c.controller_id and a.repository_group_id = rg.repository_group_id and a.is_active = 1)
                or 
                EXISTS (SELECT * FROM
                                     SecondaryRepositoryMap s
                                     INNER JOIN  Api a ON s.api_id = a.api_id AND a.is_active = 1
                                 WHERE
                                     a.controller_id = c.controller_id and s.is_active = 1
                                     AND s.repository_group_id = rg.repository_group_id)
            )
    )
WHEN 0 THEN 0 ELSE 1 END AS Used
FROM
    RepositoryGroup rg 
    INNER JOIN VendorRepositoryGroup vrg on rg.repository_group_id = vrg.repository_group_id  and vrg.is_active = 1
    INNER JOIN Vendor v on v.vendor_id = vrg.vendor_id  and v.is_active = 1 and v.is_enable = 1
WHERE 
    rg.is_active = 1
ORDER BY
    rg.reg_date
";
            }

            return Connection.Query<VendorRepositoryGroupModel>(sql).ToList();
        }

        public IList<VendorRepositoryGroupModel> GetVendorRepositoryGroupInfoList(string vendorId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    vrg.vendor_id AS VendorId
    ,vrg.repository_group_id AS RepositoryGroupId
    ,rg.repository_group_name AS RepositoryGroupName
FROM 
    VENDOR v
    INNER JOIN VENDOR_REPOSITORY_GROUP vrg ON v.vendor_id=vrg.vendor_id AND vrg.is_active=1
    INNER JOIN REPOSITORY_GROUP rg ON vrg.repository_group_id=rg.repository_group_id AND rg.is_active=1
WHERE 
/*ds if vendor_id != null*/
    vrg.vendor_id= /*ds vendor_id*/'1'  AND
/*ds end if*/
    v.is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    vrg.vendor_id AS VendorId
    ,vrg.repository_group_id AS RepositoryGroupId
    ,rg.repository_group_name AS RepositoryGroupName
FROM 
    Vendor AS v
    INNER JOIN VendorRepositoryGroup AS vrg ON v.vendor_id=vrg.vendor_id AND vrg.is_active=1
    INNER JOIN RepositoryGroup AS rg ON vrg.repository_group_id=rg.repository_group_id AND rg.is_active=1
WHERE 
/*ds if vendor_id != null*/
    vrg.vendor_id=@vendor_id AND
/*ds end if*/
    v.is_active=1
";
            }

            var param = new { vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = Connection.Query<VendorRepositoryGroupModel>(twowaySql.Sql, dynParams).ToList();

            if (result == null || !result.Any())
            {
                throw new NotFoundException($"Not Found endorRepositoryGroup vendorId={vendorId}");
            }
            return result;
        }


        private void ExecuteUpsertVendorRepositoryGroup(string vendorId, string repositoryGroupId, bool isActive)
        {
            var now = this.PerRequestDataContainer.GetDateTimeUtil().GetUtc(this.PerRequestDataContainer.GetDateTimeUtil().LocalNow);
            var updUserId = Convert.ToString(this.PerRequestDataContainer.OpenId);

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
                    MERGE INTO VENDOR_REPOSITORY_GROUP /*with (xlock, rowlock)*/ target
                        USING
                        (
                            SELECT
                                /*ds vendor_id*/'1' AS vendor_id
                                ,/*ds repository_group_id*/'1' AS repository_group_id
                            FROM DUAL
                        ) source
                        ON
                            (target.repository_group_id = source.repository_group_id AND target.vendor_id = source.vendor_id)
                        WHEN MATCHED THEN
                            UPDATE
                            SET
                                is_active = /*ds is_active*/1 
                                ,upd_date = /*ds upd_date*/systimestamp 
                                ,upd_username = /*ds upd_username*/'1' 
                        WHEN NOT MATCHED THEN
                            INSERT
                            (
                                vendor_repositorygroup_id
                                ,vendor_id
                                ,repository_group_id
                                ,reg_date
                                ,reg_username
                                ,upd_date
                                ,upd_username
                                ,is_active
                            )
                            VALUES
                            (
                                /*ds vendor_repositorygroup_id*/'1' 
                                ,/*ds vendor_id*/'1' 
                                ,/*ds repository_group_id*/'1' 
                                ,/*ds reg_date*/systimestamp 
                                ,/*ds reg_username*/'1' 
                                ,/*ds upd_date*/systimestamp 
                                ,/*ds upd_username*/'1' 
                                ,/*ds is_active*/1 
                            )
                        ";
            }
            else
            {
                sql = @"
                    MERGE INTO VendorRepositoryGroup with (xlock, rowlock) AS target
                        USING
                        (
                            SELECT
                                @vendor_id AS vendor_id
                                ,@repository_group_id AS repository_group_id
                        ) AS source
                        ON
                            target.repository_group_id = source.repository_group_id AND target.vendor_id = source.vendor_id
                        WHEN MATCHED THEN
                            UPDATE
                            SET
                                is_active = @is_active
                                ,upd_date = @upd_date
                                ,upd_username = @upd_username
                        WHEN NOT MATCHED THEN
                            INSERT
                            (
                                vendor_repositorygroup_id
                                ,vendor_id
                                ,repository_group_id
                                ,reg_date
                                ,reg_username
                                ,upd_date
                                ,upd_username
                                ,is_active
                            )
                            VALUES
                            (
                                @vendor_repositorygroup_id
                                ,@vendor_id
                                ,@repository_group_id
                                ,@reg_date
                                ,@reg_username
                                ,@upd_date
                                ,@upd_username
                                ,@is_active
                            );
                        ";
            }

            var param = new
            {
                vendor_repositorygroup_id = Guid.NewGuid().ToString(),
                vendor_id = vendorId,
                repository_group_id = repositoryGroupId,
                reg_date = now,
                reg_username = updUserId,
                upd_date = now,
                upd_username = updUserId,
                is_active = isActive
            };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            Connection.Execute(twowaySql.Sql, dynParams);
        }

        /// <summary>
        /// デフォルトのベンダーリポジトリ作成
        /// </summary>
        /// <param name="vendorId"></param>
        public void RegistDefaultVendorRepositoryGroup(string vendorId)
        {

            // is_defaultがTrueのReposigoryGroupIdを取得する

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT repository_group_id FROM REPOSITORY_GROUP WHERE is_active = 1 AND is_default = 1
";
            }
            else
            {
                sql = @"
SELECT repository_group_id FROM RepositoryGroup WHERE is_active = 1 AND is_default = 1
";
            }

            var idList = this.Connection.Query<DB_RepositoryGroup>(sql);

            if (!idList.Any())
            {
                return;
            }

            idList.ToList().ForEach(x =>
                this.ExecuteUpsertVendorRepositoryGroup(vendorId, x.repository_group_id.ToString(), true)
            );
        }
        public bool ExistsRepositoryGroup(string repositoryGroupId)
        {
            var db_model = Connection.Get<DB_RepositoryGroup>(repositoryGroupId);
            if (db_model == null || !db_model.is_active)
            {
                return false;
            }
            return true;
        }
    }
}
