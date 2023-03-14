using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog
{
    internal class ScriptRuntimeLogAppendFile : IEntity
    {
        public ScriptRuntimeLogAppendFile(string name, Guid? vendorId, Guid id, string content, string contentType)
        {
            Name = new FileName(name);
            ScriptRuntimeLogId = id;
            FilePath = new FilePath(Path.Combine((vendorId ?? throw new ArgumentNullException("vendorId")).ToString(), id.ToString()));
            AppendContent = new AppendContents(content);
            ContentType = new ContentType(contentType);
        }

        public FileName Name { get; private set; }
        public FilePath FilePath { get; private set; }
        public Guid ScriptRuntimeLogId { get; private set; }
        public AppendContents AppendContent { get; private set; }
        public ContentType ContentType { get; private set; }
        public string FilePathIncludeName() => Path.Combine(FilePath.Value, Name.Value);
    }
}
