using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Settings
{
    public class ServerSettings
    {
        public static readonly string SECTION_NAME = "Server";
        public string Server { get; set; }
        public string Environment { get; set; }
    }
}
