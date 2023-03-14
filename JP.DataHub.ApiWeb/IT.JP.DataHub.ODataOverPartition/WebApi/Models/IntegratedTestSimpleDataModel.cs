using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ODataOverPartition.WebApi.Models
{
    public class IntegratedTestSimpleDataModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AreaUnitCode { get; set; }
        public string AreaUnitName { get; set; }
        public int ConversionSquareMeters { get; set; }
    }
}
