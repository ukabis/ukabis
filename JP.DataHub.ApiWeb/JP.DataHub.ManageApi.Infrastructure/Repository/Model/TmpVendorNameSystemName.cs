using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Infrastructure.Repository.Model
{
    internal class TmpVendorNameSystemName
    {
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string RepresentativeMailAddress { get; set; }
        public bool IsDataUse { get; set; }
        public bool IsDataOffer { get; set; }
        public DateTime VendorUpdDate { get; set; }
        public bool IsEnableVendor { get; set; }

        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string ApplicationId { get; set; }
        public DateTime SystemUpdDate { get; set; }
        public bool IsEnableSystem { get; set; }
    }
}
