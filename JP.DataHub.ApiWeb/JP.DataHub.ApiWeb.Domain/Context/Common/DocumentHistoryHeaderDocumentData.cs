using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DocumentHistoryHeaderDocumentData : IValueObject
    {
        public string documentKey { get; }
        public string versionKey { get; }

        public DocumentHistoryHeaderDocumentData(string documentKey, string versionKey)
        {
            this.documentKey = documentKey;
            this.versionKey = versionKey;
        }

        public static bool operator ==(DocumentHistoryHeaderDocumentData me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DocumentHistoryHeaderDocumentData me, object other) => !me?.Equals(other) == true;
    }
}
