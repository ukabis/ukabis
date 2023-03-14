using JP.DataHub.Infrastructure.Database.Data.MongoDb;
using MongoDB.Bson.Serialization.Attributes;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    internal class SeparatedQuerySyntax
    {
        public string Select { get; set; }
        public string Where { get; set; }
        public string OrderBy { get; set; }
        public int? Top { get; set; }
        public int? Skip { get; set; }

        public string Aggregate { get; set; }


        [BsonIgnoreIfDefault]
        public bool IsAggregation => !string.IsNullOrWhiteSpace(Aggregate);


        public SeparatedQuerySyntax(string select, string where, string orderby, int? top, int? skip, string aggregate = null)
        {
            Select = select;
            Where = where;
            OrderBy = orderby;
            Top = top;
            Skip = skip;
            Aggregate = aggregate;
        }


        public bool IsCountQuery()
        {
            if (string.IsNullOrWhiteSpace(Select))
            {
                return false;
            }

            var bson = Select.ToDecimalizedBsonDocument();
            return bson?.Contains("$count") ?? false;
        }
    }
}
