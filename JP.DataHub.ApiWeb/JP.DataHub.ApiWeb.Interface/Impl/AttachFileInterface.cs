using AutoMapper;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model;

namespace JP.DataHub.ApiWeb.Interface.Impl
{
    internal class AttachFileInterface : IAttachFileInterface
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AttachFileModel, AttachFileInformation>()
                .ForMember(dst => dst.FileName, ops => ops.MapFrom(src => new FileName(src.FileName)))
                .ForMember(dst => dst.Key, ops => ops.MapFrom(src => new Key(src.Key)))
                .ForMember(dst => dst.AttachFileStorageId, ops => ops.MapFrom(src => new AttachFileStorageId(src.AttachFileStorageId)))
                .ForMember(dst => dst.BlobUrl, ops => ops.MapFrom(src => new BlobUrl(src.BlobUrl)))
                .ForMember(dst => dst.IsDrm, ops => ops.MapFrom(src => new IsDrm(src.IsDrm)))
                .ForMember(dst => dst.DrmKey, ops => ops.MapFrom(src => new DrmKey(src.DrmKey)))
                .ForMember(dst => dst.DrmType, ops => ops.MapFrom(src => new DrmType(src.DrmType)))
                .ForMember(dst => dst.ContentType, ops => ops.MapFrom(src => new ContentType(src.ContentType)))
                .ForMember(dst => dst.FileLength, ops => ops.MapFrom(src => new FileLength(src.FileLength)))
                .ForMember(dst => dst.Meta, ops => ops.Ignore())
                .ForMember(dst => dst.VendorId, ops => ops.MapFrom(src => !string.IsNullOrWhiteSpace(src.VendorId) ? new VendorId(Guid.Parse(src.VendorId)) : null))
                ;
                cfg.CreateMap<AttachFileInformation, AttachFileModel>()
                .ForMember(dst => dst.FileId, ops => ops.MapFrom(src => src.FileId.Value))
                .ForMember(dst => dst.FileName, ops => ops.MapFrom(src => src.FileName.Value))
                .ForMember(dst => dst.Key, ops => ops.MapFrom(src => src.Key.Value))
                .ForMember(dst => dst.AttachFileStorageId, ops => ops.MapFrom(src => src.AttachFileStorageId.Value))
                .ForMember(dst => dst.BlobUrl, ops => ops.MapFrom(src => src.BlobUrl.Value))
                .ForMember(dst => dst.IsDrm, ops => ops.MapFrom(src => src.IsDrm.Value))
                .ForMember(dst => dst.DrmKey, ops => ops.MapFrom(src => src.DrmKey.Value))
                .ForMember(dst => dst.DrmType, ops => ops.MapFrom(src => src.DrmType.Value))
                .ForMember(dst => dst.ContentType, ops => ops.MapFrom(src => src.ContentType.Value))
                .ForMember(dst => dst.FileLength, ops => ops.MapFrom(src => src.FileLength.Value))
                .ForMember(dst => dst.MetaList, ops => ops.Ignore())
                .ForMember(dst => dst.VendorId, ops => ops.MapFrom(src => src.VendorId.Value))
                ;
                cfg.CreateMap<AttachFileListElement, AttachFileListElementModel>()
                    .ForMember(dst => dst.FileId, ops => ops.MapFrom(src => src.FileId.Value))
                    .ForMember(dst => dst.FileName, ops => ops.MapFrom(src => src.FileName.Value));
            });

            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper => s_lazyMapper.Value;

        private IAttachFileApplicationService _attachFileApplicationService = UnityCore.Resolve<IAttachFileApplicationService>();


        /// <summary>
        /// ファイル情報を登録する
        /// </summary>
        public string CreateFile(AttachFileModel registerModel, bool notAuthentication = false)
        {
            var meta = registerModel.MetaList.Select(x => new { key = new MetaKey(x.MetaKey), value = new MetaValue(x.MetaValue) }).ToDictionary(x => x.key, x => x.value);
            var fileId = _attachFileApplicationService.CreateFile(
                new FileName(registerModel.FileName),
                new Key(registerModel.Key),
                new ContentType(registerModel.ContentType),
                new FileLength(registerModel.FileLength),
                new IsDrm(registerModel.IsDrm),
                new DrmType(registerModel.DrmType),
                new DrmKey(registerModel.DrmKey),
                new Meta(meta),
                new NotAuthentication(notAuthentication));
            return fileId.Value.ToString();
        }

        /// <summary>
        /// ファイルをアップロードする。
        /// </summary>
        public void UploadFile(AttachFileUploadFileModel model, bool isEndStream, bool isAppendStream, long appendPosition, bool notAuthentication = false)
        {
            _attachFileApplicationService.UploadFile(
                new FileId(Guid.Parse(model.FileId)),
                new InputStream(model.InputStream),
                new IsEndStream(isEndStream),
                new IsAppendStream(isAppendStream),
                new AppendPosition(appendPosition),
                new NotAuthentication(notAuthentication));
            return;
        }

        /// <summary>
        /// ファイルを取得する。
        /// </summary>
        public DynamicApiResponse GetFile(string fileId, string key, bool notAuthentication = false)
        {
            return _attachFileApplicationService.GetFile(
                new FileId(Guid.Parse(fileId)),
                new Key(key),
                new NotAuthentication(notAuthentication));
        }

        /// <summary>
        /// ファイルのメタ情報を取得する。
        /// </summary>
        public AttachFileModel GetFileMeta(string fileId, string key)
        {
            var attachFile = _attachFileApplicationService.GetFileMeta(new FileId(Guid.Parse(fileId)), new Key(key));
            var result = s_mapper.Map<AttachFileModel>(attachFile);
            result.MetaList = attachFile.Meta.Select(x => new AttachFileMeta() { MetaKey = x.Key.Value, MetaValue = x.Value.Value }).ToList();

            return result;
        }

        /// <summary>
        /// ファイルのリストを取得する。
        /// </summary>
        public IEnumerable<AttachFileListElementModel> GetFileList(string vendorId, string sortIndex, string sortOrder)
        {
            GetAttachFileListParam.SortIndexEnum? sortIndexParam = null;
            if (sortIndex == "FileName")
            {
                sortIndexParam = GetAttachFileListParam.SortIndexEnum.FileName;
            }
            if (sortIndex == "RegisterDateTime")
            {
                sortIndexParam = GetAttachFileListParam.SortIndexEnum.RegisterDateTime;
            }
            if (sortIndex == "RegisterUserId")
            {
                sortIndexParam = GetAttachFileListParam.SortIndexEnum.RegisterUserId;
            }
            GetAttachFileListParam.SortOrderEnum sortOrderParam = (sortOrder == "asc")
                ? GetAttachFileListParam.SortOrderEnum.Asc
                : GetAttachFileListParam.SortOrderEnum.Desc;
            var attachFileList = _attachFileApplicationService.GetFileList(
                new VendorId(Guid.Parse(vendorId)),
                new GetAttachFileListParam(
                    sortIndexParam,
                    sortOrderParam
                )
            );
            return attachFileList.Select(item => s_mapper.Map<AttachFileListElementModel>(item));
        }

        /// <summary>
        /// ファイルを検索する。
        /// </summary>
        public List<AttachFileModel> SearchByMeta(Dictionary<string, string> metaDictionary)
        {
            var dictionary = new Dictionary<MetaKey, MetaValue>();
            foreach (var meta in metaDictionary)
            {
                dictionary.Add(new MetaKey(meta.Key), new MetaValue(meta.Value));
            }

            var attachFiles = _attachFileApplicationService.SearchByMeta(new Meta(dictionary));
            var result = s_mapper.Map<List<AttachFileModel>>(attachFiles);
            foreach (var file in result)
            {
                file.MetaList = attachFiles.Where(x => x.FileId.Value == file.FileId.ToString()).First().Meta.Select(x => new AttachFileMeta() { MetaKey = x.Key.Value, MetaValue = x.Value.Value }).ToList();
            }
            return result;
        }

        /// <summary>
        /// ファイルを削除する。
        /// </summary>
        public DynamicApiResponse DeleteFile(string fileId, string key, bool notAuthentication = false)
        {
            return _attachFileApplicationService.DeleteFile(new FileId(Guid.Parse(fileId)), new Key(key), new NotAuthentication(notAuthentication));
        }
    }
}
