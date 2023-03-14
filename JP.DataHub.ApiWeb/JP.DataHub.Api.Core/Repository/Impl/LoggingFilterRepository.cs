using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Service;

namespace JP.DataHub.Api.Core.Repository.Impl
{
    internal class LoggingFilterRepository : ILoggingFilterRepository
    {
#if Oracle
        private static Lazy<IEventHubStreamingService> s_lazyEventHubConnection = new Lazy<IEventHubStreamingService>(() => UnityCore.Resolve<IEventHubStreamingService>("LoggingStreamingService"));
        private static IEventHubStreamingService s_eventHubConnection { get => s_lazyEventHubConnection.Value; }
#else
        private static Lazy<IJPDataHubEventHub> s_lazyEventHubConnection = new Lazy<IJPDataHubEventHub>(() => UnityCore.Resolve<IJPDataHubEventHub>("Logging"));
        private static IJPDataHubEventHub s_eventHubConnection { get => s_lazyEventHubConnection.Value; }
#endif

        public void Write(ApiRequestResponseLogModel model)
        {
            var tmp = s_eventHubConnection.SendMessageAsync(model.ToJson()).Result;
        }
    }
}
