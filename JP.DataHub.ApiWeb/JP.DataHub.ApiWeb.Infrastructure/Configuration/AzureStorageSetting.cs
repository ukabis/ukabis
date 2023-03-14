using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Log;

namespace JP.DataHub.ApiWeb.Infrastructure.Configuration
{
    /// <summary>
    /// Azure Storage Setting.
    /// </summary>
    public class AzureStorageSetting
    {
        private JPDataHubLogger logger = new JPDataHubLogger(typeof(AzureStorageSetting));

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        public AzureStorageSetting()
        {
        }

        /// <summary>
        /// Initialize Instance.
        /// </summary>
        /// <param name="connectionString">ConnectionString</param>
        /// <param name="retryInterval">Retry Interval(ms)</param>
        /// <param name="retryCount">Retry Count</param>
        public AzureStorageSetting(string connectionString, int retryInterval = 100, int retryCount = 5)
        {
            this.ConnectionString = connectionString;
            this.RetryInterval = retryInterval;
            this.RetryCount = retryCount;
        }

        /// <summary>
        /// connectionStringsのname. CloudConfigurationManager.GetSetting(ConnectionStringSettingName); で接続文字列を取得する。
        /// </summary>
        /// <value>
        /// The name of the connection string setting.
        /// </value>
        public string ConnectionStringSettingName { get; private set; }

        /// <summary>
        /// AzureStorage接続文字列
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// リトライ間隔(ミリ秒)
        /// </summary>
        public int RetryInterval { get; private set; }

        /// <summary>
        /// リトライ回数
        /// </summary>
        public int RetryCount { get; private set; }
    }
}
