using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.ServiceBus;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    public class ServiceBusEventRepository : IServiceBusEventRepository
    {
        /// <summary>
        /// ログ出力する何か
        /// </summary>
        private static readonly JPDataHubLogger s_logger = new JPDataHubLogger(typeof(ServiceBusEventRepository));

        private string _connectionString;
        private bool _isEnable;

        public ServiceBusEventRepository(string connectionString, bool isEnable = true)
        {
            _connectionString = connectionString;
            _isEnable = isEnable;
        }

        public async Task SendObjectAsync(object obj)
            => SendAsync(obj.ToJsonString());

        public async Task SendAsync(string message)
        {
            if (_isEnable == false)
            {
                return;
            }

            // ServiceBusClientのインスタンス化のコストは低いため都度作成で問題なし
            var pcs = new ParseConnectionString(_connectionString);
            var serviceBusClient = new ServiceBusClient(pcs.ToServiceBusConnectionString());
            var serviceBusSender = serviceBusClient.CreateSender(pcs.ToQueueName());
            var messageBatch = await serviceBusSender.CreateMessageBatchAsync();
            messageBatch.TryAddMessage(new ServiceBusMessage(message));
            try
            {
                await serviceBusSender.SendMessagesAsync(messageBatch);
            }
            catch (Exception e)
            {
                s_logger.Error(e.Message);
                s_logger.Error(e.StackTrace);
            }
            finally
            {
                await serviceBusSender.DisposeAsync();
                await serviceBusClient.DisposeAsync();
            }
        }
    }
}
