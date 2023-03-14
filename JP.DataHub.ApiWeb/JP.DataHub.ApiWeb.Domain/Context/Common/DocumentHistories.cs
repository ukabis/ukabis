using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    internal record DocumentHistories : IValueObject
    {
        public string Id { get; }

        public string Type { get; }

        public string Etag { get; }

        public string Partitionkey { get; }

        public string RegUserId { get; }

        public string RegDate { get; }

        public string UpdUserId { get; }

        public string UpdDate { get; }

        /// <summary>
        /// 添付ファイルの履歴用で通常のドキュメントのIDが入る
        /// (添付ファイルの履歴ドキュメントにはIDにFileIdが入るため、こちらにドキュメントIDを格納。ベンダー領域超えのチェックに利用)
        /// </summary>
        public string DocumentIdForAttachFile { get; }

        public List<DocumentHistory> DocumentVersions { get; }

        public DocumentHistories()
        {
            DocumentVersions = new List<DocumentHistory>();
        }

        public DocumentHistories(List<DocumentHistory> documentVersions)
        {
            DocumentVersions = documentVersions;
        }
        public DocumentHistory Latest { get { return DocumentVersions.OrderByDescending(x => x.VersionNo).FirstOrDefault(); } }

        public DocumentHistories(string id, string type, string etag, string partitionkey, string regUserId, string regDate, string updUserId, string updDate, List<DocumentHistory> documentVersions, string documentIdForAttachFile)
        {
            Id = id;
            Type = type;
            Etag = etag;
            Partitionkey = partitionkey;
            RegUserId = regUserId;
            RegDate = regDate;
            UpdUserId = updUserId;
            UpdDate = updDate;
            DocumentVersions = documentVersions;
            DocumentIdForAttachFile = documentIdForAttachFile;
        }

        public static bool operator ==(DocumentHistories me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DocumentHistories me, object other) => !me?.Equals(other) == true;
    }
}