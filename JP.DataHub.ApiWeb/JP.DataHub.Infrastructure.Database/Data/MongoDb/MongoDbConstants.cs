using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Database.Data.MongoDb
{
    public static class MongoDbConstants
    {
        /// <summary>
        /// コレクション名に置換されるAPIクエリ用変数
        /// </summary>
        public const string CollectionNameVariable = "COLLECTION_NAME";

        /// <summary>
        /// パイプラインの使用禁止ステージ
        /// </summary>
        public static readonly ReadOnlyCollection<string> UnusableMongoDbAggregationStages = Array.AsReadOnly<string>(
            new string[]
            {
                "$currentOp",
                "$facet",
                "$indexStats",
                "$listLocalSessions",
                "$listSessions",
                "$planCacheStats",
                "$redact",
                "$graphLookup",
                "$lookup",
                "$merge",
                "$out"
            });

        /// <summary>
        /// クエリの使用禁止オペレータ
        /// </summary>
        public static readonly ReadOnlyCollection<string> UnusableMongoDbOperators = Array.AsReadOnly<string>(
            new string[]
            {
                "$where"
            });
    }
}
