using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models
{
    [Serializable]
    public class AgreementModel
    {
        public string AgreementId { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public string VendorId { get; set; }
    }
    public class AgreementRegisterResponseModel
    {
        public string AgreementId { get; set; }
    }
}
