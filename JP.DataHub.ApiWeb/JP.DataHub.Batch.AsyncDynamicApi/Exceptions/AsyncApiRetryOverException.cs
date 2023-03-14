using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.AsyncDynamicApi.Exceptions
{
    public class AsyncApiRetryOverException : Exception
    {
		public AsyncApiRetryOverException()
			: base()
		{
		}

		/// <summary>
		/// Initialize Instance.
		/// </summary>
		/// <param name="message">Error Message</param>
		public AsyncApiRetryOverException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initialize Instance.
		/// </summary>
		/// <param name="message">Error Message</param>
		/// <param name="innerException">Inner Exception</param>
		public AsyncApiRetryOverException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
