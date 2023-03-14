using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal class AttachFileListElement : IValueObject
    {
        public FileId FileId { get; }
        public FileName FileName { get; }
        public string RegisterDateTime { get; }
        public string RegisterUserId { get; }

        public AttachFileListElement(FileId fileId, FileName fileName, string registerDateTime, string registerUserId)
        {
            FileId = fileId;
            FileName = fileName;
            RegisterDateTime = registerDateTime;
            RegisterUserId = registerUserId;
        }
    }
}
