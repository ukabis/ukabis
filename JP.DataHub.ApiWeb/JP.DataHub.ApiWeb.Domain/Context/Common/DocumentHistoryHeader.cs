using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DocumentHistoryHeader : IValueObject
    {
        public bool isSelfHistory { get; }
        public string resourcePath { get; }
        public List<DocumentHistoryHeaderDocumentData> documents { get; }

        public DocumentHistoryHeader(bool isSelfHistory, string resourcePath, List<DocumentHistoryHeaderDocumentData> documents)
        {
            this.isSelfHistory = isSelfHistory;
            this.resourcePath = resourcePath;
            this.documents = documents;
        }

        public static bool operator ==(DocumentHistoryHeader me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DocumentHistoryHeader me, object other) => !me?.Equals(other) == true;
    }
}
