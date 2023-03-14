﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.UriParser;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.OData.CosmosDb.ODataToSqlTranslator
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
        COUNT_INCLUDE_ANY = 1 << 4,

    }

    /// <summary>
    /// ODataToSqlTranslator
    /// </summary>
    public class ODataToSqlTranslator
    {
        /// <summary>
        /// function that takes in an <see cref="ODataQueryOptions"/>, a string representing the type to filter, and a <see cref="FeedOptions"/>
        /// </summary>
        /// <param name="odataQueryOptions"></param>
        /// <param name="translateOptions"></param>
        /// <param name="additionalWhereClause"></param>
        /// <returns>returns an SQL expression if successfully translated, otherwise a null string</returns>
        public string Translate(ODataQueryOptions odataQueryOptions, TranslateOptions translateOptions, string additionalWhereClause = null)
        {
            //TODO: refactor to use a StringBuilder
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
                if (odataQueryOptions?.Count?.Value == true && (translateOptions & TranslateOptions.COUNT_INCLUDE_ANY) != TranslateOptions.COUNT_INCLUDE_ANY)
                {
                    selectClause = $"{Constants.SQLSelectSymbol} {Constants.SqlValueKeyWord} {Constants.SqlCountKeyWord} {Constants.SQLFromSymbol} {Constants.SQLFieldNameSymbol} ";
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
                        : string.Join(", ", selectExpandClause.SelectedItems.Select(c =>
                        {
                            var conditionKey = string.Join(Constants.SymbolDot, ((PathSelectItem)c).SelectedPath.Select(p => p.Identifier)).Trim();

                            // 予約語の場合はエスケープする
                            if (Constants.SelectEscapeKeywords.Contains(conditionKey.ToUpper()))
                            {
                                return string.Concat(Constants.SQLFieldNameSymbol, Constants.SymbolBeginEscapeKeyword, conditionKey, Constants.SymbolEndEscapeKeyword);
                            }
                            else
                            {
                                return string.Concat(Constants.SQLFieldNameSymbol, Constants.SymbolDot, conditionKey);
                            }
                        }));
                    if ((selectExpandClause == null || selectExpandClause.SelectedItems.Count() == 0) && hasJoinClause)
                    {
                        selectClause = $"{Constants.SQLSelectSymbol} {Constants.SqlDistinctKeyWord} {topClause}{selectClause} {Constants.SQLFromSymbol} {Constants.SQLFieldNameSymbol} ";
                    }
                    else
                    {
                        selectClause = $"{Constants.SQLSelectSymbol} {topClause}{selectClause} {Constants.SQLFromSymbol} {Constants.SQLFieldNameSymbol} ";
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
            if (odataQueryOptions?.Count?.Value == true && (translateOptions & TranslateOptions.COUNT_INCLUDE_ANY) == TranslateOptions.COUNT_INCLUDE_ANY)
                return $"{Constants.SQLSelectSymbol} {Constants.SqlValueKeyWord} {Constants.SqlCountKeyWord} {Constants.SQLFromSymbol} {Constants.SymbolOpenParen}{sb}{Constants.SymbolClosedParen}";
            else return sb.ToString();
        }

        /// <summary>
        /// Constructor for ODataSqlTranslator
        /// </summary>
        /// <param name="queryFormatter">Optional QueryFormatter, if no formatter provided, a SQLQueryFormatter is used by default</param>
        public ODataToSqlTranslator(IQueryFormatter queryFormatter = null)
        {
            queryFormatter = queryFormatter ?? new SQLQueryFormatter();
            oDataNodeToStringBuilder = new ODataNodeToStringBuilder(queryFormatter);
        }

        /// <summary>
        /// 
        /// </summary>
        private ODataToSqlTranslator() { }

        /// <summary>Translates a <see cref="FilterClause"/> into a <see cref="FilterClause"/>.</summary>
        /// <param name="filterClause">The filter clause to translate.</param>
        /// <returns>The translated string.</returns>
        private Tuple<string, string> TranslateFilterClause(FilterClause filterClause)
        {
            // リテラル内のDelimiter('|')は'||'にエスケープされているためここで退避と復元を行う。
            var placeHolder = $"{{{Guid.NewGuid()}}}";
            var tmp = oDataNodeToStringBuilder.TranslateNode(filterClause.Expression).Replace(Constants.Delimiter + Constants.Delimiter, placeHolder);

            if (!string.IsNullOrEmpty(tmp) && tmp.IndexOf(Constants.Delimiter) >= 0)
            {
                var splited = tmp.Split(new[] { Constants.Delimiter[0] }, options: StringSplitOptions.RemoveEmptyEntries);
                var sbJoin = new StringBuilder();
                var sbWhere = new StringBuilder();
                var sp = "";

                foreach (var a in splited)
                {
                    if (a.StartsWith(Constants.SQLJoinSymbol))
                    {
                        sbJoin.Append(sp);
                        sbJoin.Append(a.Replace(placeHolder, Constants.Delimiter));
                        if (sp.Length == 0)
                        {
                            sp = " ";
                        }
                    }
                    else
                    {
                        sbWhere.Append(a.Replace(placeHolder, Constants.Delimiter));
                    }
                }
                return new Tuple<string, string>(sbJoin.ToString(), sbWhere.ToString());
            }
            return new Tuple<string, string>(null, tmp.Replace(placeHolder, Constants.Delimiter));
        }

        /// <summary>Translates a <see cref="OrderByClause"/> into a <see cref="OrderByClause"/>.</summary>
        /// <param name="orderByClause">The orderBy clause to translate.</param>
        /// <param name="preExpr">expression built so far.</param>
        /// <returns>The translated string.</returns>
        private string TranslateOrderByClause(OrderByClause orderByClause, string preExpr = null)
        {
            string expr = string.Concat(oDataNodeToStringBuilder.TranslateNode(orderByClause.Expression), Constants.SymbolSpace, orderByClause.Direction == OrderByDirection.Ascending ? Constants.KeywordAscending.ToUpper() : Constants.KeywordDescending.ToUpper());

            expr = string.IsNullOrWhiteSpace(preExpr) ? expr : string.Concat(preExpr, Constants.SymbolComma, Constants.SymbolSpace, expr);

            if (orderByClause.ThenBy != null)
            {
                expr = this.TranslateOrderByClause(orderByClause.ThenBy, expr);
            }

            return expr;
        }

        /// <summary>
        /// Visitor patterned ODataNodeToStringBuilder
        /// </summary>
        private readonly ODataNodeToStringBuilder oDataNodeToStringBuilder;
    }
}
