using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class ODataInvalidFilterColumnException : DomainException
    {
        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public ODataInvalidFilterColumnException()
            : base()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        public ODataInvalidFilterColumnException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Inner Exception</param>
        public ODataInvalidFilterColumnException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
