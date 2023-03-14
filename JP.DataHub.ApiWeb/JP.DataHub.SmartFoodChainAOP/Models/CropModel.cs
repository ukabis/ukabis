using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Models
{
    [MessagePackObject(true)]
    [Serializable]
    public class CropModel
    {
        public string CropCode { get; set; }
        public string CropName { get; set; }
        public List<CropModel> DownLevelCrop { get; set; }
        public List<CropLang> CropLang { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class CropLang
    {
        public string LocaleCode { get; set; }
        public string CropName { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class CropResultModel
    {
        public string CropCode { get; set; }
        public string CropName { get; set; }
        public List<CropLang> CropLang { get; set; }
    }
}