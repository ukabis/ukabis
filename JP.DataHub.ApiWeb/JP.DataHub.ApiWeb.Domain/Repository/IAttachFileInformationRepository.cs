using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    internal interface IAttachFileInformationRepository
    {
        AttachFileStorageId GetAttachFileStorageId(VendorId vendorId);
        AttachFileInformation GetAttachFileInformation(FileId fileId, NotAuthentication notAuthentication);
        List<AttachFileInformation> GetAttachFileInformationSearchByMeta(Meta meta);
        HttpResponseMessage RegisterAttachFile(AttachFileInformation attachFile, NotAuthentication notAuthentication);
        HttpResponseMessage DeleteAttachFile(FileId fileId, NotAuthentication notAuthentication);
        IEnumerable<AttachFileListElement> GetAttachFileList(VendorId vendorId, GetAttachFileListParam getAttachFileListParam);
    }
}
