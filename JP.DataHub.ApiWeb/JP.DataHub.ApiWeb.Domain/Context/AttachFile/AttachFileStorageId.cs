using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    [MessagePackObject]
    internal record AttachFileStorageId : IValueObject
    {
        [Key(0)]
        public string Value { get; }

        public AttachFileStorageId(string value)
        {
            this.Value = value;
        }
    }
}
