using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Cache
{
    public class CacheResult<T>
    {
        public bool IsHit { get; }

        public T Result { get; }

        public CacheResult(bool isHit, T result)
        {
            IsHit = isHit;
            Result = result;

        }
    }
}
