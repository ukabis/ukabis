using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Aop;

namespace JP.DataHub.ApiWeb.Domain.ApiFilter
{
    // .NET6
    internal interface IFilterManager
    {
        List<IApiFilter> GetApiFilter(IApiFilterActionParam actionParam);
    }
}
