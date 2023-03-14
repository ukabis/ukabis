using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Interface.Model
{
    public class VendorSystemAuthenticationAccessTokenResultModel
    {
        public bool IsSuccess { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public string ErrorCode { get; set; }

        public Uri Instance { get; set; }

        public HttpStatusCode Status { get; set; }

        public string VendorId { get; set; }

        public string SystemId { get; set; }
    }
}
