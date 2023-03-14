using JP.DataHub.ApiWeb.Infrastructure.Configuration;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage
{
    public class LoggingBlobStorage : BlobStorage
    {
        public LoggingBlobStorage(string connectionString) : base(new AzureStorageSetting(connectionString))
        {
        }
    }
}
