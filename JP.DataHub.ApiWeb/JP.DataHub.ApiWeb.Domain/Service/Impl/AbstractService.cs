using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;

namespace JP.DataHub.ApiWeb.Domain.Service.Impl
{
    internal class AbstractService
    {
        private readonly Lazy<IPerRequestDataContainer> _requestDataContainer = new Lazy<IPerRequestDataContainer>(
            () =>
            {
                try
                {
                    return UnityCore.Resolve<IPerRequestDataContainer>();
                }
                catch
                {
                    // ignored
                    return UnityCore.Resolve<IPerRequestDataContainer>("multiThread");
                }
            });
        protected IPerRequestDataContainer PerRequestDataContainer => _requestDataContainer.Value;
    }
}
