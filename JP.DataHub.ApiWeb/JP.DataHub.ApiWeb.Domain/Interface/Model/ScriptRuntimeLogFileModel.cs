using System;
using System.Net.Http;

namespace JP.DataHub.ApiWeb.Domain.Interface.Model
{
    public class ScriptRuntimeLogFileModel
    {
        public string Name { get; private set; }
        public string FilePath { get; private set; }
        public Guid ScriptRuntimeLogId { get; private set; }
        public string ContentType { get; private set; }
        public StreamContent Content { get; private set; }
    }
}
