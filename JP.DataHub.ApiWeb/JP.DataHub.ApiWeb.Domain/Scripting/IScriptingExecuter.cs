using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Scripting
{
    internal interface IScriptingExecuter
    {
        T ExecuteScript<T>(object parameters, string scriptCode = null, bool isEnableCatchException = false);

        string Compile<T>(string scriptCode, object parameters, bool isEnableCatchException = false);
    }
}