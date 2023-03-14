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
    public class TermsModel
    {
        public string TermsId { get; set; }
        public string VersionNo { get; set; }
        public DateTime FromDate { get; set; }
        public string TermsText { get; set; }
        public string TermsGroupCode { get; set; }
    }
}
