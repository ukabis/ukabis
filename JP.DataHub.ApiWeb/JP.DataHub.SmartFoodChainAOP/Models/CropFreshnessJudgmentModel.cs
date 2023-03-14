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
    public class CropFreshnessJudgmentModel
    {
        public string CropFreshnessManagementId { get; set; }
        public string CropCode { get; set; }
        public string BreedCode { get; set; }
        public string BrandCode { get; set; }
        public List<MeasurementDetailModel> MeasurementDetail { get; set; }
        public string JudgmentSyntax { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class MeasurementDetailModel
    {
        public List<string> ObservedPropertiesCode { get; set; }
        public List<FailThresholdModel> FailThreshold { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class FailThresholdModel
    {
        public int ThresholdValue { get; set; }
        public string Operator { get; set; }
        public int? ThresholdLessTimes { get; set; }
        public int? ThresholdGreatTimes { get; set; }
        public int? ThresholdGreatTotalMinute { get; set; }
        public int? ThresholdLessTotalMinute { get; set; }
    }
}
