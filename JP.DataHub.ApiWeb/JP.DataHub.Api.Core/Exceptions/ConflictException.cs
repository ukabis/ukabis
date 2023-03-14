using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class ConflictException : JPDataHubException
    {
        public ConflictException(string message = null)
            : base(message)
        {
        }
    }
}
