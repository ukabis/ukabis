using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Service
{
    internal interface IUserRoleCheckService
    {
        bool Check(string openId, string vendorId, string systemId, string functionName, UserRoleAccessType access, bool isVendorAccess, IDictionary<string, object?> arguments);
        bool CheckModel(string vendorId, string systemId, bool isVendorAccess, object model);
    }
}
