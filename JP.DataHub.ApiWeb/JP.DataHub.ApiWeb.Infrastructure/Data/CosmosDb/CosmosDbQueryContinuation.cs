using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.CosmosDb
{
    internal class CosmosDbQueryContinuation
    {
        public string ContinuationString { get; }

        public CosmosDbQueryContinuation(string continuationString)
        {
            // 空文字は不正なトークンと見做されるためnullに変換
            ContinuationString = string.IsNullOrEmpty(continuationString) ? null : continuationString;
        }
    }
}
