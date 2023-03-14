using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Exceptions
{
    [Serializable]
    public class ForeignKeyException : JPDataHubException
    {
        public ForeignKeyException()
            : base()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        public ForeignKeyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Inner Exception</param>
        public ForeignKeyException(string message, Exception innerException)
#if (DEBUG)
            : base(message, innerException)
#else
            : base(message)
#endif
        {
        }
    }
}
