using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class AsyncApiResultNotFoundException : DomainException
    {
        public AsyncApiResultNotFoundException()
        {
        }

        public AsyncApiResultNotFoundException(string message) : base(message)
        {
        }

        public AsyncApiResultNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
