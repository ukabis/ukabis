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
    public class ApplicationUserModel
    {
        public string ApplicationUserId { get; set; }
        public string ApplicationId { get; set; }
        public string OpenId { get; set; }
        public List<string> PrivateRoleId { get; set; }
        public List<string> Functions { get; set; }
    }
}
