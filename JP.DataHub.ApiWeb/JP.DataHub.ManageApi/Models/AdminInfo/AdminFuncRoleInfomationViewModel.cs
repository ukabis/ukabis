using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.AdminInfo
{
    public class AdminFuncRoleInfomationViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<AdminFuncInfomationViewModel> AdminFuncInfoList { get; set; }
    }
}
