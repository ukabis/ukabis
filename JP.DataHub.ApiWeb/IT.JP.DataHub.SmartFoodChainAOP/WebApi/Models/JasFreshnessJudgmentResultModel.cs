using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class JasFreshnessJudgmentResultModel
    {
        public bool result { get; set; }
        public List<JasFreshnessJudgmentFailModel> fails { get; set; }
    }

    public class JasFreshnessJudgmentFailModel
    {
        public string[] ObservedPropertiesCode { get; set; }
        public string MeasurementUnitId { get; set; }
        public string DatastreamId { get; set; }
        public List<FailThresholdModel> Threshold { get; set; }
        public int Count { get; set; }
    }
}
