
namespace JP.DataHub.OData.Interface.Exceptions
{
	[Serializable]
	public class JPDataHubODataException : Exception
	{
		/// <summary>Create instance.</summary>
		public JPDataHubODataException()
			: base()
		{
		}

		/// <summary>Create instance with message.</summary>
		/// <param name="message"></param>
		public JPDataHubODataException(string message)
			: base(message)
		{
		}

		/// <summary>Create instance with message and innerException.</summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public JPDataHubODataException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
