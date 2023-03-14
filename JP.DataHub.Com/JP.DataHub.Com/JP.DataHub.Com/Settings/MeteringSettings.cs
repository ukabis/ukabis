using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Settings
{
    public class MeteringSettings
    {
        public static readonly string SECTION_NAME = "Metering";

        public MeteringSettings()
        {
        }

        public bool ExternalApi { get; set; }
        public bool InternalApi { get; set; }
    }
}
