using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.Data.EventHub;
using Newtonsoft.Json;
using System.Net;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    internal class NewEventHubStoreRepository : NewAbstractDynamicApiDataStoreRepository
    {
#if Oracle
        protected IEventHubStreamingService EventHub => _lazyEventHub.Value;
        private Lazy<IEventHubStreamingService> _lazyEventHub = new Lazy<IEventHubStreamingService>(() => UnityCore.Resolve<IEventHubStreamingService>());
#else
        protected IJPDataHubEventHub EventHub => _lazyEventHub.Value;
        private Lazy<IJPDataHubEventHub> _lazyEventHub = new Lazy<IJPDataHubEventHub>(() => UnityCore.Resolve<IJPDataHubEventHub>());
#endif

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
            EventHub.ConnectionString = RepositoryInfo.ConnectionString;
            var prtition = EventHubPartitionKey.CreateRegisterPartition(param.PartitionKey, param.IsVendor, param.VendorId, param.SystemId, param.Json);
            _ = EventHub.SendMessageAsync(param.Json, prtition?.Value ?? "").Result;
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
