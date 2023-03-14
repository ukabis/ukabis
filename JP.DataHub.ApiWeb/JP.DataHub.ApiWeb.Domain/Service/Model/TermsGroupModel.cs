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
    public class TermsGroupModel
    {
        public string TermsGroupCode { get; set; }
        public string TermsGroupName { get; set; }
        public string ResourceGroupTypeCode { get; set; }
    }
}
