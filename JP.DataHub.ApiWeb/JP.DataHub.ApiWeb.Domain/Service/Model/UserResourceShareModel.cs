using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class UserResourceShareModel
    {
        public string UserResourceGroupId { get; set; }
        public string OpenId { get; set; }
        public string ResourceGroupId { get; set; }
        public string UserShareTypeCode { get; set; }
        public string UserGroupId { get; set; }

    }
}
