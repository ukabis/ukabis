using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.TrailEventProcess.Models
{
    public class TrailAdmin
    {
        public string TrailId { get; }

        public string Screen { get; }

        public string TrailOperation { get; }

        public string ContollerClassName { get; }

        public string ActionMethodName { get; }

        public string OpenId { get; }

        public string VendorId { get; }

        public string IpAddress { get; }

        public string UserAgent { get; }

        public string Url { get; }

        public string HttpMethodType { get; }

        public int? HttpStatusCode { get; }

        public object MethodParameter { get; }

        public object MethodResult { get; }

        public DateTime? RegDate { get; }

        public TrailAdmin(string trailId, string screen, string trailOperation, string contollerClassName, string actionMethodName,
                          string openId, string vendorId, string ipAddress, string userAgent, string url, string httpMethodType,
                          int? httpStatusCode, object methodParameter, object methodResult, DateTime? regDate = null)
        {
            TrailId = trailId;
            Screen = screen;
            TrailOperation = trailOperation;
            ContollerClassName = contollerClassName;
            ActionMethodName = actionMethodName;
            OpenId = openId;
            VendorId = vendorId;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            Url = url;
            HttpMethodType = httpMethodType;
            HttpStatusCode = httpStatusCode;
            MethodParameter = methodParameter;
            MethodResult = methodResult;
            RegDate = regDate;
        }
    }
}