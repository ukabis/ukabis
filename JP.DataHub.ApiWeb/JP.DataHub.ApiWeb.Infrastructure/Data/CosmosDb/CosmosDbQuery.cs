using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.CosmosDb
{
    internal class CosmosDbQuery
    {
        public string Query { get; }

        public ReadOnlyDictionary<string, object> QueryParams { get; }

        public CosmosDbQuery(string query, Dictionary<string, object> queryParams)
        {
            Query = query;
            QueryParams = new ReadOnlyDictionary<string, object>(queryParams);
        }

        public new string ToString()
        {
            string param = QueryParams == null ? "null" : string.Join(",", QueryParams.Select(x => string.Format("{0}={1}", x.Key, x.Value)).ToList());
            return $"Query:{Query} Params:{param}";
        }
    }
}
