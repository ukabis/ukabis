using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DocumentHistoryReferenceHeader : IValueObject
    {
        public bool? reference { get; }
        public List<DocumentHistoryReferenceHeaderDocumentData> refhistinfo { get; }

        public DocumentHistoryReferenceHeader(bool reference, List<DocumentHistoryReferenceHeaderDocumentData> refhistinfo)
        {
            this.reference = reference;
            this.refhistinfo = refhistinfo;
        }

        public static bool operator ==(DocumentHistoryReferenceHeader me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DocumentHistoryReferenceHeader me, object other) => !me?.Equals(other) == true;
    }
}