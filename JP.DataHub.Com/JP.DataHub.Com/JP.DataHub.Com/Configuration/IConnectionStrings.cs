using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Configuration
{
    public interface IConnectionStrings : IList<ConnectionStringConfig>
    {
        bool IsTransactionManage { get; set; }
        bool IsTransactionScope { get; set; }
        string LifeTimeManager { get; set; }
    }
}
