using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class InvalidAccountFormat : JPDataHubException
    {
        private static string errMsg = "invalid account format.";

        public InvalidAccountFormat() : base(errMsg) { }

        public InvalidAccountFormat(string message) : base(message) { }

        public InvalidAccountFormat(string message, Exception innerException) : base(errMsg, innerException) { }
    }
}