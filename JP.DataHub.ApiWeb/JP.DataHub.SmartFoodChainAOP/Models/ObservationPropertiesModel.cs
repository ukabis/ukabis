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
    public class ObservationPropertiesModel
    {
        public string key { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string definition { get; set; }
        public List<ObservedPropertyLangModel> lang { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class ObservedPropertyLangModel
    {
        public string localeCode { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
