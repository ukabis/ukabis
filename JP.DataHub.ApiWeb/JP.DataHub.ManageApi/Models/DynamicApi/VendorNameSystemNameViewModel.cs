using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class VendorNameSystemNameViewModel
    {
        public string VendorId { get; set; }

        public string VendorName { get; set; }

        public bool IsDataUse { get; set; }

        public IList<SystemNameSystemIdViewModel> Systems { get; set; } = new List<SystemNameSystemIdViewModel>();

    }

    public class SystemNameSystemIdViewModel
    {
        public string SystemId { get; set; }


        public string SystemName { get; set; }

    }
}
