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
    public class IsAuthorizationResult
    {
        public string OpenId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public bool Result { get; set; }
        public List<string> FunctionList { get; set; }
    }
}
