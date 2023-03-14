using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.Http
{
    public class AdditionalHttpResponseHeader : IAdditionalHttpResponseHeader
    {
        public string HttpMethod { get; set; }
        public string HttpStatusCode { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
