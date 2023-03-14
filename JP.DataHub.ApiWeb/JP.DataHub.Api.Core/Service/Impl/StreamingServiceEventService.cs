using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Repository;

namespace JP.DataHub.Api.Core.Service.Impl
{
    public class StreamingServiceEventService : IStreamingServiceEventService
    {
        private Lazy<IStreamingServiceEventRepository> _lazyStreamingServiceRepository = new Lazy<IStreamingServiceEventRepository>(() => UnityCore.Resolve<IStreamingServiceEventRepository>("TrailOci"));
        private IStreamingServiceEventRepository _streamingServiceRepository { get => _lazyStreamingServiceRepository.Value; }

        public Task SendObjectAsync(object obj)
            => _streamingServiceRepository.SendObjectAsync(obj);

        public Task SendAsync(string message)
            => _streamingServiceRepository.SendAsync(message);
    }
}
