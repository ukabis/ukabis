using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ApiResourceTagInfoModel
    {
        public string ControllerTagId { get; set; }

        public string ControllerId { get; set; }

        public string TagId { get; set; }

        public string TagName { get; set; }

        public bool IsActive { get; set; }
    }
}
