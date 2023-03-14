using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.DataContainer;

namespace JP.DataHub.ManageApi.Core.DataContainer
{
    public class PerRequestDataContainer : ApiDataContainer, IPerRequestDataContainer
    {
        public bool IsOperatingVendorUser { get => (UnityCore.Resolve<IList<string>>("OperatingVendorVendorId").Contains(VendorId.ToLower())); }
        public Dictionary<string, List<string>> RequestHeaders { get; set; }
    }
}
