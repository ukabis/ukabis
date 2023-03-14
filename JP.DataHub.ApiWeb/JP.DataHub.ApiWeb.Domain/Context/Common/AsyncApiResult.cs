using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record AsyncApiResult : IValueObject
    {
        public string RequestId { get; }

        public AsyncStatus Status { get; }

        public DateTime? RequestDate { get; }

        public DateTime? EndDate { get; }

        public string ResultPath { get; }

        public AsyncApiResult(string requestId, string status, DateTime? requestDate, DateTime? endDate, string resultPath)
        {
            RequestId = requestId;

            AsyncStatus parseAsyncStatus;
            if (Enum.TryParse<AsyncStatus>(status, out parseAsyncStatus))
            {
                Status = parseAsyncStatus;
            }
            else
            {
                throw new FormatException($"Status is not Valid");
            }
            RequestDate = requestDate;
            EndDate = endDate;
            ResultPath = resultPath;
        }

        public static bool operator ==(AsyncApiResult me, object other) => me?.Equals(other) == true;

        public static bool operator !=(AsyncApiResult me, object other) => !me?.Equals(other) == true;
    }
}
