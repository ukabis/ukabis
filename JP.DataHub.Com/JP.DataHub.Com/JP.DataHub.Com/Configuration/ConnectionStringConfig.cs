using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Configuration
{
    public class ConnectionStringConfig
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public Dictionary<string,string> Options { get; set; }
    }
}
