using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Api.Core.Service;

namespace JP.DataHub.Api.Core.Filters
{
    internal interface IUserRoleValudator
    {
        bool CheckModel(string openId, string vendorId, string systemId, string functionName, UserRoleAccessType access, IDictionary<string, object?> arguments);
    }
}
