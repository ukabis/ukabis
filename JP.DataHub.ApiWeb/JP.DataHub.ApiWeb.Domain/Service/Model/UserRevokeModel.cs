using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Service.Model
{
    public class UserRevokeModel
    {
        public string UserRevokeId { get; set; }
        public string UserTermsId { get; set; }
        public string TermsId { get; set; }
        public bool IsFinish { get; set; }
        public string OpenId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
