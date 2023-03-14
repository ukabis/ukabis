using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    internal static class NotificationServiceFactory
    {
        public static INotificationService Create(string type, string connectionString)
        {
            switch (type?.ToLower())
            {
                case "eventhub":
                    return new EventHubNotificationService(connectionString);
                case "servicebus":
                    return new ServiceBusNotificationService(connectionString);
                case "nothing":
                    return new NothingNotificationService(connectionString);
                case "objectstorage":
                    return new StreamingServiceNotificationService(connectionString);
                default:
                    throw new Exception($"unkown INotificationService. Name {type}");
            }
        }
    }
}
