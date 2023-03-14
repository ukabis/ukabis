using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Cosmos;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    public interface IJPDataHubCosmosDb
    {
        string ConnectionString { get; set; }

        string UpsertDocument(JToken token, ItemRequestOptions opt);

        IEnumerable<JToken> QueryDocument(string query, IDictionary<string, object> conditions, bool isOverPartition);
        List<JToken> QueryDocumentContinuation(string query, IDictionary<string, object> conditions, bool isOverPartition, int maxItemCount, string requestContinuation, out string responseContinuation);

        void DisposeDocumentClient();

        IEnumerable<string> DeleteDocument(string query, IDictionary<string, object> conditions, bool isOverPartition);

        bool DeleteDocument(JToken deltarget, JToken partitionKey);
    }
}
