using System;
using System.Collections.Generic;

namespace IT.JP.DataHub.ODataOverPartition.Config
{
    public class AppConfig
    {
        public string Server { get; set; }
        public string Environment { get; set; }
        public string Account { get; set; }
        public Guid AdminVendorId { get; set; }
        public Guid AdminSystemId { get; set; }
        public Guid NormalVendorId { get; set; }
        public Guid NormalSystemId { get; set; }
        public Guid RepositoryGroupId { get; set; }
        public Guid SqlServerRepositoryGroupId { get; set; }
        public string ExpiredToken { get; set; }
        public string ExternalStorageAzureBlob { get; set; }
        public List<string> SupportedRepositories { get; set; }
        public List<string> SkipList { get; set; }
    }
}
