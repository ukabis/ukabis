using JP.DataHub.ApiWeb.Infrastructure.Configuration;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.OracleStorage
{
    /// <summary>
    /// Abstract Class of AzureStorage.
    /// </summary>
    public abstract class AbstractOciStorage
    {
        /// <summary>OciStorageSetting</summary>
        protected OciStorageSetting storageSetting;

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public AbstractOciStorage()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="storageSetting">OciStorageSetting</param>
        public AbstractOciStorage(OciStorageSetting storageSetting)
        {
            this.storageSetting = storageSetting;
        }
    }
}
