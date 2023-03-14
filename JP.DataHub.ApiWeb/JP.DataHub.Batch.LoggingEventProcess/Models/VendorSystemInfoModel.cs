using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingEventProcess.Models
{
    public class VendorSystemInfoModel
    {
        public string VendorId { get; private set; }

        public string VendorName { get; private set; }

        private List<SystemInfoModel> systems = new List<SystemInfoModel>();

        public VendorSystemInfoModel(string vendorId, string vendorName)
        {
            VendorId = vendorId;
            VendorName = vendorName;
        }

        public void AddSystem(SystemInfoModel systemInfo)
        {
            systems.Add(systemInfo);

        }
    }
}
