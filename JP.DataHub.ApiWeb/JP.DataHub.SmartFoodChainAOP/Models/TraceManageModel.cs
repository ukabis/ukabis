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
    public class TraceManageModel
    {
        public string ProductCode { get; set; }

        public string Password { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class TraceManagePasswordModel
    {
        public string Password { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class TraceManageAdditionalOwenerIdModel : TraceManageModel
    {
        public string _Owner_Id { get; set; }
    }
}
