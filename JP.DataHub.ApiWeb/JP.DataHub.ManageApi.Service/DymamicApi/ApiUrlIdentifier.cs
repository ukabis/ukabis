using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi
{
    public class ApiUrlIdentifier
    {
        public string ControllerId { get; set; }
        public string ApiId { get; set; }
        public string ActionType { get; set; }
        public string MethodType { get; set; }
        public string ControllerUrl { get; set; }
        public string ApiUrl { get; set; }
    }
}
