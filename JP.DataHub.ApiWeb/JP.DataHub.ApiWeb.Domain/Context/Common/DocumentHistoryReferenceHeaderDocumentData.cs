using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DocumentHistoryReferenceHeaderDocumentData : IValueObject
    {
        public string resourcePath { get; }
        public string versionKey { get; }
        public string documentKey { get; }

        public DocumentHistoryReferenceHeaderDocumentData(string resourcePath, string versionKey, string documentKey)
        {
            this.resourcePath = resourcePath;
            this.versionKey = versionKey;
            this.documentKey = documentKey;
        }

        public static bool operator ==(DocumentHistoryReferenceHeaderDocumentData me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DocumentHistoryReferenceHeaderDocumentData me, object other) => !me?.Equals(other) == true;
    }
}
