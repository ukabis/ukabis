using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class VendorOpenIdCaModel : OpenIdCaModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid? VendorId { get; set; }
    }
}
