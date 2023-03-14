using Newtonsoft.Json.Linq;
using MongoDB.Bson;

namespace JP.DataHub.Infrastructure.Database.Data.MongoDb
{
    public interface IJPDataHubMongoDb
    {
        string ConnectionString { get; set; }
        string CollectionName { get; }

        string UpsertDocument(JToken json);
        long DeleteDocument(string query);
        IEnumerable<JToken> QueryDocument(IEnumerable<BsonDocument> pipelineBsonArray);
        IEnumerable<Tuple<JToken, string>> QueryDocumentContinuation(string requestContinuation, int top, IEnumerable<BsonDocument> pipelineBsonArray);
        JToken CountDocument(IEnumerable<BsonDocument> pipelineBsonArray);
        void CreateWildcardIndex();
    }
}
