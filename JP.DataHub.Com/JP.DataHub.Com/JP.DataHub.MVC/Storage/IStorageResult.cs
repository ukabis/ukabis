using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.Storage
{
    public interface IStorageResult<TValue>
    {
        bool Success { get; }
        TValue? Value { get; }
    }
}
