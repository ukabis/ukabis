using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Settings
{
    public class IpAddressSettings
    {
        public static readonly string SECTION_NAME = "IpAddress";
        public bool XFFIPAddressesCouldBeRegardedAsRepresentiveClientIP { get; set; }
    }
}
