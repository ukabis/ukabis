using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.Revoke.Models
{
    public class ResourceGroupModel
    {
        public string ResourceGroupId { get; set; }
        public string ResourceGroupName { get; set; }
        public string TermsGroupCode { get; set; }
        public bool IsRequireConsent { get; set; }
        public IList<ResourceGroupInResourceMode> Resources { get; set; }
    }

    public class ResourceGroupInResourceMode
    {
        public string ControllerId { get; set; }
        public string ControllerUrl { get; set; }
        public bool IsPerson { get; set; }

    }
}
