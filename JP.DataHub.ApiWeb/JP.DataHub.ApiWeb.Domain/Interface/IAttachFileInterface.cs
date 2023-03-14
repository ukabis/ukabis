using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Interface.Model;

namespace JP.DataHub.ApiWeb.Domain.Interface
{
    public interface IAttachFileInterface
    {
        string CreateFile(AttachFileModel registerModel, bool notAuthentication = false);
        void UploadFile(AttachFileUploadFileModel model, bool isEndStream, bool isAppendStream, long appendPosition, bool notAuthentication = false);
        DynamicApiResponse GetFile(string fileId, string key, bool notAuthentication = false);
        AttachFileModel GetFileMeta(string fileId, string key);
        IEnumerable<AttachFileListElementModel> GetFileList(string vendorId, string sortIndex, string sortOrder);
        List<AttachFileModel> SearchByMeta(Dictionary<string, string> metaDictionary);
        DynamicApiResponse DeleteFile(string fileId, string key, bool notAuthentication = false);
    }
}
