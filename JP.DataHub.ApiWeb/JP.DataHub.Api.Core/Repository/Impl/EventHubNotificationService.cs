using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs.Producer;
using Azure.Messaging.EventHubs;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Transaction;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    internal class EventHubNotificationService : AbstractNotificationService, INotificationService
    {
        private static readonly JPDataHubLogger s_logger = new JPDataHubLogger(typeof(EventHubNotificationService));

        private string _connectionString;

        public EventHubNotificationService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public override async Task SendAsync(string message)
        {
            var pcs = new ParseConnectionString(_connectionString);
            var eventhubName = pcs.GetValue("EntityPath");
            var partitionKey = pcs.GetValue("PartitionKey");
            var connectionString = pcs.GetWithoutValue("EntityPath", "PartitionKey");
            await using var client = new EventHubProducerClient(_connectionString, eventhubName);
            using var eventBatch = string.IsNullOrEmpty(partitionKey)
            ? await client.CreateBatchAsync()
                : await client.CreateBatchAsync(new CreateBatchOptions() { PartitionKey = partitionKey });

            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(message.ToString())));
            await client.SendAsync(eventBatch);
        }
    }
}
