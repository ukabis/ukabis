using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.OData.OracleDb.ODataToSqlTranslator
{
    /// <summary>
    /// TranslateOptions
    /// </summary>
    public enum TranslateOptions
    {
        /// <summary>
        /// translate option for Sql SELECT clause
        /// </summary>
        SELECT_CLAUSE = 1,

        /// <summary>
        /// translate option for Sql WHERE clause
        /// </summary>
        WHERE_CLAUSE = 1 << 1,

        /// <summary>
        /// translate option for Sql ORDER BY clause
        /// </summary>
        ORDERBY_CLAUSE = 1 << 2,

        /// <summary>
        /// translate option for sql TOP clause
        /// </summary>
        TOP_CLAUSE = 1 << 3,

        /// <summary>
        /// translate option for all Sql clauses: SELECT, WHERE, ORDER BY, and TOP
        /// </summary>
        ALL = SELECT_CLAUSE | WHERE_CLAUSE | ORDERBY_CLAUSE | TOP_CLAUSE,

        /// <summary>
        /// Anyを含んでる
        /// </summary>
        COUNT_INCLUDE_ANY = 1 << 4
    }

    /// <summary>
    /// ODataToSqlTranslator
    /// </summary>
    public class ODataToSqlTranslator
    {
        /// <summary>
        /// QueryFormatter
        /// </summary>
        private IQueryFormatter QueryFormatter { get; }

        /// <summary>
        /// Visitor patterned ODataNodeToStringBuilder
        /// </summary>
        private ODataNodeToStringBuilder ODataNodeToStringBuilder;


        /// <summary>
        /// Constructor for ODataSqlTranslator
        /// </summary>
        /// <param name="queryFormatter">Optional QueryFormatter, if no formatter provided, a SQLQueryFormatter is used by default</param>
        public ODataToSqlTranslator(IQueryFormatter queryFormatter = null)
        {
            QueryFormatter = queryFormatter ?? new SQLQueryFormatter();
        }


        /// <summary>
        /// function that takes in an <see cref="ODataQueryOptions"/>, a string representing the type to filter, and a <see cref="FeedOptions"/>
        /// </summary>
        /// <returns>returns an SQL expression if successfully translated, otherwise a null string</returns>
        public string Translate(ODataQueryOptions odataQueryOptions, TranslateOptions translateOptions, string additionalWhereClause, out Dictionary<string, object> parameters)
        {
            parameters = new Dictionary<string, object>();
            ODataNodeToStringBuilder = new ODataNodeToStringBuilder(QueryFormatter, parameters);

            string selectClause, whereClause, orderbyClause, topClause;
            bool hasJoinClause = false;
            selectClause = whereClause = orderbyClause = topClause = string.Empty;

            // ORDER BY CLAUSE
            if ((translateOptions & TranslateOptions.ORDERBY_CLAUSE) == TranslateOptions.ORDERBY_CLAUSE)
            {
                orderbyClause = odataQueryOptions?.OrderBy?.OrderByClause == null
                    ? string.Empty
                    : $"{Constants.SQLOrderBySymbol} {this.TranslateOrderByClause(odataQueryOptions.OrderBy.OrderByClause)} ";
            }

            // WHERE CLAUSE
            if ((translateOptions & TranslateOptions.WHERE_CLAUSE) == TranslateOptions.WHERE_CLAUSE)
            {
                var customWhereClause = additionalWhereClause == null
                    ? string.Empty
                    : $"{additionalWhereClause}";
                var hasFilterClause = odataQueryOptions?.Filter?.FilterClause;
                Tuple<string, string> retVal = null;
                if (hasFilterClause != null)
                {
                    retVal = this.TranslateFilterClause(hasFilterClause);
                }
                whereClause = hasFilterClause == null
                    ? string.Empty
                    : retVal.Item2;
                whereClause = (!string.IsNullOrEmpty(customWhereClause) && !string.IsNullOrEmpty(whereClause))
                    ? $"({customWhereClause}) AND ({whereClause})"
                    : $"{customWhereClause}{whereClause}";
                whereClause = string.IsNullOrEmpty(whereClause)
                    ? string.Empty
                    : $"{Constants.SQLWhereSymbol} {whereClause}";
                if (retVal != null && !string.IsNullOrEmpty(retVal.Item1))
                {
                    hasJoinClause = true;
                    whereClause = string.Concat(retVal.Item1, " ", whereClause);
                }
            }
            // SELECT CLAUSE
            if ((translateOptions & TranslateOptions.SELECT_CLAUSE) == TranslateOptions.SELECT_CLAUSE)
            {
                // Count CLAUSE
                if (odataQueryOptions?.Count?.Value == true)
                {
                    selectClause = $"{Constants.SQLSelectSymbol} {Constants.SqlCountKeyWord} {Constants.SQLFromSymbol} {Constants.SQLTableNameSymbol} ";
                }
                else
                {
                    // TOP CLAUSE
                    if ((translateOptions & TranslateOptions.TOP_CLAUSE) == TranslateOptions.TOP_CLAUSE)
                    {
                        topClause = odataQueryOptions?.Top?.Value > 0
                            ? $"{Constants.SQLTopSymbol} {odataQueryOptions.Top.Value} "
                            : string.Empty;
                    }

                    var selectExpandClause = odataQueryOptions?.SelectExpand?.SelectExpandClause;
                    selectClause = selectExpandClause == null || selectExpandClause.SelectedItems.Count() == 0
                        ? hasJoinClause ? string.Concat(Constants.SymbolSpace, Constants.SqlValueKeyWord, Constants.SymbolSpace, Constants.SQLFieldNameSymbol) : Constants.SQLAsteriskSymbol
                        : string.Join(", ", selectExpandClause.SelectedItems.Select(c => string.Join(Constants.SymbolDot, ODataNodeToStringBuilder.QueryFormatter.TranslateFieldName(((PathSelectItem)c).SelectedPath.First().Identifier))));
                    if ((selectExpandClause == null || selectExpandClause.SelectedItems.Count() == 0) && hasJoinClause)
                    {
                        selectClause = $"{Constants.SQLSelectSymbol} {Constants.SqlDistinctKeyWord} {topClause}{selectClause} {Constants.SQLFromSymbol} {Constants.SQLTableNameSymbol} ";
                    }
                    else
                    {
                        selectClause = $"{Constants.SQLSelectSymbol} {topClause}{selectClause} {Constants.SQLFromSymbol} {Constants.SQLTableNameSymbol} ";
                    }
                }
            }

            var sb = new StringBuilder();
            sb.Append(selectClause);

            sb.Append(whereClause);
            var sp = default(string);
            if (!string.IsNullOrEmpty(whereClause))
            {
                sp = " ";
            }
            if (!string.IsNullOrEmpty(orderbyClause))
            {
                sb.Append(sp);
                sb.Append(orderbyClause);
            }

            // Count CLAUSE
            return sb.ToString();
        }

        /// <summary>Translates a <see cref="FilterClause"/> into a <see cref="FilterClause"/>.</summary>
        /// <param name="filterClause">The filter clause to translate.</param>
        /// <returns>The translated string.</returns>
        private Tuple<string, string> TranslateFilterClause(FilterClause filterClause)
        {
            var tmp = ODataNodeToStringBuilder.TranslateNode(filterClause.Expression, true);
            return new Tuple<string, string>(null, tmp);
        }

        /// <summary>Translates a <see cref="OrderByClause"/> into a <see cref="OrderByClause"/>.</summary>
        /// <param name="orderByClause">The orderBy clause to translate.</param>
        /// <param name="preExpr">expression built so far.</param>
        /// <returns>The translated string.</returns>
        private string TranslateOrderByClause(OrderByClause orderByClause, string preExpr = null)
        {
            string expr = string.Concat(ODataNodeToStringBuilder.TranslateNode(orderByClause.Expression), Constants.SymbolSpace, orderByClause.Direction == OrderByDirection.Ascending ? Constants.KeywordAscending.ToUpper() : Constants.KeywordDescending.ToUpper());

            expr = string.IsNullOrWhiteSpace(preExpr) ? expr : string.Concat(preExpr, Constants.SymbolComma, Constants.SymbolSpace, expr);

            if (orderByClause.ThenBy != null)
            {
                expr = this.TranslateOrderByClause(orderByClause.ThenBy, expr);
            }

            return expr;
        }
    }
}
