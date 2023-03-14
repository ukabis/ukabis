using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.RFC7807;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class AuthenticateJwtTokenResult
    {
        public bool IsSuccess { get; }

        public string VendorId { get; }

        public string SystemId { get; }

        public AuthenticateJwtTokenResult(string vendorId, string systemId)
        {
            IsSuccess = true;
            VendorId = vendorId;
            SystemId = systemId;
        }
    }
}
