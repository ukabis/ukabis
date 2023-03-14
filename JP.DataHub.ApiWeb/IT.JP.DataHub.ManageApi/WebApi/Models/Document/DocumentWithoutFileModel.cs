using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Document
{
    public class DocumentWithoutFileModel
    {
        public string DocumentId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string CategoryName { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public bool IsEnable { get; set; }
        public bool IsAdminCheck { get; set; }
        public bool IsAdminStop { get; set; }
        public string AgreementId { get; set; }
        public DateTime UpdDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublicPortal { get; set; }
        public bool IsPublicAdmin { get; set; }
        public bool IsPublicPortalHidden { get; set; }
        public bool IsPublicAdminHidden { get; set; }
        public string Password { get; set; }
    }
    public class DocumentWithoutFileViewModel
    {
        public string DocumentId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string CategoryName { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public bool IsEnable { get; set; }
        public bool IsAdminCheck { get; set; }
        public bool IsAdminStop { get; set; }
        public string AgreementId { get; set; }
        public DateTime UpdDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPublicPortal { get; set; }
        public bool IsPublicAdmin { get; set; }
        public bool IsPublicPortalHidden { get; set; }
        public bool IsPublicAdminHidden { get; set; }
        public string Password { get; set; }
    }

}
