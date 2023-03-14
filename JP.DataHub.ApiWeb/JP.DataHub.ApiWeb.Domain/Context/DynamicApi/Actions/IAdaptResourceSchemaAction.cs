using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal interface IAdaptResourceSchemaAction : IDynamicApiAction
    {
        ReadOnlyCollection<RepositoryInfo> RepositoryInfoList { get; set; }
    }
}
