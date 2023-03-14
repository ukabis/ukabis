using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Configuration
{
    public class DbProviderFactoriesConfig : IDbProviderFactoriesConfig
    {
        public string Invariant { get; set; }
        public string DbType { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Init { get; set; }
    }
}
