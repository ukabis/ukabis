using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    [DataContract]
    internal class VersionInfo
    {
        [DataMember(Name = "currentversion")]
        public int CurrentVersion { get; set; }
        [DataMember(Name = "_Upduser_Id")]
        public string UpdUserId { get; set; }
        [DataMember(Name = "_Upddate")]
        public DateTime UpdDate { get; set; }
        [DataMember(Name = "documentversions")]
        public List<DocumentVersion> DocumentVersions { get; set; }
    }
}
