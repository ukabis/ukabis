using System.Diagnostics;
using MongoDB.Driver;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Transaction;

namespace JP.DataHub.Infrastructure.Database.Data.MongoDb
{
    public class ProfiledMongoDbClient<TDocument> : IProfiledMongoDbClient<TDocument>
    {
        private const string ENDPOINT = "endpoint";
        private const string DATABASE = "database";
        private const string COLLECTION = "collection";

        private static bool OutputSqlLog = false; // ConfigurationHelper.Get<bool>("OutputSqlLog", false); // NIY

        public string CollectionName { get; }

        private MongoClient MongoClient;
        private IMongoDatabase Database;
        private IMongoCollection<TDocument> Collection;
        private JPDataHubLogger Logger = new JPDataHubLogger(typeof(ProfiledMongoDbClient<TDocument>));


        public ProfiledMongoDbClient(string connectionString)
        {
            var pcs = new ParseConnectionString(connectionString);
            MongoClient = new MongoClient(pcs[ENDPOINT]);
            Database = MongoClient.GetDatabase(pcs[DATABASE]);
            Collection = Database.GetCollection<TDocument>(pcs[COLLECTION]);
            CollectionName = pcs[COLLECTION];
        }

        public TProjection FindOneAndReplace<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null)
        {
            var message = OutputSqlLog ? $"Replace = {replacement}" : null;
            return Execute(() => Collection.FindOneAndReplace(filter, replacement, options), message);
        }

        public IAsyncCursor<TResult> Aggregate<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null)
        {
            var message = OutputSqlLog ? $"Aggregate = {pipeline}" : null;
            return Execute(() => Collection.Aggregate(pipeline, options), message);
        }

        public DeleteResult DeleteMany(FilterDefinition<TDocument> filter)
        {
            var message = OutputSqlLog ? $"DeleteMany = {filter.Render(Collection.DocumentSerializer, Collection.Settings.SerializerRegistry)}" : null;
            return Execute(() => Collection.DeleteMany(filter), message);
        }

        public string CreateIndex(CreateIndexModel<TDocument> model, CreateOneIndexOptions options = null)
        {
            var message = OutputSqlLog ? $"CreateIndex = {model.Keys.Render(Collection.DocumentSerializer, Collection.Settings.SerializerRegistry)}" : null;
            return Execute(() => Collection.Indexes.CreateOne(model, options), message);
        }


        private T Execute<T>(Func<T> execute, string message = null)
        {
            if (OutputSqlLog)
            {
                var stopwatch = Stopwatch.StartNew();
                var result = execute();
                stopwatch.Stop();

                Logger.Info($"{message} ExecuteTime={stopwatch.Elapsed}");
                return result;
            }
            else
            {
                return execute();
            }
        }
    }
}
