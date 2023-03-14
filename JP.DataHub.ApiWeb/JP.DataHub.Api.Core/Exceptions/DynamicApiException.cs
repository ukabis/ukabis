using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class DynamicApiException : Exception
    {
        private const string DefaultMessage = "Failed to Edit Document.";

        /// <summary>
        /// Enum of ErrorType.
        /// </summary>
        public enum ErrorTypeEnum
        {

            VersionInfoNotFound,
            ConflictVersionInfo,
            NotFound,
        }
        /// <summary>ErrorType</summary>
        public ErrorTypeEnum ErrorType { get; }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="errorType">ErrorType</param>
        public DynamicApiException(ErrorTypeEnum errorType)
            : base(DefaultMessage)
        {
            this.ErrorType = errorType;
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="errorType">ErrorType</param>
        /// <param name="inner">Inner Exception</param>
        public DynamicApiException(ErrorTypeEnum errorType, Exception inner)
            : base(DefaultMessage, inner)
        {
            this.ErrorType = errorType;
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="errorType">ErrorType</param>
        public DynamicApiException(string message, ErrorTypeEnum errorType)
            : base(message)
        {
            this.ErrorType = errorType;
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <param name="errorType">ErrorType</param>
        /// <param name="inner">Inner Exception</param>
        public DynamicApiException(string message, ErrorTypeEnum errorType, Exception inner)
            : base(message, inner)
        {
            this.ErrorType = errorType;
        }

        /// <summary>
        /// Get ObjectData.
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ErrorType", ErrorType);
        }
    }
}
