using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    // .NET6
    internal interface IQuerySqlBuilder : ISqlBuilder
    {
        string CountSql { get; }
        bool IsNativeCountQuery { get; }
        PagingInfo PagingInfo { get; }
        bool IsCustomSql { get; }

        string UserShareConditionString { get; }

        IRdbmsSqlParameterList PrepareQueryParameters(QueryParam param);
    }

    public class PagingInfo
    {
        public bool IsPaging { get; set; } = false;
        public long? OffsetCount { get; set; } = null;
        public long? FetchCount { get; set; } = null;

        public long ReadCount => (OffsetCount ?? 0) + (FetchCount ?? 0);
    }
}
