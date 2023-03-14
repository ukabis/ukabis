using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Repository;

namespace JP.DataHub.Api.Core.Service.Impl
{
    internal class LoggingFilterService : ILoggingFilterService
    {
        private Lazy<ILoggingFilterRepository> _lazyLoggingRepository => new Lazy<ILoggingFilterRepository>(() => UnityCore.Resolve<ILoggingFilterRepository>());
        private ILoggingFilterRepository _loggingRepository { get => _lazyLoggingRepository.Value; }

        public void Write(ApiRequestResponseLogModel model)
            => _loggingRepository.Write(model);
    }
}
