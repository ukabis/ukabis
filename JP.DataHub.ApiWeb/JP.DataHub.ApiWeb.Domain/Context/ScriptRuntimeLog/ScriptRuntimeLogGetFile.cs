using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog
{
    internal class ScriptRuntimeLogGetFile : IEntity
    {
        public ScriptRuntimeLogGetFile(string name, Guid? vendorId, Guid id, Stream content, string contentType)
        {
            Name = new FileName(name);
            ScriptRuntimeLogId = id;
            FilePath = new FilePath(Path.Combine((vendorId ?? throw new ArgumentNullException("vendorId")).ToString(), id.ToString()));
            Content = new StreamContent(content ?? Stream.Null);
            ContentType = new ContentType(contentType);
        }

        public FileName Name { get; }
        public FilePath FilePath { get; }
        public Guid ScriptRuntimeLogId { get; }
        public StreamContent Content { get; }
        public ContentType ContentType { get; }
        public string FilePathIncludeName() => Path.Combine(FilePath.Value, Name.Value);
    }
}
