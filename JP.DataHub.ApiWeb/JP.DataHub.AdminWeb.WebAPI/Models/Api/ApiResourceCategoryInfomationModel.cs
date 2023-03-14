using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ControllerCategoryInfomationModel
    {
        public string ControllerCategoryId { get; set; }

        public string ControllerId { get; set; }

        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public bool IsActive { get; set; }

        /// <summary>
        /// このインスタンスの簡易コピーを取得する。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public ControllerCategoryInfomationModel Clone()
        {
            return (ControllerCategoryInfomationModel)MemberwiseClone();
        }
    }
}
