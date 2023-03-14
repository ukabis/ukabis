using Dapper.Oracle;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Core.Database;
using System;
using System.Data;
using System.Threading.Tasks;
using Unity;
using static JP.DataHub.Com.Configuration.EventNotiryConfig;

namespace JP.DataHub.Infrastructure.Core.EventNotify
{
    public class OracleAqEventNotifyProvider : IEventNotifyProvider
    {
        private readonly IJPDataHubDbConnection _dbConnection;
        private readonly string _subscriberName;
        private readonly string _queueName;

        public OracleAqEventNotifyProvider(EventNotiryConfigOracleAqSetting config, bool isMultithread = false)
        {
            _dbConnection = UnityCore.UnityContainer.Resolve<IJPDataHubDbConnection>(isMultithread ? config.ConnectionStringName + "-Multithread" : config.ConnectionStringName);
            _subscriberName = config.SubscriberName;
            _queueName = config.QueueName;
        }

        public async Task<string> NotifyAsync(string message, string category = null)
        {
            string id = Guid.NewGuid().ToString();
            var sql = $"JP_DATAHUB_EVENT.AQ_ENQUEUE";
            var dynamicParameters = new OracleDynamicParameters();
            dynamicParameters.Add(name: ":QueueName", _queueName, dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Input);
            dynamicParameters.Add(name: ":Subscriber", _subscriberName, dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Input);
            dynamicParameters.Add(name: ":MessageId", id, dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Input);
            dynamicParameters.Add(name: ":Massage", message, dbType: OracleMappingType.NVarchar2, direction: ParameterDirection.Input);
            
            await _dbConnection.ExecuteAsync(sql, param: dynamicParameters, commandType: CommandType.StoredProcedure);

            return id;
        }
    }
}
