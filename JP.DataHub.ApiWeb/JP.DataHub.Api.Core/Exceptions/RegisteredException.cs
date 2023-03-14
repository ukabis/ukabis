using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class RegisteredException : JPDataHubException
    {
        public RegisteredException()
        {
        }

        public RegisteredException(string message)
            : base(message)
        {
        }

        public RegisteredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
