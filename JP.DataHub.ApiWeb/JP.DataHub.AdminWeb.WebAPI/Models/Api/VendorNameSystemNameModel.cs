using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class VendorNameSystemNameModel
    {
        public string VendorId { get; set; }

        public string VendorName { get; set; }

        public bool IsDataUse { get; set; }

        public IList<SystemNameSystemIdModel> Systems { get; set; } = new List<SystemNameSystemIdModel>();

    }

    public class SystemNameSystemIdModel
    {
        public string SystemId { get; set; }


        public string SystemName { get; set; }

    }
}
