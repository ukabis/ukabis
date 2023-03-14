using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    class SqlServerSqlParameterList : AbstractRdbmsSqlParameterList
    {
        public SqlServerSqlParameterList(string autoKeyPrefix = "p") : base(RepositoryType.SQLServer2, autoKeyPrefix)
        {
        }
    }
}
