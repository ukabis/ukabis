using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    // .NET6
    internal static class DataStoreParamFactory
    {
        public static QueryParam CreateQueryParam(AbstractDynamicApiAction action, IPerRequestDataContainer perRequestDataContainer, HasSingleData hasSingleData, ApiQuery query = null)
        {
            return ValueObjectUtil.Create<QueryParam>(query, action, hasSingleData, new XResourceSharingWith(perRequestDataContainer.XResourceSharingWith),
                new XResourceSharingPerson(perRequestDataContainer.XResourceSharingPerson));
        }

        public static DeleteParam CreateDeleteParam(AbstractDynamicApiAction action, IPerRequestDataContainer perRequestDataContainer, Action<JToken> callbackDelete)
        {
            return ValueObjectUtil.Create<DeleteParam>(action, new XResourceSharingWith(perRequestDataContainer.XResourceSharingWith),
                new XResourceSharingPerson(perRequestDataContainer.XResourceSharingPerson), callbackDelete);
        }
    }
}
