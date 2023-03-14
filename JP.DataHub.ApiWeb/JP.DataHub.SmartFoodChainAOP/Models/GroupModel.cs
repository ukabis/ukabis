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
    public class GroupModel
    {
        public enum Scope
        {
            All,
            SmartFoodChain,
            Sensor,
            FMIS
        }

        public string groupId { get; set; }

        public List<string> scope { get; set; }

        public string groupName { get; set; }

        public MemberModel representativeMember { get; set; }

        public List<MemberModel> member { get; set; }

        public List<string> manager { get; set; }

        public string CompanyId { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class GroupWithOwnerIdModel : GroupModel
    {
        public string _Owner_Id { get; set; }
    }
}
