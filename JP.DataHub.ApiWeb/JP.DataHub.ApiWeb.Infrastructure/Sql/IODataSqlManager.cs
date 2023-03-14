using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    internal interface IODataSqlManager
    {
        string CreateSqlQuery(QueryParam queryParam, string queryString, string sqlWhere2, int sqlMode, out int? TopCount, out int? SkipCount, out Dictionary<string, object> parameters);
        int GetSqlMode(QueryParam queryParam);
    }
}
