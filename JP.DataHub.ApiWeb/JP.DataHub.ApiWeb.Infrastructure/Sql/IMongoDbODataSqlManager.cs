
namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    internal interface IMongoDbODataSqlManager : IODataSqlManager
    {
        SeparatedQuerySyntax CreateSqlQueryEx(string queryString, string sqlWhere2);
    }
}
