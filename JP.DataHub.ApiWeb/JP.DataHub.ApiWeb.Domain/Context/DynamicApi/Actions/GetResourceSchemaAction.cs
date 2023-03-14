using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class GetResourceSchemaAction : AbstractDynamicApiAction, IGetResourceSchemaAction, IEntity
    {
        public override HttpResponseMessage ExecuteAction()
        {
            return TupleToHttpResponseMessage(new Tuple<HttpStatusCode, string>(HttpStatusCode.OK, ControllerSchema?.Value));
        }
    }
}
