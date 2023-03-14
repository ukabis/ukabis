using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Resources;
using JP.DataHub.Com.RFC7807;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record VendorSystemAuthenticationResult : IValueObject
    {
        public bool IsSuccess { get; }

        public string VendorId { get; }

        public string SystemId { get; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public string ErrorCode { get; set; }

        public Uri Instance { get; }

        public HttpStatusCode Status { get; }

        public VendorSystemAuthenticationResult(RFC7807ProblemDetailExtendErrors rFC7807ProblemDetailCode = null)
        {
            this.Title = rFC7807ProblemDetailCode?.Title;
            this.Status = (HttpStatusCode)rFC7807ProblemDetailCode?.Status;
            this.Detail = rFC7807ProblemDetailCode?.Detail;
            this.Instance = rFC7807ProblemDetailCode?.Instance;
            this.ErrorCode = rFC7807ProblemDetailCode?.ErrorCode;
            this.VendorId = null;
            this.SystemId = null;
        }

        public VendorSystemAuthenticationResult(VendorId vendorId, SystemId systemId)
        {
            IsSuccess = true;
            VendorId = vendorId.Value;
            SystemId = systemId.Value;
        }

        public static bool operator ==(VendorSystemAuthenticationResult me, object other) => me?.Equals(other) == true;

        public static bool operator !=(VendorSystemAuthenticationResult me, object other) => !me?.Equals(other) == true;
    }
}
