using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public  class ApiResourceFieldInfoModel
    {
        public string ControllerFieldId { get; set; }

        public string ControllerId { get; set; }

        public string FieldId { get; set; }

        public string FieldName { get; set; }

        public bool IsActive { get; set; }
    }
}
