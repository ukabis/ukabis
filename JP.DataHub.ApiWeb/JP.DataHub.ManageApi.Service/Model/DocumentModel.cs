using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class DocumentModel
    {
        public string DocumentId { get; set; }

        public string VendorId { get; set; }

        public string SystemId { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public string CategoryId { get; set; }

        public string AgreementId { get; set; }

        public bool IsEnable { get; set; }

        public bool IsAdminCheck { get; set; }

        public bool IsAdminStop { get; set; }

        public DateTime? LastUpdDate { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsPublicPortal { get; set; }

        public bool IsPublicAdmin { get; set; }

        public bool IsPublicPortalHidden { get; set; }

        public bool IsPublicAdminHidden { get; set; }

        public string Password { get; set; }
    }
}
