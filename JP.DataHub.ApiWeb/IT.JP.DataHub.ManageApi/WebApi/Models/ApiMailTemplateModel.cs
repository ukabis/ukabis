using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models
{
    [Serializable]
    public class ApiMailTemplateModel
    {
        public string ApiMailTemplateId { get; set; }
        public string ApiId { get; set; }
        public string VendorId { get; set; }
        public string MailTemplateId { get; set; }
        public string NotifyRegister { get; set; }
        public string NotifyUpdate { get; set; }
        public string NotifyDelete { get; set; }
    }

    [Serializable]
    public class RegisterApiMailTemplateModel
    {
        public string ApiId { get; set; }
        public string VendorId { get; set; }
        public string MailTemplateId { get; set; }
        public string NotifyRegister { get; set; }
        public string NotifyUpdate { get; set; }
        public string NotifyDelete { get; set; }
    }
}
