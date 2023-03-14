using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    // .NET6
    [DataContract]
    internal class DocumentDbDocumentVersions
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
        public List<DocumentDbDocumentVersion> DocumentVersions { get; set; } = new List<DocumentDbDocumentVersion>();

        /// <summary>
        /// 添付ファイルの履歴用で通常のドキュメントのIDが入る
        /// (添付ファイルの履歴ドキュメントにはIDにFileIdが入るため、こちらにドキュメントIDを格納。ベンダー領域超えのチェックに利用)
        /// </summary>
        [DataMember(Name = "DocumentIdForAttachFile")]
        public string DocumentIdForAttachFile { get; set; }

    }
}
