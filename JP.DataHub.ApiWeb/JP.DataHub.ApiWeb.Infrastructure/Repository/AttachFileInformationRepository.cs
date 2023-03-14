using System.Net;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Settings;
using JP.DataHub.Infrastructure.Database.AttachFile;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.Cache;
using JP.DataHub.ApiWeb.Core.Cache.Attributes;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Models.AttachFile;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [CacheKey]
    internal class AttachFileInformationRepository : IAttachFileInformationRepository
    {
        [CacheKey(CacheKeyType.Id, "vendor_id")]
        public static string CACHE_KEY_ATTACH_FILE_STORAGE_ID = "AttachFileStorageId";

        private const string MEDIATYPE_JSON = "application/json";

        private static readonly TimeSpan s_cacheExpireTime = TimeSpan.Parse("0:30:00");

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AttachFileInformation, AttachFileDynamicApiReturnModel>()
                .ForMember(dst => dst.FileId, ops => ops.MapFrom(src => src.FileId.Value))
                .ForMember(dst => dst.FileName, ops => ops.MapFrom(src => src.FileName.Value))
                .ForMember(dst => dst.Key, ops => ops.MapFrom(src => src.Key.Value))
                .ForMember(dst => dst.BlobUrl, ops => ops.MapFrom(src => src.BlobUrl.Value))
                .ForMember(dst => dst.IsDrm, ops => ops.MapFrom(src => src.IsDrm.Value))
                .ForMember(dst => dst.DrmKey, ops => ops.MapFrom(src => src.DrmKey.Value))
                .ForMember(dst => dst.DrmType, ops => ops.MapFrom(src => src.DrmType.Value))
                .ForMember(dst => dst.ContentType, ops => ops.MapFrom(src => src.ContentType.Value))
                .ForMember(dst => dst.FileLength, ops => ops.MapFrom(src => src.FileLength.Value))
                .ForMember(dst => dst.VendorId, ops => ops.MapFrom(src => src.VendorId.Value))
                .ForMember(dst => dst.AttachFileStorageId, ops => ops.MapFrom(src => src.AttachFileStorageId.Value))
                .ForMember(dst => dst.IsUploaded, ops => ops.MapFrom(src => src.IsUploaded.Value))
                .ForMember(dst => dst.MetaList, ops => ops.Ignore());
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper => s_lazyMapper.Value;


        private Lazy<IDynamicApiInterface> _lazyDynamicApiInterface = new Lazy<IDynamicApiInterface>(() => UnityCore.Resolve<IDynamicApiInterface>());
        private IDynamicApiInterface _dynamicApiInterface => _lazyDynamicApiInterface.Value;

        private Lazy<IJPDataHubDbConnection> _lazySqlConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("AttachFile"));
        private IJPDataHubDbConnection _sqlConnection => _lazySqlConnection.Value;

        private Lazy<ICache> _lazyCache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>());
        private ICache _cache => _lazyCache.Value;


        /// <summary>
        /// 添付ファイル情報を取得する。
        /// </summary>
        public AttachFileInformation GetAttachFileInformation(FileId fileId, NotAuthentication notAuthentication)
        {
            var requestModel = new DynamicApiRequestModel
            {
                HttpMethod = "GET",
                RelativeUri = $"{UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:AttachFileDynamicApiUrlGet")}/{fileId.Value}",
                MediaType = MEDIATYPE_JSON,
            };
            SetAdminHeader();
            var response = _dynamicApiInterface.Request(requestModel, notAuthentication.Value).ToHttpResponseMessage();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response);
            }

            var tempResult = JsonConvert.DeserializeObject<AttachFileDynamicApiReturnModel>(response.Content.ReadAsStringAsync().Result);
            return AttachFileInformation.Restore(new FileId(tempResult.FileId), new FileName(tempResult.FileName),
                new BlobUrl(tempResult.BlobUrl), new AttachFileStorageId(tempResult.AttachFileStorageId),
                new Key(tempResult.Key), new ContentType(tempResult.ContentType), new FileLength(tempResult.FileLength),
                new IsDrm(tempResult.IsDrm), new DrmType(tempResult.DrmType), new DrmKey(tempResult.DrmKey),
                new VendorId(tempResult.VendorId),
                new Meta(tempResult.MetaList.ToDictionary(key => new MetaKey(key.MetaKey), value => new MetaValue(value.MetaValue))),
                new IsUploaded(tempResult.IsUploaded == null ? false : (bool)tempResult.IsUploaded));
        }

        /// <summary>
        /// 添付ファイル情報をメタ情報から取得する。
        /// </summary>
        public List<AttachFileInformation> GetAttachFileInformationSearchByMeta(Meta meta)
        {
            string query = "";
            foreach (var m in meta)
            {
                if (!string.IsNullOrEmpty(query))
                {
                    query += " and ";
                }
                query += $"MetaList/any(o: o/MetaKey eq '{m.Key.Value}' and o/MetaValue eq '{m.Value.Value}')";
            }
            query = "?$filter=" + query;

            var requestModel = new DynamicApiRequestModel
            {
                HttpMethod = "GET",
                RelativeUri = UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:AttachFileDynamicApiUrlSearchByMeta"),
                QueryString = query,
                MediaType = MEDIATYPE_JSON,
            };
            SetAdminHeader();
            var response = _dynamicApiInterface.Request(requestModel, true).ToHttpResponseMessage();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response);
            }
            var tempResult = JsonConvert.DeserializeObject<List<AttachFileDynamicApiReturnModel>>(response.Content.ReadAsStringAsync().Result);

            return tempResult.Select(x => AttachFileInformation.Restore(new FileId(x.FileId), new FileName(x.FileName),
                new BlobUrl(x.BlobUrl), new AttachFileStorageId(x.AttachFileStorageId),
                new Key(x.Key), new ContentType(x.ContentType), new FileLength(x.FileLength),
                new IsDrm(x.IsDrm), new DrmType(x.DrmType), new DrmKey(x.DrmKey),
                new VendorId(x.VendorId),
                new Meta(x.MetaList.ToDictionary(y => new MetaKey(y.MetaKey),
                    value => new MetaValue(value.MetaValue))),
                new IsUploaded(x.IsUploaded == null ? true : (bool)x.IsUploaded)
                    )).ToList();
        }

        /// <summary>
        /// 添付ファイル情報を登録する。
        /// </summary>
        public HttpResponseMessage RegisterAttachFile(AttachFileInformation attachFile, NotAuthentication notAuthentication)
        {
            var requestContents = s_mapper.Map<AttachFileDynamicApiReturnModel>(attachFile);
            requestContents.MetaList = attachFile.Meta.Select(x => new AttachFileMeta() { MetaKey = x.Key.Value, MetaValue = x.Value.Value }).ToList();

            var requestModel = new DynamicApiRequestModel
            {
                HttpMethod = "POST",
                RelativeUri = UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:AttachFileDynamicApiUrlRegister"),
                Contents = JsonConvert.SerializeObject(
                    requestContents,
                    Formatting.None,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                MediaType = MEDIATYPE_JSON,
            };
            SetAdminHeader();
            var response = _dynamicApiInterface.Request(requestModel, notAuthentication.Value).ToHttpResponseMessage();
            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new ApiException(response);
            }

            return response;
        }

        /// <summary>
        /// 添付ファイル情報を削除する。
        /// </summary>
        public HttpResponseMessage DeleteAttachFile(FileId fileId, NotAuthentication notAuthentication)
        {
            var requestModel = new DynamicApiRequestModel
            {
                HttpMethod = "DELETE",
                RelativeUri = $"{UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:AttachFileDynamicApiUrlDelete")}/{fileId.Value}",
                MediaType = MEDIATYPE_JSON,
            };
            SetAdminHeader();
            var response = _dynamicApiInterface.Request(requestModel, notAuthentication.Value).ToHttpResponseMessage();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response);
            }

            return response;
        }

        /// <summary>
        /// 添付ファイルのストレージIDを取得する。
        /// </summary>
        public AttachFileStorageId GetAttachFileStorageId(VendorId vendorId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    a.attachfile_storage_id
FROM
attach_file_storage a INNER JOIN vendor_attachfilestorage va ON a.attachfile_storage_id = va.attachfile_storage_id 
WHERE
va.vendor_id = /*ds vendor_id*/'1' 
and a.is_active = 1 
and va.is_active = 1
and va.is_current = 1
and a.is_full = 0";
            }
            else
            {
                sql = @"
SELECT
    a.attachfile_storage_id
FROM
AttachFileStorage a INNER JOIN VendorAttachfilestorage va ON a.attachfile_storage_id = va.attachfile_storage_id 
WHERE
va.vendor_id = @vendor_id
and a.is_active = 1 
and va.is_active = 1
and va.is_current = 1
and a.is_full = 0";
            }
            var param = new { vendor_id = vendorId.Value };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);

            var result = _cache.Get<DB_AttachFileStorage>(
                CacheManager.CreateKey(CACHE_KEY_ATTACH_FILE_STORAGE_ID, vendorId.Value),
                s_cacheExpireTime,
                () => _sqlConnection.Query<DB_AttachFileStorage>(twowaySql.Sql, dynParams).FirstOrDefault());

            return new AttachFileStorageId(result?.attachfile_storage_id.ToString());
        }

        /// <summary>
        /// 添付ファイルリストを取得する
        /// </summary>
        public IEnumerable<AttachFileListElement> GetAttachFileList(VendorId vendorId, GetAttachFileListParam getAttachFileListParam)
        {
            string orderByField = "FileName";
            string orderByOrder = "asc";
            switch (getAttachFileListParam.SortIndex)
            {
                case GetAttachFileListParam.SortIndexEnum.FileName:
                    orderByField = "FileName";
                    break;
                case GetAttachFileListParam.SortIndexEnum.RegisterDateTime:
                    orderByField = "_Regdate";
                    break;
                case GetAttachFileListParam.SortIndexEnum.RegisterUserId:
                    orderByField = "_Reguser_Id";
                    break;
                default:
                    break;
            }
            if ((getAttachFileListParam.SortIndex != null) && (getAttachFileListParam.SortOrder == GetAttachFileListParam.SortOrderEnum.Desc))
            {
                orderByOrder = "desc";
            }
            string orderBy = $"{orderByField} {orderByOrder}";

            string query = $"?$select=FileId,FileName,_Regdate,_Reguser_Id&$filter=VendorId eq {vendorId.Value}&$orderby={orderBy}";
            var perRequestDataContainer = UnityCore.Resolve<PerRequestDataContainer>();
            perRequestDataContainer.XgetInternalAllField = true;
            var requestModel = new DynamicApiRequestModel
            {
                HttpMethod = "GET",
                RelativeUri = UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:AttachFileDynamicApiUrlSearchByMeta"),
                QueryString = query,
                MediaType = MEDIATYPE_JSON,
            };
            SetAdminHeader();
            var response = _dynamicApiInterface.Request(requestModel, true).ToHttpResponseMessage();
            perRequestDataContainer.XgetInternalAllField = false;
            if (response.StatusCode == HttpStatusCode.NotFound ||
                response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new List<AttachFileListElement>();
            }
            if (!response.IsSuccessStatusCode)
            {
                throw new ApiException(response);
            }
            var tempResult = JArray.Parse(response.Content.ReadAsStringAsync().Result);

            return tempResult.Select(item => new AttachFileListElement(
                new FileId(item["FileId"].ToObject<Guid>()),
                new FileName(item["FileName"]?.ToObject<string>()),
                item["_Regdate"]?.ToObject<DateTimeOffset>().ToUniversalTime().ToString("o"),
                item["_Reguser_Id"]?.ToObject<string>()
            ));
        }


        private void SetAdminHeader()
        {
            UnityCore.Resolve<IPerRequestDataContainer>().Xadmin = UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:AdminKeyword");
        }
    }
}
