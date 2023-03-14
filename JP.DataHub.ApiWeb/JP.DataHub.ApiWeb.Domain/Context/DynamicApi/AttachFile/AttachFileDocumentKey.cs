using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile
{
    internal record AttachFileDocumentKey : DocumentKey, IValueObject
    {
        public string DocumentIdForAttachFile { get; }

        public AttachFileDocumentKey(RepositoryKey repositoryKey, string id, string documentIdForAttachFile) : base(repositoryKey, id)
        {
            this.DocumentIdForAttachFile = documentIdForAttachFile;
        }

        public static bool operator ==(AttachFileDocumentKey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(AttachFileDocumentKey me, object other) => !me?.Equals(other) == true;
    }
}
