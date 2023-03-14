using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Dapper
{
    public class OracleBoolHandler : SqlMapper.TypeHandler<bool>
    {
        const int TRUE = 1;
        const int FALSE = 0;

        public override bool Parse(object value)
        {
            return value switch
            {
                short val when val != 0 => true,
                short val when val == 0 => false,
                int val when val != 0 => true,
                int val when val == 0 => false,
                long val when val != 0 => true,
                long val when val == 0 => false,
                double val when val != 0 => true,
                double val when val == 0 => false,
                decimal val when val != 0 => true,
                decimal val when val == 0 => false,
                _ => throw new InvalidCastException()
            };
        }

        public override void SetValue(IDbDataParameter parameter, bool value)
        {
            parameter.DbType = DbType.Int32;
            parameter.Value = value ? TRUE : FALSE;
        }
    }
}
