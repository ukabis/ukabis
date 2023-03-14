using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IAdminInfoRepository
    {
        AdminFuncInfomationModel RegistrationAdminFuncInfomation(AdminFuncInfomationModel funcInfo);
        AdminFuncInfomationModel GetAdminInfo(string adminFuncRoleId);
        void UpdateAdminFuncInfo(AdminFuncInfomationModel funcInfo);
        void DeleteAdminInfo(string adminFuncRoleId);
    }
}
