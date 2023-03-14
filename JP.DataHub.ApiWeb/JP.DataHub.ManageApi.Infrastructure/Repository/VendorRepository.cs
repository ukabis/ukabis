using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Infrastructure.Repository.Model;
using JP.DataHub.Infrastructure.Database.Authority;
using DynamicApiDB = JP.DataHub.Infrastructure.Database.DynamicApi;
using Microsoft.Azure.Amqp.Framing;
using System.Security.Policy;
using JP.DataHub.Com.Settings;
using StackExchange.Redis;
using JP.DataHub.Com.Sql;
using Dapper.Oracle;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class VendorRepository : AbstractRepository, IVendorRepository
    {
        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Authority"));
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }

        private Lazy<IJPDataHubDbConnection> _lazyDynamicConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));
        private IJPDataHubDbConnection _dynamicConnection { get => _lazyDynamicConnection.Value; }

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TmpVendorNameSystemName, VendorModel>()
                    .ForMember(dst => dst.IsEnable, ops => ops.MapFrom(src => src.IsEnableVendor));
                cfg.CreateMap<TmpVendorNameSystemName, VendorSystemModel>();
                cfg.CreateMap<TmpVendorNameSystemName, StaffRoleModel>();
                cfg.CreateMap<TmpStaffModel, StaffRoleModel>();
                cfg.CreateMap<VendorModel, DB_Vendor>()
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
                cfg.CreateMap<StaffModel, DB_Staff>()
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
                cfg.CreateMap<RegisterVendorLinkModel, DynamicApiDB.DB_VendorLink>()
                    .ForMember(dst => dst.vendor_link_id, ops => ops.MapFrom(src => src.VendorLinkId))
                    .ForMember(dst => dst.vendor_id, ops => ops.Ignore())
                    .ForMember(dst => dst.title, ops => ops.MapFrom(src => src.LinkTitle))
                    .ForMember(dst => dst.detail, ops => ops.MapFrom(src => src.LinkDetail))
                    .ForMember(dst => dst.url, ops => ops.MapFrom(src => src.LinkUrl))
                    .ForMember(dst => dst.is_visible, ops => ops.MapFrom(src => src.IsVisible))
                    .ForMember(dst => dst.is_default, ops => ops.MapFrom(src => src.IsDefault))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
                cfg.CreateMap<RegisterVendorLinkModel, VendorLinkModel>();
                cfg.CreateMap<RegisterOpenIdCaModel, DynamicApiDB.DB_VendorOpenIdCA>()
                    .ForMember(dst => dst.vendor_openid_ca_id, ops => ops.MapFrom(src => src.VendorOpenidCaId))
                    .ForMember(dst => dst.vendor_id, ops => ops.MapFrom(src => src.VendorId))
                    .ForMember(dst => dst.application_id, ops => ops.MapFrom(src => src.ApplicationId))
                    .ForMember(dst => dst.access_control, ops => ops.MapFrom(src => src.AccessControl))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
            }).CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        [Cache]
        [CacheArg(allParam: true)]
        [CacheEntity(AuthorityDatabase.TABLE_VENDOR, AuthorityDatabase.TABLE_SYSTEM, AuthorityDatabase.TABLE_STAFF)]
        public IList<VendorModel> GetVendorSystemNameList(bool includeDelete = false, bool enableOnly = false, bool isGetStaffList = false, string vendorId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    v.vendor_id AS VendorId
    ,v.vendor_name AS VendorName
    ,v.is_data_use AS IsDateUse
    ,v.is_data_offer AS IsDataOffer
    ,v.is_enable AS IsEnableVendor
    ,v.upd_date AS VendorUpdDate
    ,v.representative_mail_address AS RepresentativeMailAddress
    ,s.system_id AS SystemId
    ,s.system_name AS SystemName
    ,s.openid_applicationid AS ApplicationId
    ,s.upd_date AS SystemUpdDate
	,s.is_enable AS IsEnableSystem
FROM
    VENDOR v
    LEFT OUTER JOIN SYSTEM s ON v.vendor_id= s.vendor_id
/*ds if active != null*/
    AND s.is_active = 1
/*ds end if*/
/*ds if enable != null*/
    AND s.is_enable = 1
/*ds end if*/
WHERE
/*ds if vendorId != null*/
    v.vendor_id = /*ds vendorId*/'1' AND
/*ds end if*/
/*ds if active != null*/
    v.is_active = 1 AND
/*ds end if*/
/*ds if enable != null*/
    v.is_enable = 1 AND
/*ds end if*/
    1 = 1
ORDER BY 
    v.vendor_name
    ,s.system_name
";
            }
            else
            {
                sql = @"
SELECT
    v.vendor_id AS VendorId
    ,v.vendor_name AS VendorName
    ,v.is_data_use AS IsDateUse
    ,v.is_data_offer AS IsDataOffer
    ,v.is_enable AS IsEnableVendor
    ,v.upd_date AS VendorUpdDate
    ,v.representative_mail_address AS RepresentativeMailAddress
    ,s.system_id AS SystemId
    ,s.system_name AS SystemName
    ,s.openid_applicationid AS ApplicationId
    ,s.upd_date AS SystemUpdDate
	,s.is_enable AS IsEnableSystem
FROM
    vendor v
    LEFT OUTER JOIN system s ON v.vendor_id=s. vendor_id AND s.is_active IN @active AND s.is_enable IN @enable
WHERE
/*ds if vendorId != null*/
    v.vendor_id = @vendorId AND
/*ds end if*/
    v.is_active IN @active AND
    v.is_enable IN @enable
ORDER BY 
    v.vendor_name
    ,s.system_name
";
            }
            bool? isActiveListForOracle = true;
            var isActiveListForSQLServer = new List<bool>() { true };
            if (includeDelete)
            {
                isActiveListForOracle = null;
                isActiveListForSQLServer.Add(false);
            }
            bool? isEnableListForOracle = true;
            var isEnableListForSQLServer = new List<bool>() { true };
            if (!enableOnly)
            {
                isEnableListForOracle = null;
                isEnableListForSQLServer.Add(false);
            }
            var dict = new Dictionary<string, object?>()
            {
                { "active", dbSettings.Type == "Oracle" ? isActiveListForOracle : isActiveListForSQLServer },
                { "enable", dbSettings.Type == "Oracle" ? isEnableListForOracle : isEnableListForSQLServer },
                { "vendorId", vendorId }
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, dict);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(dict);
            var tmp = _connection.Query<TmpVendorNameSystemName>(twowaySql.Sql, dynParams);

            var vendorids = tmp.Select(x => x.VendorId).Distinct().ToList();
            var result = vendorids.Select(x => s_mapper.Map<VendorModel>(tmp.FirstOrDefault(t => t.VendorId == x))).ToList();
            result.ForEach(v => v.SystemList = tmp.Where(x => x.VendorId == v.VendorId && x.SystemId != null).Select(x => s_mapper.Map<VendorSystemModel>(x)).ToList());

            // Staffリストの取得
            if (isGetStaffList)
            {
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT
    s.staff_id AS StaffId
    ,s.account AS Account
    ,s.vendor_id AS VendorId
    ,s.email_address AS EmailAddress
FROM
    STAFF s
WHERE
    s.vendor_id IN /*ds vendorId*/'1' 
    AND s.is_active = 1
";
                }
                else
                {
                    sql = @"
SELECT
    s.staff_id AS StaffId
    ,s.account AS Account
    ,s.vendor_id AS VendorId
    ,s.email_address AS EmailAddress
FROM
    [Staff] AS s
WHERE
    s.vendor_id IN @vendorId
    AND s.is_active = 1
";
                }
                var paramStaff = new { vendorId = vendorids };
                twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, paramStaff);
                // dynamicParameterで配列がうまく扱えない為、dynamicParameter化未対応
                var staffs = _connection.Query<TmpStaffModel>(twowaySql.Sql, paramStaff).ToList();
                result.ForEach(v => v.StaffList = staffs.Where(x => x.VendorId == v.VendorId).Select(x => s_mapper.Map<StaffRoleModel>(x)).ToList());
            }

            return result;
        }

        [CacheIdFire("VendorId", "model.VendorId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_VENDOR)]
        [DomainDataSync(ParameterType.Result, AuthorityDatabase.TABLE_VENDOR, "VendorId")]
        public VendorModel Register(VendorModel model)
        {
            if (ExistsVendorName(model.VendorName))
            {
                throw new AlreadyExistsException("指定されたベンダー名は既に使われています");
            }
            // VendorModelにはメールアドレスリストが無いので変換はしないが、持たせた場合には変換が必要
            try
            {
                var now = DateTime.UtcNow;
                var vendorModel = s_mapper.Map<DB_Vendor>(model);
                model.VendorId = _connection.Insert(vendorModel).ToString();
            }
            catch (SqlException ex)
            {
                throw (ex.Number == 2627) ? new AlreadyExistsException("指定されたベンダー名は既に使われています", ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return model;
        }

        [CacheIdFire("VendorId", "model.VendorId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_VENDOR)]
        [DomainDataSync(AuthorityDatabase.TABLE_VENDOR, "model.VendorId")]
        public VendorModel Update(VendorModel model)
        {
            if (ExistsVendorName(model.VendorName, model.VendorId))
            {
                throw new AlreadyExistsException("指定されたベンダー名は既に使われています");
            }

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    VENDOR
SET
    vendor_name= /*ds vendor_name*/'1' 
    ,is_enable= /*ds is_enable*/1 
    ,is_data_offer= /*ds is_data_offer*/'1' 
    ,is_data_use= /*ds is_data_use*/'1' 
    ,upd_date=SYSTIMESTAMP
    ,upd_username= /*ds open_id*/'1' 
    ,representative_mail_address= /*ds representative_mail_address*/'1' 
WHERE
    vendor_id= /*ds vendor_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE
    vendor
SET
    vendor_name=@vendor_name
    ,is_enable=@is_enable
    ,is_data_offer=@is_data_offer
    ,is_data_use=@is_data_use
    ,upd_date=GETDATE()
    ,upd_username=@open_id
    ,representative_mail_address=@representative_mail_address
WHERE
    vendor_id=@vendor_id
    AND is_active=1
";
            }
            try
            {
                var param = new { vendor_id = model.VendorId, vendor_name = model.VendorName, is_enable = model.IsEnable, is_data_offer = model.IsDataOffer, is_data_use = model.IsDataUse, representative_mail_address = model.RepresentativeMailAddress, open_id = PerRequestDataContainer.OpenId };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                if (_connection.ExecutePrimaryKey(twowaySql.Sql, dynParams) <= 0)
                {
                    throw new NotFoundException($"Not Found VendorId={model.VendorId}");
                }
            }
            catch (SqlException ex)
            {
                throw (ex.Number == 2627) ? new AlreadyExistsException("指定されたベンダー名は既に使われています", ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return model;
        }

        [CacheIdFire("VendorId", "vendorId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_VENDOR)]
        [DomainDataSync(AuthorityDatabase.TABLE_VENDOR, "vendorId")]
        public void Delete(string vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    VENDOR
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username= /*ds staff_id*/'1' 
 WHERE
    vendor_id= /*ds vendor_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    Vendor
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@staff_id
WHERE
    vendor_id=@vendor_id
    AND is_active=1
";
            }
            var param = new { vendor_id = vendorId, staff_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            if (_connection.ExecutePrimaryKey(sql, dynParams) <= 0)
            {
                throw new NotFoundException($"Not Found VendorId={vendorId}");
            }
        }

        [CacheArg("vendorId")]
        [CacheEntity(AuthorityDatabase.TABLE_VENDOR)]
        [Cache]
        public VendorModel Get(string vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    v.vendor_id AS VendorId
    ,v.vendor_name AS VendorName
    ,v.is_data_use AS IsDataUse
    ,v.is_data_offer AS IsDataOffer
    ,v.is_enable AS IsEnableVendor
    ,v.upd_date AS VendorUpdDate
    ,v.representative_mail_address AS RepresentativeMailAddress
    ,s.system_id AS SystemId
    ,s.system_name AS SystemName
    ,s.openid_applicationid AS ApplicationId
    ,s.upd_date AS SystemUpdDate
	,s.is_enable AS IsEnableSystem
FROM
    VENDOR v
    LEFT OUTER JOIN SYSTEM s ON v.vendor_id=s. vendor_id AND s.is_active=1 AND s.is_enable=1
WHERE
    v.vendor_id= /*ds vendor_id*/'1' 
    AND v.is_active=1
ORDER BY 
    v.vendor_name
    ,s.system_name
";
            }
            else
            {
                sql = @"
SELECT
    v.vendor_id AS VendorId
    ,v.vendor_name AS VendorName
    ,v.is_data_use AS IsDataUse
    ,v.is_data_offer AS IsDataOffer
    ,v.is_enable AS IsEnableVendor
    ,v.upd_date AS VendorUpdDate
    ,v.representative_mail_address AS RepresentativeMailAddress
    ,s.system_id AS SystemId
    ,s.system_name AS SystemName
    ,s.openid_applicationid AS ApplicationId
    ,s.upd_date AS SystemUpdDate
	,s.is_enable AS IsEnableSystem
FROM
    vendor v
    LEFT OUTER JOIN system s ON v.vendor_id=s. vendor_id AND s.is_active=1 AND s.is_enable=1
WHERE
    v.vendor_id=@vendor_id
    AND v.is_active=1
ORDER BY 
    v.vendor_name
    ,s.system_name
";
            }
            var param = new { vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var tmp = _connection.Query<TmpVendorNameSystemName>(twowaySql.Sql, dynParams).ToList();
            if (tmp?.Count == 0)
            {
                throw new NotFoundException();
            }
            var result = s_mapper.Map<VendorModel>(tmp[0]);
            var systems = tmp.Select(x => x.SystemId).Distinct().ToList();
            result.SystemList = tmp.Select(x => s_mapper.Map<VendorSystemModel>(x)).ToList();
            return result;
        }

        public bool IsExists(string vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    VENDOR v
WHERE
    v.vendor_id= /*ds vendor_id*/'1' 
    AND v.is_active=1
    AND v.is_enable=1
";
            }
            else
            {
                sql = @"
SELECT
    COUNT(*)
FROM
    vendor v
WHERE
    v.vendor_id=@vendor_id
    AND v.is_active=1
    AND v.is_enable=1
";
            }
            var param = new { vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<int>(twowaySql.Sql, dynParams).FirstOrDefault() == 1 ? true : false;
        }

        [CacheArg("vendorId")]
        [CacheEntity(AuthorityDatabase.TABLE_STAFF)]
        [Cache]
        public IList<StaffRoleModel> GetStaffList(string vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    s.staff_id AS StaffId
    ,s.account AS Account
    ,s.vendor_id AS VendorId
    ,s.email_address AS EmailAddress
    ,sr.staff_role_id AS StaffRoleId
    ,r.role_id AS RoleId
    ,r.role_name AS RoleName
    ,v.vendor_id AS VendorId
    ,v.vendor_name AS VendorName
FROM
    STAFF s
    INNER JOIN STAFF_ROLE sr ON s.staff_id=sr.staff_id AND sr.is_active=1
    INNER JOIN ROLE r ON sr.role_id=r.role_id AND r.is_active=1
    INNER JOIN VENDOR v ON s.vendor_id=v.vendor_id AND v.is_active=1
WHERE
    s.vendor_id= /*ds vendor_id*/'1' 
    AND s.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    s.staff_id AS StaffId
    ,s.account AS Account
    ,s.vendor_id AS VendorId
    ,s.email_address AS EmailAddress
    ,sr.staff_role_id AS StaffRoleId
    ,r.role_id AS RoleId
    ,r.role_name AS RoleName
    ,v.vendor_id AS VendorId
    ,v.vendor_name AS VendorName
FROM
    Staff AS s
    INNER JOIN StaffRole AS sr ON s.staff_id=sr.staff_id AND sr.is_active=1
    INNER JOIN Role AS r ON sr.role_id=r.role_id AND r.is_active=1
    INNER JOIN Vendor AS v ON s.vendor_id=v.vendor_id AND v.is_active=1
WHERE
    s.vendor_id=@vendor_id
    AND s.is_active = 1
";
            }
            var param = new { vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result =  _connection.Query<StaffRoleModel>(twowaySql.Sql, dynParams).ToList();
            if (!result.Any())
            {
                throw new NotFoundException();
            }
            return result;
        }

        [CacheArg(allParam: true)]
        [CacheEntity(DynamicApiDatabase.TABLE_VENDORLINK)]
        [Cache]
        public IList<VendorLinkModel> GetVendorLinkList(string vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    vendor_link_id AS VendorLinkId
    ,title AS LinkTitle
    ,detail AS LinkDetail
    ,url AS LinkUrl
    ,is_visible AS IsVisible
    ,is_default AS IsDefault
    ,vendor_Id AS VendorId
    ,is_active AS IsActive
FROM
    VENDOR_LINK
Where
/*ds if vendor_id != null*/
    vendor_id= /*ds vendor_id*/'1' AND
/*ds end if*/
    is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    vendor_link_id AS VendorLinkId
    ,title AS LinkTitle
    ,detail AS LinkDetail
    ,url AS LinkUrl
    ,is_visible AS IsVisible
    ,is_default AS IsDefault
    ,vendor_Id AS VendorId
    ,is_active AS IsActive
FROM
    VendorLink
Where
/*ds if vendor_id != null*/
    vendor_id=@vendor_id AND
/*ds end if*/
    is_active=1
";
            }
            var param = new { vendor_id = vendorId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result =_dynamicConnection.Query<VendorLinkModel>(twowaySql.Sql, dynParams).ToList();
            if(!result.Any())
            {
                throw new NotFoundException();
            }
            return result;
        }

        [CacheArg(allParam: true)]
        [Cache]
        public VendorModel GetByOpenId(string openId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    v.vendor_id AS VendorId
	,v.vendor_name AS VendorName
	,v.is_data_offer AS IsDataOffer
	,v.is_data_use AS IsDataUse
FROM
    VENDOR v
WHERE
    v.vendor_id = (SELECT s.vendor_id FROM STAFF s WHERE s.account=/*ds account*/'1' AND s.is_active=1)
    AND v.is_enable = 1
    AND v.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    v.vendor_id AS VendorId
	,v.vendor_name AS VendorName
	,v.is_data_offer AS IsDataOffer
	,v.is_data_use AS IsDataUse
FROM
    Vendor v
WHERE
    v.vendor_id = (SELECT s.vendor_id FROM Staff s WHERE s.account=@account AND s.is_active=1)
    AND v.is_enable = 1
    AND v.is_active = 1
";
            }
            var param = new { account = openId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.QuerySingle<VendorModel>(twowaySql.Sql, dynParams);
        }

        [CacheArg(allParam: true)]
        [CacheEntity(DynamicApiDatabase.TABLE_VENDOROPENIDCA)]
        [Cache]
        public IList<OpenIdCaModel> GetVendorOpenIdCaListByVendorId(string vendorId)
            => GetOpenIdCaListByVendorId(new string[] { vendorId }, OpenIdType.Vendor);

        [CacheArg(allParam: true)]
        [CacheEntity(DynamicApiDatabase.TABLE_VENDOROPENIDCA)]
        [Cache]
        public IList<OpenIdCaModel> GetVendorOpenIdCaListByVendorIdList(IList<string> vendorId)
            => GetOpenIdCaListByVendorId(vendorId, OpenIdType.Vendor);

        [CacheArg(allParam: true)]
        [CacheEntity(DynamicApiDatabase.TABLE_VENDOROPENIDCA)]
        [Cache]
        public IList<OpenIdCaModel> GetControllerOpenIdCaListByVendorId(string controllerId)
            => GetOpenIdCaListByVendorId(new string[] { controllerId }, OpenIdType.Controller);

        [CacheArg(allParam: true)]
        [CacheEntity(DynamicApiDatabase.TABLE_VENDOROPENIDCA)]
        [Cache]
        public IList<OpenIdCaModel> GetApiOpenIdCaListByVendorId(string apiId)
            => GetOpenIdCaListByVendorId(new string[] { apiId }, OpenIdType.Api);

        [CacheArg(allParam: true)]
        [Cache]
        public IList<OpenIdCaModel> GetOpenIdCaListByVendorId(IList<string> id, OpenIdType type)
        {
            string tableName = string.Empty;
            string idColumnName = string.Empty;
            string relationIdColumnName = string.Empty;
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                switch (type)
                {
                    case OpenIdType.Vendor:
                        tableName = "VENDOR_OPEN_ID_CA";
                        idColumnName = "vendor_openid_ca_id";
                        relationIdColumnName = "vendor_id";
                        break;
                    case OpenIdType.Controller:
                        tableName = "CONTROLLER_OPENID_CA_ID";
                        idColumnName = "controller_openid_ca_id";
                        relationIdColumnName = "controller_id";
                        break;
                    case OpenIdType.Api:
                        tableName = "API_OPENID_CA_ID";
                        idColumnName = "api_openid_ca_id";
                        relationIdColumnName = "api_id";
                        break;
                    default:
                        break;
                }

                sql = @"
SELECT
    ca.application_id AS ApplicationId
    ,ca.application_name AS ApplicationName
    ,COALESCE(t.is_active, 1) AS IsActive
    ,t.{0} AS Id
    ,COALESCE(t.access_control, 'inh') AS AccessControl
FROM
    OPEN_ID_CERTIFICATION_AUTHORITY ca
    LEFT OUTER JOIN {1} t ON ca.application_id=t.application_id AND t.{2} in /*ds relationId*/'1' AND t.is_active=1
WHERE
    ca.is_active=1
ORDER  BY
    ca.application_id
";
            }
            else
            {
                switch (type)
                {
                    case OpenIdType.Vendor:
                        tableName = "VendorOpenIdCA";
                        idColumnName = "vendor_openid_ca_id";
                        relationIdColumnName = "vendor_id";
                        break;
                    case OpenIdType.Controller:
                        tableName = "ControllerOpenIdCA";
                        idColumnName = "controller_openid_ca_id";
                        relationIdColumnName = "controller_id";
                        break;
                    case OpenIdType.Api:
                        tableName = "ApiOpenIdCA";
                        idColumnName = "api_openid_ca_id";
                        relationIdColumnName = "api_id";
                        break;
                    default:
                        break;
                }

                sql = @"
SELECT
    ca.application_id AS ApplicationId
    ,ca.application_name AS ApplicationName
    ,ISNULL(t.is_active, '1') AS IsActive
    ,t.{0} AS Id
    ,ISNULL(t.access_control, 'inh') AS AccessControl
FROM
    OpenIdCertificationAuthority AS ca
    LEFT OUTER JOIN {1} AS t ON ca.application_id=t.application_id AND t.{2} in @relationId AND t.is_active=1
WHERE
    ca.is_active=1
ORDER  BY
    ca.application_id
";
            }

            sql = string.Format(sql, idColumnName, tableName, relationIdColumnName);

            var param = new { relationId = id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            // dynamicParameterで配列がうまく扱えない為、dynamicParameter化未対応
            var x = _dynamicConnection.Query<OpenIdCaModel>(twowaySql.Sql, param).ToList();
            return x;
        }

        [CacheEntityFire(AuthorityDatabase.TABLE_STAFF)]
        //[CacheIdFire("staf_id", "account")] // ResultModelからの発火が必要
        [DomainDataSync(ParameterType.Result, AuthorityDatabase.TABLE_STAFF, "StaffId")]
        public StaffModel AddStaff(string vendor_id, string account, string emailaddress, string? staffId = null)
        {
            var model  =new StaffModel()
            {
                VendorId = vendor_id,
                Account = account,
                EmailAddress = emailaddress,
                StaffId = staffId ?? Guid.NewGuid().ToString(),
            };
            try
            {
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
MERGE INTO STAFF a
    USING (SELECT  /*ds vendor_id*/'1' AS vendor_id, /*ds account*/'1' AS account, /*ds staff_id*/'1' AS staff_id FROM DUAL) b
    ON (a.staff_id=b.staff_id)
    WHEN MATCHED THEN
        UPDATE SET
            is_active=1, upd_username= /*ds open_id*/'1' , upd_date=SYSTIMESTAMP, email_address= /*ds emailaddress*/'1' 
    WHEN NOT MATCHED THEN
        INSERT (staff_id,account,vendor_id,reg_date,reg_username,upd_date,upd_username,is_active, email_address)
        VALUES ( /*ds staff_id*/'1' , /*ds account*/'1' , /*ds vendor_id*/'1' ,SYSTIMESTAMP, /*ds open_id*/'1' ,SYSTIMESTAMP, /*ds open_id*/'1' ,1, /*ds emailaddress*/'1' )
";
                }
                else
                {
                    sql = @"
MERGE INTO Staff AS a
    USING (SELECT @vendor_id AS vendor_id, @account AS account, @staff_id AS staff_id) AS b
    ON (a.staff_id=b.staff_id)
    WHEN MATCHED THEN
        UPDATE SET
            is_active=1, upd_username=@open_id, upd_date=GETDATE(), vendor_id=@vendor_id, email_address=@emailaddress
    WHEN NOT MATCHED THEN
        INSERT (staff_id,account,vendor_id,reg_date,reg_username,upd_date,upd_username,is_active, email_address)
        VALUES (@staff_id,@account,@vendor_id,GETDATE(),@open_id,GETDATE(),@open_id,1,@emailaddress);
";
                }
                var param = new { staff_id = model.StaffId, account = model.Account, vendor_id = model.VendorId, open_id = PerRequestDataContainer.OpenId, emailaddress = model.EmailAddress };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            }
            catch (SqlException ex)
            {
                throw (ex.Number == 2627) ? new AlreadyExistsException("指定されたアカウントは既に使われています", ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return model;
        }

        [CacheIdFire("staf_id", "model.StaffId")]
        [CacheEntityFire(AuthorityDatabase.TABLE_STAFF)]
        [DomainDataSync(ParameterType.Result, AuthorityDatabase.TABLE_STAFF, "StaffId")]
        public StaffModel UpdateStaff(StaffModel model)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    STAFF
SET
    account= /*ds account*/'1' 
    ,vendor_id= /*ds vendor_id*/'1' 
    ,upd_username= /*ds open_id*/'1' 
    ,email_address= /*ds mailaddress*/'1' 
WHERE
    staff_id= /*ds staff_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE
    Staff
SET
    account=@account
    ,vendor_id=@vendor_id
    ,upd_username=@open_id
    ,email_address=@mailaddress
WHERE
    staff_id=@staff_id
    AND is_active=1
";
            }

            var param = new { vendor_id = model.VendorId, account = model.Account, staff_id = model.StaffId, mailaddress = model.EmailAddress, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            try
            {
                _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547) throw new ForeignKeyException(ex.Message, ex);
                if (ex.Number == 2627) throw new AlreadyExistsException("指定されたアカウントは既に使われています", ex);
                throw new SqlDatabaseException(ex.Message, ex);
            }
            return model;
        }

        public string GetVendorIdByStaffAccount(string account)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    vendor_id
FROM
    STAFF
WHERE
    account= /*ds account*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    vendor_id
FROM
    Staff
WHERE
    account=@account
    AND is_active=1
";
            }
            var param = new { account = account };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.QuerySingle<string>(twowaySql.Sql, dynParams);
        }

        [DomainDataSync(AuthorityDatabase.TABLE_STAFF, "staff_id")]
        public void DeleteStaff(string staff_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    STAFF
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username= /*ds open_id*/'1' 
WHERE
    staff_id= /*ds staff_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    Staff
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    staff_id=@staff_id
    AND is_active=1
";
            }
            var param = new { staff_id = staff_id, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        public bool IsExistsStaffByAccount(string account)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = "SELECT staff_id FROM STAFF WHERE account= /*ds account*/'1' AND is_active=1";
            }
            else
            {
                sql = "SELECT staff_id FROM Staff WHERE account=@account AND is_active=1";
            }
            var param = new { account = account };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<string>(twowaySql.Sql, dynParams) == null ? false : true;
        }

        public bool IsExistsStaff(string staff_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = "SELECT staff_id FROM STAFF WHERE staff_id= /*ds staff_id*/'1' AND is_active=1";
            }
            else
            {
                sql = "SELECT staff_id FROM Staff WHERE staff_id=@staff_id AND is_active=1";
            }
            var param = new { staff_id = staff_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<string>(twowaySql.Sql, dynParams) == null ? false : true;
        }

        public StaffModel GetStaff(string staff_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    s.staff_id AS StaffId
    ,s.account AS Account
    ,s.vendor_id AS VendorId
    ,s.email_address AS EmailAddress
    ,sr.staff_role_id AS StaffRoleId
    ,r.role_id AS RoleId
FROM
    STAFF s
    INNER JOIN STAFF_ROLE sr ON s.staff_id=sr.staff_id AND sr.is_active=1
    INNER JOIN ROLE r ON sr.role_id=r.role_id AND r.is_active=1
WHERE
    s.staff_id=/*ds staff_id*/'1' 
    AND s.is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    s.staff_id AS StaffId
    ,s.account AS Account
    ,s.vendor_id AS VendorId
    ,s.email_address AS EmailAddress
    ,sr.staff_role_id AS StaffRoleId
    ,r.role_id AS RoleId
FROM
    Staff AS s
    INNER JOIN StaffRole AS sr ON s.staff_id=sr.staff_id AND sr.is_active=1
    INNER JOIN Role AS r ON sr.role_id=r.role_id AND r.is_active=1
WHERE
    s.staff_id=@staff_id
    AND s.is_active=1
";
            }
            var param = new { staff_id = staff_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.QuerySingle<StaffModel>(twowaySql.Sql, dynParams);
        }

        public StaffModel GetStaffByAccount(string account, bool isActive = true)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    s.staff_id AS StaffId
    ,s.account AS Account
    ,s.vendor_id AS VendorId
    ,s.email_address AS EmailAddress
    ,sr.staff_role_id AS StaffRoleId
    ,r.role_id AS RoleId
FROM
    STAFF s
    INNER JOIN STAFF_ROLE sr ON s.staff_id=sr.staff_id AND sr.is_active=/*ds is_active*/1 
    INNER JOIN ROLE r ON sr.role_id=r.role_id AND r.is_active=1
WHERE
    s.account= /*ds account*/'1' 
    AND s.is_active=/*ds is_active*/1 
";
            }
            else
            {
                sql = @"
SELECT
    s.staff_id AS StaffId
    ,s.account AS Account
    ,s.vendor_id AS VendorId
    ,s.email_address AS EmailAddress
    ,sr.staff_role_id AS StaffRoleId
    ,r.role_id AS RoleId
FROM
    Staff AS s
    INNER JOIN StaffRole AS sr ON s.staff_id=sr.staff_id AND sr.is_active=@is_active
    INNER JOIN Role AS r ON sr.role_id=r.role_id AND r.is_active=1
WHERE
    s.account=@account
    AND s.is_active=@is_active
";
            }
            var param = new { account = account, is_active = isActive ? "1" : "0" };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            // StaffRoleはstaff_idにunique制約がないのでSingleではなくFirstで取得
            return _connection.Query<StaffModel>(twowaySql.Sql, dynParams).FirstOrDefault();
        }

        public IList<StaffModel> GetStaffListByVendorId(string vendor_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    staff_id AS StaffId
    ,account AS Account
    ,vendor_id AS VendorId
    ,email_address AS EmailAddress
FROM
    STAFF
WHERE
/*ds if vendor_id != null*/
    vendor_id= /*ds vendor_id*/'1' AND
/*ds end if*/
    is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    staff_id AS StaffId
    ,account AS Account
    ,vendor_id AS VendorId
    ,email_address AS EmailAddress
FROM
    Staff
WHERE
/*ds if vendor_id != null*/
    vendor_id=@vendor_id AND
/*ds end if*/
    is_active=1
";
            }
            var param = new { vendor_id = vendor_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<StaffModel>(twowaySql.Sql, dynParams).ToList();
        }

        [DomainDataSync(ParameterType.Result, AuthorityDatabase.TABLE_STAFFROLE, ".")]
        public string AddStaffRole(string staffId, string roleId, string? staffRoleId = null)
        {
            staffRoleId ??= Guid.NewGuid().ToString();

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
                MERGE INTO STAFF_ROLE a
                    USING (SELECT  /*ds staff_role_id*/'1' AS staff_role_id,  /*ds staff_id*/'1'  AS staff_id,  /*ds role_id*/'1'  AS role_id FROM DUAL) b
                    ON (a.staff_role_id=b.staff_role_id)
                    WHEN MATCHED THEN
                        UPDATE SET
                            is_active=1, upd_username= /*ds open_id*/'1' , upd_date=SYSTIMESTAMP, role_id= /*ds role_id*/'1' 
                    WHEN NOT MATCHED THEN
                        INSERT (staff_role_id,staff_id,role_id,reg_date,reg_username,upd_date,upd_username,is_active)
                        VALUES ( /*ds staff_role_id*/'1' , /*ds staff_id*/'1' , /*ds role_id*/'1' ,SYSTIMESTAMP, /*ds open_id*/'1' ,SYSTIMESTAMP, /*ds open_id*/'1' ,1)
                ";
            }
            else
            {
                sql = @"
                MERGE INTO StaffRole AS a
                    USING (SELECT @staff_role_id AS staff_role_id, @staff_id AS staff_id, @role_id AS role_id) AS b
                    ON (a.staff_role_id=b.staff_role_id)
                    WHEN MATCHED THEN
                        UPDATE SET
                            is_active=1, upd_username=@open_id, upd_date=GETDATE(), role_id=@role_id
                    WHEN NOT MATCHED THEN
                        INSERT (staff_role_id,staff_id,role_id,reg_date,reg_username,upd_date,upd_username,is_active)
                        VALUES (@staff_role_id,@staff_id,@role_id,GETDATE(),@open_id,GETDATE(),@open_id,1);
                ";
            }
            var param = new { staff_role_id = staffRoleId, staff_id = staffId, role_id = roleId, open_id = PerRequestDataContainer.OpenId};
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);

            return staffRoleId;
        }

        [DomainDataSync(AuthorityDatabase.TABLE_STAFFROLE, "staffRoleId")]
        public void DeleteStaffRole(string staffRoleId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    STAFF_ROLE
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username=/*ds open_id*/'1' 
WHERE
    staff_role_id=/*ds staff_role_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    StaffRole
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    staff_role_id=@staff_role_id
    AND is_active=1
";
            }
            var param = new { staff_role_id = staffRoleId, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_VENDORLINK)]
        [DomainDataSync(ParameterType.Result, DynamicApiDatabase.TABLE_VENDORLINK, "VendorLinkId")]
        public IList<VendorLinkModel> RegisterVendorLink(IList<RegisterVendorLinkModel> model)
        {
            try
            {
                model.ForEach(x =>
                {
                    var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                    var sql = "";

                    if (dbSettings.Type == "Oracle")
                    {
                        sql = @"
MERGE INTO VENDOR_LINK a
    USING (SELECT  /*ds vendor_link_id*/'1' AS vendor_link_id FROM DUAL) b
    ON (a.vendor_link_id=b.vendor_link_id)
    WHEN MATCHED THEN
        UPDATE SET
            vendor_id= /*ds vendor_id*/'1' 
            ,title= /*ds title*/'1' 
            ,detail= /*ds detail*/'1' 
            ,url= /*ds url*/'1' 
            ,is_visible= /*ds is_visible*/1 
            ,is_default= /*ds is_default*/1 
            ,upd_date= /*ds now*/systimestamp 
            ,upd_username= /*ds openid*/'1' 
    WHEN NOT MATCHED THEN
        INSERT (vendor_link_id,vendor_id,title,detail,url,is_visible,is_default,reg_date,reg_username,upd_date,upd_username,is_active)
        VALUES ( /*ds vendor_link_id*/'1' , /*ds vendor_id*/'1' , /*ds title*/'1' , /*ds detail*/'1' , /*ds url*/'1' , /*ds is_visible*/1 , /*ds is_default*/1 , /*ds now*/systimestamp , /*ds openid*/'1' , /*ds now*/systimestamp , /*ds openid*/'1' ,1)
";
                    }
                    else
                    {
                        sql = @"
MERGE INTO VendorLink AS a
    USING (SELECT @vendor_link_id AS vendor_link_id) AS b
    ON (a.vendor_link_id=b.vendor_link_id)
    WHEN MATCHED THEN
        UPDATE SET
            vendor_id=@vendor_id
            ,title=@title
            ,detail=@detail
            ,url=@url
            ,is_visible=@is_visible
            ,is_default=@is_default
            ,upd_date=@now
            ,upd_username=@openid
    WHEN NOT MATCHED THEN
        INSERT (vendor_link_id,vendor_id,title,detail,url,is_visible,is_default,reg_date,reg_username,upd_date,upd_username,is_active)
        VALUES (@vendor_link_id,@vendor_id,@title,@detail,@url,@is_visible,@is_default,@now,@openid,@now,@openid,1);
";
                    }

                    if (x.VendorLinkId == null)
                    {
                        x.VendorLinkId = Guid.NewGuid().ToString();
                    }
                    var param = new
                    {
                        vendor_link_id = x.VendorLinkId,
                        vendor_id = x.VendorId,
                        title = x.LinkTitle,
                        detail = x.LinkDetail,
                        url = x.LinkUrl,
                        is_visible = x.IsVisible,
                        is_default = x.IsDefault,
                        now = UtcNow,
                        openid = PerRequestDataContainer.OpenId,
                    };
                    var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                    var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                    _dynamicConnection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
                });
            }
            catch (SqlException ex)
            {
                throw ex.Number == 547 ? new ForeignKeyException(ex.Message, ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return s_mapper.Map<IList<VendorLinkModel>>(model);
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_VENDORLINK)]
        [CacheIdFire("vendorLinkId", "vendorLinkId")]
        [DomainDataSync(DynamicApiDatabase.TABLE_VENDORLINK, "vendorLinkId")]
        public void DeleteVendorLink(string vendorLinkId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    VENDOR_LINK
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username= /*ds open_id*/'1' 
 WHERE
    vendor_link_id= /*ds vendor_link_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    VendorLink
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    vendor_link_id=@vendor_link_id
    AND is_active=1
";
            }
            var param = new { vendor_link_id = vendorLinkId, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            _dynamicConnection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        public VendorLinkModel GetVendorLink(string vendorLinkId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    vendor_link_id AS VendorLinkId
    ,vendor_id AS VendorId
    ,title AS LinkTitle
    ,detail AS LinkDetail
    ,url AS LinkUrl
    ,is_visible AS IsVisible
    ,is_default AS IsDefault
    ,is_active AS IsActive
FROM
    VENDOR_LINK
WHERE
    vendor_link_id= /*ds vendor_link_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    vendor_link_id AS VendorLinkId
    ,vendor_id AS VendorId
    ,title AS LinkTitle
    ,detail AS LinkDetail
    ,url AS LinkUrl
    ,is_visible AS IsVisible
    ,is_default AS IsDefault
    ,is_active AS IsActive
FROM
        VendorLink
WHERE
    vendor_link_id=@vendor_link_id
    AND is_active=1
";
            }
            var param = new { vendor_link_id = vendorLinkId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _dynamicConnection.QuerySingle<VendorLinkModel>(twowaySql.Sql, dynParams);
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_VENDOROPENIDCA)]
        [DomainDataSync(ParameterType.Result, DynamicApiDatabase.TABLE_VENDOROPENIDCA, "Id")]
        public IList<RegisterOpenIdCaModel> RegisterVendorOpenIdCaList(IList<RegisterOpenIdCaModel> model)
        {
            try
            {
                model.ForEach(x =>
                {
                    var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                    var sql = "";

                    if (dbSettings.Type == "Oracle")
                    {
                        sql = @"
MERGE INTO VENDOR_OPEN_ID_CA a
    USING (SELECT /*ds vendor_openid_ca_id*/'1' AS vendor_openid_ca_id FROM DUAL) b
    ON (a.vendor_openid_ca_id=b.vendor_openid_ca_id)
    WHEN MATCHED THEN
        UPDATE SET
            vendor_id=/*ds vendor_id*/'1' 
            ,application_id=/*ds application_id*/'1' 
            ,access_control=/*ds access_control*/'1' 
            ,upd_date=/*ds now*/systimestamp 
            ,upd_username=/*ds openid*/'1' 
    WHEN NOT MATCHED THEN
        INSERT (vendor_openid_ca_id,vendor_id,application_id,access_control,reg_date,reg_username,upd_date,upd_username,is_active)
        VALUES (/*ds vendor_openid_ca_id*/'1' ,/*ds vendor_id*/'1' ,/*ds application_id*/'1' ,/*ds access_control*/'1' ,/*ds now*/systimestamp ,/*ds openid*/'1' ,/*ds now*/systimestamp ,/*ds openid*/'1' ,1)
";
                    }
                    else
                    {
                        sql = @"
MERGE INTO VendorOpenIdCA AS a
    USING (SELECT @vendor_openid_ca_id AS vendor_openid_ca_id) AS b
    ON (a.vendor_openid_ca_id=b.vendor_openid_ca_id)
    WHEN MATCHED THEN
        UPDATE SET
            vendor_id=@vendor_id
            ,application_id=@application_id
            ,access_control=@access_control
            ,upd_date=@now
            ,upd_username=@openid
    WHEN NOT MATCHED THEN
        INSERT (vendor_openid_ca_id,vendor_id,application_id,access_control,reg_date,reg_username,upd_date,upd_username,is_active)
        VALUES (@vendor_openid_ca_id,@vendor_id,@application_id,@access_control,@now,@openid,@now,@openid,1);
";
                    }

                    if (x.VendorOpenidCaId == null)
                    {
                        x.VendorOpenidCaId = Guid.NewGuid();
                    }
                    var param = new
                    {
                        vendor_openid_ca_id = x.VendorOpenidCaId,
                        vendor_id = x.VendorId,
                        application_id = x.ApplicationId,
                        access_control = x.AccessControl,
                        now = UtcNow,
                        openid = PerRequestDataContainer.OpenId,
                    };
                    var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                    var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                    _dynamicConnection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
                });
                return model;
            }
            catch (SqlException ex)
            {
                throw ex.Number == 547? throw new ForeignKeyException(ex.Message, ex) : new SqlDatabaseException(ex.Message, ex);
            }
        }

        [CacheEntityFire(DynamicApiDatabase.TABLE_VENDOROPENIDCA)]
        [DomainDataSync(DynamicApiDatabase.TABLE_VENDOROPENIDCA, "vendorOpenidCaId")]
        public void DeleteVendorOpenIdCa(string vendorId, string vendorOpenidCaId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    VENDOR_OPEN_ID_CA
SET 
    is_active=0
    ,upd_date=SYSTIMESTAMP
    ,upd_username= /*ds open_id*/'1' 
 WHERE 
/*ds if vendor_id != null*/
    vendor_id= /*ds vendor_id*/'1' AND
/*ds end if*/
    vendor_openid_ca_id= /*ds vendor_openid_ca_id*/'1' AND
    is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    VendorOpenIdCA
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@open_id
WHERE
    vendor_openid_ca_id=@vendor_openid_ca_id AND
/*ds if vendor_id != null*/
    vendor_id=@vendor_id AND
/*ds end if*/
    is_active=1
";
            }
            var param = new { vendor_id = vendorId, vendor_openid_ca_id = vendorOpenidCaId, open_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            _dynamicConnection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        private bool ExistsVendorName(string vendorName, string vendorId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var vendorNameSql = "";

            if (dbSettings.Type == "Oracle")
            {
                vendorNameSql = @"SELECT COUNT(vendor_name) FROM VENDOR /*WITH(UPDLOCK)*/ WHERE vendor_name = /*ds VendorName*/'1' AND is_active = 1";
            }
            else
            {
                vendorNameSql = @"SELECT COUNT(vendor_name) FROM Vendor WITH(UPDLOCK) WHERE vendor_name = @VendorName AND is_active = 1";
            }

            // VendorIdを指定した場合は自信を除く
            if (!string.IsNullOrEmpty(vendorId))
            {
                if (dbSettings.Type == "Oracle")
                {
                    vendorNameSql += " AND vendor_id <> /*ds VendorId*/'1' ";
                }
                else
                {
                    vendorNameSql += " AND vendor_id <> @VendorId";
                }

                var param = new { VendorName = vendorName, VendorId = vendorId };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), vendorNameSql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                return _connection.QuerySingle<int>(twowaySql.Sql, dynParams) > 0;
            }
            else
            {
                var param = new { VendorName = vendorName };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), vendorNameSql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                return _connection.QuerySingle<int>(twowaySql.Sql, dynParams) > 0;
            }
        }
    }
}
