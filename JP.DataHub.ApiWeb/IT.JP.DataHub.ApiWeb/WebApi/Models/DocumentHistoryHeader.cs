using System.Collections.Generic;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    class DocumentHistoryHeader
    {
        public string resourcePath { get; }
        public List<DocumentHistoryHeaderDocumentData> documents { get; }

        public DocumentHistoryHeader(string resourcePath, List<DocumentHistoryHeaderDocumentData> documents)
        {
            this.resourcePath = resourcePath;
            this.documents = documents;
        }
    }

    class DocumentHistoryHeaderDocumentData
    {
        public string documentKey { get; }
        public string versionKey { get; }

        public DocumentHistoryHeaderDocumentData(string documentKey, string versionKey)
        {
            this.documentKey = documentKey;
            this.versionKey = versionKey;
        }
    }
}
