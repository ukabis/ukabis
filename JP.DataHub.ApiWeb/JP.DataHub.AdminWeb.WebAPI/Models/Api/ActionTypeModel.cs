using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ActionTypeModel
    {
        /// <summary>
        /// ActionTypeコード
        /// </summary>
        public string ActionTypeCd { get; set; }

        /// <summary>
        /// ActionType名
        /// </summary>
        public string ActionTypeName { get; set; }

        /// <summary>
        /// 表示するかどうか
        /// </summary>
        public bool IsVisible { get; set; }
    }
}
