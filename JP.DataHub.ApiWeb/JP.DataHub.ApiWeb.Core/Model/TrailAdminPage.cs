using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Core.Model
{
    public class TrailAdminPage
    {

        public TrailAdminPage(){}
        public TrailAdminPage(int total, List<TrailAdminViewRow> rows)
        {
            this.Total = total;
            this.Rows = rows;
        }

        public int Total { get; set; }
        public List<TrailAdminViewRow> Rows { get; set; }

    }

    public class TrailAdminViewRow
    {
        public Guid TrailId { get; set; }

        public string Screen { get; set; }

        public string TrailOperation { get; set; }

        public string ContollerClassName { get; set; }

        public string ActionMethodName { get; set; }

        public Guid? OpenId { get; set; }

        public string EmailAddress { get; set; }

        public Guid? VendorId { get; set; }

        public string IpAddress { get; set; }

        public string UserAgent { get; set; }

        public string Url { get; set; }

        public string HttpMethodType { get; set; }

        public int? HttpStatusCode { get; set; }

        public DateTime? RegDate { get; set; }

    }
}
