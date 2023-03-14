using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class AsyncApiStatusNotFoundException : DomainException
    {
        public AsyncApiStatusNotFoundException()
        {
        }

        public AsyncApiStatusNotFoundException(string message) : base(message)
        {
        }

        public AsyncApiStatusNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
