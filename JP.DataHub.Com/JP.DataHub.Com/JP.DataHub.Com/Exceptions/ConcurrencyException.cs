using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Exceptions
{
    [Serializable]
    public class ConcurrencyException : JPDataHubException
    {
        public ConcurrencyException()
            : base()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        public ConcurrencyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Inner Exception</param>
        public ConcurrencyException(string message, Exception innerException)
#if (DEBUG)
            : base(message, innerException)
#else
            : base(message)
#endif
        {
        }
    }
}
