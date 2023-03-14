using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using JP.DataHub.ApiWeb.Infrastructure.Configuration;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage
{
    internal class QueueStorage : AbstractAzureStorage, IQueueStorage
    {
        /// <summary>
        /// キュー操作クライアント
        /// </summary>
        private CloudQueueClient storageClient;

        public string ConnectionString { get; }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="connectionString">接続文字列</param>
        public QueueStorage(string connectionString) : base(new AzureStorageSetting(connectionString))
        {
            ConnectionString = connectionString;
            storageClient = storageAccount.CreateCloudQueueClient();
        }

        /// <summary>
        /// Get Queue.
        /// </summary>
        /// <param name="queueName">Queue Name</param>
        /// <returns>CloudQueue</returns>
        public CloudQueue GetQueue(string queueName)
        {
            CloudQueue queue = this.storageClient.GetQueueReference(queueName);
            var tmp = queue.CreateIfNotExistsAsync().Result;
            return queue;
        }

        /// <summary>
        /// En Queue.
        /// </summary>
        /// <param name="queueName">Queue Name</param>
        /// <param name="message">Message</param>
		public void EnQueue(string queueName, string message)
        {
            this.GetQueue(queueName).AddMessageAsync(new CloudQueueMessage(message));
        }

        /// <summary>
        /// En Queue.
        /// </summary>
        /// <param name="queueName">Queue Name</param>
        /// <param name="message">Message</param>
        /// <param name="initialVisibilityDelay">Initial Visibility Delay</param>
		public void EnQueue(string queueName, string message, TimeSpan initialVisibilityDelay)
        {
            //キューの生存期間は5日としておく。
            this.GetQueue(queueName).AddMessageAsync(new CloudQueueMessage(message), null, initialVisibilityDelay, null, null);
        }

        /// <summary>
        /// Check Exists Queue.
        /// </summary>
        /// <param name="queueName">Queue Name</param>
        /// <returns>ExistsQueue</returns>
		public bool ExistsQueue(string queueName)
        {
            CloudQueue queue = GetQueue(queueName);
            queue.FetchAttributesAsync();
            return queue.ApproximateMessageCount == null || queue.ApproximateMessageCount == 0 ? false : true;
        }
    }
}