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
    public class MeasurementUnitsModel
    {
        public string key { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string observationPropertyId { get; set; }
        public string Uri { get; set; }
        public List<MeasurementUnitsLangModel> lang { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class MeasurementUnitsLangModel
    {
        public string localeCode { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
