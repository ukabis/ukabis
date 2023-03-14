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
    public interface IAdminInfoService
    {
        AdminFuncInfomationModel RegistrationAdminFuncInfomation(AdminFuncInfomationModel funcInfo);
        AdminFuncInfomationModel GetAdminInfo(string adminFuncRoleId);
        IList<AdminFuncRoleInfomationModel> GetAdminFuncRoleInfomationList();
        void UpdateAdminFuncInfo(AdminFuncInfomationModel funcInfo);
        void DeleteAdminInfo(string adminFuncRoleId);
    }
}
