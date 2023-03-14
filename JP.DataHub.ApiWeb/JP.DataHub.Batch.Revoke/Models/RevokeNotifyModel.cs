using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.Revoke.Models
{
    public  class RevokeNotifyModel
    {
        public string OpenId { get; set; }

        public string UserTermsId { get; set; }

        public string ResourceGroupId { get; set; }
    }
}
