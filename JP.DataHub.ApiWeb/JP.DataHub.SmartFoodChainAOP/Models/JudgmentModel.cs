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
    public class JudgmentModel
    {
        public string JudgmentHistoryId { get; set; }

        public string ProductCode { get; set; }

        public string GlnCode { get; set; }
        
        public JudgmentResultModel Result { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class JudgmentResultModel
    {
        public bool result { get; set; }

        public List<JasFreshnessJudgmentFailResult> fails { get; set; }
    }
}
