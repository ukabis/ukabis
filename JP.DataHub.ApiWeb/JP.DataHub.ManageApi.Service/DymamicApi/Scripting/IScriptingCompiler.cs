using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting
{
    internal interface IScriptingCompiler
    {
        string Compile<T>(string scriptCode, object parameters, bool isEnableCatchException = false);
    }
}