using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace JP.DataHub.Data.Oracle
{
    public class OracleDatetimeHandler : SqlMapper.TypeHandler<DateTime?>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime? value)
        {
            parameter.DbType = DbType.DateTime;
            parameter.Value = value;
        }

        public override DateTime? Parse(object value)
        {
            return (value == null) ? null : DateTime.Parse(value.ToString());
        }
    }
}
