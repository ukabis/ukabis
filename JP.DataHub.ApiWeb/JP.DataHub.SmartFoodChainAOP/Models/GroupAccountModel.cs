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
    public class GroupAccountModel
    {
        public string Account { get; set; }

        public string DisplayName { get; set; }

        public string OpenId { get; set; }

        public string Password { get; set; }
    }
}
