using System.Collections.Generic;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Service;

namespace JP.DataHub.ManageApi.Validators
{
    public class SystemIdModelValidator : IUserRoleValudator
    {
        public bool CheckModel(string openId, string vendorId, string systemId, string functionName, UserRoleAccessType access, IDictionary<string, object?> arguments)
        {
            return false;
        }
    }
}
