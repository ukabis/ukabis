using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Context.Attributes
{
    internal enum ConvertType
    {
        None,
        HttpResponseDirect,
        ExeptionMsgToDetail,
        ExeptionMsgToDetailWithLog,
        AddInnerExeptionMessage,
        RoslynScriptRuntimeError,
    }
}
