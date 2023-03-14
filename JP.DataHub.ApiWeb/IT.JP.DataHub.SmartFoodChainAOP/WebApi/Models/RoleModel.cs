using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class RoleModel
    {
        public string ApplicationId { get; set; }
        public string PrivateRoleId { get; set; }
        public string RoleName { get; set; }
        public string[] Functions{ get; set; }
    }
}
