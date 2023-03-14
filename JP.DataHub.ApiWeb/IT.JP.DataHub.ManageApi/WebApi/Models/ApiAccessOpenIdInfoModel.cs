using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models
{
    public class ApiAccessOpenIdInfoModel
    {
        /// <summary>
        /// ApiAccessOpenId
        /// </summary>
        public string ApiAccessOpenId { get; set; }

        /// <summary>
        /// ApiId
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// アクセスキー
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
