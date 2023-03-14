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
    public class JasFreshnessJudgmentResultModel
    {
        public string ProductCode { get; set; }
        public string GlnCode { get; set; }
        public JasFreshnessJudgmentResultResult Result { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class JasFreshnessJudgmentResultResult
    {
        public bool result { get; set; }
        public List<JasFreshnessJudgmentFailResult> fails { get; set; }
    }
}
