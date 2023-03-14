using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class CropFreshnessManagementModel
    {
        public string CropFreshnessManagementId { get; set; }
        public string CropCode { get; set; }
        public string BreedCode { get; set; }
        public string BrandCode { get; set; }
        public List<MeasurementDetailModel> MeasurementDetail { get; set; }
        public string JudgmentSyntax { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Explanation { get; set; }
    }

    public class MeasurementDetailModel
    {
        public string[] ObservedPropertiesCode { get; set; }
        public List<FailThresholdModel> FailThreshold { get; set; }
    }

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
