using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.Storage
{
    public class StorageResult<TValue>
    {
        public bool Success { get; }
        public TValue? Value { get; }

        public StorageResult()
        {
            Success = false;
        }

        public StorageResult(bool success, TValue? val)
        {
            Success = success;
            Value = val;
        }
    }
}
