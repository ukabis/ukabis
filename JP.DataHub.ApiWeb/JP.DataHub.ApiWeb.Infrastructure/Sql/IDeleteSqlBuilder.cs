using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    // .NET6
    internal interface IDeleteSqlBuilder : ISqlBuilder
    {
        bool HasPreAdditionalSqls { get; }
        IEnumerable<(string Sql, IRdbmsSqlParameterList SqlParameterList)> PreAdditionalSqls { get; }
    }
}
