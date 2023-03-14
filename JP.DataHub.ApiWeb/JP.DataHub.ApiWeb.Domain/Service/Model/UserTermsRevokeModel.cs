using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class UserTermsRevokeModel
    {
        public string UserTermsId { get; set; }
        public string TermsGroupCode { get; set; }
        public string ResourceGroupId { get; set; }
    }
}
