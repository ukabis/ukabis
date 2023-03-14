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
    public class MemberModel
    {
        public string openId { get; set; }

        public string mailAddress { get; set; }

        public List<string> accessControl { get; set; }

        public override bool Equals(object obj)
        {
            return obj is MemberModel model &&
                   openId == model.openId &&
                   mailAddress == model.mailAddress &&
                   accessControl?.SequenceEqual(model.accessControl) == true;
        }

        public override int GetHashCode()
        {
            int hashCode = 1189137278;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(openId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(mailAddress);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(accessControl);
            return hashCode;
        }
    }
}
