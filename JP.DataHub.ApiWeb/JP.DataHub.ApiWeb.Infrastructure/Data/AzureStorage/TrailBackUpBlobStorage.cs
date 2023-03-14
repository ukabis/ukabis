using JP.DataHub.ApiWeb.Infrastructure.Configuration;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage
{
    public class TrailBackUpBlobStorage : BlobStorage
    {
        public TrailBackUpBlobStorage(string connectionString) : base(new AzureStorageSetting(connectionString))
        {
        }
    }
}
