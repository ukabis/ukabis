using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    [Log]
    [TransactionScope]
    class AttachFileApplicationService : IAttachFileApplicationService
    {
        public FileId CreateFile(
            FileName fileName,
            Key key,
            ContentType contentType,
            FileLength fileLength,
            IsDrm isDrm,
            DrmType drmType,
            DrmKey drmKey,
            Meta meta,
            NotAuthentication notAuthentication)
        {
            var AttachFile = AttachFileInformation.Create(fileName, key, contentType, fileLength, isDrm, drmType, drmKey, meta);
            AttachFile.Save(notAuthentication);
            return AttachFile.FileId;
        }

        public void UploadFile(FileId fileId, InputStream inputStream, IsEndStream isEndStream, IsAppendStream isAppendStream, AppendPosition appendPosition, NotAuthentication notAuthentication)
        {
            var AttachFile = AttachFileInformation.Restore(fileId, notAuthentication);
            AttachFile.Upload(inputStream, isEndStream, isAppendStream, appendPosition);
        }

        public DynamicApiResponse GetFile(FileId fileId, Key key, NotAuthentication notAuthentication)
        {
            var AttachFile = AttachFileInformation.Restore(fileId, key, notAuthentication);
            return new DynamicApiResponse(AttachFile.GetFile());
        }

        public OutputStream GetFileStream(FileId fileId, Key key)
        {
            var AttachFile = AttachFileInformation.Restore(fileId, key, new NotAuthentication(true));
            return AttachFile.GetFileStream();
        }

        public AttachFileInformation GetFileMeta(FileId fileId, Key key)
        {
            return AttachFileInformation.Restore(fileId, key, new NotAuthentication(true));
        }

        public IEnumerable<AttachFileListElement> GetFileList(VendorId vendorId, GetAttachFileListParam getAttachFileListParam)
        {
            return UnityCore.Resolve<IAttachFileInformationRepository>().GetAttachFileList(vendorId, getAttachFileListParam);
        }

        public DynamicApiResponse DeleteFile(FileId fileId, Key key, NotAuthentication notAuthentication)
        {
            var AttachFile = AttachFileInformation.Restore(fileId, key, notAuthentication);
            return new DynamicApiResponse(AttachFile.DeleteFile(notAuthentication));
        }

        public List<AttachFileInformation> SearchByMeta(Meta meta)
        {
            return AttachFileInformation.SearchByMeta(meta);

        }
    }
}
