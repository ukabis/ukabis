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
    public class ReservedAccountModel
    {
        public GroupAccountModel Account { get; set; }

        public bool IsUsed { get; set; }

        public string ReservedAccountId { get; set; }

        public string _etag { get; set; }
    }
}
