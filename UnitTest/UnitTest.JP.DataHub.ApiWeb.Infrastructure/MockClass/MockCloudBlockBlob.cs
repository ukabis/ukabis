using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.MockClass
{
    class MockCloudBlockBlob : CloudBlockBlob
    {
        Stream OpenReadStream { get; }

        public MockCloudBlockBlob(Uri blobAbsoluteUri, Stream openReadStream, string contentType) : base(blobAbsoluteUri)
        {
            OpenReadStream = openReadStream;
            this.Properties.ContentType = contentType;
        }

        public override async Task<Stream> OpenReadAsync(AccessCondition accessCondition = null, BlobRequestOptions options = null, OperationContext operationContext = null)
        {
            return OpenReadStream;
        }

        public override async Task<string> DownloadTextAsync()
        {
            using (var sr = new StreamReader(await OpenReadAsync()))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}
