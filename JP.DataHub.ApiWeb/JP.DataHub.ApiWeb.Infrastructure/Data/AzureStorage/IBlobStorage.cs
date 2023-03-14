using Microsoft.WindowsAzure.Storage.Blob;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage
{
    public interface IBlobStorage
    {
        Task<CloudBlockBlob> GetBlockBlobAsync(Uri blobUri);

        Task<string> GetBlobOntimeTokenAsync(string containerName, string blobName, SharedAccessBlobPermissions sharedAccessBlobPermissions, int expireMinutes);

        Task<CloudBlobContainer> GetContainerAsync(string containerName, BlobContainerPublicAccessType accessLevel = BlobContainerPublicAccessType.Blob);

        Task DeleteContainerAsync(string containerName);

        Task<CloudBlockBlob> GetBlockBlobAsync(string containerName, string blobName);

        Task<ICloudBlob> GetAnyBlobAsync(string containerName, string blobName);

        Task<Uri> CreateNewBlobAsync(string containerName, string blobName, string message);

        Task<Uri> SaveBlobAsync(string containerName, string blobName, Stream message);
        Task<Uri> SaveBlobAsync(string containerName, string blobName, Stream message, string contentType);
        Task<Uri> SaveBlobAsync(string containerName, string blobName, CloudBlockBlob srcBlob);
        Task<Uri> SaveBlobAsync(string containerName, string blobName, string message);
        Task<Uri> SaveBlobAsync(string containerName, string blobName, string message, string contentType);

        Task<Uri> AppendBlobAsync(string containerName, string blobName, Stream message, string contentType);

        Task<Uri> OverwriteAppendBlobAsync(string containerName, string blobName, Stream message, string contentType = null);
        Task<Uri> OverwriteAppendBlobAsync(string containerName, string blobName, string message, string contentType = null);

        Task<CloudAppendBlob> GetAppendBlobAsync(string containerName, string blobName);

        Task<bool> DeleteAppendBlobAsync(string containerName, string blobName);

        Task DeleteBlobAsync(CloudBlockBlob blob);
        Task DeleteBlobAsync(string containerName, string blobName);

        Task<Uri> MoveBlobAsync(string srcContainerName, string srcBlobName, string destContainerName, string destBlobName);

        Task<CloudBlockBlob> PutBlockAsync(string containerName, string blobName, string contentType, string blockId, byte[] chunk, bool commit, List<string> ids = null);

        Task<Uri> CopyBlobAsync(string srcContainerName, string srcBlobName, string destContainerName, string destBlobName);

        Task<List<CloudBlockBlob>> GetBlobListAsync(string containerName, string prefix);
    }
}
