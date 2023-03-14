using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class RoslynScriptRuntimeException : JPDataHubException
    {
        public RoslynScriptRuntimeException() : base()
        {
        }

        public RoslynScriptRuntimeException(string message) : base(message)
        {
        }

        public RoslynScriptRuntimeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RoslynScriptRuntimeException(string message, Exception innerException, string scriptRuntimeLogId) : base(message, innerException)
        {
            ScriptRuntimeLogId = scriptRuntimeLogId;
        }

        public string ScriptRuntimeLogId { get; internal set; }
    }
}