using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.ActionInjector
{
    class ValidateWithBlockchainActionInjector : ActionInjector
    {
        public override void Execute(Action action)
        {
            // notimplement
        }
    }
}
