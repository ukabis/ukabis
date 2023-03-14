using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Repository;

namespace JP.DataHub.Api.Core.Service.Impl
{
    public class ServiceBusEventService : IServiceBusEventService
    {
        private Lazy<IServiceBusEventRepository> _lazyServiceBusRepository = new Lazy<IServiceBusEventRepository>(() => UnityCore.Resolve<IServiceBusEventRepository>("Trail"));
        private IServiceBusEventRepository _serviceBusRepository { get => _lazyServiceBusRepository.Value; }

        public Task SendObjectAsync(object obj)
            => _serviceBusRepository .SendObjectAsync(obj);

        public Task SendAsync(string message)
            => _serviceBusRepository .SendAsync(message);
    }
}
