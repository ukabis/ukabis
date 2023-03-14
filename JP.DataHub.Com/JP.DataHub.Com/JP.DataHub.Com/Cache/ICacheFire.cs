using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Cache
{
    public interface ICacheFire
    {
        IEnumerable<string> CacheKeys(IEnumerable<string> keys);
    }
}
