using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using Unity.Resolution;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.Data;
using JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    internal class NewQueueStoreRepository : NewAbstractDynamicApiDataStoreRepository
    {
        private IQueueStorage QueueStorage { get; set; }

        public const string CONNECTIONSTRING_KEY_DEFAULTENDPOINTSPROTOCOL = "DefaultEndpointsProtocol";
        public const string CONNECTIONSTRING_KEY_ACCOUNTNAME = "AccountName";
        public const string CONNECTIONSTRING_KEY_ACCOUNTKEY = "AccountKey";
        public const string CONNECTIONSTRING_KEY_ENDPOINTSUFFIX = "EndpointSuffix";
        public const string CONNECTIONSTRING_KEY_QUEUENAME = "QueueName";

        /// <summary>
        /// 楽観排他機能が利用できるか
        /// </summary>
        public override bool CanOptimisticConcurrency => false;

        public override JsonDocument QueryOnce(QueryParam param)
        {
            throw new NotImplementedException();
        }

        public override IList<JsonDocument> Query(QueryParam param, out XResponseContinuation xResponseContinuation)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<JsonDocument> QueryEnumerable(QueryParam param)
        {
            yield break;
        }

        public override RegisterOnceResult RegisterOnce(RegisterParam param)
        {
            var pcs = new ParseConnectionString(RepositoryInfo.ConnectionString);
            QueueStorage = UnityCore.Resolve<IQueueStorage>(new ParameterOverride(QueueStorageConst.DEFAULTCONSTRUCTOR_ARG1_NAME, pcs.CreateConnectionString(new string[] { CONNECTIONSTRING_KEY_DEFAULTENDPOINTSPROTOCOL, CONNECTIONSTRING_KEY_ACCOUNTNAME, CONNECTIONSTRING_KEY_ACCOUNTKEY, CONNECTIONSTRING_KEY_ENDPOINTSUFFIX })));
            QueueStorage.EnQueue(pcs[CONNECTIONSTRING_KEY_QUEUENAME], param.Json.ToString());
            return null;
        }

        public override void DeleteOnce(DeleteParam param)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> Delete(DeleteParam param)
        {
            throw new NotImplementedException();
        }
    }
}
