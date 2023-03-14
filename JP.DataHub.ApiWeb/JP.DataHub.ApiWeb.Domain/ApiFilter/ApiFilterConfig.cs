using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.ApiFilter
{
    internal class ApiFilterConfig
    {
        public int Level { get; set; }
        public string ResourceUrl { get; set; }
        public string ApiUrl { get; set; }
        public string HttpMethod { get; set; }
        public string Action { get; set; }
        public string VendorId { get; set; }
        public string SystemId { get; set; }
        public string RequestVendorId { get; set; }
        public string RequestSystemId { get; set; }
        public string Param { get; set; }
        public string Assembly { get; set; }
        public string Type { get; set; }
        public int Seq { get; set; }
    }
}
