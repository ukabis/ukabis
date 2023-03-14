using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.OData.SqlServerDb.ObsoleteODataToSqlTranslator
{
    /// <summary>
    /// ODataの予約語
    /// </summary>
    [Obsolete("互換性のために残している旧版です。")]
    public static class ODataKeywords
    {
        /// <summary><c>"asc"</c> keyword for expressions.</summary>
        public const string Ascending = "asc";

        /// <summary><c>"desc"</c> keyword for expressions.</summary>
        public const string Descending = "desc";

        /// <summary><c>"contains"</c> keyword for expressions.</summary>
        public const string Contains = "contains";

        /// <summary><c>"startswith"</c> keyword for expressions.</summary>
        public const string StartsWith = "startswith";

        /// <summary><c>"endswith"</c> keyword for expressions.</summary>
        public const string EndsWith = "endswith";

        /// <summary><c>"toupper"</c> keyword for expressions.</summary>
        public const string ToUpper = "toupper";

        /// <summary><c>"tolower"</c> keyword for expressions.</summary>
        public const string ToLower = "tolower";

        /// <summary><c>"length"</c> keyword for expressions.</summary>
        public const string Length = "length";

        /// <summary><c>"indexof"</c> keyword for expressions.</summary>
        public const string IndexOf = "indexof";

        /// <summary><c>"trim"</c> keyword for expressions.</summary>
        public const string Trim = "trim";

        /// <summary><c>"geo.distance"</c> keyword for expressions.</summary>
        public const string GeoDistance = "geo.distance";

        /// <summary><c>"geo.intersects"</c> keyword for expressions.</summary>
        public const string GeoIntersects = "geo.intersects";

        /// <summary>"null" keyword for expressions.</summary>
        public const string Null = "null";

        /// <summary>"not" keyword for expressions.</summary>
        public const string Not = "not";

        /// <summary>"max" keyword for expressions.</summary>
        public const string Max = "max";

        /// <summary>"NOT" keyword for search option.</summary>
        public const string SearchNot = "NOT";

        /// <summary>"AND" keyword for search option.</summary>
        public const string SearchAnd = "AND";

        /// <summary>"OR" keyword for search option.</summary>
        public const string SearchOr = "OR";
    }
}
