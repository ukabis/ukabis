using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using System.Data.SqlClient;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class ApiMailTemplateRepository : AbstractRepository, IApiMailTemplateRepository
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ApiMailTemplateModel, DB_ControllerMailTemplate>()
                    .ForMember(d => d.controller_mail_template_id, o => o.MapFrom(s => s.ApiMailTemplateId))
                    .ForMember(d => d.controller_id, o => o.MapFrom(s => s.ApiId))
                    .ForMember(d => d.vendor_id, o => o.MapFrom(s => s.VendorId))
                    .ForMember(d => d.vendor_mail_template_id, o => o.MapFrom(s => s.MailTemplateId))
                    .ForMember(d => d.notify_register, o => o.MapFrom(s => s.NotifyRegister))
                    .ForMember(d => d.notify_update, o => o.MapFrom(s => s.NotifyUpdate))
                    .ForMember(d => d.notify_delete, o => o.MapFrom(s => s.NotifyDelete))
                    .ForMember(d => d.is_active, o => o.MapFrom(s => s.IsActive))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
            });
            return mappingConfig.CreateMapper();
        });

        private static IMapper Mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }

        public IList<ApiMailTemplateModel> GetList(string apiId, string vendorId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    cmt.controller_mail_template_id AS ApiMailTemplateId
    ,cmt.controller_id AS ApiId
    ,cmt.vendor_id AS VendorId
    ,cmt.vendor_mail_template_id AS MailTemplateId
    ,cmt.notify_register AS NotifyRegister
    ,cmt.notify_update AS NotifyUpdate
    ,cmt.notify_delete AS NotifyDelete 
FROM CONTROLLER_MAIL_TEMPLATE cmt 
JOIN VENDOR v ON v.vendor_id = cmt.vendor_id AND v.is_enable = 1 AND v.is_active = 1 
JOIN VENDOR_MAIL_TEMPLATE vmt ON vmt.vendor_mail_template_id = cmt.vendor_mail_template_id AND vmt.is_active = 1 
WHERE
    cmt.controller_id = /*ds apiId*/'00000000-0000-0000-0000-000000000000' AND 
/*ds if vendorId != null*/
    cmt.vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' AND 
/*ds end if*/
    cmt.is_active = 1 
ORDER BY v.vendor_name, vmt.mail_template_name
";
            }
            else
            {
                sql = @"
SELECT 
    cmt.controller_mail_template_id AS ApiMailTemplateId
    ,cmt.controller_id AS ApiId
    ,cmt.vendor_id AS VendorId
    ,cmt.vendor_mail_template_id AS MailTemplateId
    ,cmt.notify_register AS NotifyRegister
    ,cmt.notify_update AS NotifyUpdate
    ,cmt.notify_delete AS NotifyDelete
FROM ControllerMailTemplate cmt
JOIN Vendor v ON v.vendor_id = cmt.vendor_id AND v.is_enable = 1 AND v.is_active = 1
JOIN VendorMailTemplate vmt ON vmt.vendor_mail_template_id = cmt.vendor_mail_template_id AND vmt.is_active = 1
WHERE
    cmt.controller_id = @apiId AND
/*ds if vendorId != null*/
    cmt.vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
    cmt.is_active = 1
ORDER BY v.vendor_name, vmt.mail_template_name
";
            }

            object param = new { apiId = apiId, vendorId = vendorId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<ApiMailTemplateModel>(twowaySql.Sql, dynParams).ToList();
        }

        public ApiMailTemplateModel Get(string apiMailTemplateId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    cmt.controller_mail_template_id AS ApiMailTemplateId
    ,cmt.controller_id AS ApiId
    ,cmt.vendor_id AS VendorId
    ,cmt.vendor_mail_template_id AS MailTemplateId
    ,cmt.notify_register AS NotifyRegister
    ,cmt.notify_update AS NotifyUpdate
    ,cmt.notify_delete AS NotifyDelete 
FROM CONTROLLER_MAIL_TEMPLATE cmt 
JOIN VENDOR v ON v.vendor_id = cmt.vendor_id AND v.is_enable = 1 AND v.is_active = 1 
JOIN VENDOR_MAIL_TEMPLATE vmt ON vmt.vendor_mail_template_id = cmt.vendor_mail_template_id AND vmt.is_active = 1 
WHERE
    cmt.controller_mail_template_id = /*ds apiMailTemplateId*/'00000000-0000-0000-0000-000000000000' 
    AND cmt.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT 
    cmt.controller_mail_template_id AS ApiMailTemplateId
    ,cmt.controller_id AS ApiId
    ,cmt.vendor_id AS VendorId
    ,cmt.vendor_mail_template_id AS MailTemplateId
    ,cmt.notify_register AS NotifyRegister
    ,cmt.notify_update AS NotifyUpdate
    ,cmt.notify_delete AS NotifyDelete
FROM ControllerMailTemplate cmt
JOIN Vendor v ON v.vendor_id = cmt.vendor_id AND v.is_enable = 1 AND v.is_active = 1
JOIN VendorMailTemplate vmt ON vmt.vendor_mail_template_id = cmt.vendor_mail_template_id AND vmt.is_active = 1
WHERE
    cmt.controller_mail_template_id = @apiMailTemplateId
    AND cmt.is_active = 1
";
            }
            var param = new { apiMailTemplateId = apiMailTemplateId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingle<ApiMailTemplateModel>(twowaySql.Sql, dynParams);
        }

        public void Register(ApiMailTemplateModel model)
        {
            if (string.IsNullOrEmpty(model.ApiMailTemplateId))
            {
                model.ApiMailTemplateId = Guid.NewGuid().ToString();
            }
            try
            {
                // 登録実行
                Connection.Insert(Mapper.Map<DB_ControllerMailTemplate>(model));
            }
            catch (SqlException ex)
            {
                // UNIQUE制約違反の場合、独自例外を返す
                if (ex.Number == 2627) throw new AlreadyExistsException(ex.Message);
                // 外部キー制約違反の場合、独自例外を返す
                if (ex.Number == 547) throw new ForeignKeyException(ex.Message);
                throw new SqlDatabaseException(ex.Message);
            }
        }

        public ApiMailTemplateModel Update(ApiMailTemplateModel model)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    CONTROLLER_MAIL_TEMPLATE
SET
    controller_id = /*ds apiId*/'00000000-0000-0000-0000-000000000000' 
    ,vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' 
    ,vendor_mail_template_id = /*ds mailTemplateId*/'00000000-0000-0000-0000-000000000000' 
    ,notify_register = /*ds notifyRegister*/1 
    ,notify_update = /*ds notifyUpdate*/1 
    ,notify_delete = /*ds notifyDelete*/1 
    ,upd_date = SYSTIMESTAMP
    ,upd_username = /*ds openId*/'00000000-0000-0000-0000-000000000000' 
WHERE
    controller_mail_template_id = /*ds apiMailTemplateId*/'00000000-0000-0000-0000-000000000000' 
    AND is_active = 1
";
            }
            else
            {
                sql = @"
UPDATE
    ControllerMailTemplate
SET
    controller_id = @apiId
    ,vendor_id = @vendorId
    ,vendor_mail_template_id = @mailTemplateId
    ,notify_register = @notifyRegister
    ,notify_update = @notifyUpdate
    ,notify_delete = @notifyDelete
    ,upd_date = GETDATE()
    ,upd_username = @openId
WHERE
    controller_mail_template_id = @apiMailTemplateId
    AND is_active = 1
";
            }
            var param = new
            {
                apiId = model.ApiId.To<Guid>(),
                vendorId = model.VendorId.To<Guid>(),
                mailTemplateId = model.MailTemplateId.To<Guid>(),
                notifyRegister = model.NotifyRegister,
                notifyUpdate = model.NotifyUpdate,
                notifyDelete = model.NotifyDelete,
                openId = PerRequestDataContainer.OpenId,
                apiMailTemplateId = model.ApiMailTemplateId
            };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            try
            {
                Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            }
            catch (SqlException ex)
            {
                // UNIQUE制約違反の場合、独自例外を返す
                if (ex.Number == 2627) throw new AlreadyExistsException(ex.Message);
                // 外部キー制約違反の場合、独自例外を返す
                if (ex.Number == 547) throw new ForeignKeyException(ex.Message);
                throw new SqlDatabaseException(ex.Message);
            }
            return model;
        }

        public void Delete(string apiMailTemplateId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    CONTROLLER_MAIL_TEMPLATE
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username = /*ds openId*/'00000000-0000-0000-0000-000000000000' 
WHERE
    controller_mail_template_id = /*ds apiMailTemplateId*/'00000000-0000-0000-0000-000000000000' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    ControllerMailTemplate
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username = @openId
WHERE
    controller_mail_template_id = @apiMailTemplateId
    AND is_active=1
";
            }
            var param = new { apiMailTemplateId = apiMailTemplateId, openId = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        public bool IsExistsApiMailTemplate(string apiId, string vendorId, string mailTemplateId, string? excludeApiMailTemplateId = null)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(*) 
FROM CONTROLLER_MAIL_TEMPLATE cmt 
WHERE
    cmt.controller_id = /*ds apiId*/'00000000-0000-0000-0000-000000000000' AND 
    cmt.vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' AND 
    cmt.vendor_mail_template_id = /*ds mailTemplateId*/'00000000-0000-0000-0000-000000000000' AND 
/*ds if excludeApiMailTemplateId != null*/
    cmt.controller_mail_template_id != /*ds excludeApiMailTemplateId*/'00000000-0000-0000-0000-000000000000' AND 
/*ds end if*/
    cmt.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(*)
FROM ControllerMailTemplate cmt
WHERE
    cmt.controller_id = @apiId AND
    cmt.vendor_id = @vendorId AND
    cmt.vendor_mail_template_id = @mailTemplateId AND
/*ds if excludeApiMailTemplateId != null*/
    cmt.controller_mail_template_id != @excludeApiMailTemplateId AND
/*ds end if*/
    cmt.is_active = 1
";
            }
            var param = new
            {
                apiId = apiId,
                vendorId = vendorId,
                mailTemplateId = mailTemplateId,
                excludeApiMailTemplateId = excludeApiMailTemplateId
            };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<int>(twowaySql.Sql, dynParams).ToList().FirstOrDefault() >= 1 ? true : false;
        }

        public bool CheckUsedMailTemplate(string mailTemplateId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(vendor_mail_template_id) 
FROM
    CONTROLLER_MAIL_TEMPLATE 
WHERE
    vendor_mail_template_id = /*ds mailTemplateId*/'00000000-0000-0000-0000-000000000000' 
    AND is_active = 1";
            }
            else
            {
                sql = @"
SELECT
    COUNT(vendor_mail_template_id)
FROM
    ControllerMailTemplate
WHERE
    vendor_mail_template_id = @mailTemplateId
    AND is_active = 1";
            }
            var param = new { mailTemplateId = mailTemplateId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.QuerySingle<int>(twowaySql.Sql, dynParams) == 0 ? true : false;
        }
    }
}