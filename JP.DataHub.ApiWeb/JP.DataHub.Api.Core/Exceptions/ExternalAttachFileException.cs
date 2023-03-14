using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Api.Core.ErrorCode;

namespace JP.DataHub.Api.Core.Exceptions
{
    // .NET6
    [Serializable]
    public class ExternalAttachFileException : JPDataHubException
    {
        public ErrorCodeMessage.Code? ErrorCode { get; set; }

        public ExternalAttachFileException(ErrorCodeMessage.Code errorCode)
        {
            ErrorCode = errorCode;
        }

        public ExternalAttachFileException(ErrorCodeMessage.Code errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public ExternalAttachFileException(ErrorCodeMessage.Code errorCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
