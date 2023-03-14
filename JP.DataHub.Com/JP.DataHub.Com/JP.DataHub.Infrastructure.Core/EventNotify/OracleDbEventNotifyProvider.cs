using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Core.Database;
using System;
using System.Threading.Tasks;
using System.Transactions;
using Unity;
using static JP.DataHub.Com.Configuration.EventNotiryConfig;

namespace JP.DataHub.Infrastructure.Core.EventNotify
{
    public class OracleDbEventNotifyProvider : IEventNotifyProvider
    {
        private readonly string _tableName;
        private readonly IJPDataHubDbConnection _dbConnection;
        
        public OracleDbEventNotifyProvider(EventNotiryConfigOracleDBSetting config, bool isMultithread = false)
        {
            _dbConnection = UnityCore.UnityContainer.Resolve<IJPDataHubDbConnection>(isMultithread ? config.ConnectionStringName + "-Multithread" : config.ConnectionStringName);
            _tableName = config.TableName;
        }

        public Task<string> NotifyAsync(string message, string category = null)
        {
            return Task.Run(() =>
            {
                var id = Guid.NewGuid().ToString();
                using (TransactionScope ts = new TransactionScope())
                {
                    string sql = @$"INSERT INTO {_tableName}(MESSAGE_ID,MESSAGE,CATEGORY,REG_DATE)VALUES(:MessageId,:Message,:Category,:Now)";
                    _dbConnection.Execute(sql, new { MessageId = id, Message = message, Category = category, Now = DateTime.UtcNow });
                    ts.Complete();
                }
                return id;
            });
        }
    }
}
