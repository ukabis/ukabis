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
    public class UserTermsModel
    {
        public string UserTermsId { get; set; }
        public string OpenId { get; set; }
        public string TermsId { get; set; }
        public DateTime AgreementDate { get; set; }
        public DateTime RevokeDate { get; set; }
    }
}
