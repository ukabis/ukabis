using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.OData.SqlServerDb.ObsoleteODataToSqlTranslator
{
    /// <summary>
    /// SQLの予約語
    /// </summary>
    [Obsolete("互換性のために残している旧版です。")]
    internal static class SqlKeywords
    {
        /// <summary>Sql "SELECT" clause</summary>
        public const string Select = "SELECT";

        /// <summary>Sql "WHERE" clause</summary>
        public const string Where = "WHERE";

        /// <summary>Sql "TOP" clause</summary>
        public const string Top = "TOP";

        /// <summary>Sql "FROM" clause</summary>
        public const string From = "FROM";

        /// <summary>Sql JOIN clause</summary>
        public const string Join = "JOIN";

        /// <summary>SQL IN keyword</summary>
        public const string In = "IN";

        /// <summary>Sql "ORDER BY" clause</summary>
        public const string OrderBy = "ORDER BY";

        /// <summary>distinct keyword</summary>
        public const string Distinct = "DISTINCT";

        /// <summary>Count</summary>
        public const string Count = "COUNT(1)";

        /// <summary>Sql "UPPER" function</summary>
        public const string Upper = "UPPER";

        /// <summary>Sql "LOWER" function</summary>
        public const string Lower = "LOWER";

        /// <summary>Sql "LEN" function</summary>
        public const string Len = "LEN";

        /// <summary>Sql "INDEX_OF" function</summary>
        public const string IndexOf = "CHARINDEX";

        /// <summary>Sql "STDistance" function</summary>
        public const string StDistance = "STDistance";

        /// <summary>Sql "STIntersects" function</summary>
        public const string StIntersects = "STIntersects";

        /// <summary>Sql "*" wild card</summary>
        public const string Asterisk = "*";

        /// <summary>Sql TableName</summary>
        public const string TableName = "@tableName as";

        /// <summary>Sql <c>"c"</c> default filed name</summary>
        public const string FieldName = "c";

        /// <summary>Sql equal operator</summary>
        public const string Equal = "=";

        /// <summary>Sql not equal operator</summary>
        public const string NotEqual = "!=";

        /// <summary>Sql greater than operator</summary>
        public const string GreaterThan = ">";

        /// <summary>Sql greater than or equal operator</summary>
        public const string GreaterThanOrEqual = ">=";

        /// <summary>Sql less than operator</summary>
        public const string LessThan = "<";

        /// <summary>Sql less than or equal operator</summary>
        public const string LessThanOrEqual = "<=";

        /// <summary>Sql like operator</summary>
        public const string Like = "LIKE";

        /// <summary>Sql "AND" operator</summary>
        public const string And = "AND";

        /// <summary>Sql "OR" operator</summary>
        public const string Or = "OR";
    }
}
