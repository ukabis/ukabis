using System.Net;
using System.Net.Http.Headers;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal class AttachFileInformation : IEntity
    {
        /// <summary>
        /// ファイルID
        /// </summary>
        public FileId FileId { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public FileName FileName { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public Key Key { get; set; }

        /// <summary>
        /// AttachFileStorageId
        /// </summary>
        public AttachFileStorageId AttachFileStorageId { get; set; }

        /// <summary>
        /// BlobUrl
        /// </summary>
        public BlobUrl BlobUrl { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        public ContentType ContentType { get; set; }

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        public FileLength FileLength { get; set; }

        /// <summary>
        /// DRMありか
        /// </summary>
        public IsDrm IsDrm { get; set; }

        /// <summary>
        /// Drmのタイプ
        /// </summary>
        public DrmType DrmType { get; set; }

        /// <summary>
        /// Drmのキー
        /// </summary>
        public DrmKey DrmKey { get; set; }

        /// <summary>
        /// ファイルをアップロード済みか
        /// </summary>
        public IsUploaded IsUploaded { get; set; }

        /// <summary>
        /// メタ
        /// </summary>
        public Meta Meta { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public VendorId VendorId { get; set; }

        private IAttachFileRepository AttachFileRepository => _lazyAttachFileRepository.Value;
        private Lazy<IAttachFileRepository> _lazyAttachFileRepository = new Lazy<IAttachFileRepository>(() => UnityCore.Resolve<IAttachFileRepository>());

        private IAttachFileInformationRepository AttachFileInformationRepository => _lazyAttachFileInformationRepository.Value;
        private Lazy<IAttachFileInformationRepository> _lazyAttachFileInformationRepository = new Lazy<IAttachFileInformationRepository>(() => UnityCore.Resolve<IAttachFileInformationRepository>());


        protected AttachFileInformation() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachFileInformation" /> class.
        /// </summary>
        protected AttachFileInformation(
                    FileId fileId,
                    FileName fileName,
                    Key key,
                    AttachFileStorageId attachFilestorageId,
                    BlobUrl blobUrl,
                    ContentType contentType,
                    FileLength fileLength,
                    IsDrm isDrm,
                    DrmType drmType,
                    DrmKey drmKey,
                    VendorId vendorId,
                    Meta meta,
                    IsUploaded isUploaded)
        {
            FileId = fileId;
            FileName = fileName;
            Key = key;
            AttachFileStorageId = attachFilestorageId;
            BlobUrl = blobUrl;
            ContentType = contentType;
            FileLength = fileLength;
            IsDrm = isDrm;
            DrmType = drmType;
            DrmKey = drmKey;
            VendorId = vendorId;
            Meta = meta;
            IsUploaded = isUploaded;
        }


        public static AttachFileInformation Create(
            FileName fileName,
            Key key,
            ContentType contentType,
            FileLength fileLength,
            IsDrm isDrm,
            DrmType drmType,
            DrmKey drmKey,
            Meta meta
)
        {
            var vendorId = new VendorId(Guid.Parse(UnityCore.Resolve<IPerRequestDataContainer>().VendorId));

            // ファイルID発行
            var fileId = new FileId(Guid.NewGuid());
            var attachFilestorageId = UnityCore.Resolve<IAttachFileInformationRepository>().GetAttachFileStorageId(vendorId);
            if (string.IsNullOrEmpty(attachFilestorageId.Value))
            {
                throw new Exception("attachFilestorageId not found");
            }

            // VendorId(コンテナ)/FileId/FileName
            var blobUrl = new BlobUrl($"/{vendorId.Value}/{fileId.Value}/{fileName.Value}");
 
            // DyanamicAPIを呼び出してファイル情報を登録する
            AttachFileInformation AttachFile = new AttachFileInformation(
                fileId,
                fileName,
                key,
                attachFilestorageId,
                blobUrl,
                contentType,
                fileLength,
                isDrm,
                drmType,
                drmKey,
                vendorId,
                meta,
                new IsUploaded(false));

            return AttachFile;
        }

        public static AttachFileInformation Restore(
            FileId fileId, FileName fileName, BlobUrl blobUrl, AttachFileStorageId attachFilestorageId,
            Key key, ContentType contentType, FileLength fileLength, IsDrm isDrm, DrmType drmType,
            DrmKey drmKey, VendorId vendorId, Meta meta, IsUploaded isUploaded)
        {
            return new AttachFileInformation(fileId, fileName, key, attachFilestorageId, blobUrl, contentType, fileLength, isDrm, drmType, drmKey, vendorId, meta, isUploaded);
        }

        public static AttachFileInformation Restore(FileId fileId, NotAuthentication notAuthentication)
        {
            return UnityCore.Resolve<IAttachFileInformationRepository>().GetAttachFileInformation(fileId, notAuthentication);
        }

        public static AttachFileInformation Restore(FileId fileId, Key requestKey, NotAuthentication notAuthentication)
        {
            var atttachFile = UnityCore.Resolve<IAttachFileInformationRepository>().GetAttachFileInformation(fileId, notAuthentication);
            if (!atttachFile.Key.IsKeyMach(requestKey))
            {
                throw new ApiException(new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden));
            }
            return atttachFile;
        }

        public static List<AttachFileInformation> SearchByMeta(Meta meta)
        {
            return UnityCore.Resolve<IAttachFileInformationRepository>().GetAttachFileInformationSearchByMeta(meta);
        }


        public void Save(NotAuthentication notAuthentication)
        {
            UnityCore.Resolve<IAttachFileInformationRepository>().RegisterAttachFile(this, notAuthentication);
        }

        public HttpResponseMessage GetFile()
        {
            Stream stream = null;
            try
            {
                stream = AttachFileRepository.GetFileStream(VendorId, FileId, FileName, AttachFileStorageId);
            }
            catch (NotFoundException nfe)
            {
                // アップロード済みの状態でファイルが存在しない場合はエラーとする
                if (this.IsUploaded.Value)
                {
                    throw new Exception($"storage file not found : {nfe.Message}");
                }
                else
                {
                    throw;
                }
            }

            if (stream.Length == 0)
            {
                throw new Exception("storage file not found");
            }
            var result = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(ContentType.Value);

            return result;
        }

        public OutputStream GetFileStream()
        {
            Stream stream = null;
            try
            {
                stream = AttachFileRepository.GetFileStream(VendorId, FileId, FileName, AttachFileStorageId);
            }
            catch (NotFoundException nfe)
            {
                // アップロード済みの状態でファイルが存在しない場合はエラーとする
                if (this.IsUploaded.Value)
                {
                    throw new Exception($"storage file not found : {nfe.Message}");
                }
                else
                {
                    throw;
                }
            }
            if (stream.Length == 0)
            {
                throw new Exception("storage file not found");
            }

            return new OutputStream(stream);
        }

        public void Upload(InputStream inputStream, IsEndStream isEndStream, IsAppendStream isAppendStream, AppendPosition appendPosition)
        {
            var path = AttachFileRepository.TempFileUpload(FileId, FileName, inputStream, isAppendStream, appendPosition);
            if (isEndStream.Value)
            {
                AttachFileRepository.Upload(path, FileId, FileName, VendorId, ContentType, AttachFileStorageId);
                IsUploaded = new IsUploaded(true);
                AttachFileInformationRepository.RegisterAttachFile(this, new NotAuthentication(true));
            }
        }

        public HttpResponseMessage DeleteFile(NotAuthentication notAuthentication)
        {
            // DBからファイル情報を削除
            var response = AttachFileInformationRepository.DeleteAttachFile(FileId, notAuthentication);
            // Blobからファイルを削除
            AttachFileRepository.DeleteFile(VendorId, FileId, FileName, AttachFileStorageId);

            return response;
        }
    }
}
