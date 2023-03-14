using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService
{
    [Log]
    [TransactionScope]
    interface IAttachFileApplicationService
    {
        FileId CreateFile(
            FileName fileName,
            Key key,
            ContentType contentType,
            FileLength fileLength,
            IsDrm isDrm,
            DrmType drmType,
            DrmKey drmKey,
            Meta meta,
            NotAuthentication notAuthentication);
        void UploadFile(FileId fileId, InputStream inputStream, IsEndStream isEndStream, IsAppendStream isAppendStream, AppendPosition appendPosition, NotAuthentication notAuthentication);
        DynamicApiResponse GetFile(FileId fileId, Key key, NotAuthentication notAuthentication);
        OutputStream GetFileStream(FileId fileId, Key key);
        AttachFileInformation GetFileMeta(FileId fileId, Key key);
        IEnumerable<AttachFileListElement> GetFileList(VendorId vendorId, GetAttachFileListParam getAttachFileListParam);
        DynamicApiResponse DeleteFile(FileId fileId, Key key, NotAuthentication notAuthentication);
        List<AttachFileInformation> SearchByMeta(Meta meta);
    }
}
