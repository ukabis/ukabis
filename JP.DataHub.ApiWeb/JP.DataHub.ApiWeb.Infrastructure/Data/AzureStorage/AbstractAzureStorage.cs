using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using JP.DataHub.ApiWeb.Infrastructure.Configuration;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage
{
    /// <summary>
    /// Abstract Class of AzureStorage.
    /// </summary>
    public abstract class AbstractAzureStorage
    {
        /// <summary>AzureStorageSetting</summary>
        protected AzureStorageSetting storageSetting;
        /// <summary>CloudStorageAccount</summary>
        protected CloudStorageAccount storageAccount;

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public AbstractAzureStorage()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="storageSetting">AzureStorageSetting</param>
        public AbstractAzureStorage(AzureStorageSetting storageSetting)
        {
            this.storageSetting = storageSetting;
            this.storageAccount = CloudStorageAccount.Parse(storageSetting.ConnectionString);
        }

        /// <summary>
        /// Get RetrySetting.
        /// </summary>
        /// <returns>LinearRetry</returns>
        protected LinearRetry GetRetrySetting()
        {
            return new LinearRetry(TimeSpan.FromMilliseconds(this.storageSetting.RetryInterval), this.storageSetting.RetryCount);
        }
    }
}
