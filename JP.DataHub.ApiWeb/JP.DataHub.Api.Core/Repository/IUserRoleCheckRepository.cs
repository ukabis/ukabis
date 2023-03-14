using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Repository
{
    internal interface IUserRoleCheckRepository
    {
        IList<AccessRight> GetAllAccessRights(string openId);
    }
}
