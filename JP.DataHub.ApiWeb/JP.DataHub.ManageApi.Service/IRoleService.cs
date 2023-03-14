using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Transaction.Attributes;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Attributes;

namespace JP.DataHub.ManageApi.Service
{
    [TransactionScope]
    [Authentication]
    public interface IRoleService
    {
        RoleModel GetRole(Guid roleId);
        IList<RoleModel> GetRoleList();
        RoleModel RegistrationRole(RoleModel role);
        void UpdateRole(RoleModel role);
        void DeleteRole(Guid roleId);
    }
}
