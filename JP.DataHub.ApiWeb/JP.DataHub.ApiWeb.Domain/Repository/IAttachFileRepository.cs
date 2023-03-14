using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface IAttachFileRepository
    {
        Stream GetFileStream(VendorId vendorId, FileId fileId, FileName fileName, AttachFileStorageId attachFilestorageId);
        void DeleteFile(VendorId vendorId, FileId fileId, FileName fileName, AttachFileStorageId attachFilestorageId);
        FilePath TempFileUpload(FileId fileId, FileName fileName, InputStream inputStream, IsAppendStream isAppendStream, AppendPosition appendPosition);
        void Upload(FilePath filePath, FileId fileId, FileName fileName, VendorId vendorId, ContentType contentType, AttachFileStorageId attachFilestorageId);
    }
}
