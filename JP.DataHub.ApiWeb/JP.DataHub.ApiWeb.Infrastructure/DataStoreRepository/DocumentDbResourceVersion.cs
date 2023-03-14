using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [DataContract]
    internal class DocumentDbResourceVersion
    {
        [DataMember(Name = "version")]
        public int Version { get; set; }

        [DataMember(Name = "_Reguser_Id")]
        public string RegUserId { get; set; }

        [DataMember(Name = "_Regdate")]
        public DateTime RegDate { get; set; }

        [DataMember(Name = "is_current")]
        public bool IsCurrent { get; set; }
    }
}
