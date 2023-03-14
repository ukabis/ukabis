using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Core.Model
{
    public class TrailAdminSearchConditionModel
    {
        public TrailAdminSearchConditionModel(DateTime start, DateTime end)
        {
            this.Start = start;
            this.End = end;
        }

        public TrailAdminSearchConditionModel(DateTime start, DateTime end, string openId, string emailAddress)
        {
            Start = start;
            End = end;
            OpenId = openId;
            EmailAddress = emailAddress;
        }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string OpenId { get; set; }

        public string EmailAddress { get; set; }

        public string VendorId { get; set; }

        public string VendorName { get; set; }

        public string Screens { get; set; }

        public string Status { get; set; }

        public string Slot { get; set; }
    }
}
