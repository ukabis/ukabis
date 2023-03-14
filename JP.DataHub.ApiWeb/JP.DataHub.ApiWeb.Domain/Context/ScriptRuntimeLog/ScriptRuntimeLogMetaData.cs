using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog
{
    internal class ScriptRuntimeLogMetaData : IEntity
    {
        public ScriptRuntimeLogMetaData(Guid scriptRuntimeLogId, Guid? apiId, DateTime? execStartDate, int execDurationMsec, bool isError, DateTime regDate, Guid regUser)
        {
            ScriptRuntimeLogId = scriptRuntimeLogId;
            ApiId = apiId ?? throw new ArgumentNullException("ApiId");
            ExecStartDate = new ExecStartDate(execStartDate ?? throw new ArgumentNullException("ExecStartDate"));
            ExecDurationMsec = new ExecDurationMsec(execDurationMsec);
            IsError = new IsError(isError);
            RegDate = new RegDate(regDate);
            RegUserName = regUser;
        }
        public Guid ScriptRuntimeLogId { get; }
        public Guid ApiId { get; }
        public ExecStartDate ExecStartDate { get; }
        public ExecDurationMsec ExecDurationMsec { get; }
        public IsError IsError { get; }
        public RegDate RegDate { get; }
        public Guid RegUserName { get; }
    }
}
