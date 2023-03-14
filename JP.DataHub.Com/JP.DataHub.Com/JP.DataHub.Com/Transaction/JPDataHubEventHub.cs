using System.Configuration;
using System.Text;
using System;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Polly;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.Transaction
{
    public class JPDataHubEventHub : IJPDataHubEventHub
    {
        public string ConnectionString { get; set; }

        private JPDataHubLogger _logger = new JPDataHubLogger(typeof(JPDataHubEventHub));

        public JPDataHubEventHub(string connectionString = null)
        {
            ConnectionString = connectionString;
        }

        public async Task<bool> SendMessageAsync(JToken message, string partitionKey = null)
        {
            var appConfig = UnityCore.Resolve<IConfiguration>().GetSection("AppConfig");
            var retryCount = appConfig.GetValue<int>("EventHub:RetryCount", 1);
            var retrySpan = appConfig.GetValue<TimeSpan>("EventHub:RetrySpan", TimeSpan.Parse("00:01:30"));

            var parseString = new ParseConnectionString(ConnectionString);
            var connectionString = $"Endpoint={parseString["Endpoint"]};SharedAccessKeyName={parseString["SharedAccessKeyName"]};SharedAccessKey={parseString["SharedAccessKey"]}";
            var eventhubName = parseString["EntityPath"];

            try
            {
                await Policy.HandleResult<Task<bool>>(success => !success.Result)
                    .WaitAndRetry(retryCount, i => retrySpan)
                    .Execute(async () =>
                    {
                        await using var client = new EventHubProducerClient(connectionString, eventhubName);
                        using var eventBatch = string.IsNullOrEmpty(partitionKey)
                            ? await client.CreateBatchAsync()
                            : await client.CreateBatchAsync(new CreateBatchOptions() { PartitionKey = partitionKey });

                        eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(message.ToString())));
                        await client.SendAsync(eventBatch);
                        return true;
                    });
            }
            catch (Exception ex)
            {
                _logger.Error("EventHub Write Error", ex);
                throw;
            }

            return true;
        }
    }
}