using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Exceptions
{
    [Serializable]
    public class MultipleUpdateException : Exception
    {
        public MultipleUpdateException()
        {
        }

        public MultipleUpdateException(string message)
            : base(message)
        {
        }

        public MultipleUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
