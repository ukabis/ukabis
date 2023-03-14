using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class AlreadyExistsException : DomainException
    {
        public AlreadyExistsException()
            : base()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        public AlreadyExistsException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Inner Exception</param>
        public AlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
