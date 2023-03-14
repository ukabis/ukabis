using AutoMapper;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Infrastructure.Database.Authority;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using StackExchange.Redis;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class RoleRepository : AbstractRepository, IRoleRepository
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DB_Role, RoleModel>()
                    .ForMember(dst => dst.RoleId, ops => ops.MapFrom(src => src.role_id))
                    .ForMember(dst => dst.RoleName, ops => ops.MapFrom(src => src.role_name));
            });
            return mappingConfig.CreateMapper();
        });


        private static IMapper _mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Authority"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }

        public bool IsExstis(Guid roleId)
            => IsExstis(roleId.ToString());

        public bool IsExstis(string roleId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql = "SELECT role_id FROM Role WHERE role_id= /*ds role_id*/'1' AND is_active=1";
            var param = new { role_id = roleId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<string>(twowaySql.Sql, dynParams).FirstOrDefault() != null ? true : false;
        }

        public RoleModel GetRole(Guid roleId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
role_id AS RoleId,
role_name AS RoleName 
FROM
ROLE
WHERE
role_id = /*ds role_id*/'00000000-0000-0000-0000-000000000000' 
AND is_active=1";
            }
            else
            {
                sql = @"
SELECT
role_id AS RoleId,
role_name AS RoleName 
FROM
Role
WHERE
role_id=@role_id
AND is_active=1";
            }

            var param = new { role_id = roleId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            var result = Connection.QuerySingleOrDefault<RoleModel>(twowaySql.Sql, dynParams);

            if (result == null)
            {
                throw new NotFoundException($"Not Found Role id={roleId}");
            }

            return result;
        }

        [CacheEntity(AuthorityDatabase.TABLE_ROLE)]
        [Cache]
        public IList<RoleModel> GetRoleList()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"SELECT role_id AS RoleId, role_name AS RoleName FROM ROLE WHERE is_active=1";
            }
            else
            {
                sql = @"SELECT role_id AS RoleId, role_name AS RoleName FROM Role WHERE is_active=1";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            var result = Connection.Query<RoleModel>(twowaySql.Sql).ToList();
            return result;
        }

        public IList<RoleDetailModel> GetRoleDetail()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {

                sql = @"
SELECT
  r.role_id AS RoleId
  ,r.role_name AS RoleName
  ,af.admin_name AS FuncName
  ,afr.is_read AS IsRead
  ,afr.is_write AS IsWrite
FROM
    ROLE r
    INNER JOIN ADMIN_FUNC_ROLE afr ON r.role_id=afr.role_id AND afr.is_active=1
    INNER JOIN ADMIN_FUNC af ON afr.admin_func_id=af.admin_func_id AND af.is_active=1
WHERE
    r.is_active=1
ORDER BY
    r.role_id
    ,r.role_name
    ,af.admin_name
";
            }
            else
            {
                sql = @"
SELECT
  r.role_id AS RoleId
  ,r.role_name AS RoleName
  ,af.admin_name AS FuncName
  ,afr.is_read AS IsRead
  ,afr.is_write AS IsWrite
FROM
    Role AS r
    INNER JOIN AdminFuncRole afr ON r.role_id=afr.role_id AND afr.is_active=1
    INNER JOIN AdminFunc af ON afr.admin_func_id=af.admin_func_id AND af.is_active=1
WHERE
    r.is_active=1
ORDER BY
    r.role_id
    ,r.role_name
    ,af.admin_name
";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            var result = Connection.Query<RoleDetailModel>(twowaySql.Sql).ToList();
            return result;
        }

        public IList<RoleDetailModel> GetRoleDetailEx(string openId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    r.role_id AS RoleId
    ,r.role_name AS RoleName
    ,af.admin_name AS FuncName
    ,afr.is_read AS IsRead
    ,afr.is_write AS IsWrite
FROM
    ROLE r
    INNER JOIN STAFF_ROLE sf ON r.role_id=sf.role_id AND sf.is_active=1
    INNER JOIN STAFF s ON sf.staff_id=s.staff_id AND s.account=/*ds account*/'a' AND s.is_active=1
    INNER JOIN VENDOR v ON s.vendor_id=v.vendor_id AND v.is_active=1
    INNER JOIN ADMIN_FUNC_ROLE afr ON r.role_id=afr.role_id AND afr.is_active=1
    INNER JOIN ADMIN_FUNC af ON afr.admin_func_id=af.admin_func_id AND af.is_active=1
WHERE
    r.is_active=1
ORDER BY
    r.role_id
    ,r.role_name
    ,af.admin_name
";
            }
            else
            {
                sql = @"
SELECT
    r.role_id AS RoleId
    ,r.role_name AS RoleName
    ,af.admin_name AS FuncName
    ,afr.is_read AS IsRead
    ,afr.is_write AS IsWrite
FROM
    Role AS r
    INNER JOIN StaffRole sf ON r.role_id=sf.role_id AND sf.is_active=1
    INNER JOIN Staff s ON sf.staff_id=s.staff_id AND s.account=@account AND s.is_active=1
    INNER JOIN Vendor v ON s.vendor_id=v.vendor_id AND v.is_active=1
    INNER JOIN AdminFuncRole afr ON r.role_id=afr.role_id AND afr.is_active=1
    INNER JOIN AdminFunc af ON afr.admin_func_id=af.admin_func_id AND af.is_active=1
WHERE
    r.is_active=1
ORDER BY
    r.role_id
    ,r.role_name
    ,af.admin_name
";
            }
            var param = new { account = openId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<RoleDetailModel>(twowaySql.Sql, dynParams).ToList();
        }

        [CacheIdFire("roleId", "role.RoleId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_ROLE)]
        [DomainDataSync(AuthorityDatabase.TABLE_ROLE, "RoleId")]
        public RoleModel RegistrationRole(RoleModel role)
        {
            if (!ValidateRoleName(role))
            {
                throw new AlreadyExistsException("指定された権限名は既に使用されています。");
            }

            var now = PerRequestDataContainer.GetDateTimeUtil().GetUtc(PerRequestDataContainer.GetDateTimeUtil().LocalNow);
            var updUserId = Convert.ToString(PerRequestDataContainer.OpenId);

            DB_Role regRole = new DB_Role
            {
                role_id = role.RoleId,
                role_name = role.RoleName,
                reg_date = now,
                reg_username = updUserId,
                upd_date = now,
                upd_username = updUserId,
                is_active = true
            };
            Connection.Insert(regRole);
            return role;
        }

        [CacheIdFire("roleId", "role.RoleId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_ROLE)]
        [DomainDataSync(AuthorityDatabase.TABLE_ROLE, "RoleId")]
        public void UpdateRole(RoleModel role)
        {
            if (!ValidateRoleName(role))
            {
                throw new AlreadyExistsException("指定された権限名は既に使用されています。");
            }

            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {

                sql = @"
UPDATE
    ROLE
SET
    role_name = /*ds role_name*/'aaa' 
    ,upd_date = SYSTIMESTAMP
    ,upd_username = /*ds upd_username*/'a' 
WHERE
    role_id = /*ds role_id*/'00000000-0000-0000-0000-000000000000' 
";
            }
            else
            {
                sql = @"
UPDATE
    Role
SET
    role_name = @role_name
    ,upd_date = GETDATE()
    ,upd_username = @upd_username
WHERE
    role_id = @role_id
";
            }
            var param = new
            {
                role_name = role.RoleName,
                upd_username = PerRequestDataContainer.OpenId,
                role_id = role.RoleId
            };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            if (Connection.Execute(twowaySql.Sql, dynParams) <= 0)
            {
                throw new NotFoundException($"Not Found Admin Func Role id={role.RoleId}");
            }
        }

        [CacheIdFire("roleId", "roleId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_ROLE)]
        [DomainDataSync(AuthorityDatabase.TABLE_ROLE, "roleId")]
        public void DeleteRole(Guid roleId)
        {
            if (!ValidateInUseRole(roleId))
            {
                throw new InUseException();
            }
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {

                sql = @"
UPDATE 
    ROLE
SET 
    is_active = 0
    ,upd_date = SYSTIMESTAMP
    ,upd_username = /*ds upd_username*/'aaa' 
WHERE
    role_id = /*ds role_id*/'00000000-0000-0000-0000-000000000000' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    Role
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@upd_username
WHERE
    role_id = @role_id
    AND is_active=1
";
            }
            var param = new { upd_username = PerRequestDataContainer.OpenId, role_id = roleId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            if (Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams) <= 0)
            {
                throw new NotFoundException($"Not Found admin_func_role_id={roleId}");
            }
        }

        public IList<AdminFuncInfomationModel> GetAdminFuncInfomationList()
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
                            SELECT
	                            af.admin_func_id AS AdminFuncId, 
	                            af.admin_name AS AdminName, 
                                af.display_description AS DisplayDescription, 
	                            afr.admin_func_role_id AS AdminFuncRoleId, 
	                            afr.role_id AS RoleId, 
	                            afr.is_read AS IsRead, 
	                            afr.is_write AS IsWrite 
                            FROM
	                            ADMIN_FUNC af 
                            LEFT OUTER JOIN
	                            ADMIN_FUNC_ROLE afr ON af.admin_func_id = afr.admin_func_id AND afr.is_active = 1 
                            WHERE
	                            af.is_active = 1 
                            ORDER BY
	                            af.admin_name
                        ";
            }
            else
            {
                sql = @"
                            SELECT
	                            af.admin_func_id AS AdminFuncId,
	                            af.admin_name AS AdminName,
                                af.display_description AS DisplayDescription,
	                            afr.admin_func_role_id AS AdminFuncRoleId,
	                            afr.role_id AS RoleId,
	                            afr.is_read AS IsRead,
	                            afr.is_write AS IsWrite
                            FROM
	                            AdminFunc af
                            LEFT OUTER JOIN
	                            AdminFuncRole afr ON af.admin_func_id = afr.admin_func_id AND afr.is_active = 1
                            WHERE
	                            af.is_active = 1
                            ORDER BY
	                            af.admin_name
                        ";
            }
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, null);
            var result = Connection.Query<AdminFuncInfomationModel>(twowaySql.Sql).ToList();
            return result;
        }


        #region Validate

        /// <summary>
        /// 登録する権限名が使用されているか
        /// </summary>
        private bool ValidateRoleName(RoleModel role)
        {
            var list = GetRoleList();

            // 新規作成時用
            if (role.RoleId == Guid.Empty)
            {
                role.RoleId = Guid.NewGuid();
            }

            if (list.Where(x => x.RoleName == role.RoleName && x.RoleId != role.RoleId).Count() > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 権限がStaffRoleで使用されているか
        /// </summary>
        private bool ValidateInUseRole(Guid roleId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(r.staff_role_id)
FROM
    STAFF_ROLE r
INNER JOIN STAFF s ON s.staff_id = r.staff_id AND s.is_active = 1
INNER JOIN VENDOR v ON v.vendor_id = s.vendor_id AND v.is_active = 1
WHERE
    r.role_id = /*ds role_id*/'00000000-0000-0000-0000-000000000000' 
    AND r.is_active = 1
    ";
            }
            else
            {
                sql = @"
SELECT
    COUNT(r.staff_role_id)
FROM
    StaffRole r
INNER JOIN Staff s ON s.staff_id = r.staff_id AND s.is_active = 1
INNER JOIN Vendor v ON v.vendor_id = s.vendor_id AND v.is_active = 1
WHERE
    r.role_id = @role_id
    AND r.is_active = 1
    ";
            }
            var param = new { role_id = roleId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            if (Connection.QuerySingle<int>(twowaySql.Sql, dynParams) > 0)
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
