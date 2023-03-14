using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Infrastructure.Repository.Model
{
    internal class GetAgreementModel
    {
		public string agreement_id { get; set; }
		public string title { get; set; }
		public string detail { get; set; }
		public string vendor_id { get; set; }
    }
}
