using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    /// <summary>
    ///  Exception for Domain.
    /// </summary>
    [Serializable]
    public class DomainException : JPDataHubException
    {
        /// <summary>
        /// Initialize Instance.
        /// </summary>
		public DomainException()
            : base()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
		public DomainException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Inner Exception</param>
		public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
