using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class RoleDetailModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string FuncName { get; set; }
        public bool IsRead { get; set; }
        public bool IsWrite { get; set; }
    }
}
