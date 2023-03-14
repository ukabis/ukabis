using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface IDynamicApiAttachFileRepository
    {
        /// <summary>
        /// 添付ファイルのアップロード
        /// </summary>
        void UploadAttachFile(DynamicApiAttachFileInformation info, DynamicApiAttachFileInputStream input);

        /// <summary>
        /// Blobからファイルストリームを取得する
        /// </summary>
        Stream GetAttachFileFileStream(DynamicApiAttachFileInformation info);

        /// <summary>
        /// Blobからファイルを削除する
        /// </summary>
        void DeleteAttachFile(DynamicApiAttachFileInformation info);

        string GetFiletoBase64String(string filePath);
        void UploadBase64ToFile(VendorId vendorId, string base64String, string filePath);
        void DeleteFiletoBase64(VendorId vendorId, string filePath);
        void DeleteFilestoBase64(VendorId vendorId, string prefix);
        Uri CopyFile(string destBlobName, string destContainerName, string srcBlobName, string srcContainerName);
        Uri CopyFile(string destBlobName, string destContainerName, Uri srcUri);
        Uri GetUriWithSharedAccessSignature(DynamicApiAttachFileInformation info);
    }
}
