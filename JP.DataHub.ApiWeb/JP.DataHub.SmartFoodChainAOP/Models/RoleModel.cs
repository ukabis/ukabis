using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Models
{
    [MessagePackObject(true)]
    [Serializable]
    public class RoleModel
    {
        public string PrivateRoleId { get; set; }
        public string RoleName { get; set; }
        public string ApplicationId { get; set; }
        public List<string> Functions { get; set; }
    }
}
