using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Service.Model
{
    public class ResourceGroupModel
    {
        public string ResourceGroupId { get; set; }
        public string ResourceGroupName { get; set; }
        public string TermsGroupCode { get; set; }
        public bool IsRequireConsent { get; set; }
        public IList<ResourceGroupInResourceModel> Resources { get; set; }
    }

    public class ResourceGroupInResourceModel
    {
        public string ControllerId { get; set; }
        public string ControllerUrl { get; set; }
        public bool IsPerson { get; set; }
    }
}
