using System;

namespace JP.DataHub.ManageApi.Models.Document
{
    public class DocumentAgreementViewModel
    {
        public string AgreementId { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public DateTime UpdDate { get; set; }

        public bool IsActive { get; set; }
    }
}
