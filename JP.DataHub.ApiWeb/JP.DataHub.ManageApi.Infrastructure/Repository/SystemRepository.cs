using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.Infrastructure.Database.Authority;
using DynamicApi = JP.DataHub.Infrastructure.Database.DynamicApi;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;
using Dapper.Oracle;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class SystemRepository : AbstractRepository, ISystemRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Authority"));
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }

        private Lazy<IJPDataHubDbConnection> _lazyApiConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection _apiConnection { get => _lazyApiConnection.Value; }

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SystemModel, DB_System>()
                    .ForMember(dst => dst.Client, ops => ops.Ignore())
                    .ForMember(dst => dst.SystemAdmin, ops => ops.Ignore())
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
                cfg.CreateMap<DynamicApi.DB_SystemLink, SystemLinkModel>();
                cfg.CreateMap<SystemLinkModel, DynamicApi.DB_SystemLink>()
                    .ForMember(dst => dst.system_link_id, ops => ops.MapFrom(src => src.SystemLinkId))
                    .ForMember(dst => dst.system_id, ops => ops.MapFrom(src => src.SystemId))
                    .ForMember(dst => dst.title, ops => ops.MapFrom(src => src.Title))
                    .ForMember(dst => dst.detail, ops => ops.MapFrom(src => src.Detail))
                    .ForMember(dst => dst.url, ops => ops.MapFrom(src => src.Url))
                    .ForMember(dst => dst.is_visible, ops => ops.MapFrom(src => src.IsVisible))
                    .ForMember(dst => dst.is_default, ops => ops.MapFrom(src => src.IsDefault))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
                cfg.CreateMap<UpdateClientModel, DB_Client>()
                    .ForMember(dst => dst.client_id, ops => ops.MapFrom(src => src.ClientId))
                    .ForMember(dst => dst.system_id, ops => ops.MapFrom(src => src.SystemId))
                    .ForMember(dst => dst.client_secret, ops => ops.MapFrom(src => src.ClientSecret))
                    .ForMember(dst => dst.accesstoken_expiration_timespan, ops => ops.MapFrom(src => src.AccessTokenExpirationTimeSpan.TotalSeconds))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
                cfg.CreateMap<UpdateClientModel, ClientModel>();
                cfg.CreateMap<DB_Client, ClientModel>()
                    .ForMember(dst => dst.ClientId, ops => ops.MapFrom(src => src.client_id))
                    .ForMember(dst => dst.ClientSecret, ops => ops.MapFrom(src => src.client_secret))
                    .ForMember(dst => dst.SystemId, ops => ops.MapFrom(src => src.system_id))
                    .ForMember(dst => dst.AccessTokenExpirationTimeSpan, ops => ops.MapFrom(src => new TimeSpan(0, 0, (int)src.accesstoken_expiration_timespan)))
                    .ForMember(dst => dst.IsActive, ops => ops.MapFrom(src => src.is_active));
            }).CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        public SystemModel GetSystem(string systemId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    s.system_id AS SystemId
    ,s.system_name AS SystemName
    ,s.openid_applicationid AS OpenIdApplicationId
    ,s.openid_client_secret AS OpenIdClientSecret
    ,s.is_enable AS IsEnable
    ,s.representative_mail_address AS RepresentativeMailAddress
    ,v.vendor_id AS VendorId
    ,v.vendor_name AS VendorName
FROM
    SYSTEM s
    INNER JOIN VENDOR v ON s.vendor_id=v.vendor_id AND v.is_enable=1 AND v.is_active=1
WHERE
    s.system_id= /*ds system_id*/'00000000-0000-0000-0000-000000000000' 
    AND s.is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    s.system_id AS SystemId
    ,s.system_name AS SystemName
    ,s.openid_applicationid AS OpenIdApplicationId
    ,s.openid_client_secret AS OpenIdClientSecret
    ,s.is_enable AS IsEnable
    ,s.representative_mail_address AS RepresentativeMailAddress
    ,v.vendor_id AS VendorId
    ,v.vendor_name AS VendorName
FROM
    System AS s
    INNER JOIN Vendor AS v ON s.vendor_id=v.vendor_id AND v.is_enable=1 AND v.is_active=1
WHERE
    s.system_id=@system_id
    AND s.is_active=1
";
            }
            var param = new { system_id = systemId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.QuerySingle<SystemModel>(twowaySql.Sql, dynParams);
        }
        public bool IsSystemExists(string systemId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    SYSTEM s
WHERE
    s.system_id= /*ds system_id*/'1' 
    AND s.is_active=1
    AND s.is_enable=1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    system s
WHERE
    s.system_id=@system_id
    AND s.is_active=1
    AND s.is_enable=1
";
            }
            var param = new { system_id = systemId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<int>(twowaySql.Sql, dynParams).FirstOrDefault() == 1 ? true : false;
        }
        [CacheIdFire("SystemId", "model.SystemId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_SYSTEM)]
        [DomainDataSync(ParameterType.Result, AuthorityDatabase.TABLE_SYSTEM, "SystemId")]
        public SystemModel RegisterSystem(SystemModel model)
        {
            if(ExistsSystemName(model.SystemName, model.VendorId))
            {
                throw new AlreadyExistsException("指定されたベンダー名は既に使われています");

            }
            // VendorModelにはメールアドレスリストが無いので変換はしないが、持たせた場合には変換が必要
            try
            {
                model.SystemId = _connection.Insert(s_mapper.Map<DB_System>(model)).ToString();
            }
            catch (SqlException ex)
            {
                throw (ex.Number == 2627) ? new ForeignKeyException(ex.Message, ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return model;
        }

        [CacheIdFire("SystemId", "model.SystemId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_SYSTEM)]
        [DomainDataSync(AuthorityDatabase.TABLE_SYSTEM, "model.SystemId")]
        public SystemModel UpdateSystem(SystemModel model)
        {
            if (ExistsSystemName(model.SystemName, model.VendorId,model.SystemId))
            {
                throw new AlreadyExistsException("指定されたベンダー名は既に使われています");

            }
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    SYSTEM
SET
    system_name = /*ds system_name*/'1' 
    ,openid_applicationid = /*ds openid_applicationid*/'1' 
    ,openid_client_secret = /*ds openid_client_secret*/'1' 
    ,is_enable = /*ds is_enable*/1 
    ,upd_date = SYSTIMESTAMP
    ,upd_username = /*ds open_id*/'1' 
    ,representative_mail_address = /*ds representative_mail_address*/'1' 
WHERE
    system_id = /*ds system_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE
    System
SET
    system_name = @system_name
    ,openid_applicationid = @openid_applicationid
    ,openid_client_secret = @openid_client_secret
    ,is_enable = @is_enable
    ,upd_date = GETDATE()
    ,upd_username = @open_id
    ,representative_mail_address = @representative_mail_address
WHERE
    system_id = @system_id
    AND is_active=1
";
            }
            try
            {
                var param = new { system_id = model.SystemId, system_name = model.SystemName, openid_applicationid = model.OpenIdApplicationId, openid_client_secret = model.OpenIdClientSecret, is_enable = model.IsEnable, open_id = PerRequestDataContainer.OpenId, representative_mail_address = model.RepresentativeMailAddress };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                var result = _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
                if (result <= 0)
                {
                    throw new NotFoundException($"Not Found SystemId={model.SystemId}");
                }
            }
            catch (SqlException ex)
            {
                throw (ex.Number == 2627) ? new AlreadyExistsException("指定されたベンダー名は既に使われています", ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return model;
        }

        [Cache]
        [CacheArg("systemId")]
        [CacheIdFire("SystemId", "systemId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_SYSTEM)]
        [DomainDataSync(AuthorityDatabase.TABLE_SYSTEM, "systemId")]
        public void DeleteSystem(string systemId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    SYSTEM
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username= /*ds open_id*/'1' 
 WHERE
    system_id= /*ds system_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    System
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    system_id=@system_id
    AND is_active=1
";
            }
            var param = new
            {
                system_id = systemId,
                open_id = PerRequestDataContainer.OpenId
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            if (result <= 0)
            {
                throw new NotFoundException($"Not Found SystemId={systemId}");
            }
        }

        public IList<SystemModel> GetSystemListByVendorId(string vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    s.system_id AS SystemId
	,s.system_name AS SystemName
	,s.openid_applicationid AS OpenIdApplicationId
	,s.openid_client_secret AS OpenIdClientSecret
    ,s.is_enable AS IsEnable
    ,s.vendor_id AS VendorId
    ,v.vendor_name  AS VendorName
FROM 
    SYSTEM s 
    INNER JOIN VENDOR v ON s.vendor_id = v.vendor_id AND v.is_active = 1 
WHERE 
    s.vendor_id= /*ds vendor_id*/'1' 
    AND s.is_active=1
";
            }
            else
            {
                sql = @"
SELECT 
    s.system_id AS SystemId
	,s.system_name AS SystemName
	,s.openid_applicationid AS OpenIdApplicationId
	,s.openid_client_secret AS OpenIdClientSecret
    ,s.is_enable AS IsEnable
    ,s.vendor_id AS VendorId
    ,v.vendor_name  AS VendorName
FROM 
    System s 
    INNER JOIN Vendor v ON s.vendor_id = v.vendor_id AND v.is_active = 1 
WHERE 
    s.vendor_id=@vendor_id 
    AND s.is_active=1
";
            }
            var param = new { vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<SystemModel>(twowaySql.Sql, dynParams).ToList();
        }

        public IList<FunctionNodeModel> GetFunctionBySystemId(string systemId)
        {
            // 実装しない
            return null;
        }

        public IList<string> RegisterFunction(string systemId, IList<FunctionNodeModel> list)
        {
            // 実装しない
            return null;
        }

        public IList<ClientModel> GetClientBySystemId(string systemId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    c.client_id AS ClientId
	,c.client_secret AS ClientSecret
	,s.system_id AS SystemId
	,c.accesstoken_expiration_timespan AS AccessTokenExpirationTimeSpan
    ,c.is_active AS IsActive
FROM 
    SYSTEM s
    INNER JOIN CLIENT c ON s.system_id=c.system_id AND c.is_active=1
    INNER JOIN VENDOR v ON s.vendor_id=v.vendor_id AND v.is_active=1
WHERE 
    s.system_id= /*ds system_id*/'1' 
    AND s.is_active=1
";
            }
            else
            {
                sql = @"
SELECT 
    c.client_id AS ClientId
	,c.client_secret AS ClientSecret
	,s.system_id AS SystemId
	,c.accesstoken_expiration_timespan AS AccessTokenExpirationTimeSpan
    ,c.is_active AS IsActive
FROM 
    System AS s
    INNER JOIN Client AS c ON s.system_id=c.system_id AND c.is_active=1
    INNER JOIN Vendor AS v ON s.vendor_id=v.vendor_id AND v.is_active=1
WHERE 
    s.system_id=@system_id
    AND s.is_active=1
";
            }
            var param = new { system_id = systemId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<ClientModel>(twowaySql.Sql, dynParams).ToList();
        }

        public IList<SystemLinkModel> GetLinkBySystemId(string systemId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    l.system_link_id AS SystemLinkId
	,s.system_id AS SystemId
	,l.title AS Title
	,l.detail AS Detail
    ,l.url AS Url
    ,l.is_visible AS IsVisible
    ,l.is_default AS IsDefault
FROM 
    SYSTEM s
    INNER JOIN SYSTEM_LINK l ON s.system_id=l.system_id AND l.is_active=1
    INNER JOIN VENDOR v ON s.vendor_id=v.vendor_id AND v.is_active=1
WHERE 
    s.system_id= /*ds system_id*/'1' 
    AND s.is_active=1
";
            }
            else
            {
                sql = @"
SELECT 
    l.system_link_id AS SystemLinkId
	,s.system_id AS SystemId
	,l.title AS Title
	,l.detail AS Detail
    ,l.url AS Url
    ,l.is_visible AS IsVisible
    ,l.is_default AS IsDefault
FROM 
    System AS s
    INNER JOIN SystemLink AS l ON s.system_id=l.system_id AND l.is_active=1
    INNER JOIN Vendor AS v ON s.vendor_id=v.vendor_id AND v.is_active=1
WHERE 
    s.system_id=@system_id
    AND s.is_active=1
";
            }
            var param = new { system_id = systemId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _apiConnection.Query<SystemLinkModel>(twowaySql.Sql, dynParams).ToList();
        }

        public IList<string> RegisterLink(string systemId, IList<SystemLinkModel> list)
        {
            return null;
        }

        public SystemAdminModel GetAdminBySystemId(string systemId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    a.admin_secret AS AdminSecret
    ,a.system_id AS SystemId
    ,a.is_active AS IsActive
FROM 
    SYSTEM s
    INNER JOIN SYSTEM_ADMIN a ON s.system_id=a.system_id AND a.is_active=1
    INNER JOIN VENDOR v ON s.vendor_id=v.vendor_id AND v.is_active=1
WHERE 
    s.system_id= /*ds system_id*/'1' 
    AND s.is_active=1
";
            }
            else
            {
                sql = @"
SELECT 
    a.admin_secret AS AdminSecret
    ,a.system_id AS SystemId
    ,a.is_active AS IsActive
FROM 
    System AS s
    INNER JOIN SystemAdmin AS a ON s.system_id=a.system_id AND a.is_active=1
    INNER JOIN Vendor AS v ON s.vendor_id=v.vendor_id AND v.is_active=1
WHERE 
    s.system_id=@system_id
    AND s.is_active=1
";
            }
            var param = new { system_id = systemId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.QuerySingle<SystemAdminModel>(twowaySql.Sql, dynParams);
        }

        [DomainDataSync(ParameterType.Result, AuthorityDatabase.TABLE_SYSTEMADMIN, ".")]
        public SystemAdminModel RegisterAdmin(SystemAdminModel model)
        {
            SystemAdminModel data = new()
            {
                SystemId = model.SystemId,
                AdminSecret = model.AdminSecret,
                IsActive = model.IsActive
            };
            try
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";

                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
MERGE INTO SYSTEM_ADMIN a
USING(SELECT /*ds system_id*/'1' AS system_id FROM DUAL) b
ON(a.system_id = b.system_id)
WHEN MATCHED THEN
    UPDATE SET
        admin_secret = /*ds admin_secret*/'1' 
        , is_active = /*ds is_active*/1 
        , upd_username = /*ds open_id*/'1' 
        , upd_date = SYSTIMESTAMP 
WHEN NOT MATCHED THEN
    INSERT(system_admin_id, system_id, admin_secret, reg_date, reg_username, upd_date, upd_username, is_enable, is_active)
    VALUES(/*ds system_admin_id*/'1' , /*ds system_id*/'1' , /*ds admin_secret*/'1' , SYSTIMESTAMP, /*ds open_id*/'1' , SYSTIMESTAMP, /*ds open_id*/'1' , 1, /*ds is_active*/1 )
";
                }
                else
                {
                    sql = @"
MERGE INTO SystemAdmin AS a
USING(SELECT @system_id AS system_id) AS b
ON(a.system_id = b.system_id)
WHEN MATCHED THEN
    UPDATE SET
        system_id = @system_id
        , admin_secret = @admin_secret
        , is_active = @is_active
        , upd_username = @open_id
        , upd_date = GETDATE()
WHEN NOT MATCHED THEN
    INSERT(system_admin_id, system_id, admin_secret, reg_date, reg_username, upd_date, upd_username, is_enable, is_active)
    VALUES(@system_admin_id, @system_id, @admin_secret, GETDATE(), @open_id, GETDATE(), @open_id, 1, @is_active);
";
                }
                var param = new { system_admin_id = Guid.NewGuid(), system_id = data.SystemId, admin_secret = data.AdminSecret, open_id = PerRequestDataContainer.OpenId, is_active = data.IsActive };
//                var param = new Dictionary<string, object>()
//                {
//                    { "system_admin_id", Guid.NewGuid() },
//                    { "system_id", data.SystemId },
//                    { "admin_secret", data.AdminSecret },
//                    { "open_id", PerRequestDataContainer.OpenId },
//                    { "is_active", dbSettings.Type == "Oracle" ? (data.IsActive ? 1 : 0) : data.IsActive }
//                };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            }
            catch (SqlException ex)
            {
                throw ex.Number == 547 ? new ForeignKeyException(ex.Message, ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return data;
        }

        [DomainDataSync(ParameterType.Result, AuthorityDatabase.TABLE_SYSTEMADMIN, ".")]
        public IList<string> DeleteAdminBySystemId(string systemId)
        {
            // SystemAdminの削除対象をDDSに通知するために取得する
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
    system_admin_id
FROM 
    SYSTEM_ADMIN
WHERE 
    system_id= /*ds system_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT 
    system_admin_id
FROM 
    SystemAdmin
WHERE 
    system_id=@system_id
    AND is_active=1
";
            }
            var param = new { system_id = systemId, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = _connection.Query<string>(twowaySql.Sql, dynParams).ToList();

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    SYSTEM_ADMIN
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username= /*ds open_id*/'1' 
WHERE
    system_id= /*ds system_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    SystemAdmin
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    system_id=@system_id
    AND is_active=1
";
            }
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var deleteResult = _connection.Execute(twowaySql.Sql, dynParams);
            if (deleteResult <= 0)
            {
                throw new NotFoundException($"Not Found SystemId={systemId}");
            }

            return result;
        }

        public IList<SystemLinkModel> GetSystemLinkListBySystemId(string systemId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    system_link_id AS SystemLinkId
    ,title AS Title
    ,detail AS Detail
    ,url AS Url
    ,is_visible AS IsVisible
    ,is_default AS IsDefault
    ,system_id AS SystemId
    ,is_active AS IsActiveFROM
FROM
    SYSTEM_LINK
Where
/*ds if system_id != null*/
    system_id= /*ds system_id*/'1' AND
/*ds end if*/
    is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    system_link_id AS SystemLinkId
    ,title AS Title
    ,detail AS Detail
    ,url AS Url
    ,is_visible AS IsVisible
    ,is_default AS IsDefault
    ,system_id AS SystemId
    ,is_active AS IsActiveFROM
FROM
    SystemLink
Where
/*ds if system_id != null*/
    system_id=@system_id AND
/*ds end if*/
    is_active=1
";
            }
            var param = new { system_id = systemId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _apiConnection.Query<SystemLinkModel>(twowaySql.Sql, dynParams).ToList();
        }

        public SystemLinkModel GetSystemLink(string systemLinkId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    system_link_id AS SystemLinkId
    ,title AS Title
    ,detail AS Detail
    ,url AS Url
    ,is_visible AS IsVisible
    ,is_default AS IsDefault
    ,system_id AS SystemId
    ,is_active AS IsActive
FROM
    SYSTEM_LINK
Where
/*ds if system_link_id != null*/
    system_link_id= /*ds system_link_id*/'1' AND
/*ds end if*/
    is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    system_link_id AS SystemLinkId
    ,title AS Title
    ,detail AS Detail
    ,url AS Url
    ,is_visible AS IsVisible
    ,is_default AS IsDefault
    ,system_id AS SystemId
    ,is_active AS IsActive
FROM
    SystemLink
Where
/*ds if system_link_id != null*/
    system_link_id=@system_link_id AND
/*ds end if*/
    is_active=1
";
            }
            var param = new { system_link_id = systemLinkId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _apiConnection.QuerySingle<SystemLinkModel>(twowaySql.Sql, dynParams);
        }

        [DomainDataSync(ParameterType.Result, DynamicApiDatabase.TABLE_SYSTEMLINK, "SystemLinkId")]
        public IList<SystemLinkModel> UpsertSystemLink(IList<SystemLinkModel> model)
        {

            try
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";

                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
MERGE INTO SYSTEM_LINK a
    USING(
    SELECT 
    /*ds system_link_id*/'1' AS system_link_id FROM DUAL) b
    ON(a.system_link_id = b.system_link_id)
        WHEN MATCHED THEN
            UPDATE SET
                system_id = /*ds system_id*/'1' 
                , title = /*ds title*/'1' 
                , detail = /*ds detail*/'1' 
                , url = /*ds url*/'1' 
                , is_active = 1
                , is_visible = /*ds is_visible*/1 
                , is_default = /*ds is_default*/1 
                , upd_username = /*ds open_id*/'1' 
                , upd_date = SYSTIMESTAMP
        WHEN NOT MATCHED THEN
            INSERT(system_link_id, system_id, title, detail, url, is_visible, reg_date, reg_username, upd_date, upd_username, is_active, is_default)
            VALUES(/*ds system_link_id*/'1' , /*ds system_id*/'1' , /*ds title*/'1' , /*ds detail*/'1' , /*ds url*/'1' , /*ds is_visible*/1 , SYSTIMESTAMP, /*ds open_id*/'1' , SYSTIMESTAMP, /*ds open_id*/'1' , 1, /*ds is_default*/1 )
";
                }
                else
                {
                    sql = @"
MERGE INTO SystemLink AS a
    USING(
    SELECT 
    @system_link_id AS system_link_id) AS b
    ON(a.system_link_id = b.system_link_id)
        WHEN MATCHED THEN
            UPDATE SET
		        system_link_id = @system_link_id
                , system_id = @system_id
		        , title = @title
		        , detail = @detail
		        , url = @url
                , is_active = 1
                , is_visible = @is_visible
                , is_default = @is_default
                , upd_username = @open_id
                , upd_date = GETDATE()
        WHEN NOT MATCHED THEN
            INSERT(system_link_id, system_id, title, detail, url, is_visible, reg_date, reg_username, upd_date, upd_username, is_active, is_default)
            VALUES(@system_link_id, @system_id, @title, @detail, @url, @is_visible, GETDATE(), @open_id, GETDATE(), @open_id, 1, @is_default);
";
                }
                model.ForEach(x =>
                {
                    if (string.IsNullOrEmpty(x.SystemLinkId))
                    {
                        x.SystemLinkId = Guid.NewGuid().ToString();
                    }
                    var param = new { system_link_id = x.SystemLinkId, system_id = x.SystemId, 
                        title = x.Title, detail = x.Detail, open_id = PerRequestDataContainer.OpenId, url = x.Url,
                        is_visible = x.IsVisible, is_default = x.IsDefault
                    };
                    var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                    var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                    _apiConnection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
                });
            }
            catch (SqlException ex)
            {
                throw ex.Number == 547 ? new ForeignKeyException(ex.Message, ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return s_mapper.Map<IList<SystemLinkModel>>(model);
        }

        [DomainDataSync(DynamicApiDatabase.TABLE_SYSTEMLINK, "systemLinkId")]
        public void DeleteSystemLink(string systemLinkId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    SYSTEM_LINK
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username= /*ds open_id*/'1' 
 WHERE
    system_link_id= /*ds system_link_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    SystemLink
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    system_link_id=@system_link_id
    AND is_active=1
";
            }
            var param = new { system_link_id = systemLinkId, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = _apiConnection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            if (result <= 0)
            {
                throw new NotFoundException($"Not Found SystemLinkId={systemLinkId}");
            }
        }

        public IList<ClientModel> GetClientListBySystemId(string systemId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    client_id
    ,system_id
    ,accesstoken_expiration_timespan
    ,is_active
FROM
    CLIENT
Where
/*ds if system_id != null*/
    system_id= /*ds system_id*/'1' AND
/*ds end if*/
    is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    client_id
    ,system_id
    ,accesstoken_expiration_timespan
    ,is_active
FROM
    Client
Where
/*ds if system_id != null*/
    system_id=@system_id AND
/*ds end if*/
    is_active=1
";
            }
            var param = new { system_id = systemId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<DB_Client>(twowaySql.Sql, dynParams).Select(x => s_mapper.Map<ClientModel>(x)).ToList();
        }

        public IList<ClientModel> GetClientListBySystemIds(IList<string> systemIds)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    client_id
    ,client_secret
    ,system_id
    ,accesstoken_expiration_timespan
FROM
    CLIENT
Where
    system_id in /*ds system_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    client_id
    ,client_secret
    ,system_id
    ,accesstoken_expiration_timespan
FROM
    Client
Where
    system_id in @system_id
    AND is_active=1
";
            }
            var param = new { system_id = systemIds };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            // dynamicParameterで配列がうまく扱えない為、dynamicParameter化未対応
            return _connection.Query<DB_Client>(twowaySql.Sql, param).Select(x => s_mapper.Map<ClientModel>(x)).ToList();
        }

        public ClientModel GetClient(string clientId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    client_id
    ,client_secret
    ,system_id
    ,accesstoken_expiration_timespan
    ,is_active
FROM
    CLIENT
Where
/*ds if client_id != null*/
    client_id= /*ds client_id*/'1' AND
/*ds end if*/
    is_active=1";
            }
            else
            {
                sql = @"
SELECT
    client_id
    ,client_secret
    ,system_id
    ,accesstoken_expiration_timespan
    ,is_active
FROM
    Client
Where
/*ds if client_id != null*/
    client_id=@client_id AND
/*ds end if*/
    is_active=1";
            }
            var param = new { client_id = clientId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return s_mapper.Map<ClientModel>(_connection.QuerySingle<DB_Client>(twowaySql.Sql, dynParams));
        }

        [DomainDataSync(ParameterType.Result, AuthorityDatabase.TABLE_CLIENT, "ClientId")]
        public ClientModel RegisterClient(UpdateClientModel model)
        {
            var result = s_mapper.Map<ClientModel>(model);
            result.ClientId = _connection.Insert(s_mapper.Map<DB_Client>(model)).ToString();
            return result;
        }

        [DomainDataSync(AuthorityDatabase.TABLE_CLIENT, "model.ClientId")]
        public ClientModel UpdateClient(UpdateClientModel model)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    CLIENT
SET
    client_secret= /*ds client_secret*/'1' 
    ,accesstoken_expiration_timespan= /*ds timespan*/'1' 
    ,upd_date = SYSTIMESTAMP
    ,upd_username = /*ds open_id*/'1' 
    ,is_active=1
 WHERE
    client_id= /*ds client_id*/'1'
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE
    Client
SET
    client_secret=@client_secret
    ,accesstoken_expiration_timespan=@timespan
    ,upd_date = GETDATE()
    ,upd_username = @open_id
    ,is_active=1
WHERE
    client_id=@client_id
    AND is_active=1
";
            }
            try
            {
                var param = new { client_id = model.ClientId, client_secret = model.ClientSecret, timespan = model.AccessTokenExpirationTimeSpan.TotalSeconds, open_id = PerRequestDataContainer.OpenId };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
                return s_mapper.Map<ClientModel>(model);
            }
            catch (SqlException ex)
            {
                throw (ex.Number == 2627) ? new AlreadyExistsException("指定されたベンダー名は既に使われています", ex) : new SqlDatabaseException(ex.Message, ex);
            }
        }

        [DomainDataSync(AuthorityDatabase.TABLE_CLIENT, "clientId")]
        public void DeleteClient(string clientId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    CLIENT
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username= /*ds open_id*/'1' 
 WHERE
    client_id= /*ds client_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    Client
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    client_id=@client_id
    AND is_active=1
";
            }
            var param = new { client_id = clientId, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            if (result <= 0)
            {
                throw new NotFoundException($"Not Found ClientId={clientId}");
            }

        }
        private bool ExistsSystemName(string systemName, string vendorId, string systemId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(system_name) 
FROM 
    SYSTEM 
WHERE 
    system_name = /*ds SystemName*/'1' 
 AND vendor_id = /*ds VendorId*/'1' 
 AND is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(system_name) 
FROM 
    System 
WHERE 
    system_name = @SystemName 
AND vendor_id = @VendorId 
AND is_active = 1
";
            }
            int count = 0;

            if (string.IsNullOrEmpty(systemId))
            {
                var param = new { SystemName = systemName, VendorId = vendorId };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                count = _connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            }
            else
            {
                if (dbSettings.Type == "Oracle")
                {
                    sql += " AND system_id <> /*ds SystemId*/'1' ";
                }
                else
                {
                    sql += " AND system_id <> @SystemId";
                }
                var param = new { SystemName = systemName, VendorId = vendorId, SystemId = systemId };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                count = _connection.QuerySingle<int>(twowaySql.Sql, dynParams);
            }

            return count != 0;
        }
    }
}