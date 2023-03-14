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
    public class JasFreshnessJudgmentFailResult
    {
        public List<string> ObservedPropertiesCode { get; set; } = new List<string>();
        public string MeasurementUnitId { get; set; }
        public string DatastreamId { get; set; }
        public List<FailThresholdModel> Threshold { get; set; }
        public int Count { get; set; }
    }
}
