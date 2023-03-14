using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Exceptions;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class InUseException : JPDataHubException
    {
        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public InUseException()
            : base()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        public InUseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Inner Exception</param>
        public InUseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
