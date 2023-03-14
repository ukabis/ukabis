using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IRoleRepository
    {
        bool IsExstis(Guid roleId);
        bool IsExstis(string roleId);
        RoleModel GetRole(Guid roleId);
        IList<RoleDetailModel> GetRoleDetail();
        IList<RoleDetailModel> GetRoleDetailEx(string openId);
        IList<RoleModel> GetRoleList();
        RoleModel RegistrationRole(RoleModel role);
        void UpdateRole(RoleModel role);
        void DeleteRole(Guid roleId);
        IList<AdminFuncInfomationModel> GetAdminFuncInfomationList();
    }
}
