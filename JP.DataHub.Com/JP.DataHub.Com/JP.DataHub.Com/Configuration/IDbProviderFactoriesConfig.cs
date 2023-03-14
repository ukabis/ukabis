using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Configuration
{
    public interface IDbProviderFactoriesConfig
    {
        string Invariant { get; set; }
        string DbType { get; set; }
        string Type { get; set; }
        Dictionary<string,string> Init { get; set; }
    }
}
