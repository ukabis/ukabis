using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class ControllerCategoryInfomationViewModel
    {
        public string ControllerCategoryId { get; set; }

        public string ControllerId { get; set; }

        public string CategoryId { get; set; }

        public string CategoryName { get; set; }

        public bool IsActive { get; set; }
    }
}
