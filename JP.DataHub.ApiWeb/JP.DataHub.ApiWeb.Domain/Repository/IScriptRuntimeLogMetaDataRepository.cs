using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface IScriptRuntimeLogMetaDataRepository
    {
        HttpResponseMessage Create(ScriptRuntimeLogMetaData data);
    }
}
