using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IDynamicApiAttachFileRepository
    {
        /// <summary>
        /// 添付ファイルのアップロード
        /// </summary>
        //void UploadAttachFile(DynamicApiAttachFileInformation info, DynamicApiAttachFileInputStream input);

        /// <summary>
        /// Blobからファイルストリームを取得する
        /// </summary>
        //Stream GetAttachFileFileStream(DynamicApiAttachFileInformation info);

        /// <summary>
        /// Blobからファイルを削除する
        /// </summary>
        //void DeleteAttachFile(DynamicApiAttachFileInformation info);

        string GetFiletoBase64String(string filePath);
        void UploadBase64ToFile(string vendorId, string base64String, string filePath);
        void DeleteFiletoBase64(string vendorId, string filePath);
        void DeleteFilestoBase64(string vendorId, string prefix);
        Uri CopyFile(string destBlobName, string destContainerName, string srcBlobName, string srcContainerName);
        Uri CopyFile(string destBlobName, string destContainerName, Uri srcUri);
        //Uri GetUriWithSharedAccessSignature(DynamicApiAttachFileInformation info);
    }
}
