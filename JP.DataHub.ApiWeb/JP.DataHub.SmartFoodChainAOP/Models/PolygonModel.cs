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
    public class PolygonModel
    {
        public enum Type
        {
            Polygon
        }

        public string type { get; set; }

        public List<List<List<double>>> coordinates { get; set; }
    }
}
