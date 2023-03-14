using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.Storage
{
    public class BlobStorageClient : IStorageClient
    {
        private readonly BlobServiceClient Client;
        private readonly string ContainerName;
        private readonly string RootPath;

        public BlobStorageClient(string connectionString, string containerName, string rootPath)
        {
            RootPath = rootPath;
            ContainerName = containerName;
            Client = new BlobServiceClient(connectionString);
        }

        public void CopyTo(string objectName, Stream createObject)
        {
            var container = Client.GetBlobContainerClient(ContainerName);
            BlobClient blobClient = container.GetBlobClient(objectName);
            blobClient.Upload(createObject, true);

        }

        public Task CopyToAsync(string objectName, Stream createObject)
        {
            var container = Client.GetBlobContainerClient(ContainerName);
            return container.UploadBlobAsync($"{RootPath}/{objectName}", createObject);
        }

        public void Delete(string objectName)
        {
            var container = Client.GetBlobContainerClient(ContainerName);
            container.GetBlobClient(objectName).DeleteIfExists();
        }

        public Task DeleteAsync(string objectName)
        {
            var container = Client.GetBlobContainerClient(ContainerName);
            return container.GetBlobClient(objectName).DeleteIfExistsAsync();
        }

        public bool Exist(string objectName)
        {
            throw new NotImplementedException();
        }

        public string GetObjectPath(string objectName)
        {
            var container = Client.GetBlobContainerClient(ContainerName);
            return container.GetBlobClient(objectName).Uri.AbsoluteUri;
        }

        public long GetSize(string objectName)
        {
            throw new NotImplementedException();
        }

        public Stream GetStream(string objectName)
        {
            var container = Client.GetBlobContainerClient(ContainerName);
            return container.GetBlobClient(objectName).OpenRead();
        }

        public IEnumerable<string> List(string prefix)
        {
            throw new NotImplementedException();
        }
    }
}
