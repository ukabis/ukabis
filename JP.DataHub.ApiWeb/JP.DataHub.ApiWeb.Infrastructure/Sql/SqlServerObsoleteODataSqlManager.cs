using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Routing;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.OData.SqlServerDb.ObsoleteODataToSqlTranslator;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    // .NET6
    /// <summary>
    /// SqlServer用のODataSqlManagerです。
    /// </summary>
    [Obsolete("互換性のために残している旧版です。")]
    internal class SqlServerObsoleteODataSqlManager : ODataSqlManager, IODataSqlManager
    {
        /// <summary>
        /// ODataクエリーからSqlServerのSQL文を生成します。
        /// </summary>
        /// <param name="queryParam">クエリパラメータ</param>
        /// <param name="queryString">ODataクエリー</param>
        /// <param name="sqlWhere2">追加のWhere句</param>
        /// <param name="sqlMode">SqlMode</param>
        /// <param name="topCount">Top件数</param>
        /// <param name="skipCount">Skip件数</param>
        /// <param name="parameters">クエリ変数</param>
        /// <returns>SqlServerのSQL文</returns>
        public override string CreateSqlQuery(QueryParam queryParam, string queryString, string sqlWhere2, int sqlMode, out int? topCount, out int? skipCount, out Dictionary<string, object> parameters)
        {
            topCount = 0;
            skipCount = 0;
            parameters = new Dictionary<string, object>();
            try
            {
                var uri = new Uri("https://localhost/?" + queryString);
                var oDataQueryOptions = SetupODataQueryOptions(uri);
                topCount = oDataQueryOptions.Top?.Value;
                skipCount = oDataQueryOptions.Skip?.Value;

                // Translator作成
                var translator = new ODataToSqlTranslator(SetupSQLQueryFormatter());
                TranslateOptions translateOptions = (sqlMode == TOP_EXCLUDE_CAUSE) ? TranslateOptions.ALL & (~TranslateOptions.TOP_CLAUSE) : TranslateOptions.ALL;
                translateOptions = (sqlMode == COUNT_INCLUDE_ANY) ? translateOptions | TranslateOptions.COUNT_INCLUDE_ANY : translateOptions;

                // SQL文を生成
                return translator.Translate(oDataQueryOptions, translateOptions, sqlWhere2);
            }
            catch (Microsoft.OData.ODataException ex)
            {
                throw new ODataException(ex.Message, ex);
            }
        }

        public override int GetSqlMode(QueryParam queryParam)
        {
            int sqlMode = ODataSqlManager.ALL_CAUSE;
            if (queryParam.XRequestContinuation != null && queryParam.XRequestContinuation.ContinuationString != null)
            {
                sqlMode = ODataSqlManager.TOP_EXCLUDE_CAUSE;
            }
            if (queryParam.ODataQuery != null)
            {
                if (queryParam.ODataQuery.HasCountQuery && !queryParam.ODataQuery.HasAnyQuery)
                {
                    sqlMode = ODataSqlManager.COUNT;
                }
                else if (queryParam.ODataQuery.HasCountQuery && queryParam.ODataQuery.HasAnyQuery)
                {
                    sqlMode = ODataSqlManager.COUNT_INCLUDE_ANY;
                }
            }
            return sqlMode;
        }

        private SQLQueryFormatter SetupSQLQueryFormatter()
            => new SQLQueryFormatter();
    }
}
