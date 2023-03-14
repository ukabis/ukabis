using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Polly;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;
using AutoMapper;
using Oci.Common;
using Oci.Common.Auth;
using Oci.Common.Waiters;
using Oci.StreamingService;
using Oci.StreamingService.Models;
using Oci.StreamingService.Requests;
using Oci.StreamingService.Responses;

namespace JP.DataHub.Com.Transaction
{
    public class EventHubStreamingService : IEventHubStreamingService
    {
        public string ConnectionString { get; set; }

        private JPDataHubLogger _logger = new JPDataHubLogger(typeof(EventHubStreamingService));

        private string _confiFilePath;
        private string _profile;
        private string _pemFilePath;
        private string _streamOcid;
        private string _streamEndPoint;

        public EventHubStreamingService() { }

        public EventHubStreamingService(string configFilePath, string profile, string pemFilePath, string ocid, string endPoint)
        {
            ConnectionString = $"ConfigurationFilePath={configFilePath};Profile={profile};PemFilePath={pemFilePath};EndPoint={endPoint};Ocid={ocid}";
        }

        public async Task<bool> SendMessageAsync(JToken message, string partitionKey = null)
        {
            foreach (var part in ConnectionString.Split(';'))
            {
                var parts = part.Split('=');
                switch (parts[0])
                {
                    case "ConfigurationFilePath":
                        _confiFilePath = parts[1];
                        break;
                    case "Profile":
                        _profile = parts[1];
                        break;
                    case "PemFilePath":
                        _pemFilePath = parts[1];
                        break;
                    case "EndPoint":
                        _streamEndPoint = parts[1];
                        break;
                    case "Ocid":
                        _streamOcid = parts[1];
                        break;
                }
            }
            // コンフィグ設定取得
            IConfiguration appConfig = UnityCore.Resolve<IConfiguration>().GetSection("AppConfig");
            int retryCount = appConfig.GetValue<int>("EventHubStreamingService:RetryCount", 1);
            TimeSpan retrySpan = appConfig.GetValue<TimeSpan>("EventHubStreamingService:RetrySpan", new TimeSpan(0, 1, 30));

            try
            {
                var pemFile = new FilePrivateKeySupplier(_pemFilePath, null);
                var provider = new ConfigFileAuthenticationDetailsProvider(_confiFilePath, _profile, pemFile);

                using var streamClient = new StreamClient(provider);
                streamClient.SetEndpoint(_streamEndPoint);

                await Policy.HandleResult<Task<bool>>(success => !success.Result)
                .WaitAndRetry(retryCount, i => retrySpan)
                .Execute(async () =>
                {
                    var messages = new List<PutMessagesDetailsEntry>();
                    var detailsEntry = new PutMessagesDetailsEntry
                    {
                        Key = !string.IsNullOrEmpty(partitionKey)
                        ? Encoding.UTF8.GetBytes(partitionKey)
                        : null,
                        Value = Encoding.UTF8.GetBytes(message.ToString())
                    };
                    messages.Add(detailsEntry);
                    var putRequest = new PutMessagesRequest
                    {
                        StreamId = _streamOcid,
                        PutMessagesDetails = new PutMessagesDetails { Messages = messages }
                    };

                    PutMessagesResponse putResponse = await streamClient.PutMessages(putRequest);

                    foreach (PutMessagesResultEntry entry in putResponse.PutMessagesResult.Entries)
                    {
                        if (!string.IsNullOrEmpty(entry.Error))
                        {
                            _logger.Error($"Error({entry.Error}): {entry.ErrorMessage}");
                            throw new Exception();
                        }
                        else
                        {
                            // HACK : 送信成功時のメッセージが必要であれば然るべき場所に出力する。
                            //Console.WriteLine($"Published message to partition {entry.Partition}, offset {entry.Offset}");
                        }
                    }
                    return true;
                });
            }
            catch (Exception ex)
            {
                string partitionKeyValue = string.IsNullOrEmpty(partitionKey) ? "null" : partitionKey;
                _logger.Error($"EventHubStreamingService Write Error; message：【{message}】 ,partitionKey：【{partitionKeyValue}】", ex.ToString());
                throw;
            }
            return true;
        }
    }
}
