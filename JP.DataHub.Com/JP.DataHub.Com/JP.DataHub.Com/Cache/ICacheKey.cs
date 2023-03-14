using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Cache
{
    public interface ICacheKey
    {
        public bool ModifyCacheKey(CacheKeyParam param);
    }
}
