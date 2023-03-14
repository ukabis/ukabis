using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface IDynamicGatewayRepository
    {
        GatewayResponse Put(RequestGatewayUrl url, GatewayInfo gatewayInfo, Contents contents, bool isCache = false);
        GatewayResponse Delete(RequestGatewayUrl url, GatewayInfo gatewayInfo, Contents contents, bool isCache = false);
        GatewayResponse Get(RequestGatewayUrl url, GatewayInfo gatewayInfo, Contents contents, bool isCache = false);
        GatewayResponse Post(RequestGatewayUrl url, GatewayInfo gatewayInfo, Contents contents, bool isCache = false);
        GatewayResponse Patch(RequestGatewayUrl url, GatewayInfo gatewayInfo, Contents contents, bool isCache = false);

        string CreateCacheKey(IGatewayAction action);
    }
}
