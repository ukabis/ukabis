using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Settings
{
    public class VendorAuthenticationDefaultSettings
    {
        public static readonly string SECTION_NAME = "VendorSystemAuthenticationDefault";

        public string VendorId { get; set; }
        public string SystemId { get; set; }
    }
}
