using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Data.Oracle
{
    public class OracleGuidHandler : SqlMapper.TypeHandler<Guid>
    {
        public override Guid Parse(object value)
        {
            if (value is Guid)
            {
                return (Guid)value;
            }
            return new Guid((string)value);
        }

        public override void SetValue(IDbDataParameter parameter, Guid value)
        {
            parameter.DbType = DbType.String;
            parameter.Value = value.ToString().ToLowerInvariant();
        }
    }
}
