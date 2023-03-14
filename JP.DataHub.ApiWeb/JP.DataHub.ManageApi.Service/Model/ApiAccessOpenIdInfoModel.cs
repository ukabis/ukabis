using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class ApiAccessOpenIdInfoModel
    {
        public string ApiAccessOpenId { get; set; }

        public string ApiId { get; set; }

        public string AccessKey { get; set; }

        public string OpenId { get; set; }

        public bool IsEnable { get; set; } = true;
    }
}
