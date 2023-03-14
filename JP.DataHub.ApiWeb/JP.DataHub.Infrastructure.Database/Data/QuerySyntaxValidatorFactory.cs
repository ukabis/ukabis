using System.Text.RegularExpressions;
using MongoDB.Bson;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.Data.MongoDb;

namespace JP.DataHub.Infrastructure.Database.Data
{
    public class QuerySyntaxValidatorFactory : IQuerySyntaxValidatorFactory
    {
        public IQuerySyntaxValidator Create(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            if (IsMongoDbQuery(query))
            {
                return UnityCore.Resolve<IQuerySyntaxValidator>("mng");
            }

            return null;
        }


        /// <summary>
        /// MongoDBクエリかどうか
        /// </summary>
        private bool IsMongoDbQuery(string query)
        {
            // パラメータが含まれているとパースできないため置換
            var queryWithoutParameters = ReplaceParameters(query);
            return BsonDocument.TryParse(queryWithoutParameters, out _);
        }

        /// <summary>
        /// パラメータを置換
        /// </summary>
        private string ReplaceParameters(string targetString)
        {
            var pattern = @"{[-_0-9a-zA-Z]+}";
            new Regex(pattern)
                .Matches(targetString)
                .Cast<Match>()
                .ToList()
                .ForEach(x =>
                {
                    if (x.Value == $"{{{MongoDbConstants.CollectionNameVariable}}}")
                    {
                        targetString = targetString.Replace($"{{{MongoDbConstants.CollectionNameVariable}}}", $"\"{MongoDbConstants.CollectionNameVariable}\"");
                    }
                    else
                    {
                        targetString = targetString.Replace(x.Value, $"\"\"");
                    }
                });

            return targetString;
        }
    }
}
