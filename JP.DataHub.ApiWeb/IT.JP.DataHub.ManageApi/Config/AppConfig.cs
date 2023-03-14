using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.Config
{
    public class AppConfig
    {
        public string Server { get; set; }
        public string Environment { get; set; }
        public string Account { get; set; }
        public string AdminVendorId { get; set; }
        public string AdminSystemId { get; set; }
        public string NormalVendorId { get; set; }
        public string NormalSystemId { get; set; }
        public string RepositoryGroupId { get; set; }
        public string SqlServerRepositoryGroupId { get; set; }
        public string AttachFileMetaRepositoryId { get; set; }
        public string AttachFileBlobRepositoryId { get; set; }
        public string DocumentHistoryRepositoryId { get; set; }
        public bool AllowMailTest { get; set; }
        public string MailTestAddressTo { get; set; }
    }
}
