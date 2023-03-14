using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class ExpirationException : JPDataHubException
    {
        public ExpirationException()
        {
        }

        public ExpirationException(string message)
            : base(message)
        {
        }

        public ExpirationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
