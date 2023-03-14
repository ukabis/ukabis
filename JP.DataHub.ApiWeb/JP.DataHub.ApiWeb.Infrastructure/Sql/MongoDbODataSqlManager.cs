using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Newtonsoft.Json;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Infrastructure.OData;
using JP.DataHub.OData.MongoDb.ODataToSqlTranslator;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    internal class MongoDbODataSqlManager : ODataSqlManager, IMongoDbODataSqlManager
    {
        public override string CreateSqlQuery(QueryParam queryParam, string queryString, string sqlWhere2, int sqlMode, out int? topCount, out int? skipCount, out Dictionary<string, object> parameters)
        {
            var syntax = CreateSqlQueryEx(queryString, sqlWhere2);
            topCount = syntax.Top;
            skipCount = syntax.Skip;
            parameters = new Dictionary<string, object>();
            return JsonConvert.SerializeObject(syntax);
        }

        public SeparatedQuerySyntax CreateSqlQueryEx(string queryString, string sqlWhere2)
        {
            try
            {
                var uri = new Uri("https://localhost/?" + queryString);
                var oDataQueryOptions = SetupODataQueryOptions(uri);
                var oDataToSqlTranslator = new ODataToSqlTranslator(SetupSQLQueryFormatter());

                string where = oDataToSqlTranslator.Translate(oDataQueryOptions, TranslateOptions.WHERE_CLAUSE, sqlWhere2.ToString());
                string select = oDataToSqlTranslator.Translate(oDataQueryOptions, TranslateOptions.SELECT_CLAUSE);
                string orderby = oDataToSqlTranslator.Translate(oDataQueryOptions, TranslateOptions.ORDERBY_CLAUSE);
                int? top = oDataQueryOptions.Top?.Value;
                int? skip = oDataQueryOptions.Skip?.Value;

                return new SeparatedQuerySyntax(select, where, orderby, top, skip);
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
