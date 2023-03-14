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
using JP.DataHub.OData.CosmosDb.ODataToSqlTranslator;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    // .NET6
    internal class CosmosDBODataSqlManager : ODataSqlManager, IODataSqlManager
    {
        public override string CreateSqlQuery(QueryParam queryParam, string queryString, string sqlWhere2, int sqlMode, out int? topCount, out int? skipCount, out Dictionary<string, object> parameters)
        {
            topCount = 0;
            skipCount = 0;
            parameters = new Dictionary<string, object>();
            try
            {
                var uri = new Uri("https://localhost/?" + queryString);
                var oDataQueryOptions = SetupODataQueryOptions(uri);
                var oDataToSqlTranslator = new ODataToSqlTranslator(SetupSQLQueryFormatter());
                /*
                var sqlQuery = oDataToSqlTranslator.Translate(oDataQueryOptions, TranslateOptions.SELECT_CLAUSE | TranslateOptions.TOP_CLAUSE);
                var sqlWhere = oDataToSqlTranslator.Translate(oDataQueryOptions, TranslateOptions.WHERE_CLAUSE);
                var sqlOrderby = oDataToSqlTranslator.Translate(oDataQueryOptions, TranslateOptions.ORDERBY_CLAUSE);
                */

                TranslateOptions translateOptions = (sqlMode == TOP_EXCLUDE_CAUSE) ? TranslateOptions.ALL & (~TranslateOptions.TOP_CLAUSE) : TranslateOptions.ALL;
                translateOptions = (sqlMode == COUNT_INCLUDE_ANY) ? translateOptions | TranslateOptions.COUNT_INCLUDE_ANY : translateOptions;
                string sqlQuery = oDataToSqlTranslator.Translate(oDataQueryOptions, translateOptions, sqlWhere2.ToString());

                topCount = oDataQueryOptions.Top?.Value;
                return sqlQuery;
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

        private ODataQueryContext CreateODataQueryContext()
        {
            var entityType = new EdmEntityType("ns", "ns", null, false, true);
            var edmModel = new EdmModel();
            edmModel.AddElement(entityType);
            return new ODataQueryContext(edmModel, entityType, new ODataPath());
        }
    }
}
