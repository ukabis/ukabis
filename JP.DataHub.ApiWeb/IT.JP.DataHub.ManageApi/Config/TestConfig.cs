using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.Config
{
    public class TestConfig
    {
        public Guid AdminVendorId { get; set; }
        public Guid AdminSystemId { get; set; }
        public Guid NormalVendorId { get; set; }
        public Guid NormalSystemId { get; set; }
        public Guid RepositoryGroupId { get; set; }
        public Guid SqlServerRepositoryGroupId { get; set; }
    }
}
