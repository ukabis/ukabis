using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using JP.DataHub.Com.Log;
using JP.DataHub.ApiWeb.Infrastructure.Configuration;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage
{
    /// <summary>
    /// Blob Storage.
    /// </summary>
    public class BlobStorage : AbstractAzureStorage, IBlobStorage
    {
        private JPDataHubLogger _logger = new JPDataHubLogger(typeof(BlobStorage));
        private CloudBlobClient _storageClient = null;


        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public BlobStorage()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="storageSetting">AzureStorageSetting</param>
        public BlobStorage(AzureStorageSetting storageSetting)
            : base(storageSetting)
        {
            _storageClient = storageAccount.CreateCloudBlobClient();
            _storageClient.DefaultRequestOptions.RetryPolicy = GetRetrySetting();
        }

        /// <summary>
        /// Get Container.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="accessLevel">accessLevel</param>
        /// <returns>CloudBlobContainer</returns>
        public async Task<CloudBlobContainer> GetContainerAsync(string containerName, BlobContainerPublicAccessType accessLevel = BlobContainerPublicAccessType.Off)
        {
            var container = this._storageClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync(accessLevel, null, null);
            return container;
        }

        /// <summary>
        /// Delete Container.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        public async Task DeleteContainerAsync(string containerName)
        {
            var container = _storageClient.GetContainerReference(containerName);
            await container.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Get BlockBlob.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="blobName">Blob Name</param>
        /// <returns>CloudBlockBlob</returns>
        public async Task<CloudBlockBlob> GetBlockBlobAsync(string containerName, string blobName)
        {
            return (await GetContainerAsync(containerName)).GetBlockBlobReference(blobName);
        }

        /// <summary>
        /// Get BlockBlob.
        /// </summary>
        /// <param name="blobUri">Blob Uri</param>
        /// <returns>CloudBlockBlob</returns>
        public async Task<CloudBlockBlob> GetBlockBlobAsync(Uri blobUri)
        {
            return (CloudBlockBlob)(await _storageClient.GetBlobReferenceFromServerAsync(blobUri));
        }

        /// <summary>
        /// Create the specified file if the file does not exists.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="message">The message.</param>
        /// <returns>blobのURI</returns>
        public async Task<Uri> CreateNewBlobAsync(string containerName, string blobName, string message)
        {
            try
            {
                var blob = (await GetContainerAsync(containerName)).GetBlockBlobReference(blobName);
                await blob.UploadTextAsync(message, accessCondition: AccessCondition.GenerateIfNoneMatchCondition("*"), null, null);
                return blob.Uri;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
                {
                    return null;
                }
                throw;
            }
        }

        /// <summary>
        /// Saves the specified container name.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="message">The message.</param>
        /// <returns>blobのURI</returns>
        public async Task<Uri> SaveBlobAsync(string containerName, string blobName, Stream message)
        {
            var blob = (await GetContainerAsync(containerName)).GetBlockBlobReference(blobName);
            await blob.UploadFromStreamAsync(message);
            return blob.Uri;
        }

        /// <summary>
        /// Saves the specified container name.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="message">The message.</param>
        /// <param name="contentType">The contentType.</param>
        /// <returns>blobのURI</returns>
        public async Task<Uri> SaveBlobAsync(string containerName, string blobName, Stream message, string contentType)
        {
            var blob = (await GetContainerAsync(containerName)).GetBlockBlobReference(blobName);
            await blob.UploadFromStreamAsync(message);
            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();
            return blob.Uri;
        }

        /// <summary>
        /// Save Blob.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="blobName">Blob Name</param>
        /// <param name="srcBlob">Src Blob</param>
        /// <returns>Uri</returns>
        public async Task<Uri> SaveBlobAsync(string containerName, string blobName, CloudBlockBlob srcBlob)
        {
            var destBlob = (await GetContainerAsync(containerName)).GetBlockBlobReference(blobName);
            await destBlob.StartCopyAsync(srcBlob, null, null, null, null);
            while (true)
            {
                await destBlob.FetchAttributesAsync();
                if (destBlob.CopyState.Status == CopyStatus.Success)
                {
                    break;
                }
                await Task.Delay(200);
            }
            return destBlob.Uri;
        }

        /// <summary>
        /// Save Blob.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="blobName">Blob Name</param>
        /// <param name="message">Message</param>
        /// <returns>Uri</returns>
        public async Task<Uri> SaveBlobAsync(string containerName, string blobName, string message)
        {
            var blob = (await GetContainerAsync(containerName)).GetBlockBlobReference(blobName);
            await blob.UploadTextAsync(message);
            return blob.Uri;
        }

        /// <summary>
        /// Saves the specified container name.
        /// </summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="blobName">Name of the BLOB.</param>
        /// <param name="message">The message.</param>
        /// <param name="contentType">The contentType.</param>
        /// <returns>blobのURI</returns>
        public async Task<Uri> SaveBlobAsync(string containerName, string blobName, string message, string contentType)
        {
            var blob = (await GetContainerAsync(containerName)).GetBlockBlobReference(blobName);
            await blob.UploadTextAsync(message);
            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();
            return blob.Uri;
        }

        /// <summary>
        /// Delete Blob.
        /// </summary>
        /// <param name="blob">CloudBlockBlob</param>
        public async Task DeleteBlobAsync(CloudBlockBlob blob)
        {
            if (blob == null)
            {
                return;
            }

            await blob.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Delete Blob.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="blobName">Blob Name</param>
        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            await DeleteBlobAsync(await GetBlockBlobAsync(containerName, blobName));
        }

        /// <summary>
        /// Move Blob.
        /// </summary>
        /// <param name="srcContainerName">Source ContainerName</param>
        /// <param name="srcBlobName">Source Blob Name</param>
        /// <param name="destContainerName">Destination ContainerName</param>
        /// <param name="destBlobName">Destination Blob Name</param>
        /// <returns>Uri</returns>
        public async Task<Uri> MoveBlobAsync(string srcContainerName, string srcBlobName, string destContainerName, string destBlobName)
        {
            var blob = await GetBlockBlobAsync(srcContainerName, srcBlobName);
            if (blob == null || !(await blob.ExistsAsync()))
            {
                return null;
            }

            var destUri = await SaveBlobAsync(destContainerName, destBlobName, blob);
            await DeleteBlobAsync(blob);
            return destUri;
        }

        /// <summary>
        /// Copy Blob.
        /// </summary>
        /// <param name="srcUri">Source ContainerName</param>
        /// <param name="destContainerName">Destination ContainerName</param>
        /// <param name="destBlobName">Destination Blob Name</param>
        /// <returns>Uri</returns>
        public async Task<Uri> CopyBlob(Uri srcUri, string destContainerName, string destBlobName)
        {
            var blob = await GetBlockBlobAsync(srcUri);
            if (blob == null || !(await blob.ExistsAsync()))
            {
                return null;
            }

            var destUri = await SaveBlobAsync(destContainerName, destBlobName, blob);
            return destUri;
        }

        /// <summary>
        /// Copy Blob.
        /// </summary>
        /// <param name="srcContainerName">Source ContainerName</param>
        /// <param name="srcBlobName">Source Blob Name</param>
        /// <param name="destContainerName">Destination ContainerName</param>
        /// <param name="destBlobName">Destination Blob Name</param>
        /// <returns>Uri</returns>
        public async Task<Uri> CopyBlobAsync(string srcContainerName, string srcBlobName, string destContainerName, string destBlobName)
        {
            var blob = await GetBlockBlobAsync(srcContainerName, srcBlobName);
            if (blob == null || !(await blob.ExistsAsync()))
            {
                return null;
            }

            var destUri = await SaveBlobAsync(destContainerName, destBlobName, blob);
            return destUri;
        }

        /// <summary>
        /// GetBlobOntimeToken.
        /// </summary>
        /// <param name="containerName">ContainerName</param>
        /// <param name="blobName">Blob Name</param>
        /// <param name="sharedAccessBlobPermissions">sharedAccessBlobPermissions</param>
        /// <param name="expireMinutes">expireMinutes</param>
        /// <returns>CloudBlockBlob</returns>
        public async Task<string> GetBlobOntimeTokenAsync(
            string containerName,
            string blobName,
            SharedAccessBlobPermissions sharedAccessBlobPermissions,
            int expireMinutes = 5)
        {
            var blob = await GetBlockBlobAsync(containerName, blobName);
            var adHocSAS = new SharedAccessBlobPolicy()
            {
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(expireMinutes),
                Permissions = sharedAccessBlobPermissions
            };
            var url = blob.Uri + blob.GetSharedAccessSignature(adHocSAS);
            return url;
        }

        public async Task<CloudBlockBlob> PutBlockAsync(string containerName, string blobName, string contentType, string blockId, byte[] chunk, bool commit, List<string> ids = null)
        {
            var blob = (await GetContainerAsync(containerName)).GetBlockBlobReference(blobName);
            blob.Properties.ContentType = contentType;
            await blob.PutBlockAsync(blockId, new MemoryStream(chunk), null);

            if (commit)
            {
                // コミット時には今まで分割登録してきた分のblockIdのリストが必須
                if (ids == null || ids.Count == 0)
                {
                    throw new ArgumentNullException("blockId List Empty");
                }

                await blob.PutBlockListAsync(ids);
            }

            return blob;
        }

        /// <summary>
        /// Append Blob, If not exist target block, create this.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="message"></param>
        /// <param name="contentType"></param>
        /// <returns>Uri</returns>
        public async Task<Uri> AppendBlobAsync(string containerName, string blobName, Stream message, string contentType = null)
        {
            var blob = (await GetContainerAsync(containerName)).GetAppendBlobReference(blobName);
            if (!(await blob.ExistsAsync())) 
            { 
                await blob.CreateOrReplaceAsync(); 
            }
            if (contentType != null) 
            { 
                blob.Properties.ContentType = contentType; 
            }
            await blob.AppendFromStreamAsync(message);
            return blob.Uri;
        }

        /// <summary>
        /// Append Blob, create this regardless of its existence.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="message"></param>
        /// <param name="contentType"></param>
        /// <returns>Uri</returns>
        public async Task<Uri> OverwriteAppendBlobAsync(string containerName, string blobName, Stream message, string contentType = null)
        {
            var blob = (await GetContainerAsync(containerName)).GetAppendBlobReference(blobName);
            await blob.CreateOrReplaceAsync();
            if (contentType != null) 
            { 
                blob.Properties.ContentType = contentType; 
            }
            await blob.AppendFromStreamAsync(message);
            return blob.Uri;
        }

        /// <summary>
        /// Append Blob, create this regardless of its existence.
        /// </summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <param name="message"></param>
        /// <param name="contentType"></param>
        /// <returns>Uri</returns>
        public async Task<Uri> OverwriteAppendBlobAsync(string containerName, string blobName, string message, string contentType = null)
        {
            var blob = (await GetContainerAsync(containerName)).GetAppendBlobReference(blobName);
            await blob.CreateOrReplaceAsync();
            if (contentType != null) 
            { 
                blob.Properties.ContentType = contentType; 
            }
            await blob.AppendTextAsync(message);
            return blob.Uri;
        }

        public async Task<CloudAppendBlob> GetAppendBlobAsync(string containerName, string blobName)
        {
            return (await GetContainerAsync(containerName)).GetAppendBlobReference(blobName);
        }

        public async Task<bool> DeleteAppendBlobAsync(string containerName, string blobName)
        {
            return await (await GetContainerAsync(containerName)).GetAppendBlobReference(blobName).DeleteIfExistsAsync();
        }

        public async Task<List<CloudBlockBlob>> GetBlobListAsync(string containerName, string prefix)
        {
            var container = await GetContainerAsync(containerName);
            var result = await container.ListBlobsSegmentedAsync(prefix, true, BlobListingDetails.Metadata, null, null, null, null);
            var list = new List<CloudBlockBlob>();
            foreach (var blob in result.Results)
            {
                list.Add((CloudBlockBlob)blob);
            }
            return list;
        }

        public async Task<ICloudBlob> GetAnyBlobAsync(string containerName, string blobName)
        {
            return await (await GetContainerAsync(containerName)).GetBlobReferenceFromServerAsync(blobName);
        }
    }
}
