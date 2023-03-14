using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Exceptions
{
    [Serializable]
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public NotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        public NotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Inner Exception</param>
        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}