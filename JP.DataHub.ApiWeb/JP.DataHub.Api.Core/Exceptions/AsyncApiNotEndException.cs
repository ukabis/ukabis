using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class AsyncApiNotEndException : DomainException
    {
        public AsyncApiNotEndException()
        {
        }

        public AsyncApiNotEndException(string message) : base(message)
        {
        }

        public AsyncApiNotEndException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
