using System;
using System.Runtime.Serialization;

namespace JP.DataHub.Com.Exceptions
{
    [Serializable]
    public class JPDataHubException : Exception
    {
        public JPDataHubException()
        {
        }

        public JPDataHubException(string message) : base(message)
        {
        }

        public JPDataHubException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected JPDataHubException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
