using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [DataContract]
    internal class DocumentDbResourceVersions
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "_Type")]
        public string Type { get; set; }

        [DataMember(Name = "_etag")]
        public string Etag { get; set; }

        [DataMember(Name = "_partitionkey")]
        public string Partitionkey { get; set; }

        [DataMember(Name = "_Reguser_Id")]
        public string RegUserId { get; set; }

        [DataMember(Name = "_Regdate")]
        public DateTime RegDate { get; set; }

        [DataMember(Name = "_Upduser_Id")]
        public string UpdUserId { get; set; }

        [DataMember(Name = "_Upddate")]
        public DateTime UpdDate { get; set; }

        [DataMember(Name = "documentversions")]
        public List<DocumentDbResourceVersion> DocumentVersions { get; set; }

        [DataMember(Name = "currentversion")]
        public int CurrentVersion { get; set; }
    }
}
