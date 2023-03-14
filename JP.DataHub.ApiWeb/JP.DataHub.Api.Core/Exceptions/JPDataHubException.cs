using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
	[Serializable]
	public class JPDataHubException : Exception
	{
		/// <summary>Create instance.</summary>
		public JPDataHubException()
			: base()
		{
		}

		/// <summary>Create instance with message.</summary>
		/// <param name="message"></param>
		public JPDataHubException(string message)
			: base(message)
		{
		}

		/// <summary>Create instance with message and innerException.</summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public JPDataHubException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
