using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    /// <summary>
    /// DynamicAPIのURLに設定できない例外
    /// </summary>
    [Serializable]
    public class NotParseCsvException : JPDataHubException
    {
        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public NotParseCsvException()
            : base()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        public NotParseCsvException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="innerException">Inner Exception</param>
        public NotParseCsvException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}