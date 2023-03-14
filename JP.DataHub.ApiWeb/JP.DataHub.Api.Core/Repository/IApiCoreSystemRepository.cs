using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Repository
{
    internal interface IApiCoreSystemRepository
    {
        string SystemIdToVendorId(string systemId);
    }
}
