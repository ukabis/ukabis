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
    public class JasPrintLogModel
    {
        public string LastGln { get; set; }

        public string CompanyId { get; set; }

        public int PrintCount { get; set; }

        public DateTime PrintDate { get; set; }

        public string PrinterId { get; set; }

        public string PrintUser { get; set; }

        public string ProductCode { get; set; }

        public int ReprintCount { get; set; }

        public string ReprintReason { get; set; }

        public string OpenId { get; set; }

        public string ArrivalId { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class GetJasPrintLogModel : JasPrintLogModel
    {
        public string _Owner_Id { get; set; }
    }
}
