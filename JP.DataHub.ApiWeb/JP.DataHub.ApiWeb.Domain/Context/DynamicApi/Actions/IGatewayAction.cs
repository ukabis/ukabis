using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [Log]
    internal interface IGatewayAction : IDynamicApiAction
    {
        GatewayInfo GatewayInfo { get; set; }
    }
}
