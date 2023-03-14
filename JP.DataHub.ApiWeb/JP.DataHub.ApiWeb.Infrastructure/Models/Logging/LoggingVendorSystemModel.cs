using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Logging
{
    public class LoggingVendorSystemModel
    {
        public string VendorId { get; set; }

        public string VendorName { get; set; }


        public IList<LoggingSystemModel> Systems { get; set; }



    }
    public class LoggingSystemModel
    {
        public string SystemId { get; set; }


        public string SystemName { get; set; }

    }

}
