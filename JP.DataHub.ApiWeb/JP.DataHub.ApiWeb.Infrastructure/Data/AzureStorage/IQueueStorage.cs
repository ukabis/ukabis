using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage
{
    public interface IQueueStorage
    {
        string ConnectionString { get; }

        void EnQueue(string queueName, string message);
        void EnQueue(string queueName, string message, TimeSpan initialVisibilityDelay);
        bool ExistsQueue(string queueName);
    }

    public static class QueueStorageConst
    {
        public const string DEFAULTCONSTRUCTOR_ARG1_NAME = "connectionString";
    }
}
