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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using JP.DataHub.Com.Consts;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.OData.Interface;
using JP.DataHub.OData.SqlServerDb.ODataToSqlTranslator;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    // .NET6
    /// <summary>
    /// SqlServerのOData変換
    /// </summary>
    internal class SqlServerODataSqlManager : ODataSqlManager, IODataSqlManager
    {
        private readonly Dictionary<string, JSchema> ManagementFieldSchemas = new Dictionary<string, JSchema>()
        {
            { JsonPropertyConst.ID, JSchema.Parse("{ 'type': 'string' }") },
            { JsonPropertyConst.VERSION_COLNAME, JSchema.Parse("{ 'type': 'number' }") },
            { JsonPropertyConst.VENDORID, JSchema.Parse("{ 'type': 'string' }") },
            { JsonPropertyConst.SYSTEMID, JSchema.Parse("{ 'type': 'string' }") },
            { JsonPropertyConst.OWNERID, JSchema.Parse("{ 'type': 'string' }") },
            { JsonPropertyConst.REGDATE, JSchema.Parse("{ 'type': 'string', 'format': 'date-time' }") },
            { JsonPropertyConst.OPENID, JSchema.Parse("{ 'type': 'string' }") },
            { JsonPropertyConst.UPDDATE, JSchema.Parse("{ 'type': 'string', 'format': 'date-time' }") },
            { JsonPropertyConst.UPDUSERID, JSchema.Parse("{ 'type': 'string' }") }
        };


        public override string CreateSqlQuery(QueryParam queryParam, string queryString, string sqlWhere2, int sqlMode, out int? topCount, out int? skipCount, out Dictionary<string, object> parameters)
        {
            topCount = 0;
            skipCount = 0;
            try
            {
                var uri = new Uri("https://localhost/?" + queryString);
                var oDataQueryOptions = SetupODataQueryOptions(uri);

                var oDataToSqlTranslator = new ODataToSqlTranslator(SetupSQLQueryFormatter(queryParam));
                var translateOptions = TranslateOptions.ALL & (~TranslateOptions.TOP_CLAUSE);
                string sqlQuery = oDataToSqlTranslator.Translate(oDataQueryOptions, translateOptions, sqlWhere2, out parameters);

                topCount = oDataQueryOptions.Top?.Value;
                skipCount = oDataQueryOptions.Skip?.Value;
                return sqlQuery;
            }
            catch (Microsoft.OData.ODataException ex)
            {
                throw new ODataException(ex.Message, ex);
            }
        }

        public override int GetSqlMode(QueryParam queryParam)
        {
            // OFFSET/FETCHを利用するためTOPは使用しない
            int sqlMode = TOP_EXCLUDE_CAUSE;

            return sqlMode;
        }

        private SQLQueryFormatter SetupSQLQueryFormatter(QueryParam queryParam)
        {
            IODataFilterValidator validator = null;
            if (queryParam?.ControllerSchema?.Value != null)
            {
                // 管理項目をJSONスキーマに追加
                var schema = JSchema.Parse(queryParam.ControllerSchema.Value);
                foreach (var property in ManagementFieldSchemas)
                {
                    schema.Properties.Add(property.Key, property.Value);
                }
                validator = new SqlServerODataFilterValidator(schema);
            }
            return new SQLQueryFormatter(validator);
        }

        private ODataQueryContext CreateODataQueryContext()
        {
            var entityType = new EdmEntityType("ns", "ns", null, false, true);
            var edmModel = new EdmModel();
            edmModel.AddElement(entityType);
            return new ODataQueryContext(edmModel, entityType, new ODataPath());
        }
    }
}
