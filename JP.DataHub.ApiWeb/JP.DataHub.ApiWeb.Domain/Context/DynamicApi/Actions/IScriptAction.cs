using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal interface IScriptAction : IDynamicApiAction
    {
        Script Script { get; set; }

        ScriptTypeVO ScriptType { get; set; }

        VendorId ProviderVendorId { get; set; }
    }
}
