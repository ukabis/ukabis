using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Query.Validator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Moq;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Infrastructure.OData;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    internal abstract class ODataSqlManager : IODataSqlManager
    {
        /// <summary>
        /// すべて取得
        /// </summary>
        public static readonly int ALL_CAUSE = 1;
        /// <summary>
        /// ページング
        /// </summary>
        public static readonly int TOP_EXCLUDE_CAUSE = 2;
        /// <summary>
        /// カウント取得
        /// </summary>
        public static readonly int COUNT = 3;
        /// <summary>
        /// カウント取得　Anyクエリを含む
        /// </summary>
        public static readonly int COUNT_INCLUDE_ANY = 4;

        public abstract string CreateSqlQuery(QueryParam queryParam, string queryString, string sqlWhere2, int sqlMode, out int? topCount, out int? skipCount, out Dictionary<string, object> prameters);
        public abstract int GetSqlMode(QueryParam queryParam);

        protected ODataQueryOptions SetupODataQueryOptions(Uri uri)
        {
            var context = CreateODataQueryContext();

            var collection = new ServiceCollection();
            collection.AddControllers().AddOData();
            collection.AddODataQueryFilter();
            collection.AddTransient<ODataUriResolver>();
            collection.AddTransient<ODataQueryValidator>();
            collection.AddTransient<TopQueryValidator>();
            collection.AddTransient<FilterQueryValidator>();
            collection.AddTransient<SkipQueryValidator>();
            collection.AddTransient<OrderByQueryValidator>();
            collection.AddLogging();

            var provider = collection.BuildServiceProvider();
            var routeBuilder = new RouteBuilder(Mock.Of<IApplicationBuilder>(x => x.ApplicationServices == provider));

            var httpContext = new DefaultHttpContext() { RequestServices = provider };
            httpContext.Request.Method = "GET";
            httpContext.Request.Host = new HostString(uri.Host, uri.Port);
            httpContext.Request.Path = uri.LocalPath;
            httpContext.Request.QueryString = new Microsoft.AspNetCore.Http.QueryString(uri.Query);

            var oDataQueryOptions = new ODataQueryOptions(context, httpContext.Request);
            if (oDataQueryOptions.Filter?.FilterClause?.Expression?.CheckODataFilter() ?? false)
            {
                var rfc7807 = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10426);
                rfc7807.Detail = string.Format(rfc7807.Detail);
                rfc7807.Errors = (dynamic)new Dictionary<string, string[]>() { { "ODataError", new string[] { ErrorCodeMessage.GetString(nameof(DynamicApiMessages.SyntaxErrorOfOData_InvalidFilterColumn)) } } };
                throw new Rfc7807Exception(rfc7807);
            }

            return oDataQueryOptions;
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
