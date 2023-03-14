
namespace JP.DataHub.ApiWeb.Infrastructure.Models.ScriptRuntimeLog
{
    internal class ScriptRuntimeLogMetaDataModel
    {
        public Guid ScriptRuntimeLogId { get; set; }
        public Guid ApiId { get; set; }
        public DateTime ExecStartDate { get; set; }
        public int ExecDurationMsec { get; set; }
        public bool IsError { get; set; }
        public DateTime RegDate { get; set; }
        public Guid RegUserName { get; set; }
    }
}
