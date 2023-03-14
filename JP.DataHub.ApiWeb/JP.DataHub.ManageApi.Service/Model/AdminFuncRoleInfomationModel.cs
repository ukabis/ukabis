using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Serializable]
    public class AdminFuncRoleInfomationModel
    {
        public Guid RoleId { get; set; }

        public string RoleName { get; set; }

        public IList<AdminFuncInfomationModel> AdminFuncInfoList { get; set; }
    }
}
