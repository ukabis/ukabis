using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class AdminFuncInfomationModel
    {
        public string AdminFuncId { get; set; }
        public string AdminName { get; set; }
        public string AdminFuncRoleId { get; set; }
        public string RoleId { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
        public string DisplayDescription { get; set; }
    }
}
