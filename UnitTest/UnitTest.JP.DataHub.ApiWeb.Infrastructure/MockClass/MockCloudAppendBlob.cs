using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.MockClass
{
    class MockCloudAppendBlob : CloudAppendBlob
    {
        Stream OpenReadStream { get; }

        public MockCloudAppendBlob(Uri blobAbsoluteUri, Stream openReadStream, string contentType) : base(blobAbsoluteUri)
        {
            OpenReadStream = openReadStream;
            this.Properties.ContentType = contentType;
        }

        public override async Task<Stream> OpenReadAsync()
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
