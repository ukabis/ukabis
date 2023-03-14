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
    public class GetPrintableCountResultModel
    {
        /// <summary>
        /// 商品コード
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 印刷枚数（＝商品の個数）
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 印刷可能枚数
        /// </summary>
        public int PrintableCount { get; set; }

        /// <summary>
        /// 印刷済枚数
        /// </summary>
        public int PrintedCount { get; set; }
    }
}
