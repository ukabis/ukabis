using JP.DataHub.ApiWeb.Infrastructure.Configuration;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage
{
    public class ScriptRuntimeLogFileBlobStorage : BlobStorage
    {
        public ScriptRuntimeLogFileBlobStorage(string connectionString) : base(new AzureStorageSetting(connectionString))
        {
        }
    }
}
