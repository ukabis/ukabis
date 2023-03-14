using AutoMapper;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.Authority;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using System.Data.SqlClient;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class AdminInfoRepository : AbstractRepository, IAdminInfoRepository
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AdminFuncInfomationModel, DB_AdminFuncRole>()
                    .ForMember(dst => dst.admin_func_role_id, ops => ops.MapFrom(src => src.AdminFuncRoleId))
                    .ForMember(dst => dst.admin_func_id, ops => ops.MapFrom(src => src.AdminFuncId))
                    .ForMember(dst => dst.role_id, ops => ops.MapFrom(src => src.RoleId))
                    .ForMember(dst => dst.is_read, ops => ops.MapFrom(src => src.IsRead))
                    .ForMember(dst => dst.is_write, ops => ops.MapFrom(src => src.IsWrite))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper _mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Authority"));
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }

        [CacheIdFire("adminFuncRoleId", "funcInfo.AdminFuncRoleId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_ROLE)]
        public AdminFuncInfomationModel RegistrationAdminFuncInfomation(AdminFuncInfomationModel adminFunc)
        {
            if (string.IsNullOrEmpty(adminFunc.AdminFuncRoleId))
            {
                adminFunc.AdminFuncRoleId = Guid.NewGuid().ToString();
            }

            try
            {
                adminFunc.AdminFuncRoleId = _connection.Insert(_mapper.Map<DB_AdminFuncRole>(adminFunc)).ToString();
                return adminFunc;
            }
            catch (SqlException e)
            {
                throw e.Number == 547 ? new ForeignKeyException(e.Message, e) : new SqlDatabaseException(e.Message, e);
            }
        }

        [CacheArg("adminFuncRoleId")]
        [CacheEntity(AuthorityDatabase.TABLE_ADMINFUNCROLE)]
        [Cache]
        public AdminFuncInfomationModel GetAdminInfo(string adminFuncRoleId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
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
    INNER JOIN ADMIN_FUNC_ROLE afr ON af.admin_func_id = afr.admin_func_id AND afr.admin_func_role_id = /*ds admin_func_role_id*/'1' AND afr.is_active = 1
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
    INNER JOIN AdminFuncRole afr ON af.admin_func_id = afr.admin_func_id AND afr.admin_func_role_id = @admin_func_role_id AND afr.is_active = 1
WHERE
    af.is_active = 1
ORDER BY
	af.admin_name
";
            }
            var param = new { admin_func_role_id = adminFuncRoleId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = _connection.QuerySingleOrDefault<AdminFuncInfomationModel>(twowaySql.Sql, dynParams);

            if (result == null)
            {
                throw new NotFoundException($"Not Found AdminFuncRoleId={adminFuncRoleId}");
            }
            return result;

        }

        [CacheIdFire("adminFuncRoleId", "funcInfo.AdminFuncRoleId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_ADMINFUNCROLE)]
        public void UpdateAdminFuncInfo(AdminFuncInfomationModel adminFunc)
        {
            if (string.IsNullOrEmpty(adminFunc.AdminFuncRoleId))
            {
                adminFunc.AdminFuncRoleId = Guid.NewGuid().ToString();
            }
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
                           UPDATE
                               ADMIN_FUNC_ROLE
                           SET
                               is_read = /*ds is_read*/1 
                               ,is_write = /*ds is_write*/1 
                               ,upd_date = /*ds upd_date*/systimestamp 
                               ,upd_username = /*ds upd_username*/'1' 
                               ,is_active = 1
                           WHERE
                               admin_func_role_id = /*ds admin_func_role_id*/'1' 
                          ";
            }
            else
            {
                sql = @"
                           UPDATE
                               AdminFuncRole
                           SET
                               is_read = @is_read
                               ,is_write = @is_write
                               ,upd_date = @upd_date
                               ,upd_username = @upd_username
                               ,is_active = 1
                           WHERE
                               admin_func_role_id = @admin_func_role_id
                          ";
            }
            var param = new
            {
                admin_func_role_id = adminFunc.AdminFuncRoleId,
                is_read = adminFunc.IsRead,
                is_write = adminFunc.IsWrite,
                upd_date = this.PerRequestDataContainer.GetDateTimeUtil().GetUtc(this.PerRequestDataContainer.GetDateTimeUtil().LocalNow),
                upd_username = Convert.ToString(this.PerRequestDataContainer.OpenId),
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            if (_connection.Execute(twowaySql.Sql, dynParams) <= 0)
            {
                throw new NotFoundException($"Not Found Admin Func Role id={adminFunc.AdminFuncRoleId}");
            }
        }

        [CacheIdFire("adminFuncRoleId", "adminFuncRoleId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_ADMINFUNCROLE)]
        public void DeleteAdminInfo(string adminFuncRoleId)
        {
            var model = new DB_AdminFuncRole() { admin_func_role_id = Guid.Parse(adminFuncRoleId) };
            model.UpdateFiveColumn();
            model.is_active = false;
            if (_connection.LogicalDelete(_mapper.Map<DB_AdminFuncRole>(model)) <= 0)
            {
                throw new NotFoundException($"Not Found admin_func_role_id={adminFuncRoleId}");
            }
        }
    }
}
