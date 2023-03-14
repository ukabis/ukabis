using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Log
{
    public interface ISimpleLogWriter
    {
        void Write(ILogEntry logEntry);
    }
}
