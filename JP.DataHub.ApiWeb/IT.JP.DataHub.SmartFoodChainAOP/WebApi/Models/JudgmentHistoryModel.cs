using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class JudgmentHistoryModel
    {
        //public string JudgmentHistoryId { get; set; }
        public string ProductCode { get; set; }
        public string GlnCode { get; set; }
        public string ArrivalId { get; set; }
        public JudgmentHistoryResultModel Result { get; set; }
    }

    public class JudgmentHistoryResultModel
    {
        public bool result { get; set; }
    }
}
