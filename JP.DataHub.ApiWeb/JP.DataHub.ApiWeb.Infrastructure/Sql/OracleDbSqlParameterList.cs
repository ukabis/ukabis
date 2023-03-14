using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    class OracleDbSqlParameterList : AbstractRdbmsSqlParameterList
    {
        public OracleDbSqlParameterList(string autoKeyPrefix = "p") : base(RepositoryType.OracleDb, autoKeyPrefix)
        {
        }
    }
}
