using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.Scripting
{
    internal class DynamicApiDataContainer : IDynamicApiDataContainer
    {
        public IDynamicApiAction baseApiAction { get; set; }
    }
}
