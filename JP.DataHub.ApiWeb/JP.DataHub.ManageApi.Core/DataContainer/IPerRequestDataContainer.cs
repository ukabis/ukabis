using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Api.Core.DataContainer;

namespace JP.DataHub.ManageApi.Core.DataContainer
{
    public interface IPerRequestDataContainer : IApiDataContainer
    {
        bool IsOperatingVendorUser { get; }
        Dictionary<string, List<string>> RequestHeaders { get; set; }
    }
}
