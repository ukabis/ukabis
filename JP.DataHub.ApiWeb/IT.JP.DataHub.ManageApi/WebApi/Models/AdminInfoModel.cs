using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models
{
    [Serializable]
    public class AdminInfoModel
    {
        public string AdminFuncId { get; set; }
        public string AdminName { get; set; }
        public string AdminFuncRoleId { get; set; }
        public string RoleId { get; set; }
        public string IsRead { get; set; }
        public string IsWrite { get; set; }
        public string DisplayDescription { get; set; }
    }
    [Serializable]
    public class AdminFuncRoleInfomationModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<AdminInfoModel> AdminFuncInfoList { get; set; }
    }
}
