using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.CosmosDb
{
    internal class CosmosDbQueryItemCount
    {
        public int Count { get; }

        public CosmosDbQueryItemCount(int count)
        {
            Count = count;
        }
    }
}
