using MongoDB.Driver;

namespace JP.DataHub.Infrastructure.Database.Data.MongoDb
{
    public interface IProfiledMongoDbClient<TDocument>
    {
        string CollectionName { get; }

        TProjection FindOneAndReplace<TProjection>(FilterDefinition<TDocument> filter, TDocument replacement, FindOneAndReplaceOptions<TDocument, TProjection> options = null);
        IAsyncCursor<TResult> Aggregate<TResult>(PipelineDefinition<TDocument, TResult> pipeline, AggregateOptions options = null);
        DeleteResult DeleteMany(FilterDefinition<TDocument> filter);
        string CreateIndex(CreateIndexModel<TDocument> model, CreateOneIndexOptions options = null);
    }
}
