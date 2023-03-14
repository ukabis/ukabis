using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using AutoMapper;
using System.Threading.Tasks;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.ManageApi.Service.Repository.Model;
using JP.DataHub.Infrastructure.Database.Document;
using JP.DataHub.Api.Core.Database;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class DocumentRepository : AbstractRepository, IDocumentRepository
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DocumentModel, DB_Document>()
                    .ForMember(dst => dst.document_id, ops => ops.MapFrom(src => src.DocumentId))
                    .ForMember(dst => dst.title, ops => ops.MapFrom(src => src.Title))
                    .ForMember(dst => dst.detail, ops => ops.MapFrom(src => src.Detail))
                    .ForMember(dst => dst.category_id, ops => ops.MapFrom(src => src.CategoryId))
                    .ForMember(dst => dst.vendor_id, ops => ops.MapFrom(src => src.VendorId))
                    .ForMember(dst => dst.system_id, ops => ops.MapFrom(src => src.SystemId))
                    .ForMember(dst => dst.is_enable, ops => ops.MapFrom(src => src.IsEnable))
                    .ForMember(dst => dst.is_admin_check, ops => ops.MapFrom(src => src.IsAdminCheck))
                    .ForMember(dst => dst.is_admin_stop, ops => ops.MapFrom(src => src.IsAdminStop))
                    .ForMember(dst => dst.agreement_id, ops => ops.MapFrom(src => src.AgreementId))
                    .ForMember(dst => dst.is_public_portal, ops => ops.MapFrom(src => src.IsPublicPortal))
                    .ForMember(dst => dst.is_public_admin, ops => ops.MapFrom(src => src.IsPublicAdmin))
                    .ForMember(dst => dst.is_public_portal_hidden, ops => ops.MapFrom(src => src.IsPublicPortalHidden))
                    .ForMember(dst => dst.is_public_admin_hidden, ops => ops.MapFrom(src => src.IsPublicAdminHidden))
                    .ForMember(dst => dst.password, ops => ops.MapFrom(src => src.Password))
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
                cfg.CreateMap<DB_File, FileModel>()
                    .ForMember(dst => dst.FileId, ops => ops.MapFrom(src => src.file_id))
                    .ForMember(dst => dst.Title, ops => ops.MapFrom(src => src.title))
                    .ForMember(dst => dst.Url, ops => ops.MapFrom(src => src.url))
                    .ForMember(dst => dst.IsEnable, ops => ops.MapFrom(src => src.is_enable))
                    .ForMember(dst => dst.UpdDate, ops => ops.MapFrom(src => src.upd_date))
                    .ForMember(dst => dst.IsActive, ops => ops.MapFrom(src => src.is_active))
                    .ForMember(dst => dst.OrderNo, ops => ops.MapFrom(src => src.order_no));
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Document"));
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }

        public DocumentInformationModel GetDocumentInformation(string documentId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
	d.document_id AS DocumentId
    ,d.title AS Title
    ,d.detail AS Detail
    ,c.category_id AS CategoryId
    ,c.category_name AS CategoryName
    ,d.vendor_id AS VendorId
    ,d.system_id AS SystemId
    ,d.is_enable AS IsEnable
    ,d.is_admin_check AS IsAdminCheck
    ,d.is_admin_stop AS IsAdminStop
    ,d.agreement_id AS AgreementId
    ,d.upd_date AS UpdDate
    ,d.is_active AS IsActive
    ,d.is_public_portal AS IsPublicPortal
    ,d.is_public_admin AS IsPublicAdmin
    ,d.is_public_portal_hidden AS IsPublicPortalHidden
    ,d.is_public_admin_hidden AS IsPublicAdminHidden
    ,d.password AS Password
FROM
    DOCUMENT d
    INNER JOIN CATEGORY c ON d.category_id = c.category_id 
WHERE
    d.document_id= /*ds documentId*/'1' 
    AND d.is_active=1
ORDER BY
    title ASC
";
            }
            else
            {
                sql = @"
SELECT
	d.document_id AS DocumentId
    ,d.title AS Title
    ,d.detail AS Detail
    ,c.category_id AS CategoryId
    ,c.category_name AS CategoryName
    ,d.vendor_id AS VendorId
    ,d.system_id AS SystemId
    ,d.is_enable AS IsEnable
    ,d.is_admin_check AS IsAdminCheck
    ,d.is_admin_stop AS IsAdminStop
    ,d.agreement_id AS AgreementId
    ,d.upd_date AS UpdDate
    ,d.is_active AS IsActive
    ,d.is_public_portal AS IsPublicPortal
    ,d.is_public_admin AS IsPublicAdmin
    ,d.is_public_portal_hidden AS IsPublicPortalHidden
    ,d.is_public_admin_hidden AS IsPublicAdminHidden
    ,d.password AS Password
FROM
    Document d
    INNER JOIN Category c ON d.category_id = c.category_id 
WHERE
    d.document_id=@documentId
    AND d.is_active=1
ORDER BY
    title ASC
";
            }

            var param = new { documentId = documentId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = _connection.Query<DocumentInformationModel>(twowaySql.Sql, dynParams).FirstOrDefault();

            if (result == null)
            {
                throw new NotFoundException($"Not Found DocumentId={documentId}");
            }
            return result;

        }

        public IList<DocumentInformationModel> GetDocumentInformation(bool isPublicPortal, bool isPublicAdmin)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
	d.document_id AS DocumentId
    ,d.title AS Title
    ,d.detail AS Detail
    ,c.category_id AS CategoryId
    ,c.category_name AS CategoryName
    ,d.vendor_id AS VendorId
    ,d.system_id AS SystemId
    ,d.is_enable AS IsEnable
    ,d.is_admin_check AS IsAdminCheck
    ,d.is_admin_stop AS IsAdminStop
    ,d.agreement_id AS AgreementId
    ,d.upd_date AS UpdDate
    ,d.is_active AS IsActive
    ,d.is_public_portal AS IsPublicPortal
    ,d.is_public_admin AS IsPublicAdmin
    ,d.is_public_portal_hidden AS IsPublicPortalHidden
    ,d.is_public_admin_hidden AS IsPublicAdminHidden
    ,d.password AS Password
FROM
    DOCUMENT d
    INNER JOIN CATEGORY c ON d.category_id = c.category_id 
WHERE
/*ds if isPublicPortal != null*/
    is_public_portal=/*ds is_public_portal*/'1' AND
/*ds end if*/
/*ds if isPublicAdmin != null*/
    is_public_admin=/*ds isPublicAdmin*/'1' AND
/*ds end if*/
    d.is_active=1
ORDER BY
    title ASC
";
            }
            else
            {
                sql = @"
SELECT
	d.document_id AS DocumentId
    ,d.title AS Title
    ,d.detail AS Detail
    ,c.category_id AS CategoryId
    ,c.category_name AS CategoryName
    ,d.vendor_id AS VendorId
    ,d.system_id AS SystemId
    ,d.is_enable AS IsEnable
    ,d.is_admin_check AS IsAdminCheck
    ,d.is_admin_stop AS IsAdminStop
    ,d.agreement_id AS AgreementId
    ,d.upd_date AS UpdDate
    ,d.is_active AS IsActive
    ,d.is_public_portal AS IsPublicPortal
    ,d.is_public_admin AS IsPublicAdmin
    ,d.is_public_portal_hidden AS IsPublicPortalHidden
    ,d.is_public_admin_hidden AS IsPublicAdminHidden
    ,d.password AS Password
FROM
    Document d
    INNER JOIN Category c ON d.category_id = c.category_id 
WHERE
/*ds if isPublicPortal != null*/
    is_public_portal=/*ds is_public_portal*/'1' AND
/*ds end if*/
/*ds if isPublicAdmin != null*/
    is_public_admin=/*ds isPublicAdmin*/'1' AND
/*ds end if*/
    d.is_active=1
ORDER BY
    title ASC
";
            }

            var param = new { isPublicPortal = isPublicPortal ? "1" : null, isPublicAdmin = isPublicAdmin ? "1" : null };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = _connection.Query<DocumentInformationModel>(twowaySql.Sql, dynParams).ToList();

            var sql_File = "";
            if (dbSettings.Type == "Oracle")
            {
                sql_File = @"
SELECT
	file_id
    ,document_id
    ,title
    ,url
    ,is_enable
    ,upd_date
    ,is_active
    ,order_no
FROM
    FILES
WHERE
    document_id in /*ds documentId*/'1' 
ORDER BY 
    order_no ASC
    ,title ASC
";
            }
            else
            {
                sql_File = @"
SELECT
	file_id
    ,document_id
    ,title
    ,url
    ,is_enable
    ,upd_date
    ,is_active
    ,order_no
FROM
    [File]
WHERE
    document_id in @documentId
ORDER BY 
    order_no ASC
    ,title ASC
";
            }
            var param_File = new { documentId = "" };
            var twowaySql_File = new TwowaySqlParser(dbSettings.GetDbType(), sql_File, param_File);
            dynParams = dbSettings.GetParameters().AddDynamicParams(param_File);
            var files = _connection.Query<DB_File>(twowaySql_File.Sql, dynParams).ToList();
            result.ForEach(x =>
            {
                x.FileList = s_mapper.Map<IList<FileModel>>(files.Where(f => f.document_id?.ToString() == x.DocumentId).ToList());
            });
            return result;
        }

        [CacheEntity(DocumentDatabase.TABLE_CATEGORY)]
        [Cache]
        public IList<DocumentCategoryModel> GetDocumentCategory()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    category_id AS CategoryId
    ,category_name AS CategoryName
FROM
    CATEGORY
WHERE
    is_active=1
ORDER BY
    order_no
";
            }
            else
            {
                sql = @"
SELECT
    category_id AS CategoryId
    ,category_name AS CategoryName
FROM
    Category
WHERE
    is_active=1
ORDER BY
    order_no
";
            }

            return _connection.Query<DocumentCategoryModel>(sql).ToList();
        }

        public IList<DocumentModel> GetDocumentList(string vendorId, bool? isEnable = null, bool? isAdminCheck = null, bool? isAdminStop = null, bool? isActive = null,
            bool? isPublicPortal = null, bool? isPublicAdmin = null, bool? isPublicPortalHidden = null, bool? isPublicAdminHidden = null)
        {
            var param = new
            {
                VendorId = vendorId,
                IsEnable = isEnable,
                IsAdminCheck = isAdminCheck,
                IsAdminStop = isAdminStop,
                IsActive = isActive,
                IsPublicPortal = isPublicPortal,
                IsPublicAdmin = isPublicAdmin,
                IsPublicPortalHidden = isPublicPortalHidden,
                IsPublicAdminHidden = isPublicAdminHidden,
            };
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sqlIpFilter = "";
            if (dbSettings.Type == "Oracle")
            {
                sqlIpFilter = @"
SELECT
	document_id AS DocumentId
	,title AS Title
	,detail AS Detail
	,category_id AS CategoryId
	,vendor_id AS VendorId
	,system_id AS SystemId
	,is_enable AS IsEnable
	,is_admin_check AS IsAdminCheck
	,is_admin_stop AS IsAdminStop
	,agreement_id AS AgreementId
    ,upd_date AS LastUpdDate
	,is_active AS IsActive
	,is_public_portal AS IsPublicPortal
	,is_public_admin AS IsPublicAdmin
    ,is_public_portal_hidden AS IsPublicPortalHidden
    ,is_public_admin_hidden AS IsPublicAdminHidden
    ,password AS Password
FROM
	DOCUMENT
WHERE
/*ds if IsEnable != null*/
     is_enable = /*ds IsEnable*/'1' AND 
/*ds end if*/
/*ds if IsAdminCheck != null*/
     is_admin_check = /*ds IsAdminCheck*/'1' AND 
/*ds end if*/
/*ds if IsAdminStop != null*/
     is_admin_stop = /*ds IsAdminStop*/'1' AND 
/*ds end if*/
/*ds if IsActive != null*/
     is_active = /*ds IsActive*/'1' AND 
/*ds end if*/
/*ds if IsPublicPortal != null*/
     is_public_portal = /*ds IsPublicPortal*/'1' AND 
/*ds end if*/
/*ds if IsPublicAdmin != null*/
     is_public_admin = /*ds IsPublicAdmin*/'1' AND 
/*ds end if*/
/*ds if IsPublicPortalHidden != null*/
     is_public_portal_hidden = /*ds IsPublicPortalHidden*/'0' AND 
/*ds end if*/
/*ds if IsPublicAdminHidden != null*/
     is_public_admin_hidden = /*ds IsPublicAdminHidden*/'0' AND 
/*ds end if*/
/*ds if VendorId != null*/
     vendor_id = /*ds VendorId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
	is_active = 1
ORDER BY
    title
";
            }
            else
            {
                sqlIpFilter = @"
SELECT
	document_id AS DocumentId
	,title AS Title
	,detail AS Detail
	,category_id AS CategoryId
	,vendor_id AS VendorId
	,system_id AS SystemId
	,is_enable AS IsEnable
	,is_admin_check AS IsAdminCheck
	,is_admin_stop AS IsAdminStop
	,agreement_id AS AgreementId
    ,upd_date AS LastUpdDate
	,is_active AS IsActive
	,is_public_portal AS IsPublicPortal
	,is_public_admin AS IsPublicAdmin
    ,is_public_portal_hidden AS IsPublicPortalHidden
    ,is_public_admin_hidden AS IsPublicAdminHidden
    ,password AS Password
FROM
	Document
WHERE
/*ds if IsEnable != null*/
     is_enable = /*ds IsEnable*/'1' AND 
/*ds end if*/
/*ds if IsAdminCheck != null*/
     is_admin_check = /*ds IsAdminCheck*/'1' AND 
/*ds end if*/
/*ds if IsAdminStop != null*/
     is_admin_stop = /*ds IsAdminStop*/'1' AND 
/*ds end if*/
/*ds if IsActive != null*/
     is_active = /*ds IsActive*/'1' AND 
/*ds end if*/
/*ds if IsPublicPortal != null*/
     is_public_portal = /*ds IsPublicPortal*/'1' AND 
/*ds end if*/
/*ds if IsPublicAdmin != null*/
     is_public_admin = /*ds IsPublicAdmin*/'1' AND 
/*ds end if*/
/*ds if IsPublicPortalHidden != null*/
     is_public_portal_hidden = /*ds IsPublicPortalHidden*/'0' AND 
/*ds end if*/
/*ds if IsPublicAdminHidden != null*/
     is_public_admin_hidden = /*ds IsPublicAdminHidden*/'0' AND 
/*ds end if*/
/*ds if VendorId != null*/
     vendor_id = /*ds VendorId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
	is_active = 1
ORDER BY
    title
";
            }

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sqlIpFilter, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<DocumentModel>(twowaySql.Sql, dynParams).ToList();
        }

        [DomainDataSync(DocumentDatabase.TABLE_DOCUMENT, "model.DocumentId")]
        public DocumentModel Register(DocumentModel model, IList<DocumentFileModel> files = null)
        {
            try
            {
                if (string.IsNullOrEmpty(model.DocumentId))
                {
                    model.DocumentId = Guid.NewGuid().ToString();
                }
                _connection.Insert(s_mapper.Map<DB_Document>(model));
                files?.ForEach(x => _connection.Insert(s_mapper.Map<DB_File>(x)));
            }
            catch (SqlException ex)
            {
                throw ex.Number == 547 ? new ForeignKeyException(ex.Message, ex) : new SqlDatabaseException(ex.Message, ex);
            }
            return model;
        }

        [DomainDataSync(DocumentDatabase.TABLE_DOCUMENT, "model.DocumentId")]
        public DocumentModel Update(DocumentModel model, IList<DocumentFileModel> files = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE
    DOCUMENT
SET
    title = /*ds title*/'1' 
	,detail = /*ds detail*/'1' 
	,category_id = /*ds category_id*/'1' 
	,vendor_id = /*ds vendor_id*/'1' 
	,system_id = /*ds system_id*/'1' 
	,is_enable = /*ds is_enable*/1 
	,is_admin_check = /*ds is_admin_check*/1 
	,is_admin_stop = /*ds is_admin_stop*/1 
	,agreement_id = /*ds agreement_id*/'1' 
    ,upd_date = SYS_EXTRACT_UTC(SYSTIMESTAMP)
    ,upd_username = /*ds upd_username*/'1' 
	,is_public_portal = /*ds is_public_portal*/1 
	,is_public_admin = /*ds is_public_admin*/1 
	,is_public_portal_hidden = /*ds is_public_portal_hidden*/1 
	,is_public_admin_hidden = /*ds is_public_admin_hidden*/1 
	,password = /*ds password*/'1' 
WHERE
    document_id = /*ds document_id*/'1' 
    AND is_active = 1
";
            }
            else
            {
                sql = @"
UPDATE
    Document
SET
    title = @title
	,detail = @detail
	,category_id = @category_id
	,vendor_id = @vendor_id
	,system_id = @system_id
	,is_enable = @is_enable
	,is_admin_check = @is_admin_check
	,is_admin_stop = @is_admin_stop
	,agreement_id = @agreement_id
    ,upd_date = GETDATE()
    ,upd_username = @upd_username
	,is_public_portal = @is_public_portal
	,is_public_admin = @is_public_admin
	,is_public_portal_hidden = @is_public_portal_hidden
	,is_public_admin_hidden = @is_public_admin_hidden
	,password = @password
WHERE
    document_id = @document_id
    AND is_active = 1
";
            }
            var param = new
            {
                document_id = model.DocumentId.To<Guid>(),
                vendor_id = model.VendorId.To<Guid>(),
                system_id = model.SystemId.To<Guid>(),
                agreement_id = string.IsNullOrEmpty(model.AgreementId) ? null : (Guid?)model.AgreementId.To<Guid>(),
                category_id = model.CategoryId.To<Guid>(),
                title = model.Title,
                detail = model.Detail,
                is_enable = model.IsEnable,
                is_admin_check = model.IsAdminCheck,
                is_admin_stop = model.IsAdminStop,
                upd_date = UtcNow,
                upd_username = PerRequestDataContainer.OpenId,
                is_active = model.IsActive,
                is_public_portal = model.IsPublicPortal,
                is_public_admin = model.IsPublicAdmin,
                is_public_portal_hidden = model.IsPublicPortalHidden,
                is_public_admin_hidden = model.IsPublicAdminHidden,
                password = model.Password,
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);

            try
            {
                var result = _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
                if (result <= 0)
                {
                    throw new NotFoundException($"Not Found DocumentId={model.DocumentId}");
                }
            }
            catch (SqlException ex)
            {
                throw ex.Number == 547 ? new ForeignKeyException(ex.Message, ex) : new SqlDatabaseException(ex.Message, ex);
            }

            return model;
        }

        [DomainDataSync(DocumentDatabase.TABLE_DOCUMENT, "documentId")]
        public void DeleteDocument(string documentId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
UPDATE 
    DOCUMENT
SET 
    is_active=0
    ,upd_date=SYS_EXTRACT_UTC(SYSTIMESTAMP)
    ,upd_username= /*ds staff_id*/'1' 
WHERE
    document_id= /*ds document_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
UPDATE 
    Document
SET 
    is_active=0
    ,upd_date=GETDATE()
    ,upd_username=@staff_id
WHERE
    document_id=@document_id
    AND is_active=1
";
            }
            var param = new { document_id = documentId, staff_id = PerRequestDataContainer.OpenId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }

        public IList<string> GetDocumentOwnerEmailAddressList(string documentId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    s.email_address
FROM
    DOCUMENT d
    INNER JOIN STAFF s ON s.account IN (d.reg_username, d.upd_username)
WHERE
    d.document_id = /*ds document_Id*/'1' 
    AND d.vendor_id = s.vendor_id
    AND COALESCE(s.email_address, '') <> ''
    AND d.is_active = 1
    AND s.is_active = 1
";
            }
            else
            {
                sql = @"
SELECT
    s.email_address
FROM
    Document d
    INNER JOIN Staff s ON s.account IN (d.reg_username, d.upd_username)
WHERE
    d.document_id = @document_Id
    AND d.vendor_id = s.vendor_id
    AND ISNULL(s.email_address, '') <> ''
    AND d.is_active = 1
    AND s.is_active = 1
";
            }
            var param = new { document_Id = documentId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<DB_Staff>(twowaySql.Sql, dynParams).Select(x => x.email_address).ToList();
        }

        public IList<DocumentFileModel> GetDocumentFileList(string documentId, string documentTitle)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
	file_id AS FileId
	,document_id AS DocumentId
    ,title AS Title
    ,url AS Url
    ,is_enable AS IsEnable
    ,is_active AS IsActive
    ,file_update_date AS FileUpdateDate
    ,html_link AS HtmlLink
    ,order_no AS OrderNo
FROM 
	files
WHERE
	is_active = 1
    AND document_id = /*ds document_id*/'1' 
ORDER BY 
    order_no ASC
    ,title ASC
";
            }
            else
            {
                sql = @"
SELECT 
	file_id AS FileId
	,document_id AS DocumentId
    ,title AS Title
    ,url AS Url
    ,is_enable AS IsEnable
    ,is_active AS IsActive
    ,file_update_date AS FileUpdateDate
    ,html_link AS HtmlLink
    ,order_no AS OrderNo
FROM 
	[File]
WHERE
	is_active = 1
    AND document_id = @document_id
ORDER BY 
    order_no ASC
    ,title ASC
";
            }
            var param = new { document_id = documentId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = _connection.Query<DocumentFileModel>(twowaySql.Sql, dynParams).ToList();
            result.ForEach(x => x.DocumentTitle = documentTitle);
            return result;
        }

        public VendorSystemCategoryNameResultModel GetVendorSystemCategoryName(string vendorId, string systemId, string categoryId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    (SELECT vendor_id FROM VENDOR WHERE vendor_id= /*ds vendor_id*/'1' AND is_active=1) AS VendorId
    ,(SELECT vendor_name FROM VENDOR WHERE vendor_id= /*ds vendor_id*/'1' AND is_active=1) AS VendorName
    ,(SELECT system_id FROM SYSTEM WHERE vendor_id= /*ds vendor_id*/'1' AND system_id= /*ds system_id*/'1' AND is_active=1) AS SystemId
    ,(SELECT system_name FROM SYSTEM WHERE vendor_id= /*ds vendor_id*/'1' AND system_id= /*ds system_id*/'1' AND is_active=1) AS SystemName
    ,(SELECT category_id FROM CATEGORY WHERE category_id= /*ds category_id*/'1' AND is_active=1) AS CategoryId
    ,(SELECT category_name FROM CATEGORY WHERE category_id= /*ds category_id*/'1' AND is_active=1) AS CategoryName
FROM DUAL
";
            }
            else
            {
                sql = @"
SELECT
    (SELECT vendor_id FROM vendor WHERE vendor_id=@vendor_id AND is_active=1) AS VendorId
    ,(SELECT vendor_name FROM vendor WHERE vendor_id=@vendor_id AND is_active=1) AS VendorName
    ,(SELECT system_id FROM [system] WHERE vendor_id=@vendor_id AND system_id=@system_id AND is_active=1) AS SystemId
    ,(SELECT system_name FROM [system] WHERE vendor_id=@vendor_id AND system_id=@system_id AND is_active=1) AS SystemName
    ,(SELECT category_id FROM category WHERE category_id=@category_id AND is_active=1) AS CategoryId
    ,(SELECT category_name FROM category WHERE category_id=@category_id AND is_active=1) AS CategoryName
";
            }
            var param = new { vendor_id = vendorId, system_id = systemId, category_id = categoryId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.Query<VendorSystemCategoryNameResultModel>(twowaySql.Sql, dynParams).FirstOrDefault();
        }
    }
}
