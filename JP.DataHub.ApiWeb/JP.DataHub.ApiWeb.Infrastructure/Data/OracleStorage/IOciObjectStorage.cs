using Oci.ObjectstorageService.Responses;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.OracleStorage
{
    public interface IOciObjectStorage
    {
        Task<GetObjectResponse> GetObjectAsync(string containerName, string objectName);
        GetObjectResponse GetObject(string containerName, string objectName);
        HeadObjectResponse HeadObject(string containerName, string objectName);
        Task<GetObjectResponse> GetObjectAsync(Uri srcUri);
        Task<string> CreateNewObjectAsync(string containerName, string objectName, string message);
        Task<string> SaveObjectAsync(string containerName, string objectName, Stream message);
        Task<string> SaveObjectAsync(string containerName, string objectName, Stream message, string contentType);
        Task<string> SaveObjectAsync(string containerName, string objectName, Uri srcUri);
        Task<string> SaveObjectAsync(string containerName, string objectName, string message);
        Task<string> SaveObjectAsync(string containerName, string objectName, string message, string contentType);
        Task<bool?> DeleteObjectAsync(string objectPathName);
        Task DeleteObjectAsync(string containerName, string objectName);
        Task<string> MoveObjectAsync(string srcContainerName, string srcObjectName, string destContainerName, string destObjectName);
        Task<string> CopyObject(Uri srcUri, string destContainerName, string destObjectName);
        Task<string> CopyObjectAsync(string srcContainerName, string srcObjectName, string destContainerName, string destObjectName);
        Task<string> GetObjectPreSignedUrlAsync(string containerName, string objectName, int expireMinutes);
        Task<GetObjectResponse> PutObjectAsync(string containerName, string blobNobjectNameame, string contentType, string blockId, byte[] chunk, bool commit, List<string> id);
        Task<string> AppendObjectAsync(string containerName, string objectName, Stream message, string contentType);
        Task<string> OverwriteAppendObjectAsync(string containerName, string objectName, Stream message, string contentType);
        Task<string> OverwriteAppendObjectAsync(string containerName, string objectName, string message, string contentType);
        Task<GetObjectResponse> GetAppendObjectAsync(string containerName, string objectName);
        Task<bool?> DeleteAppendObjectAsync(string containerName, string objectName);
        Task<ListObjectsResponse> GetObjectListAsync(string prefix);
        Task<GetObjectResponse> GetAnyObjectAsync(string containerName, string objectName);
        Task<string> GetObjectPathName(string objectName, string containerName);
        Task<Stream> GenerateStreamFromString(string s);
    }
}
