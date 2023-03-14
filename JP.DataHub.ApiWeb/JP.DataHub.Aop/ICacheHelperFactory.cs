using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Aop
{
    // .NET6
    public interface ICacheHelperFactory
    {
        IAopCacheHelper Create(string type);
    }
}
