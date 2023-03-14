using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Document
{
    public class DocumentAgreementModel
    {
        public string AgreementId { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public DateTime UpdDate { get; set; }
        public bool IsActive { get; set; }
    }
}
