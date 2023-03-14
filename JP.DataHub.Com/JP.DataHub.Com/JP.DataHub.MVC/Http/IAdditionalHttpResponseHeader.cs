using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.Http
{
    public interface IAdditionalHttpResponseHeader
    {
        string HttpMethod { get; set; }
        string HttpStatusCode { get; set; }
        string Name { get; set; }
        string Value { get; set; }
    }
}
