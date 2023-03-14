using JP.DataHub.ApiWeb.Infrastructure.Configuration;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage
{
    public class AttachFileBlobStorage : BlobStorage
    {
        public AttachFileBlobStorage(string connectionString) : base(new AzureStorageSetting(connectionString))
        {
        }
    }
}
