using AutoMapper;
using JP.DataHub.Api.Core.Exceptions;
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
    internal class MailTemplateRepository : AbstractRepository, IMailTemplateRepository
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MailTemplateModel, DB_VendorMailTemplate>()
                    .ForMember(d => d.vendor_mail_template_id, o => o.MapFrom(s => s.MailTemplateId))
                    .ForMember(d => d.mail_template_name, o => o.MapFrom(s => s.MailTemplateName))
                    .ForMember(d => d.vendor_id, o => o.MapFrom(s => s.VendorId))
                    .ForMember(d => d.from_mailaddress, o => o.MapFrom(s => s.FromMailAddress))
                    .ForMember(d => d.to_mailaddress, o => o.MapFrom(s => string.Join(",", s.ToMailAddress.Select(a => a))))
                    .ForMember(d => d.cc_mailaddress, o => o.MapFrom(s => string.Join(",", s.CcMailAddress.Select(a => a))))
                    .ForMember(d => d.bcc_mailaddress, o => o.MapFrom(s => string.Join(",", s.BccMailAddress.Select(a => a))))
                    .ForMember(d => d.title, o => o.MapFrom(s => s.Subject))
                    .ForMember(d => d.body, o => o.MapFrom(s => s.Body))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
                cfg.CreateMap<DB_VendorMailTemplate, MailTemplateModel>()
                    .ForMember(d => d.MailTemplateId, o => o.MapFrom(s => s.vendor_mail_template_id))
                    .ForMember(d => d.VendorId, o => o.MapFrom(s => s.vendor_id))
                    .ForMember(d => d.MailTemplateName, o => o.MapFrom(s => s.mail_template_name))
                    .ForMember(d => d.FromMailAddress, o => o.MapFrom(s => s.from_mailaddress))
                    .ForMember(d => d.ToMailAddress, o => o.MapFrom(s => s.to_mailaddress.Split(',', StringSplitOptions.None).ToArray()))
                    .ForMember(d => d.CcMailAddress, o => o.MapFrom(s => s.cc_mailaddress.Split(',', StringSplitOptions.None).ToArray()))
                    .ForMember(d => d.BccMailAddress, o => o.MapFrom(s => s.bcc_mailaddress.Split(',', StringSplitOptions.None).ToArray()))
                    .ForMember(d => d.Subject, o => o.MapFrom(s => s.title))
                    .ForMember(d => d.Body, o => o.MapFrom(s => s.body));
            });
            return mappingConfig.CreateMapper();
        });

        private static IMapper Mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection Connection { get => _lazyConnection.Value; }

        public IList<MailTemplateModel> GetList(string vendorId)
        {

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    vmt.vendor_mail_template_id
    ,vmt.vendor_id
    ,vmt.mail_template_name
    ,vmt.from_mailaddress
    ,vmt.to_mailaddress
    ,vmt.cc_mailaddress
    ,vmt.bcc_mailaddress
    ,vmt.title
    ,vmt.body
FROM VENDOR_MAIL_TEMPLATE vmt
JOIN VENDOR v ON v.vendor_id = vmt.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
/*ds if vendorId != null*/
    vmt.vendor_id = /*ds vendorId*/'id' AND
/*ds end if*/
    vmt.is_active = 1
ORDER BY vmt.mail_template_name
";
            }
            else
            {
                sql = @"
SELECT 
    vmt.vendor_mail_template_id
    ,vmt.vendor_id
    ,vmt.mail_template_name
    ,vmt.from_mailaddress
    ,vmt.to_mailaddress
    ,vmt.cc_mailaddress
    ,vmt.bcc_mailaddress
    ,vmt.title
    ,vmt.body
FROM VendorMailTemplate vmt
JOIN Vendor v ON v.vendor_id = vmt.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
/*ds if vendorId != null*/
    vmt.vendor_id = @vendorId AND
/*ds end if*/
    vmt.is_active = 1
ORDER BY vmt.mail_template_name
";
            }

            object param = new { vendorId = vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Mapper.Map<List<MailTemplateModel>>(Connection.Query<DB_VendorMailTemplate>(twowaySql.Sql, dynParams).ToList());
        }

        public bool IsExistsVendorMailTemplate(string vendorId, string mailTemplateId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    VENDOR_MAIL_TEMPLATE vmt
WHERE
    vmt.vendor_id = /*ds vendorId*/'id' 
    AND vmt.vendor_mail_template_id = /*ds mailTemplateId*/'id' 
    AND vmt.is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    VendorMailTemplate vmt
WHERE
    vmt.vendor_id = @vendorId
    AND vmt.vendor_mail_template_id = @mailTemplateId
    AND vmt.is_active=1
";
            }

            var param = new { vendorId = vendorId, mailTemplateId = mailTemplateId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<int>(twowaySql.Sql, dynParams).FirstOrDefault() == 0 ? false : true;
        }

        public MailTemplateModel Get(string mailTemplateId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string sql;
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    vmt.vendor_mail_template_id
    ,vmt.vendor_id
    ,vmt.mail_template_name
    ,vmt.from_mailaddress
    ,vmt.to_mailaddress
    ,vmt.cc_mailaddress
    ,vmt.bcc_mailaddress
    ,vmt.title
    ,vmt.body
FROM VENDOR_MAIL_TEMPLATE vmt
JOIN VENDOR v ON v.vendor_id = vmt.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
    vmt.vendor_mail_template_id = /*ds mailTemplateId*/'id' AND
    vmt.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT 
    vmt.vendor_mail_template_id
    ,vmt.vendor_id
    ,vmt.mail_template_name
    ,vmt.from_mailaddress
    ,vmt.to_mailaddress
    ,vmt.cc_mailaddress
    ,vmt.bcc_mailaddress
    ,vmt.title
    ,vmt.body
FROM VendorMailTemplate vmt
JOIN Vendor v ON v.vendor_id = vmt.vendor_id AND v.is_enable = 1 AND v.is_active = 1
WHERE
    vmt.vendor_mail_template_id = @mailTemplateId AND
    vmt.is_active = 1
";
            }

            var param = new { mailTemplateId = mailTemplateId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return Mapper.Map<MailTemplateModel>(Connection.QuerySingle<DB_VendorMailTemplate>(twowaySql.Sql, dynParams));
        }

        public void Register(MailTemplateModel model)
        {
            try
            {
                Connection.Insert(Mapper.Map<DB_VendorMailTemplate>(model));
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

        public MailTemplateModel Update(MailTemplateModel model)
        {
            var updateModel = Mapper.Map<DB_VendorMailTemplate>(model);
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    VENDOR_MAIL_TEMPLATE 
SET 
    mail_template_name = /*ds mailTemplateName*/'aaa' 
    ,vendor_id = /*ds vendorId*/'00000000-0000-0000-0000-000000000000' 
    ,from_mailaddress = /*ds fromMailAddress*/'aaa' 
    ,to_mailaddress = /*ds toMailAddress*/'aaa' 
    ,cc_mailaddress = /*ds ccMailAddress*/'aaa' 
    ,bcc_mailaddress = /*ds bccMailAddress*/'aaa' 
    ,title = /*ds subject*/'aaa' 
    ,body = /*ds body*/'aaa' 
    ,upd_date = SYSTIMESTAMP 
    ,upd_username = /*ds openId*/'00000000-0000-0000-0000-000000000000' 
WHERE 
    vendor_mail_template_id = /*ds mailTemplateId*/'00000000-0000-0000-0000-000000000000' 
    AND is_active = 1 
";
            }
            else
            {
                sql = @"
UPDATE
    VendorMailTemplate
SET
    mail_template_name = @mailTemplateName
    ,vendor_id = @vendorId
    ,from_mailaddress = @fromMailAddress
    ,to_mailaddress = @toMailAddress
    ,cc_mailaddress = @ccMailAddress
    ,bcc_mailaddress = @bccMailAddress
    ,title = @subject
    ,body = @body
    ,upd_date = GETDATE()
    ,upd_username = @openId
WHERE
    vendor_mail_template_id = @mailTemplateId
    AND is_active = 1
";
            }
            try
            {
                var param = new
                {
                    mailTemplateName = updateModel.mail_template_name,
                    vendorId = updateModel.vendor_id,
                    fromMailAddress = updateModel.from_mailaddress,
                    toMailAddress = updateModel.to_mailaddress,
                    ccMailAddress = updateModel.cc_mailaddress,
                    bccMailAddress = updateModel.bcc_mailaddress,
                    subject = updateModel.title,
                    body = updateModel.body,
                    openId = PerRequestDataContainer.OpenId,
                    mailTemplateId = updateModel.vendor_mail_template_id
                };
                var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
                var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
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
        public void Delete(string mailTemplateId)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    VENDOR_MAIL_TEMPLATE
SET 
    is_active=0 
    ,upd_date=SYSTIMESTAMP 
    ,upd_username = /*ds openId*/'00000000-0000-0000-0000-000000000000' 
WHERE 
    vendor_mail_template_id = /*ds mailTemplateId*/'00000000-0000-0000-0000-000000000000' 
    AND is_active=1 
";
            }
            else
            {
                sql = @"
UPDATE 
    VendorMailTemplate
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username = @openId
WHERE
    vendor_mail_template_id = @mailTemplateId
    AND is_active=1
";
            }
            var param = new
            {
                mailTemplateId = mailTemplateId,
                openId = PerRequestDataContainer.OpenId
            };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            Connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        public bool IsExistsMailTemplateName(string mailTemplateName, string? excludeMailTemplateId = null)
        {
            var dbsettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbsettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    COUNT(*) 
FROM 
    VENDOR_MAIL_TEMPLATE vmt 
WHERE 
    mail_template_name = /*ds @mailTemplateName*/'aaa' AND 
/*ds if excludeMailTemplateId != null*/ 
    vmt.vendor_mail_template_id != /*ds excludeMailTemplateId*/'00000000-0000-0000-0000-000000000000' AND 
/*ds end if*/ 
    is_active = 1";
            }
            else
            {
                sql = @"
SELECT
    COUNT(*) 
FROM
    VendorMailTemplate vmt
WHERE
    mail_template_name = @mailTemplateName AND
/*ds if excludeMailTemplateId != null*/
    vmt.vendor_mail_template_id != @excludeMailTemplateId AND
/*ds end if*/
    is_active = 1";
            }
            var param = new { mailTemplateName = mailTemplateName, excludeMailTemplateId = excludeMailTemplateId };
            var twowaySql = new TwowaySqlParser(dbsettings.GetDbType(), sql, param);
            var dynParams = dbsettings.GetParameters().AddDynamicParams(param);
            return Connection.Query<int>(twowaySql.Sql, dynParams).FirstOrDefault() == 0 ? false : true;
        }
    }
}