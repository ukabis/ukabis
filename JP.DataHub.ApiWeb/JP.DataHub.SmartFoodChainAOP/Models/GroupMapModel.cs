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
    public class GroupMapModel
    {
        public GroupAccountModel Account { get; set; }

        public string groupId { get; set; }

        public string groupMapId { get; set; }

        public string ReservedAccountId { get; set; }
    }
}
