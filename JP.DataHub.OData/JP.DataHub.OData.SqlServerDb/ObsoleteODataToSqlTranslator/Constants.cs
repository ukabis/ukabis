using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.OData.SqlServerDb.ObsoleteODataToSqlTranslator
{
    /// <summary>
    /// Provides constant string/char values
    /// </summary>
    [Obsolete("互換性のために残している旧版です。")]
    public static class Constants
    {
        /// <summary>used e.g. to parse and recompose a SQL string containing a JOIN clause</summary>
        public const string Delimiter = "|";

        /// <summary>'.' constant to represent the dot.</summary>
        public const string Dot = ".";

        /// <summary>":" keyword for expression.</summary>
        public const string Colon = ":";

        /// <summary>'=' constant to represent an assignment in name=value</summary>
        public const char Equal = '=';

        /// <summary>'(' constant to represent an open parenthesis</summary>
        public const char OpenParen = '(';

        /// <summary>')' constant to represent an closed parenthesis</summary>
        public const char ClosedParen = ')';

        /// <summary>'\'' constant to represent a single quote as prefix/suffix for literals</summary>
        public const char SingleQuote = '\'';

        /// <summary>' ' constant to represent a single white space for literals</summary>
        public const char Space = ' ';

        /// <summary>',' constant to represent a single comma for literals</summary>
        public const char Comma = ',';

        /// <summary>'-' constant to represent an negate unary operator.</summary>
        public const string Negate = "-";
    }
}
